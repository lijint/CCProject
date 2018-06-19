using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Web;
using System.Web.Services;
using DownLoad.BLL.Common;
using DownLoad.Entity;
using DownLoad.Model;
using log4net;
using Newtonsoft.Json;
using System.Net.Mail;
namespace DownLoad
{
    /// <summary>
    /// Service1 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class Service1 : System.Web.Services.WebService
    {

        [WebMethod]
        public Result Add()
        {
            Result result = new Result();
            result.IsOK = true; 
            try
            {
            MTMS_BarCodeSNTB b = new MTMS_BarCodeSNTB();
            b.BarCodeSN = "1";
            b.MaterialCode = "xxx";
            b.SummonsNumber = "111111";
         //   string str = System.Configuration.ConfigurationManager.ConnectionStrings["XNet"].ConnectionString;
            XNetEntities e = new XNetEntities();
            e.AddToMTMS_BarCodeSNTB(b);
            result.IsOK=Convert.ToBoolean(e.SaveChanges());

           
          
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(GetType()).Info(ex.InnerException.Message);
                result.IsOK = false;
                result.Description = ex.InnerException.Message;
            }
            return result;
        }


        [WebMethod]
        public Result Delete()
        {
            Result result = new Result();
            result.IsOK = true; try
            {
              
                XNetEntities db = new XNetEntities();
                IQueryable<MTMS_BarCodeSNTB> barCodeSNTB = db.MTMS_BarCodeSNTB.Where(a => a.BarCodeSN == "1").OrderBy(a => a.BarCodeSN);
                foreach (var item in barCodeSNTB)
                {
                    db.DeleteObject(item);
                }
                result.IsOK = Convert.ToBoolean(db.SaveChanges());

              //  MTMS_BarCodeSNTB mB = CreateQuery<MTMS_BarCodeSNTB>();
                //user.UserEmail = "Jone@123.com";);
               // IQueryable<MTMS_BarCodeSNTB> barCodeSNTB = from p in db.MTMS_BarCodeSNTB select p;
                                                                      


            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(GetType()).Info(ex.InnerException.Message);
                result.IsOK = false;
                result.Description = ex.InnerException.Message;
            }
            return result;
        }

        [WebMethod]
        public Result Serach()
        {
            Result result = new Result();
            result.IsOK = true; try
            {

                XNetEntities db = new XNetEntities();

                var list = db.MTMS_BarCodeSNTB.Select(p => new { p.BarCodeSN,p.SummonsNumber});
            
                MTMS_BarCodeSNTB dt = db.MTMS_BarCodeSNTB.FirstOrDefault(a => a.BarCodeSN == "2");
                 JsonSerializerSettings settings = new JsonSerializerSettings();
                 settings.MissingMemberHandling = MissingMemberHandling.Ignore;
                 var data = JsonConvert.SerializeObject(list);
                 var data1 = JsonConvert.SerializeObject(dt);    


            }
            catch (Exception ex)
            {

                LogHelper.WriteLog(GetType()).Info(ex.InnerException.Message);
                result.IsOK = false;
                result.Description = ex.InnerException.Message;
            }
            return result;
        }


        [WebMethod]
        public Result SendMail()
        {
            Result result = new Result();
            result.IsOK = true; 
            try
            {
                MailAddress from = new MailAddress("wptool@well-pay.com", "wtool "); //邮件的发件人

                MailMessage mail = new MailMessage();
                mail.Subject = "H客服解绑申请";
                mail.From = from;
                mail.To.Add("747327841@qq.com");
             //   mail.CC.Add("chenc@mail.landicorp.com");
                mail.Body = "xxxxxxxx";
                mail.BodyEncoding = System.Text.Encoding.UTF8;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.Normal;
               // mail.Attachments.Add();

                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
                SmtpClient client = new SmtpClient();
                //设置用于 SMTP 事务的主机的名称，填IP地址也可以了
                client.Host = "smtp.well-pay.com";
                //设置用于 SMTP 事务的端口，默认的是 25
                client.Port = 25;
               // client.UseDefaultCredentials = false;
                client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
              //  //这里才是真正的邮箱登陆名和密码，比如我的邮箱地址是 hbgx@hotmail， 我的用户名为 hbgx ，我的密码是 xgbh
                client.Credentials = new System.Net.NetworkCredential("wptool@well-pay.com", "Tiscenter2017");
               // client.EnableSsl = true;
                //都定义完了，正式发送了，很是简单吧！
                client.Send(mail);

            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(GetType()).Info(ex.InnerException.Message);
                result.IsOK = false;
                result.Description = ex.InnerException.Message;
            }
            return result;
        
        }

        [WebMethod]
        public Result test()
        {
            Result result = new Result();
            PMSEntities pMSEntities = new PMSEntities();
            Sys_UserTB userTB = pMSEntities.Sys_UserTB.FirstOrDefault(p => p.Account == "111111" && p.StateCode == true);
            if (userTB == null)
            {
                result.IsOK = false;
                result.Description = "续期异常";
                return result;
            }
            int months = Convert.ToInt16(ParameterAPI.GetConfigValue("RenewalWay1"));
            userTB.FailureDateTime = userTB.FailureDateTime.AddMonths(months);
            userTB.UpdateDateTime = DateTime.Now;
            pMSEntities.Refresh(System.Data.Objects.RefreshMode.ClientWins, userTB);
            result.IsOK = Convert.ToBoolean(pMSEntities.SaveChanges());
            return result;
        }
    }
}