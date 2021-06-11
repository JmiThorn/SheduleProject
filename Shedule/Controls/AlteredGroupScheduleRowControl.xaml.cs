using LearningProcessesAPIClient.model;
using Shedule.Models.AlteredSchedules;
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

        private void groups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (groups.SelectedItem != null)
            {
                (DataContext as AlteredScheduleRow).SelectedGroup = groups.SelectedItem as Group;
                AlteredScheduleRow.UsedGroupsList.Add(groups.SelectedItem as Group);
            }
            else
            {
                (DataContext as AlteredScheduleRow).SelectedGroup = null;
            }
        }

        private void groups_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //самоцничтожаемся
            if (groups.SelectedItem != null)
            {
                AlteredScheduleRow.UsedGroupsList.Remove(groups.SelectedItem as Group);
            }
            groups.SelectedIndex = -1;
            //TODO удаляться 
        }
    }
}
