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
using Shedule.Utils;

namespace Shedule.ViewPages
{
    /// <summary>
    /// Логика взаимодействия для SpecialitiesView.xaml
    /// </summary>
    public partial class AddSpecialitiesView : Page
    {
        public delegate Task Updater();
        public event Updater UpdateParent;
        public AddSpecialitiesView(Updater updater)
        {
            InitializeComponent();
            loadDepartment();
            UpdateParent += updater;
        }
        public async Task loadDepartment()
        {
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                var department = await LearningProcessesAPI.getAllDepartments();
                departmentCB.ItemsSource = department;
            });
        }

        private void save_butt_Click(object sender, RoutedEventArgs e)
        {
            save();
        }

        private async Task save()
        {
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                if (Convert.ToInt32(week.Text) > 168 || Convert.ToInt32(day.Text) > 24)
                {
                    MessageBox.Show("Неправильное количество выставленных часов", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {

                    var result = await LearningProcessesAPI.createSpeciality(name.Text, Convert.ToInt32(day.Text), Convert.ToInt32(week.Text), Convert.ToInt32(departmentCB.SelectedValue), code.Text, codename.Text);
                    MessageBox.Show("Специльность успешно добавлена", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                    UpdateParent?.Invoke();
                }
                AppUtils.PageContentAreSaved = true;
            });
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

        private void day_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if ((e.Text) == null || !(e.Text).All(char.IsDigit) || (sender as TextBox).Text.Length >= 2)
            {
                e.Handled = true;
            }
        }

        private void week_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
                if ((e.Text) == null || !(e.Text).All(char.IsDigit) || (sender as TextBox).Text.Length >= 3)
                {
                    e.Handled = true;
            }
        }

        private void departmentCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
                AppUtils.PageContentAreSaved = false;
        }
    }
}
