using didi_UP01.Classes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace didi_UP01
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private List<ZayavkaItems> zayavkiList;
        private List<Workers> workersList;
        private Workers workSelect;
        public int idStrCB;

        public Window1()
        {
            InitializeComponent();

            LoadZayavkiList();
            LoadWorkers();

            LoadStatistics();
        }

        private void LoadZayavkiList()
        {
            zayavkiList = ZayavkaItems.LoadDataFromDatabase();
            ZayavkiLV.ItemsSource = zayavkiList;
        }
        
        public int selectedZayavkaId;
        public string selectedZayavkaStatus;
        public string selectedZayavkaPriorit;
        private void ZayavkiLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ZayavkaItems selectedZayavka = ZayavkiLV.SelectedItem as ZayavkaItems;

            if (selectedZayavka != null)
            {
                selectedZayavkaId = selectedZayavka.IdZayavi - 1;
                selectedZayavkaStatus = selectedZayavka.Status; 
                selectedZayavkaPriorit = selectedZayavka.Priorit;
            }
        }

        private void LoadWorkers()
        {
            workersList = new List<Workers>();
            try
            {
                sql.OpenConnection();
                string query = "SELECT Код_Сотрудника, CONCAT(Фамилия, ' ',  Имя, ' ', Отчество) AS ФИО FROM Сотрудники " +
                    "WHERE Код_Должности = 3 ";
                SqlCommand command = new SqlCommand(query, sql.str);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Workers work = new Workers
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Код_Сотрудника")),
                        Name = reader.GetString(reader.GetOrdinal("ФИО"))
                    };
                    workersList.Add(work);
                }

                SotrudCB.ItemsSource = workersList;
                SotrudCB.DisplayMemberPath = "Name";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
            finally
            {
                sql.CloseConnection();
            }
        }

        private void NeisprBtn_Click(object sender, RoutedEventArgs e)
        {
            ZayavkaItems selectedZayavka = ZayavkiLV.SelectedItem as ZayavkaItems;

            if (selectedZayavka == null)
            {
                MessageBox.Show("Выберите заявку!", "Предупреждение");
            }
            else if (selectedZayavkaStatus == "В работе" || selectedZayavkaStatus == "Выполнено")
            {
                MessageBox.Show("Заявка не в статусе Ожидания!", "Предупреждение");
            }
            else
            {
                sql.OpenConnection();

                string query = $"UPDATE Заявки SET Код_Статуса = 2 " +
                    $"WHERE Код_Заяки = {selectedZayavkaId}";

                SqlCommand command = new SqlCommand(query, sql.str);
                command.ExecuteNonQuery();
            }

            LoadZayavkiList();
        }

        private void NizkiqBtn_Click(object sender, RoutedEventArgs e)
        {
            ZayavkaItems selectedZayavka = ZayavkiLV.SelectedItem as ZayavkaItems;

            if (selectedZayavka == null)
            {
                MessageBox.Show("Выберите заявку!", "Предупреждение");
            }
            else
            {
                sql.OpenConnection();

                string query = $"UPDATE Заявки SET Код_Приоритета = {1} " +
                    $"WHERE Код_Заяки = {selectedZayavkaId}";

                SqlCommand command = new SqlCommand(query, sql.str);
                command.ExecuteNonQuery();
            }

            LoadZayavkiList();
        }

        private void SredBtn_Click(object sender, RoutedEventArgs e)
        {
            ZayavkaItems selectedZayavka = ZayavkiLV.SelectedItem as ZayavkaItems;

            if (selectedZayavka == null)
            {
                MessageBox.Show("Выберите заявку!", "Предупреждение");
            }
            else
            {
                sql.OpenConnection();

                string query = $"UPDATE Заявки SET Код_Приоритета = {2} " +
                    $"WHERE Код_Заяки = {selectedZayavkaId}";

                SqlCommand command = new SqlCommand(query, sql.str);
                command.ExecuteNonQuery();
            }

            LoadZayavkiList();
        }

        private void VisBtn_Click(object sender, RoutedEventArgs e)
        {
            ZayavkaItems selectedZayavka = ZayavkiLV.SelectedItem as ZayavkaItems;

            if (selectedZayavka == null)
            {
                MessageBox.Show("Выберите заявку!", "Предупреждение");
            }
            else
            {
                sql.OpenConnection();

                string query = $"UPDATE Заявки SET Код_Приоритета = {3} " +
                    $"WHERE Код_Заяки = {selectedZayavkaId}";

                SqlCommand command = new SqlCommand(query, sql.str);
                command.ExecuteNonQuery();
            }

            LoadZayavkiList();
        }

        private void SotrudCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            workSelect = SotrudCB.SelectedItem as Workers;
            if (workSelect != null)
            {
                idStrCB = workSelect.Id;
            }
        }

        private void IspRedBtn_Click(object sender, RoutedEventArgs e)
        {
            ZayavkaItems selectedZayavka = ZayavkiLV.SelectedItem as ZayavkaItems;

            if (selectedZayavka == null)
            {
                MessageBox.Show("Выберите заявку!", "Предупреждение");
            }
            else if (selectedZayavkaStatus == "Выполнено")
            {
                MessageBox.Show("Для этой заявки нельзя менять исполнителя!", "Предупреждение");
            }
            else
            {
                int stat = 2;

                sql.OpenConnection();

                string query = $"UPDATE Заявки_на_ремонт SET Код_Сотрудника = {idStrCB} " +
                    $"WHERE Код_Заявки = {selectedZayavkaId}";

                SqlCommand command = new SqlCommand(query, sql.str);
                command.ExecuteNonQuery();

                string updateQuery = "UPDATE Заявки SET Код_Статуса = @stat " +
                                         "WHERE Код_Заяки = @selectedZayavkaId";

                SqlCommand updateCommand = new SqlCommand(updateQuery, sql.str);
                updateCommand.Parameters.AddWithValue("@stat", stat);
                updateCommand.Parameters.AddWithValue("@selectedZayavkaId", selectedZayavkaId);

                updateCommand.ExecuteNonQuery();
            }

            LoadZayavkiList();
        }

        private List<FaultTypeStatisticsItem> faultTypeStatisticsList;
        private void LoadStatistics()
        {
            faultTypeStatisticsList = new List<FaultTypeStatisticsItem>();
            DateTime date = DateTime.Now;

            sql.OpenConnection();
            try
            {
                string query = "SELECT Тип_неисправности.Наименование AS FaultType, COUNT(*) AS RequestCount " +
               "FROM Заявки " +
               "INNER JOIN Тип_неисправности ON Заявки.Код_Неисправности = Тип_неисправности.Код_Неисправности " +
               "WHERE Тип_неисправности.Код_Неисправности <> 16 " + // Добавляем фильтр
               "GROUP BY Тип_неисправности.Наименование";


                SqlCommand command = new SqlCommand(query, sql.str);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    FaultTypeStatisticsItem item = new FaultTypeStatisticsItem
                    {
                        FaultType = reader.GetString(reader.GetOrdinal("FaultType")),
                        RequestCount = reader.GetInt32(reader.GetOrdinal("RequestCount"))
                    };

                    faultTypeStatisticsList.Add(item);
                }
                reader.Close();

                SqlCommand commandTime = new SqlCommand(
             $"SELECT SUM(DATEDIFF(HOUR, Дата_добавления, Дата_окончания)) " +
             $"FROM Заявки " +
             $"WHERE Код_Статуса = 3 AND Дата_окончания IS NOT NULL", sql.str);

                // Получаем общее время выполнения
                object totalTimeObj = commandTime.ExecuteScalar();
                if (totalTimeObj != DBNull.Value && totalTimeObj != null)
                {
                    int totalTime = Convert.ToInt32(totalTimeObj);

                    // Запрос для получения количества выполненных заявок
                    SqlCommand commandCount = new SqlCommand($"SELECT COUNT(*) FROM Заявки WHERE Код_Статуса = 3 AND Дата_окончания IS NOT NULL", sql.str);
                    int _executedRequestsCount = (int)commandCount.ExecuteScalar();
                    ExecutedRequestsCount.Text = Convert.ToString(_executedRequestsCount);

                    // Рассчитываем среднее время выполнения
                    if (_executedRequestsCount > 0)
                    {
                        double averageTime = totalTime / (double)_executedRequestsCount;
                        AverageTime.Text = $"{averageTime:F2} ч.";
                    }
                    else
                    {
                        AverageTime.Text = "Нет выполненных заявок";
                    }
                }
                else
                {
                    AverageTime.Text = "Нет выполненных заявок";
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex}", "Ошибка");
            }
            faultTypeListView.ItemsSource = faultTypeStatisticsList;

            sql.CloseConnection();
        }
    }

    public class FaultTypeStatisticsItem
    {
        public string FaultType { get; set; }
        public int RequestCount { get; set; }
    }

}
