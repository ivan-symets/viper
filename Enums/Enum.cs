using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VIPER
{
    public enum CreativeMode
    {
        None = 0,
        Module = 1,
        Node = 2,
        LineBegin = 3,
        LineEnd = 4,
        Line = 5
    }

    public enum StateType
    {
        WorkingState,
        DowntimeState,
        RefusalState
    }

    public enum LogicOperation
    {
       OR,
       AND
    }

    public enum SchemaElements
    {
        Module,
        Node,
        ConnectionLine
    }
}
