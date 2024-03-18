using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VIPER.Models;
using VIPER.Presenters;
using VIPER.StateModels;
using VIPER.System_of_differential_equations;

namespace VIPER.Presenters
{

    public static  class ConfigurationHelper{

        public static int CountDragingElement { get; set; } = 1;
        public static string CountDragingElementLine { get; set; }

        public static string WorkCondition { get; set; }

        public static bool OnlyWorkState { get; set; } = false;

        public static bool UseRepairModules { get; set; } = true;

        public static List<ShortModule> Modules { get; set; } = new List<ShortModule>();

        public static int CountGetFormula { get; set; } = 0;

        public static bool ComputeGraph { get; set; } = true;
        public static bool ComputeEquations { get; set; } = true;
        public static bool ComputeCharts { get; set; } = true;

        public static double MaxTime { get; set; }
        public static double Step { get; set; }
        public static string SelectedLenguage { get; set; }
        // public static MatLabEquation MatLabContext { get; set; } = new MatLabEquation();

    }

    class Segment
    {
        public string LastModuleName { get; set; }
        public Node StartNode { get; set;}
        public Node EndNode { get; set; }
        public string SubCondition { get; set; }
        public List<Module> Modules { get; set; } = new List<Module>();
        public bool IsCompletedNodePath { get; set; } = false;

        public int Id { get; set; }=0;
        public bool IsJoin { get; set; } =false;
        public bool IsUnion { get; set; } = false;
    }

     static class Lines
    {
        public static List<LineBuilder> LineBuilders { get; set; } = new List<LineBuilder>();

    }


     class LineBuilder
    {
        public  ConnectionLine Line { get; set; } = new ConnectionLine();
        public  List<Point> Points { get; set; } = new List<Point>();

    }

     class Path
    {
        public static List<Segment> PathNodes { get; set; } = new List<Segment>();

        public static List<Segment> PathNodeBase { get; set; } = new List<Segment>();

        public static List<UIElement> AllDeletedObjects { get; set; } = new List<UIElement>();


        public static List<string> AllReplaceNode { get; set; } = new List<string>();

        public static ObservableCollection<string> logFormElements { get; set; } = new ObservableCollection<string>();

        public static ObservableCollection<string> LogFormElements
        {
            get { return logFormElements; }
        }

        public static string Operation { get; set; } = "";

    }
}
