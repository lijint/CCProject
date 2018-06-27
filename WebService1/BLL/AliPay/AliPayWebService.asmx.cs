using System;
using System.Linq;
using System.Web.Services;
using Aop.Api.Response;
using Com.Alipay;
using Com.Alipay.Business;
using Com.Alipay.Domain;
using Com.Alipay.Model;
using DownLoad.BLL.Common;
using DownLoad.Entity;
using DownLoad.Model;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DownLoad.BLL.AliPay
{
    /// <summary>
    /// AliPayWebService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class AliPayWebService : System.Web.Services.WebService
    {

        #region 预下单
        /// <summary>
        /// 预下单
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public Result PreCreate(string account, string fileCoverId)
        {
            Result result = new Result();

            try
            {
                IAlipayTradeService serviceClient = F2FBiz.CreateClientInstance(Config.serverUrl, Config.appId, Config.merchant_private_key, Config.version, Config.sign_type, Config.alipay_public_key, Config.charset);
                AlipayTradePrecreateContentBuilder builder = Common.Common.BuildPrecreateContent(account, fileCoverId);
                string out_trade_no = builder.out_trade_no;
                AlipayF2FPrecreateResult precreateResult = serviceClient.tradePrecreate(builder);

                switch (precreateResult.Status)
                {
                    case ResultEnum.SUCCESS:
                        result.IsOK = true;
                        result = Common.Common.RecordPreRenewal(account, fileCoverId, out_trade_no);
                        break;
                    case ResultEnum.FAILED:
                        result.Description = precreateResult.response.Body;
                        result.IsOK = false;
                        break;

                    case ResultEnum.UNKNOWN:

                        if (precreateResult.response == null)
                        {
                            result.Description = "配置或网络异常，请检查后重试";
                        }
                        else
                        {
                            result.Description = precreateResult.response.Body;// "系统异常，请更新外部订单后重新发起请求";
                        }
                        result.IsOK = false;
                        break;
                }
                if (!result.IsOK)
                {
                    return result;
                }
                result.ExtData = precreateResult.response.QrCode;
                result.Description = "预下单成功";
                //  result.SucceedDescription = out_trade_no;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(GetType()).Info(ex.StackTrace);
                result.IsOK = false;
                result.Description = ex.Message;
            }
            return result;
        }
        #endregion

        #region 查询订单
        /// <summary>
        /// 查询订单
        /// </summary>
        /// <param name="account"></param>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [WebMethod]
        public Result QueryByOrderNo(string account, string orderNo)
        {
            Result result = new Result();
            try
            {
                result.IsOK = true;
                result.StateCodeID = 0;
                IAlipayTradeService serviceClient = F2FBiz.CreateClientInstance(Config.serverUrl, Config.appId, Config.merchant_private_key, Config.version, Config.sign_type, Config.alipay_public_key, Config.charset);
                AlipayF2FQueryResult queryResult = serviceClient.tradeQuery(orderNo);
                if (queryResult != null)
                {
                    LogHelper.WriteLog(GetType()).Info(queryResult.response.OutTradeNo);
                    switch (queryResult.Status)
                    {
                        case ResultEnum.SUCCESS:
                            {
                                result = Common.Common.UpdateOrderTB(account, orderNo);
                                if (!result.IsOK)
                                {
                                    return result;
                                }
                                result.StateCodeID = 1;
                                return result;
                            }
                        case ResultEnum.FAILED:
                            {
                                result.IsOK = true;
                                result.Description = "未付款";
                                return result;
                            }
                        case ResultEnum.UNKNOWN:
                            if (queryResult.response == null)
                            {
                                //result = "网络异常，请检查网络配置";
                                result.IsOK = false;
                                result.Description = "配置或网络异常，请检查";
                                return result;
                            }
                            else
                            {
                                result.IsOK = false;
                                result.Description = "系统异常，请重试";
                                return result;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(GetType()).Info(ex.StackTrace);
                result.IsOK = false;
                result.Description = ex.Message;
            }
            return result;
        }
        #endregion

        #region 取消订单
        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="orderNo">订单号</param>
        /// <returns></returns>
        [WebMethod]
        public Result CancelPreOrder(string account, string orderNo)
        {
            Result result = new Result();
            try
            {
                result.IsOK = true;

                IAlipayTradeService serviceClient = F2FBiz.CreateClientInstance(Config.serverUrl, Config.appId, Config.merchant_private_key, Config.version, Config.sign_type, Config.alipay_public_key, Config.charset);
                AlipayF2FQueryResult queryResult = serviceClient.tradeQuery(orderNo);
                if (queryResult != null)
                {
                    if (queryResult.Status == ResultEnum.SUCCESS)
                    {
                        result = Common.Common.UpdateOrderTB(account, orderNo);
                        if (!result.IsOK)
                        {
                            return result;
                        }
                        result.Description = "用户已付款";
                        result.StateCodeID = 0;
                    }
                    else
                    {
                        //防止扫码之后退出续期
                        AlipayTradeCancelResponse cancelResult = serviceClient.tradeCancelResponse(orderNo);
                        if (cancelResult.Action == "close")
                        {
                            result = Common.Common.DelectOrderTB(account, orderNo);
                            if (!result.IsOK)
                            {
                                return result;
                            }
                            result.Description = "取消订单成功";
                            result.StateCodeID = 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(GetType()).Info(ex.StackTrace);
                result.IsOK = false;
                result.Description = ex.Message;
            }
            return result;
        }
        #endregion

        #region 校验查询

        /// <summary>
        /// 校验查询，用户点击封面时调用
        /// 1.校验封面是否有设置金额，若未设置，返回下载地址；
        /// 2.若有设置金额，校验是否存在预下单记录，若有且未支付，则删除返回金额，若有且已支付，则返回下载地址；
        /// 3.若有设置金额，校验是否存在预下单记录，若没有预下单记录，则返回金额；
        /// </summary>
        /// <param name="filecoverID"></param>
        /// <returns></returns>
        [WebMethod]
        public Result CheckAndQueryFilecover(string filecoverID)
        {
            Result result = new Result();
            try
            {
                result.IsOK = true;

                PCBEntities pcbEntities = new PCBEntities();
                Guid fid = new Guid(filecoverID);
                PCB_FileCoverTB pcbfilecoverTB = pcbEntities.PCB_FileCoverTB.FirstOrDefault(p => p.FileCoverID == fid);
                if (pcbfilecoverTB == null)
                {
                    result.IsOK = false;
                    result.Description = "未查询到指定数据";
                    return result;
                }
                List<AccessFile> accessFileList = new List<AccessFile>();
                foreach (PCB_AccessFileTB pa in pcbEntities.PCB_AccessFileTB)
                {
                    if (pa.FileCoverID == fid)
                    {
                        AccessFile af = new AccessFile();
                        af.AccessFileName = pa.AccessFileName;
                        af.AccessFileURL = pa.AccessFileURL;
                        af.FileExtension = pa.FileExtension;
                        af.FileSize = pa.FileSize;
                        af.FileMD5 = pa.FileMD5;
                        accessFileList.Add(af);
                    }
                }

                decimal filecoverprice;
                if(decimal.TryParse(pcbfilecoverTB.Price,out filecoverprice))
                {
                    if(filecoverprice>0)  
                    {
                        //金额>0，查询预下单记录
                        LogHelper.WriteLog(GetType()).Info(filecoverprice.ToString());

                        PCB_OrderTB pcborderTB = pcbEntities.PCB_OrderTB.FirstOrDefault(p => p.FileCoverID == fid);
                        if(pcborderTB!=null)
                        {
                            //LogHelper.WriteLog(GetType()).Info(pcborderTB);
                            //有记录
                            if(pcborderTB.IsPay)
                            {
                                result.Description = "文件已支付，返回数据类型为：文件下载地址";

                                result.ExtData = JsonConvert.SerializeObject(accessFileList);
                            }
                            else
                            {
                                result.Description = "文件需支付，返回数据类型为：文件价格";
                                result.ExtData = pcbfilecoverTB.Price;
                                pcbEntities.DeleteObject(pcborderTB);
                                pcbEntities.SaveChanges();
                            }

                        }
                        else
                        {
                            //LogHelper.WriteLog(GetType()).Info(pcborderTB);
                            result.ExtData = pcbfilecoverTB.Price;
                            result.Description = "文件需支付，返回数据类型为：文件价格";
                        }

                    }
                    else
                    {
                        //金额==0，返回下载地址
                        LogHelper.WriteLog(GetType()).Info(filecoverprice.ToString());
                        result.ExtData = JsonConvert.SerializeObject(accessFileList);
                        result.Description = "文件无需支付，返回数据类型为：文件下载地址";

                    }
                }
                else
                {
                    throw new Exception("封面金额转换错误，pcbfilecoverTB.Price : " + pcbfilecoverTB.Price);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(GetType()).Info(ex.StackTrace);
                result.IsOK = false;
                result.Description = ex.Message;
            }
            return result;
        }

        private class AccessFile
        {
            public string AccessFileName;
            public string AccessFileURL;
            public string FileExtension;
            public string FileSize;
            public string FileMD5;
        }

        #endregion


    }
}
