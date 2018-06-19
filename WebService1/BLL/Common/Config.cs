using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace DownLoad.BLL.Common
{
    public class Config
    {
        public static string alipay_public_key = ParameterAPI.GetConfig("AliPublicKey").ConfigValue; //= @"MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAsFet0B7s1Ayxypehme1bv30H2DRPYfIMPM3nV0vOPmvkxjAP5EAkM1VSSbOT4a8DmO/KLj21Pqr2bFoD3MvQb/Gu/J8lowZSkB+wx8mBS4dVS6Z+W9nqb8uH728A9ElrmzyiLZK8OqCPYGTXs6bBogP6sfI8XfR8Q9CEC7bdlKDl1zag+JboFwt3yuPxyC9R6iNYX3szI4bte6E9mmCMvxwPxFxc8NroeCz8opzXogEV3jAWadGpe2O+sWT7JOVF7NVAsDJp3bJtcvA1Qi1pMVIMOy7CVXIXnTZKEIiM5Mo+YCVAGi3ao0SlCsxrawcz+p+LJG9js2Jw1sAnUHv5cwIDAQAB";


        //这里要配置没有经过的原始私钥

        //开发者私钥
        public static string merchant_private_key = ParameterAPI.GetConfig("DevPrivateKey").ConfigValue; //= @"MIIEpAIBAAKCAQEAtC151N0QTLL75dOhipFwqVhO4lTyQWr1TTXNPsCoOPjNxOY4/cbN6xino3uI5I1Zx8cREo3tWXazDFzpOOWLBRAoByQN2I0f7Z/NHdg5ERsQg5bQUUecXWj3R8YXesGfnpSnFqe00ZCQEXZcw9XfZ5En6xGA+Xnni9MJbv+GQiurv0/jghtHrdEH7tq1joR7sp9+NOvg+SsRuECzD03f0MGVdgmIjUTgp06ilV+YNPi//NV9SFjVIR/QXFSmcjrphSXDvJUMpmZLRSIZMC08gPjh/hno/zU7vVLBr74m7dNUR9rVcTXSkVua+LvfDsisOjOQaHIOuJN+0RFHwvvfgwIDAQABAoIBAQCwCvLLoYbIeeSXBLUEb+BVBbxldbwRYND2NIgqNRDoQWjDZnHuuuz9NGSu3ge9z8IV6RFsQJHUZJ7CiJEzD0xKkSOa/oYsvI6inQ1LiLURWpFDEPrPP5muxt09GnZccYxk5DdxckDAtW+eMDio+3HBflkzWzHaD1rGCldgRXfHc8Ae/nlvbHLV0b1NSjA0LwPoiHCQjLMpHf1XWIAg0BrQj2iMq+NYtSDsdPFAZotSMrbU3XRRw+HdaaAZjynpbvKhJuM+dbb3r5BqR4qbyeTypGYCaYJ/VzyvC1FALnyUBRpEjJL9nDj0Jd594TLKV3mvoHMcsVbaP8QSUkKu3NxRAoGBAO1PwrzsPCKPNO/wgu/rM6+0dfvKJNROFMhrpZb295xGTlZMVp1Mq6nf1TNJktqAaa2AJ+xJm/Ybx1JPem80OLtKMomM2srX6/MjdRdYN9x5LiLCnTiRm0zB/qROG2B9qpd3K4HOIsQuFmkKIot+PUUQVeEZEb04n/n3k8yi+ByJAoGBAMJd4tdAMTrJjigLjdieYZuZfh8VmK21ws20pBE2H+rXA4kwOtMmf4Au2X6QFuFCj79sbNvoViZNjfJfnNh18LOLjfIoftaGNonwQOMskJqodrg/rMG6UZ1d4LSBZof69PN8AEx98NPdTdgSRRAtXdq3LjhnHkIcARpHuVeSJlCrAoGBAI7T9ozmUbsrHd1bkDL+CHmzz17f5xKwe+m0gFFACv9PgU6HW4oI3zi6swLPQUepCfGWtHCOTQPu7CQqGbJcZ9ixa4FF+VkkY6pOqOaH/fcAKDhbkWy+lg24pJ4XNHr6p3XKD7Fsc5hHdM17gBccv5uVnVqBUB/muMIltBrx6wqhAoGACj/pNnCsJ+15EaOKV2ICw7HUdM8uvfpXy4E7Ja01Aa1VAmcIsxnUn23ZClOO4VIhQnb1RqN554svcRiZrNCLYOx7D8oS5j0toxiIU3KGLaG0oQK6mXy50RKiRuKMBZ+2SyC+40nA6WtCL6LHAZNi36XJSjAl6FaRuFEP9Q4/9wECgYA8VoSAwb4pj7tzTmIjcIJGnP0drWinUG30UgYbS/oCJwMlEV0W5zU5hdPw0NhnTtuagIRHWjSdJGeX3YE+nfZvAbCiV86HMUhoI83iIzKAJLV05vew29pxkdkBvg1xSr1IxhLAGONbUYxl4jMoT9cjVD1Obr4P2tuW4uq5B3llQg==";


        //开发者公钥
        public static string merchant_public_key =ParameterAPI.GetConfig("DevPublicKey").ConfigValue;//= @"MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAtC151N0QTLL75dOhipFwqVhO4lTyQWr1TTXNPsCoOPjNxOY4/cbN6xino3uI5I1Zx8cREo3tWXazDFzpOOWLBRAoByQN2I0f7Z/NHdg5ERsQg5bQUUecXWj3R8YXesGfnpSnFqe00ZCQEXZcw9XfZ5En6xGA+Xnni9MJbv+GQiurv0/jghtHrdEH7tq1joR7sp9+NOvg+SsRuECzD03f0MGVdgmIjUTgp06ilV+YNPi//NV9SFjVIR/QXFSmcjrphSXDvJUMpmZLRSIZMC08gPjh/hno/zU7vVLBr74m7dNUR9rVcTXSkVua+LvfDsisOjOQaHIOuJN+0RFHwvvfgwIDAQAB";

        //应用ID
        public static string appId = ParameterAPI.GetConfig("APPID").ConfigValue;// "2016082100302839";

        //合作伙伴ID：partnerID
        public static string pid = ParameterAPI.GetConfig("PID").ConfigValue;//"2088102172374984";


        //支付宝网关
        public static string serverUrl = ParameterAPI.GetConfig("ServerUrl").ConfigValue;
        public static string mapiUrl = "https://mapi.alipay.com/gateway.do";
        public static string monitorUrl = "http://mcloudmonitor.com/gateway.do";

        //编码，无需修改
        public static string charset = "utf-8";
        //签名类型，支持RSA2（推荐！）、RSA
        //public static string sign_type = "RSA2";
        public static string sign_type = "RSA2";
        //版本号，无需修改
        public static string version = "1.0";


        /// <summary>
        /// 公钥文件类型转换成纯文本类型
        /// </summary>
        /// <returns>过滤后的字符串类型公钥</returns>
        public static string getMerchantPublicKeyStr()
        {
            StreamReader sr = new StreamReader(merchant_public_key);
            string pubkey = sr.ReadToEnd();
            sr.Close();
            if (pubkey != null)
            {
                pubkey = pubkey.Replace("-----BEGIN PUBLIC KEY-----", "");
                pubkey = pubkey.Replace("-----END PUBLIC KEY-----", "");
                pubkey = pubkey.Replace("\r", "");
                pubkey = pubkey.Replace("\n", "");
            }
            return pubkey;
        }
        /// <summary>
        /// 私钥文件类型转换成纯文本类型
        /// </summary>
        /// <returns>过滤后的字符串类型私钥</returns>
        public static string getMerchantPriveteKeyStr()
        {
            StreamReader sr = new StreamReader(merchant_private_key);
            string pubkey = sr.ReadToEnd();
            sr.Close();
            if (pubkey != null)
            {
                pubkey = pubkey.Replace("-----BEGIN PUBLIC KEY-----", "");
                pubkey = pubkey.Replace("-----END PUBLIC KEY-----", "");
                pubkey = pubkey.Replace("\r", "");
                pubkey = pubkey.Replace("\n", "");
            }
            return pubkey;
        }

    }
}