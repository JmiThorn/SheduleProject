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
    /// Логика взаимодействия для GroupsView.xaml
    /// </summary>
    public partial class AddGroupsView : Page
    {
        public delegate Task Updater();
        public event Updater UpdateParent;
        public AddGroupsView(Updater updater)
        {
            InitializeComponent();
            loadSpeciality();
            UpdateParent += updater;
        }

        private void edit_butt_Click(object sender, RoutedEventArgs e)
        {
           // codename.IsEnabled = true;
            course.IsEnabled = true;
            subgroup.IsEnabled = true;
            specialityCB.IsEnabled = true;
        }
        public async Task loadSpeciality()
        {
            AppUtils.ProcessClientLibraryRequest(async () =>
            {
                var specialities = await LearningProcessesAPI.getAllSpecialities();
                specialityCB.ItemsSource = specialities;
            });
        }

        private async void save_butt_Click(object sender, RoutedEventArgs e)
        {
            AppUtils.ProcessClientLibraryRequest(async () =>
            {
                if (subgroup.Text == "")
                {
                    var result = await LearningProcessesAPI.createGroup(Convert.ToInt32(course.Text), null, true, Convert.ToInt32(specialityCB.SelectedValue));
                    MessageBox.Show("Группа добавлена успешно", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                    UpdateParent?.Invoke();
                }
                else
                {

                    var result = await LearningProcessesAPI.createGroup(Convert.ToInt32(course.Text), Convert.ToInt32(subgroup.Text), true, Convert.ToInt32(specialityCB.SelectedValue));
                    MessageBox.Show("Группа добавлена успешно", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                    UpdateParent?.Invoke();
                }
            });
        }
        private void DigitCheck_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if ((e.Text) == null || !(e.Text).All(char.IsDigit) || (sender as TextBox).Text.Length >= 1)
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
