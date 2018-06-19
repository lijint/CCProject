using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace DownLoad.BLL.Common
{
    public class SqlCommand
    {
        public static DataTable Query(string strSql)
        {
            string str = System.Configuration.ConfigurationManager.ConnectionStrings["XNet"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(str))
            {
                DataTable dt = new DataTable();
              //  MyDBEntities context = new MyDBEntities();
                try
                {
                    conn.Open();
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(strSql, conn);
                    sqlDataAdapter.Fill(dt);
                }
                catch (SqlException ex)
                {
                    throw new Exception(ex.InnerException.Message);
                }

                return dt;
            }
        }

    }
}