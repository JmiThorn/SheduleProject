using LearningProcessesAPIClient.api;
using LearningProcessesAPIClient.model;
using Shedule.Pages;
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
    /// Логика взаимодействия для SemestersView.xaml
    /// </summary>
    public partial class SemestersView : Page
    {
        public SemestersView(Semester semester)
        {
            InitializeComponent();
            DataContext = semester;
            loadSpeciality(semester);
            loadCurriculum(semester);
            loadCurriculumPractice(semester);
        }
        private void edit_butt_Click(object sender, RoutedEventArgs e)
        {
            weekscount.IsEnabled = true;
            startdate.IsEnabled = true;
            number.IsEnabled = true;
            //groupCB.IsEnabled = true;
        }
        public async Task loadSpeciality(Semester speciality)
        {
           
        }

        private async void save_butt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                number.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                startdate.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                weekscount.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                Semester semester  = (Semester)DataContext;
                var result = await LearningProcessesAPI.updateSemester(semester.Id, semester);
                MessageBox.Show("Данные успешно обновлены", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                foreach (Curriculum cur in CurriculumsListView.Items)
                {

                    
                 var cur_result = await LearningProcessesAPI.updateCommonCurriculum(cur.Id,cur);
                    if (cur_result==null)
                    {
                        await LearningProcessesAPI.createCommonCurriculumItem(cur.PlannedHours, cur.UsedHours, cur.SemesterId, cur.SpecialitySubjectId);
                    }
                }


            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }
        private void DigitCheck_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if ((e.Text) == null || !(e.Text).All(char.IsDigit) || number.Text.Length >= 2)
            {
                e.Handled = true;
            }
        }
        private void DigitCheck_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {
            if ((e.Text) == null || !(e.Text).All(char.IsDigit) || weekscount.Text.Length >= 2)
            {
                e.Handled = true;
            }
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double remainingSpace = CurriculumsListView.ActualWidth;
            double remainingSpace2 = CurriculumsPracticeListView.ActualWidth;


            if (remainingSpace > 0 )
            {

                (CurriculumsListView.View as GridView).Columns[0].Width = Math.Ceiling(remainingSpace / 4);
                (CurriculumsListView.View as GridView).Columns[1].Width = Math.Ceiling(remainingSpace / 4);
                (CurriculumsListView.View as GridView).Columns[2].Width = Math.Ceiling(remainingSpace / 4);
                (CurriculumsListView.View as GridView).Columns[3].Width = Math.Ceiling(remainingSpace / 4);
            } 
            if (remainingSpace2 > 0 )
            {

                (CurriculumsPracticeListView.View as GridView).Columns[0].Width = Math.Ceiling(remainingSpace / 4);
                (CurriculumsPracticeListView.View as GridView).Columns[1].Width = Math.Ceiling(remainingSpace / 4);
                (CurriculumsPracticeListView.View as GridView).Columns[2].Width = Math.Ceiling(remainingSpace / 4);
                (CurriculumsPracticeListView.View as GridView).Columns[3].Width = Math.Ceiling(remainingSpace / 4);
            }
        }
        public async Task loadCurriculum(Semester semester)
        {

            var cur = await LearningProcessesAPI.getCurriculaForSemester(semester.Id);
            foreach(var n in cur)
            {

                n.SpecialitySubject = await LearningProcessesAPI.getSpecialitySubject(n.SpecialitySubjectId);
            }
            cur = cur.FindAll(n => n.SpecialitySubject.Subject.IsPractise!=true);
 
            CurriculumsListView.ItemsSource = cur;
                var sub = await LearningProcessesAPI.getSpecialitySubjects(semester.Group.SpecialityId);
                sub = sub.AsQueryable().Except(cur.Select(c => c.SpecialitySubject),new spec()).ToList();
                specSub.ItemsSource = sub;
        }
        public async Task loadCurriculumPractice(Semester semester)
        {

            var cur = await LearningProcessesAPI.getCurriculaForSemester(semester.Id);
            foreach(var n in cur)
            {

                n.SpecialitySubject = await LearningProcessesAPI.getSpecialitySubject(n.SpecialitySubjectId);
            }
            cur = cur.FindAll(n => n.SpecialitySubject.Subject.IsPractise==true);

            CurriculumsPracticeListView.ItemsSource = cur;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OneStr_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void addNew_Click(object sender, RoutedEventArgs e)
        {
            Curriculum curriculum = (Curriculum)DataContext;
            CurriculumsListView.Items.Add("2");
        }

        private void addNewPractice_Click(object sender, RoutedEventArgs e)
        {

        }
    }


    class spec : IEqualityComparer<SpecialitySubject>
    {
        public bool Equals(SpecialitySubject b1, SpecialitySubject b2)
        {
            if (b2 == null && b1 == null)
                return true;
            else if (b1 == null || b2 == null)
                return false;
            else if (b1.Id == b2.Id)
                return true;
            else
                return false;
        }

        public int GetHashCode(SpecialitySubject bx)
        {
            int hCode = bx.Id ^ bx.SpecialityId ^ bx.SubjectId;
            return hCode.GetHashCode();
        }
    }


}
