using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VIPER.System_of_differential_equations
{
    //public class MatLabEquation
    //{
     

    //      MLApp.MLApp matLabConnection { get; set; }
    //      public MatLabEquation()
    //      {
    //         matLabConnection = new MLApp.MLApp();
    //      }
    //      public List<List<double>> ComputeDifferentialEquations(List<string> equations, List<int> baseValues, double step, int minTime, int maxTime)
    //      {

    //          var aaa = $"F=@(t,p) [{String.Join("; ", equations).Replace(",", ".")}]; [t p]=ode45(F,[{minTime} : {step} : {maxTime}], [{String.Join(" ", baseValues)}]);";
    //          var result = matLabConnection.Execute($"F=@(t,p) [{String.Join("; ", equations).Replace(",", ".")}]; [t p]=ode45(F,[{minTime} : {step} : {maxTime}], [{String.Join(" ", baseValues)}]);");
    //          var valueList = matLabConnection.Execute("p");
    //          return ConvertToDouble(valueList, step, maxTime, equations.Count - 1);
    //      }


    //      public List<List<double>> ConvertToDouble(string matLabResult, double step, double maxTime, int stateCount)
    //      {
    //          List<List<double>> result = new List<List<double>>();
    //          List<List<double>> intColumns = new List<List<double>>();
    //          var columns = matLabResult.Replace(".", ",").Split(new string[] { "Columns" }, StringSplitOptions.None).ToList();


    //          var splitColumns = columns.Where(w=>w!= "\np =\n\n  ").Select(s=>s.Split('\n').Where(w => w != "p =" && w != "" && w != "  " && w != " " && !w.Contains("through")).ToList()).ToList();

    //         var baseColumn = splitColumns[0];

    //          for (var i=0;i< baseColumn.Count(); i++)
    //          {
    //              for (var j = 1; j < splitColumns.Count(); j++)
    //              {
    //                  baseColumn[i] += splitColumns[j][i];
    //              }
    //          }


    //          for (double i = 0, index = 0; i < maxTime; i += step, index++)
    //          {
    //              var split = baseColumn[(int)index].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).ToList();

    //              result.Add(split.Select(s => Convert.ToDouble(s.Trim())).ToList());
    //          }

    //          return result;
    //      }
    //}

}
