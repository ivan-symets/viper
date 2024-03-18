using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VIPER.Helpers;
using VIPER.StateModels;

namespace VIPER.Models
{

    public class State
    {
        public int StateId { get; set; }
        public List<int> ModuleRefusals { get; set; } = new List<int>();
        public string ModuleRefusalsString { get; set; }
        public float ModuleRefusalsLamda { get; set; } = 0;
        public bool GoToRefusalState { get; set; }

        public int StateLevel { get; set; }
        public State BaseState { get; set; }

        public List<State> BaseStates { get; set; } = new List<State>();
        public List<State> ChildStates { get; set; } = new List<State>();

        public List<ElementInformation> StateElements { get; set; } = new List<ElementInformation>();
        public StateType StateType { get; set; }
        public string StateTypeString { get; set; }
        public string StateString { get; set; } = String.Empty;

        public State Copy()
        {
            var newState = new State();

            newState.StateElements = this.StateElements;

            return newState;
        }

        public StateDifference GetDifference(State otherState)
        {
            var result = new StateDifference();

            var indexBase = 0;

            for (var i = 0; i < this.StateElements.Count - 1; i++)
            {

                if (this.StateElements[i].State != otherState.StateElements[i].State || this.StateElements[i].CurrentRepairCount != otherState.StateElements[i].CurrentRepairCount)
                {
                    result.RefusalStateElement = i;
                    result.IsRepair = this.StateElements[i].CurrentRepairCount != otherState.StateElements[i].CurrentRepairCount;
                    break;
                }

            }
            foreach (var element in this.StateElements)
            {
                var indexCheck = 0;
                foreach (var elementCheck in otherState.StateElements)
                {

                    if (indexBase == indexCheck && (element.State != elementCheck.State || element.CurrentRepairCount != elementCheck.CurrentRepairCount))
                    {
                        result.RefusalStateElement = indexBase;
                        result.IsRepair = element.CurrentRepairCount != elementCheck.CurrentRepairCount;


                        break;
                    }
                    indexCheck++;

                }
                indexBase++;
            }
            return result;
        }


        public void GetStateConditions(string systemWorkCondition, int countModule, int elementIndex)
        {
            BuildStateString();

            var moduleCondition = new ModuleCondition();

            for (var i = 0; i < countModule; i++)
            {


                var index = i + 1;
                PropertyInfo propertyInfo = moduleCondition.GetType().GetProperty("M" + index);
                propertyInfo.SetValue(moduleCondition, Convert.ChangeType(StateElements[i].State, propertyInfo.PropertyType), null);
            }
            var list = new List<ModuleCondition>();
            list.Add(moduleCondition);



            string whereClause = String.Format("m1 and m2");

            var queryResult = list
                   .Where(StringHelper.GetUpScript(systemWorkCondition)).Any();
            // see if it's a match


            if (queryResult)
            {
                StateType = StateType.WorkingState;
                StateTypeString = Properties.Resources.WorkingState;
            }

            if (!queryResult)
            {
                if (StateElements[elementIndex].State == false && StateElements[elementIndex].AllRepairCount == StateElements[elementIndex].CurrentRepairCount)
                {
                    StateType = StateType.RefusalState;
                    StateTypeString = Properties.Resources.RefusalState;
                }
                else if (StateElements.Any(w => w.AllRepairCount != w.CurrentRepairCount))
                {
                    StateType = StateType.DowntimeState;
                    StateTypeString = Properties.Resources.DowntimeState;
                }
            }




        }

        public void BuildStateString()
        {
            StringBuilder builder = new StringBuilder();

            foreach (var element in StateElements)
            {
                builder.Append((element.State ? $"1" : $"0") + (StringHelper.GetSubScript(element.CurrentRepairCount)));
            }
            this.StateString = builder.ToString();
        }


    }

    public class BaseState
    {
        public State State { get; set; }
        List<ElementInformation> ElementInformations { get; set; }
    }

    public class StateDifference
    {
        public int RefusalStateElement { get; set; }

        public bool IsRepair { get; set; }
    }
    public class StatesForElementResponse
    {
        public List<ElementInformation> SystemElements { get; set; }
        public bool IsNewSystemElements { get; set; } = false;
    }
}
