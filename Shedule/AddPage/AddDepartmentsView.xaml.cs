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
    public partial class AddDepartmentsView : Page
    {
        public delegate Task Updater();
        public event Updater UpdateParent;
        public AddDepartmentsView(Updater updater)
        {
            InitializeComponent();
            UpdateParent += updater;
        }



        private async void save_butt_Click(object sender, RoutedEventArgs e)
        {
            AppUtils.ProcessClientLibraryRequest(async () =>
            {
                var result = await LearningProcessesAPI.createDepartment(name.Text);
                MessageBox.Show("Отделение успешно добавлено", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                UpdateParent?.Invoke();
            });
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
