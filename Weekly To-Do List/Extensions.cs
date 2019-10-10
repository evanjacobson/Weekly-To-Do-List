using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Weekly_ToDo_List
{
    public static class Extensions
    {
        //extension method -- find sunday's date
        public static DateTime Sunday(this DateTime dt)
        {
            DayOfWeek startOfWeek = DayOfWeek.Sunday;
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            DayOfWeek SundayDay = dt.AddDays(-1 * diff).DayOfWeek;
            return dt.AddDays(-1 * diff).Date;
        }

        //extension method -- saturday's date
        public static DateTime Saturday(this DateTime dt)
        {
            DayOfWeek endOfWeek = DayOfWeek.Saturday;
            int diff = (7 - (dt.DayOfWeek - endOfWeek)) % 7;
            DateTime SaturdayDay = dt.AddDays(1 * diff);
            return dt.AddDays(1 * diff).Date;
        }

        private static Random rng = new Random();

        //shuffle list
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        //extension method -- get listbox's datetime
        public static DateTime GetDateTime(this ListBox listBox, DateTime sunday)
        {
            switch (listBox.Name)
            {
                case "Sunday":
                    return sunday;
                case "Monday":
                    return sunday.AddDays(1);
                case "Tuesday":
                    return sunday.AddDays(2);
                case "Wednesday":
                    return sunday.AddDays(3);
                case "Thursday":
                    return sunday.AddDays(4);
                case "Friday":
                    return sunday.AddDays(5);
                case "Saturday":
                    return sunday.AddDays(6);
            }
            return sunday;
        }
    }
}
