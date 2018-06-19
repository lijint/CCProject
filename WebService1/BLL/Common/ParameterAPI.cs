using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DownLoad.Model;

namespace DownLoad.BLL.Common
{
    public class ParameterAPI
    {
        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <param name="configCoede"></param>
        /// <returns></returns>
        public static PCB_ConfigTB GetConfig(string configCoede)
        {
            PCB_ConfigTB config=null;
            try
            {
                PCBEntities pCBEntities = new PCBEntities();
                 config = pCBEntities.PCB_ConfigTB.FirstOrDefault(p => p.ConfigCode == configCoede);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.InnerException.Message);
            }
           
             return config;
          
        }
        
    }
}