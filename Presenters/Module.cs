using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using VIPER.Helpers;

namespace VIPER.Presenters
{
    internal class Module : DraggingElement
    {
        public static int Counter;

        public bool _isSelect;
        public int Number { get; set; }
        static Module _selectedModule;
         public static Module SelectedModule
         {
             get
             {
                 return _selectedModule;
             }
             set
             {
                 _selectedModule = value;
                 NotifyStaticPropertyChanged("SelectedModule");
             }
         }
        string type="Type_1";
        double _lambda;
        double _mu;
        int _repairCount;

        public string Type
        {
            get { return type; }
            set
            {
                type = value;
                NotifyPropertyChanged("Type");
            }
        }

        public double Lambda
        {
            get { return _lambda;}
            set
            {
                _lambda = value;
                NotifyPropertyChanged("Lambda");
            }
        }
        public double Mu
        {
            get { return _mu; }
            set
            {
                _mu = value;
                NotifyPropertyChanged("Mu");
            }
        }
        public int RepairCount 
        {
            get { return _repairCount; }
            set
            {
                _repairCount = value;
                NotifyPropertyChanged("RepairCount");
            }
        }

        public bool IsSelected
        {
            get { return _isSelect; }
            set
            {
                _isSelect = value;
                if (IsSelected == true)
                {
                    if (SelectedModule!=null)
                    SelectedModule.IsSelected = false;
                    SelectedModule = this;
                }
                else
                {
                    SelectedModule = null;
                }
                NotifyPropertyChanged("IsSelected");
            }
        }

        public string ModuleName
        {
            get { return _name; }
        }
        
        public static readonly DependencyProperty InputCoordinatesProperty = DependencyProperty.Register("InputCoordinates", typeof(Point),
            typeof(Module), new PropertyMetadata(new Point(0, 0)));
        public static readonly DependencyProperty OutputCoordinatesProperty = DependencyProperty.Register("OutputCoordinates", typeof(Point),
           typeof(Module), new PropertyMetadata(new Point(0, 0)));

        //public static readonly DependencyProperty InputLineProperty = DependencyProperty.Register("InputLine", typeof(ConnectionLine), typeof(Module));
        //public static readonly DependencyProperty OutputLineProperty = DependencyProperty.Register("OutputLine", typeof(ConnectionLine), typeof(Module));

        public ConnectionLine InputLine { get; set; }
        public ConnectionLine OutputLine { get; set; }

        public Point InputCoordinates
        {
            get { return (Point)this.GetValue(InputCoordinatesProperty); }
            set
            {
                this.SetValue(InputCoordinatesProperty, value);
            }
        }
        public Point OutputCoordinates
        {
            get { return (Point)this.GetValue(OutputCoordinatesProperty); }
            set
            {
                this.SetValue(OutputCoordinatesProperty, value);
            }
        }
        
        public DependencyProperty SelectedEllipse
        {
            get;
            set;
        }

        static Module()
        {
            Counter = 0;
        }

        public Module()
        {
            Mu = 0.01;
            Lambda = 0.001 ;
            RepairCount = 1;
            
            Name = "M" + StringHelper.GetSubScript(++Counter);
            Number = Counter;
            Canvas.SetLeft(this, 0);
            Canvas.SetTop(this, 0);
            Panel.SetZIndex(this, 2);
            LayoutUpdated += delegate
                             {
                                 InputCoordinates = new Point(
                                     Canvas.GetLeft(this),
                                     Canvas.GetTop(this) + this.ActualHeight / 2
                                     );

                                 OutputCoordinates = new Point(
                                     Canvas.GetLeft(this) + this.ActualWidth,
                                     Canvas.GetTop(this) + this.ActualHeight / 2
                                     );
                             };
            
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.ChangedButton == MouseButton.Left)
            {
                var d = e.GetPosition(this);
                SelectedEllipse =
                (d.X < this.ActualWidth / 2) ? Module.InputCoordinatesProperty : Module.OutputCoordinatesProperty;
            }

        }

        public override PathInfo GetFormula(ConnectionLine invoker, PathInfo path)
        {
            if (InputLine == null)
                throw new Exception(String.Format(Properties.Resources.ErrorInputModule, Name));

            if (OutputLine == null)
                throw new Exception(String.Format(Properties.Resources.ErrorOutputModule, Name));


            foreach (var uiElement in Path.AllDeletedObjects)
            {
                if (uiElement.GetType().Name == "ConnectionLine")
                {
                    var line = (uiElement as ConnectionLine);

                    if (InputLine.Name == line.Name)
                        InputLine = null;

                    if (OutputLine.Name == line.Name)
                        OutputLine = null;
                }
            }


            bool isAnd = path.IsM2MConnection;
            var rez = Name;
            if (invoker == InputLine)
            {
                rez += OutputLine.GetFormula(this, path);
            }
            if (invoker == OutputLine)
            {
                rez += InputLine.GetFormula(this, path);
            }
            if (isAnd) path.Formula = " AND " + rez; else path.Formula = rez;

            return path;
        }

        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.ChangedButton == MouseButton.Right)
            {
                IsSelected = !IsSelected;
            }
        }

        internal Node GetNextNode(ConnectionLine invoker, PathFinder path)
        {
            if (invoker == InputLine)
            {
                return OutputLine.GetNextNode(this, path);
            }
            if (invoker == OutputLine)
            {
                return InputLine.GetNextNode(this, path);
            }

            return null;
        }

    }
}