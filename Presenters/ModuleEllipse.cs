using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VIPER.Presenters
{

    public class ModuleEllipse : Control
    {
        public static readonly DependencyProperty CenterProperty = DependencyProperty.Register("Center", typeof(Point),
           typeof(ModuleEllipse), new PropertyMetadata(new Point(0, 0)));
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool),
          typeof(ModuleEllipse), new PropertyMetadata(false));

        public Point Center
        {
            get { return (Point)this.GetValue(CenterProperty); }
            set
            {
                this.SetValue(CenterProperty, value);
            }
        }
    }
}
