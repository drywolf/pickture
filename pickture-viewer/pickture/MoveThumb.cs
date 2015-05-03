using pickture.Utilities.WPF;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace pickture
{
    public interface IPositionable
    {
        int X { get; set; }
        int Y { get; set; }
    }

    public class MoveThumb : Thumb, INotifyPropertyChanged
    {
        public MoveThumb()
        {
            DragDelta += MoveThumb_DragDelta;
            //DragCompleted += MoveThumb_DragCompleted;
            Focusable = true;
        }

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(this);

            //// TODO: add dependency property to set target area framework element
            //if (pos.Y > 32)
            //    return;

            base.OnMouseLeftButtonDown(e);
        }

        // NOTE: this should be an optional feature ?!
        //void MoveThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        //{
        //    var pos = DataContext as IPositionable;

        //    if (pos == null)
        //        return;

        //    var conv = new PositionSnapConverter();

        //    pos.X = (double)conv.Convert(pos.X, null, null, null);
        //    pos.Y = (double)conv.Convert(pos.Y, null, null, null);
        //}

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var pos = DataContext as IPositionable;

            if (pos == null)
                pos = DragTarget as IPositionable;

            // TODO: make this handling more generic / abstract
            if (pos == null)
            {
                var designer_item = DataContext as Control;

                if (designer_item != null)
                {
                    pos = designer_item.DataContext as IPositionable;

                    if (pos != null)
                    {
                        pos.X += (int)e.HorizontalChange;
                        pos.Y += (int)e.VerticalChange;
                        return;
                    }
                }
            }

            // TODO: make this handling more generic / abstract
            if (pos == null)
            {
                var x = Canvas.GetLeft(this);
                var y = Canvas.GetTop(this);

                if (double.IsNaN(x))
                    x = 0;

                if (double.IsNaN(y))
                    y = 0;

                x += e.HorizontalChange;
                y += e.VerticalChange;

                Canvas.SetLeft(this, x);
                Canvas.SetTop(this, y);

                return;
            }

            pos.X += (int)e.HorizontalChange;
            pos.Y += (int)e.VerticalChange;
        }

        //public Type ControlType
        //{
        //    set
        //    {
        //        ControlTemplate template = new ControlTemplate();
        //        var fec = new FrameworkElementFactory(value);
        //        template.VisualTree = fec;
        //        Template = template;
        //    }
        //}

        public static readonly DependencyProperty DragTargetProperty = DependencyProperty.Register("DragTarget", typeof(object), typeof(MoveThumb), new PropertyMetadata(null, (d, e) =>
            ((MoveThumb)d).DragTarget = e.NewValue));
        public object DragTarget
        {
            get { return GetValue(DragTargetProperty); }
            set
            {
                SetValue(DragTargetProperty, value);
                PropertyChanged.Notify(() => DragTarget);
            }
        }

        /// <summary>
        /// INotifyPropertyChanged.PropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class MoveThumb_BAK : Thumb
    {
        //private FrameworkElement designerItem;
        //private IEditorHost editor_host;

        //public MoveThumb_BAK()
        //{
        //    DragStarted += new DragStartedEventHandler(this.MoveThumb_DragStarted);
        //    DragDelta += new DragDeltaEventHandler(this.MoveThumb_DragDelta);
        //}

        //public Type ControlType
        //{
        //    set
        //    {
        //        ControlTemplate template = new ControlTemplate();
        //        var fec = new FrameworkElementFactory(value);
        //        template.VisualTree = fec;
        //        Template = template;
        //    }
        //}

        //private void MoveThumb_DragStarted(object sender, DragStartedEventArgs e)
        //{
        //    this.designerItem = this.DataContext as IEditorItem;

        //    if (this.designerItem != null)
        //    {
        //        this.editor_host = VisualTreeHelper.GetParent(this.designerItem) as IEditorHost;
        //    }
        //}

        //private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        //{
        //    if (this.designerItem != null && this.editor_host != null && this.designerItem.IsSelected)
        //    {
        //        double minLeft = double.MaxValue;
        //        double minTop = double.MaxValue;

        //        foreach (var item in this.editor_host.Editor.SelectedItems)
        //        {
        //            //minLeft = Math.Min(Canvas.GetLeft(item), minLeft);
        //            //minTop = Math.Min(Canvas.GetTop(item), minTop);
        //        }

        //        double deltaHorizontal = Math.Max(-minLeft, e.HorizontalChange);
        //        double deltaVertical = Math.Max(-minTop, e.VerticalChange);

        //        foreach (var item in this.editor_host.Editor.SelectedItems)
        //        {
        //            //Canvas.SetLeft(item, Canvas.GetLeft(item) + deltaHorizontal);
        //            //Canvas.SetTop(item, Canvas.GetTop(item) + deltaVertical);
        //        }

        //        var host_fe = editor_host as FrameworkElement;
        //        host_fe.InvalidateMeasure();
        //        e.Handled = true;
        //    }
        //}
    }
}
