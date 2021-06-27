using LearningProcessesAPIClient.api;
using LearningProcessesAPIClient.model;
using Microsoft.Win32;
using Shedule.Models.Parsing;
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
            AppUtils.PageContentAreSaved = false;
        }
        public async Task loadSpeciality()
        {
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                var specialities = await LearningProcessesAPI.getAllSpecialities();
                specialityCB.ItemsSource = specialities;
            });
        }
        public async Task loadSemesters(Group group)
        {
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                var semesters = await LearningProcessesAPI.getSemesters(group.Id);
                SemesterListView.ItemsSource = semesters;
                SemesterListView.Items.Refresh();
            });
        }
        public async Task loadSpecSub(Group group)
        {
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                var getGroupTeachings = await LearningProcessesAPI.getGroupTeachings(group.Id);
                TeachingListView.ItemsSource = getGroupTeachings;
            });
        }
        public async Task loadTeachings()
        {
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                var list = await LearningProcessesAPI.getGroupTeachings((DataContext as Group).Id);
                TeachingListView.ItemsSource = list;
            });
            //var list = await LearningProcessesAPI.ge
        }

        private void DigitCheck_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if ((e.Text) == null || !(e.Text).All(char.IsDigit) || (sender as TextBox).Text.Length >= 1)
            {
                e.Handled = true;
            }
        }
        private async Task save()
        {
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                //codename.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                course.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                subgroup.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                specialityCB.GetBindingExpression(ComboBox.SelectedValueProperty).UpdateSource();
                Group group = (Group)DataContext;
                var result = await LearningProcessesAPI.updateGroup(group.Id, group);
                codename.Text = result.Codename;
                codename.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                AppUtils.PageContentAreSaved = true;
                MessageBox.Show("Данные успешно обновлены", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }
        private void save_butt_Click(object sender, RoutedEventArgs e)
        {
            save();
        }



        public async Task deleteSemester(Semester semester)
        {
            //LearningProcessesAPI.updateTeacher();
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                List<Semester> list = (List<Semester>)SemesterListView.ItemsSource;
                var result = await LearningProcessesAPI.deleteSemester(semester.Id);
                list.Remove(semester);
                SemesterListView.Items.Refresh();
            });

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
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                List<Teaching> list = (List<Teaching>)TeachingListView.ItemsSource;
                var result = await LearningProcessesAPI.deleteTeaching(teaching.Id);
                list.Remove(teaching);
                TeachingListView.Items.Refresh();
            });

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


        private void back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void addNew_teaching(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.MainFrame.Navigate(new AddGroupTeachingView((Group)DataContext, loadTeachings));
        }

        private async void import_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Excel files(*.xlsx)|*.xlsx";
                if (openFileDialog.ShowDialog() == true)
                {
                    await ParsingUtils.ParseFile(openFileDialog.FileName, ((DataContext as Group).Id));
                    loadSemesters((Group)DataContext);
                }
                MessageBox.Show("Импорт успешно выполнен, не забудьте проставить даты начала семестров","Уведомление",MessageBoxButton.OK,MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SemesterListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SemesterListView.SelectedItem != null)
            {
                MainWindow.Instance.MainFrame.Navigate(new SemestersView((Semester)SemesterListView.SelectedItem));
            }
        }

        private void TeachingListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(TeachingListView.SelectedItem != null)
            {
                MainWindow.Instance.MainFrame.Navigate(new GroupTeachingView((Teaching)TeachingListView.SelectedItem, loadTeachings));
            }
        }
    }
}
