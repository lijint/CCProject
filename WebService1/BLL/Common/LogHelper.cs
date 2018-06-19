using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;

namespace DownLoad.BLL.Common
{
    public class LogHelper
    {
        public static ILog WriteLog(Type name)
        {
            log4net.ILog logger = LogManager.GetLogger(name);
            return logger;
        }
    }
}