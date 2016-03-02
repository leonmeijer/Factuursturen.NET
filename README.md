# LVMS.FactuurSturen.NET
Unofficial open source C# helper library for communication with the [API of FactuurSturen.nl](https://www.factuursturen.nl/docs/api_v1.pdf).
All calls support the async/await model. Previous API-responses are cached in-memory by default and overrides (optional parameters in the method calls) are available to force fresh data.
All calls use Polly, a transient fault handling library. If calls fail due to transient errors, they are automatically retried.

Supported:
- Get list of clients, specific client
- Get list of products, specific product, create product, update product, delete product
- Get list of invoices, specific invoice, list of filtered invoices (e.g. paid invoices)
- Get list of saved invoices, specific saved invoice, delete saved invoice
- Get list of repeating invoices, specific repeating invoices, delete repeating invoice
- Create (and send) invoices
- Get the PDF version of a sent invoice

To do:
- Payment, Balances, Tax, Country and Search API

This Portable Library is compatible with: (ASP).Net Framework 4.0.3/4.5/4.6, Windows (Phone) 8.1 Universal Apps and Windows Phone 8.1 Silverlight. So you can use this library to automate invoicing in desktop, web and mobile apps.

## How to use?
Use the source code from this repository or download the NuGet package: [LVMS.FactuurSturenNet.Signed](https://www.nuget.org/packages/LVMS.FactuurSturenNet.Signed/). 
In this repo, you can find an example application named LVMS.FactuurSturenNet.NET.TestClient (ConsoleApplication1). If you run it, it will
prompt you for credentials; that's your user name and API key. Or if you run it often, make sure you use the credentials text file.
	
	var client = new FactuurSturenClient(); // has overload to disable all caching
	await client.LoginAsync(userName, apiKey);

The password is the API key that you can find under settings in the FactuurSturen.nl website.
	
To retrieve lists:	

	var clients = await client.GetClients();
	var products = await client.GetProducts();
	var invoices = await client.GetInvoices();
	var overDueInvoices = await client.GetInvoicesWithFilter(InvoiceFilters.Overdue);
	var profiles = await client.GetProfiles();

To create a draft invoice (to be send later via the web application):	

	var to = await client.GetClient("My client name"); // can also be via Id
    var invoice = new Invoice(to, InvoiceActions.Save, "draft 1");
    var line1 = new InvoiceLine(1, "Test line", taxRate: 21, price: 125);
    invoice.AddLine(line1);
    await client.CreateDraftInvoice(invoice);
    	
To create and send an invoice immediately:	

	var to = await client.GetClient("My client name"); // can also be via Id
    var invoice = new Invoice(to, InvoiceActions.Send, SendMethods.Email);
    var line1 = new InvoiceLine(1, "hrs", "Consulting", 21, 100); // variation with amount description
    invoice.AddLine(line1);
    var createdInvoice = await client.CreateInvoice(invoice, true);

## Contributions

Contributions are welcome. Fork this repository and send a pull request if you have something useful to add.
