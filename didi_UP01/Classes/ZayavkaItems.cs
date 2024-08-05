using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace didi_UP01.Classes
{
    class ZayavkaItems
    {
        public int IdZayavi { get; set; }

        public string Oborud { get; set; }
        public string Neispr { get; set; }
        public string Client { get; set; }
        public string DateOforml { get; set; }
        public string Sotrud { get; set; }
        public string Status { get; set; }
        public string Priorit { get; set; }
        public int IdPriorit { get; set; }
        public DateTime DateEndList { get; set; }


        public static List<ZayavkaItems> LoadDataFromDatabase()
        {
            List<ZayavkaItems> zayavki = new List<ZayavkaItems>();

            sql.OpenConnection();

            string query = "SELECT зр.Код_заявки_ремонт, о.Наименование AS Оборуд, тн.Наименование AS Неиспр, " +
                           "к.Фамилия + ' ' + к.Имя + ' ' + к.Отчество AS Клиент, " +
                           "з.Дата_добавления, ср.Фамилия + ' ' + ср.Имя + ' ' + ср.Отчество AS Sotrud, " +
                           "ст.Статус, п.Приоритет, з.Код_Приоритета, з.Дата_окончания " +
                           "FROM Заявки_на_ремонт зр " +
                           "JOIN Заявки з ON зр.Код_Заявки = з.Код_Заяки " +
                           "JOIN Сотрудники ср ON зр.Код_Сотрудника = ср.Код_Сотрудника " +
                           "JOIN Оборудование о ON з.Код_Оборудования = о.Код_Оборудования " +
                           "JOIN Тип_неисправности тн ON з.Код_Неисправности = тн.Код_Неисправности " +
                           "JOIN Клиенты к ON з.Код_Клиента = к.Код_Клиента " +
                           "JOIN Статусы ст ON з.Код_Статуса = ст.Код_Статуса " +
                           "JOIN Приоритеты п ON з.Код_Приоритета = п.Код_Приоритета";

            SqlCommand command = new SqlCommand(query, sql.str);                
            SqlDataReader reader = command.ExecuteReader();            
            while (reader.Read())
            {
                ZayavkaItems zayavka = new ZayavkaItems
                {
                    IdZayavi = reader.GetInt32(reader.GetOrdinal("Код_заявки_ремонт")),
                    Oborud = reader.GetString(reader.GetOrdinal("Оборуд")),
                    Neispr = reader.GetString(reader.GetOrdinal("Неиспр")),
                    Client = reader.GetString(reader.GetOrdinal("Клиент")),
                    DateOforml = reader.GetDateTime(reader.GetOrdinal("Дата_добавления")).ToString(),
                    Sotrud = reader.GetString(reader.GetOrdinal("Sotrud")),
                    Status = reader.GetString(reader.GetOrdinal("Статус")),
                    Priorit = reader.GetString(reader.GetOrdinal("Приоритет")),
                    IdPriorit = reader.GetInt32(reader.GetOrdinal("Код_Приоритета")),
                    DateEndList = reader.GetDateTime(reader.GetOrdinal("Дата_окончания"))
                };

                zayavki.Add(zayavka);
            }

            return zayavki;
        }

        public static List<ZayavkaItems> LoadDataFromDatabaseLog(string log)
        {
            int stat = 1;
            List<ZayavkaItems> zayavki = new List<ZayavkaItems>(); 

            sql.OpenConnection();

            string query = $"SELECT зр.Код_заявки_ремонт, о.Наименование AS Оборуд, тн.Наименование AS Неиспр, " +
                           $"к.Фамилия + ' ' + к.Имя + ' ' + к.Отчество AS Клиент, " +
                           $"з.Дата_добавления, ср.Фамилия + ' ' + ср.Имя + ' ' + ср.Отчество AS Sotrud, " +
                           $"ст.Статус, п.Приоритет, з.Код_Приоритета " +
                           $"FROM Заявки_на_ремонт зр " +
                           $"JOIN Заявки з ON зр.Код_Заявки = з.Код_Заяки " +
                           $"JOIN Сотрудники ср ON зр.Код_Сотрудника = ср.Код_Сотрудника " +
                           $"JOIN Оборудование о ON з.Код_Оборудования = о.Код_Оборудования " +
                           $"JOIN Тип_неисправности тн ON з.Код_Неисправности = тн.Код_Неисправности " +
                           $"JOIN Клиенты к ON з.Код_Клиента = к.Код_Клиента " +
                           $"JOIN Статусы ст ON з.Код_Статуса = ст.Код_Статуса " +
                           $"JOIN Приоритеты п ON з.Код_Приоритета = п.Код_Приоритета " +
                           $"WHERE Логин = @log AND з.Код_Статуса <> @stat";

            SqlCommand command = new SqlCommand(query, sql.str);
            command.Parameters.AddWithValue("@log", log);
            command.Parameters.AddWithValue("@stat", stat);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                ZayavkaItems zayavka = new ZayavkaItems
                {
                    IdZayavi = reader.GetInt32(reader.GetOrdinal("Код_заявки_ремонт")),
                    Oborud = reader.GetString(reader.GetOrdinal("Оборуд")),
                    Neispr = reader.GetString(reader.GetOrdinal("Неиспр")),
                    Client = reader.GetString(reader.GetOrdinal("Клиент")),
                    DateOforml = reader.GetDateTime(reader.GetOrdinal("Дата_добавления")).ToString(),
                    Sotrud = reader.GetString(reader.GetOrdinal("Sotrud")),
                    Status = reader.GetString(reader.GetOrdinal("Статус")),
                    Priorit = reader.GetString(reader.GetOrdinal("Приоритет")),
                    IdPriorit = reader.GetInt32(reader.GetOrdinal("Код_Приоритета"))
                };

                zayavki.Add(zayavka);
            }

            return zayavki;
        }
    }
}
