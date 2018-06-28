using DownLoad.BLL.Common;
using DownLoad.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DownLoad.BLL.Login
{
    public partial class Hand : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string context = "hand?" +Common.Common.Decrypt(HttpContext.Current.Request.Url.Query.TrimStart('?'));
            HttpContext.Current.RewritePath(context);
            string mailAddress = HttpContext.Current.Request.QueryString.Get("mailAddress");
            string dateTime= HttpContext.Current.Request.QueryString.Get("dateTime");
            TimeSpan UseTime = DateTime.Now - Convert.ToDateTime(dateTime);
            if (UseTime.Seconds >= 60)
            {
                Response.Redirect(ParameterAPI.GetConfig("RegisterFURL").ConfigValue + "?mailaddr=" + mailAddress, false);
            }
            else
            {
                PCBEntities pCBEntities = new PCBEntities();
                PCB_EngineerInfo engineerInfo = pCBEntities.PCB_EngineerInfo.FirstOrDefault(p => p.MailAddress == mailAddress);
                if (engineerInfo != null && engineerInfo != default(PCB_EngineerInfo))
                {
                    engineerInfo.UpdateDateTime = DateTime.Now;
                    engineerInfo.StateCode = true;
                    pCBEntities.Refresh(System.Data.Objects.RefreshMode.ClientWins, engineerInfo);
                    bool isok = Convert.ToBoolean(pCBEntities.SaveChanges());
                    if (isok)
                    {
                        Response.Redirect(ParameterAPI.GetConfig("RegisterSURL").ConfigValue+ "?mailaddr="+ mailAddress + "&isconfirm=true", false);
                    }
                    else
                    {
                        Response.Redirect(ParameterAPI.GetConfig("RegisterFURL").ConfigValue + "?mailaddr=" + mailAddress, false);
                    }
                }
            }
        }
    }
}