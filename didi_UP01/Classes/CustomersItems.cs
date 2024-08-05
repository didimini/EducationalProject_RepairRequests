using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace didi_UP01.Classes
{
    class CustomersItems
    {
        public int IdCust { get; set; }
        public string FIO { get; set; }
        public string NomerTel { get; set; }
        public string Adres { get; set; }


        public static List<CustomersItems> LoadDataFromDatabase2()
        {
            List<CustomersItems> customersItems = new List<CustomersItems>();

            sql.OpenConnection();
            string query = "SELECT Код_Клиента, CONCAT(Фамилия, ' ',  Имя, ' ', Отчество) AS ФИО, " +
                "Номер_телефона, Адрес " +
                "FROM Клиенты";
            SqlCommand command = new SqlCommand(query, sql.str);
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                CustomersItems customers = new CustomersItems
                {
                    IdCust = reader.GetInt32(reader.GetOrdinal("Код_Клиента")),
                    FIO = reader.GetString(reader.GetOrdinal("ФИО")),
                    NomerTel = reader.GetString(reader.GetOrdinal("Номер_телефона")),
                    Adres = reader.GetString(reader.GetOrdinal("Адрес"))
                };

                customersItems.Add(customers);
            }

            return customersItems;
        }
    }

}
