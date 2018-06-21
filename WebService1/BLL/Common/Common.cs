using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Com.Alipay;
using Com.Alipay.Business;
using Com.Alipay.Domain;
using Com.Alipay.Model;
using DownLoad.Entity;
using DownLoad.Model;

using System.Net;

using System.Net.Mail;

namespace DownLoad.BLL.Common
{
    public class Common
    {
        #region  DataSet转换为Json
        /// <summary>    
        /// DataSet转换为Json   
        /// </summary>    
        /// <param name="dataSet">DataSet对象</param>   
        /// <returns>Json字符串</returns>    
        public static string ToJson(DataSet dataSet)
        {
            string jsonString = "{";
            foreach (DataTable table in dataSet.Tables)
            {
                jsonString += "\"" + table.TableName + "\":" + CreateJsonParameters(table) + ",";
            }
            jsonString = jsonString.TrimEnd(',');
            return jsonString + "}";
        }
        #endregion

        #region 将DataTable转换为Json格式
        /// <summary>
        /// 将DataTable转换为Json格式
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string CreateJsonParameters(DataTable dt)
        {
            StringBuilder JsonString = new StringBuilder();
            if (dt != null && dt.Rows.Count > 0)
            {


                JsonString.Append("[ ");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    JsonString.Append("{ ");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (j < dt.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + dt.Columns[j].ColumnName.ToString() +
                                  "\":" + "\"" +
                                  dt.Rows[i][j].ToString() + "\",");
                        }
                        else if (j == dt.Columns.Count - 1)
                        {
                            JsonString.Append("\"" +
                               dt.Columns[j].ColumnName.ToString() + "\":" +
                               "\"" + dt.Rows[i][j].ToString() + "\"");
                        }
                    }
                    if (i == dt.Rows.Count - 1)
                    {
                        JsonString.Append("} ");
                    }
                    else
                    {
                        JsonString.Append("}, ");
                    }
                }

                JsonString.Append("] ");
                return JsonString.ToString().Replace("\\", "\\\\");
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 根据路径获取文件
        /// <summary>
        /// 根据路径获取文件
        /// </summary>
        /// <param name="fileDir">文件目录</param>
        /// <param name="rootPath">根目录</param>
        /// <param name="dt">表</param>
        /// <returns></returns>
        public static Result GetFileList(string subDir, string rootPath, ref DataTable dt)
        {
            Result result = new Result();
            try
            {
                result.IsOK = true;
                string fileDir = "";
                if (string.IsNullOrEmpty(subDir))
                    fileDir = rootPath.TrimEnd('\\') + "\\";
                else
                    fileDir = rootPath.TrimEnd('\\') + "\\" + subDir.TrimEnd('\\') + "\\";

                string[] dirs = Directory.GetDirectories(fileDir);
                string[] files = Directory.GetFiles(fileDir);
                // string dir = "ClientUpdateFiles";
                //string url = HttpContext.Current.Request.Url.AbsoluteUri;
                //  int length = url.IndexOf('/', 7);
                //string fileUrl = url.Substring(0, length) + @"/" + dir + @"/"; //文件的下载地址
                string fileUrl = ParameterAPI.GetConfig("FileURLDir").ConfigValue;// "http://172.20.8.24/TIS.ClientUpdateFile";


                foreach (string item in files)
                {
                    DataRow dr = dt.NewRow();
                    //  dr["FileName"] = Path.GetFileName(item);
                    //   dr["FileSize"] = new FileInfo(fileDir + Path.GetFileName(item)).Length;
                    string dir = ParameterAPI.GetConfig("FileDir").ConfigValue;
                    dr["FilePath"] = Path.GetFullPath(item).Remove(0, dir.Length - 1);//string.IsNullOrEmpty(subDir) ? "" : subDir.TrimEnd('\\') + "\\";// fileDir.Substring(fileDir.IndexOf(rootPath) + rootPath.Length, fileDir.Length - rootPath.Length);//.TrimStart(rootPath.ToCharArray());
                    string p = Path.GetFullPath(item);
                    // LogHelper.Write("xxx"+p, LogHelper.LogMessageType.Error, typeof(ClientUpdateWebService));
                    if (File.Exists(p))
                        File.SetAttributes(p, FileAttributes.Normal);//将只读文件设置成正常属性

                   // dr["FileMD5"] = GetMD5Content(p);
                    if (string.IsNullOrEmpty(subDir))
                        dr["FileURL"] = fileUrl + "/" + Path.GetFileName(item);
                    else
                        dr["FileURL"] = fileUrl + "/" + (subDir.TrimEnd('/') + "/").Replace("\\", "/") + Path.GetFileName(item);

                    dt.Rows.Add(dr);
                }
                foreach (string item in dirs)
                {
                    string str = item.Substring(item.IndexOf(rootPath) + rootPath.Length, item.Length - rootPath.Length);
                    GetFileList(str, rootPath, ref dt);
                }
            }
            catch (Exception ex)
            {

                LogHelper.WriteLog(typeof(Common)).Info(ex.StackTrace);
                result.IsOK = false;
                result.Description = ex.InnerException.Message;
            }
            return result;

        }
        #endregion

    

        #region 邮件发送
        public static Result SendMail(string mailAddress, string dateTime)
        {
            Result result = new Result();
            try
            {

                //实例化一个发送邮件类。
                MailMessage mailMessage = new MailMessage();
                //发件人邮箱地址，方法重载不同，可以根据需求自行选择。
                string platformName = ParameterAPI.GetConfig("PlatformName").ConfigValue;
                string sender= ParameterAPI.GetConfig("Sender").ConfigValue;
                mailMessage.From = new MailAddress(sender, platformName);
                //收件人邮箱地址。
                mailMessage.To.Add(mailAddress);
                mailMessage.SubjectEncoding = Encoding.UTF8;//标题格式为UTF8  
                                                            //邮件标题。
                mailMessage.Subject = platformName;
                //邮件内容。
                string registerBody = ParameterAPI.GetConfig("RegisterBody").ConfigValue;
                mailMessage.Body = registerBody;//"请点击以下链接确认注册\r\n http://172.20.8.24:80/BLL/Login/Hand.aspx" + "?";
                string content ="mailAddress="+mailAddress+ "&dateTime="+dateTime;
                mailMessage.Body += Common.Encrypt(content);
               //实例化一个SmtpClient类。
               SmtpClient client = new SmtpClient();
                //在这里我使用的是qq邮箱，所以是smtp.qq.com，如果你使用的是126邮箱，那么就是smtp.126.com。
                client.Host = "smtp.qq.com";
                //使用安全加密连接。
                client.EnableSsl = true;
                //不和请求一块发送。
                client.Port = 587;
                client.UseDefaultCredentials = false;
                //验证发件人身份(发件人的邮箱，邮箱里的生成授权码);
                string pwd = ParameterAPI.GetConfig("SendPWD").ConfigValue;
                client.Credentials = new NetworkCredential(sender, pwd);
                //发送
                client.Send(mailMessage);
                result.IsOK = true;


            }
            catch (Exception ex)
            {

                LogHelper.WriteLog(typeof(Common)).Info(ex.StackTrace);
                result.IsOK = false;
                result.Description = ex.InnerException.Message;
            }
            return result;
        }
        #endregion

        #region 加密字符串  
        /// <summary> /// 加密字符串   
        /// </summary>  
        /// <param name="str">要加密的字符串</param>  
        /// <returns>加密后的字符串</returns>  
       public static string Encrypt(string str)
        {
            string encryptKey = "Oyea";
            DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();   //实例化加/解密类对象   

            byte[] key = Encoding.Unicode.GetBytes(encryptKey); //定义字节数组，用来存储密钥    

            byte[] data = Encoding.Unicode.GetBytes(str);//定义字节数组，用来存储要加密的字符串  

            MemoryStream MStream = new MemoryStream(); //实例化内存流对象      

            //使用内存流实例化加密流对象   
            CryptoStream CStream = new CryptoStream(MStream, descsp.CreateEncryptor(key, key), CryptoStreamMode.Write);

            CStream.Write(data, 0, data.Length);  //向加密流中写入数据      

            CStream.FlushFinalBlock();              //释放加密流      

            return Convert.ToBase64String(MStream.ToArray());//返回加密后的字符串  
        }
        #endregion

        #region 解密字符串   
        /// <summary>  
        /// 解密字符串   
        /// </summary>  
        /// <param name="str">要解密的字符串</param>  
        /// <returns>解密后的字符串</returns>  
        public static string Decrypt(string str)
        {
            string encryptKey = "Oyea";
            DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();   //实例化加/解密类对象    

            byte[] key = Encoding.Unicode.GetBytes(encryptKey); //定义字节数组，用来存储密钥    

            byte[] data = Convert.FromBase64String(str);//定义字节数组，用来存储要解密的字符串  

            MemoryStream MStream = new MemoryStream(); //实例化内存流对象      

            //使用内存流实例化解密流对象       
            CryptoStream CStream = new CryptoStream(MStream, descsp.CreateDecryptor(key, key), CryptoStreamMode.Write);

            CStream.Write(data, 0, data.Length);      //向解密流中写入数据     

            CStream.FlushFinalBlock();               //释放解密流      

            return Encoding.Unicode.GetString(MStream.ToArray());       //返回解密后的字符串  
        }
        #endregion

        #region 计算MD5

        public static string GetMD5Hash(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5Hash() fail,error:" + ex.Message);
            }
        }

        public static string GetMD5Hash(byte[] bytedata)
        {
            try
            {
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(bytedata);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5Hash() fail,error:" + ex.Message);
            }
        }

        #endregion

        #region 文件保存
        public static Result FileWrite(string saveToUrl,byte[] fileData)
        {
            Result result = new Result();
            try
            {
                result.IsOK = true;
                FileStream fs = new FileStream(saveToUrl, FileMode.OpenOrCreate, FileAccess.Write);
                fs.Write(fileData, 0, fileData.Length);
                fs.Close();

            }
            catch (Exception ex)
            {

                LogHelper.WriteLog(typeof(Common)).Info(ex.StackTrace);
                result.IsOK = false;
                result.Description = ex.InnerException.Message;
            }
            return result;
        }
        #endregion


        #region AliPay
        /// <summary>
        /// 构造支付请求数据
        /// </summary>
        /// <returns>请求数据集</returns>
        public static AlipayTradePrecreateContentBuilder BuildPrecreateContent(string account, string fileCoverID)
        {
            //线上联调时，请输入真实的外部订单号。
            //string out_trade_no = "";
            //if (String.IsNullOrEmpty(WIDout_request_no.Text.Trim()))
            AlipayTradePrecreateContentBuilder builder = new AlipayTradePrecreateContentBuilder();
            //{
            try
            {
                string out_trade_no = System.DateTime.Now.ToString("yyyyMMddHHmmss") + "0000" + (new Random()).Next(1, 10000).ToString();


                //收款账号
                builder.seller_id = Config.pid;
                //订单编号
                builder.out_trade_no = out_trade_no;

                PCBEntities pCBEntities = new PCBEntities();
                Guid g = new Guid(fileCoverID);

                //订单总金额
                builder.total_amount = pCBEntities.PCB_FileCoverTB.FirstOrDefault(p => p.FileCoverID == g).Price;
                //参与优惠计算的金额
                //builder.discountable_amount = "";
                //不参与优惠计算的金额
                //builder.undiscountable_amount = "";
                //订单名称
                builder.subject = ParameterAPI.GetConfig("Subject").ConfigValue;// "xxxx";// WIDsubject.Text.Trim();
                //自定义超时时间
                builder.timeout_express = ParameterAPI.GetConfig("Timeout").ConfigValue;
                //订单描述
                builder.body = ParameterAPI.GetConfig("BuilderBody").ConfigValue;
                //门店编号，很重要的参数，可以用作之后的营销
                builder.store_id = ParameterAPI.GetConfig("StoreID").ConfigValue;
                //操作员编号，很重要的参数，可以用作之后的营销
                builder.operator_id = ParameterAPI.GetConfig("OperatorID").ConfigValue;

                //传入商品信息详情
                List<GoodsInfo> gList = new List<GoodsInfo>();
                GoodsInfo goods = new GoodsInfo();
                goods.goods_id = "goods id";
                goods.goods_name = "goods name";
                goods.price = "0.01";
                goods.quantity = "1";
                gList.Add(goods);
                builder.goods_detail = gList;

                //系统商接入可以填此参数用作返佣
                //ExtendParams exParam = new ExtendParams();
                //exParam.sysServiceProviderId = "20880000000000";
                //builder.extendParams = exParam;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.InnerException.Message);
            }


            return builder;

        }

        #endregion

        #region AliPay订单

        /// <summary>
        /// 预下单记录
        /// </summary>
        /// <returns></returns>
        public static Result RecordPreRenewal(string account, string fileCoverId, string tradeNO)
        {
            Result result = new Result();
            try
            {
                PCBEntities pCBEntities = new PCBEntities();

                PCB_OrderTB orderTB = new PCB_OrderTB();
                orderTB.OrderID = System.Guid.NewGuid();
                orderTB.CreateAccount = account;
                orderTB.CreateDateTime = DateTime.Now;
                Guid fid = new Guid(fileCoverId);
                orderTB.FileCoverID = fid;
                orderTB.IsPay = false;
                orderTB.OrderNumber = tradeNO;
                orderTB.OrderPrice = pCBEntities.PCB_FileCoverTB.FirstOrDefault(p => p.FileCoverID == fid).Price;
                orderTB.UpdateDateTime = DateTime.Now;


                pCBEntities.AddToPCB_OrderTB(orderTB);
                result.IsOK = Convert.ToBoolean(pCBEntities.SaveChanges());
            }
            catch (Exception ex)
            {

                LogHelper.WriteLog(typeof(Common)).Info(ex.StackTrace);
                result.IsOK = false;
                result.Description = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 更新订单表记录
        /// </summary>
        /// <param name="account"></param>
        /// <param name="tradeNO"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static Result UpdateOrderTB(string account, string tradeNO)
        {
            Result result = new Result();
            try
            {
                result.IsOK = true;
                result.StateCodeID = 0;
                PCBEntities pCBEntities = new PCBEntities();
                PCB_OrderTB PCBOrderTB = pCBEntities.PCB_OrderTB.FirstOrDefault(p => p.CreateAccount == account && p.OrderNumber == tradeNO);
                if (PCBOrderTB != null)
                {
                    PCBOrderTB.IsPay = true;
                    PCBOrderTB.UpdateDateTime = DateTime.Now;

                    pCBEntities.Refresh(System.Data.Objects.RefreshMode.ClientWins, PCBOrderTB);

                    result.IsOK = Convert.ToBoolean(pCBEntities.SaveChanges());
                }
            }
            catch (Exception ex)
            {

                LogHelper.WriteLog(typeof(Common)).Info(ex.StackTrace);
                result.IsOK = false;
                result.Description = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 删除订单表记录
        /// </summary>
        /// <param name="account"></param>
        /// <param name="tradeNO"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static Result DelectOrderTB(string account, string tradeNO)
        {
            Result result = new Result();
            try
            {
                result.IsOK = true;
                result.StateCodeID = 0;
                PCBEntities pCBEntities = new PCBEntities();
                PCB_OrderTB PCBOrderTB = pCBEntities.PCB_OrderTB.FirstOrDefault(p => p.CreateAccount == account && p.OrderNumber == tradeNO && p.IsPay == false);
                if (PCBOrderTB != null)
                {
                    pCBEntities.DeleteObject(PCBOrderTB);
                    pCBEntities.SaveChanges();
                }
                else
                {
                    result.IsOK = false;
                }

            }
            catch (Exception ex)
            {

                LogHelper.WriteLog(typeof(Common)).Info(ex.StackTrace);
                result.IsOK = false;
                result.Description = ex.Message;
            }
            return result;
        }
        #endregion


        ///// <summary>
        ///// 续期异常检测
        ///// </summary>
        ///// <param name="account"></param>
        ///// <returns></returns>
        //public static Result CheckRenewal(string account)
        //{
        //    Result result = new Result();
        //    try
        //    {
        //        result.IsOK = true;
        //        result.StateCodeID = 0;
        //        PMSEntities pMSEntities = new PMSEntities();
        //        Sys_RenewalTB renewalTB = pMSEntities.Sys_RenewalTB.FirstOrDefault(p => p.Account == account && p.StateCode == false);
        //        if (renewalTB == null)
        //        {

        //            return result;
        //        }
        //        IAlipayTradeService serviceClient = F2FBiz.CreateClientInstance(Config.serverUrl, Config.appId, Config.merchant_private_key, Config.version, Config.sign_type, Config.alipay_public_key, Config.charset);
        //        AlipayF2FQueryResult queryResult = serviceClient.tradeQuery(renewalTB.TradeNO);
        //        if (queryResult != null)
        //        {
        //            if (queryResult.Status == ResultEnum.SUCCESS)
        //            {
        //                result = Common.UpdateRenewalStatus(account, renewalTB.TradeNO);
        //                if (!result.IsOK)
        //                {
        //                    return result;
        //                }
        //                result.StateCodeID = 1;
        //                return result;
        //            }
        //            else
        //            {
        //                pMSEntities.DeleteObject(renewalTB);
        //                pMSEntities.SaveChanges();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        LogHelper.WriteLog(typeof(Common)).Info(ex.StackTrace);
        //        result.IsOK = false;
        //        result.Description = ex.InnerException.Message;
        //    }
        //    return result;
        //}


    }
}