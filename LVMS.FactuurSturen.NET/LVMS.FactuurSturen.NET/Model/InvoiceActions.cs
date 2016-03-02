namespace LVMS.FactuurSturen.Model
{
    public enum InvoiceActions
    {
        None,
        /// <summary>
        /// Send the invoice
        /// </summary>
        Send,
        /// <summary>
        /// Save the invoice as a draft
        /// </summary>
        Save,
        /// <summary>
        /// Plan a recurring invoice
        /// </summary>
        Repeat
    }
}