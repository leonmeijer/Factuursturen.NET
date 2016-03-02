using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LVMS.FactuurSturen.Exceptions;
using LVMS.FactuurSturen.Model;
using PortableRest;

namespace LVMS.FactuurSturen
{
    public enum InvoiceFilters
    {
        /// <summary>
        /// Get open invoice (all invoices that are not fully paid yet)
        /// </summary>
        Open,
        /// <summary>
        /// Get overdue invoices
        /// </summary>
        Overdue,
        /// <summary>
        /// Get sent invoices
        /// </summary>
        Sent,
        /// <summary>
        /// Get party paid invoices
        /// </summary>
        Partly,
        /// <summary>
        /// Get invoices with too much paid
        /// </summary>
        TooMuch,
        /// <summary>
        /// Get paid invoices
        /// </summary>
        Paid,
        /// <summary>
        /// Get invoices that couldn't be collected
        /// </summary>
        Uncollectible
    }

    public partial class FactuurSturenClient
    {
        private List<Invoice> _cachedInvoices;

        public async Task<Invoice[]> GetInvoices(bool allowCache = true)
        {
            if (allowCache && _cachedInvoices != null)
                return _cachedInvoices.ToArray();

            var request = new RestRequest("invoices", HttpMethod.Get, ContentTypes.Json);

            var result = await _httpClient.ExecuteWithPolicyAsync<Invoice[]>(this, request);

            if (allowCache || _cachedInvoices != null)
                _cachedInvoices = new List<Invoice>(result);
            return result;
        }

        public async Task<Invoice[]> GetInvoicesWithFilter(InvoiceFilters filter)
        {

            var request = new RestRequest("invoices", HttpMethod.Get, ContentTypes.Json);
            request.AddQueryString("filter", Enum.GetName(typeof(InvoiceFilters), filter).ToLowerInvariant());

            var result = await _httpClient.ExecuteWithPolicyAsync<Invoice[]>(this, request);
            return result;
        }

        public async Task<Invoice> GetInvoice(string invoiceNr, bool allowCache = true)
        {
            if (allowCache && _cachedInvoices != null && _cachedInvoices.Any(p=>p.Id == invoiceNr))
                return _cachedInvoices.FirstOrDefault(p=>p.Id == invoiceNr);

            var request = new RestRequest($"invoices/{invoiceNr}", HttpMethod.Get, ContentTypes.Json);
            

            var result = await _httpClient.ExecuteWithPolicyAsync<Invoice>(this, request);
            if (result != null && allowCache)
            {
                StoreInCache(result);
            }
            return result;
        }

        public async Task<Invoice> CreateInvoice(Invoice invoice, bool storeInCache = true)
        {
            var request = new RestRequest("invoices", HttpMethod.Post, ContentTypes.Json)
            {
                ContentType = ContentTypes.Json
            };


            request.AddParameter(new[] { invoice});
            var response = await _httpClient.SendWithPolicyAsync<object>(this, request);
            if (response.HttpResponseMessage.IsSuccessStatusCode)
            {
                //invoice.Id = Convert.ToInt32(response.Content);
                if (storeInCache)
                {
                    StoreInCache(invoice);
                }
                return invoice;
            }
            throw new RequestFailedLibException(response.HttpResponseMessage.StatusCode);
        }

        public async Task<Invoice> UpdateInvoice(Invoice invoice, bool storeInCache = true)
        {
            var request = new RestRequest($"Invoices/{invoice.Id}", HttpMethod.Put, ContentTypes.Json)
            {
                ContentType = ContentTypes.Json,
            };

            request.AddParameter(new[] { invoice });
            var response = await _httpClient.SendWithPolicyAsync<string>(this, request);
            if (response.HttpResponseMessage.IsSuccessStatusCode)
            {
                if (storeInCache)
                {
                    StoreInCache(invoice);
                }
                return invoice;
            }
            throw new RequestFailedLibException(response.HttpResponseMessage.StatusCode);
        }

        public async Task DeleteInvoice(Invoice invoice, bool updateInCache = true)
        {
            var request = new RestRequest($"invoices/{invoice.Id}", HttpMethod.Delete, ContentTypes.Json)
            {
                ContentType = ContentTypes.Json,
            };

            var response = await _httpClient.SendWithPolicyAsync<string>(this, request);
            if (response.HttpResponseMessage.IsSuccessStatusCode)
            {
                if (updateInCache)
                {
                    RemoveFromCache(invoice);
                }
            }
            throw new RequestFailedLibException(response.HttpResponseMessage.StatusCode);
        }

        private void StoreInCache(Invoice Invoice)
        {
            if (_cachedInvoices == null)
                _cachedInvoices = new List<Invoice>();
            _cachedInvoices.Add(Invoice);
        }

        private void RemoveFromCache(Invoice Invoice)
        {
            if (Invoice == null)
                throw new ArgumentNullException(nameof(Invoice));

            lock (_cachedInvoices)
            {
                if (_cachedInvoices != null && _cachedInvoices.Any(p => p.Id == Invoice.Id))
                    _cachedInvoices.Remove(_cachedInvoices.First(p => p.Id == Invoice.Id));
            }
        }
    }
}
