using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace VIPER.Presenters
{
    internal abstract class DraggingElement : Thumb, INotifyPropertyChanged
    {

        protected string _name;


        public new string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public DraggingElement()
            : base()
        {
            DragDelta += Thumb_OnDragDelta;
        }

        public abstract PathInfo GetFormula(ConnectionLine invoker, PathInfo path);


        private void Thumb_OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb thumb = e.Source as Thumb;

            Canvas.SetLeft(thumb, Canvas.GetLeft(thumb) + e.HorizontalChange);
            Canvas.SetTop(thumb, Canvas.GetTop(thumb) + e.VerticalChange);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged
                 = delegate { };
        protected static void NotifyStaticPropertyChanged(string propertyName)
        {
            StaticPropertyChanged(null, new PropertyChangedEventArgs(propertyName));
        }
    }
}
