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
using Shedule.Utils;
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
            clearJournal();
            MainFrame.Navigate(new SheduleView());
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            clearJournal();
            MainFrame.Navigate(new TeachersList());
        }

        //private void TextBlock_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        //{
        //    MainFrame.Navigate(new Import());
        //}

        private void Chang_but_Click(object sender, RoutedEventArgs e)
        {
            clearJournal();
            MainFrame.Navigate(new Сhanges());
        }

        private void Settings_butt_Click(object sender, RoutedEventArgs e)
        {
            clearJournal();
            MainFrame.Navigate(new Settings());
        }

        private void spec_butt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            clearJournal();
            MainFrame.Navigate(new SpecialtiesList());
        }

        private void aud_butt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            clearJournal();
            MainFrame.Navigate(new АudiencesList());
        }

        private void groups_butt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            clearJournal();
            MainFrame.Navigate(new GroupsList());
        }

        private void disciplines_butt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            clearJournal();
            MainFrame.Navigate(new DisciplinesList());
        }

        //private void semesters_butt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    MainFrame.Navigate(new Semesters());
        //}

        private void department_butt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            clearJournal();
            MainFrame.Navigate(new BranchesList());
        }

        private void MainFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (!AppUtils.PageContentAreSaved)
            {
                if(MessageBox.Show("Внесеенные изменения не будут сохранены.\nПокинуть страницу?", "Покинуть страницу?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.No)
                {
                    e.Cancel = true;
                    return;
                }
                else
                {
                    AppUtils.PageContentAreSaved = true;
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key != Key.Escape && e.Key != Key.Tab)
            {
                AppUtils.PageContentAreSaved = false;
            }
        }

        //Очистка журнала во избежание утечек памяти
        private void clearJournal()
        {
            while (MainFrame.CanGoBack)
            {
                MainFrame.RemoveBackEntry();
            }
        }

    }
 }
