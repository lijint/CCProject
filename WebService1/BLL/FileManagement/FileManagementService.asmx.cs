using DownLoad.BLL.Common;
using DownLoad.Entity;
using DownLoad.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace DownLoad.BLL.FileManagement
{
    /// <summary>
    /// FileManagementService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
     [System.Web.Script.Services.ScriptService]
    public class FileManagementService : System.Web.Services.WebService
    {
        /// <summary>
        /// 封面上传
        /// </summary>
        /// <param name="fileData"></param>
        /// <param name="account"></param>
        /// <param name="fileExtension"></param>
        /// <returns></returns>
        [WebMethod(Description = "封面上传")]
        public Result UploadFileCover(Byte[] fileData,string account,string fileExtension,string fileCoverName)
        {
            Result result = new Result();
            try
            {



                //byte[] coverPhoto = Convert.FromBase64String(fileData);
                //MemoryStream ms = new MemoryStream(coverPhoto);
                //Bitmap bmp = new Bitmap(ms);

                PCB_FileCoverTB pCB_FileCoverTB = new PCB_FileCoverTB();
                pCB_FileCoverTB.FileCoverID = System.Guid.NewGuid();
                pCB_FileCoverTB.FileCoverName = fileCoverName;
                pCB_FileCoverTB.FileMD5 =Common.Common.GetMD5Hash(fileData);
                pCB_FileCoverTB.FileExtension = fileExtension;
                pCB_FileCoverTB.FileSize = fileData.Length.ToString();
                pCB_FileCoverTB.IsPublish = false;
                pCB_FileCoverTB.FileCoverURL = ParameterAPI.GetConfig("DowLoadFileURL").ConfigValue + "//" + account + "//" + pCB_FileCoverTB.FileCoverID.ToString() + "//" + pCB_FileCoverTB.FileCoverName + "."+ fileExtension;
                pCB_FileCoverTB.CreateAccount = account;
                pCB_FileCoverTB.CreateDateTime = DateTime.Now;
                

                String dir = @ParameterAPI.GetConfig("FileURL").ConfigValue + @"\\" + account + @"\\" + pCB_FileCoverTB.FileCoverID.ToString(); 
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }      
                string saveToUrl = dir+@"\\"+ pCB_FileCoverTB.FileCoverName + "."+ fileExtension;
                result = Common.Common.FileWrite(saveToUrl, fileData);
                if (!result.IsOK)
                {
                    return result;
                }
                //bmp.Save(saveDir);
                //bmp.Dispose();
                //ms.Close();
                PCBEntities pCBEntities = new PCBEntities();
                pCBEntities.AddToPCB_FileCoverTB(pCB_FileCoverTB);
                result.IsOK = Convert.ToBoolean(pCBEntities.SaveChanges());
                if (!result.IsOK)
                {
                    Directory.Delete(dir,true);
                    result.Description = "上传失败";
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

        /// <summary>
        /// 附件上传
        /// </summary>
        /// <param name="accessFile"></param>
        /// <param name="fileCoverID"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        [WebMethod(Description = "附件上传")]
        public Result UploadAccessFile(byte[] accessFile, string fileCoverID,string account,string fileExtension,string accessFileName)
        {
            Result result = new Result();
            string saveToUrl = @ParameterAPI.GetConfig("FileURL").ConfigValue + @"\\" + account + @"\\" + fileCoverID + @"\\" + accessFileName + "." + fileExtension;
            try
            {
                Guid g = new Guid(fileCoverID);
                PCBEntities pCBEntities = new PCBEntities();
                int count = pCBEntities.PCB_FileCoverTB.Count<PCB_FileCoverTB>(p => p.FileCoverID == g);
                if (count<=0)
                {
                    result.IsOK = false;
                    result.Description = "封面ID不存在";
                    return result;
                }
                PCB_AccessFileTB pCB_AccessFileTB = pCBEntities.PCB_AccessFileTB.FirstOrDefault(p => p.CreateAccount == account && p.FileExtension == fileExtension&&p.FileCoverID==g&&p.AccessFileName==accessFileName);// pCBEntities.PCB_AccessFileTB.FirstOrDefault(p => p.FileCoverID == new Guid(fileCoverID) && p.CreateAccount == account && p.FileExtension == fileExtension);
                if (pCB_AccessFileTB!=null || pCB_AccessFileTB!=default(PCB_AccessFileTB))
                {
                    result.IsOK = false;
                    result.Description = "上传的文件已经存在";
                    return result;
                }
                //string fileName=Guid.NewGuid().ToString();
              
                 result = Common.Common.FileWrite(saveToUrl, accessFile);
                if (!result.IsOK)
                {
                    return result;

                }
                pCB_AccessFileTB = new PCB_AccessFileTB();
                pCB_AccessFileTB.AccessFileID = Guid.NewGuid();
                pCB_AccessFileTB.AccessFileName = accessFileName;
                pCB_AccessFileTB.FileCoverID =new Guid(fileCoverID);
                pCB_AccessFileTB.AccessFileURL= ParameterAPI.GetConfig("DowLoadFileURL").ConfigValue + "//" + account + "//" + fileCoverID + "//" + accessFileName + "." + fileExtension;
                pCB_AccessFileTB.FileExtension = fileExtension;
                pCB_AccessFileTB.CreateAccount = account;
                pCB_AccessFileTB.FileMD5 = Common.Common.GetMD5Hash(accessFile);
                pCB_AccessFileTB.FileSize = accessFile.Length.ToString();
                pCB_AccessFileTB.CreateDateTime = DateTime.Now;
                pCBEntities.AddToPCB_AccessFileTB(pCB_AccessFileTB);
                result.IsOK = Convert.ToBoolean(pCBEntities.SaveChanges());
                if (!result.IsOK)
                {
                    File.Delete(saveToUrl);
                    result.Description = "上传失败";
                    return result;
                }
                //Bitmap bmp = new Bitmap(Imagefilename);

                //MemoryStream ms = new MemoryStream();
                //bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                //byte[] arr = new byte[ms.Length];
                //ms.Position = 0;
                //ms.Read(arr, 0, (int)ms.Length);
                //ms.Close();
                //string r = Convert.ToBase64String(arr);
                //Result ret = UpdateFileCover(r, "chenc");
            }
            catch (Exception ex)
            {
                File.Delete(saveToUrl);
                LogHelper.WriteLog(GetType()).Info(ex.StackTrace);
                result.IsOK = false;
                result.Description = ex.InnerException.Message;
            }
            return result;


        }

        /// <summary>
        /// 文件发布
        /// </summary>
        /// <param name="fileCoverID"></param>
        /// <param name="account"></param>
        /// <param name="setPrice"></param>
        /// <param name="publishStatus"></param>
        /// <returns></returns>
        [WebMethod(Description = "文件发布")]
        public Result FileIsPublish(string fileCoverID,string account,string setPrice,int publishStatus)
        {
            Result result = new Result();
            try
            {
                Guid g = new Guid(fileCoverID);
                PCBEntities pCBEntities = new PCBEntities();
                PCB_FileCoverTB pCB_FileCoverTB = pCBEntities.PCB_FileCoverTB.FirstOrDefault(p => p.FileCoverID == g && p.CreateAccount == account);
                if (pCB_FileCoverTB==null|| pCB_FileCoverTB==default(PCB_FileCoverTB))
                {
                    result.IsOK = false;
                    result.Description = "找不到该文件信息";
                    return result;

                }
                pCB_FileCoverTB.Price = setPrice;
                pCB_FileCoverTB.IsPublish =Convert.ToBoolean(publishStatus);
                pCB_FileCoverTB.UpdateDateTime = DateTime.Now;
                pCBEntities.Refresh(System.Data.Objects.RefreshMode.ClientWins,pCB_FileCoverTB);
                result.IsOK = Convert.ToBoolean(pCBEntities.SaveChanges());
                if (!result.IsOK)
                {
                 
                    result.Description = "发布失败";
                    return result;
                }
                result.Description = "发布成功";
            }
            catch (Exception ex)
            {
             
                LogHelper.WriteLog(GetType()).Info(ex.StackTrace);
                result.IsOK = false;
                result.Description = ex.InnerException.Message;
            }
            return result;
        }

        /// <summary>
        /// 编辑封面
        /// </summary>
        /// <param name="fileCoverID"></param>
        /// <param name="fileExtension"></param>
        /// <param name="fileCoverName"></param>
        /// <param name="account"></param>
        /// <param name="newFileData"></param>
        /// <returns></returns>
        [WebMethod(Description = "编辑封面")]
        public Result EditFileCover(string fileCoverID, string fileExtension, string fileCoverName, string account, Byte[] newFileData)
        {
            string saveToUrl = "";
            Result result = new Result();
            try
            {
                Guid g = new Guid(fileCoverID);
                PCBEntities pCBEntities = new PCBEntities();
                PCB_FileCoverTB pCB_FileCoverTB = pCBEntities.PCB_FileCoverTB.FirstOrDefault(p => p.FileCoverID == g && p.CreateAccount == account);
                if (pCB_FileCoverTB == null || pCB_FileCoverTB == default(PCB_FileCoverTB))
                {
                    result.IsOK = false;
                    result.Description = "找不到该文件信息";
                    return result;

                }
                String dir = @ParameterAPI.GetConfig("FileURL").ConfigValue + @"\\" + account + @"\\" + fileCoverID;
                string oldUrl = dir + @"\\" + pCB_FileCoverTB.FileCoverName + "." + pCB_FileCoverTB.FileExtension;
                if (string.IsNullOrEmpty(fileCoverName))
                {
                    pCB_FileCoverTB.FileCoverName = fileCoverName;
                }
                if (newFileData.Length>0)
                {
                    pCB_FileCoverTB.FileSize = newFileData.Length.ToString();
                    pCB_FileCoverTB.FileMD5 = Common.Common.GetMD5Hash(newFileData);
                    pCB_FileCoverTB.FileCoverURL = ParameterAPI.GetConfig("DowLoadFileURL").ConfigValue + "//" + account + "//" + g + "//" + fileCoverName + "." + fileExtension;
                                 
                    saveToUrl = dir + @"\\" + fileCoverName + "." + fileExtension;
                    result = Common.Common.FileWrite(saveToUrl, newFileData);
                    if (!result.IsOK)
                    {
                        return result;
                    }
                    pCB_FileCoverTB.UpdateDateTime = DateTime.Now;
                    pCBEntities.Refresh(System.Data.Objects.RefreshMode.ClientWins, pCB_FileCoverTB);
                    result.IsOK = Convert.ToBoolean(pCBEntities.SaveChanges());
                    if (!result.IsOK)
                    {
                        File.Delete(saveToUrl);
                        result.Description = "编辑失败";
                        return result;
                    }
                    File.Delete(oldUrl);
                    result.Description = "编辑成功";
                }
            }
            catch (Exception ex)
            {
                File.Delete(saveToUrl);
                LogHelper.WriteLog(GetType()).Info(ex.StackTrace);
                result.IsOK = false;
                result.Description = ex.InnerException.Message;
            }
            return result;
        }

        /// <summary>
        /// 删除附件
        /// </summary>
        /// <param name="fileCoverID"></param>
        /// <param name="accessFileName"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        [WebMethod(Description = "删除附件")]
        public Result DeleteAccessFile(string fileCoverID, string accessFileName, string account)
        {
            Result result = new Result();
            try
            {
                Guid g = new Guid(fileCoverID);
                PCBEntities pCBEntities = new PCBEntities();
                PCB_AccessFileTB pCB_AccessFileTB = pCBEntities.PCB_AccessFileTB.FirstOrDefault(p => p.CreateAccount == account  && p.FileCoverID == g && p.AccessFileName == accessFileName);// pCBEntities.PCB_AccessFileTB.FirstOrDefault(p => p.FileCoverID == new Guid(fileCoverID) && p.CreateAccount == account && p.FileExtension == fileExtension);
                if (pCB_AccessFileTB == null || pCB_AccessFileTB == default(PCB_AccessFileTB))
                {
                    result.IsOK = false;
                    result.Description = "找不到对应的附件";
                    return result;
                }
                pCBEntities.DeleteObject(pCB_AccessFileTB);
                result.IsOK = Convert.ToBoolean(pCBEntities.SaveChanges());
                if (!result.IsOK)
                {
                    
                    result.Description = "删除失败";
                    return result;
                }
                string saveToUrl= @ParameterAPI.GetConfig("FileURL").ConfigValue + @"\\" + account + @"\\" + fileCoverID + @"\\" + accessFileName + "." + pCB_AccessFileTB.FileExtension;
                File.Delete(saveToUrl);
            }
            catch (Exception ex)
            {
               
                LogHelper.WriteLog(GetType()).Info(ex.StackTrace);
                result.IsOK = false;
                result.Description = ex.InnerException.Message;
            }
            return result;

        }

        /// <summary>
        /// 删除整个包附件
        /// </summary>
        /// <param name="fileCoverID"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        [WebMethod(Description = "删除整个包附件")]
        public Result DeleteFileCover(string fileCoverID, string account)
        {
            Result result = new Result();
            try
            {
                Guid g = new Guid(fileCoverID);
                PCBEntities pCBEntities = new PCBEntities();
                PCB_FileCoverTB pCB_FileCoverTB = pCBEntities.PCB_FileCoverTB.FirstOrDefault(p => p.FileCoverID == g && p.CreateAccount == account);
                if (pCB_FileCoverTB == null || pCB_FileCoverTB == default(PCB_FileCoverTB))
                {
                    result.IsOK = false;
                    result.Description = "找不到该文件信息";
                    return result;

                }
                pCBEntities.DeleteObject(pCB_FileCoverTB);
                IQueryable<PCB_AccessFileTB> queryable = pCBEntities.PCB_AccessFileTB.Where(p=>p.FileCoverID==g&&p.CreateAccount==account);
                foreach (var item in queryable)
                {
                    pCBEntities.DeleteObject(item);
                }
                result.IsOK = Convert.ToBoolean(pCBEntities.SaveChanges());
                if (!result.IsOK)
                {

                    result.Description = "删除失败";
                    return result;
                }
                string saveToUrl = @ParameterAPI.GetConfig("FileURL").ConfigValue + @"\\" + account + @"\\" + fileCoverID ;
                Directory.Delete(saveToUrl,true);
            }
            catch (Exception ex)
            {

                LogHelper.WriteLog(GetType()).Info(ex.StackTrace);
                result.IsOK = false;
                result.Description = ex.InnerException.Message;
            }
            return result;
        }

        /// <summary>
        /// 获取所有发布的文件封面信息
        /// </summary>
        /// <returns></returns>
        [WebMethod(Description = "获取所有发布的文件封面信息")]
        public Result GetAllFileCoverList()
        {
            Result result = new Result();
            try
            {
                PCBEntities pCBEntities = new PCBEntities();
                var infoList = pCBEntities.PCB_FileCoverTB.Where(p=>p.IsPublish==true).Select(p=> new { p.FileCoverID,p.FileCoverName,p.FileCoverURL,p.FileExtension,p.FileSize,p.Price});
                result.IsOK = true;
                result.ExtData = JsonConvert.SerializeObject(infoList);
            }
            catch (Exception ex)
            {

                LogHelper.WriteLog(GetType()).Info(ex.StackTrace);
                result.IsOK = false;
                result.Description = ex.InnerException.Message;
            }
            return result;
        }

        /// <summary>
        /// 通过账号获取自己的文件封面信息
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        [WebMethod(Description = "通过账号获取自己的文件封面信息")]
        public Result GetOwnFileCoverList(string account)
        {
            Result result = new Result();
            try
            {
                PCBEntities pCBEntities = new PCBEntities();
                var infoList = pCBEntities.PCB_FileCoverTB.Where(p => p.CreateAccount == account).Select(p => new { p.FileCoverID, p.FileCoverName, p.FileCoverURL, p.FileExtension, p.FileSize, p.Price,p.IsPublish,p.CreateAccount,p.CreateDateTime,p.UpdateDateTime });
                result.IsOK = true;
                result.ExtData = JsonConvert.SerializeObject(infoList);
            }
            catch (Exception ex)
            {

                LogHelper.WriteLog(GetType()).Info(ex.StackTrace);
                result.IsOK = false;
                result.Description = ex.InnerException.Message;
            }
            return result;

        }

        /// <summary>
        /// 通过账号和封面ID获取附件
        /// </summary>
        /// <param name="account"></param>
        /// <param name="fileCoverID"></param>
        /// <returns></returns>
        [WebMethod(Description = "通过账号和封面ID获取附件")]
        public Result GetOwnAccessFile(string account, string fileCoverID)
        {
            Result result = new Result();
            try
            {
                Guid g = new Guid(fileCoverID);
                PCBEntities pCBEntities = new PCBEntities();
                var infoList = pCBEntities.PCB_AccessFileTB.Where(p => p.CreateAccount == account&&p.FileCoverID==g).Select(p => new { p.FileCoverID, p.AccessFileName, p.AccessFileURL, p.FileExtension, p.FileSize, p.FileMD5, p.CreateDateTime,p.CreateAccount });
                result.IsOK = true;
                result.ExtData = JsonConvert.SerializeObject(infoList);
            }
            catch (Exception ex)
            {

                LogHelper.WriteLog(GetType()).Info(ex.StackTrace);
                result.IsOK = false;
                result.Description = ex.InnerException.Message;
            }
            return result;
        }




        [WebMethod]
        public Result test(string file,string id,string type,string account,string hz,string name)
        {
            FileStream fs = new FileStream(file, FileMode.Open, FileAccess.ReadWrite);
            byte[] bt = new byte[fs.Length];
            fs.Read(bt, 0,(int)fs.Length);
            fs.Close();
            Result result = new Result();
           // UpdateFileCover();
            string fileData = System.Text.Encoding.Default.GetString(bt);
            if (type == "1")
            {
                result = UploadAccessFile(bt, id, account, hz, name);
            }
            else
            {
                result = UploadFileCover(bt, account, hz, name);
            }
            return result;

        }
    }
}
