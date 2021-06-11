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


        public Сhanges()
        {
            InitializeComponent();
            Dispatcher.Invoke(async () =>
            {
            await loadGroups();
            await loadSchedules();
            },DispatcherPriority.Background);
            
        }

        async Task loadSchedules()
        {
            await AppUtils.ProcessClientLibraryRequest(async () => {
                var mains = await LearningProcessesAPI.getAllMainSchedules();
                //DateTime date = DateTime.Now;
                DateTime date = new DateTime(2020,1,13);
                mainSchedules = mains.Where(m => m.Semester.StartDate <= date && m.Semester.StartDate.AddDays(7 * m.Semester.WeeksCount) >= date).ToList();
                var altereds = await LearningProcessesAPI.getAlteredSchedules(date);
                foreach(var a in altereds){
                    a.MainSchedule = mainSchedules.First(m => m.Id == a.MainScheduleId);
                }
                var query = altereds.GroupBy(a => a.MainSchedule.Semester.GroupId, a => a, (groupId, alters) => new
                {
                    GroupId = groupId,
                    AlteredSchedules = alters
                });
                foreach (var result in query){
                    createNewGroupAlteredRow();
                    foreach(var alteredSchedule in result.AlteredSchedules){
                        foreach(var item in (alteredRows.Items[alteredRows.Items.Count - 1] as AlteredGroupScheduleRowControl).Grid.Children)
                        {
                            if (item is ComboBox)
                                (item as ComboBox).SelectedIndex = ((item as ComboBox).DataContext as AlteredScheduleRow).AvailableGroupsList.FindIndex(g => g.Id == result.GroupId);
                            if (!(item is AlteredScheduleItemControl))
                                continue;
                            if(Grid.GetColumn(item as UIElement) == alteredSchedule.MainSchedule.DayOfWeekId + 1)
                            {
                                (item as AlteredScheduleItemControl).DataContext = alteredSchedule;
                            }
                        }
                    }
                }
            });
        }

        public void createNewGroupAlteredRow()
        {
            AlteredScheduleRow row = new AlteredScheduleRow();
            AlteredGroupScheduleRowControl control = new AlteredGroupScheduleRowControl();
            control.DataContext = row;
            for (int i = 0; i < 6; i++)
            {
                AlteredScheduleItemControl combos = new AlteredScheduleItemControl();
                Grid.SetColumn(combos, i + 1);
                control.Grid.Children.Add(combos);
            }
            alteredRows.Items.Add(control);
        }

        async Task loadGroups()
        {
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                var groups = await LearningProcessesAPIClient.api.LearningProcessesAPI.getAllGroups();
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
    }
}
