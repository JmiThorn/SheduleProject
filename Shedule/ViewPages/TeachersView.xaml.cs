using LearningProcessesAPIClient;
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
    /// Логика взаимодействия для TeachersList.xaml
    /// </summary>
    public partial class TeachersView : Page
    {
        public TeachersView(Teacher teacher)
        {
            InitializeComponent();
            DataContext = teacher;
            loadDepartments();
        }

        private void edit_butt_Click(object sender, RoutedEventArgs e)
        {
            name.IsEnabled = true;
            surname.IsEnabled = true;
            patronymic.IsEnabled = true;
            departmentsCB.IsEnabled = true;
            AppUtils.PageContentAreSaved = false;
        }

        public async Task loadDepartments()
        {
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                var departments = await LearningProcessesAPI.getAllDepartments();
                departmentsCB.ItemsSource = departments;
            });
        }
        private async Task save()
        {
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                name.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                surname.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                patronymic.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                departmentsCB.GetBindingExpression(ComboBox.SelectedValueProperty).UpdateSource();
                Teacher teacher = (Teacher)DataContext;
                //teacher.DepartmentId = 1;
                var result = await LearningProcessesAPI.updateTeacher(teacher.Id, teacher);
                AppUtils.PageContentAreSaved = true;
                MessageBox.Show("Данные успешно обновлены", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }
        private void save_butt_Click(object sender, RoutedEventArgs e)
        {
            save();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
