using Shedule.Controls;
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

namespace Shedule.Pages
{
    /// <summary>
    /// Логика взаимодействия для Сhanges.xaml
    /// </summary>
    public partial class Сhanges : Page
    {
        class Person
        {
            private int name; // field
            public int Name   // property
            {
                get { return name; }
                set { name = value; }
            }
        }
        public Сhanges()
        {
            InitializeComponent();
            loadGroups();
        }
        async Task loadGroups()
        {
            await AppUtils.ProcessClientLibraryRequest(async () =>
            {
                var rezult = await LearningProcessesAPIClient.api.LearningProcessesAPI.getAllGroups();
                //tet.ItemsSource = rezult;
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Person k = new Person();
            k.Name = 1;
            Frame.RowDefinitions.Add(new RowDefinition());
            AlteredRowItemControl control = new AlteredRowItemControl();
            Grid.SetRow(control, Frame.RowDefinitions.Count-2);
            Grid.SetColumnSpan(control, 7);
            Frame.Children.Add(control);
        }
    }
}
