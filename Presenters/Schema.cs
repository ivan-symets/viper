using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VIPER.Logic;

namespace VIPER.Presenters
{
    class Schema : Canvas
    {
        public static List<ConnectionLine> CheckedList;
        public Node startNode;
        public Node endNode;

        public double ZoomFactor { get; set; }
        static Schema()
        {
            CheckedList = new List<ConnectionLine>();
        }


        public Schema():base()
        {
            ZoomFactor = 1;
            RenderTransformOrigin = new Point(0, 0);
            Background = Brushes.Azure;
            startNode = new Node();
            endNode = new Node();
            startNode.Style = (Style)FindResource("EdgeNodeStartStyle");
            startNode.IsStartNode = true;
            endNode.Style = (Style)FindResource("EdgeNodeEndStyle");
            endNode.IsEndNode = true;
            this.Children.Add(startNode);
            this.Children.Add(endNode);

           


        }

        public void Clear()
        {
            this.Children.Clear();

            Module.Counter = 0;
            Node.Counter = 0;
            ConnectionLine.Counter = 0;
            startNode = new Node();
                endNode = new Node();
                startNode.Style = (Style)FindResource("EdgeNodeStartStyle");
                startNode.IsStartNode = true;
                endNode.Style = (Style)FindResource("EdgeNodeEndStyle");
                endNode.IsEndNode = true;
                this.Children.Add(startNode);
                this.Children.Add(endNode);

            

                Path.logFormElements.Add($"({DateTime.Now:H:mm:ss}) Action: Clear from.");
            

            Path.PathNodes.Clear();
            Lines.LineBuilders.Clear();
            Path.Operation = "";
            Path.AllReplaceNode.Clear();

            Path.logFormElements.Clear();

      
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            this.ArrangeOverride(sizeInfo.NewSize);
        }

         protected override System.Windows.Size ArrangeOverride(System.Windows.Size arrangeSize)
         {
             foreach (UIElement child in InternalChildren)
             {
                 //ДУЖЕ ВЕЛИКЕ НАВАНТАЖЕННЯ!!! ТРЕБА ЗМЕНШИТИ
                 if (child == endNode || child == startNode)
                 {
                     var left = Canvas.GetLeft(child);
                     var top = Canvas.GetTop(child);

                     left = left < 0 ? 0 : left;
                     top = top < 0 ? 0 : top;

                     top = top + child.RenderSize.Height > this.ActualHeight ? top - (top + child.RenderSize.Height - this.ActualHeight) : top;
                     left = left + child.RenderSize.Width > this.ActualWidth ? left - (left + child.RenderSize.Width - this.ActualWidth) : left;

                     if (child == startNode)
                     {
                         left = 0;
                     }

                     if (child == endNode)
                     {
                         left = this.ActualWidth - endNode.ActualWidth;
                     }

                     if (child is ConnectionLine)
                     {
                         var line = child as ConnectionLine;
                         if (line.Start.X < 0) line.Start = new Point(0, line.Start.Y);
                         if (line.End.X < 0) line.End = new Point(0, line.End.Y);
                         if (line.Start.X > this.ActualWidth) line.Start = new Point(this.ActualWidth, line.Start.Y);
                         if (line.End.X > this.ActualWidth) line.End = new Point(this.ActualWidth, line.End.Y);
                         if (line.Start.Y < 0) line.Start = new Point(line.Start.X, 0);
                         if (line.End.Y < 0) line.End = new Point(line.End.X, 0);
                         if (line.Start.Y > this.ActualHeight) line.Start = new Point(line.Start.X, ActualHeight);
                         if (line.End.Y > this.ActualHeight) line.End = new Point(line.End.X, ActualHeight);
                     }

                     Canvas.SetLeft(child, left);
                     Canvas.SetTop(child, top);

                 }
             }
             return base.ArrangeOverride(arrangeSize);

         }

        protected override void OnMouseWheel(System.Windows.Input.MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            double divider = 120;
            if ((e.Delta < 0 && ZoomFactor <= 1) || ZoomFactor < 1) divider = 1200;
            ZoomFactor += e.Delta / divider;
            if (ZoomFactor <= 0.4) ZoomFactor = 0.4;
            if (ZoomFactor >= 1) ZoomFactor = 1;


            Matrix m = new Matrix(ZoomFactor, 0,
                                           0, ZoomFactor,
                                           0, 0);

            MatrixTransform mt = new MatrixTransform(m);
            this.LayoutTransform = mt;
           // foreach (UIElement item in Children)
            //{
              //  if (item == startNode || item == endNode || item is ConnectionLine) continue;
              //  item.LayoutTransform = mt;// new ScaleTransform(ZoomFactor, ZoomFactor);
            //}
        }


        public string GetFormula()
        {
            Schema.CheckedList.Clear();

            var result = startNode.GetFormula(null, null).ToString();

            var generator =new  WorkabilityConditionGenerator(Path.PathNodes);

            return generator.GenerateCondition();
        }

    }
}
