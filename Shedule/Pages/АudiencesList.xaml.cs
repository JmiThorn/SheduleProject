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
    /// Логика взаимодействия для Аudiences.xaml
    /// </summary>
    public partial class АudiencesList : Page
    {
        public АudiencesList()
        {
            InitializeComponent();
            sss();
        }
        public async Task sss()
        {
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                var result = await LearningProcessesAPI.getAllClassrooms();
                ClassroomsListView.ItemsSource = result;
                totalCount.Content = ClassroomsListView.Items.Count;
            });
        }

        private void OneStr_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                MainWindow.Instance.MainFrame.Navigate(new ClassroomsView((Classroom)ClassroomsListView.SelectedItem,sss));
                //MessageBox.Show("fgfgfg");
            }
        }

        public async Task deleteClassroom(Classroom classroom)
        {
            //LearningProcessesAPI.updateTeacher();
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                List<Classroom> list = (List<Classroom>)ClassroomsListView.ItemsSource;
                var result = await LearningProcessesAPI.deleteClassroom(classroom.Id);
                list.Remove(classroom);
                ClassroomsListView.Items.Refresh();
                totalCount.Content = ClassroomsListView.Items.Count;
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
                MessageBoxResult result = MessageBox.Show("Это действие приведёт к удалению, без возможности восстановления.\nПродолжить?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (MessageBoxResult.Yes == result)
                {
                deleteClassroom((Classroom)((Button)sender).DataContext);
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double remainingSpace = ClassroomsListView.ActualWidth;


            if (remainingSpace > 0)
            {
                (ClassroomsListView.View as GridView).Columns[0].Width = Math.Ceiling(remainingSpace / 3);
                (ClassroomsListView.View as GridView).Columns[1].Width = Math.Ceiling(remainingSpace / 3);
                (ClassroomsListView.View as GridView).Columns[2].Width = Math.Ceiling(remainingSpace / 3);
            }
        }

        private void addNew_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.MainFrame.Navigate(new AddClassroomsView(sss));
        }
        private void search_box_TextChanged(object sender, TextChangedEventArgs e)
        {
            ClassroomsListView.Items.Filter = x =>
            ((Classroom)x).Number.ToString().IndexOf(search_box.Text, StringComparison.OrdinalIgnoreCase) >= 0
            || ((Classroom)x).Building.ToString().IndexOf(search_box.Text, StringComparison.OrdinalIgnoreCase) >= 0
            || ((Classroom)x).Teacher?.Name.IndexOf(search_box.Text, StringComparison.OrdinalIgnoreCase) >= 0
            || ((Classroom)x).Teacher?.Surname.IndexOf(search_box.Text, StringComparison.OrdinalIgnoreCase) >= 0
            || ((Classroom)x).Teacher?.Patronymic.IndexOf(search_box.Text, StringComparison.OrdinalIgnoreCase) >= 0;
        }

    }
}
