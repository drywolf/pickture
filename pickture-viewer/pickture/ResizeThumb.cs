using pickture.Utilities.WPF;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace pickture
{
    public class ResizeThumb : Thumb
    {
        private IEditorItem designerItem;
        private IEditorHost designerCanvas;
        private MoveThumb mt;

        public ResizeThumb()
        {
            DragStarted += new DragStartedEventHandler(this.ResizeThumb_DragStarted);
            DragDelta += new DragDeltaEventHandler(this.ResizeThumb_DragDelta);
        }

        private void ResizeThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            var cp = DataContext as ContentPresenter;
            mt = cp.DataContext as MoveThumb;
            this.designerItem = mt.DataContext as IEditorItem;

            if (this.designerItem != null)
            {
                this.designerCanvas = Find.Parent<IEditorHost>(cp);
            }
        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.designerItem != null && this.designerCanvas != null && this.designerItem.IsSelected)
            {
                double minLeft = double.MaxValue;
                double minTop = double.MaxValue;
                double minDeltaHorizontal = double.MaxValue;
                double minDeltaVertical = double.MaxValue;
                double dragDeltaVertical, dragDeltaHorizontal;

                foreach (IPositionable item in this.designerCanvas.Editor.SelectedItems)
                {
                    minLeft = Math.Min(item.X, minLeft);
                    minTop = Math.Min(item.Y, minTop);

                    minDeltaVertical = Math.Min(minDeltaVertical, item.Height);
                    minDeltaHorizontal = Math.Min(minDeltaHorizontal, item.Width);
                }

                foreach (IPositionable item in this.designerCanvas.Editor.SelectedItems)
                {
                    switch (VerticalAlignment)
                    {
                        case VerticalAlignment.Bottom:
                            dragDeltaVertical = Math.Min(-e.VerticalChange, minDeltaVertical);
                            item.Height = (item.Height - dragDeltaVertical);
                            break;
                        case VerticalAlignment.Top:
                            dragDeltaVertical = Math.Min(Math.Max(-minTop, e.VerticalChange), minDeltaVertical);
                            item.Y = (item.Y + dragDeltaVertical);
                            item.Height = (item.Height - dragDeltaVertical);
                            break;
                    }

                    switch (HorizontalAlignment)
                    {
                        case HorizontalAlignment.Left:
                            dragDeltaHorizontal = Math.Min(Math.Max(-minLeft, e.HorizontalChange), minDeltaHorizontal);
                            item.X = (item.X + dragDeltaHorizontal);
                            item.Width = (item.Width - dragDeltaHorizontal);
                            break;
                        case HorizontalAlignment.Right:
                            dragDeltaHorizontal = Math.Min(-e.HorizontalChange, minDeltaHorizontal);
                            item.Width = (item.Width - dragDeltaHorizontal);
                            break;
                    }
                }

                e.Handled = true;
            }
        }
    }
}
