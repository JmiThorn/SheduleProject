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
    /// Логика взаимодействия для ClassroomsView.xaml
    /// </summary>
    public partial class AddClassroomsView : Page
    {
        public delegate Task Updater();
        public event Updater UpdateParent;
        public AddClassroomsView(Updater updater)
        {
            InitializeComponent();
            loadTeachers();
            UpdateParent += updater;
        }

        public async Task loadTeachers()
        {
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                var specialities = await LearningProcessesAPI.getAllTeachers();
                affiliationCB.ItemsSource = specialities;
            });
        }

        private async Task save()
        {
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                Teacher teacher;
                teacher = null;
                if ((affiliationCB.SelectedIndex) == -1)
                {
                    var result = await LearningProcessesAPI.createClassroom(Convert.ToInt32(number.Text), Convert.ToInt32(building.SelectedIndex) + 1, teacher);
                    MessageBox.Show("Аудитория успешно добавлена", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                    UpdateParent?.Invoke();
                }
                else
                {
                    var result = await LearningProcessesAPI.createClassroom(Convert.ToInt32(number.Text), Convert.ToInt32(building.SelectedIndex) + 1, Convert.ToInt32(affiliationCB.SelectedValue));
                    MessageBox.Show("Аудитория успешно добавлена", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                AppUtils.PageContentAreSaved = true;
            });
        }

        private void save_butt_Click(object sender, RoutedEventArgs e)
        {
            save();
        }

        private void number_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if ((e.Text) == null || !(e.Text).All(char.IsDigit) || number.Text.Length>=3)
            {
                e.Handled = true;
            }
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void building_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            affiliationCB.SelectedIndex = -1;
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(IsLoaded)
                AppUtils.PageContentAreSaved = false;
        }
    }
}
