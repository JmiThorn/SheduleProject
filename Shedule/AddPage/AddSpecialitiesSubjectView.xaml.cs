using LearningProcessesAPIClient.api;
using LearningProcessesAPIClient.model;
using Shedule.Utils;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Shedule.ViewPages
{
    /// <summary>
    /// Логика взаимодействия для SpecialitiesSubjectView.xaml
    /// </summary>
    public partial class AddSpecialitiesSubjectView : Page
    {

        public delegate Task Updater();
        public event Updater UpdateParent;

        public AddSpecialitiesSubjectView(Speciality speciality, Updater updater)
        {
            InitializeComponent();
            DataContext = speciality;
            loadSubject();
            UpdateParent += updater;
        }
        public async Task loadSubject()
        {
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                var specialities = await LearningProcessesAPI.getAllSubjects();
                nameCB.ItemsSource = specialities;
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
                var result = await LearningProcessesAPI.createSpecialitySubject(((Speciality)(DataContext)).Id, Convert.ToInt32(nameCB.SelectedValue), code.Text);
                AppUtils.PageContentAreSaved = true;
                MessageBox.Show("Дисциплина успешно добавлена", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                UpdateParent?.Invoke();
            });
        }

        private void nameCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
                AppUtils.PageContentAreSaved = false;
        }
    }
}
