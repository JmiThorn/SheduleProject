using LearningProcessesAPIClient.api;
using LearningProcessesAPIClient.exceptions;
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
using System.Windows.Controls.Primitives;
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
    /// Логика взаимодействия для TeachersList.xaml
    /// </summary>
    public partial class TeachersList : Page
    {
        public TeachersList()
        {
            InitializeComponent();
            sss();

        }

        

        public async Task sss()
        {
            await AppUtils.ProcessClientLibraryRequest(async () => {
                var result = await LearningProcessesAPI.getAllTeachers();
                TeacherListView.ItemsSource = result;
                totalCount.Content = TeacherListView.Items.Count;
            });
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void OneStr_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                MainWindow.Instance.MainFrame.Navigate(new TeachersView((Teacher)TeacherListView.SelectedItem));
            }
        }
        private void search_box_TextChanged(object sender, TextChangedEventArgs e)
        {
            TeacherListView.Items.Filter = x => ((Teacher)x).Name.IndexOf(search_box.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
            ((Teacher)x).Surname.IndexOf(search_box.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
            ((Teacher)x).Patronymic.IndexOf(search_box.Text, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //double remainingSpace = TeacherListView.ActualWidth;

            //if (remainingSpace > 0)
            //{

            //    (TeacherListView.View as GridView).Columns[0].Width = Math.Ceiling(remainingSpace / 1);
            //}
            //double gddfg = TreeStr.ActualWidth;
           

            //if (remainingSpace > 0)
            //{

            //    (TeacherListView.View as GridView).Columns[0].Width = Math.Ceiling(remainingSpace / 1);
            //}
        }

        public async Task deleteTeacher(Teacher teacher)
        {
            //LearningProcessesAPI.updateTeacher();
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                List<Teacher> list = (List<Teacher>)TeacherListView.ItemsSource;
                var result = await LearningProcessesAPI.deleteTeacher(teacher.Id);
                list.Remove(teacher);
                //TeacherListView.Items.Remove(teacher);
                TeacherListView.Items.Refresh();
                totalCount.Content = TeacherListView.Items.Count;
            });

            //MessageBox.Show(result.Count + "");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Это действие приведёт к удалению, без возможности восстановления.\nПродолжить?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (MessageBoxResult.Yes == result)
            {
                deleteTeacher((Teacher)((Button)sender).DataContext);
            }
        }

        private void addNew_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.MainFrame.Navigate(new AddTeachersView(sss));
        }

        private void TeacherListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = ((FrameworkElement)e.OriginalSource).DataContext as Track;
            if (item != null)
            {
                MessageBox.Show("Item's Double Click handled!");
            }
        }
    }
}
