using LearningProcessesAPIClient.api;
using LearningProcessesAPIClient.model;
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
        public AddGroupsView()
        {
            InitializeComponent();
            loadSpeciality();
        }

        private void edit_butt_Click(object sender, RoutedEventArgs e)
        {
            codename.IsEnabled = true;
            course.IsEnabled = true;
            subgroup.IsEnabled = true;
            specialityCB.IsEnabled = true;
        }
        public async Task loadSpeciality()
        {
            var specialities = await LearningProcessesAPI.getAllSpecialities();
            specialityCB.ItemsSource = specialities;
        }

        private async void save_butt_Click(object sender, RoutedEventArgs e)
        {
            try
            {   
                var result = await LearningProcessesAPI.createGroup(Convert.ToInt32(course.Text),Convert.ToInt32(subgroup.Text),true, Convert.ToInt32(specialityCB.SelectedValue));
                MessageBox.Show("Группа добавлена успешно", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }
        private void DigitCheck_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if ((e.Text) == null || !(e.Text).All(char.IsDigit) || subgroup.Text.Length >= 1 || course.Text.Length >=1)
            {
                e.Handled = true;
            }
        }
    }
}
