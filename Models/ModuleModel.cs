using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VIPER.Models
{
    public class ElementInformation
    {
        public string Name { get; set; }
        public int AllRepairCount { get; set; } = 1;
        public int CurrentRepairCount { get; set; } = 0;
        public bool State { get; set; } = true;
        public bool IsRepair { get; set; } = false;
        public double L { get; set; }
        public double M { get; set; }

        public ElementInformation Copy()
        {
            var cloneValue = new ElementInformation();
            cloneValue.Name = this.Name;
            cloneValue.AllRepairCount = this.AllRepairCount;
            cloneValue.CurrentRepairCount = this.CurrentRepairCount;
            cloneValue.State = this.State;
            cloneValue.IsRepair = this.IsRepair;
            cloneValue.L = this.L;
            cloneValue.M = this.M;

            return cloneValue;
        }
    }

    public class StateElement
    {
        public string ModuleName { get; set; }
        public int State { get; set; }
        public int Repair { get; set; }
    }


    public class ModuleCondition
    {
        public bool M1 { get; set; }
        public bool M2 { get; set; }
        public bool M3 { get; set; }
        public bool M4 { get; set; }
        public bool M5 { get; set; }
        public bool M6 { get; set; }
        public bool M7 { get; set; }
        public bool M8 { get; set; }
        public bool M9 { get; set; }
        public bool M10 { get; set; }
        public bool M11 { get; set; }
        public bool M12 { get; set; }
        public bool M13 { get; set; }
        public bool M14 { get; set; }
        public bool M15 { get; set; }
        public bool M16 { get; set; }
        public bool M17 { get; set; }
        public bool M18 { get; set; }
        public bool M19 { get; set; }
        public bool M20 { get; set; }
        public bool M21 { get; set; }

    }
    public class ShortModule
    {
        public string Name { get; set; }
        public int RepairCount { get; set; }
        public double M { get; set; }
        public double L { get; set; }

    }
}
