using LearningProcessesAPIClient.api;
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
    /// Логика взаимодействия для Semesters.xaml
    /// </summary>
    public partial class Semesters : Page
    {
        public Semesters()
        {
            InitializeComponent();
            sss();
        }

        public async Task sss()
        {
            try
            {
                //var result = await LearningProcessesAPI.getAllGroups();
                //SemestrListView.ItemsSource = result;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }


        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double remainingSpace = SemestrListView.ActualWidth;

            if (remainingSpace > 0)
            {

                (SemestrListView.View as GridView).Columns[0].Width = Math.Ceiling(remainingSpace / 3);
                (SemestrListView.View as GridView).Columns[1].Width = Math.Ceiling(remainingSpace / 3);
                (SemestrListView.View as GridView).Columns[2].Width = Math.Ceiling(remainingSpace / 3);
            }
        }
    }
}
