using LearningProcessesAPIClient.api;
using LearningProcessesAPIClient.model;
using Shedule.Pages;
using Shedule.Utils;
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
    public partial class AddSemestersView : Page
    {
        public AddSemestersView(Group group)
        {
            InitializeComponent();
            DataContext = group;
        }


        private async void save_butt_Click(object sender, RoutedEventArgs e)
        {
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                var result = await LearningProcessesAPI.createSemester(Convert.ToInt32(weekscount.Text), Convert.ToDateTime(startdate.Text), Convert.ToInt32(number.Text), Convert.ToInt32(((Group)DataContext).Id));
                MessageBox.Show("Семестр успешно добавлен", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            });
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
