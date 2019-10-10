using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Weekly_ToDo_List
{
    [Serializable()]
    public class Entry
    {

        public Entry() { } //unused

        public Entry(String assignment, Classes forClass, String notes, DateTime assignToDate, DateTime dueOn, int MinutesToComplete, bool repeats_daily, bool repeats_weekly, bool repeats_monthly, bool repeats_yearly)
        {
            this.assignToDate = assignToDate.Date;
            this.dueOn = dueOn.Date;
            this.assignment = assignment;
            this.forClass = forClass;
            this.notes = notes;
            this.completed = false;
            this.deleted = false;
            this.minutesToComplete = MinutesToComplete;

            Repeats_Daily = repeats_daily;
            Repeats_Weekly = repeats_weekly;
            Repeats_Monthly = repeats_monthly;
            Repeats_Yearly = repeats_yearly;

            Repeats = (repeats_daily || repeats_weekly || repeats_monthly || repeats_yearly) ? true : false;
               
            
        }

        public bool Repeats { get; set; }

        private String assignment;
        public String Assignment
        {
            get
            {
                return assignment;
            }
            set
            {
                assignment = value;
                HandleData.SetModified(true);
            }
        }

        private Classes forClass;
        public Classes ForClass
        {
            get
            {
                return (forClass == null) ? new Classes("Misc", "Gray") : forClass;
            }
            set
            {
                forClass = value;
                HandleData.SetModified(true);
            }
        }

        private String notes;
        public String Notes
        {
            get
            {
                return notes;
            }
            set
            {
                notes = value;
                HandleData.SetModified(true);
            }
        }

        private DateTime dueOn;
        public DateTime DueOn
        {
            get
            {
                return dueOn;
            }
            set
            {
                dueOn = value;
                HandleData.SetModified(true);
            }
        }

        private DateTime assignToDate;
        public DateTime AssignToDate
        {
            get
            {
                return assignToDate;
            }
            set
            {
               assignToDate = value;
                HandleData.SetModified(true);
            }
        }

        private Boolean completed = false;
        public Boolean Completed
        {
            get
            {
                if (!Repeats)
                    return completed;
                return false;
            }
            set
            {
                completed = value;
                HandleData.SetModified(true);
            }
        }

        private Boolean deleted = false;
        public Boolean Deleted
        {
            get
            {
                return deleted;
            }
            set
            {
                deleted = value;
                HandleData.SetModified(true);
            }
        }

        private int minutesToComplete;
        public int MinutesToComplete
        {
            get
            {
                return minutesToComplete;
            }
            set
            {
                minutesToComplete = value;
                HandleData.SetModified(true);
            }
        }

        private bool repeats_daily = false;
        public Boolean Repeats_Daily
        {
            get
            {
                return repeats_daily;
            }
            set
            {
                repeats_daily = value;
                HandleData.SetModified(true);
            }
        }

        private bool repeats_weekly = false;
        public Boolean Repeats_Weekly
        {
            get
            {
                return repeats_weekly;
            }
            set
            {
                repeats_weekly = value;
                HandleData.SetModified(true);
            }
        }

        private bool repeats_monthly = false;
        public Boolean Repeats_Monthly
        {
            get
            {
                return repeats_monthly;
            }
            set
            {
                repeats_monthly = value;
                HandleData.SetModified(true);
            }
        }

        private bool repeats_yearly = false;
        public Boolean Repeats_Yearly
        {
            get
            {
                return repeats_yearly;
            }
            set
            {
                repeats_yearly = value;
                HandleData.SetModified(true);
            }
        }
        public List<DateTime> DatesCompleted { get; } = new List<DateTime>();

        public void AddCompletedDay(DateTime date)
        {
            DatesCompleted.Add(date.Date);
            HandleData.SetModified(true);
        }

        public bool HasCompletedDate(DateTime date)
        {
            if(DatesCompleted != null)
            {
                if (DatesCompleted.Contains(date.Date))
                    return true;
            }
            return false;
        }

        public List<DateTime> DatesDeleted { get; } = new List<DateTime>();

        public void AddDeletedDay(DateTime date)
        {
            DatesDeleted.Add(date.Date);
            HandleData.SetModified(true);
        }

        public bool HasDeletedDate(DateTime date)
        {
            date = date.Date;
            if (DatesDeleted != null)
            {
                if (DatesDeleted.Contains(date))
                    return true;
            }
            return false;
        }

        public override string ToString()
        {
            String str = $"{this.Assignment}"; 

            return str;
        }
    }
}
