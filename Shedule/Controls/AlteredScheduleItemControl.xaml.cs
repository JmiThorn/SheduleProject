using Shedule.Models.AlteredSchedules;
using Shedule.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
            if(AlteredScheduleRow.createAlteredSchedule((DataContext as ClassAvailabilityInfoModel)))
            {
                ChangeContent.Visibility = Visibility.Visible;
                NoChangeContent.Visibility = Visibility.Collapsed;
            }
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
                }
        }

        private void Teachings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(Teachings.SelectedItem != null)
            {
                (DataContext as ClassAvailabilityInfoModel).AlteredSchedule.Teaching = (Teachings.SelectedItem as TeachingAvailabilityInfo).ExtendedTeaching;
                Classrooms.ItemsSource = (Teachings.SelectedItem as TeachingAvailabilityInfo).getClassroomModels();
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(Classrooms.ItemsSource);
                view.SortDescriptions.Add(new SortDescription("RecommendationLevel", ListSortDirection.Ascending));
                if ((Teachings.SelectedItem as TeachingAvailabilityInfo).TeacherParallels.Count > 0)
                {
                    //Генерируем новые строки
                    (Teachings.SelectedItem as TeachingAvailabilityInfo).TeacherParallels.ForEach(t => {
                        Сhanges.Instance.createNewGroupAlteredRow(t.Item2.GroupId, Grid.GetColumn(this));
                    });
                }
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
