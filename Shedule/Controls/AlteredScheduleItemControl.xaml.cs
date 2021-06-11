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
    }
}
