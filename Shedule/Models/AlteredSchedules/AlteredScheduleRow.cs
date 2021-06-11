using LearningProcessesAPIClient.model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Shedule.Models.AlteredSchedules
{
    public class AlteredScheduleRow : INotifyPropertyChanged
    {
        private static List<AlteredScheduleRow> alteredScheduleRows = new List<AlteredScheduleRow>();
        public Group SelectedGroup { get; set; }

        public static List<Group> GroupsList { get; } = new List<Group>();
        
        public List<Group> AvailableGroupsList {
            get
            {
                return GroupsList.Except(UsedGroupsList.Where(u => u != SelectedGroup)).ToList();
            }
        }

        public static MyList<Group> UsedGroupsList { get; } = new MyList<Group>();
        public static List<Classroom> AllClassrooms { get; } = new List<Classroom>();

        public AlteredScheduleRow() : base()
        {
            alteredScheduleRows.Add(this);
            UsedGroupsList.OnChanged += update;
        }

        public void update()
        {
            alteredScheduleRows.ForEach(a => a.OnPropertyChanged("AvailableGroupsList"));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


        //public AlteredSchedule[] AlteredSchedules { get; } = new AlteredSchedule[6];

        public class MyList<T> : List<T> {
            public delegate void Notifier();
            public event Notifier OnChanged;

            public new void Add(T item)
            {
                base.Add(item);
                OnChanged?.Invoke();
            }

            public new void Remove(T item)
            {
                base.Remove(item);
                OnChanged?.Invoke();
            }

            public new void RemoveAt(int pos)
            {
                base.RemoveAt(pos);
                OnChanged?.Invoke();
            }
        }

    }
}
