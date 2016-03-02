using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LVMS.FactuurSturen.Model;

namespace LVMS.FactuurSturen
{
    public partial class FactuurSturenClient
    {
        private readonly List<Invoice> _cachedRepeatingInvoices = new List<Invoice>();
        private const string ResourceInvoicesRepeating = "invoices_repeated";

        /// <summary>
        /// Returns a list of repeating invoices.
        /// </summary>
        /// <param name="allowCache"></param>
        /// <returns></returns>
        public async Task<Invoice[]> GetRepeatingInvoices(bool? allowCache = true)
        {
            return await GetInvoicesInternal(ResourceInvoicesRepeating, allowCache, _cachedRepeatingInvoices);
        }
      
        /// <summary>
        /// Return a specific repeating invoice
        /// </summary>
        /// <param name="invoiceNr">Invoice number</param>
        /// <param name="allowCache">Whether or not to look it up/store it in cache</param>
        /// <returns></returns>
        public async Task<Invoice> GetRepeatingInvoice(string invoiceNr, bool allowCache = true)
        {
            return await GetInvoiceInternal(ResourceInvoicesRepeating, invoiceNr, allowCache, _cachedRepeatingInvoices);
        }

        /// <summary>
        /// Delete a repeating invoice.
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns></returns>
        public async Task DeleteRepeatingInvoice(Invoice invoice)
        {
            if (invoice == null)
                throw new ArgumentNullException(nameof(invoice));
            await DeleteInvoiceInternal(ResourceInvoicesRepeating, invoice.InvoiceNr, _cachedRepeatingInvoices);
        }

        /// <summary>
        /// Delete a repeating invoice.
        /// </summary>
        /// <param name="invoiceNr"></param>
        /// <returns></returns>
        public async Task DeleteRepeatingInvoice(string invoiceNr)
        {
            await DeleteInvoiceInternal(ResourceInvoicesRepeating, invoiceNr, _cachedRepeatingInvoices);
        }
    }
}
