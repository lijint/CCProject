using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DownLoad.Entity
{
    public class Result
    {

        // 摘要:
        //     结果描述属性
        public string Description { get; set; }
        //
        // 摘要:
        //     扩展数据属性
        public string ExtData { get; set; }  
        //
        // 摘要:
        //     是否执行成功属性
        public bool IsOK { get; set; }

        // 摘要:
        //     状态参数标识属性
        public int StateCodeID { get; set; }
        

    }
}