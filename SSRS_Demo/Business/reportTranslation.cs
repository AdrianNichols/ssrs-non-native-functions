using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Demonstrations.Reports
{
    public static class reportTranslation
    {

        public static Dictionary<string, string> translateInvoice(string targetLanguage)
        {
            switch (targetLanguage)
            {
                case "de-DE":
                    return translateInvoiceGerman();
                case "en-US":
                    return translateInvoiceAmerican();
                default: // Default to UK English
                    return translateInvoiceEnglish();
            }
        }

        public static Dictionary<string, string> translateInvoiceEnglish()
        {
            return new Dictionary<string, string>()
            {
                { "lblCompanyTel", "Tel" }, 
                { "lblCompanyFax", "Fax" }, 
                { "lblCompanyEmail", "Email" }, 
                {"lblInvoiceDate", "Tax Point Date"},
                {"lblInvoiceNumber", "Invoice No."},
                {"lblInvoiceDueDate", "Due Date"},
                {"lblProductCode", "Code"},
                {"lblProductDescription", "Description"},
                {"lblProductUnitCost", "£ per Unit"},
                {"lblProductQuantity", "Quantity"},
                {"lblProductTotalCost", "£ Total"},
            };
        }

        public static Dictionary<string, string> translateInvoiceGerman()
        {
            return new Dictionary<string, string>()
            {
                { "lblCompanyTel", "Tel" }, 
                { "lblCompanyFax", "Fax" }, 
                { "lblCompanyEmail", "Email" }, 
                {"lblInvoiceDate", "Rechnungs Datum"},
                {"lblInvoiceNumber", "Rechnungs Nr."},
                {"lblInvoiceDueDate", "Zahlungsfälligkeit"},
                {"lblProductCode", "Code"},
                {"lblProductDescription", "Beschreibung"},
                {"lblProductUnitCost", "€ Preis"},
                {"lblProductQuantity", "Menge"},
                {"lblProductTotalCost", "€ Gesamt"},
            };
        }

        public static Dictionary<string, string> translateInvoiceAmerican()
        {
            return new Dictionary<string, string>()
            {
                { "lblCompanyTel", "Tel" }, 
                { "lblCompanyFax", "Fax" }, 
                { "lblCompanyEmail", "Email" }, 
                {"lblInvoiceDate", "Invoice Date"},
                {"lblInvoiceNumber", "Invoice No."},
                {"lblInvoiceDueDate", "Due Date"},
                {"lblProductCode", "Code"},
                {"lblProductDescription", "Description"},
                {"lblProductUnitCost", "$ per Unit"},
                {"lblProductQuantity", "Qunatity"},
                {"lblProductTotalCost", "$ Total"},
            };
        }
    }
}