using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using didi_UP01.Classes;

namespace didi_UP01
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string loginPerem;
        public string passPerem;

        Authorization authorization = new Authorization();
        public MainWindow()
        {
            InitializeComponent();

            chkShowPassword.Checked += ChkShowPassword_Checked;
            chkShowPassword.Unchecked += ChkShowPassword_Unchecked;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            loginPerem = txtLogin.Text;
            if (chkShowPassword.IsChecked == true)
            {
                passPerem = txtPasswordTB.Text;
            }
            else
            {
                passPerem = txtPasswordPB.Password;
            }

            if (authorization.AuthenticateUser(loginPerem, passPerem))
            {
                this.Close();
            }
        }


        private void ChkShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            txtPasswordPB.Visibility = Visibility.Collapsed;
            txtPasswordTB.Visibility = Visibility.Visible;
            txtPasswordTB.Text = txtPasswordPB.Password; 
            
        }

        private void ChkShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            txtPasswordTB.Visibility = Visibility.Collapsed;
            txtPasswordPB.Visibility = Visibility.Visible;
            txtPasswordPB.Password = txtPasswordTB.Text; 
        }

        private void Vihod_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
