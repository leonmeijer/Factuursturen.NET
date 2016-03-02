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
}