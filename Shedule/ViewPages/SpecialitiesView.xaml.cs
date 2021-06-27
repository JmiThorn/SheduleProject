using LearningProcessesAPIClient.api;
using LearningProcessesAPIClient.model;
using Shedule.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public delegate Task Updater();
        public event Updater UpdateParent;
        public SpecialitiesView(Speciality speciality, Updater updater)
        {
            InitializeComponent();
            DataContext = speciality;
            loadDepartment();
            // loadSubject(speciality);
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
        private async Task save()
        {
            await AppUtils.ProcessClientLibraryRequest(async () => {
                if (Convert.ToInt32(week.Text) > 168 || Convert.ToInt32(day.Text) > 24)
                {
                    MessageBox.Show("Неправильное количество выставленных часов", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    name.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                    code.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                    codename.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                    day.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                    week.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                    departmentCB.GetBindingExpression(ComboBox.SelectedValueProperty).UpdateSource();
                    Speciality speciality = (Speciality)DataContext;
                    var result = await LearningProcessesAPI.updateSpeciality(speciality.Id, speciality);
                    AppUtils.PageContentAreSaved = true;
                    MessageBox.Show("Данные успешно обновлены", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                    DataContext = result;
                    UpdateParent?.Invoke();
                }
            });
        }
        private void save_butt_Click(object sender, RoutedEventArgs e)
        {
            save();
        }

        private void edit_butt_Click(object sender, RoutedEventArgs e)
        {
            code.IsEnabled = true;
            codename.IsEnabled = true;
            name.IsEnabled = true;
            departmentCB.IsEnabled = true;
            day.IsEnabled = true;
            week.IsEnabled = true;
            AppUtils.PageContentAreSaved = false;
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
        private void back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void addNew_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.MainFrame.Navigate(new AddSpecialitiesSubjectView((Speciality)DataContext, loadSubject));
        }
        public async Task deleteSpecialitySubject(SpecialitySubject subject)
        {
            //LearningProcessesAPI.updateTeacher();
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                HashSet<SpecialitySubject> list = (HashSet<SpecialitySubject>)SpecSubjectView.ItemsSource;
                var result = await LearningProcessesAPI.deleteSpecialitySubject(subject.Id);
                list.Remove(subject);
                SpecSubjectView.Items.Refresh();
            });

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
        public async Task loadSubject()
        {
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                HashSet<SpecialitySubject> Ss;
                Ss = new HashSet<SpecialitySubject>();
                var s2 = await (LearningProcessesAPI.getSpecialitySubjects(((Speciality)DataContext).Id));

                s2.ForEach(n => Ss.Add(n));

                ((Speciality)DataContext).SpecialitySubjects = Ss;
                SpecSubjectView.GetBindingExpression(ListView.ItemsSourceProperty)?.UpdateTarget();
            });
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

        private void SpecSubjectView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(SpecSubjectView.SelectedItem != null)
            {
                MainWindow.Instance.MainFrame.Navigate(new SpecialitiesSubjectView((SpecialitySubject)SpecSubjectView.SelectedItem, updateManually));
            }
        }
    }
}
