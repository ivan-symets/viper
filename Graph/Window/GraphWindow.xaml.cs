using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VIPER.Helpers;
using VIPER.Presenters;
using VIPER.StateModels;
using VIPER.System_of_differential_equations;
using System.Windows.Controls.Primitives;
using VIPER.Models;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;
using System.IO;

namespace VIPER.Graph.Window
{
    /// <summary>
    /// Interaction logic for GraphWindow.xaml
    /// </summary>
    public partial class GraphWindow 
    {

        public DockPanel zoomPanel;
     


        public SystemState SystemState { get; set; }



        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
          
            zoomPanel = base.GetTemplateChild("ZoomPanel") as DockPanel;
    
        }

        public GraphWindow()
        {
            try
            {
                InitializeComponent();
                CanvasStates.Clear();

                canvas.Children.Clear();
                canvasGraph.Children.Clear();


            


                if (ConfigurationHelper.ComputeCharts || ConfigurationHelper.ComputeGraph || ConfigurationHelper.ComputeEquations)
                {
                    System.Diagnostics.Stopwatch myStopwatch = new System.Diagnostics.Stopwatch();
                    myStopwatch.Start();

                    var elementInf = new List<ElementInformation>();

                    foreach (var elm in ConfigurationHelper.Modules.DistinctBy(b => b.Name))
                    {
                        elementInf.Add(new ElementInformation() { M = elm.M, L = elm.L, Name = elm.Name, AllRepairCount = ConfigurationHelper.UseRepairModules ? elm.RepairCount : 0 });
                    }

                    var systemState = new SystemState(elementInf, ConfigurationHelper.WorkCondition, ConfigurationHelper.Modules.Count);
                    systemState.GetDistinctSystemStates(elementInf);

                    dateGrid.ItemsSource = systemState.SystemStates;

                    SystemState = systemState;
                    myStopwatch.Stop();
                    var ms = myStopwatch.Elapsed;
                    Presenters.Path.logFormElements.Add($"Get system states {ms} ms.");
                }
                if (ConfigurationHelper.ComputeGraph)
                {
                    System.Diagnostics.Stopwatch myStopwatch = new System.Diagnostics.Stopwatch();
                    myStopwatch.Start();
                    ShowGraph();
                    myStopwatch.Stop();
                    var ms = myStopwatch.Elapsed;
                    Presenters.Path.logFormElements.Add($"Get graph {ms} ms.");
                }


                if (ConfigurationHelper.ComputeEquations)
                {
                    System.Diagnostics.Stopwatch myStopwatch = new System.Diagnostics.Stopwatch();
                    myStopwatch.Start();
                    var result = GetEquations();
                    dateGridQ.ItemsSource = result;
                    myStopwatch.Stop();
                    var ms = myStopwatch.Elapsed;
                    Presenters.Path.logFormElements.Add($"Get sysytem equations {ms} ms.");
                }

                if (ConfigurationHelper.ComputeCharts)
                {
               


                    System.Diagnostics.Stopwatch myStopwatch = new System.Diagnostics.Stopwatch();
                    myStopwatch.Start();
                  

                    List<double> workValues = new List<double>();
                    List<double> downTimeValues = new List<double>();
                    List<double> errorTimeValues = new List<double>();
                    List<double> timeList = new List<double>();
                  

                        baseValues[0] = 1;

                        System.Diagnostics.Stopwatch myStopwatch1 = new System.Diagnostics.Stopwatch();
                        myStopwatch1.Start();

                          //   var result1 = ConfigurationHelper.MatLabContext.ComputeDifferentialEquations(equations, baseValues, ConfigurationHelper.Step, 0, (int)ConfigurationHelper.MaxTime);

                        //var aaa = $"F=@(t,p) [{String.Join("; ", equations).Replace(",", ".")}]; [t p]=ode45(F,[{1} : {1} : {1}], [{String.Join(" ", baseValues)}]);";
                        myStopwatch1.Stop();



                        System.Diagnostics.Stopwatch myStopwatch2 = new System.Diagnostics.Stopwatch();
                        myStopwatch2.Start();

                        var result1 = new List<List<double>>();

                        //var result1 = ConfigurationHelper.MatLabContext.ComputeDifferentialEquations(equations, baseValues, ConfigurationHelper.Step, 0, (int)ConfigurationHelper.MaxTime);

                        var result12 = RungeKuttaMethod.RungeKuttaMethods(equations.Count, (int)ConfigurationHelper.Step, (int)ConfigurationHelper.MaxTime, equationCoefficients, baseValues.Select(s => (double)s).ToArray());

                        var baseValuesq = baseValues.Select(s => (double)s).ToList();

                        result1.Add(baseValuesq);
                        result1.AddRange(result12);
                        myStopwatch2.Stop();




                        double startTime = 0;


                        // Add columns
                        var gridView = new GridView();
                        this.listView.View = gridView;
                        gridView.Columns.Add(new GridViewColumn
                        {
                            Header = $"{Properties.Resources.Time}",
                            DisplayMemberBinding = new Binding($"{Properties.Resources.Time}")
                        });

             
                       for(var i=0;i<= SystemState.SystemStates.Count;i++)
                        {
                            gridView.Columns.Add(new GridViewColumn
                            {
                                Header = $"S{i+1}",
                                DisplayMemberBinding = new Binding($"S{i+1}")
                            });


                        }

                        gridView.Columns.Add(new GridViewColumn
                        {
                            Header = $"{Properties.Resources.SumWorkingStates}",
                            DisplayMemberBinding = new Binding($"Working")
                        });

                        gridView.Columns.Add(new GridViewColumn
                        {
                            Header = $"{Properties.Resources.SumDownTiimeStates}",
                            DisplayMemberBinding = new Binding($"Downtime")
                        });

                        gridView.Columns.Add(new GridViewColumn
                        {
                            Header = $"{Properties.Resources.SumRefusalStates}",
                            DisplayMemberBinding = new Binding($"Refusal")
                        });


                    var workDictionary = new Dictionary<double, double>();
                    var downTimeDictionary = new Dictionary<double, double>();
                    var refusalDictionary = new Dictionary<double, double>();

                    double indexCount = 0;
                        foreach (var item in result1)
                        {
                            var ListViewObject = new System.Dynamic.ExpandoObject() as IDictionary<string, Object>;

                            timeList.Add(startTime);
                            double workValue = 0;
                            double downTimeValue = 0;

                            for (var index = 0; index < item.Count; index++)
                            {
                                if (SystemState.WorkStateIndexList.Contains(index))
                                    workValue += item[index];
                                if (SystemState.DownTimeStateIndexList.Contains(index))
                                    downTimeValue += item[index];

                                ListViewObject.Add($"S{index + 1}", Math.Round(item[index], 15));
                            }
                            workValues.Add(workValue);
                            downTimeValues.Add(downTimeValue);
                            var sum = downTimeValue + workValue;
                            errorTimeValues.Add(1 - sum);
                            workDictionary.Add(indexCount, workValue);
                            downTimeDictionary.Add(indexCount, downTimeValue);
                        refusalDictionary.Add(indexCount, 1- sum);
                        indexCount += ConfigurationHelper.Step;



                            ListViewObject.Add($"{Properties.Resources.Time}", startTime);

                            ListViewObject.Add("Working", Math.Round(workValue, 15));
                             ListViewObject.Add("Downtime", Math.Round(downTimeValue, 15));
                            ListViewObject.Add("Refusal", Math.Round(1 - sum, 15));



                            // Populate list
                            this.listView.Items.Add(ListViewObject);



                            startTime += ConfigurationHelper.Step;
                        }
                    



                   var RectangleMethodWork = Integral.RectangleMethod(workDictionary, 0, ConfigurationHelper.MaxTime, (int)(ConfigurationHelper.MaxTime/ ConfigurationHelper.Step));
                   var TrapeziumMethodWork = Integral.TrapeziumMethod(workDictionary, 0, ConfigurationHelper.MaxTime, (int)(ConfigurationHelper.MaxTime / ConfigurationHelper.Step));

                   // this.T_work.Content = $"{Properties.Resources.AverageWorkTime} = {Math.Round(TrapeziumMethodWork,6)}";


                    var RectangleMethodDownTime = Integral.RectangleMethod(downTimeDictionary, 0, ConfigurationHelper.MaxTime, (int)(ConfigurationHelper.MaxTime / ConfigurationHelper.Step));
                    var TrapeziumMethodDownTime  = Integral.TrapeziumMethod(downTimeDictionary, 0, ConfigurationHelper.MaxTime, (int)(ConfigurationHelper.MaxTime / ConfigurationHelper.Step));

                   // this.T_down.Content = $"{Properties.Resources.AverageDowntimeTime} = {Math.Round(TrapeziumMethodDownTime, 6)}";


                    var RectangleMethodRefusal = Integral.RectangleMethod(refusalDictionary, 0, ConfigurationHelper.MaxTime, (int)(ConfigurationHelper.MaxTime / ConfigurationHelper.Step));
                    var TrapeziumMethodRefusal = Integral.TrapeziumMethod(refusalDictionary, 0, ConfigurationHelper.MaxTime, (int)(ConfigurationHelper.MaxTime / ConfigurationHelper.Step));
                  //  this.T_error.Content = $"{Properties.Resources.AverageRefusalTime} = {Math.Round(TrapeziumMethodRefusal, 6)}";

                    var yDataSource = new EnumerableDataSource<double>(workValues);

                    var suumm = RectangleMethodWork + RectangleMethodDownTime + RectangleMethodRefusal;
                    var suumm1 = TrapeziumMethodWork + TrapeziumMethodDownTime + TrapeziumMethodRefusal;


                    yDataSource.SetYMapping(Y => Y);

                    var xDataSource = new EnumerableDataSource<double>(timeList);
                    xDataSource.SetXMapping(X => X);

                    CompositeDataSource compositeDataSource = new CompositeDataSource(xDataSource, yDataSource);

                 //   plotterWork.AddHandler(CircleElementPointMarker.ToolTipTextProperty, s => String.Format("Y-Data : {0}\nX-Data : {1}", s.Y, s.X));
                    plotterWork.Children.RemoveAll(typeof(LineGraph));
                    plotterWork.AddLineGraph(compositeDataSource, new Pen(Brushes.Green, 2),  new PenDescription("Work Line"));
                    plotterWork.FitToView();


                    yDataSource = new EnumerableDataSource<double>(downTimeValues);
                    yDataSource.SetYMapping(Y => Y);

                    xDataSource = new EnumerableDataSource<double>(timeList);
                    xDataSource.SetXMapping(X => X);

                    compositeDataSource = new CompositeDataSource(xDataSource, yDataSource);

                    plotterDownTime.Children.RemoveAll(typeof(LineGraph));
                    plotterDownTime.AddLineGraph(compositeDataSource, new Pen(Brushes.Orange, 2), new PenDescription("Down Time Line"));
                    plotterDownTime.FitToView();

                    yDataSource = new EnumerableDataSource<double>(errorTimeValues);
                    yDataSource.SetYMapping(Y => Y);

                    xDataSource = new EnumerableDataSource<double>(timeList);
                    xDataSource.SetXMapping(X => X);

                    compositeDataSource = new CompositeDataSource(xDataSource, yDataSource);

                   // plotterErrorTime.Children.RemoveAll(typeof(LineGraph));
                   // plotterErrorTime.AddLineGraph(compositeDataSource, new Pen(Brushes.Red, 2), new PenDescription("Error Time Line"));
                   // plotterErrorTime.FitToView();

                    myStopwatch.Stop();
                    var ms = myStopwatch.Elapsed;
                    Presenters.Path.logFormElements.Add($"Get charts of reliability time ex. {ms} ms.");
                }
              

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
            }
        }

        private void dateGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex()+1).ToString();
        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var point = e.GetPosition(this.canvas);
        }

        private void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void dateGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            State currentState = dateGrid.SelectedItem as State;

            SetDetailsGraph(currentState);

        }

        public int RectangleBaseWidth { get; set; } = 35;
        public int RectangleHeight { get; set; } = 30;

        public class CanvasState
        {
            public double LeftPoint { get; set; }
            public double TopPoint { get; set; }
            public int StateId { get; set; }
            public string StateString { get; set; }
        }

        public List<CanvasState> CanvasStates = new List<CanvasState>();


        public Dictionary<StateType, SolidColorBrush> colorsStateType = new Dictionary<StateType, SolidColorBrush>() { { StateType.WorkingState, Brushes.LightGreen },
            { StateType.DowntimeState, Brushes.Yellow},
            { StateType.RefusalState, Brushes.OrangeRed}

        };

        public Dictionary<StateType, SolidColorBrush> colorsLine = new Dictionary<StateType, SolidColorBrush>() { { StateType.WorkingState, Brushes.DarkGreen },
            { StateType.DowntimeState, Brushes.Yellow},
            { StateType.RefusalState, Brushes.OrangeRed},

        };

        public void SetDetailsGraph(State currentState)
        {
            canvas.Children.Clear();

           var baseStates = SystemState.GetUpStates(currentState);

            var childStates = SystemState.GetChildStates(currentState);

            var RectangleWidth = RectangleBaseWidth * currentState.StateElements.Count();

            var maxStateLevel=Math.Max(baseStates.Count, childStates.Count);

            var baseLeft = ((maxStateLevel * (RectangleWidth + 25))/2.0) + 200;


            var grid = new Grid();
            grid.Children.Add(new Rectangle() { Stroke = Brushes.LightGray, Fill = colorsStateType[currentState.StateType], Width = RectangleWidth, Height = RectangleHeight });
            grid.Children.Add(GetTextBlock(currentState));

            var baseTop = 180;

            Canvas.SetLeft(grid, baseLeft);
            Canvas.SetTop(grid, baseTop);
            canvas.Children.Add(grid);

            var baseSize = Math.Ceiling(baseStates.Count / 2.0);
            var childSize = Math.Ceiling(childStates.Count / 2.0);

            if (currentState.GoToRefusalState)
            {
                var startPoint = new Point() { X = baseLeft + RectangleWidth, Y = baseTop + (RectangleHeight / 5) };
                var endPoint = new Point() { X = (baseLeft + RectangleWidth) + 30, Y = baseTop + (RectangleHeight / 5) };

                canvas.Children.Add(DrawLinkArrow(startPoint, endPoint, colorsLine[StateType.RefusalState]));


                grid = new Grid();


                var textBlock = new TextBlock() { Foreground = Brushes.Black, TextAlignment = TextAlignment.Center };

                foreach (var state in currentState.ModuleRefusals)
                {
                    textBlock.Inlines.Add(new Run() { BaselineAlignment = BaselineAlignment.Superscript, Text = "λ", FontSize = 16 });
                    textBlock.Inlines.Add(new Run() { BaselineAlignment = BaselineAlignment.Center, Text = $"{state + 1}" });
                }

                grid.Children.Add(textBlock);

                Canvas.SetLeft(grid, baseLeft + RectangleWidth + 5);
                Canvas.SetTop(grid, baseTop + (RectangleHeight / 5) + 1);
                canvas.Children.Add(grid);

            }


            foreach (var baseState in baseStates)
            {
                grid = new Grid();
                grid.Children.Add(new Rectangle() { Stroke = Brushes.LightGray, Fill = colorsStateType[baseState.StateType], Width = RectangleWidth, Height = RectangleHeight });
                grid.Children.Add(GetTextBlock(baseState));

                var recLeftPosition = baseLeft - baseSize * (RectangleWidth + 25);
                var recTopPosition = baseTop - 100;

                Canvas.SetLeft(grid, recLeftPosition);
                Canvas.SetTop(grid, recTopPosition);
                canvas.Children.Add(grid);

                var startPoint = new Point() { X = recLeftPosition + RectangleWidth / 2, Y=recTopPosition + RectangleHeight };
                var endPoint = new Point() { X = baseLeft + RectangleWidth / 2, Y = baseTop };

                canvas.Children.Add(DrawLinkArrow(startPoint, endPoint, colorsLine[currentState.StateType]));

                baseSize--; 
            }

            foreach (var childState in childStates)
            {
                grid = new Grid();
                grid.Children.Add(new Rectangle() { Stroke = Brushes.LightGray, Fill = colorsStateType[childState.StateType], Width = RectangleWidth, Height = RectangleHeight });
                grid.Children.Add(GetTextBlock(childState));


                var recLeftPosition = baseLeft - childSize * (RectangleWidth + 25);
                var recTopPosition = baseTop + 100;

                Canvas.SetLeft(grid, recLeftPosition);
                Canvas.SetTop(grid, recTopPosition);
                canvas.Children.Add(grid);

                var startPoint = new Point() { X = baseLeft + RectangleWidth / 2, Y = baseTop + RectangleHeight };
                var endPoint = new Point() { X = recLeftPosition+ RectangleWidth / 2, Y = recTopPosition };

                canvas.Children.Add(DrawLinkArrow(startPoint, endPoint, colorsLine[childState.StateType]));

                childSize--;
            }
        }

        public  Shape DrawLinkArrow(Point p1, Point p2, Brush color)
        {
            GeometryGroup lineGroup = new GeometryGroup();
            double theta = Math.Atan2((p2.Y - p1.Y), (p2.X - p1.X)) * 180 / Math.PI;

            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure();
            Point p = new Point(p1.X + ((p2.X - p1.X) / 1.0), p1.Y + ((p2.Y - p1.Y) / 1.0));
            pathFigure.StartPoint = p;

            Point lpoint = new Point(p.X + 2, p.Y + 6);
            Point rpoint = new Point(p.X - 2, p.Y + 6);
            LineSegment seg1 = new LineSegment();
            seg1.Point = lpoint;
            pathFigure.Segments.Add(seg1);

            LineSegment seg2 = new LineSegment();
            seg2.Point = rpoint;
            pathFigure.Segments.Add(seg2);

            LineSegment seg3 = new LineSegment();
            seg3.Point = p;
            pathFigure.Segments.Add(seg3);

            pathGeometry.Figures.Add(pathFigure);
            RotateTransform transform = new RotateTransform();
            transform.Angle = theta + 90;
            transform.CenterX = p.X;
            transform.CenterY = p.Y;
            pathGeometry.Transform = transform;
            lineGroup.Children.Add(pathGeometry);

            LineGeometry connectorGeometry = new LineGeometry();
            connectorGeometry.StartPoint = p1;
            connectorGeometry.EndPoint = p2;
            lineGroup.Children.Add(connectorGeometry);
            System.Windows.Shapes.Path path = new System.Windows.Shapes.Path();
            path.Data = lineGroup;
            path.StrokeThickness = 1;
            path.Stroke = path.Fill = color;

            return path;
        }

        private void CalculateGraph_Click(object sender, RoutedEventArgs e)
        {

           
        }


        public TextBlock GetTextBlock(State baseState)
        {
            var textBlock = new TextBlock() { Foreground = Brushes.Black, TextAlignment = TextAlignment.Center };

            textBlock.Inlines.Add(new Run() { BaselineAlignment = BaselineAlignment.Superscript, Text = $"{baseState.StateId}", Foreground=Brushes.Red, FontSize = 20 });
            textBlock.Inlines.Add(new Run() { BaselineAlignment = BaselineAlignment.Superscript, Text = $"| {baseState.StateString}", FontSize = 20 });

            return textBlock;
        }

        private void ShowGraph_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ShowGraph()
        {

            var levelGroups= SystemState.SystemStates.GroupBy(g => g.StateLevel).ToList();
            CanvasStates.Clear();

            canvasGraph.Children.Clear();

            foreach (UIElement child in canvasGraph.Children)
            {
                UIElement uiElement = child;
                canvasGraph.Children.Remove(uiElement);
            }

            var stateCount = levelGroups.FirstOrDefault().FirstOrDefault().StateElements.Count;

            var RectangleWidth = RectangleBaseWidth * stateCount;

            var aa = levelGroups.Max(s => s.Count());

            var baseLeft = ((levelGroups.Max(s=>s.Count())* (RectangleWidth + (stateCount * 20)))/2.0)+100;
            var baseTop = 100;




            foreach (var levelGroup in levelGroups)
            {

                var baseSize = Math.Ceiling(levelGroup.Count() / 2.0);
                var childSize = Math.Ceiling(levelGroup.Count() / 2.0);

                foreach (var baseState in levelGroup)
                {
                    var  grid = new Grid();
                    grid.Children.Add(new Rectangle() { Stroke = Brushes.LightGray, Fill = colorsStateType[baseState.StateType], Width = RectangleWidth, Height = RectangleHeight });

                    grid.Children.Add(GetTextBlock(baseState));

                    var recLeftPosition = baseLeft - (baseSize) * (RectangleWidth + (stateCount*20));
                    var recTopPosition = baseTop ;

                    CanvasState canvasState = new CanvasState();
                    canvasState.StateId = baseState.StateId;
                    canvasState.StateString = baseState.StateString;
                    canvasState.TopPoint = recTopPosition;
                    canvasState.LeftPoint = recLeftPosition;

                    CanvasStates.Add(canvasState);

                    Canvas.SetLeft(grid, recLeftPosition);
                    Canvas.SetTop(grid, recTopPosition);
                    canvasGraph.Children.Add(grid);



                       if (baseState.GoToRefusalState)
                       {
                           var startPoint = new Point() { X = recLeftPosition + RectangleWidth, Y = recTopPosition + (RectangleHeight/5) };
                           var endPoint = new Point() { X = (recLeftPosition + RectangleWidth)+30, Y = recTopPosition + (RectangleHeight / 5) };

                           canvasGraph.Children.Add(DrawLinkArrow(startPoint, endPoint, colorsLine[StateType.RefusalState]));


                         grid = new Grid();


                        var textBlock = new TextBlock() { Foreground = Brushes.Black, TextAlignment = TextAlignment.Center };

                        foreach (var state in baseState.ModuleRefusals)
                        {
                            textBlock.Inlines.Add(new Run() { BaselineAlignment = BaselineAlignment.Superscript, Text = "λ", FontSize = 16 });
                            textBlock.Inlines.Add(new Run() { BaselineAlignment = BaselineAlignment.Center, Text = $"{state +1}"});
                        }

                        grid.Children.Add(textBlock);

                        Canvas.SetLeft(grid, recLeftPosition + RectangleWidth+5);
                        Canvas.SetTop(grid, recTopPosition + (RectangleHeight / 5)+1);
                        canvasGraph.Children.Add(grid);

                    }


                    foreach (var baseS in baseState.BaseStates)
                        {
                            var canvasBaseState = CanvasStates.Where(w => baseS.StateId == w.StateId).FirstOrDefault();

                            if (canvasBaseState != null)
                            {
                                var startPoint = new Point() { X = canvasBaseState.LeftPoint + RectangleWidth / 2, Y = canvasBaseState.TopPoint + RectangleHeight };
                                var endPoint = new Point() { X = recLeftPosition + RectangleWidth / 2, Y = recTopPosition };

                                 canvasGraph.Children.Add(DrawLinkArrow(startPoint, endPoint, colorsLine[baseState.StateType]));
                            }

                        }

                    

                   

                    baseSize--;
                }
                baseTop=baseTop + 100;
            }


        }

        public float[,] equationCoefficients;

        public List<string> equations = new List<string>();
        public List<Equation> viewEquations = new List<Equation>();
        public List<int> baseValues = new List<int>();

        public class Equation
        {
            public string Equations { get; set; }
            private StateType stateType;
            public StateType StateType
            {
                get
                {
                    return stateType;
                }

                set
                {
                    stateType = value;

                    if (this.StateType == StateType.WorkingState)
                        StateString = Properties.Resources.WorkingState;
                    else if (this.StateType == StateType.DowntimeState)
                        StateString = Properties.Resources.DowntimeState;
                    else
                        StateString = Properties.Resources.RefusalState;
                }
            }
            public string StateString { get; set; }
          
        }

        public List<Equation> GetEquations()
        {

            var systemState = SystemState;

            var listL = systemState.ListL;
            var listM = systemState.ListM ;

            equationCoefficients = new float[systemState.SystemStates.Count + 1, systemState.SystemStates.Count + 1];

            for(var i=0;i< systemState.SystemStates.Count; i++)
            {
                for (var j = 0; j < systemState.SystemStates.Count; j++)
                {
                    equationCoefficients[i, j] = 0;
                }
            }

            var indexState = 0;

            foreach (var currentState in systemState.SystemStates)
            {
                var equationMatLabBuilder=new StringBuilder();
                var equationViewBuilder = new StringBuilder();

                baseValues.Add(0);
                var baseStates = systemState.GetUpStates(currentState);

                var childStates = systemState.GetChildStates(currentState);

                equationViewBuilder.Append( $"dP{StringHelper.GetSubScript(currentState.StateId)}(t) / dt = ");

                if (childStates.Count > 0 || currentState.ModuleRefusals.Count > 0)
                {
                    equationViewBuilder.Append($" -  ");
                    equationMatLabBuilder.Append(" - ");
                    if (childStates.Count > 0 || currentState.ModuleRefusals.Count > 0)
                    {
                        equationMatLabBuilder.Append(" ( ");
                        equationViewBuilder.Append(" ( ");
                    }

                    var index = 0;

                    float childStateCoff = 0;

                    foreach (var state in childStates)
                    {
                        var stateDifference = currentState.GetDifference(state);

                        equationMatLabBuilder.Append($" {(stateDifference.IsRepair ? listM[stateDifference.RefusalStateElement] : listL[stateDifference.RefusalStateElement])} ");

                        childStateCoff+= stateDifference.IsRepair ? listM[stateDifference.RefusalStateElement] : listL[stateDifference.RefusalStateElement];


                        equationViewBuilder.Append($"{(stateDifference.IsRepair ? "μ" : "λ")}{StringHelper.GetSubScript(stateDifference.RefusalStateElement + 1)}");

                        if (index != childStates.Count - 1)
                        {
                            equationMatLabBuilder.Append($" + ");
                            equationViewBuilder.Append($" + ");
                        }
                        index++;
                    }

                    if ((currentState.ModuleRefusals.Count>0 && childStates.Count > 0))
                    {
                        equationMatLabBuilder.Append($" + ");
                        equationViewBuilder.Append($" + ");
                    }

                    if ((currentState.ModuleRefusals.Count >0))
                    {
                        childStateCoff += currentState.ModuleRefusalsLamda;
                        equationMatLabBuilder.Append($"{currentState.ModuleRefusalsLamda}");
                        equationViewBuilder.Append($"{currentState.ModuleRefusalsString}");
                    }

                    if (childStates.Count > 0 || currentState.ModuleRefusals.Count > 0)
                    {
                        equationMatLabBuilder.Append($" ) ");
                        equationViewBuilder.Append($" ) ");
                    }

                    equationCoefficients[indexState,currentState.StateId - 1] = - childStateCoff;

                    equationMatLabBuilder.Append($" * p({currentState.StateId}) ");
                    equationViewBuilder.Append($"  * P{StringHelper.GetSubScript(currentState.StateId)}(t)");
                }


                if (baseStates.Count > 0)
                {
                    equationMatLabBuilder.Append($" + ");
                    equationViewBuilder.Append($" + ");

                    var index = 0;
                    foreach (var state in baseStates)
                    {
                        if (index != 0)
                        {
                            equationMatLabBuilder.Append($" + ");
                            equationViewBuilder.Append($" + ");
                        }
                        equationMatLabBuilder.Append(" ( ");
                        equationViewBuilder.Append(" ( ");

                        var stateDifference = currentState.GetDifference(state);


                        equationMatLabBuilder.Append($" {(stateDifference.IsRepair ? listM[stateDifference.RefusalStateElement] : listL[stateDifference.RefusalStateElement])} ");

                        equationViewBuilder.Append($"{(stateDifference.IsRepair ? "μ" : "λ")}{StringHelper.GetSubScript(stateDifference.RefusalStateElement + 1)}");

                        float childStateCoff = (stateDifference.IsRepair ? listM[stateDifference.RefusalStateElement] : listL[stateDifference.RefusalStateElement]);

                        equationMatLabBuilder.Append($" ) * p({ state.StateId}) ");


                        equationViewBuilder.Append($" ) * P{StringHelper.GetSubScript(state.StateId)}(t)");

                        equationCoefficients[indexState, state.StateId - 1] = childStateCoff;

                        index++;
                    }

                }

                viewEquations.Add(new Equation() { Equations = equationViewBuilder.ToString(), StateType = currentState.StateType });

                equations.Add(equationMatLabBuilder.ToString());

                indexState++;

            }

            var errorViewEquation = new StringBuilder();
             var errorEquation = new StringBuilder();

            errorViewEquation.Append($"dP{StringHelper.GetSubScript(systemState.SystemStates.Count() + 1)}(t) / dt = ");

            for (int i = 0; i < systemState.CountModule; i++)
            {
                var states = systemState.SystemStates.Where(w => w.ModuleRefusals.Contains(i));

                errorViewEquation.Append($"λ{StringHelper.GetSubScript(i + 1)}");
          


                if (states.Count() > 1)
                    errorViewEquation.Append($"( ");

                var index = 0;
                foreach (var currentState in states)
                {
                    equationCoefficients[systemState.SystemStates.Count, currentState.StateId - 1] = currentState.ModuleRefusalsLamda;

                    errorEquation.Append($" {currentState.ModuleRefusalsLamda} * ");

                    errorEquation.Append($"p({currentState.StateId})"); ;

                    errorViewEquation.Append($"P{StringHelper.GetSubScript(currentState.StateId)}(t)");

                    if ((index != states.Count() - 1))
                    {
                        errorViewEquation.Append($" + ");

                        errorEquation.Append($" + ");
                    }

                    index++;
                }

                if (states.Count() > 1)
                    errorViewEquation.Append($" )");

                if ((i != systemState.CountModule - 1))
                {
                    errorViewEquation.Append($" + ");
                    errorEquation.Append($" + ");
                }

            }

         viewEquations.Add(new Equation() { Equations = errorViewEquation.ToString(), StateType = StateType.RefusalState });

            equations.Add(errorEquation.ToString());
            baseValues.Add(0);

            return viewEquations;

        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Only_Work_State_Checked(object sender, RoutedEventArgs e)
        {
            ConfigurationHelper.OnlyWorkState = !ConfigurationHelper.OnlyWorkState;
        }

        private void Use_Repair_Checked(object sender, RoutedEventArgs e)
        {
            ConfigurationHelper.UseRepairModules = !ConfigurationHelper.UseRepairModules;
        }

        const double ScaleRate = 1.1;

        private void canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            if (e.Delta > 0)
            {
                scaleDetailGraph.ScaleX *= ScaleRate;
                scaleDetailGraph.ScaleY *= ScaleRate;
            }
            else
            {
                scaleDetailGraph.ScaleX /= ScaleRate;
                scaleDetailGraph.ScaleY /= ScaleRate;
            }
        }

        private void canvasGraph_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            if (e.Delta > 0)
            {
                scaleGraph.ScaleX *= ScaleRate;
                scaleGraph.ScaleY *= ScaleRate;
               
            }
            else
            {
                scaleGraph.ScaleX /= ScaleRate;
                scaleGraph.ScaleY /= ScaleRate;
            }
        }

        private void dateGridQ_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void plotterWork_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void plotterWork_MouseMove(object sender, MouseEventArgs e)
        {
          
        }

       
        public static CoordinateTransform GetTransform(PointsGraphBase graph)
        {
            if (!(graph.Plotter is Plotter2D))
                return null;
            var transform = ((Plotter2D)graph.Plotter).Viewport.Transform;
            if (graph.DataTransform != null)
                transform = transform.WithDataTransform(graph.DataTransform);

            return transform;
        }
    }
}
