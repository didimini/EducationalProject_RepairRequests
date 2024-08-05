using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using didi_UP01.Classes;
using System.Windows;

namespace didi_UP01.Classes
{
    class Authorization
    {
        public string log;
        public bool AuthenticateUser(string login, string password)
        {
            sql.OpenConnection();
            bool isAuthenticated = false;
            try
            {

                string query = $"SELECT * FROM Сотрудники WHERE Логин = @Login AND Пароль = @Password";
                using (SqlCommand command = new SqlCommand(query, sql.str))
                {
                    command.Parameters.AddWithValue("@Login", login);
                    command.Parameters.AddWithValue("@Password", password);
                    log = login;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Пользователь найден, можете выполнить дополнительные действия
                            int positionCode = reader.GetInt32(reader.GetOrdinal("Код_Должности"));
                            OpenAppropriateWindow(positionCode);
                            isAuthenticated = true;
                            
                        }
                        else
                        {
                            MessageBox.Show("Неверный логин или пароль");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
            finally
            {
                sql.CloseConnection();
            }

            return isAuthenticated;            
        }

        private void OpenAppropriateWindow(int positionCode)
        {

            switch (positionCode)
            {
                case 1:
                    Window1 win1 = new Window1();
                    win1.Show();
                    break;
                case 2:
                    RegdtrOgApp Reg = new RegdtrOgApp();
                    Reg.Show();
                    break;
                case 3:
                    Window2 win2 = new Window2(log);
                    win2.Show();
                    break;
                default:
                    MessageBox.Show("Неизвестная должность");
                    break;
            }
        }
    }
}
