using System;
using System.Collections.Generic;
using System.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Weekly_ToDo_List
{
    [Serializable()]
    public class Classes
    {
        public Classes() { }
        public Classes(String name, String color)
        {
            this.name = name;
            this.color = color;
            this.id = HandleData.GenerateNumForNewClass();
        }

        public override string ToString()
        {
            return Name;
        }

        private string name;
        public String Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                HandleData.SetModified(true);
            }
        }

        private String color;
        public String Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
                HandleData.SetModified(true);
            }
        }

        private int id;
        public int ID
        {
            get
            {
                return id;
            }
        }


    }
}
