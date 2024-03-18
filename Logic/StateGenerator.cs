using MoreLinq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using VIPER.Helpers;
using VIPER.Models;
using VIPER.Presenters;
using VIPER.StateModels;

namespace VIPER.StateModels
{


    public class SystemState
    {
        public List<int> WorkStateIndexList = new List<int>();
        public List<int> DownTimeStateIndexList = new List<int>();
        public List<ElementInformation> SystemElements { get; set; } = new List<ElementInformation>();

        public  List<State> SystemStates { get; set; } = new List<State>();

        public SystemState(List<ElementInformation> _systemElements ,string systemWorkCondition, int countModule)
        {
            CountModule = countModule;
            SystemWorkCondition = systemWorkCondition;
            SystemElements = _systemElements;
        }

        public List<float> ListL = new List<float>();
        public List<float> ListM = new List<float>();

  
        public int CountModule { get; set; }
        public string SystemWorkCondition { get; set; }

        public List<State> GetUpStates(State currentState)
        {
            
            return currentState.BaseStates;
        }

        public List<State> GetChildStates(State currentState)
        {

            var childStates = new List<State>();

            foreach (var state in SystemStates)
            {
                if (state.BaseStates.Select(s=>s.StateString).Contains(currentState.StateString))
                    childStates.Add(state);
            }
            return childStates;

        }

        public int CurentSystemStateCount { get; set; } = 1;

        public void GetDistinctSystemStates(List<ElementInformation> _systemElements)
        {

            var newState = new State();

            ListL = _systemElements.Select(s => (float)(s.L)).ToList();
            ListM = _systemElements.Select(s => (float)(s.M)).ToList();


            newState.StateId = this.CurentSystemStateCount;
            newState.StateElements = _systemElements;
            newState.GetStateConditions(SystemWorkCondition,CountModule,0);
            newState.StateLevel = 0;
            //  newState.StateTextView = GetTextBlock(_systemElements);
            WorkStateIndexList.Add(this.CurentSystemStateCount - 1);
                SystemStates.Add(newState);

            List<List<ElementInformation>> text = new List<List<ElementInformation>>() { _systemElements };

            List<State> text1 = new List<State> () { newState };

          
            GetStates(text1);
        
        }

        public void GetStates(List<State> searchStates)
        {
            List<State> nextSearchStateSystemElements = new List<State>();

            foreach (var searchState in searchStates)
            {
                nextSearchStateSystemElements.AddRange(GetBaseStates(searchState));
            }

            nextSearchStateSystemElements = Distinct(nextSearchStateSystemElements);

            if (nextSearchStateSystemElements.Count()!=0)
                GetStates(nextSearchStateSystemElements);
        }


        public List<State> Distinct(List<State> searchStates)
        {

            var newStates = new List<State>();

            var searchStateGroups = searchStates.GroupBy(g => g.StateString);

            foreach (var group in searchStateGroups)
            {
                var newState = new State();
                this.CurentSystemStateCount = this.CurentSystemStateCount + 1;

                newState.StateId = this.CurentSystemStateCount;
                newState.StateLevel = group.Max(m => m.StateLevel);
                newState.BaseState = group.FirstOrDefault().BaseState;

                newState.BaseStates = group.Select(m => m.BaseState).ToList();

                newState.StateElements = group.FirstOrDefault().StateElements; 
                newState.StateType = group.Max(m => m.StateType);
                newState.StateTypeString = group.Max(m => m.StateTypeString);
                newState.StateString = group.Max(m => m.StateString);

                newStates.Add(newState);
            }

            WorkStateIndexList.AddRange(newStates.Where(w=>w.StateType==StateType.WorkingState).Select(s=>s.StateId - 1));
            DownTimeStateIndexList.AddRange(newStates.Where(w => w.StateType == StateType.DowntimeState).Select(s => s.StateId - 1));


            SystemStates.AddRange(newStates);


            return newStates;


    }


        public List<State> GetBaseStates(State systemState)
        {
            var resuly = new List<State> ();


            var baseStateElements = new List<ElementInformation>();

            foreach (var el in systemState.StateElements)
                baseStateElements.Add(el.Copy());

            var index = 0;
            foreach (var element in systemState.StateElements)
            {

                if (element.State && systemState.StateType== StateType.WorkingState)
                {
                        var newStateElements = new List<ElementInformation>();

                        foreach (var el in baseStateElements)
                            newStateElements.Add(el.Copy());


                        newStateElements[index].State = false;

                        var newState = new State();

                        newState.BaseStates.Add(systemState);
                        newState.BaseState = systemState;
                        newState.StateElements = newStateElements;
                        newState.StateLevel = systemState.StateLevel + 1;
                        newState.GetStateConditions(SystemWorkCondition, CountModule,index);

                    if (newState.StateType == StateType.RefusalState)
                    {
                        systemState.GoToRefusalState = true;

                        if (systemState.ModuleRefusals.Count == 0)
                            systemState.ModuleRefusalsString = $"λ{StringHelper.GetSubScript(index + 1)}";
                        else
                            systemState.ModuleRefusalsString += $" + λ{StringHelper.GetSubScript(index + 1)}";

                        systemState.ModuleRefusals.Add(index);

                      

                        systemState.ModuleRefusalsLamda += this.ListL[index];
                    }
                    else
                    {

                        resuly.Add(newState);
                    }
                    
                }

                else if (element.CurrentRepairCount != element.AllRepairCount && !element.State)
                {

                    var newStateElements = new List<ElementInformation>();

                    foreach (var el in baseStateElements)
                        newStateElements.Add(el.Copy());

                    newStateElements[index].CurrentRepairCount += 1;
                    newStateElements[index].State = true;

                    var newState = new State();

                    newState.BaseState = systemState;
                    newState.BaseStates.Add(systemState);
                    newState.StateElements = newStateElements;
                    newState.StateLevel = systemState.StateLevel + 1;
                    newState.GetStateConditions(SystemWorkCondition, CountModule,index);
                    resuly.Add(newState);
                }

                index++;
            }
            return resuly;
        }

      
    
        
    }
}
