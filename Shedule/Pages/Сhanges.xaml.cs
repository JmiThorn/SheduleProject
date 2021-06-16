using LearningProcessesAPIClient.api;
using LearningProcessesAPIClient.exceptions;
using LearningProcessesAPIClient.model;
using Shedule.Controls;
using Shedule.Models.AlteredSchedules;
using Shedule.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Shedule.Pages
{
    /// <summary>
    /// Логика взаимодействия для Сhanges.xaml
    /// </summary>
    public partial class Сhanges : Page
    {

        private List<MainSchedule> mainSchedules = new List<MainSchedule>();
        private static Сhanges _instance;

        public static Сhanges Instance { get => _instance; }
        private List<AlteredSchedule> allAlteredSchedulesBeforeUpdate { get; set; } = new List<AlteredSchedule>();

        DateTime lastDateChange;

        public Сhanges()
        {
            _instance = this;
            InitializeComponent();

            DatePicker.SelectedDate = DateTime.Today;
        }

        async Task loadSchedules()
        {
            if (DatePicker.SelectedDate == null)
                return;
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                var mains = await LearningProcessesAPI.getAllMainSchedules();
                //DateTime date = DateTime.Now;
                DateTime date = (DateTime)DatePicker.SelectedDate;
                mainSchedules = mains.Where(m => m.Semester.StartDate <= date && m.Semester.StartDate.AddDays(7 * m.Semester.WeeksCount) >= date).ToList();
                foreach (var m in mainSchedules)
                {
                    if (m.TeachingId != null)
                    {
                        m.Teaching = await LearningProcessesAPI.getTeaching((int)m.TeachingId);
                    }
                }
                AlteredScheduleRow.AllMainSchedules.Clear();
                AlteredScheduleRow.AllMainSchedules.AddRange(mainSchedules);

                var altereds = await LearningProcessesAPI.getAlteredSchedules(date);
                foreach (var a in altereds)
                {
                    a.MainSchedule = mainSchedules.First(m => m.Id == a.MainScheduleId);
                }

                copyIntoBeforeUpdateState(altereds);

                var query = altereds.GroupBy(a => a.MainSchedule.Semester.GroupId, a => a, (groupId, alters) => new
                {
                    GroupId = groupId,
                    AlteredSchedules = alters
                });

                AlteredScheduleRow.AllAlteredSchedules.Clear();
                AlteredScheduleRow.AllAlteredSchedules.AddRange(altereds);

                foreach (var result in query)
                {
                    createNewGroupAlteredRow();
                    foreach (var alteredSchedule in result.AlteredSchedules)
                    {
                        foreach (var item in (alteredRows.Items[alteredRows.Items.Count - 1] as AlteredGroupScheduleRowControl).Grid.Children)
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

        private void copyIntoBeforeUpdateState(List<AlteredSchedule> list)
        {
            allAlteredSchedulesBeforeUpdate.Clear();
            allAlteredSchedulesBeforeUpdate.AddRange(list.Select(a => new AlteredSchedule()
            {
                Classroom = a.Classroom,
                Date = a.Date,
                Event = a.Event,
                EventId = a.EventId,
                Id = a.Id,
                MainSchedule = a.MainSchedule,
                MainScheduleId = a.MainScheduleId,
                NewClassroomId = a.NewClassroomId,
                NewTeachingId = a.NewTeachingId,
                Teaching = a.Teaching
            }));
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

        public void createNewGroupAlteredRow(int groupId, int highlightedColumn)
        {
            if ((alteredRows.Items[alteredRows.Items.Count - 1] as AlteredGroupScheduleRowControl).groups.SelectedIndex == -1)
            {
                int index = ((alteredRows.Items[alteredRows.Items.Count - 1] as AlteredGroupScheduleRowControl).DataContext as AlteredScheduleRow).AvailableGroupsList.FindIndex(g => g.Id == groupId);
                if (index >= 0)
                {
                    (alteredRows.Items[alteredRows.Items.Count - 1] as AlteredGroupScheduleRowControl).groups.SelectedIndex = index;
                    if (highlightedColumn != -1)
                    {
                        (alteredRows.Items[alteredRows.Items.Count - 1] as AlteredGroupScheduleRowControl).addCellForHighlight(highlightedColumn);
                    }
                }
                else
                {
                    alteredRows.Items.Remove(alteredRows.Items[alteredRows.Items.Count - 1]);
                    foreach (var row in alteredRows.Items)
                    {
                        if (((Group)(row as AlteredGroupScheduleRowControl).groups.SelectedValue).Id == groupId)
                        {
                            (row as AlteredGroupScheduleRowControl).addCellForHighlight(highlightedColumn);
                        }
                    }
                }
            }
            else
            {
                createNewGroupAlteredRow();
                createNewGroupAlteredRow(groupId, highlightedColumn);
            }
        }

        public void createNewGroupAlteredRow(int groupId)
        {
            createNewGroupAlteredRow(groupId, -1);
        }

        private void Groups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem == null)
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
            if (alteredRows.Items.Count < AlteredScheduleRow.GroupsList.Count)
            {
                createNewGroupAlteredRow();
            }
            else
            {
                MessageBox.Show("Использованы все существующие группы", "Изменение расписания", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        //Вызывать перед загрузкой в бд
        private List<AlteredSchedule> removeUnnecessaryAlteredSchedules(List<AlteredSchedule> list)
        {
            List<AlteredSchedule> altereds = new List<AlteredSchedule>();
            altereds.AddRange(list);
            altereds.RemoveAll(a => a.NewTeachingId == a.MainSchedule.TeachingId && a.NewClassroomId == a.MainSchedule.ClassroomId);
            return altereds;
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            //Предотвращаем двойной вызов события
            if (DateTime.Now.Subtract(lastDateChange).Milliseconds < 100)
                return;
            lastDateChange = DateTime.Now;

            if (alteredRows.Items.Count > 0)
            {
                if (MessageBox.Show("Очистить изменения?", "Смена дня", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    alteredRows.Items.Clear();
                }
                else
                {
                    return;
                }
            }
            var color = WeeksColoringUtils.getWeekColor(DatePicker.SelectedDate.Value);
            if (color == WeeksColoringUtils.WeekColors.BLUE)
            {
                week.Fill = Brushes.Blue;
                week.ToolTip = null;
            }
            else if (color == WeeksColoringUtils.WeekColors.RED)
            {
                week.Fill = Brushes.Red;
                week.ToolTip = null;
            }
            else
            {
                week.Fill = Brushes.DarkGray;
                week.ToolTip = "День находится вне границ учебного года";
            }
            Dispatcher.Invoke(async () =>
            {
                await loadGroups();
                await loadSchedules();
            }, DispatcherPriority.Background);
        }

        private async Task saveAltereds()
        {
            saveButton.IsEnabled = false;
            bool errorHappened = false;

            List<AlteredSchedule> newAlteredSchedules = new List<AlteredSchedule>();
            List<AlteredSchedule> updatedAlteredSchedules = new List<AlteredSchedule>();
            List<AlteredSchedule> deletedAlteredSchedules = new List<AlteredSchedule>();

            List<AlteredSchedule> allAlteredSchedules = removeUnnecessaryAlteredSchedules(AlteredScheduleRow.AllAlteredSchedules);
            allAlteredSchedules.ForEach(a =>
            {
                if (a.Id == -1)
                    newAlteredSchedules.Add(a);
                else
                {
                    int index = allAlteredSchedulesBeforeUpdate.FindIndex(ai => ai.MainScheduleId == a.MainScheduleId);
                    if (index == -1)
                    {
                        newAlteredSchedules.Add(a);
                    }
                    else
                    {
                        AlteredSchedule a2 = allAlteredSchedulesBeforeUpdate[index];
                        if (a2.NewClassroomId != a.NewClassroomId || a2.NewTeachingId != a.NewTeachingId)
                        {
                            updatedAlteredSchedules.Add(a);
                        }
                    }
                }
            });

            allAlteredSchedulesBeforeUpdate.ForEach(a =>
            {
                int index = allAlteredSchedules.FindIndex(ai => ai.MainScheduleId == a.MainScheduleId);
                if (index == -1)
                {
                    deletedAlteredSchedules.Add(a);
                }
            });
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                try
                {
                    if (newAlteredSchedules.Count > 0)
                    {
                        newAlteredSchedules.ForEach(async n =>
                        {
                            AlteredSchedule added;
                            if (n.NewTeachingId == null)
                            {
                                added = await LearningProcessesAPI.createAlteredScheduleCancel(n.MainSchedule, n.Date);
                            }
                            else
                            {
                                added = await LearningProcessesAPI.createAlteredScheduleChange(n.MainSchedule, n.Classroom, n.Date, n.Teaching);
                            }
                            if (added == null)
                            {
                                throw new ServerErrorException("Сервер вернул ошибку 404 при обновлении изменений");
                            }
                            //Не вижу необходимости заменять на данные их бд в этом случае
                        });

                    }
                    if (updatedAlteredSchedules.Count > 0)
                    {
                        updatedAlteredSchedules.ForEach(async u =>
                        {
                            var updated = await LearningProcessesAPI.updateAlteredScheduleItem(u.Id, u);
                            if (updated == null)
                            {
                                throw new ServerErrorException("Сервер вернул ошибку 404 при обновлении изменений (не найдены обновляемые компоненты)");
                            }
                            allAlteredSchedules.First(a => a.MainScheduleId == u.MainScheduleId).Id = updated.Id;
                            AlteredScheduleRow.AllAlteredSchedules.First(a => a.MainScheduleId == u.MainScheduleId).Id = updated.Id;
                        });
                    }
                    if (deletedAlteredSchedules.Count > 0)
                    {
                        deletedAlteredSchedules.ForEach(async u =>
                        {
                            var updated = await LearningProcessesAPI.deleteAlteredScheduleItem(u.Id);
                            if (!updated)
                            {
                                throw new ServerErrorException("Сервер вернул ошибку 404 при обновлении изменений (не найдены удаляемые компоненты)");
                            }
                        });
                    }
                    //Новое состояние. Мы сюда не доходим в случае throw
                    copyIntoBeforeUpdateState(allAlteredSchedules);
                    //allAlteredSchedulesBeforeUpdate = allAlteredSchedules.ToList();
                }
                catch (Exception)
                {
                    errorHappened = true;
                }
            });

            if (!errorHappened)
            {
                MessageBox.Show($"Успешно сохранено ({newAlteredSchedules.Count} добавлено, {updatedAlteredSchedules.Count} обновлено, {deletedAlteredSchedules.Count} удалено)", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private bool validatePage()
        {
            bool isValid = true;
            foreach (var el in alteredRows.Items)
            {
                foreach (var cell in (el as AlteredGroupScheduleRowControl).Grid.Children)
                {
                    if (!(cell is AlteredScheduleItemControl))
                        continue;
                    var teaching = (cell as AlteredScheduleItemControl).Teachings.SelectedIndex;
                    var classroom = (cell as AlteredScheduleItemControl).Classrooms.SelectedIndex;
                    if (teaching == -1 && classroom != -1 || teaching != -1 && classroom == -1)
                    {
                        isValid = false;
                    }
                }
            }
            return isValid;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (validatePage())
            {
                Dispatcher.Invoke(async () =>
                {
                    await saveAltereds();
                    saveButton.IsEnabled = true;
                }, DispatcherPriority.Background);
            }
            else
            {
                MessageBox.Show("Необходимо установить аудиторию(и)", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (validatePage())
            {
                try
                {
                    var list = removeUnnecessaryAlteredSchedules(AlteredScheduleRow.AllAlteredSchedules);
                    AlterExport.ExportAlterShedule(list, DatePicker.SelectedDate.Value);
                }
                catch
                {
                    MessageBox.Show("Некорректная дата");
                }
            }
            else
            {
                MessageBox.Show("Необходимо установить аудиторию(и)", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
