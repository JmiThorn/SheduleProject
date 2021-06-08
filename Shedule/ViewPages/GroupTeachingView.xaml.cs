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
    /// Логика взаимодействия для GroupTeachingView.xaml
    /// </summary>
    public partial class GroupTeachingView : Page
    {
        public delegate Task Updater();
        public event Updater UpdateParent;
        public GroupTeachingView(Teaching teaching, Updater updater)
        {
            InitializeComponent();
            DataContext = teaching;
            loadTeacher();
            UpdateParent += updater;
        }
        public async Task loadTeacher()
        {
            AppUtils.ProcessClientLibraryRequest(async () =>
            {
                var teacher = await LearningProcessesAPI.getAllTeachers();
                teacherCB.ItemsSource = teacher;
            });
        }
        private void back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
        private async void save_butt_Click(object sender, RoutedEventArgs e)
        {
            AppUtils.ProcessClientLibraryRequest(async () =>
            {
                name.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                teacherCB.GetBindingExpression(ComboBox.SelectedValueProperty).UpdateSource();
                Teaching teaching = (Teaching)DataContext;
                var result = await LearningProcessesAPI.updateTeaching(teaching.Id, teaching);
                MessageBox.Show("Данные успешно обновлены", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                UpdateParent?.Invoke();
            });
        }
        private void edit_butt_Click(object sender, RoutedEventArgs e)
        {
            teacherCB.IsEnabled = true;
        }
    }
}
