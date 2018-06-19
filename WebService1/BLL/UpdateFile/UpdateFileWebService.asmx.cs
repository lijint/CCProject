using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using DownLoad.BLL.Common;
using DownLoad.Entity;
using Newtonsoft.Json;
using System.IO;
using DownLoad.Model;

namespace DownLoad.BLL.UpdateFile
{
    /// <summary>
    /// UpdateFileWebService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class UpdateFileWebService : System.Web.Services.WebService
    {

        #region 获取文件更新列表
        /// <summary>
        /// 获取文件更新列表
        /// </summary>
        /// <returns>返回需要更新的文件更表，格式：名称、大小、路径、版本号、文件内容</returns>
        [WebMethod]
        public Result GetUpdateFileList(string account)
        {
            Result result=new Result();
            try
            {
                PMSEntities pMSEntities = new PMSEntities();
                Sys_UserTB userTB = pMSEntities.Sys_UserTB.FirstOrDefault(p => p.Account == account&&p.StateCode==true);
                if (userTB == null)
                {
                    result.IsOK = false;
                    result.Description = "该账号:"+account+ "不存在";
                    return result;
                }
                if (string.IsNullOrEmpty(userTB.FilePermission))
                {
                    result.IsOK = true;
                    result.ExtData="";
                    return result;
                }
                result.IsOK = true;
                DataTable dt = new DataTable("FileTable");
                DataColumn column;


                //文件版本号，如果版本号为空则为：1.01.1753.0101
                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "FilePath";
                dt.Columns.Add(column);

                //文件MD5校验码
                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "FileMD5";
                dt.Columns.Add(column);

                //文件下载地址
                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "FileURL";
                dt.Columns.Add(column);
               string updatePath = ParameterAPI.GetConfig("FileDir").ConfigCode;
                result = Common.Common.GetFileList("", updatePath, ref dt);
                DataTable newData = dt.Clone();
                foreach (DataRow item in dt.Rows)
                {
                    string fileName= item["FilePath"].ToString().Split('\\')[1];
                    if (userTB.FilePermission.Contains(fileName))
                    {
                        newData.ImportRow(item);
                    }
                }

                result.ExtData = JsonConvert.SerializeObject(newData);
                // List<xx> xxList= JsonConvert.DeserializeObject<List<xx>>(result.ExtData);

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

        #region 获取文件权限值列表
        [WebMethod]
        public Result GetFilePermissionList()
        {
            Result result = new Result();
            try
            {
                result.IsOK = true;
                DataTable dt = new DataTable("FileTable");
                DataColumn column;
                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "FileName";
                dt.Columns.Add(column);
                string filePath = ParameterAPI.GetConfig("FileDir").ConfigCode;
                string[] dirs = Directory.GetDirectories(filePath);
                if (dirs.Length > 0)
                {
                   
                    DirectoryInfo directoryInfo = new DirectoryInfo(dirs[0]);
                    var directroies = directoryInfo.GetDirectories();
                    foreach (var directroy in directroies)
                    {
                        DataRow dr = dt.NewRow();
                        dr["FileName"] = directroy.Name;
                        dt.Rows.Add(dr);
                    }
                    result.ExtData= JsonConvert.SerializeObject(dt);
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


    }
}
