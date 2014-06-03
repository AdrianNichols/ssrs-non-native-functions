using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Microsoft.Reporting.WebForms;
using System.IO;

namespace Demonstrations
{
    public partial class SalesInvoiceDemo : System.Web.UI.Page
    {
        private DataTable dt;

        protected void Page_Load(object sender, EventArgs e)
        {
            Data.demoData dd = new Data.demoData();
            dt = dd.sampleInvoiceDataTable(Request);
        }

        protected void reportSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (reportSelector.SelectedIndex >= 3)
            {
                // Show Culture Selector
                divcultureSelector.Style.Remove(HtmlTextWriterStyle.Display);
                divcultureSelector.Style.Add(HtmlTextWriterStyle.Display, "block");
            }
            else 
            {
                // Hide Culture Selector
                divcultureSelector.Style.Remove(HtmlTextWriterStyle.Display);
                divcultureSelector.Style.Add(HtmlTextWriterStyle.Display, "none");
            }
        }
        protected void executeReport_click(object sender, EventArgs e)
        {
            if (reportSelector.SelectedIndex == 9)
                executeReportInAppDomain_click(sender, e);
            else
            {
                LocalReport rpt = new LocalReport();
                rpt.EnableExternalImages = true;
                rpt.ReportPath = String.Concat(Path.GetDirectoryName(Request.PhysicalPath), "\\Reports\\", reportSelector.SelectedValue);
                string orientation = (rpt.GetDefaultPageSettings().IsLandscape) ? "landscape" : "portrait";
                StringReader formattedReport = Business.reportHelper.FormatReportForTerritory(rpt.ReportPath, orientation, cultureSelector.SelectedValue);
                rpt.LoadReportDefinition(formattedReport);

                // Add Data Source
                rpt.DataSources.Add(new ReportDataSource("InvoiceDataTable", dt));

                // Internationlisation: Add uiCulture and Translation Labels
                if (reportSelector.SelectedIndex >= 3)
                {
                    Dictionary<string, string> reportLabels = Reports.reportTranslation.translateInvoice(cultureSelector.SelectedValue);
                    ReportParameterCollection reportParams = new ReportParameterCollection();

                    reportParams.Add(new ReportParameter("uiCulture", cultureSelector.SelectedValue));
                    foreach (string key in reportLabels.Keys)
                        reportParams.Add(new ReportParameter(key, reportLabels[key]));

                    rpt.SetParameters(reportParams);
                }

                // Render To Browser
                renderPDFToBrowser(rpt.Render("PDF", Business.reportHelper.GetDeviceInfoFromReport(rpt, cultureSelector.SelectedValue, "PDF")));
            }
        }

        protected void executeReportInAppDomain_click(object sender, EventArgs e)
        {
            // Internationlisation: Add uiCulture and Translation Labels
            Dictionary<string, string> reportParams = new Dictionary<string,string>();
            if (reportSelector.SelectedIndex >= 3)
            {
                reportParams = Reports.reportTranslation.translateInvoice(cultureSelector.SelectedValue);
                reportParams.Add("uiCulture", cultureSelector.SelectedValue);
            }

            // Render to Browser (in Another App Domain - Addresses known Memory Leak)
            renderPDFToBrowser(Business.reportHelper.RenderReportToMemoryAsPDFInAnotherAppDomain(
                   Path.GetDirectoryName(Request.PhysicalPath), 
                   String.Concat(Path.GetDirectoryName(Request.PhysicalPath), "\\Reports\\", reportSelector.SelectedValue), 
                   "InvoiceDataTable", 
                   dt, 
                   reportParams, 
                   cultureSelector.SelectedValue
                   )
            );
            
        }

        private void renderPDFToBrowser(Byte[] reportData)
        {
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", "inline");
            Response.AddHeader("Content-Length", reportData.Length.ToString());
            Response.BinaryWrite(reportData);
            Response.End();
        }
        

    }
}
