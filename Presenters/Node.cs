using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VIPER.Presenters
{
    internal class Node : DraggingElement
    {
        public static int Counter = 1;

        public static readonly DependencyProperty CenterProperty = DependencyProperty.Register("Center", typeof (Point),
            typeof (Node), new PropertyMetadata(new Point(0, 0)));      

        static Node()
        {
            Counter = 0;
        }

        #region Properties
        private bool _isSelect;

        public bool IsEndNode { get; set; }
        public bool IsStartNode { get; set; }

        public List<ConnectionLine> ConnectionLines
        {
            get;
            set;
        }

        public int Cardinality { get { return ConnectionLines.Count; } }

        public Point Center
        {
            get { 
                return (Point) this.GetValue(CenterProperty); }
            set
            {
                if ((Point)this.GetValue(CenterProperty) != value)
                {
                    this.SetValue(CenterProperty, value);
                   // (this.Parent as Panel).InvalidateArrange();
                }
            }
        }
        public bool IsSelected
        {
            get { return _isSelect; }
            set
            {

                _isSelect = value;
                NotifyPropertyChanged("IsSelected");
            }
        }
        #endregion

        public Node()
        {
            Name = "Node"+Counter++.ToString();
            ConnectionLines = new List<ConnectionLine>();
            Canvas.SetLeft(this, 0);
            Canvas.SetTop(this, 0);
            Panel.SetZIndex(this, 2);
            LayoutUpdated += delegate
                             {
                                 
                                 Center = new Point(
                                     Canvas.GetLeft(this) + this.ActualWidth / 2,
                                     Canvas.GetTop(this) + this.ActualHeight/2
                                     );
                             };
        }
        public override PathInfo GetFormula(ConnectionLine l, PathInfo path)
        {


            if (IsStartNode && ConnectionLines.Count == 0)
                throw new Exception(String.Format(Properties.Resources.ErrorStartNode, Name));


            if (IsEndNode && ConnectionLines.Count == 0)
                throw new Exception(String.Format(Properties.Resources.ErrorEndNode, Name));


            if (ConnectionLines.Count <= 1 && !IsStartNode && !IsEndNode)
                    throw new Exception(String.Format(Properties.Resources.ErrorStartNode, Name));

            var rez = "";
            var pathInfoTable = new List<PathInfo>();

            foreach(var uiElement in Path.AllDeletedObjects)
            {
                if (uiElement.GetType().Name == "ConnectionLine")
                    this.ConnectionLines = this.ConnectionLines.Where(w => w.Name != (uiElement as ConnectionLine).Name).ToList();
            }

            Node endBranch = null;
            if (this.ConnectionLines.Count > 2 && this != path?.EndBranch)
            {
                endBranch = FindEndNode();
            }

            if (path == null || path.Start == null)
            {
                
                foreach (var line in ConnectionLines)
                {
                    if (!Schema.CheckedList.Contains(line))
                    {
                        PathInfo patrhInfo = new PathInfo()
                                             {
                                                 Start = this,
                                                 Parent = this,
                                                 EndBranch = endBranch
                                             };
                        line.GetFormula(this, patrhInfo);
                        pathInfoTable.Add(patrhInfo);
                    }
                }
            }
            else
            {
                path.End = this;
                return path;
            }

            var newTable = new List<PathInfo>();

            for (int i = 0; i < pathInfoTable.Count; i++)
            {
                if (pathInfoTable[i].Formula == "")
                {
                    if (!pathInfoTable[i].End.IsEndNode)
                        pathInfoTable[i] = pathInfoTable[i].End.GetFormula(null, new PathInfo { Parent = this });
                    //pathInfoTable[i].Start.IsStartNode = true;
                }
            }

            string orString = "";

           foreach (PathInfo pathInfo in pathInfoTable)
            {
                if (pathInfo.Formula != "")
                {
                    rez += orString + "(" + pathInfo.Formula + ")";
                    orString = " OR ";
                }
            }

           if (pathInfoTable.Count == 0) 
           {
               return new PathInfo()
                                {
                                    Formula = rez
                                };
           }

            pathInfoTable = new List<PathInfo>()
                            {
                                new PathInfo()
                                {
                                    Start = pathInfoTable[0].Start,
                                    End = pathInfoTable[0].End,
                                    Formula = rez
                                }
                            };

            if (pathInfoTable.Count == 0)
            {
                pathInfoTable.Add(new PathInfo
                {
                    Formula = ""
                }
            );
            }

            if ((pathInfoTable[0].Formula != "") && pathInfoTable[0].End != null && !pathInfoTable[0].End.IsEndNode)
            {
                var end = pathInfoTable[0].End.GetFormula(null, new PathInfo { Parent = this});
                pathInfoTable[0].Formula += end.ToString() != ""&& end.ToString() != "()"? " AND (" + end +")":"";
                pathInfoTable[0].End = end.End;
            }
            return pathInfoTable[0];


        }

        public Node FindEndNode()
        {
            PathFinder pf = new PathFinder(this);
            return pf.Find();
        }
        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.ChangedButton == MouseButton.Right)
            {
                IsSelected = !IsSelected;
            }
        }

    }

    class PathFinder
    {
        private List<Node> PassedNodes { get; set; }
        public static List<ConnectionLine> PasedLines { get; set; }

        public PathFinder ParallelPath;

        public Node CurrenNode { get; set; }

        public Node BeginNode;


        private PathFinder()
        {
            PassedNodes = new List<Node>();
            PasedLines = new List<ConnectionLine>();
        }

        public PathFinder(Node begin)
            : this()
        {
            CurrenNode = begin;
            BeginNode = begin;
            ParallelPath = new PathFinder()
            {
                ParallelPath = this,
                CurrenNode = begin,
                BeginNode = begin
            };
        }



        public Node AddNode(Node node)
        {
            PassedNodes.Add(node);
            bool hasSame = false;
            foreach (var item in PassedNodes)
            {
                hasSame = hasSame || ParallelPath.PassedNodes.Contains(item);
                if (hasSame)
                {
                    return item;
                }
            }
            return null;
        }

        public Node NextStep(ConnectionLine line)
        {
            CurrenNode = line.GetNextNode(CurrenNode, this);
            Node Common = AddNode(CurrenNode);
            if (Common == null) return ParallelPath.Find();
            return Common;
        }

        public Node Find()
        {
            ConnectionLine line = null;
            foreach (var item in CurrenNode.ConnectionLines)
            {
                if (!PasedLines.Contains(item) && !Schema.CheckedList.Contains(item))
                {
                    line = item; break;
                }

            }
            if (line!=null)
            return NextStep(line);
            return null;
        }

    }

    class PathInfo
    {
        public Node Parent { get; set; }
        public Node Start { get; set; }
        public String Formula { get; set; }
        public Node End { get; set;}

        public Node EndBranch { get; set; }
        public bool IsM2MConnection { get; set; }

        public PathInfo()
        {
            Formula = "";
        }

        public override string ToString()
        {
            return Formula;
        }
    }
}