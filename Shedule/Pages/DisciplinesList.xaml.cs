using LearningProcessesAPIClient.api;
using LearningProcessesAPIClient.model;
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
    /// Логика взаимодействия для Disciplines.xaml
    /// </summary>
    public partial class DisciplinesList : Page
    {
        public DisciplinesList()
        {
            InitializeComponent();
            sss();
        }

        public async Task sss()
        {
            try
            {
                var result = await LearningProcessesAPI.getAllSubjects();

                DisciplinesListView.ItemsSource = result;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {         
        }

        public async Task deleteSubject(Subject subject)
        {
            //LearningProcessesAPI.updateTeacher();
            try
            {
                List<Subject> list = (List<Subject>)DisciplinesListView.ItemsSource;
                var result = await LearningProcessesAPI.deleteSubject(subject.Id);
                list.Remove(subject);
                //TeacherListView.Items.Remove(teacher);
                DisciplinesListView.Items.Refresh();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }

            //MessageBox.Show(result.Count + "");
        }

        private void OneStr_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                MainWindow.Instance.MainFrame.Navigate(new SubjectsView((Subject)DisciplinesListView.SelectedItem));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Это действие приведёт к удалению, без возможности восстановления.\nПродолжить?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (MessageBoxResult.Yes == result)
            {
                deleteSubject((Subject)((Button)sender).DataContext);
            }
          
        }

        private void addNew_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.MainFrame.Navigate(new AddSubjectsView());
        }

        private void search_box_TextChanged(object sender, TextChangedEventArgs e)
        {
                DisciplinesListView.Items.Filter = x => ((Subject)x).Name.ToString().IndexOf(search_box.Text, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
