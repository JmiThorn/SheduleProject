using LearningProcessesAPIClient.api;
using LearningProcessesAPIClient.model;
using Shedule.Utils;
using Shedule.ViewPages;
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

namespace Shedule.Pages
{
    /// <summary>
    /// Логика взаимодействия для Specialties.xaml
    /// </summary>
    public partial class SpecialtiesList : Page
    {
        public SpecialtiesList()
        {
            InitializeComponent();
            sss();
        }

        public async Task sss()
        {
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                var result = await LearningProcessesAPI.getAllSpecialities();
                SpecialtiesListView.ItemsSource = result;
                totalCount.Content = SpecialtiesListView.Items.Count;
            });
        }
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double remainingSpace = SpecialtiesListView.ActualWidth;


            if (remainingSpace > 0)
            {

                (SpecialtiesListView.View as GridView).Columns[0].Width = Math.Ceiling(remainingSpace / 6);
                (SpecialtiesListView.View as GridView).Columns[1].Width = Math.Ceiling(remainingSpace / 6);
                (SpecialtiesListView.View as GridView).Columns[2].Width = Math.Ceiling(remainingSpace / 6);
                (SpecialtiesListView.View as GridView).Columns[3].Width = Math.Ceiling(remainingSpace / 6);
                (SpecialtiesListView.View as GridView).Columns[4].Width = Math.Ceiling(remainingSpace / 6);
                (SpecialtiesListView.View as GridView).Columns[5].Width = Math.Ceiling(remainingSpace / 6);
            }
        }

        public async Task deleteSpeciality(Speciality speciality)
        {
            //LearningProcessesAPI.updateTeacher();
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                List<Speciality> list = (List<Speciality>)SpecialtiesListView.ItemsSource;
                var result = await LearningProcessesAPI.deleteSpeciality(speciality.Id);
                list.Remove(speciality);
                //TeacherListView.Items.Remove(teacher);
                SpecialtiesListView.Items.Refresh();
                totalCount.Content = SpecialtiesListView.Items.Count;
            });

            //MessageBox.Show(result.Count + "");
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Это действие приведёт к удалению, без возможности восстановления.\nПродолжить?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (MessageBoxResult.Yes == result)
            {
                deleteSpeciality((Speciality)((Button)sender).DataContext);
            }
        }

        private void addNew_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.MainFrame.Navigate(new AddSpecialitiesView(sss));
        }

        private void SpecialtiesListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SpecialtiesListView.SelectedItem != null)
            {
                MainWindow.Instance.MainFrame.Navigate(new SpecialitiesView((Speciality)SpecialtiesListView.SelectedItem, sss));
            }
        }
    }
}
