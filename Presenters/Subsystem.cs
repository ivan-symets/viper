using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace VIPER.Presenters
{


    class Subsystem : Control, INotifyPropertyChanged
    {
        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(CreativeMode), typeof(Subsystem), new PropertyMetadata(CreativeMode.None));

        public Schema schema { get; set; }
        DockPanel zoomPanel;

        public bool isSelectDelete { get; set; } = false;

        public Module SelectedModule
        {
            get
            {
                return Module.SelectedModule;
            }

        }


        public CreativeMode Mode
        {
            get { return (CreativeMode)this.GetValue(ModeProperty); }
            set { this.SetValue(ModeProperty, value); NotifyPropertyChanged("Mode"); }
        }

        private ConnectionLine CreationLine { get; set; }
        
        public Subsystem():base()
        {
        Module.StaticPropertyChanged
            +=delegate{
            NotifyPropertyChanged("SelectedModule");};
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            schema = base.GetTemplateChild("schema") as Schema;
            zoomPanel = base.GetTemplateChild("ZoomPanel") as DockPanel;
        }
        protected override void OnPreviewMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);
            var canvas = schema;


            if (Mode == CreativeMode.Module)
            {
                for(var i = 0; i < ConfigurationHelper.CountDragingElement; i++)
                {
                    var module = new Module();
                    canvas.Children.Add(module);
                    if (Properties.Resources.Horizontal.Equals(ConfigurationHelper.CountDragingElementLine))
                    {
                        var a1 = Mouse.GetPosition(canvas).X + 275 * i;
                        var a2 = Mouse.GetPosition(canvas).Y;

                       Canvas.SetLeft(module, Mouse.GetPosition(canvas).X + 275 * i);
                        Canvas.SetTop(module, Mouse.GetPosition(canvas).Y);
                    }
                    else
                    {
                        Canvas.SetLeft(module, Mouse.GetPosition(canvas).X );
                        Canvas.SetTop(module, Mouse.GetPosition(canvas).Y + 175 * i);
                    }

                    Path.logFormElements.Add($"({DateTime.Now:H:mm:ss}) Added module: {module.Name}.");
                }

                
            }
            if (Mode == CreativeMode.Node)
            {

                for (var i = 0; i < ConfigurationHelper.CountDragingElement; i++)
                {
                    var node = new Node();
                    canvas.Children.Add(node);
                    if (Properties.Resources.Horizontal.Equals(ConfigurationHelper.CountDragingElementLine))
                    {
                        Canvas.SetLeft(node, Mouse.GetPosition(canvas).X + 275 * i);
                        Canvas.SetTop(node, Mouse.GetPosition(canvas).Y);
                    }
                    else
                    {
                        Canvas.SetLeft(node, Mouse.GetPosition(canvas).X);
                        Canvas.SetTop(node, Mouse.GetPosition(canvas).Y + 175 * i);
                    }


                    Path.logFormElements.Add($"({DateTime.Now:H:mm:ss}) Added node: {node.Name}.");
                }
            }
            if (Mode == CreativeMode.LineBegin)
            {

                var line = new ConnectionLine();
                canvas.Children.Add(line);
                if (e.OriginalSource is Node || e.OriginalSource is Module)
                {
                    line.Element1 = e.OriginalSource as DraggingElement;
                    Mode = CreativeMode.LineEnd;
                    CreationLine = line;
                }



                return;
            }
            if (Mode == CreativeMode.LineEnd)
            {
             
                if (e.OriginalSource is Node || e.OriginalSource is Module)
                {
                    Mode = CreativeMode.LineBegin;
                    CreationLine.Element2 = e.OriginalSource as DraggingElement;


                    Path.logFormElements.Add($"({DateTime.Now:H:mm:ss}) Con.: {CreationLine?.Element1.Name} to {CreationLine?.Element2.Name}.");
                }


                        foreach(var builder in Lines.LineBuilders.Where(w => w.Line.Name == CreationLine.Name))
                            builder.Points.Add(Mouse.GetPosition(canvas));
                
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (Mode == CreativeMode.LineEnd)
            {
                Point p = e.GetPosition(schema as IInputElement);
                CreationLine.End = p;
            }
        }


        protected override void OnMouseRightButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);
            Mode = CreativeMode.None;

            if (this.isSelectDelete)
            {
                List<UIElement> removeElements = new List<UIElement>();
                UIElement removeStructureElement=new UIElement();

                foreach (UIElement uiElement in this.schema.Children)
                {
                    if (uiElement.GetType() == typeof(Module))
                    {
                        var module = uiElement as Module;

                        if (module.IsSelected)
                        {
                            Module.Counter = Module.Counter - 1;
                            removeElements.Add(module);
                            if (module.InputLine != null)
                            {
                                removeElements.Add(module.InputLine);
                                RemoveLineConnection(module.InputLine, module);
                            }

                            if (module.OutputLine != null)
                            {
                                removeElements.Add(module.OutputLine);
                                RemoveLineConnection(module.OutputLine, module);
                            }
                            module.IsSelected = false;
                            break;
                        }
                    }
                    else if (uiElement.GetType() == typeof(Node))
                    {
                        var node = uiElement as Node;
                        if (node.IsSelected)
                        {
                            removeElements.Add(node);

                            foreach (var line in node.ConnectionLines)
                            {
                                removeElements.Add(line);
                                RemoveLineConnection(line, node);
                            }

                            node.IsSelected = false;
                            break;
                        }
                    }
                    else if (uiElement.GetType() == typeof(ConnectionLine))
                    {
                        var connectionLine = uiElement as ConnectionLine;

                        if (connectionLine.IsSelected)
                        {
                            removeElements.Add(connectionLine);
                            RemoveLineConnection(connectionLine, connectionLine);
                            connectionLine.IsSelected = false;
                            break;
                        }

                    }
                }

                foreach (UIElement uiElement in removeElements)
                    this.schema.Children.Remove(uiElement);
            }
        }

        public void RemoveLineConnection(ConnectionLine connectionLine, UIElement removeInitElement)
        {
            if (connectionLine.Element1?.GetType() == typeof(Module))
            {
                var element1 = connectionLine.Element1 as Module;

                if (element1.OutputLine == connectionLine)
                    element1.OutputLine = null;
                else if (element1.InputLine == connectionLine)
                    element1.InputLine = null;
            }
            else if (connectionLine.Element1?.GetType() == typeof(Node))
            {

                var element1 = connectionLine.Element1 as Node;

                if((removeInitElement.GetType() != typeof(Node)) || (removeInitElement.GetType() == typeof(Node) && (removeInitElement as Node) != element1))
                {
                element1.ConnectionLines.Remove(connectionLine);
                }
            }

            if (connectionLine.Element2?.GetType() == typeof(Module))
            {
                var element2 = connectionLine.Element2 as Module;

                if (element2.OutputLine == connectionLine)
                    element2.OutputLine = null;
                else if (element2.InputLine == connectionLine)
                    element2.InputLine = null;
            }
            else if (connectionLine.Element2?.GetType() == typeof(Node))
            {
                var element2 = connectionLine.Element2 as Node;

                if ((removeInitElement.GetType() != typeof(Node)) || (removeInitElement.GetType() == typeof(Node) && (removeInitElement as Node) != element2))
                {
                    element2.ConnectionLines.Remove(connectionLine);
                }
            }
        }
        
        public void Clear()
        {
            schema.Clear();
        }

        public string GetFormula()
        {
            return schema.GetFormula();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
