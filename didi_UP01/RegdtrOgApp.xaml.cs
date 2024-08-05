using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using didi_UP01.Classes;

namespace didi_UP01
{
    /// <summary>
    /// Логика взаимодействия для RegdtrOgApp.xaml
    /// </summary>
    public partial class RegdtrOgApp : Window
    {
        private List<Equipment> equipmentList;
        private List<TypeMalfun> typeMalfunList;
        private List<Customers> customersList;
        private List<Workers> workersList;

        private List<ZayavkaItems> zayavkiList;
        private List<CustomersItems> customersItems;

        private Equipment equipmentSelect;
        private TypeMalfun typeMalSelect;
        private Customers custSelect;
        private Workers workSelect;
        public int idOborCB, idTneisCB, idKCientCB, idStrCB;

        public RegdtrOgApp()
        {
            InitializeComponent();

            LoadEquipmentList();
            //LoadTypeMalfun();
            LoadCustomers();
            LoadWorkers();

            LoadZayavkiList();
            LoadCustList();
        }
        private void LoadEquipmentList()
        {
            equipmentList = new List<Equipment>();
            try
            {
                sql.OpenConnection();
                string query = "SELECT Код_Оборудования, Наименование FROM Оборудование";
                SqlCommand command = new SqlCommand(query, sql.str);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Equipment equipment = new Equipment
                    {
                        EquipmentCode = reader.GetInt32(reader.GetOrdinal("Код_Оборудования")),
                        Name = reader.GetString(reader.GetOrdinal("Наименование"))
                    };
                    equipmentList.Add(equipment);
                }
                // Установка списка в качестве источника данных для ComboBox
                OborudCB.ItemsSource = equipmentList;
                OborudCB.DisplayMemberPath = "Name"; // Отображение наименования в ComboBox
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

        private void LoadCustomers()
        {
            customersList = new List<Customers>();
            try
            {
                sql.OpenConnection();
                string query = "SELECT Код_Клиента, CONCAT(Фамилия, ' ',  Имя, ' ', Отчество) AS ФИО FROM Клиенты";
                SqlCommand command = new SqlCommand(query, sql.str);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Customers cuts = new Customers
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Код_Клиента")),
                        Name = reader.GetString(reader.GetOrdinal("ФИО"))
                    };
                    customersList.Add(cuts);
                }

                ClientCB.ItemsSource = customersList;
                ClientCB.DisplayMemberPath = "Name";
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

        private void LoadZayavkiList()
        {
            zayavkiList = ZayavkaItems.LoadDataFromDatabase();
            ZayavkiLV.ItemsSource = zayavkiList;
        }

        private void LoadCustList()
        {
            customersItems = CustomersItems.LoadDataFromDatabase2();
            ClientLV.ItemsSource = customersItems;
        }       

        private void Oformit_Click(object sender, RoutedEventArgs e)
        {
            string opisProbl = txtData.Text;
            DateTime currentDate = DateTime.Now;
            int stat = 1;
            int prior = 1;
            int sexteen = 16;
            string dateEnd = DateEnd.Text;

            sql.OpenConnection();

            try
            {

                // Добавление заявки в таблицу Заявки
                string insertQuery = "INSERT INTO Заявки (Дата_добавления, Код_Оборудования, Код_Неисправности, Описание_проблемы, Код_Клиента, Код_Статуса, Код_Приоритета, Дата_окончания) " +
                                    "VALUES (@dateAdd, @idOborCB, @sexteen, @opisProbl, @idKCientCB, @stat, @prior, @dateEnd);" +
                                    "SELECT SCOPE_IDENTITY();"; // Получение идентификатора добавленной записи

                SqlCommand insertCommand = new SqlCommand(insertQuery, sql.str);
                insertCommand.Parameters.AddWithValue("@dateAdd", currentDate);
                insertCommand.Parameters.AddWithValue("@idOborCB", idOborCB);
                insertCommand.Parameters.AddWithValue("@sexteen", sexteen);
                insertCommand.Parameters.AddWithValue("@opisProbl", opisProbl);
                insertCommand.Parameters.AddWithValue("@idKCientCB", idKCientCB);
                insertCommand.Parameters.AddWithValue("@stat", stat);
                insertCommand.Parameters.AddWithValue("@prior", prior);
                insertCommand.Parameters.AddWithValue("@dateEnd", dateEnd);

                int newZayavkaId = Convert.ToInt32(insertCommand.ExecuteScalar()); // Получение идентификатора новой заявки

                // Добавление заявки в таблицу Заявки_на_ремонт
                string insertRepairQuery = "INSERT INTO Заявки_на_ремонт (Код_Сотрудника, Код_Заявки) VALUES (@idSotrudCB, @newZayavkaId)";
                SqlCommand insertRepairCommand = new SqlCommand(insertRepairQuery, sql.str);
                insertRepairCommand.Parameters.AddWithValue("@idSotrudCB", idStrCB);
                insertRepairCommand.Parameters.AddWithValue("@newZayavkaId", newZayavkaId);

                insertRepairCommand.ExecuteNonQuery();

                MessageBox.Show("Заявка успешно добавлена");
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
        }

        private void DobClienta_Click(object sender, RoutedEventArgs e)
        {
            // Проверка наличия данных во всех TextBox
            if (string.IsNullOrEmpty(FamilTB.Text) || string.IsNullOrEmpty(ImaTB.Text) ||
                string.IsNullOrEmpty(OtchesTB.Text) || string.IsNullOrEmpty(NomerTelTB.Text) ||
                string.IsNullOrEmpty(AdresTB.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            // Проверка корректности номера телефона с использованием Regex
            string phoneNumber = NomerTelTB.Text;
            if (!Regex.IsMatch(phoneNumber, @"^\d{10}$"))
            {
                MessageBox.Show("Некорректный номер телефона. Пожалуйста, введите 10 цифр без пробелов и других символов.");
                return;
            }

            sql.OpenConnection();

            try
            {
                string insertQuery = "INSERT INTO Клиенты (Фамилия, Имя, Отчество, Номер_телефона, Адрес) " +
                                    "VALUES (@FamilTB, @ImaTB, @OtchesTB, @NomerTelTB, @AdresTB)";

                SqlCommand insertCommand = new SqlCommand(insertQuery, sql.str);
                insertCommand.Parameters.AddWithValue("@FamilTB", FamilTB.Text);
                insertCommand.Parameters.AddWithValue("@ImaTB", ImaTB.Text);
                insertCommand.Parameters.AddWithValue("@OtchesTB", OtchesTB.Text);
                insertCommand.Parameters.AddWithValue("@NomerTelTB", NomerTelTB.Text);
                insertCommand.Parameters.AddWithValue("@AdresTB", AdresTB.Text);

                insertCommand.ExecuteNonQuery();

                MessageBox.Show("Клиент успешно добавлен.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
            finally
            {
                sql.CloseConnection();
            }

            LoadCustList();
        }

        public int selectedZayavkaId;
        public string opisanie;
        private void ZayavkiLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ZayavkaItems selectedZayavka = ZayavkiLV.SelectedItem as ZayavkaItems;

            if (selectedZayavka != null)
            {
                OborudCB.SelectedItem = equipmentList.FirstOrDefault(equipment => equipment.Name == selectedZayavka.Oborud);;

                ClientCB.SelectedItem = customersList.FirstOrDefault(customer => customer.Name == selectedZayavka.Client);
                SotrudCB.SelectedItem = workersList.FirstOrDefault(worker => worker.Name == selectedZayavka.Sotrud);

                selectedZayavkaId = selectedZayavka.IdZayavi;

                sql.OpenConnection();
                string query = "select Описание_проблемы from Заявки where Код_Заяки = @selectedZayavkaId";
                SqlCommand command = new SqlCommand(query, sql.str);
                command.Parameters.AddWithValue("@selectedZayavkaId", selectedZayavkaId);

                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                //opisanie = reader.GetString(reader.GetOrdinal("Описание_проблемы"));
                reader.Close();
                sql.CloseConnection();

                txtData.Text = opisanie;
            }

            Oformit.IsEnabled = false;
            Red.IsEnabled = true;
            ExitRed.Visibility = Visibility.Visible;
            Islp.Visibility = Visibility.Collapsed;
            dopIspl.Visibility = Visibility.Visible;


            ClientCB.IsEnabled = false;
        }

        private void Red_Click(object sender, RoutedEventArgs e)
        {
            string opisProbl = txtData.Text;
            DateTime currentDate = DateTime.Now;
            int stat = 1;
            int prior = 1;
            int sexteen = 16;
            DateTime dateEnd = DateEnd.DisplayDate;

            sql.OpenConnection();

            try
            {
                // Проверка, существует ли запись для выбранной заявки и сотрудника
                string checkExistenceQuery = "SELECT COUNT(*) FROM Заявки_на_ремонт WHERE Код_Заявки = @selectedZayavkaId AND Код_Сотрудника = @idSotrudCB";
                SqlCommand checkExistenceCommand = new SqlCommand(checkExistenceQuery, sql.str);
                checkExistenceCommand.Parameters.AddWithValue("@selectedZayavkaId", selectedZayavkaId-1);
                checkExistenceCommand.Parameters.AddWithValue("@idSotrudCB", idStrCB);

                int existenceCount = (int)checkExistenceCommand.ExecuteScalar();

                if (existenceCount > 0)
                {
                    // Обновление данных в таблице Заявки
                    string updateQuery = "UPDATE Заявки SET Код_Оборудования = @idOborCB, " +
                                         "Описание_проблемы = @opisProbl, " +
                                         "Код_Клиента = @idKCientCB, " +
                                         "Дата_окончания = @dateEnd " +
                                         "WHERE Код_Заяки = @selectedZayavkaId";

                    SqlCommand updateCommand = new SqlCommand(updateQuery, sql.str);
                    updateCommand.Parameters.AddWithValue("@idOborCB", idOborCB);
                    updateCommand.Parameters.AddWithValue("@opisProbl", opisProbl);
                    updateCommand.Parameters.AddWithValue("@idKCientCB", idKCientCB);
                    updateCommand.Parameters.AddWithValue("@dateEnd", dateEnd);
                    updateCommand.Parameters.AddWithValue("@selectedZayavkaId", selectedZayavkaId);

                    updateCommand.ExecuteNonQuery();

                    MessageBox.Show("Заявка успешно обновлена");
                }
                else
                {
                    // Добавление новой записи
                    // Добавление заявки в таблицу Заявки
                    string insertQuery = "INSERT INTO Заявки (Дата_добавления, Код_Оборудования, Код_Неисправности, Описание_проблемы, Код_Клиента, Код_Статуса, Код_Приоритета, Дата_окончания) " +
                                        "VALUES (@dateAdd, @idOborCB, @sexteen, @opisProbl, @idKCientCB, @stat, @prior, @dateEnd);" +
                                        "SELECT SCOPE_IDENTITY();"; // Получение идентификатора добавленной записи

                    SqlCommand insertCommand = new SqlCommand(insertQuery, sql.str);
                    insertCommand.Parameters.AddWithValue("@dateAdd", currentDate);
                    insertCommand.Parameters.AddWithValue("@idOborCB", idOborCB);
                    insertCommand.Parameters.AddWithValue("@sexteen", sexteen);
                    insertCommand.Parameters.AddWithValue("@opisProbl", opisProbl);
                    insertCommand.Parameters.AddWithValue("@idKCientCB", idKCientCB);
                    insertCommand.Parameters.AddWithValue("@stat", stat);
                    insertCommand.Parameters.AddWithValue("@prior", prior);
                    insertCommand.Parameters.AddWithValue("@dateEnd", dateEnd);

                    int newZayavkaId = Convert.ToInt32(insertCommand.ExecuteScalar()); // Получение идентификатора новой заявки

                    // Добавление заявки в таблицу Заявки_на_ремонт
                    string insertRepairQuery = "INSERT INTO Заявки_на_ремонт (Код_Сотрудника, Код_Заявки) VALUES (@idSotrudCB, @newZayavkaId)";
                    SqlCommand insertRepairCommand = new SqlCommand(insertRepairQuery, sql.str);
                    insertRepairCommand.Parameters.AddWithValue("@idSotrudCB", idStrCB);
                    insertRepairCommand.Parameters.AddWithValue("@newZayavkaId", newZayavkaId);

                    insertRepairCommand.ExecuteNonQuery();

                    MessageBox.Show($"Заявка для другого сотрудника успешно добавлена");
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

            LoadZayavkiList();

            Oformit.IsEnabled = true;
            Red.IsEnabled = false;
            ExitRed.Visibility = Visibility.Collapsed;
            Islp.Visibility = Visibility.Visible;
            dopIspl.Visibility = Visibility.Collapsed;

            ClientCB.IsEnabled = true;
        }


        private void ExitRed_Click(object sender, RoutedEventArgs e)
        {
            OborudCB.SelectedItem = null;
            txtData.Text = "";
            ClientCB.SelectedItem = null;
            SotrudCB.SelectedItem = null;

            Oformit.IsEnabled = true;
            Red.IsEnabled = false;
            ExitRed.Visibility = Visibility.Collapsed;

            Islp.Visibility = Visibility.Visible;
            dopIspl.Visibility = Visibility.Collapsed;

            ClientCB.IsEnabled = true;
        }

        private void nazad_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OborudCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            equipmentSelect = OborudCB.SelectedItem as Equipment;
            if (equipmentSelect != null)
            {
                idOborCB = equipmentSelect.EquipmentCode;
            }
        }
        
       
        private void ClientCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            custSelect = ClientCB.SelectedItem as Customers;
            if (custSelect != null)
            {
                idKCientCB = custSelect.Id;
            }
        }

        private void SotrudCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            workSelect = SotrudCB.SelectedItem as Workers;
            if (workSelect != null)
            {
                idStrCB = workSelect.Id;
            }
        }
    }

    public class Equipment
    {
        public int EquipmentCode { get; set; }
        public string Name { get; set; }
    }

    public class TypeMalfun
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class Customers
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Workers
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
