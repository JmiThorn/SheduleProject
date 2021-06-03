using LearningProcessesAPIClient.api;
using LearningProcessesAPIClient.model;
using Shedule.Pages;
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

namespace Shedule.ViewPages
{
    /// <summary>
    /// Логика взаимодействия для SemestersView.xaml
    /// </summary>
    public partial class SemestersView : Page
    {
        public SemestersView(Semester semester)
        {
            InitializeComponent();
            DataContext = semester;
            loadGroups();
        }
        private void edit_butt_Click(object sender, RoutedEventArgs e)
        {
            weekscount.IsEnabled = true;
            startdate.IsEnabled = true;
            number.IsEnabled = true;
            //groupCB.IsEnabled = true;
        }
        public async Task loadGroups()
        {
            var groups = await LearningProcessesAPI.getAllGroups();
            //groupCB.ItemsSource = groups;
        }

        private async void save_butt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                number.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                startdate.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                weekscount.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                Semester semester  = (Semester)DataContext;
                var result = await LearningProcessesAPI.updateSemester(semester.Id, semester);
                MessageBox.Show("Данные успешно обновлены", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
      
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }
        private void DigitCheck_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if ((e.Text) == null || !(e.Text).All(char.IsDigit) || number.Text.Length >= 2)
            {
                e.Handled = true;
            }
        }
        private void DigitCheck_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {
            if ((e.Text) == null || !(e.Text).All(char.IsDigit) || weekscount.Text.Length >= 2)
            {
                e.Handled = true;
            }
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
