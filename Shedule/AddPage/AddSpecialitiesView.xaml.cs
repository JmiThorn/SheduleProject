using LearningProcessesAPIClient.model;
using LearningProcessesAPIClient.api;
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
    /// Логика взаимодействия для SpecialitiesView.xaml
    /// </summary>
    public partial class AddSpecialitiesView : Page
    {
        public AddSpecialitiesView()
        {
            InitializeComponent();
            loadDepartment();
        }
        public async Task loadDepartment()
        {
            var department = await LearningProcessesAPI.getAllDepartments();
            departmentCB.ItemsSource = department;
        }

        private async void save_butt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = await LearningProcessesAPI.createSpeciality(name.Text,Convert.ToInt32(day.Text),Convert.ToInt32(week.Text), Convert.ToInt32(departmentCB.SelectedValue), code.Text,codename.Text);
                MessageBox.Show("Специльность успешно добавлена", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void edit_butt_Click(object sender, RoutedEventArgs e)
        {
            code.IsEnabled = true;
            codename.IsEnabled = true;
            name.IsEnabled = true;
            departmentCB.IsEnabled = true;
            day.IsEnabled = true;
            week.IsEnabled = true;
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
