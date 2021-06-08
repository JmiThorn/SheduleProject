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
    public partial class ClassroomsView : Page
    {
        public delegate Task Updater();
        public event Updater UpdateParent;
        public ClassroomsView(Classroom classroom, Updater updater)
        {
            InitializeComponent();
            DataContext = classroom;
            loadTeachers();
            UpdateParent += updater;
        }

        public async Task loadTeachers()
        {
            AppUtils.ProcessClientLibraryRequest(async () =>
            {
                var specialities = await LearningProcessesAPI.getAllTeachers();
                affiliationCB.ItemsSource = specialities;
            });
        }

        private void edit_butt_Click(object sender, RoutedEventArgs e)
        {
            building.IsEnabled = true;
            number.IsEnabled = true;
            affiliationCB.IsEnabled = true;
        }

        private async void save_butt_Click(object sender, RoutedEventArgs e)
        {
            AppUtils.ProcessClientLibraryRequest(async () =>
            {
                building.GetBindingExpression(ComboBox.SelectedValueProperty).UpdateSource();
                number.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                affiliationCB.GetBindingExpression(ComboBox.SelectedValueProperty).UpdateSource();
                Classroom classroom = (Classroom)DataContext;
                var result = await LearningProcessesAPI.updateClassroom(classroom.Id, classroom);
                MessageBox.Show("Данные успешно обновлены", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                UpdateParent?.Invoke();
            });
        }

        private void number_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if ((e.Text) == null || !(e.Text).All(char.IsDigit) || number.Text.Length >= 3)
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
