using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shedule.Models.AlteredSchedules
{

    public delegate void AlteredScheduleUpdateEventHandler();

    public abstract class AlteredSchedulesObserver
    {
        protected AlteredSchedulesObserver()
        {
            AlteredScheduleRow.OnAlteredSchedulesUpdated += onAlteredSchedulesUpdated;
        }

        public abstract void onAlteredSchedulesUpdated();

    }
}
