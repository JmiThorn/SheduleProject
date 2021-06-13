using LearningProcessesAPIClient.api;
using LearningProcessesAPIClient.exceptions;
using LearningProcessesAPIClient.model;
using Shedule.Controls;
using Shedule.Models;
using Shedule.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using System;
using LearningProcessesAPIClient.exceptions;
using System.Globalization;
using Shedule.Controls;
using Shedule.Utils;
using System.Windows.Media.Animation;

namespace Shedule.ViewPages
{
    /// <summary>
    /// Логика взаимодействия для Shedule.xaml
    /// </summary>
    public partial class SheduleView : Page
    {
        //Чем выше уровень - тем меньше заблокировано
        private enum InfluenceLevel
        {
            CLASSROOM_SELECTION = 0,
            TEACHING_SELECTION = 1,
            WEEK_COLOR_SELECTION = 2,
            SEMESTERS_SELECTION = 3,
            GROUPS_SELECTION = 4
        }

        private int currentGroupIndex = -1;
        private int currentSemesterIndex = -1;
        private bool groupAreRollingBack = false;

        private List<MainSchedule> currentSchedule = new List<MainSchedule>();
        private Dictionary<(int, int), MainScheduleLiveModel> cellDataContexts = new Dictionary<(int, int), MainScheduleLiveModel>();
        private Dictionary<int, CurriculumFillingInfoModel> specialityHoursItemsSource = new Dictionary<int, CurriculumFillingInfoModel>();
        private Dictionary<int, Label> hoursInfo = new Dictionary<int, Label>();

        //Глобальные данные
        private List<MainSchedule> allMainSchedules = new List<MainSchedule>();
        private List<MainSchedule> allMainSchedulesBeforeUpdate = new List<MainSchedule>(); //Для поиска измененных
        private List<Classroom> allClassrooms = new List<Classroom>();
        //данные по текущей группе
        private List<Curriculum> groupCurriculums = new List<Curriculum>();
        private List<Teaching> groupTeachings = new List<Teaching>();
        //Текущие данные
        private List<Curriculum> currentCurriculums = new List<Curriculum>();
        private List<Teaching> currentTeachings = new List<Teaching>();


        //TODO вынести число рабочих дней в настройки
        const int DAYS = 5;
        //const int DAYS = Convert.ToInt32(Properties.Settings.Default.WorkDays);

        public SheduleView()
        {
            InitializeComponent();
            #region old staff
            /*           string gridXaml = @"<StackPanel Grid.Row=\"2\" Grid.Column=\"1\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:s=\"clr-namespace:System;assembly=mscorlib\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"> " +
                       "<ComboBox SelectedValue =\"{Binding TeachingId, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}\" Margin=\"10,0\" IsEditable = \"False\" FontSize = \"18\" Height = \"50\" FontFamily = \"Global User Interface\" SelectedValuePath = \"Teaching.Id\"> " +
                   "<ComboBox.ItemTemplate> " +
                       "<DataTemplate> " +
                               "<StackPanel Orientation=\"Horizontal\">                                                                                                                               " +
                           "<StackPanel> " +
                               "<TextBlock> " +
                                   "<TextBlock.Text> " +
                                       "<MultiBinding  StringFormat=\"{}{0} {1:C}\"> " +
                                           "<Binding Path=\"Teaching.SpecialitySubject.Subject.Name\"/> " +
                                           "<Binding Path=\"Teaching.Teacher.Surname\"/> " +
                                       "</MultiBinding> " +
                                   "</TextBlock.Text> " +
                               "</TextBlock> " +
                               "<ListBox BorderBrush=\"white\" ItemsSource = \"{Binding MainSchedulesInTheSameTime}\">"+
                               "<ListBox.ItemTemplate>"+
                                   "<DataTemplate>"+
                                       "<StackPanel Orientation = \"Horizontal\">"+
                                        "<TextBlock>"+
                                          // "<Run Text=\"{Binding Teaching.SpecialitySubject.Subject.Name}\"/>" +
                                           "<Run Text=\"{Binding ClassNumber}\"/>" + " " +
                                           //"<Run Text=\"{Binding Teaching.Teacher.Surname}\"/>" +                
                                           "<Run Text=\"{Binding Teaching.TeacherId}\"/>" +                
                                           //"<Run Text=\"{Binding Teaching.Teacher.Name}\"/>" +                
                                         "</TextBlock>"+
                                       "</StackPanel>" +
                                   "</DataTemplate>"+
                               "</ListBox.ItemTemplate>"+
                            "</ListBox >"+
                            "</StackPanel>" +
                            "<StackPanel>" +
                            "<Image Margin=\"10,0,0,0\" Width=\"20\" Height=\"20\">" +
                               "                <Image.Style>" +
                               "                    <Style TargetType=\"{x:Type Image}\">" +
                                                   " <Setter Property=\"Source\" Value=\"/Image\\bad.png\"/> " +
                               "                        <Style.Triggers>" +
                               "                            <DataTrigger Binding=\"{Binding RecommendationLevel}\" Value=\"0\">                                                          " +
                               "                                <Setter Property=\"Source\" Value=\"/Image\\good.png\"/>                                                                  " +
                               "                            </DataTrigger>                                                                                                           " +
                               "                        </Style.Triggers>                                                                                                            " +
                               "                    </Style>                                                                                                                         " +
                               "                </Image.Style>                                                                                                                       " +
                               "            </Image>  " +
                            "</StackPanel>" +

                               "</StackPanel>                                                                                                                               " +
                           "</DataTemplate>" +
                   "</ComboBox.ItemTemplate>" +
               "</ComboBox> " +
               "<ComboBox SelectedValue =\"{Binding ClassroomId, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}\" Padding=\"0\" SelectedValuePath = \"Classroom.Id\" DisplayMemberPath = \"Classroom.Number\" IsEditable=\"False\" MinWidth = \"60\" FontSize = \"18\" Height = \"20\" Margin = \"0\" FontFamily = \"Global User Interface\" HorizontalContentAlignment = \"Center\" VerticalContentAlignment = \"Center\" HorizontalAlignment = \"Center\"> " +
               "</ComboBox> " +
               "<ComboBox.ItemTemplate>
                                   <DataTemplate>
                                       <StackPanel Orientation="Horizontal">
                                       <StackPanel Orientation="Vertical">
                                           <TextBlock>
                                           <MultiBinding  StringFormat="{}{0} {1:C}">
                                               <Binding Path="ClassNumber"/>
                                               <Binding Path="Building"/>
                                           </MultiBinding>
                                           </TextBlock>
                                           <TextBlock>
                                           <MultiBinding  StringFormat="{}{0} {1:C}">
                                               <Binding Path="Teaching.SpecialitySubject.Subject.Name"/>
                                               <Binding Path="Teaching.Group.Codename"/>
                                           </MultiBinding>
                                           </TextBlock>
                                       </StackPanel>
                                       <StackPanel>
                                           <Image Width="20" Height="20" Source="/Image\bad.png">
                                               <Image.Style>
                                                   <Style>
                                                       <Style.Triggers>
                                                           <DataTrigger Binding="{Binding RecommendationLevel}" Value="0">
                                                               <Setter Property="Source" Value="/Image\good.png"/>
                                                           </DataTrigger>
                                                       </Style.Triggers>
                                                   </Style>
                                               </Image.Style>
                                           </Image>
                                       </StackPanel>
                                       </StackPanel>
                                   </DataTemplate>
                               </ComboBox.ItemTemplate>"+
           "</StackPanel> ";*/

            //            string gridXaml = @"
            //<StackPanel Grid.Row=""2"" Grid.Column=""1"" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:s=""clr-namespace:System;assembly=mscorlib"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
            //        <ComboBox SelectedValue =""{Binding TeachingId, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"" Margin=""10,0"" IsEditable = ""False"" FontSize = ""18"" Height = ""50"" FontFamily = ""Global User Interface"" SelectedValuePath = ""Teaching.Id"">
            //            <ComboBox.ItemTemplate>
            //                <DataTemplate>
            //                    <StackPanel Orientation=""Horizontal"">
            //                        <StackPanel>
            //                            <TextBlock>
            //                                <TextBlock.Text>
            //                                    <MultiBinding  StringFormat=""{}{0} {1:C}"">
            //                                        <Binding Path=""Teaching.SpecialitySubject.Subject.Name""/>
            //                                        <Binding Path=""Teaching.Teacher.Surname""/>
            //                                    </MultiBinding>
            //                                </TextBlock.Text>
            //                            </TextBlock>
            //                            <ItemsControl BorderBrush=""white"" ItemsSource = ""{Binding MainSchedulesInTheSameTime}"">
            //                                    <ItemsControl.ItemTemplate>
            //                                        <DataTemplate>
            //                                            <StackPanel Orientation = ""Horizontal"">
            //                                                <TextBlock>
            //                                        <Run Text=""{Binding Teaching.SpecialitySubject.Subject.Name}""/>
            //                                        <Run Text=""{Binding ClassNumber}""/>
            //                                        <Run Text=""{Binding Teaching.Teacher.Surname}""/>                
            //                                        <Run Text=""{Binding Teaching.TeacherId}""/>                
            //                                        <Run Text=""{Binding Teaching.Teacher.Name}""/>
            //                                                </TextBlock>
            //                                            </StackPanel>
            //                                        </DataTemplate>
            //                                    </ItemsControl.ItemTemplate>
            //                                </ItemsControl >
            //                        </StackPanel>
            //                        <StackPanel>
            //                            <Image Margin=""10,0,0,0"" Width=""20"" Height=""20"">
            //                                <Image.Style>
            //                                    <Style TargetType=""{x:Type Image}"">
            //                                        <Setter Property=""Source"" Value=""/Image\bad.png""/>
            //                                        <Style.Triggers>
            //                                            <DataTrigger Binding=""{Binding RecommendationLevel}"" Value=""0"">
            //                                                <Setter Property=""Source"" Value=""/Image\good.png""/>
            //                                            </DataTrigger>
            //                                        </Style.Triggers>
            //                                    </Style>
            //                                </Image.Style>
            //                            </Image>
            //                        </StackPanel>
            //                    </StackPanel>
            //                </DataTemplate>
            //            </ComboBox.ItemTemplate>
            //        </ComboBox>
            //        <ComboBox 
            //        SelectedValue =""{Binding ClassroomId, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"" 
            //        Padding=""0"" 
            //        SelectedValuePath = ""Classroom.Id"" 
            //        IsEditable=""False"" 
            //        MinWidth = ""60"" 
            //        FontSize = ""18"" 
            //        Height = ""20"" 
            //        Margin = ""0"" 
            //        FontFamily = ""Global User Interface"" 
            //        HorizontalContentAlignment = ""Center"" 
            //        VerticalContentAlignment = ""Center"" 
            //        HorizontalAlignment = ""Center"">
            //            <ComboBox.ItemTemplate>
            //                <DataTemplate>
            //                    <StackPanel Orientation=""Horizontal"">
            //                            <StackPanel Orientation=""Vertical"">
            //                                <TextBlock>
            //                                    <TextBlock.Text>
            //                                        <MultiBinding  StringFormat=""{}{0} {1}"">
            //                                            <Binding Path=""Classroom.Number""/>
            //                                            <Binding Path=""Classroom.Building""/>
            //                                        </MultiBinding>
            //                                    </TextBlock.Text>
            //                                </TextBlock>
            //                                <ItemsControl BorderBrush=""white"" ItemsSource = ""{Binding MainSchedulesInTheSameClassroom}"">
            //                                    <ItemsControl.ItemTemplate>
            //                                        <DataTemplate>
            //                                            <StackPanel Orientation = ""Horizontal"">
            //                                                <TextBlock>
            //                                                    <TextBlock.Text>
            //                                                        <MultiBinding  StringFormat=""{}{0} {1}"">
            //                                                            <Binding Path=""Teaching.Id""/>
            //                                                            <Binding Path=""Teaching.SpecialitySubject.Subject.Name""/>
            //                                                            <Binding Path=""Teaching.Group.Codename""/>
            //                                                        </MultiBinding>
            //                                                    </TextBlock.Text>
            //                                                </TextBlock>
            //                                            </StackPanel>
            //                                        </DataTemplate>
            //                                    </ItemsControl.ItemTemplate>
            //                                </ItemsControl >
            //                            </StackPanel>
            //                            <Image Margin=""10,0,0,0"" Width=""20"" Height=""20"">
            //                                <Image.Style>
            //                                    <Style TargetType=""{x:Type Image}"">
            //                                        <Setter Property=""Source"" Value=""/Image\bad.png""/>
            //                                        <Style.Triggers>
            //                                            <DataTrigger Binding=""{Binding RecommendationLevel}"" Value=""0"">
            //                                                <Setter Property=""Source"" Value=""/Image\good.png""/>
            //                                            </DataTrigger>
            //                                            <DataTrigger Binding=""{Binding RecommendationLevel}"" Value=""1"">
            //                                                <Setter Property=""Source"" Value=""{x:Null}""/>
            //                                            </DataTrigger>
            //                                        </Style.Triggers>
            //                                    </Style>
            //                                </Image.Style>
            //                            </Image>
            //                        </StackPanel>
            //                </DataTemplate>
            //            </ComboBox.ItemTemplate>
            //        </ComboBox>
            //        </StackPanel>
            //";
            #endregion
            //"<TextBlock.Text>" +
            //                        "<MultiBinding  StringFormat=\"{}{0} {1:C,}\">" +
            //                            "<Binding Path=\"Teaching.SpecialitySubject.Subject.Name\"/>" +
            //                            "<Binding Path=\"ClassNumber\"/>" +
            //                            "<Binding Path=\"Teaching.Teacher.Surname\"/>" +
            //                            "<Binding Path=\"Teaching.Teacher.Name\"/>" +
            //                        "</MultiBinding>" +
            //                    "</TextBlock.Text>" +
            string stacXaml = XamlWriter.Save(Frame.Children[Frame.Children.Count - 1]);
            Frame.Children.Remove(Frame.Children[Frame.Children.Count - 1]);
            //Генерация пар в сетке на основе шаблона
            //string gridXaml = XamlWriter.Save(Frame.Children[Frame.Children.Count - 1]);

            for (int x = 1; x < Frame.ColumnDefinitions.Count; x++)
            {
                for (int y = 2; y < Frame.RowDefinitions.Count - 1; y++)
                {
                    //StringReader stringReader = new StringReader(gridXaml);
                    //XmlReader xmlReader = XmlReader.Create(stringReader);
                    //StackPanel newStPnl = (StackPanel)XamlReader.Load(xmlReader);
                    MainScheduleItemControl newStPnl = new MainScheduleItemControl();
                    ////Биндинги таким методом не переносятся, так что пересоздаем их вручную
                    //Binding binding1 = new Binding()
                    //{
                    //    Path = new PropertyPath("TeachingId")
                    //};

                    //Binding binding2 = new Binding()
                    //{
                    //    Path = new PropertyPath("ClassroomId")
                    //};

                    //((ComboBox)newStPnl.Children[0]).SetBinding(ComboBox.SelectedValueProperty, binding1);
                    //((ComboBox)newStPnl.Children[1]).SetBinding(ComboBox.SelectedValueProperty, binding2);

                    ((ComboBox)newStPnl.StackPanel.Children[0]).SelectionChanged += TeachingSelectionChanged;
                    ((ComboBox)newStPnl.StackPanel.Children[1]).SelectionChanged += ClassroomgSelectionChanged;

                    Grid.SetColumn(newStPnl, x);
                    Grid.SetRow(newStPnl, y);
                    Frame.Children.Add(newStPnl);

                    //Добавляем биндинг
                    MainScheduleLiveModel model = new MainScheduleLiveModel()
                    {
                        //Эти свойства обязательно должны иметь конкретные значения, т.к. работаем с конкретными ячейками
                        DayOfWeekId = getDayOfWeekFromGridColumn(x),
                        ClassNumber = getClassNumberFromGridRow(y)
                    };
                    ((MainScheduleItemControl)Frame.Children[Frame.Children.Count - 1]).DataContext = model;
                    cellDataContexts.Add((getDayOfWeekFromGridColumn(x), getClassNumberFromGridRow(y)), model);

                }

                StringReader stringReader2 = new StringReader(stacXaml);
                XmlReader xmlReader2 = XmlReader.Create(stringReader2);
                StackPanel newStPnl2 = (StackPanel)XamlReader.Load(xmlReader2);

                Label hours = ((Label)newStPnl2.Children[1]);
                hoursInfo.Add(getDayOfWeekFromGridColumn(x), hours);


                Grid.SetColumn(newStPnl2, x);
                Grid.SetRow(newStPnl2, Frame.RowDefinitions.Count - 1);
                Frame.Children.Add(newStPnl2);

            }

            Dispatcher.Invoke(async () =>
            {
                setControlsEnabled(false, InfluenceLevel.GROUPS_SELECTION);
                await loadCommonData();

                if (groupsList.Items.Count > 0)
                    groupsList.SelectedIndex = 0;
            }, DispatcherPriority.Background);
        }


        #region Обработка данных
        //вызывать при создании страницы или после сохранения
        //TODO не вызывать посе сохранения
        private async Task loadCommonData()
        {
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                allMainSchedules = await LearningProcessesAPI.getAllMainSchedules();
                foreach (var mainSch in allMainSchedules)
                {
                    if (mainSch.TeachingId != null)
                    {
                        mainSch.Teaching = await LearningProcessesAPI.getTeaching((int)mainSch.TeachingId);
                    }
                }
                allMainSchedulesBeforeUpdate = allMainSchedules.ToList();

                allClassrooms = await LearningProcessesAPI.getAllClassrooms();

                //Группы
                var groups = await LearningProcessesAPI.getAllGroups();
                groupsList.ItemsSource = groups;
            });
        }

        //вызывать при каждой смене группы
        private async Task loadGroupData()
        {
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                int groupId = ((Group)groupsList.SelectedItem).Id;
                groupCurriculums = await LearningProcessesAPI.getCurricula(groupId);
                groupTeachings = await LearningProcessesAPI.getGroupTeachings(groupId);
            });

        }
        //вызывать при каждой смене семестра
        private async Task reloadSubjectOverview()
        {
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                int? semesterId = ((Semester)semesters.SelectedItem)?.Id;
                specialityHoursItemsSource.Clear();
                if (semesterId != null && semesterId != -1)
                {
                    foreach (var c in groupCurriculums.Where(c => c.SemesterId == semesterId))
                    {
                        if (c.SpecialitySubject.Subject == null)
                        {
                            //Узкое место? TODO
                            c.SpecialitySubject.Subject = await LearningProcessesAPI.getSubject(c.SpecialitySubject.SubjectId);
                        }
                        //Заполняем модель учебных планов
                        CurriculumFillingInfoModel model = new CurriculumFillingInfoModel();
                        model.setCurriculum(c);
                        specialityHoursItemsSource.Add(c.SpecialitySubjectId, model);
                    }
                }
                sujectsOverview.ItemsSource = specialityHoursItemsSource.Values;
            });
        }

        //вызывать при каждой смене семестра
        private void updateCurrentTeachingList()
        {
            int? semesterId = ((Semester)semesters.SelectedItem)?.Id;
            if (semesterId != null || semesterId == -1)
            {
                currentCurriculums = groupCurriculums.Where(c => c.SemesterId == semesterId).ToList();
                currentTeachings = groupTeachings
                    .Where(t =>
                        currentCurriculums.FirstOrDefault(
                            c => c.SpecialitySubjectId == t.SpecialitySubjectId) != null
                    ).ToList();
            }
            else
            {
                currentCurriculums = new List<Curriculum>();
                currentTeachings = new List<Teaching>();
            }
        }

        //Вызывать при каждой смене недели/семестра/(группы)
        private void bindTeachingLists()
        {
            foreach (var el in Frame.Children)
            {
                if (!(el is MainScheduleItemControl))
                    continue;
                int x = getDayOfWeekFromGridColumn(Grid.GetColumn((UIElement)el));
                int y = getClassNumberFromGridRow(Grid.GetRow((UIElement)el));
                if (x >= 0 && x < DAYS && y >= 0 && y < 6)
                {
                    var list = getTeachingsAvailabilityInfo(y, x);
                    list.Sort((t1, t2) => t1.RecommendationLevel - t2.RecommendationLevel);
                    ((ComboBox)((MainScheduleItemControl)el).StackPanel.Children[0]).ItemsSource = list;
                }
            }
        }

        //вызывать для каждого элемента расписания
        //!!!! WARNING ищет расписание только в allMainSchedules !!!!
        private List<TeachingAvailabilityInfo> getTeachingsAvailabilityInfo(int classNumber, int dayOfWeekId)
        {
            List<TeachingAvailabilityInfo> result = new List<TeachingAvailabilityInfo>();
            //TODO принимать в расчет часы
            currentTeachings.ForEach(t =>
            {
                int groupId = t.GroupId;
                int teacherId = t.TeacherId;
                DateTime semesterStart = ((Semester)semesters.SelectedItem).StartDate;
                DateTime semesterEnd = semesterStart.AddDays(((Semester)semesters.SelectedItem).WeeksCount * 7);
                TeachingAvailabilityInfo info = new TeachingAvailabilityInfo()
                {
                    Teaching = t,
                    GroupId = groupId,
                    TeacherId = teacherId,
                    ClassNumber = classNumber,
                    DayOfWeekId = dayOfWeekId,
                    IsRedWeek = red.IsChecked.Value,
                    SemesterStart = semesterStart,
                    SemesterEnd = semesterEnd
                };
                var mainSchedules = allMainSchedules.Where(m =>
                    m.ClassNumber == classNumber
                    && m.DayOfWeekId == dayOfWeekId
                    && m.IsRedWeek == red.IsChecked.Value
                    && m.Teaching?.TeacherId == teacherId
                    && m.Semester?.StartDate >= semesterStart
                    && m.Semester?.StartDate < semesterEnd
                    && m.SemesterId != ((Semester)semesters.SelectedItem).Id
                ).ToList();
                info.MainSchedulesInTheSameTime = mainSchedules;
                result.Add(info);
            });
            return result;
        }

        //вызывать после выбора преподавания
        //!!!! WARNING ищет расписание только в allMainSchedules !!!!
        private List<ClassroomAvailabilityInfo> getClassroomsAvailabilityInfo(TeachingAvailabilityInfo selectedTeachingInfo)
        {
            List<ClassroomAvailabilityInfo> result = new List<ClassroomAvailabilityInfo>();
            allClassrooms.ForEach(c =>
            {
                ClassroomAvailabilityInfo info = new ClassroomAvailabilityInfo()
                {
                    Classroom = c,
                    ClassNumber = selectedTeachingInfo.ClassNumber,
                    DayOfWeekId = selectedTeachingInfo.DayOfWeekId,
                    GroupId = selectedTeachingInfo.GroupId,
                    IsRecommended = selectedTeachingInfo.TeacherId == c.TeacherId,
                    TeacherId = selectedTeachingInfo.TeacherId,
                    IsRedWeek = selectedTeachingInfo.IsRedWeek,
                    SemesterStart = selectedTeachingInfo.SemesterStart,
                    SemesterEnd = selectedTeachingInfo.SemesterEnd
                };
                var mainSchedules = allMainSchedules.Where(m =>
                    m.ClassNumber == selectedTeachingInfo.ClassNumber
                    && m.DayOfWeekId == selectedTeachingInfo.DayOfWeekId
                    && m.IsRedWeek == red.IsChecked.Value
                    && m.ClassroomId == c.Id
                    && m.Semester?.StartDate >= info.SemesterStart
                    && m.Semester?.StartDate < info.SemesterEnd
                    && m.SemesterId != ((Semester)semesters.SelectedItem).Id
                //TODO настроить фильтр по семестру
                ).ToList();
                info.MainSchedulesInTheSameClassroom = mainSchedules;
                result.Add(info);
            });
            return result;
        }

        //Вызывать при обновлении недели/семестра/(группы)
        private void bindMainSchedulesToGrid()
        {
            int semesterId = ((Semester)semesters.SelectedItem)?.Id ?? -1;
            bool isRedWeek = red.IsChecked.Value;
            var mainSchedules = allMainSchedules.Where(m =>
                m.SemesterId == semesterId
                && m.IsRedWeek == isRedWeek
            ).ToList();

            for (int x = 0; x < DAYS; x++)
            {
                for (int y = 0; y < 6; y++)
                {
                    MainSchedule mainSchedule = mainSchedules.FirstOrDefault(m =>
                        m.DayOfWeekId == x
                        && m.ClassNumber == y
                    ) ?? new MainSchedule()
                    {
                        Id = -1,
                        ClassNumber = y,
                        DayOfWeekId = x,
                        IsRedWeek = isRedWeek,
                        SemesterId = semesterId,
                        TeachingId = null,
                        ClassroomId = null
                    };
                    cellDataContexts[(x, y)].setMainSchedule(mainSchedule);
                }
            }
        }

        //Вызывать перед обновлением недели/семестра/(группы)
        private void bindGridToMainSchedules()
        {
            cellDataContexts.Values.ToList().ForEach(mlm =>
            {
                //Не сохраняем для пустых семестров
                if (mlm.SemesterId == -1 || mlm.SemesterId == 0)
                    return;
                int index = allMainSchedules.FindIndex(m =>
                    m.SemesterId == mlm.SemesterId
                    && m.IsRedWeek == mlm.IsRedWeek
                    && m.ClassNumber == mlm.ClassNumber
                    && m.DayOfWeekId == mlm.DayOfWeekId
                );
                var scheduleItem = mlm.getMainSchedule();
                //Подгоняем Teaching под TeachingId
                if (scheduleItem.TeachingId != null)
                {
                    scheduleItem.Teaching = currentTeachings.Find(t => t.Id == scheduleItem.TeachingId);
                }
                if (index == -1)
                {
                    allMainSchedules.Add(scheduleItem);
                }
                else
                {
                    allMainSchedules[index] = scheduleItem;
                }
            });
        }

        //Вызывать при обновлении семестров или очистке сетки
        private void rebeindAndUpdateCellsContent()
        {
            //Сохраняем старое
            bindGridToMainSchedules();
            //Меняем преподавания и биндим их
            updateCurrentTeachingList();
            //Биндим к новому
            bindMainSchedulesToGrid();
        }

        //Вызывать при смене цвета недели
        private void rebeindCellsContent()
        {
            //Сохраняем старое
            bindGridToMainSchedules();
            //Биндим преподавания
            bindTeachingLists();
            //Биндим к новому
            bindMainSchedulesToGrid();
        }

        //Вызывать для сохранения изменений
        private async Task saveMainSchedules()
        {
            Shed_save.IsEnabled = false;
            bool errorHappened = false;

            List<MainSchedule> newMainSchedules = new List<MainSchedule>();
            List<MainSchedule> updatedMainSchedules = new List<MainSchedule>();
            allMainSchedules.ForEach(m =>
            {
                if (m.Id == -1)
                    newMainSchedules.Add(m);
                else
                {
                    MainSchedule m2 = allMainSchedulesBeforeUpdate[allMainSchedules.IndexOf(m)];
                    if (m2.ClassroomId != m.ClassroomId || m2.TeachingId != m.TeachingId)
                    {
                        updatedMainSchedules.Add(m);
                    }
                }
            });
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                if (newMainSchedules.Count > 0)
                {
                    var added = await LearningProcessesAPI.createMultipleMainSchedules(newMainSchedules);
                    if (added.Count == 0)
                    {
                        throw new ServerErrorException("Сервер вернул ошибку 404 при обновлении расписания");
                    }
                    added.ForEach(a =>
                    {
                        allMainSchedules[allMainSchedules.FindIndex(u1 =>
                            u1.ClassNumber == a.ClassNumber
                            && u1.DayOfWeekId == a.DayOfWeekId
                            && u1.IsRedWeek == a.IsRedWeek
                            && u1.SemesterId == a.SemesterId)] = a;
                    });
                }
                if (updatedMainSchedules.Count > 0)
                {
                    var updated = await LearningProcessesAPI.updateMultipleMainSchedules(updatedMainSchedules);
                    if (updated.Count == 0)
                    {
                        throw new ServerErrorException("Сервер вернул ошибку 404 при обновлении расписания (не найдены обновляемые компоненты)");
                    }
                    updated.Values.ToList().ForEach(u =>
                    {
                        allMainSchedules[allMainSchedules.FindIndex(u2 => u2 == u)] = u;
                    });
                }

                //Новое состояние. Мы сюда не доходим в случае throw
                allMainSchedulesBeforeUpdate = allMainSchedules.ToList();
            });

            if (!errorHappened)
            {
                MessageBox.Show($"Успешно сохранено ({newMainSchedules.Count} добавлено, {updatedMainSchedules.Count} обновлено)", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //Вызывать при любом обновлении тичингов?
        //TODO понять не слишком ли дорого циклы постоянно гонять?
        private void recalculateSpecialityHours()
        {
            if (semesters.SelectedItem == null)
                return;
            int semesterId = ((Semester)semesters.SelectedItem)?.Id ?? -1;
            bool isBlueWeek = !red.IsChecked.Value;
            int redWeeks = WeeksColoringUtils.getRedWeeksCount((Semester)semesters.SelectedItem);
            int blueWeeks = WeeksColoringUtils.getBlueWeeksCount((Semester)semesters.SelectedItem);
            //Выбираем данные из того-же семестра, но для другой недели
            //Для этой недели будем перебирать комбобоксы
            var mainSchedules = allMainSchedules.Where(m =>
                m.SemesterId == semesterId
                && m.IsRedWeek == isBlueWeek
            ).ToList();
            //Очищаем

            bool takeFromGrid = (WeeksColoringUtils.getWeekColor(((Semester)semesters.SelectedItem).StartDate) == WeeksColoringUtils.WeekColors.RED) == red.IsChecked.Value;

            specialityHoursItemsSource.Values.ToList().ForEach(m => m.AllocatedHours = 0);

            for (int week = 0; week < ((Semester)semesters.SelectedItem).WeeksCount; week++)
            {
                for(int day = 0; day < DAYS; day++)
                {
                    DateTime date = ((Semester)semesters.SelectedItem).StartDate.AddDays((7 * week) + day);
                    //Скипаем практики в подсчете часов
                    if (currentCurriculums.Count(c => c.PracticeSchedule != null && c.PracticeSchedule.StartDate <= date && c.PracticeSchedule.EndDate >= date) > 0)
                        continue;
                    if (takeFromGrid)
                    {
                        //Перебираем интерфейс
                        foreach (var e in Frame.Children)
                        {
                            if (!(e is MainScheduleItemControl))
                                continue;
                            int x = getDayOfWeekFromGridColumn(Grid.GetColumn((UIElement)e));
                            int y = getClassNumberFromGridRow(Grid.GetRow((UIElement)e));
                            if (x == day && y >= 0 && y < 6)
                            {
                                if (((ComboBox)((MainScheduleItemControl)e).StackPanel.Children[0]).SelectedIndex > -1)
                                {
                                    int ssId = ((TeachingAvailabilityInfo)((ComboBox)((MainScheduleItemControl)e).StackPanel.Children[0]).SelectedItem).Teaching.SpecialitySubjectId;
                                    if (!specialityHoursItemsSource.Keys.Contains(ssId))
                                        continue;
                                    specialityHoursItemsSource[ssId].AllocatedHours += 2;
                                    //Конец для четного и нечетного общего числа часов
                                    if(specialityHoursItemsSource[ssId].AbsoluteNotAllocatedHours == -1 || specialityHoursItemsSource[ssId].AbsoluteNotAllocatedHours == 0)
                                    {
                                        specialityHoursItemsSource[ssId].LastDayFact = date;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //Начинаем заполнять с mainSchedules
                        mainSchedules.ForEach(m =>
                        {
                            if (m.TeachingId == null)
                                return;
                            if(m.DayOfWeekId == day)
                            {
                                int ssId = m.Teaching.SpecialitySubjectId;
                                if (!specialityHoursItemsSource.Keys.Contains(ssId))
                                    return;
                                //По идее, здесь должны быть все предметы (если только в бд нет несоответвтвий куррикулумов и присутсвующих MainSchedule)
                                specialityHoursItemsSource[ssId].AllocatedHours += 2;
                                //Конец для четного и нечетного общего числа часов
                                if (specialityHoursItemsSource[ssId].AbsoluteNotAllocatedHours == -1 || specialityHoursItemsSource[ssId].AbsoluteNotAllocatedHours == 0)
                                {
                                    specialityHoursItemsSource[ssId].LastDayFact = date;
                                }
                            }
                        });
                    }   
                }
                takeFromGrid = !takeFromGrid;
            }

            //clock.Height = 40;
            //DoubleAnimation buttonAnimation = new DoubleAnimation();
            //buttonAnimation.From = clock.Height;
            //buttonAnimation.To = 30;
            //buttonAnimation.Duration = TimeSpan.FromSeconds(1);
            //buttonAnimation.RepeatBehavior = new RepeatBehavior(5);
            ////buttonAnimation.To = 30;
            //clock.BeginAnimation(Image.HeightProperty, buttonAnimation);
            //buttonAnimation.AutoReverse = true;
            ////            buttonAnimation.

            //DoubleAnimation buttonAnimation1 = new DoubleAnimation();
            //buttonAnimation1.From = clock.ActualHeight;
            //buttonAnimation1.To = 30;
            //buttonAnimation1.Duration = TimeSpan.FromSeconds(1);
            ////            buttonAnimation1.RepeatBehavior = new RepeatBehavior(1);
            //clock.BeginAnimation(Image.HeightProperty, buttonAnimation1);
            ////Заменяем старое на новое
            sujectsOverview.ItemsSource = specialityHoursItemsSource.Values.ToList();
        }
        #endregion

        #region Проверки и конвертеры

        //Конвертеры
        private int getDayOfWeekFromGridColumn(int column) => column - 1;
        private int getClassNumberFromGridRow(int row) => row - 2;
        private int getGridColumnFromDayOfWeek(int dayOfWeek) => dayOfWeek + 1;
        private int getGridRowFromClassNumber(int classNumber) => classNumber + 2;

        //Проверка затрагивания различных уровней интерфйеса на основе изменяемого уровня и задаваемого значения
        //Если разрешаем, то мы разрешакем высшие (более общие)
        //Если запрещаем, то мы запрещаем низшие (более частные)
        private bool isLevelAffected(InfluenceLevel level, InfluenceLevel compareTo, bool value) =>
            (level >= compareTo && value)
            || (level <= compareTo && !value);

        private void setControlsEnabled(bool value, InfluenceLevel level)
        {
            if (isLevelAffected(InfluenceLevel.CLASSROOM_SELECTION, level, value) || isLevelAffected(InfluenceLevel.TEACHING_SELECTION, level, value))
            {
                foreach (var e in Frame.Children)
                {
                    if (!(e is MainScheduleItemControl))
                        continue;
                    int x = getDayOfWeekFromGridColumn(Grid.GetColumn((UIElement)e));
                    int y = getClassNumberFromGridRow(Grid.GetRow((UIElement)e));
                    if (x >= 0 && x < DAYS && y >= 0 && y < 6)
                    {
                        if (isLevelAffected(InfluenceLevel.CLASSROOM_SELECTION, level, value))
                            ((MainScheduleItemControl)e).StackPanel.Children[1].IsEnabled = value;
                        if (isLevelAffected(InfluenceLevel.TEACHING_SELECTION, level, value))
                            ((MainScheduleItemControl)e).StackPanel.Children[0].IsEnabled = value;
                    }
                }
            }
            if (isLevelAffected(InfluenceLevel.WEEK_COLOR_SELECTION, level, value))
            {
                red.IsEnabled = value;
                blue.IsEnabled = value;
            }
            if (isLevelAffected(InfluenceLevel.SEMESTERS_SELECTION, level, value))
            {
                semesters.IsEnabled = value;
            }
            if (isLevelAffected(InfluenceLevel.GROUPS_SELECTION, level, value))
            {
                groupsList.IsEnabled = value;
                previous.IsEnabled = value;
                next.IsEnabled = value;
            }
        }

        private void setControlsEnabled(bool value, InfluenceLevel level, int col, int row)
        {
            if (level > InfluenceLevel.TEACHING_SELECTION)
            {
                setControlsEnabled(value, level);
                return;
            }
            else
            {
                foreach (var e in Frame.Children)
                {
                    if (!(e is MainScheduleItemControl))
                        continue;
                    int x = Grid.GetColumn((UIElement)e);
                    int y = Grid.GetRow((UIElement)e);
                    if (x == col && y == row)
                    {
                        if (isLevelAffected(InfluenceLevel.CLASSROOM_SELECTION, level, value))
                            ((MainScheduleItemControl)e).StackPanel.Children[1].IsEnabled = value;
                        if (isLevelAffected(InfluenceLevel.TEACHING_SELECTION, level, value))
                            ((MainScheduleItemControl)e).StackPanel.Children[0].IsEnabled = value;
                        break;
                    }
                }
            }
        }

        //Вызывать при покидании страницы или сохранении
        private bool validatePage()
        {
            bool isValid = true;
            foreach (var el in Frame.Children)
            {
                if (!(el is MainScheduleItemControl))
                    continue;
                int x = getDayOfWeekFromGridColumn(Grid.GetColumn((UIElement)el));
                int y = getClassNumberFromGridRow(Grid.GetRow((UIElement)el));
                if (x >= 0 && x < DAYS && y >= 0 && y < 6)
                {
                    MainScheduleLiveModel model = cellDataContexts[(x, y)];
                    if (model.ClassroomId == null && model.TeachingId == null)
                    {
                        continue;
                    }
                    else if (model.ClassroomId == null)
                    {
                        setValid((((MainScheduleItemControl)el).StackPanel.Children[1] as ComboBox), false);
                        isValid = false;
                    }
                    else
                    {
                        //Недостижимо?
                        //setValid((StackPanel)el).Children[0],false);
                    }
                }
            }
            return isValid;
        }

        private void updateHoursInfo(int dayOfWeek)
        {
            int hours = 0;
            foreach (var e in Frame.Children)
            {
                if (!(e is MainScheduleItemControl))
                    continue;
                int x = Grid.GetColumn((UIElement)e);
                int y = Grid.GetRow((UIElement)e);
                if (x == getGridColumnFromDayOfWeek(dayOfWeek) && y >= 2 && y < Frame.RowDefinitions.Count - 1)
                {
                    if (((ComboBox)((MainScheduleItemControl)e).StackPanel.Children[0]).SelectedIndex > -1)
                    {
                        hours += 2;
                    }
                }
            }
            //TODO подтянуть модель
            hoursInfo[dayOfWeek].Content = hours;
            if (hours > (groupsList.SelectedItem as Group).Speciality.MaxDailyHours) {
                hoursInfo[dayOfWeek].Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                hoursInfo[dayOfWeek].ToolTip = "Вы привысили максимально возможно количество часов в день для данной группы";
            }
            else
            {
                current.Background = Brushes.White;
                current.ToolTip = null;
            }
            
            int sum = 0;
            foreach (KeyValuePair<int, Label> keyValue in hoursInfo)
            {
                sum += Convert.ToInt32((keyValue.Value).Content);
            }
            current.Text = sum.ToString();
            if (Convert.ToInt32(total.Text) < Convert.ToInt32(current.Text))
            {
                current.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                current.ToolTip = "Вы привысили максимально возможно количество часов в неделю для данной группы";
            }
            else
            {
                current.Background = Brushes.White;
                current.ToolTip = null;
            }

            //Обновляем боковое меню
            recalculateSpecialityHours();
        }
        #endregion

        #region Логика страницы
        #region Перемещение по groupsList
        private void previous_Click(object sender, RoutedEventArgs e)
        {
            if (groupsList.SelectedIndex > 0)
                groupsList.SelectedIndex = groupsList.SelectedIndex - 1;
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            if (groupsList.SelectedIndex < groupsList.Items.Count - 1)
                groupsList.SelectedIndex = groupsList.SelectedIndex + 1;
        }
        #endregion

        //Выбираем группу
        private void groupsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (groupsList.SelectedIndex != currentGroupIndex)
            {
                if (validatePage())
                {
                    if (groupsList.SelectedItem != null)
                    {
                        currentGroupIndex = groupsList.SelectedIndex;

                        semesters.ItemsSource = (groupsList.SelectedItem as Group).Semesters;

                        //Обнуляем для каждой новой группы
                        currentSemesterIndex = -1;

                        if (semesters.Items.Count > 0)
                        {
                            Dispatcher.Invoke(async () =>
                            {
                                await loadGroupData();
                                semesters.SelectedIndex = 0;
                            }, DispatcherPriority.Background);
                        }
                        else if (semesters.ItemsSource != null)
                        {
                            MessageBox.Show("У выбранной группы отсуствуют семестры!", "Недостаточно данных", MessageBoxButton.OK, MessageBoxImage.Warning);
                            setControlsEnabled(false, InfluenceLevel.SEMESTERS_SELECTION);

                            Dispatcher.Invoke(async () =>
                            {
                                await loadGroupData();
                                rebeindAndUpdateCellsContent();
                            }, DispatcherPriority.Background);
                            //Очистка экрана

                            //TODO предложить создать их на месте или перейти в нужное окно
                        }
                    }
                    else
                    {
                        semesters.ItemsSource = null;
                        setControlsEnabled(false, InfluenceLevel.SEMESTERS_SELECTION);
                        //Очистка экрана
                        rebeindAndUpdateCellsContent();
                    }

                }
                else
                {
                    MessageBox.Show("Необходимо установить аудиторию(и)", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);

                    //Для избежания дополнительных сообщений об ошибке
                    groupAreRollingBack = true;
                    groupsList.SelectedIndex = currentGroupIndex;
                    groupAreRollingBack = false;
                }
            }
        }

        //Выбираем семестр
        private void semesters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (semesters.SelectedIndex != currentSemesterIndex)
            {
                if (validatePage())
                {
                    if (semesters.SelectedIndex > -1)
                    {
                        if (semesters.SelectedItem != null)
                        {
                            currentSemesterIndex = semesters.SelectedIndex;
                            setControlsEnabled(false, InfluenceLevel.GROUPS_SELECTION);

                            rebeindAndUpdateCellsContent();

                            Dispatcher.Invoke(async () =>
                            {
                                await reloadSubjectOverview();

                                //сбрасываем значение для срабатывания события
                                if (red.IsChecked.Value)
                                {
                                    red.IsChecked = false;
                                }
                                red.IsChecked = true;

                                setControlsEnabled(true, InfluenceLevel.WEEK_COLOR_SELECTION);
                            }, DispatcherPriority.Background);

                            
                        }
                        else
                        {
                            setControlsEnabled(false, InfluenceLevel.WEEK_COLOR_SELECTION);

                            Dispatcher.Invoke(async () =>
                            {
                                await reloadSubjectOverview();
                                //Очистка экрана
                                rebeindAndUpdateCellsContent();
                            }, DispatcherPriority.Background);
                        }
                    }
                }
                else
                {
                    if (!groupAreRollingBack)
                        MessageBox.Show("Необходимо установить аудиторию(и)", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                    semesters.SelectedIndex = currentSemesterIndex;
                }
            }

        }

        #region Цвет недели
        private void blue_Checked(object sender, RoutedEventArgs e)
        {
            if (validatePage())
            {
                red.IsChecked = false;
                labelWeek.Foreground = Brushes.Blue;
                labelWeek.Content = "Синяя неделя";
                //updateWeekColorRelatedContent();

                setControlsEnabled(false, InfluenceLevel.GROUPS_SELECTION);

                rebeindCellsContent();

                setControlsEnabled(true, InfluenceLevel.TEACHING_SELECTION);
            }
            else
            {
                if (!groupAreRollingBack)
                    MessageBox.Show("Необходимо установить аудиторию(и)", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                blue.IsChecked = !blue.IsChecked;
            }
        }

        private void red_Checked(object sender, RoutedEventArgs e)
        {
            if (validatePage())
            {
                blue.IsChecked = false;
                labelWeek.Foreground = Brushes.Red;
                labelWeek.Content = "Красная неделя";
                //updateWeekColorRelatedContent();

                setControlsEnabled(false, InfluenceLevel.GROUPS_SELECTION);

                rebeindCellsContent();

                setControlsEnabled(true, InfluenceLevel.TEACHING_SELECTION);
            }
            else
            {
                if (!groupAreRollingBack)
                    MessageBox.Show("Необходимо установить аудиторию(и)", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                red.IsChecked = !blue.IsChecked;
            }
        }
        #endregion

        //При выборе преподавания
        private void TeachingSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox classroomsCB = ((ComboBox)((StackPanel)((ComboBox)sender).Parent).Children[1]);

            updateHoursInfo(getDayOfWeekFromGridColumn(Grid.GetColumn(((MainScheduleItemControl)((StackPanel)classroomsCB.Parent).Parent))));

            if (((ComboBox)sender).SelectedItem == null)
            {
                classroomsCB.ItemsSource = null;
                setControlsEnabled(false, InfluenceLevel.CLASSROOM_SELECTION, Grid.GetColumn(((MainScheduleItemControl)((StackPanel)classroomsCB.Parent).Parent)), Grid.GetRow(((MainScheduleItemControl)((StackPanel)classroomsCB.Parent).Parent)));
            }
            else
            {
                var list = getClassroomsAvailabilityInfo(((ComboBox)sender).SelectedItem as TeachingAvailabilityInfo);
                list.Sort((t1, t2) => t1.RecommendationLevel - t2.RecommendationLevel);
                classroomsCB.ItemsSource = list;
                setControlsEnabled(true, InfluenceLevel.CLASSROOM_SELECTION, Grid.GetColumn(((MainScheduleItemControl)((StackPanel)classroomsCB.Parent).Parent)), Grid.GetRow(((MainScheduleItemControl)((StackPanel)classroomsCB.Parent).Parent)));
                //Вызываем валидацию
                classroomsCB.SelectedIndex = 0;
                classroomsCB.SelectedIndex = -1;
                classroomsCB.IsDropDownOpen = true;

               
            }

        }

        private void ClassroomgSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            setValid(sender as ComboBox, true);
        }

        private void Shed_save_Click(object sender, RoutedEventArgs e)
        {
            if (validatePage())
            {
                Dispatcher.Invoke(async () =>
                {
                    Shed_save.IsEnabled = false;
                    setControlsEnabled(false, InfluenceLevel.GROUPS_SELECTION);
                    bindGridToMainSchedules();
                    await saveMainSchedules();
                    //Исправляем потенциальные проблемы с доступностью
                    bindTeachingLists();
                    bindMainSchedulesToGrid();
                    setControlsEnabled(true, InfluenceLevel.TEACHING_SELECTION);
                    Shed_save.IsEnabled = true;
                }, DispatcherPriority.Background);
            }
            else
            {
                MessageBox.Show("Необходимо установить аудиторию(и)", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void setValid(ComboBox comboBox, bool isValid)
        {
            //TODO проверить на другой машине
            comboBox.Background = isValid ? Brushes.Transparent : Brushes.Red;
            comboBox.Focus();
            //comboBox.IsDropDownOpen = true;
            //Style buttonStyle = new Style(typeof(ComboBox));
            ////buttonStyle.BasedOn = new Style(typeof(ComboBox));
            ////buttonStyle.BasedOn = (Style)FindResource(typeof(ComboBox));
            //buttonStyle.Setters.Add(new Setter { Property = Control.BackgroundProperty, Value = new SolidColorBrush(Colors.Black) });
            ////BasedOn = "{StaticResource {x:Type ComboBox}
            ////buttonStyle.BasedOn = "{StaticResource {x:Type ComboBox}";
            //comboBox.Style = buttonStyle;

        }
        #endregion

        private void romb_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
           
        }

        private async void Export_Click(object sender, RoutedEventArgs e)
        {
            DateTime semesterStart = ((Semester)semesters.SelectedItem).StartDate;
            await Utils.SheduleExport.Export(allMainSchedules, semesterStart);
        }
    }



}
