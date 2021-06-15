using Shedule.Controls;
using Shedule.Models;
using Shedule.Models.AlteredSchedules;
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
using LearningProcessesAPIClient.api;
using LearningProcessesAPIClient.model;
using System.Windows.Threading;
using System.Windows.Media.Animation;

namespace Shedule.Pages
{
    /// <summary>
    /// Логика взаимодействия для Сhanges.xaml
    /// </summary>
    public partial class Сhanges : Page
    {

        private List<MainSchedule> mainSchedules = new List<MainSchedule>();

        DateTime lastDateChange;


        public Сhanges()
        {
            InitializeComponent();
            DatePicker.SelectedDate = DateTime.Parse("13.01.2020");
        }

        async Task loadSchedules()
        {
            if (DatePicker.SelectedDate == null)
                return;
            await AppUtils.ProcessClientLibraryRequest(async () => {
                var mains = await LearningProcessesAPI.getAllMainSchedules();
                //DateTime date = DateTime.Now;
                DateTime date = (DateTime)DatePicker.SelectedDate;
                mainSchedules = mains.Where(m => m.Semester.StartDate <= date && m.Semester.StartDate.AddDays(7 * m.Semester.WeeksCount) >= date).ToList();
                foreach(var m in mainSchedules)
                {
                    if (m.TeachingId != null)
                    {
                        m.Teaching = await LearningProcessesAPI.getTeaching((int)m.TeachingId);
                    }
                }
                AlteredScheduleRow.AllMainSchedules.Clear();
                AlteredScheduleRow.AllMainSchedules.AddRange(mainSchedules);

                var altereds = await LearningProcessesAPI.getAlteredSchedules(date);
                foreach(var a in altereds){
                    a.MainSchedule = mainSchedules.First(m => m.Id == a.MainScheduleId);
                }

                

                var query = altereds.GroupBy(a => a.MainSchedule.Semester.GroupId, a => a, (groupId, alters) => new
                {
                    GroupId = groupId,
                    AlteredSchedules = alters
                });

                AlteredScheduleRow.AllAlteredSchedules.Clear();
                AlteredScheduleRow.AllAlteredSchedules.AddRange(altereds);

                foreach (var result in query){
                    createNewGroupAlteredRow();
                    foreach(var alteredSchedule in result.AlteredSchedules){
                        foreach(var item in (alteredRows.Items[alteredRows.Items.Count - 1] as AlteredGroupScheduleRowControl).Grid.Children)
                        {
                            if (item is ComboBox)
                                (item as ComboBox).SelectedIndex = ((item as ComboBox).DataContext as AlteredScheduleRow).AvailableGroupsList.FindIndex(g => g.Id == result.GroupId);
                            if (!(item is AlteredScheduleItemControl))
                                continue;
                            //if(Grid.GetColumn(item as UIElement) == alteredSchedule.MainSchedule.DayOfWeekId + 1)
                            //{
                            //    (item as AlteredScheduleItemControl).DataContext = alteredSchedule;
                            //}
                        }
                    }
                }
            });
        }

        public void createNewGroupAlteredRow()
        {
            if (DatePicker.SelectedDate == null)
                return;
            AlteredScheduleRow row = new AlteredScheduleRow((DateTime)DatePicker.SelectedDate);
            AlteredGroupScheduleRowControl control = new AlteredGroupScheduleRowControl();
            control.DataContext = row;
            alteredRows.Items.Add(control);
            control.groups.SelectionChanged += Groups_SelectionChanged;
        }

        private void Groups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if((sender as ComboBox).SelectedItem == null)
            {
                alteredRows.Items.Remove(((sender as ComboBox).Parent as Grid).Parent as AlteredGroupScheduleRowControl);
            }
        }

        async Task loadGroups()
        {
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                AlteredScheduleRow.UsedGroupsList.Clear();
                AlteredScheduleRow.clearEventsObservers();
                var groups = await LearningProcessesAPIClient.api.LearningProcessesAPI.getAllGroups();
                groups.RemoveAll(g => g.Semesters.Count(c => c.StartDate <= DatePicker.SelectedDate && c.StartDate.AddDays(7 * c.WeeksCount) >= DatePicker.SelectedDate) == 0);
                AlteredScheduleRow.GroupsList.Clear();
                AlteredScheduleRow.GroupsList.AddRange(groups);
                var classrooms = await LearningProcessesAPI.getAllClassrooms();
                AlteredScheduleRow.AllClassrooms.Clear();
                AlteredScheduleRow.AllClassrooms.AddRange(classrooms);
                //tet.ItemsSource = rezult;
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            createNewGroupAlteredRow();
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            //Предотвращаем двойной вызов события
            if (DateTime.Now.Subtract(lastDateChange).Milliseconds < 100)
                return;
            lastDateChange = DateTime.Now;
            //TODO clear all rows
            if(alteredRows.Items.Count > 0)
            {
                if(MessageBox.Show("Очистить изменения?","Смена дня",MessageBoxButton.YesNo,MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    alteredRows.Items.Clear();
                }
                else
                {
                    return;
                }
            }
            Dispatcher.Invoke(async () =>
            {
                await loadGroups();
                await loadSchedules();
            }, DispatcherPriority.Background);
        }

              
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
           List<AlteredSchedule> alterToDay = new List<AlteredSchedule>();
            try
            {
            alterToDay = await LearningProcessesAPI.getAlteredSchedules(date.SelectedDate.Value);

            AlterExport.ExportAlterShedule(alterToDay, date.SelectedDate.Value);
            }
            catch
            {
                MessageBox.Show("Gecnj");
            }

        }
    }
}
