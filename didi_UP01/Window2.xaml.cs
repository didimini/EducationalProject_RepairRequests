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
    /// Логика взаимодействия для Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        public string log;

        private List<ZayavkaItems> zayavkiList;

        private List<TypeMalfun> typeMalfunList;
        private TypeMalfun typeMalSelect;
        public int idTneisCB;

        private List<MaterialClass> materialsList;
        private MaterialClass materialSelect;
        public int idMaterial;


        public Window2(string log)
        {
            InitializeComponent();
            this.log = log;

            LoadZayavkiList();
            LoadTypeMalfun();
            LoadMaterial();
        }

        private void LoadZayavkiList()
        {
            zayavkiList = ZayavkaItems.LoadDataFromDatabaseLog(log);
            ZayavkiLV.ItemsSource = zayavkiList;
        }

        private void LoadTypeMalfun()
        {
            typeMalfunList = new List<TypeMalfun>();
            try
            {
                sql.OpenConnection();
                string query = "SELECT Код_Неисправности, Наименование FROM Тип_неисправности";
                SqlCommand command = new SqlCommand(query, sql.str);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    TypeMalfun typeM = new TypeMalfun
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Код_Неисправности")),
                        Name = reader.GetString(reader.GetOrdinal("Наименование"))
                    };
                    typeMalfunList.Add(typeM);
                }

                NeisprCB.ItemsSource = typeMalfunList;
                NeisprCB.DisplayMemberPath = "Name";
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

        private void LoadMaterial()
        {
            materialsList = new List<MaterialClass>();
            try
            {
                sql.OpenConnection();
                string query = "SELECT Код_Материала, Наименование, Цена FROM Материалы";
                SqlCommand command = new SqlCommand(query, sql.str);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    MaterialClass mater = new MaterialClass
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Код_Материала")),
                        Name = reader.GetString(reader.GetOrdinal("Наименование")),
                        Cost = reader.GetString(reader.GetOrdinal("Цена"))
                    };
                    materialsList.Add(mater);
                }

                MaterialCB.ItemsSource = materialsList;
                MaterialCB.DisplayMemberPath = "Name";
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

        public int selectedZayavkaId;
        public string selectedZayavkaStatus;
        private void ZayavkiLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ZayavkaItems selectedZayavka = ZayavkiLV.SelectedItem as ZayavkaItems;

            if (selectedZayavka != null)
            {
                selectedZayavkaId = selectedZayavka.IdZayavi;
                selectedZayavkaStatus = selectedZayavka.Status;
                //selectedZayavkaPriorit = selectedZayavka.Priorit;
            }
        }

        private void NeisprCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            typeMalSelect = NeisprCB.SelectedItem as TypeMalfun;
            if (typeMalSelect != null)
            {
                idTneisCB = typeMalSelect.Id;
            }
        }

        private void EndBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder materialsSummary = new StringBuilder();
            int totalCost = 0;
            double time = 0;

            string txtComm = txtComment.Text;
            DateTime date = DateTime.Now;
            int stat = 3;
            ZayavkaItems selectedZayavka = ZayavkiLV.SelectedItem as ZayavkaItems;

            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string templatePath = System.IO.Path.Combine(currentDirectory, "Raports", "шаблон.docx");
            string outputPath = System.IO.Path.Combine(currentDirectory, "Raports", $"Договор № {date.ToString("yyyyMMddHHmmss")}.docx");

            sql.OpenConnection();

            if (selectedZayavka == null)
            {
                MessageBox.Show("Выберите заявку", "Предупреждение");
            }
            else
            {

                try
                {
                    // Обновление данных в таблице Заявки
                    string updateQuery = "UPDATE Заявки SET Код_Неисправности = @idTneisCB, " +
                                         "Код_Статуса = @stat, " +
                                         "Дата_окончания = @date, " +
                                         "Комментарий = @txtComm " +
                                         "WHERE Код_Заяки = @selectedZayavkaId";

                    SqlCommand updateCommand = new SqlCommand(updateQuery, sql.str);
                    updateCommand.Parameters.AddWithValue("@idTneisCB", idTneisCB);
                    updateCommand.Parameters.AddWithValue("@stat", stat);
                    updateCommand.Parameters.AddWithValue("@date", date);
                    updateCommand.Parameters.AddWithValue("@txtComm", txtComm);
                    updateCommand.Parameters.AddWithValue("@selectedZayavkaId", selectedZayavkaId);

                    updateCommand.ExecuteNonQuery();

                    SqlCommand commandSort = new SqlCommand($"SELECT CONCAT(Фамилия, ' ', Имя, ' ', Отчество ) AS ФИО " +
                        $"FROM Сотрудники " +
                        $"WHERE Логин = '{log}'", sql.str);
                    SqlDataReader readSotr = commandSort.ExecuteReader();
                    readSotr.Read();
                    string sotr = readSotr["ФИО"].ToString();
                    readSotr.Close();

                    SqlCommand commandTime = new SqlCommand($"SELECT з.Дата_добавления, з.Комментарий, н.Наименование " +
                        $"FROM Заявки з " +
                        $"JOIN Тип_неисправности н ON з.Код_Неисправности = н.Код_Неисправности " +
                        $"WHERE Код_Заяки = '{selectedZayavkaId}'", sql.str);
                    SqlDataReader readTime = commandTime.ExecuteReader();
                    readTime.Read();
                    // Получаем дату добавления
                    DateTime dateAdded = readTime.GetDateTime(0);

                    // Рассчитываем разницу во времени
                    TimeSpan difference = date - dateAdded;
                    time = difference.TotalHours;

                    //для комментария
                    string comment = readTime["Комментарий"].ToString();
                    string neispr = readTime["Наименование"].ToString();
                    readTime.Close();

                    SqlCommand commandMaterial = new SqlCommand($"SELECT  м.Наименование, зк.Стоимость " +
                        $"FROM Заказ_материалов зк " +
                        $"JOIN Материалы м ON зк.Код_Материала = м.Код_Материала " +
                        $"WHERE Код_Заявки = '{selectedZayavkaId}'", sql.str);
                    SqlDataReader readMaterial = commandMaterial.ExecuteReader();

                    while (readMaterial.Read())
                    {
                        // Получаем данные из результата запроса
                        string material = readMaterial.GetString(0);
                        string costAsString = readMaterial.GetString(1);

                        // Преобразуем строковое значение в числовой тип (int)
                        if (int.TryParse(costAsString, out int cost))
                        {
                            // Добавляем данные к переменным
                            materialsSummary.AppendLine($"{material} - {cost}");
                            totalCost += cost;
                        }
                        else
                        {
                            // Обработка случая, если не удалось преобразовать строку в число
                            Console.WriteLine($"Не удалось преобразовать '{costAsString}' в целое число.");
                        }
                    }
                    readMaterial.Close();

                    Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
                    Microsoft.Office.Interop.Word.Document doc = wordApp.Documents.Open(templatePath);

                    doc.Content.Find.Execute(FindText: "{date}", ReplaceWith: date);
                    doc.Content.Find.Execute(FindText: "{sotr}", ReplaceWith: sotr);
                    doc.Content.Find.Execute(FindText: "{time}", ReplaceWith: time);
                    doc.Content.Find.Execute(FindText: "{material}", ReplaceWith: materialsSummary.ToString());
                    doc.Content.Find.Execute(FindText: "{cost}", ReplaceWith: totalCost.ToString());
                    doc.Content.Find.Execute(FindText: "{neispr}", ReplaceWith: neispr);
                    doc.Content.Find.Execute(FindText: "{comment}", ReplaceWith: comment);

                    doc.SaveAs2(outputPath);
                    doc.Close();
                    wordApp.Quit();
                    System.Diagnostics.Process.Start(outputPath);

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
                finally
                {
                    sql.CloseConnection();
                }

                LoadZayavkiList();

                materialSelect.Id = 0;
                list.Items.Clear();
            }

        }

        private void refBtn_Click(object sender, RoutedEventArgs e)
        {

            if (selectedZayavkaStatus == "Выполнено")
            {
                MessageBox.Show("От этой заявки нелья отказаться!", "Предупреждение");
            }
            else
            {
                int stat = 4;
                int sotr = 1;

                sql.OpenConnection();

                try
                {
                    // Обновление данных в таблице Заявки
                    string updateQuery = "UPDATE Заявки SET Код_Статуса = @stat " +
                                         "WHERE Код_Заяки = @selectedZayavkaId";

                    SqlCommand updateCommand = new SqlCommand(updateQuery, sql.str);
                    updateCommand.Parameters.AddWithValue("@stat", stat);
                    updateCommand.Parameters.AddWithValue("@selectedZayavkaId", selectedZayavkaId);

                    updateCommand.ExecuteNonQuery();

                    string updateRepairQuery = "UPDATE Заявки_на_ремонт SET Код_Сотрудника = @idSotrudCB WHERE Код_Заявки = @selectedZayavkaId";
                    SqlCommand updateRepairCommand = new SqlCommand(updateRepairQuery, sql.str);
                    updateRepairCommand.Parameters.AddWithValue("@idSotrudCB", sotr);
                    updateRepairCommand.Parameters.AddWithValue("@selectedZayavkaId", selectedZayavkaId);

                    updateRepairCommand.ExecuteNonQuery();

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

            LoadZayavkiList();
        }

        private void KolTB_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                UpdateListBox();
            }
        }

        private void UpdateListBox()
        {
            // Получаем выбранный материал
            string selectedMaterial = null;

            if (MaterialCB.SelectedItem is MaterialClass materialClass)
            {
                selectedMaterial = materialClass.Name;
            }
            else if (MaterialCB.SelectedItem is string materialString)
            {
                selectedMaterial = materialString;
            }

            // Если выбран материал
            if (!string.IsNullOrEmpty(selectedMaterial))
            {
                // Проверяем, введено ли количество в TextBox
                if (!string.IsNullOrEmpty(KolTB.Text))
                {
                    // Получаем количество
                    if (int.TryParse(KolTB.Text, out int quantity))
                    {
                        // Добавляем запись в ListBox
                        string listItem = $"{selectedMaterial} - {quantity}";
                        list.Items.Add(listItem);
                    }
                    else
                    {
                        // Введите корректное количество
                        MessageBox.Show("Введите корректное количество.");
                    }
                }
                else
                {
                    // Если количество не введено, добавляем запись с количеством 1
                    string listItem = $"{selectedMaterial} 1";
                    list.Items.Add(listItem);
                }
            }

            // Очищаем TextBox после обновления ListBox
            KolTB.Clear();
        }

        public string materialName, materialCost;
        private void MaterialCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            materialSelect = MaterialCB.SelectedItem as MaterialClass;
            if (materialSelect != null)
            {
                idMaterial = materialSelect.Id;
                materialName = materialSelect.Name;
                materialCost = materialSelect.Cost;
            }
        }

        private void DelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (list.SelectedItem != null)
            {
                list.Items.Remove(list.SelectedItem);
            }
            else
            {
                MessageBox.Show("Выберите материал из списка");
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateListBox();
        }

        private void OrderBtn_Click(object sender, RoutedEventArgs e)
        {

            ZayavkaItems selectedZayavka = ZayavkiLV.SelectedItem as ZayavkaItems;

            if (selectedZayavka == null)
            {
                MessageBox.Show("Выберите заявку", "Предупреждение");
            }
            else
            {
                sql.OpenConnection();
                foreach (var item in list.Items)
                {
                    // Получаем наименование из ListBox
                    string itemName = item.ToString();

                    // Проверяем, является ли запись в ListBox строкой в формате "Наименование - Количество"
                    string[] parts = itemName.Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length == 2)
                    {
                        string materialName = parts[0];
                        int quantity = int.Parse(parts[1]);

                        // Ищем id по наименованию материала в базе данных
                        int idMaterial = GetMaterialId(materialName);

                        if (idMaterial != 0)
                        {
                            
                            // Получаем стоимость материала из базы данных
                            int materialCost = GetMaterialCost(idMaterial);

                            // Рассчитываем общую стоимость для данного материала
                            int totalCost = quantity * materialCost;

                            sql.OpenConnection();
                            // Создаем SQL-запрос для вставки данных в таблицу
                            string insertQuery = $"INSERT INTO Заказ_материалов (Код_Заявки, Код_Материала, Количество, Стоимость) " +
                                                 $"VALUES ('{selectedZayavka.IdZayavi}', '{idMaterial}', '{quantity}', '{totalCost}')";

                            // Выполняем SQL-запрос
                            SqlCommand insertCommand = new SqlCommand(insertQuery, sql.str);
                            insertCommand.ExecuteNonQuery();
                            sql.CloseConnection();

                            MessageBox.Show("Материалы заказаны");
                        }
                        else
                        {
                            MessageBox.Show($"Не удалось найти id для материала '{materialName}'.");
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Некорректный формат записи в ListBox: '{itemName}'.");
                    }
                }
            }
        }

        private int GetMaterialId(string materialName)
        {
            sql.OpenConnection();
            string query = $"SELECT Код_Материала FROM Материалы WHERE Наименование = '{materialName}'";

            SqlCommand command = new SqlCommand(query, sql.str);

            object result = command.ExecuteScalar();

            if (result != null)
            {
                sql.CloseConnection();
                return Convert.ToInt32(result);
            }

            sql.CloseConnection();
            return 0;
        }

        private int GetMaterialCost(int idMaterial)
        {
            sql.OpenConnection();
            string query = $"SELECT Цена FROM Материалы WHERE Код_Материала = '{idMaterial}'";
            SqlCommand command = new SqlCommand(query, sql.str);
            SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                string priceAsString = reader["Цена"].ToString();

                if (int.TryParse(priceAsString, out int result))
                {
                    sql.CloseConnection();
                    return result;
                }
                else
                {
                    // Обработка случая, когда преобразование не удалось
                    sql.CloseConnection();
                    MessageBox.Show($"Не удалось преобразовать '{priceAsString}' в целое число.");
                    return 0;
                }
            }
            else
            {
                // Обработка случая, когда записи не найдены
                sql.CloseConnection();
                MessageBox.Show($"Записи для Код_Материала = '{idMaterial}' не найдены.");
                return 0;
            }

        }

        private void nazad_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }
    }

    public class MaterialClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Cost { get; set; }
    }
}
