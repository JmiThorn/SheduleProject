using LearningProcessesAPIClient.api;
using LearningProcessesAPIClient.model;
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
    /// Логика взаимодействия для DepartmentsView.xaml
    /// </summary>
    public partial class DepartmentsView : Page
    {
        public DepartmentsView(Department department)
        {
            InitializeComponent();
            DataContext = department;
        }

        private void edit_butt_Click(object sender, RoutedEventArgs e)
        {
            name.IsEnabled = true;
        }

        private async void save_butt_Click(object sender, RoutedEventArgs e)
        {
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                name.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                Department department = (Department)DataContext;
                var result = await LearningProcessesAPI.updateDepartment(department.Id, department);
                MessageBox.Show("Данные успешно обновлены", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
