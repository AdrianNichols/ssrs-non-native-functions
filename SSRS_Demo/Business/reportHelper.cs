using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Reporting.WebForms;
using System.Xml;
using System.Reflection;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Data;

namespace Demonstrations.Business
{
    public static class reportHelper
    {
        private static string GetDeviceInfo(string uiCulture, string format = "pdf", string marginscsv = "0,0,0,0", string orientation = "portrait")
        {
            string deviceInfo;
            string[] margins = marginscsv.Split(',');

            string tm = margins[0]; // Top margin
            string lm = margins[1]; // Left margin
            string bm = margins[2]; // Bottom margin
            string rm = margins[3]; // Right margin

            if (uiCulture == "en-US")
            {
                deviceInfo = (orientation == "portrait") ? "<PageWidth>8.5in</PageWidth><PageHeight>11in</PageHeight>" : "<PageWidth>11in</PageWidth><PageHeight>8.5in</PageHeight>";
            }
            else
            {
                deviceInfo = (orientation == "portrait") ? "<PageWidth>21cm</PageWidth><PageHeight>29.7cm</PageHeight>" : "<PageWidth>29.7cm</PageWidth><PageHeight>21cm</PageHeight>";
            }

            deviceInfo = "<DeviceInfo>" + "<OutputFormat>" + format + "</OutputFormat>" + deviceInfo + "  <MarginTop>" + tm + "cm</MarginTop>" + "  <MarginLeft>" + lm + "cm</MarginLeft>" + "  <MarginRight>" + rm + "cm</MarginRight>" + "  <MarginBottom>" + bm + "cm</MarginBottom>" + "</DeviceInfo>";

            return deviceInfo;
        }

        public static string GetDeviceInfoFromReport(LocalReport rpt, string uiCulture, string format)
        {
            string strMargins;
            string orientation;

            //Render overwrites margins defined in RDLC; capture margins in RDLC
            ReportPageSettings pageSettings = rpt.GetDefaultPageSettings();
            strMargins = String.Concat((Convert.ToDouble(pageSettings.Margins.Top) / 40.0).ToString(), ",", (Convert.ToDouble(pageSettings.Margins.Left) / 40.0).ToString(), ",", (Convert.ToDouble(pageSettings.Margins.Bottom) / 40.0).ToString(), ",", (Convert.ToDouble(pageSettings.Margins.Right) / 40.0).ToString());

            //Capture report orientation
            orientation = pageSettings.IsLandscape ? "landscape" : "portrait";

            return GetDeviceInfo(uiCulture, format, strMargins, orientation);
        }

        public static StringReader FormatReportForTerritory(string reportPath, string orientation, string uiCulture)
        {
            XmlDocument xmlDoc = new XmlDocument();
            Assembly asm = Assembly.GetExecutingAssembly();
            Stream xmlStream;

            xmlStream = asm.GetManifestResourceStream(reportPath);

            try
            {
                xmlDoc.Load(reportPath);
            }
            catch
            {
                //Ignore??!?
            }

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("nm", "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition");
            nsmgr.AddNamespace("rd", "http://schemas.microsoft.com/sqlserver/reporting/reportdesigner");

            if ((uiCulture == "en-US") && (orientation == "landscape"))
            {
                xmlDoc.SelectSingleNode("//nm:Page/nm:PageWidth", nsmgr).InnerText = "11in";
                xmlDoc.SelectSingleNode("//nm:Page/nm:PageHeight", nsmgr).InnerText = "8.5in";
            }
            else if ((uiCulture == "en-US") && (orientation == "portrait"))
            {
                xmlDoc.SelectSingleNode("//nm:Page/nm:PageWidth", nsmgr).InnerText = "8.5in";
                xmlDoc.SelectSingleNode("//nm:Page/nm:PageHeight", nsmgr).InnerText = "11in";
            }
            else if (!(uiCulture == "en-US") && (orientation == "landscape"))
            {
                xmlDoc.SelectSingleNode("//nm:Page/nm:PageWidth", nsmgr).InnerText = "29.7cm";
                xmlDoc.SelectSingleNode("//nm:Page/nm:PageHeight", nsmgr).InnerText = "21cm";
            }
            else if (!(uiCulture == "en-US") && (orientation == "portrait"))
            {
                xmlDoc.SelectSingleNode("//nm:Page/nm:PageWidth", nsmgr).InnerText = "21cm";
                xmlDoc.SelectSingleNode("//nm:Page/nm:PageHeight", nsmgr).InnerText = "29.7cm";

            }

            StringReader rdlcOutputStream = new StringReader(xmlDoc.DocumentElement.OuterXml);

            return rdlcOutputStream;
        }

        public static StringReader LoadReportXml(string reportPath, string orientation)
        {
            XmlDocument xmlDoc = new XmlDocument();
            Assembly asm = Assembly.GetExecutingAssembly();
            Stream xmlStream;

            xmlStream = asm.GetManifestResourceStream(reportPath);

            try
            {
                xmlDoc.Load(reportPath);
            }
            catch
            {
                //Ignore??!?
            }

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("nm", "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition");
            nsmgr.AddNamespace("rd", "http://schemas.microsoft.com/sqlserver/reporting/reportdesigner");

            StringReader rdlcOutputStream = new StringReader(xmlDoc.DocumentElement.OuterXml);

            return rdlcOutputStream;
        }


        public static Byte[] RenderReportToMemoryAsPDFInAnotherAppDomain(string path, string reportName, string reportSourceName, DataTable dt, Dictionary<string, string> parameters, string currentTerritory)
        {
            var ads = new AppDomainSetup();
            ads.ApplicationBase = Path.Combine(path, "Bin");
            ads.PrivateBinPath = ads.ApplicationBase;
            var appDomain = AppDomain.CreateDomain("SSRS Domain", null, ads);
            var repHelper = appDomain.CreateInstanceAndUnwrap("Demonstrations", "Demonstrations.Business.ReportHelperInAppDomain") as ReportHelperInAppDomain;
            Byte[] data = repHelper.RenderReportToMemoryAsPDF(reportName, reportSourceName, dt, parameters, currentTerritory);
            AppDomain.Unload(appDomain);
            return data;
        }
    }

    public class ReportHelperInAppDomain : MarshalByRefObject
    {
        public ReportHelperInAppDomain() { }

        // Reports Load
        public static void LoadSubReport(LocalReport rpt, string reportName, string reportPath)
        {
            rpt.LoadSubreportDefinition(reportName, File.OpenRead(reportPath));
        }

        // Data Sources
        public static void AddDataSource(LocalReport rpt, string dataSourceName, DataTable dataTableName)
        {
            rpt.DataSources.Add(new ReportDataSource(dataSourceName, dataTableName));
        }

        // Parameters
        public static void SetSingleParameter(LocalReport rpt, string paramName, string paramValue)
        {
            var rptParam = new ReportParameter(paramName, paramValue);
            rpt.SetParameters(new ReportParameter[] { rptParam });
        }

        public Byte[] RenderReportToMemoryAsPDF(string reportName, string reportSourceName, DataTable dt, Dictionary<string, string> reportParameters, string currentCulture)
        {
            using (var rpt = new LocalReport())
            {
                rpt.EnableExternalImages = true;
                rpt.ReportPath = reportName;
                AddDataSource(rpt, reportSourceName, dt);

                foreach (string key in reportParameters.Keys)
                    SetSingleParameter(rpt, key, reportParameters[key]);

                Byte[] data = rpt.Render("PDF", GetDeviceInfoFromReport(rpt, currentCulture, "PDF"));
                return data;
            }
        }

        private static string GetDeviceInfo(string uiCulture, string format = "pdf", string marginscsv = "0,0,0,0", string orientation = "portrait")
        {
            string deviceInfo;
            string[] margins = marginscsv.Split(',');

            string tm = margins[0]; // Top margin
            string lm = margins[1]; // Left margin
            string bm = margins[2]; // Bottom margin
            string rm = margins[3]; // Right margin

            if (uiCulture == "en-US")
            {
                deviceInfo = (orientation == "portrait") ? "<PageWidth>8.5in</PageWidth><PageHeight>11in</PageHeight>" : "<PageWidth>11in</PageWidth><PageHeight>8.5in</PageHeight>";
            }
            else
            {
                deviceInfo = (orientation == "portrait") ? "<PageWidth>21cm</PageWidth><PageHeight>29.7cm</PageHeight>" : "<PageWidth>29.7cm</PageWidth><PageHeight>21cm</PageHeight>";
            }

            deviceInfo = "<DeviceInfo>" + "<OutputFormat>" + format + "</OutputFormat>" + deviceInfo + "  <MarginTop>" + tm + "cm</MarginTop>" + "  <MarginLeft>" + lm + "cm</MarginLeft>" + "  <MarginRight>" + rm + "cm</MarginRight>" + "  <MarginBottom>" + bm + "cm</MarginBottom>" + "</DeviceInfo>";

            return deviceInfo;
        }

        public static string GetDeviceInfoFromReport(LocalReport rpt, string uiCulture, string format)
        {
            string strMargins;
            string orientation;

            //Render overwrites margins defined in RDLC; capture margins in RDLC
            ReportPageSettings pageSettings = rpt.GetDefaultPageSettings();
            strMargins = String.Concat((Convert.ToDouble(pageSettings.Margins.Top) / 40.0).ToString(), ",", (Convert.ToDouble(pageSettings.Margins.Left) / 40.0).ToString(), ",", (Convert.ToDouble(pageSettings.Margins.Bottom) / 40.0).ToString(), ",", (Convert.ToDouble(pageSettings.Margins.Right) / 40.0).ToString());

            //Capture report orientation
            orientation = pageSettings.IsLandscape ? "landscape" : "portrait";

            return GetDeviceInfo(uiCulture, format, strMargins, orientation);
        }
    }
}