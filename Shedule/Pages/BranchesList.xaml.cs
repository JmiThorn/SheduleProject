using LearningProcessesAPIClient.api;
using LearningProcessesAPIClient.model;
using Shedule.Utils;
using Shedule.ViewPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Shedule.Pages
{
    /// <summary>
    /// Логика взаимодействия для Branches.xaml
    /// </summary>
    public partial class BranchesList : Page
    {
        public BranchesList()
        {
            InitializeComponent();
            sss();
        }
        public async Task sss()
        {
            //LearningProcessesAPI.updateTeacher();
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                var result = await LearningProcessesAPI.getAllDepartments();
                DepartmentListView.ItemsSource = result;
                DepartmentListView.Items.Refresh();
                totalCount.Content = DepartmentListView.Items.Count;

            });

        }
        public async Task deleteDepartment(Department department)
        {
            //LearningProcessesAPI.updateTeacher();
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                List<Department> list = (List<Department>)DepartmentListView.ItemsSource;
                var result = await LearningProcessesAPI.deleteDepartment(department.Id);
                list.Remove(department);
                //TeacherListView.Items.Remove(teacher);
                DepartmentListView.Items.Refresh();
                totalCount.Content = DepartmentListView.Items.Count;
            });
            //catch (Exception error)
            //{
            //    MessageBox.Show("Произошла ошибка удаления, у данного отделения имеется привязка.\nДля удаления отвяжите специальность и повторите попытку","Ошибка удаления",MessageBoxButton.OK,MessageBoxImage.Error);
            //}

            //MessageBox.Show(result.Count + "");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Это действие приведёт к удалению, без возможности восстановления.\nПродолжить?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (MessageBoxResult.Yes == result)
            {
                deleteDepartment((Department)((Button)sender).DataContext);

            }


        }


        private void addNew_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.MainFrame.Navigate(new AddDepartmentsView(sss));
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double remainingSpace = DepartmentListView.ActualWidth;

            if (remainingSpace > 0)
            {

                (DepartmentListView.View as GridView).Columns[0].Width = Math.Ceiling(remainingSpace / 1);

            }
        }

        private void DepartmentListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DepartmentListView.SelectedItem != null)
            {
                MainWindow.Instance.MainFrame.Navigate(new DepartmentsView((Department)DepartmentListView.SelectedItem));
            }
        }
    }
}
