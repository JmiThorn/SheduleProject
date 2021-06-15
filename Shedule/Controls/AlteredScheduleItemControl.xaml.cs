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
    /// Логика взаимодействия для AlteredScheduleItemControl.xaml
    /// </summary>
    public partial class AlteredScheduleItemControl : UserControl
    {
        public AlteredScheduleItemControl()
        {
            InitializeComponent();
        }

        private void ComboBox_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            (sender as ComboBox).SelectedIndex = -1;
        }

        private void AddChangeButtonClick(object sender, RoutedEventArgs e)
        {
            ChangeContent.Visibility = Visibility.Visible;
            NoChangeContent.Visibility = Visibility.Collapsed;
            AlteredScheduleRow.createAlteredSchedule((DataContext as ClassAvailabilityInfoModel));
            Teachings.ItemsSource = (DataContext as ClassAvailabilityInfoModel).getFeaturedTeachingModelsList();
        }

        private void DeleteChangeButtonClick(object sender, RoutedEventArgs e)
        {
            ChangeContent.Visibility = Visibility.Collapsed;
            NoChangeContent.Visibility = Visibility.Visible;
            AlteredScheduleRow.deleteAlteredSchedule((DataContext as ClassAvailabilityInfoModel));
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(DataContext is ClassAvailabilityInfoModel)
                if((DataContext as ClassAvailabilityInfoModel).AlteredSchedule != null)
                {
                    ChangeContent.Visibility = Visibility.Visible;
                    NoChangeContent.Visibility = Visibility.Collapsed;
                    Teachings.ItemsSource = (DataContext as ClassAvailabilityInfoModel).getFeaturedTeachingModelsList();
                }
        }

        private void Teachings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(Teachings.SelectedItem != null)
            {
                (DataContext as ClassAvailabilityInfoModel).AlteredSchedule.Teaching = (Teachings.SelectedItem as TeachingAvailabilityInfo).ExtendedTeaching;
                Classrooms.ItemsSource = (Teachings.SelectedItem as TeachingAvailabilityInfo).getClassroomModels();
            }
            else
            {
                Classrooms.ItemsSource = null;
                if((DataContext as ClassAvailabilityInfoModel).AlteredSchedule != null)
                {
                    (DataContext as ClassAvailabilityInfoModel).AlteredSchedule.Teaching = null;
                }
            }
            AlteredScheduleRow.updateAlteredSchedule();
        }

        private void Classrooms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(Classrooms.SelectedItem != null)
            {
                (DataContext as ClassAvailabilityInfoModel).AlteredSchedule.Classroom = (Classrooms.SelectedItem as ClassroomAvailabilityInfo).Classroom;
            }
            else
            {
                if((DataContext as ClassAvailabilityInfoModel).AlteredSchedule != null)
                {
                    (DataContext as ClassAvailabilityInfoModel).AlteredSchedule.Classroom = null;
                }
            }
            AlteredScheduleRow.updateAlteredSchedule();
        }
    }
}
