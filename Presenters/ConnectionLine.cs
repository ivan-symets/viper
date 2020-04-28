using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Windows;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Linq;
using VIPER.Models;

namespace VIPER.Presenters
{
    class ConnectionLine : Thumb, INotifyPropertyChanged
    {
        private PathGeometry _pathGeometry;
        protected string _name;
        private bool _isSelect;

        public static int Counter;

        public static readonly DependencyProperty StartProperty = DependencyProperty.Register("Start", typeof(Point), typeof(ConnectionLine), new PropertyMetadata(new Point(0, 0), new PropertyChangedCallback(OnCoordinateChanges)));
        public static readonly DependencyProperty EndProperty = DependencyProperty.Register("End", typeof(Point), typeof(ConnectionLine), new PropertyMetadata(new Point(0, 0), new PropertyChangedCallback(OnCoordinateChanges)));

        public static readonly DependencyProperty Element1Property = DependencyProperty.Register("Element1", typeof(DraggingElement), typeof(ConnectionLine));
        public static readonly DependencyProperty Element2Property = DependencyProperty.Register("Element2", typeof(DraggingElement), typeof(ConnectionLine));

        static ConnectionLine()
        {
            Counter = 0;
        }

        public ConnectionLine()
        {
            _name = "Line" + Counter++.ToString();
            Panel.SetZIndex(this,1);
            Canvas.SetLeft(this, 0);
            Canvas.SetTop(this, 0);
        }

        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.ChangedButton == MouseButton.Right)
            {
                IsSelected = !IsSelected;
            }
        }

        public new string Name
        {
            get { return _name; }
            set { _name = value; }
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

        public PathGeometry PathGeometry {
            get { return _pathGeometry; }
            set
            {
                _pathGeometry = value;
                NotifyPropertyChanged("PathGeometry");
            }
        }


        public Point Start
        {
            get { return (Point) this.GetValue(StartProperty); }
            set
            {
                this.SetValue(StartProperty, value);
            }
        }
        
        public Point End
        {
            get { return (Point)this.GetValue(EndProperty); }
            set
            {
                this.SetValue(EndProperty, value);
            }
        }

        public DraggingElement Element1 
        {
            get { return (DraggingElement)this.GetValue(Element1Property); }
            set
            {
                this.SetValue(Element1Property, value);
                BindElement(value, ConnectionLine.StartProperty); 
            }
        }
        public DraggingElement Element2
        {
            get { return (DraggingElement)this.GetValue(Element2Property); }
            set
            {
                this.SetValue(Element2Property, value);
                BindElement(value, ConnectionLine.EndProperty);
                    
            }
        }

        private void BindElement(DraggingElement element, DependencyProperty property)
        {
            
            Binding bind = new Binding();
            bind.Source = element;
            if (element is Node)
            {
                bind.Path = new PropertyPath("Center");
                (element as Node).ConnectionLines.Add(this);
            }
            else
            {
                var module = (element as Module);
                if(module.SelectedEllipse == Module.InputCoordinatesProperty)
                {
                    module.InputLine = this;
                }
                else
                {
                    module.OutputLine = this;
                }
                bind.Path = new PropertyPath(module.SelectedEllipse.Name);
            }
            bind.Mode = BindingMode.TwoWay;
            this.SetBinding(property, bind);
        }


        public static List<Point> PathLineCreated { get; set; } = new List<Point>();

        public static List<LineSegment> GetLineSegment()
        {

            List<LineSegment> pathSegmentList = new List<LineSegment>();

            var lastTrackPoint = PathLineCreated[0];

            var removePoints = new List<Point>();

            pathSegmentList.Add(new LineSegment(new Point((lastTrackPoint.X), lastTrackPoint.Y), true));

            for (int i=0;i< PathLineCreated.Count-1;i++)
            {
                var point2 = PathLineCreated[i+1];

                if(GetHeight(lastTrackPoint, point2)> 30)
                {
                   
                    pathSegmentList.Add(new LineSegment(new Point((lastTrackPoint.X), point2.Y), true));

                    lastTrackPoint.Y = point2.Y;
                    removePoints.Add(point2);
                }

                if (GetWidth(lastTrackPoint, point2) > 50)
                {
                    pathSegmentList.Add(new LineSegment(new Point((point2.X), lastTrackPoint.Y), true));
                    lastTrackPoint.X = point2.X;
                    removePoints.Add(point2);
                }


            }
            PathLineCreated = PathLineCreated.Where(w => !removePoints.Contains(w)).ToList();


            return pathSegmentList;
        }

        public static double GetHeight(Point p1,Point p2)
        {
            double height = 0;
            if (Math.Abs(p1.Y) > Math.Abs(p2.Y))
                height = Math.Abs(p1.Y) - Math.Abs(p2.Y);
            else
                height = Math.Abs(p2.Y) - Math.Abs(p1.Y);

            return height;
        }

        public static double GetWidth(Point p1, Point p2)
        {
            double width = 0;
            if (Math.Abs(p1.X) > Math.Abs(p2.X))
                width = Math.Abs(p1.X) - Math.Abs(p2.X);
            else
                width = Math.Abs(p2.X) - Math.Abs(p1.X);

            return width;
        }



        private static void OnCoordinateChanges(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
             var obj = d as ConnectionLine;
        
            var newPoint = new Point(obj.End.X, obj.End.Y);
           // (obj.Parent as Panel).InvalidateArrange();

            List<PathSegment> pathSegmentList = new List<PathSegment>();

            var lLineBuilders = Lines.LineBuilders.Where(w => w.Line == obj).FirstOrDefault();

            var flag = false;
            if(lLineBuilders == null){
                var startPoint = new Point(obj.Start.X, obj.Start.Y);
                var lineBuilder = new LineBuilder();
                lineBuilder.Line = obj;
                lineBuilder.Points.Add(startPoint);
                Lines.LineBuilders.Add(lineBuilder);
                flag = true;
            }
           

            if (lLineBuilders==null || lLineBuilders.Points.Count==1)
              {
                if(!flag)
                  pathSegmentList = new List<PathSegment>
                                           {
                                               new LineSegment(new Point((obj.Start.X + obj.End.X)/2, obj.Start.Y), true),
                                               new LineSegment(new Point((obj.Start.X + obj.End.X)/2, obj.End.Y), true),
                                               new LineSegment(obj.End, true)
                                           };
                else
                    pathSegmentList = new List<PathSegment>
                                           {
                                               new LineSegment(new Point((obj.Start.X), obj.Start.Y), true)
                                           };
            }
              else
              {

                for (int i = 0; i < lLineBuilders.Points.Count - 1; i++)
                {
                    var point1 = lLineBuilders.Points[i];
                    var point2 = lLineBuilders.Points[i+1];

                    pathSegmentList.Add(new LineSegment(new Point((point1.X), point1.Y), true));
                    pathSegmentList.Add(new LineSegment(new Point((point1.X), point2.Y), true));
                    pathSegmentList.Add(new LineSegment(new Point((point2.X), point2.Y), true));
                }

                var lastPoint = lLineBuilders.Points.LastOrDefault();

                if (GetWidth(lastPoint, newPoint) > GetHeight(lastPoint, newPoint))
                    pathSegmentList.Add(new LineSegment(new Point((newPoint.X), lastPoint.Y), true));
                else
                    pathSegmentList.Add(new LineSegment(new Point((lastPoint.X), newPoint.Y), true));

            }

            PathFigure pathFigure = new PathFigure(obj.Start, pathSegmentList,false);
            obj.PathGeometry = new PathGeometry(new List<PathFigure> { pathFigure});
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public string GetFormula(DraggingElement invoker, PathInfo path)
        {
            foreach (var uiElement in Path.AllDeletedObjects)
            {
                if (uiElement.GetType().Name == "Node")
                {
                    var node = (uiElement as Node);

                    if (Element1?.Name == node?.Name)
                        Element1 = null;

                    if (Element2?.Name == node?.Name)
                        Element2 = null;
                }
                else if (uiElement.GetType().Name == "Module")
                {
                    var module = (uiElement as Module);

                    if (Element1?.Name == module?.Name)
                        Element1 = null;

                    if (Element2?.Name == module?.Name)
                        Element2 = null;
                }
            }

            Schema.CheckedList.Add(this);

            if(Element1==null)
                throw new Exception(String.Format(Properties.Resources.ErrorInputLine, Name));
            if (Element2 == null)
                throw new Exception(String.Format(Properties.Resources.ErrorOutputLine, Name));


            if (Element1 is Module && Element2 is Module)
                path.IsM2MConnection = true;
            else
                path.IsM2MConnection = false;


            if (Element1 is Node && Element2 is Node)
            {
                Path.PathNodes.Add(new Segment { StartNode = Element1 as Node, EndNode = Element2 as Node, SubCondition = string.Empty, IsCompletedNodePath = true, Id= Path.PathNodes.Count+1 });
            }

            if (Element1 is Node && Element2 is Module)
            {

           
                var element2 = Element2 as Module;

                if (!ConfigurationHelper.Modules.Select(s => s.Name).Contains(element2.Name))
                    ConfigurationHelper.Modules.Add(new ShortModule() { M = element2.Mu, L = element2.Lambda, Name = element2.Name, RepairCount = element2.RepairCount });

                var nodes = Path.PathNodes.Where(w => w.LastModuleName == element2.Name).ToList();

                if (nodes.Count == 0)
                {
                    var newNodePath = new Segment();
                    newNodePath.StartNode = Element1 as Node;

                    var nextModule = Element2 as Module;
                    newNodePath.LastModuleName = nextModule.Name;
                    newNodePath.Modules.Add(nextModule);
                    newNodePath.SubCondition = nextModule.Name;
                    newNodePath.Id = Path.PathNodes.Count + 1;
                    Path.PathNodes.Add(newNodePath);
                }else
                {
                    foreach (var pathNode in nodes)
                        pathNode.StartNode = Element1 as Node;
                    
                }
            }

            if (path.IsM2MConnection)
            {
                var element1 = Element1 as Module;
                var element2 = Element2 as Module;

                if (!ConfigurationHelper.Modules.Select(s=>s.Name).Contains(element1.Name))
                    ConfigurationHelper.Modules.Add(new ShortModule() { M = element1.Mu, L = element1.Lambda, Name =element1.Name,RepairCount=element1.RepairCount});

                if (!ConfigurationHelper.Modules.Select(s => s.Name).Contains(element2.Name))
                    ConfigurationHelper.Modules.Add(new ShortModule() { M = element2.Mu, L = element2.Lambda, Name = element2.Name, RepairCount = element2.RepairCount });

                foreach (var pathNode in Path.PathNodes.Where(w => w.LastModuleName == element1.Name))
                {
                    var nextModule = Element2 as Module;
                    pathNode.LastModuleName = nextModule.Name;
                    pathNode.Modules.Add(nextModule);
                    pathNode.SubCondition = $"{pathNode.SubCondition} AND {nextModule.Name}";
                }            
            }

            if (Element1 is Module && Element2 is Node)
            {
                var element1 = Element1 as Module;

                if (!ConfigurationHelper.Modules.Select(s => s.Name).Contains(element1.Name))
                    ConfigurationHelper.Modules.Add(new ShortModule() {M=element1.Mu,L=element1.Lambda, Name = element1.Name, RepairCount = element1.RepairCount });

                var nodes = Path.PathNodes.Where(w => w.LastModuleName == element1.Name).ToList(); 


                if (nodes.Count == 0)
                {
                    var newNodePath = new Segment();
                    newNodePath.EndNode = Element2 as Node;

                    var nextModule = Element1 as Module;
                    newNodePath.LastModuleName = nextModule.Name;
                    newNodePath.Modules.Add(nextModule);
                    newNodePath.SubCondition = nextModule.Name;
                    newNodePath.Id = Path.PathNodes.Count + 1;
                    Path.PathNodes.Add(newNodePath);
                }
                else { 

                    foreach (var pathNode in nodes)
                    {
                        var endNode = Element2 as Node;
                        pathNode.LastModuleName = null;
                        pathNode.EndNode = endNode;
                        pathNode.IsCompletedNodePath = true;
                    }
                }
            }




            var rez = "";// +Name + " -";
            if(invoker == Element1)
            {
                rez += (Element2 as DraggingElement).GetFormula(this, path);
            }
            if (invoker == Element2)
            {
                rez += (Element1 as DraggingElement).GetFormula(this, path);
            }
            
            return rez;

        }

        public Node GetNextNode(DraggingElement invoker, PathFinder path)
        {
            PathFinder.PasedLines.Add(this);
            if (invoker == Element1)
            {
                if (Element2 is Node) return Element2 as Node;
                else
                    return (Element2 as Module).GetNextNode(this, path);
            }
            if (invoker == Element2)
            {
                if (Element1 is Node) return Element1 as Node;
                else
                    return (Element1 as Module).GetNextNode(this, path);
            }

            return null;

        }
    }
}
