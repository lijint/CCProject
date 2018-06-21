
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using DownLoad.BLL.Common;
using DownLoad.Entity;
using DownLoad.Model;
using Newtonsoft.Json;
using System.Net;
using System.Web.Services.Protocols;

namespace DownLoad.BLL.Login
{
    /// <summary>
    /// LoginWebService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
     [System.Web.Script.Services.ScriptService]

    public class LoginWebService : System.Web.Services.WebService
    {

        #region 工程师登录流程
        #region 修改密码
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassword">新密码</param>
        /// <returns></returns>
        [WebMethod]
        public Result ChangePassword(string account, string oldPassword, string newPassword)
        {
            Result result = new Result();
            try
            {
                PCBEntities pCBEntities = new PCBEntities();
                PCB_EngineerInfo engineerInfo = pCBEntities.PCB_EngineerInfo.FirstOrDefault(p => (p.Account == account || p.MailAddress == account) && p.StateCode == true);
                if (engineerInfo == null || engineerInfo == default(PCB_EngineerInfo))
                {
                    result.IsOK = false;
                    result.Description = "输入的用户" + account + "不存在";
                    return result;
                }
                if (oldPassword != engineerInfo.Password)
                {
                    result.IsOK = false;
                    result.Description = "输入的旧密码有误";
                    return result;
                }
                if (string.IsNullOrEmpty(newPassword))
                {
                    result.IsOK = false;
                    result.Description = "输入的新密码不能为空";
                    return result;
                }

                engineerInfo.Password = newPassword;
                engineerInfo.UpdateDateTime = DateTime.Now;
                pCBEntities.Refresh(System.Data.Objects.RefreshMode.ClientWins, engineerInfo);
                result.IsOK = Convert.ToBoolean(pCBEntities.SaveChanges());
                if (!result.IsOK)
                {
                    result.Description = "修改密码失败";
                    return result;
                }
                result.Description = "账号" + account + "的密码修改成功";
            }
            catch (Exception ex)
            {

                LogHelper.WriteLog(GetType()).Info(ex.StackTrace);
                result.IsOK = false;
                result.Description = ex.InnerException.Message;
            }
            return result;
        }
        #endregion

        #region 编辑账号
        [WebMethod]
        public Result EditeAccountInfo(string account, string password, string accountName, string phoneNo, string weChatAccount)
        {

            Result result = new Result();
            try
            {
                PCBEntities pCBEntities = new PCBEntities();
                PCB_EngineerInfo pCB_EngineerInfo = pCBEntities.PCB_EngineerInfo.FirstOrDefault(p => p.Account == account && p.Password == password && p.StateCode == true);
                if (pCB_EngineerInfo == null)
                {
                    result.IsOK = false;
                    result.Description = "没有该账号信息";
                    return result;

                }
                if (!string.IsNullOrEmpty(accountName))
                {
                    pCB_EngineerInfo.AccountName = accountName;
                }
                if (!string.IsNullOrEmpty(phoneNo))
                {
                    pCB_EngineerInfo.PhoneNo = phoneNo;
                }
                if (!string.IsNullOrEmpty(weChatAccount))
                {
                    pCB_EngineerInfo.WeChatAccount = weChatAccount;
                }


                pCB_EngineerInfo.UpdateDateTime = DateTime.Now;

                pCBEntities.Refresh(System.Data.Objects.RefreshMode.ClientWins, pCB_EngineerInfo);
                result.IsOK = Convert.ToBoolean(pCBEntities.SaveChanges());
                if (!result.IsOK)
                {
                    result.Description = "保存异常";
                    return result;
                }

            }
            catch (Exception ex)
            {

                LogHelper.WriteLog(GetType()).Info(ex.StackTrace);
                result.IsOK = false;
                result.Description = ex.InnerException.Message;
            }
            return result;
        }
        #endregion

        #region 通过账号或者邮箱账号获取用户信息
        [WebMethod]
        public Result GetEngineerInfoByAccount(string account)
        {

            Result result = new Result();
            try
            {

                PCBEntities pCBEntities = new PCBEntities();
                if (string.IsNullOrEmpty(account))
                {
                    result.IsOK = false;
                    result.Description = "账号不能为空";
                    return result;
                }

                    var info = pCBEntities.PCB_EngineerInfo.Where(p=>p.Account==account||p.MailAddress==account).Select(p => new { p.Account, p.AccountName, p.CreateDateTime, p.PhoneNo, p.MailAddress, p.WeChatAccount });
                    result.IsOK = true;
                    result.ExtData = JsonConvert.SerializeObject(info);


                
              

            }
            catch (Exception ex)
            {

                LogHelper.WriteLog(GetType()).Info(ex.StackTrace);
                result.IsOK = false;
                result.Description = ex.InnerException.Message;
            }
            return result;
        }
        #endregion

        #region 账号注册
        /// <summary>
        /// 账号注册
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="accountName">账号名称</param>
        /// <param name="password">密码</param>
        /// <param name="phoneNo">手机号码</param>
        /// <param name="mailAddress">邮箱地址</param>
        /// <param name="weChatAccount">微信号</param>
        /// <returns></returns>
        [WebMethod]
        public Result CreateEngineer(string account, string accountName, string password, string phoneNo, string mailAddress, string weChatAccount)
        {
          //  LogHelper.WriteLog(GetType()).Info("CreateEngineer");
            Result result = new Result();
            try
            {
                result.IsOK = true;
                if (string.IsNullOrEmpty(account))
                {
                    result.IsOK = false;
                    result.Description = "注册账号不能为空";
                    return result;
                }
                if (string.IsNullOrEmpty(accountName))
                {
                    result.IsOK = false;
                    result.Description = "注册账号名称不能为空";
                    return result;
                }
                if (string.IsNullOrEmpty(password))
                {
                    result.IsOK = false;
                    result.Description = "注册账号密码不能为空";
                    return result;
                }
                if (string.IsNullOrEmpty(mailAddress))
                {
                    result.IsOK = false;
                    result.Description = "注册邮箱账号不能为空";
                    return result;
                }
                if (string.IsNullOrEmpty(weChatAccount))
                {
                    result.IsOK = false;
                    result.Description = "注册微信账号不能为空";
                    return result;
                }
                PCBEntities pCBEntities = new PCBEntities();
                PCB_EngineerInfo engineerInfo = pCBEntities.PCB_EngineerInfo.FirstOrDefault(p => p.Account == account || p.MailAddress == mailAddress);
                if (engineerInfo != null || engineerInfo != default(PCB_EngineerInfo))
                {
                    if (engineerInfo.StateCode)
                    {
                        if (engineerInfo.Account == account)
                        {
                            result.Description = "用户" + account + "已经存在";
                        }
                        if (engineerInfo.MailAddress == mailAddress)
                        {
                            result.Description = "邮箱" + mailAddress + "已经存在";
                        }
                        result.IsOK = false;
                        return result;
                    }
                    else
                    {
                        TimeSpan UseTime = DateTime.Now - engineerInfo.CreateDateTime;
                        if (UseTime.TotalSeconds <= 60)
                        {
                            result.Description = "已经提交申请，请勿重复操作";
                            result.IsOK = false;
                            return result;
                        }
                        else //超过60s删除预注册信息
                        {
                            pCBEntities.DeleteObject(engineerInfo);
                        }

                    }

                }
                engineerInfo = new PCB_EngineerInfo();
                engineerInfo.EngineerID = System.Guid.NewGuid();
                engineerInfo.Account = account;
                engineerInfo.Password = password;
                engineerInfo.AccountName = accountName;
                engineerInfo.MailAddress = mailAddress;
                engineerInfo.PhoneNo = phoneNo;
                engineerInfo.WeChatAccount = weChatAccount;
                engineerInfo.CreateDateTime = DateTime.Now;
                engineerInfo.StateCode = false;
                pCBEntities.AddToPCB_EngineerInfo(engineerInfo);

                result = Common.Common.SendMail(mailAddress, engineerInfo.CreateDateTime.ToString());
                if (!result.IsOK)
                {
                    return result;
                }
                result.IsOK = Convert.ToBoolean(pCBEntities.SaveChanges());
                if (!result.IsOK)
                {
                    result.Description = "注册账号失败";
                    return result;
                }


            }
            catch (Exception ex)
            {

                LogHelper.WriteLog(GetType()).Info(ex.StackTrace);
                result.IsOK = false;
                result.Description = ex.InnerException.Message;
            }
            return result;
        }
        #endregion
        #endregion

        #region 账号登录
        /// <summary>
        /// 账号登录
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="password">密码</param>
        /// <param name="type">类型（1：工程师；2厂商）</param>
        /// <returns></returns>
        [WebMethod]
        public Result Login(string account, string password,string type)
        {
            Result result = new Result();
            try
            {
                result.StateCodeID = 0;
                PCBEntities pCBEntities = new PCBEntities();
                PCB_EngineerInfo engineerInfo = pCBEntities.PCB_EngineerInfo.FirstOrDefault(p => (p.Account == account ||p.MailAddress==account)&&(p.StateCode == true));
                if (engineerInfo == null || engineerInfo == default(PCB_EngineerInfo))
                {
                    result.IsOK = false;
                    result.Description = "输入的用户" + account + "不存在";
                    return result;
                }
                if (password != engineerInfo.Password)
                {
                    result.IsOK = false;
                    result.Description = "输入的密码有误";
                    return result;
                }
                engineerInfo.UpdateDateTime = DateTime.Now;
                pCBEntities.Refresh(System.Data.Objects.RefreshMode.ClientWins, engineerInfo);
                result.IsOK = Convert.ToBoolean(pCBEntities.SaveChanges());
                if (!result.IsOK)
                {
                    result.Description = "登录异常";
                    return result;
                }

                result.Description = "登录成功";
             
            }
            catch (Exception ex)
            {

                LogHelper.WriteLog(GetType()).Info(ex.StackTrace);
                result.IsOK = false;
                result.Description = ex.InnerException.Message;
            }
            return result;

        }
        #endregion

        #region 系统参数

        #region 设置系统参数
        /// <summary>
        /// 设置系统参数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        [WebMethod]
        public Result SetSysParams(string key, string value, string remark)
        {
            Result result = new Result();
            result.StateCodeID = 0;
            result.IsOK = false;
            try
            {
                PCBEntities pCBEntities = new PCBEntities();
                //PCB_ConfigTB configSys = ParameterAPI.GetConfig(key);
                PCB_ConfigTB configSys = pCBEntities.PCB_ConfigTB.FirstOrDefault(p => p.ConfigCode == key);
                if (configSys == null)
                {
                    result.IsOK = false;
                    result.Description = "无对应项";
                    return result;
                }
                configSys.ConfigValue = value;
                configSys.Remark = remark;
                pCBEntities.Refresh(System.Data.Objects.RefreshMode.ClientWins, configSys);
                result.IsOK = Convert.ToBoolean(pCBEntities.SaveChanges());
                //result = Common.Common.UpdateConfigTB(key, value, remark);
                if (result.IsOK)
                    result.Description = "设置成功";
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

        #region 获取系统参数
        /// <summary>
        /// 获取系统参数
        /// </summary>
        /// <param name="strSysParamsKey">系统参数key值</param>
        /// <returns></returns>
        [WebMethod]
        public Result GetSysParams(string strSysParamsKey)
        {
            Result result = new Result();
            result.StateCodeID = 0;
            result.IsOK = false;
            try
            {
                PCBEntities pcbEntities = new PCBEntities();

                collcetDictionary<string, string, string> configTB = new collcetDictionary<string, string, string>();
                if (string.IsNullOrEmpty(strSysParamsKey))
                {
                    configTB.Clear();
                    foreach (PCB_ConfigTB c in pcbEntities.PCB_ConfigTB)
                    {
                        configTB.Add(c.ConfigCode, c.ConfigValue, c.Remark);
                    }
                    result.ExtData = JsonConvert.SerializeObject(configTB.collectList);
                    result.IsOK = true;
                    result.StateCodeID = 1;
                    result.Description = "查询成功";

                }
                else
                {
                    PCB_ConfigTB configSysTB = pcbEntities.PCB_ConfigTB.FirstOrDefault(p => p.ConfigCode == strSysParamsKey);
                    if (configSysTB == null)
                    {
                        result.IsOK = false;
                        result.Description = "无对应项";
                        return result;
                    }
                    configTB.Clear();
                    configTB.Add(configSysTB.ConfigCode, configSysTB.ConfigValue, configSysTB.Remark);
                    result.ExtData = JsonConvert.SerializeObject(configTB.collectList);
                    result.IsOK = true;
                    result.StateCodeID = 1;
                    result.Description = "查询成功";
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

        #region 配置参数接口相关类
        private class collcetDictionary<T1, T2, T3>
        {
            private List<collcetCell<T1, T2, T3>> cList;
            public List<collcetCell<T1, T2, T3>> collectList
            {
                get { return cList; }
            }
            public collcetDictionary()
            {
                cList = new List<collcetCell<T1, T2, T3>>();
            }
            public void Add(T1 a1, T2 b1, T3 c1)
            {
                collcetCell<T1, T2, T3> c = new collcetCell<T1, T2, T3>(a1, b1, c1);
                cList.Add(c);
            }
            public void Clear()
            {
                cList.Clear();
            }
        }

        private class collcetCell<T1, T2, T3>
        {
            public T1 key;
            public T2 value;
            public T3 remark;
            public collcetCell(T1 k, T2 v, T3 r)
            {
                key = k;
                value = v;
                remark = r;
            }
        }

        #endregion

        #endregion

    }
}
