using LearningProcessesAPIClient.api;
using Microsoft.Win32;
using Shedule.Models.Parsing;
using Shedule.Utils;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Shedule.Pages
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {

        public Settings()
        {
            InitializeComponent();
            ConnectionString.Text = Properties.Settings.Default.ConnectionString;
            Login_field.Text = Properties.Settings.Default.Login;
            Password_field.Password = Properties.Settings.Default.Password;

            //string days = Properties.Settings.Default.WorkDays;
            //WorkDays.Items.Add("4");
            //WorkDays.Items.Add("5");
            //WorkDays.Items.Add("6");
            //WorkDays.SelectedItem = days;

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ///////////////////////////////////////////////////////////////////////////
            try
            {
                Properties.Settings.Default.ConnectionString = ConnectionString.Text;
                Properties.Settings.Default.Login = Login_field.Text;
                Properties.Settings.Default.Password = Password_field.Password;
                //Properties.Settings.Default.WorkDays = WorkDays.SelectionBoxItem.ToString();
                Properties.Settings.Default.Save();
                MessageBox.Show("Данные успешно обновлены");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            ///////////////////////////////////////////////////////////////////////////
            try
            {
                var String = Properties.Settings.Default.ConnectionString;
                LearningProcessesAPI.setEndpointAddress(String);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            ///////////////////////////////////////////////////////////////////////////проверка валидности логина и пароля
            try
            {
                var login = Properties.Settings.Default.Login;
                var password = Properties.Settings.Default.Password;
                LearningProcessesAPI.setCredentionals(login, password);
                AppUtils.PageContentAreSaved = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не корректный логин или пароль, проверьте правильность и повторите попытку");
            }
        }
    }
}
