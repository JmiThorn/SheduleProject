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
using LearningProcessesAPIClient.api;
using Shedule.Pages;
using Shedule.ViewPages;

namespace Shedule
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static MainWindow Instance { get; set; }
        public MainWindow()
        {
            Instance = this;
            InitializeComponent();
            //LearningProcessesAPI.setEndpointAddress("http://128.1.13.152:444");
            //LearningProcessesAPI.setEndpointAddress("http://localhost:666");
            try
            {
                var String = Properties.Settings.Default.ConnectionString;
                var login = Properties.Settings.Default.Login;
                var password = Properties.Settings.Default.Password;
                LearningProcessesAPI.setEndpointAddress(String);
                LearningProcessesAPI.setCredentionals(login, password);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            //LearningProcessesAPI.setCredentionals(,);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Frame1.Visibility = Visibility.Visible;
            MainFrame.Navigate(new SheduleView());
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new TeachersList());
        }

        private void TextBlock_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new Import());
        }

        private void Chang_but_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Сhanges());
        }

        private void Settings_butt_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Settings());
        }

        private void spec_butt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new SpecialtiesList());
        }

        private void teachers_butt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new TeachingList());
        }

        private void aud_butt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new АudiencesList());
        }

        private void groups_butt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new GroupsList());
        }

        private void plan_butt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new Сhanges());
        }

        private void disciplines_butt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new DisciplinesList());
        }

        //private void semesters_butt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    MainFrame.Navigate(new Semesters());
        //}

        private void department_butt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new BranchesList());
        }

        private void TextBlock_MouseLeftButtonDown_2(object sender, MouseButtonEventArgs e)
        {

        }
    }
    }
