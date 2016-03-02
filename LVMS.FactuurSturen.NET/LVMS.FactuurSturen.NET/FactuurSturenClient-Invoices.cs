using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using LVMS.FactuurSturen.Exceptions;
using LVMS.FactuurSturen.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PortableRest;

namespace LVMS.FactuurSturen
{
    public partial class FactuurSturenClient
    {
        private readonly List<Invoice> _cachedInvoices = new List<Invoice>();
        private const string ResourceInvoices = "invoices";

        /// <summary>
        /// Returns a list of sent invoices.
        /// </summary>
        /// <param name="allowCache"></param>
        /// <returns></returns>
        public async Task<Invoice[]> GetInvoices(bool? allowCache = true)
        {
            return await GetInvoicesInternal(ResourceInvoices, allowCache, _cachedInvoices);
        }

        private async Task<Invoice[]> GetInvoicesInternal(string resource, bool? allowCache, List<Invoice> cacheList)
        {
            if (!allowCache.HasValue)
                allowCache = _allowResponseCaching;
            if ((bool) allowCache && cacheList != null && cacheList.Count > 0)
                return cacheList.ToArray();

            var request = new RestRequest(resource, HttpMethod.Get, ContentTypes.Json);

            var result = await _httpClient.ExecuteWithPolicyAsync<Invoice[]>(this, request);

            if ((bool) allowCache)
                cacheList?.AddRange(result);
            return result;
        }

        /// <summary>
        /// Returns a list of sent invoices. Filter by an invoice filter.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<Invoice[]> GetInvoicesWithFilter(InvoiceFilters filter)
        {
            var request = new RestRequest(ResourceInvoices, HttpMethod.Get, ContentTypes.Json);
            request.AddQueryString("filter", Enum.GetName(typeof(InvoiceFilters), filter).ToLowerInvariant());

            var result = await _httpClient.ExecuteWithPolicyAsync<Invoice[]>(this, request);
            return result;
        }

        /// <summary>
        /// Return a specific sent invoice.
        /// </summary>
        /// <param name="invoiceNr">Invoice number</param>
        /// <param name="allowCache">Whether or not to look it up/store it in cache</param>
        /// <returns></returns>
        public async Task<Invoice> GetInvoice(string invoiceNr, bool? allowCache = true)
        {
            return await GetInvoiceInternal(ResourceInvoices, invoiceNr, allowCache, _cachedInvoices);
        }

        /// <summary>
        /// Returns the downloaded PDF version of an invoice.
        /// </summary>
        /// <param name="invoiceNr">Invoice number</param>
        /// <returns></returns>
        public async Task<byte[]> GetInvoicePdf(string invoiceNr)
        {
            var request = new RestRequest($"invoices_pdf/{invoiceNr}", HttpMethod.Get, ContentTypes.ByteArray);

            return await _httpClient.ExecuteWithPolicyAsync<byte[]>(this, request);
        }

        private async Task<Invoice> GetInvoiceInternal(string resource, string invoiceNr, bool? allowCache, List<Invoice> cacheList)
        {
            if (!allowCache.HasValue)
                allowCache = _allowResponseCaching;

            if ((bool) allowCache && _cachedInvoices != null && _cachedInvoices.Any(p => p.InvoiceNr == invoiceNr))
                return _cachedInvoices.FirstOrDefault(p => p.InvoiceNr == invoiceNr);

            var request = new RestRequest($"{resource}/{invoiceNr}", HttpMethod.Get, ContentTypes.Json);

            var json = await _httpClient.ExecuteWithPolicyAsync<string>(this, request);

            // FactuurSturen.nl doesn't just return the Invoice. They return JSON with a root element
            // and inside is the Invoice data, so a trick is needed here.
            var jsonObject = JObject.Parse(json);
            var root = jsonObject?.First;
            if (root == null) return null;
            var invoiceElement = root.First;
            var result = JsonConvert.DeserializeObject<Invoice>(invoiceElement.ToString());
            if (result != null && (bool) allowCache)
            {
                StoreInvoiceInCache(result, cacheList);
            }
            return result;
        }

        /// <summary>
        /// Creates an invoice. Do not call this method if you want to create a draft invoice.
        /// </summary>
        /// <param name="invoice">The invoice to create</param>
        /// <param name="returnReloadedInvoice">Whether or not to fetch updated invoice data</param>
        /// <param name="storeReloadedVersionInCache"></param>
        /// <returns>An updated Invoice record (retrieved from the server) when returnReloadedInvoice is True, else Null. </returns>
        public async Task<Invoice> CreateInvoice(Invoice invoice, bool returnReloadedInvoice, bool? storeReloadedVersionInCache = true)
        {
            var request = new RestRequest($"{ResourceInvoices}/", HttpMethod.Post, ContentTypes.Json)
            {
                ContentType = ContentTypes.Json
            };

            if (invoice.Action == InvoiceActions.Save)
                throw new FactuurSturenValidationLibException("The Action cannot be 'Save' when calling this method. Call CreateDraftInvoice instead.");

            ValidateInvoice(invoice);

            request.AddParameter(invoice);
            var response = await _httpClient.SendWithPolicyAsync<string>(this, request);
            if (response.HttpResponseMessage.IsSuccessStatusCode)
            {
                invoice.InvoiceNr = response.Content;
                if (returnReloadedInvoice)
                {
                    invoice = await GetInvoice(response.Content, storeReloadedVersionInCache);
                    return invoice;
                }
                else
                {
                    return null;
                }
            }
            throw new RequestFailedLibException(response.HttpResponseMessage.StatusCode);
        }

        /// <summary>
        /// Creates an invoice and save is as a draft. This invoice cannot be retrieved via GetInvoices and cannot
        /// be send later on. To send a draft invoice, you must go to the web application.
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns></returns>
        public async Task CreateDraftInvoice(Invoice invoice)
        {
            var request = new RestRequest($"{ResourceInvoices}/", HttpMethod.Post, ContentTypes.Json)
            {
                ContentType = ContentTypes.Json
            };

            if (invoice.Action != InvoiceActions.Save)
                throw new FactuurSturenValidationLibException("The Action must be set to Save when calling this method.");

            ValidateInvoice(invoice);

            request.AddParameter(invoice);
            var response = await _httpClient.SendWithPolicyAsync<string>(this, request);
            if (!response.HttpResponseMessage.IsSuccessStatusCode)
            {
                throw new RequestFailedLibException(response.HttpResponseMessage.StatusCode);
            }
        }

        private static void ValidateInvoice(Invoice invoice)
        {
            if (invoice.Action == InvoiceActions.None)
                throw new FactuurSturenValidationLibException("When creating an invoice, the action must be set.");
            if (invoice.Action == InvoiceActions.Send && invoice.SendMethod == SendMethods.None)
                throw new FactuurSturenValidationLibException("When the invoice action is Send, the SendMethod must be set.");
            if ((invoice.Action == InvoiceActions.Save || invoice.Action == InvoiceActions.Repeat) &&
                string.IsNullOrWhiteSpace(invoice.SaveName))
                throw new FactuurSturenValidationLibException("When the action is 'save' or 'repeat' you must supply a SaveName.");
            if (invoice.Action == InvoiceActions.Repeat)
            {
                if (!invoice.InitialDate.HasValue)
                    throw new FactuurSturenValidationLibException(
                        "Because the action is 'repeat', InitialDate must be set. Is the date when the first invoice must be sent");
                if (!invoice.FinalSendDate.HasValue)
                    throw new FactuurSturenValidationLibException(
                        "Because the action is 'repeat', FinalSendDate must be set. Is the date when the last invoice must be sent. After this date the recurring invoice entry is deleted");
                if (invoice.Frequency == Frequencies.None)
                    throw new FactuurSturenValidationLibException(
                        "Because the action is 'repeat', the Frequency must be set. Is the frequency when the invoice must be sent. Based on the InitialDate.");
                if (invoice.RepeatType == RepeatTypes.None)
                    throw new FactuurSturenValidationLibException(
                        "Because the action is 'repeat', the RepeatType must be set. Set if the recurring invoice is automatically sent by our system");
            }
        }

        public async Task DeleteInvoice(Invoice invoice)
        {
            if (invoice == null)
                throw new ArgumentNullException(nameof(invoice));
            var invoiceNr = invoice.InvoiceNr;
            await DeleteInvoice(invoiceNr);
        }

        public async Task DeleteInvoice(string invoiceNr)
        {
            await DeleteInvoiceInternal(ResourceInvoices, invoiceNr, _cachedInvoices);
        }

        private async Task DeleteInvoiceInternal(string resource, string invoiceNr, List<Invoice> cacheList)
        {
            var request = new RestRequest($"{resource}/{invoiceNr}", HttpMethod.Delete, ContentTypes.Json)
            {
                ContentType = ContentTypes.Json,
            };

            var response = await _httpClient.SendWithPolicyAsync<string>(this, request);
            if (!response.HttpResponseMessage.IsSuccessStatusCode)
            {
                throw new RequestFailedLibException(response.HttpResponseMessage.StatusCode);
            }

            RemoveInvoiceFromCache(invoiceNr, cacheList);
        }

        private void StoreInvoiceInCache(Invoice invoice, ICollection<Invoice> cacheList)
        {
            cacheList?.Add(invoice);
        }

        private void RemoveInvoiceFromCache(string invoiceId, List<Invoice> cacheList)
        {
            if (cacheList == null)
                return;
            lock (cacheList)
            {
                if (cacheList.Any(p => p.Id == invoiceId))
                    cacheList.Remove(cacheList.First(p => p.Id == invoiceId));
            }
        }
    }
}
