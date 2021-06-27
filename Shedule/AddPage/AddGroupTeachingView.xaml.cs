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
    public partial class AddGroupTeachingView : Page
    {
        public delegate Task Updater();
        public event Updater UpdateParent;
        public AddGroupTeachingView(Group group, Updater updater)
        {
            InitializeComponent();
            DataContext = group;
            teachigGrid.DataContext = new Teaching() { GroupId = group.Id };
            loadSpecSubject();
            loadTeacher();
            UpdateParent += updater;
        }
        public async Task loadTeacher()
        {
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                var teacher = await LearningProcessesAPI.getAllTeachers();
                teacherCB.ItemsSource = teacher;
            });
        }
        public async Task loadSpecSubject()
        {
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                var subjects = await LearningProcessesAPI.getSpecialitySubjects(((Group)DataContext).SpecialityId);
                name.ItemsSource = subjects;
            });
        }
        private void back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
        private void save_butt_Click(object sender, RoutedEventArgs e)
        {
            save();
        }

        private async Task save()
        {
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                var result = await LearningProcessesAPI.createTeaching(
                    ((Teaching)teachigGrid.DataContext).GroupId,
                    ((Teaching)teachigGrid.DataContext).SpecialitySubjectId,
                    ((Teaching)teachigGrid.DataContext).TeacherId
                );
                MessageBox.Show("Данные успешно обновлены", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                UpdateParent?.Invoke();
                AppUtils.PageContentAreSaved = true;
            });
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
                AppUtils.PageContentAreSaved = false;
        }
    }
}
