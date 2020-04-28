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

namespace VIPER.Presenters
{
    class LineResizer : DraggingElement
    {
        public static readonly DependencyProperty CenterProperty = DependencyProperty.Register("Center", typeof(Point), typeof(LineResizer), new PropertyMetadata(new Point(10, 10), new PropertyChangedCallback(OnCenterChanges)));

        public Point Center
        {
            get { return (Point) this.GetValue(CenterProperty); }
            set { this.SetValue(CenterProperty, value); }
        }

        public LineResizer()
        {
            Canvas.SetLeft(this,0);
            Canvas.SetTop(this, 0);
            Panel.SetZIndex(this, 3);
            LayoutUpdated += delegate
                             {
                                 Center = new Point(
                                     Canvas.GetLeft(this) + this.ActualHeight/2,
                                     Canvas.GetTop(this) + this.ActualWidth/2
                                     );
                             };
        }
        private static void OnCenterChanges(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var obj = d as LineResizer;
            Canvas.SetLeft(obj, obj.Center.X - obj.Width/2);
            Canvas.SetTop(obj, obj.Center.Y- obj.Height/2);
        }
        public override PathInfo GetFormula(ConnectionLine invoker, PathInfo path)
        {
            throw new NotImplementedException();
        }

    }
}
