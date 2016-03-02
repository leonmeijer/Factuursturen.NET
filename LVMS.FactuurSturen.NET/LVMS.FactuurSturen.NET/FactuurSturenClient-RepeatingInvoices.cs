using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LVMS.FactuurSturen.Model;

namespace LVMS.FactuurSturen
{
    public partial class FactuurSturenClient
    {
        private readonly List<Invoice> _cachedSavedInvoices = new List<Invoice>();
        private const string ResourceInvoicesSaved = "invoices_saved";

        /// <summary>
        /// Returns a list of saved invoices.
        /// </summary>
        /// <param name="allowCache"></param>
        /// <returns></returns>
        public async Task<Invoice[]> GetSavedInvoices(bool? allowCache = true)
        {
            return await GetInvoicesInternal(ResourceInvoicesSaved, allowCache, _cachedSavedInvoices);
        }
      
        /// <summary>
        /// Return a specific saved invoice
        /// </summary>
        /// <param name="invoiceNr">Invoice number</param>
        /// <param name="allowCache">Whether or not to look it up/store it in cache</param>
        /// <returns></returns>
        public async Task<Invoice> GetSavedInvoice(string invoiceNr, bool allowCache = true)
        {
            return await GetInvoiceInternal(ResourceInvoicesSaved, invoiceNr, allowCache, _cachedSavedInvoices);
        }

        /// <summary>
        /// Delete a saved invoice.
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns></returns>
        public async Task DeleteSavedInvoice(Invoice invoice)
        {
            if (invoice == null)
                throw new ArgumentNullException(nameof(invoice));
            await DeleteInvoiceInternal(ResourceInvoicesSaved, invoice.InvoiceNr, _cachedSavedInvoices);
        }

        /// <summary>
        /// Delete a saved invoice.
        /// </summary>
        /// <param name="invoiceNr"></param>
        /// <returns></returns>
        public async Task DeleteSavedInvoice(string invoiceNr)
        {
            await DeleteInvoiceInternal(ResourceInvoicesSaved, invoiceNr, _cachedSavedInvoices);
        }
    }
}
