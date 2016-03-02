namespace LVMS.FactuurSturen.Model
{
    public enum SendMethods
    {
        None,
        /// <summary>
        /// Print the invoices yourself. We'll send you the invoice number so you can execute a command to retrieve the PDF if you need so 
        ///  </summary>
        Mail,
        /// <summary>
        ///  Send invoices through e-mail. It will be sent immediately
        /// </summary>
        Email,
        /// <summary>
        /// Send invoice through the printcenter.
        /// </summary>
        Printcenter
    }
}