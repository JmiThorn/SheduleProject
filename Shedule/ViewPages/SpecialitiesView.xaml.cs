using LearningProcessesAPIClient.api;
using LearningProcessesAPIClient.model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Shedule.ViewPages
{
    /// <summary>
    /// Логика взаимодействия для SpecialitiesView.xaml
    /// </summary>
    public partial class SpecialitiesView : Page
    {
        public SpecialitiesView(Speciality speciality)
        {
            InitializeComponent();
            DataContext = speciality;
            loadDepartment();
            // loadSubject(speciality);

        }
        public async Task loadDepartment()
        {
            var department = await LearningProcessesAPI.getAllDepartments();
            departmentCB.ItemsSource = department;
        }

        private async void save_butt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                name.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                code.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                codename.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                day.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                week.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                departmentCB.GetBindingExpression(ComboBox.SelectedValueProperty).UpdateSource();
                Speciality speciality = (Speciality)DataContext;
                var result = await LearningProcessesAPI.updateSpeciality(speciality.Id, speciality);
                MessageBox.Show("Данные успешно обновлены", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                DataContext = result;
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
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

        private void addNew_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.MainFrame.Navigate(new AddSpecialitiesSubjectView((Speciality)DataContext));
        }
        public async Task deleteSpecialitySubject(SpecialitySubject subject)
        {
            //LearningProcessesAPI.updateTeacher();
            try
            {
                List<SpecialitySubject> list = (List<SpecialitySubject>)SpecSubjectView.ItemsSource;
                var result = await LearningProcessesAPI.deleteSpecialitySubject(subject.Id);
                list.Remove(subject);
                SpecSubjectView.Items.Refresh();
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
                deleteSpecialitySubject((SpecialitySubject)((Button)sender).DataContext);
            }
        }
        public async Task loadSubject(Speciality speciality)
        {
            var semesters = await LearningProcessesAPI.getSpecialitySubjects(speciality.Id);
            SpecSubjectView.ItemsSource = semesters;
        }

        private void OneStr_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                MainWindow.Instance.MainFrame.Navigate(new SpecialitiesSubjectView((SpecialitySubject)SpecSubjectView.SelectedItem, updateManually));
            }

        }

        private void Page_GotFocus(object sender, RoutedEventArgs e)
        {

            //  var r = ((GridView)SpecSubjectView.View).Columns[0].;
            //((GridView)SpecSubjectView.View).Columns[1].CellTemplate.GetBindingExpression(ListView.ItemsSourceProperty).UpdateTarget();
            //OneStr2.
            //  SpecSubjectView.GetBindingExpression(ListView.ItemsSourceProperty).UpdateTarget();
            //SpecSubjectView.Items.Refresh();
        }

        public void updateManually()
        {
            var buf = DataContext;
            DataContext = null;
            DataContext = buf;
        }
    }
}
