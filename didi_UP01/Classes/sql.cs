using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace didi_UP01.Classes
{
    class sql
    {
        public static SqlConnection str; 

        public static void OpenConnection() 
        {
            str = new SqlConnection
            {
                ConnectionString = @"Data Source=LAPTOP-T86M4QDS;Initial Catalog=уп01;Integrated Security=True"
            };
            str.Open(); 
        }

        public static void CloseConnection() 
        {
            str.Close(); 
        }
    }
}
