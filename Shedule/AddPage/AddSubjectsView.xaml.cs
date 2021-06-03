using LearningProcessesAPIClient.api;
using LearningProcessesAPIClient.model;
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
    /// Логика взаимодействия для SubjectsView.xaml
    /// </summary>
    public partial class AddSubjectsView : Page
    {
        public AddSubjectsView()
        {
            InitializeComponent();
        }
        private async void save_butt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
              
                var result = await LearningProcessesAPI.createSubject(name.Text, false);
                MessageBox.Show("Предмет успешно добавлен", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
