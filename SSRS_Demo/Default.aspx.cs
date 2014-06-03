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
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void lnkSalesInvoiceDemo_click(object sender, EventArgs e)
        {
            Response.Redirect("SalesInvoiceDemo.aspx", true);
        }
    }
}
