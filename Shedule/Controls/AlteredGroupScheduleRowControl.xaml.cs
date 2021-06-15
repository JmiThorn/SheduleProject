using LearningProcessesAPIClient.api;
using LearningProcessesAPIClient.model;
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

namespace Shedule.Controls
{
    /// <summary>
    /// Логика взаимодействия для AlteredGroupScheduleRow.xaml
    /// </summary>
    public partial class AlteredGroupScheduleRowControl : UserControl
    {
        public AlteredGroupScheduleRowControl()
        {
            InitializeComponent();
        }

        private void fillCells()
        {
            for (int i = 0; i < 6; i++)
            {
                AlteredScheduleItemControl combos = new AlteredScheduleItemControl();
                Grid.SetColumn(combos, i + 1);
                Grid.Children.Add(combos);
            }
        }

        private async void groups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            fillCells();
            groups.Visibility = Visibility.Collapsed;
            groupTitle.Visibility = Visibility.Visible;
            if (groups.SelectedItem != null)
            {
                if((DataContext as AlteredScheduleRow).SelectedGroup != null)
                {
                    AlteredScheduleRow.UsedGroupsList.Remove((DataContext as AlteredScheduleRow).SelectedGroup);
                }
                (DataContext as AlteredScheduleRow).SelectedGroup = groups.SelectedItem as Group;
                await (DataContext as AlteredScheduleRow).recalculateConfiguration();
                AlteredScheduleRow.UsedGroupsList.Add(groups.SelectedItem as Group);
                reloadCellsSuppliers();
            }
            else
            {
                (DataContext as AlteredScheduleRow).SelectedGroup = null;
                reloadCellsSuppliers();
            }
        }

        private void groups_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var alters = AlteredScheduleRow.AllAlteredSchedules.Where(a => a.MainSchedule.Semester.GroupId == (groups.SelectedValue as Group).Id).ToList();
            if (alters.Count > 0)
            {
                if(MessageBox.Show($"Удалить {alters.Count} замен?", "Изменения", MessageBoxButton.YesNo,MessageBoxImage.Question)== MessageBoxResult.Yes)
                {
                    alters.ForEach(a => AlteredScheduleRow.AllAlteredSchedules.Remove(a));
                }
                else
                {
                    return;
                }
            }
            AlteredScheduleRow.UsedGroupsList.Remove(groups.SelectedItem as Group);
            groups.SelectedIndex = -1;
        }

        private async Task reloadCellsSuppliers()
        {
            if ((DataContext as AlteredScheduleRow).SelectedGroup == null)
            {
                foreach (var item in Grid.Children)
                {
                    if (!(item is AlteredScheduleItemControl))
                        continue;
                    (item as AlteredScheduleItemControl).Teachings.ItemsSource = null;
                    (item as AlteredScheduleItemControl).Classrooms.ItemsSource = null;
                }
            } else {
                await AppUtils.ProcessClientLibraryRequest(async () => {
                    
                    foreach(var item in Grid.Children)
                    {
                        if (!(item is AlteredScheduleItemControl))
                            continue;
                        int classNumber = Grid.GetColumn(item as UIElement) - 1;
                        (item as AlteredScheduleItemControl).DataContext = (DataContext as AlteredScheduleRow).getClassModel(classNumber);
                        //(item as AlteredScheduleItemControl).Teachings.ItemsSource = ((item as AlteredScheduleItemControl).DataContext as ClassAvailabilityInfoModel).getFeaturedTeachingModelsList();
                    }
                });
            }
        }
    }
}
