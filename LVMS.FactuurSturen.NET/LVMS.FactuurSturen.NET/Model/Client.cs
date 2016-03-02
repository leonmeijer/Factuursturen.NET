using System;
using Newtonsoft.Json;

namespace LVMS.FactuurSturen.Model
{
    public class Client
    {
        public int ClientNr { get; set; }
        public string Contact { get; set; }
        /// <summary>
        /// Show the contact name on the invoice
        /// </summary>
        public bool ShowContact { get; set; }
        public string Company { get; set; }
        public string Address { get; set; }
        public string Zipcode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        /// <summary>
        /// Invoice is sent to this e-mail address, if the sendmethod is e-mail
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// The IBAN number of the client
        /// </summary>
        public string BankCode { get; set; }
        public string BicCode { get; set; }
        public string TaxNumber { get; set; }
        /// <summary>
        /// If the taxes on the invoice is shifted to the receiver
        /// </summary>
        public bool TaxShifted { get; set; }
        /// <summary>
        /// When last invoice to this client was sent
        /// </summary>
        public DateTime? LastInvoice { get; set; }
        /// <summary>
        /// How to send the invoice to the receiver.
        /// </summary>
        [JsonConverter(typeof(LowerCaseStringEnumConverter))]
        public SendMethods SendMethod { get; set; }
        /// <summary>
        /// How the invoice is going to be paid
        /// </summary>
        [JsonConverter(typeof(LowerCaseStringEnumConverter))]
        public PaymentMethods PaymentMethod { get; set; }
        /// <summary>
        /// The term of payment in days. Defines when the invoice has to be paid by the recipient
        /// </summary>
        public int Top { get; set; }
        /// <summary>
        /// Standard discount percentage for this client. Every invoice defined for this client will automatically get this discount percentage
        /// </summary>
        public int StdDiscount { get; set; }
        /// <summary>
        /// The first line used in the e-mail to address the recipient. Example: "Dear sir/madam,"
        /// </summary>
        public string MailIntro { get; set; }
        /// <summary>
        /// Three lines that will be printed on the invoice. Can be used for references to other documents or something else
        /// </summary>
        public Reference Reference { get; set; }
        /// <summary>
        /// Notes saved for this client
        /// </summary>
        public string Notes { get; set; }
        /// <summary>
        /// Print the field 'notes' on every invoice for the client
        /// </summary>
        public bool NotesOnInvoice { get; set; }
        /// <summary>
        /// Non-active clients are hidden in the web application.
        /// </summary>
        public bool Active { get; set; }
        /// <summary>
        /// In what language the invoice will be generated for this client.
        /// </summary>
        [JsonConverter(typeof(LowerCaseStringEnumConverter))]
        public Languages DefaultDocLang { get; set; }
        /// <summary>
        /// ID of used e-mail text
        /// </summary>
        public int DefaultEmail { get; set; }
        /// <summary>
        /// Used currency in invoice. Like 'EUR', 'USD', etc.
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// The mandate identification
        /// </summary>
        public string MandateId { get; set; }
        /// <summary>
        /// The date of the signature
        /// </summary>
        public string MandateDate { get; set; }
        /// <summary>
        /// The collection type
        /// </summary>
        [JsonConverter(typeof(LowerCaseStringEnumConverter))]
        public CollectTypes CollectType { get; set; }
        /// <summary>
        /// Will show if the products on the invoice for this client will be handled as excluding or including tax
        /// </summary>
        [JsonConverter(typeof(LowerCaseStringEnumConverter))]
        public TaxTypes TaxType { get; set; }
        public string DefaultCategory { get; set; }
        /// <summary>
        /// Date and time when record was updated
        /// </summary>
        public DateTime? TimeStamp;
    }
}
