using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController.Limits
{
    public class ZoneSchedule
    {
        public int zone;
        public DateTime start;
        public DateTime end;
        public double? threshold;
        public ScheduleTypes type;
    }
}
