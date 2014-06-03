using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;

namespace Demonstrations.Data
{
    public class demoData
    {
        /// <summary>
        /// Sample table that would be replaced by a datatable returned by a real-world
        /// query from your data.
        /// </summary>
        /// <returns></returns>
        public DataTable sampleInvoiceDataTable(HttpRequest request)
        {
            DataTable dt = new DataTable();

            // Data Types Used
            System.Type typeInt = System.Type.GetType("System.Int32");
            System.Type typeStr = System.Type.GetType("System.String");
            System.Type typeDte = System.Type.GetType("System.DateTime");
            System.Type typeDbl = System.Type.GetType("System.Double");

            // Create Customer Columns
            dt.Columns.Add(new DataColumn("customer_id", typeInt));
            dt.Columns.Add(new DataColumn("customer_name", typeStr));
            dt.Columns.Add(new DataColumn("customer_address_1", typeStr));
            dt.Columns.Add(new DataColumn("customer_address_2", typeStr));
            dt.Columns.Add(new DataColumn("customer_address_city", typeStr));
            dt.Columns.Add(new DataColumn("customer_address_state", typeStr));
            dt.Columns.Add(new DataColumn("customer_address_zip", typeStr));
            dt.Columns.Add(new DataColumn("customer_tel", typeStr));
            dt.Columns.Add(new DataColumn("customer_fax", typeStr));
            dt.Columns.Add(new DataColumn("customer_email", typeStr));

            // Create Columns for Company Data
            dt.Columns.Add(new DataColumn("company_name", typeStr));
            dt.Columns.Add(new DataColumn("company_logo_path", typeStr));
            dt.Columns.Add(new DataColumn("company_address_1", typeStr));
            dt.Columns.Add(new DataColumn("company_address_2", typeStr));
            dt.Columns.Add(new DataColumn("company_address_city", typeStr));
            dt.Columns.Add(new DataColumn("company_address_state", typeStr));
            dt.Columns.Add(new DataColumn("company_address_zip", typeStr));
            dt.Columns.Add(new DataColumn("company_tel", typeStr));
            dt.Columns.Add(new DataColumn("company_fax", typeStr));
            dt.Columns.Add(new DataColumn("company_email", typeStr));

            // Create Columns for Invoice Data
            dt.Columns.Add(new DataColumn("invoice_date", typeDte));
            dt.Columns.Add(new DataColumn("invoice_number", typeInt));
            dt.Columns.Add(new DataColumn("invoice_due_date", typeDte));

            // Create Columns for Product Data
            dt.Columns.Add(new DataColumn("product_code", typeStr));
            dt.Columns.Add(new DataColumn("product_description", typeStr));
            dt.Columns.Add(new DataColumn("product_unit_cost", typeDbl));
            dt.Columns.Add(new DataColumn("product_quantity", typeInt));

            // Loop for 10 customers
            for (int i = 1; i <= 10; i++)
            {
                Dictionary<string, object> dataForRow = new Dictionary<string, object>();
                
                // Customer Data
                string[] customerData = dataLookup.getCustomer();
                foreach (DataColumn dc in dt.Columns)
                    if (dc.ColumnName.StartsWith("customer"))
                        dataForRow.Add(dc.ColumnName, customerData[dc.Ordinal]);

                // Company Data (always the same)
                dataForRow.Add("company_name", "RND Company");
                dataForRow.Add("company_logo_path", String.Concat(request.Url.AbsoluteUri.Replace("SalesInvoiceDemo.aspx", ""), "Images/company_logo.jpg"));
                dataForRow.Add("company_address_1", "Edison Building");
                dataForRow.Add("company_address_2", "732 Green Oak Blvd.");
                dataForRow.Add("company_address_city", "New York");
                dataForRow.Add("company_address_state", "New York");
                dataForRow.Add("company_address_zip", "39234");
                dataForRow.Add("company_tel", "572-7854060");
                dataForRow.Add("company_fax", "3529901593");
                dataForRow.Add("company_email", "demo@rndco.com");

                //Invoice Data
                string[] invoiceData = dataLookup.getInvoice();
                foreach (DataColumn dc in dt.Columns)
                    if (dc.ColumnName.StartsWith("invoice"))
                        dataForRow.Add(dc.ColumnName, invoiceData[dc.Ordinal - 20]);

                //Products Data - Append a random amount of products to data row dictionary 
                //adding a new data row per product
                int pMax = dataLookup.getRandomBetween(1, 50);
                for (int p = 1; p <= pMax; p++)
                {
                    string[] productData = dataLookup.getProduct();
                    foreach (DataColumn dc in dt.Columns)
                        if (dc.ColumnName.StartsWith("product"))
                            dataForRow.Add(dc.ColumnName, productData[dc.Ordinal - 23]);

                    //Add Row
                    dt.Rows.Add(populateRow(dt, dataForRow));

                    //Reset for product loop
                    dataForRow.Remove("product_code");
                    dataForRow.Remove("product_description");
                    dataForRow.Remove("product_unit_cost");
                    dataForRow.Remove("product_quantity");

                   
                }

            }
            return dt;
        }

        private DataRow populateRow(DataTable dt, Dictionary<string, object> dataForRow)
        {
            DataRow dr = dt.NewRow();

            foreach (DataColumn dc in dt.Columns)
            {
                if (dc.DataType == System.Type.GetType("System.Int32"))
                    dr[dc] = int.Parse((String)dataForRow[dc.ColumnName]);
                else if (dc.DataType == System.Type.GetType("System.String"))
                    dr[dc] = (String)dataForRow[dc.ColumnName];
                else if (dc.DataType == System.Type.GetType("System.DateTime"))
                    dr[dc] = DateTime.Parse((String)dataForRow[dc.ColumnName]);
                else if (dc.DataType == System.Type.GetType("System.Double"))
                    dr[dc] = dataForRow[dc.ColumnName];
            }

            return dr;
        }
    }
}