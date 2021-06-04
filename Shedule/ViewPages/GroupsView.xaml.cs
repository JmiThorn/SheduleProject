using LearningProcessesAPIClient.api;
using LearningProcessesAPIClient.model;
using Microsoft.Win32;
using Shedule.Models.Parsing;
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
    public partial class GroupsView : Page
    {
        public GroupsView(Group group)
        {
            InitializeComponent();
            DataContext = group;
            loadSpeciality();
            loadSemesters(group);
            loadTeachings();
            loadSpecSub(group);
        }

        private void edit_butt_Click(object sender, RoutedEventArgs e)
        {
            //codename.IsEnabled = true;
            course.IsEnabled = true;
            subgroup.IsEnabled = true;
            specialityCB.IsEnabled = true;
        }
        public async Task loadSpeciality()
        {
            var specialities = await LearningProcessesAPI.getAllSpecialities();
            specialityCB.ItemsSource = specialities;
        }
        public async Task loadSemesters(Group group)
        {
            var semesters = await LearningProcessesAPI.getSemesters(group.Id);
            SemesterListView.ItemsSource = semesters;
        }
        public async Task loadSpecSub(Group group)
        {
            var getGroupTeachings = await LearningProcessesAPI.getGroupTeachings(group.Id);
            TeachingListView.ItemsSource = getGroupTeachings;
        }
        public async Task loadTeachings()
        {
            var list = await LearningProcessesAPI.getGroupTeachings((DataContext as Group).Id);
            TeachingListView.ItemsSource = list;
            //var list = await LearningProcessesAPI.ge
        }

        private async void save_butt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //codename.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                course.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                subgroup.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                specialityCB.GetBindingExpression(ComboBox.SelectedValueProperty).UpdateSource();
                Group group = (Group)DataContext;
                var result = await LearningProcessesAPI.updateGroup(group.Id, group);
                codename.Text = result.Codename;
                codename.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                MessageBox.Show("Данные успешно обновлены", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }



        public async Task deleteSemester(Semester semester)
        {
            //LearningProcessesAPI.updateTeacher();
            try
            {
                List<Semester> list = (List<Semester>)SemesterListView.ItemsSource;
                var result = await LearningProcessesAPI.deleteSemester(semester.Id);
                list.Remove(semester);
                SemesterListView.Items.Refresh();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }

            //MessageBox.Show(result.Count + "");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Это действие приведёт к удалению, без возможности восстановления.\nПродолжить?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (MessageBoxResult.Yes == result)
            {
                deleteSemester((Semester)((Button)sender).DataContext);
            }
        }

        public async Task delete_teaching(Teaching teaching)
        {
            //LearningProcessesAPI.updateTeacher();
            try
            {
                List<Teaching> list = (List<Teaching>)TeachingListView.ItemsSource;
                var result = await LearningProcessesAPI.deleteTeaching(teaching.Id);
                list.Remove(teaching);
                TeachingListView.Items.Refresh();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }

            //MessageBox.Show(result.Count + "");
        }

        private void delete_teaching(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Это действие приведёт к удалению, без возможности восстановления.\nПродолжить?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (MessageBoxResult.Yes == result)
            {
                delete_teaching((Teaching)((Button)sender).DataContext);
            }
        }

        private void addNew_Click(object sender, RoutedEventArgs e)
        {

            MainWindow.Instance.MainFrame.Navigate(new AddSemestersView((Group)DataContext));
        }

        private void OneStr_MouseDown(object sender, MouseButtonEventArgs e)
        {
                if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
                {
                    MainWindow.Instance.MainFrame.Navigate(new SemestersView((Semester)SemesterListView.SelectedItem));
                }
            
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void TwoStr_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                MainWindow.Instance.MainFrame.Navigate(new GroupTeachingView((Teaching)TeachingListView.SelectedItem, loadTeachings));
            }
        }

        private void addNew_teaching(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.MainFrame.Navigate(new AddGroupTeachingView((Group)DataContext, loadTeachings));
        }

        private void import_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Excel files(*.xlsx)|*.xlsx";
                if (openFileDialog.ShowDialog() == true)
                {
                    ParsingUtils.ParseFile(openFileDialog.FileName, ((DataContext as Group).Id));

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
