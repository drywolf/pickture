using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace pickture.Adorners
{
    public class RubberbandAdorner : Adorner
    {
        private Point? startPoint, endPoint;
        private Rectangle rubberband;
        private EditorCanvasAspect designerCanvas;
        private VisualCollection visuals;
        private Canvas adornerCanvas;
        private IAdornerDragOperation Operation;

        protected override int VisualChildrenCount
        {
            get
            {
                return this.visuals.Count;
            }
        }

        public RubberbandAdorner(EditorCanvasAspect designerCanvas, Point? dragStartPoint, IAdornerDragOperation operation)
            : base(designerCanvas.Canvas)
        {
            this.designerCanvas = designerCanvas;
            this.startPoint = dragStartPoint;
            Operation = operation;

            this.adornerCanvas = new Canvas();
            this.adornerCanvas.Background = Brushes.Transparent;
            this.visuals = new VisualCollection(this);
            this.visuals.Add(this.adornerCanvas);

            this.rubberband = new Rectangle();
            this.rubberband.Stroke = Brushes.Navy;
            this.rubberband.StrokeThickness = 1;
            this.rubberband.StrokeDashArray = new DoubleCollection(new double[] { 2 });                        
            
            this.adornerCanvas.Children.Add(this.rubberband);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!this.IsMouseCaptured)
                {
                    this.CaptureMouse();
                }

                this.endPoint = e.GetPosition(this);
                this.UpdateRubberband();
                this.UpdateSelection();
                e.Handled = true;
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                this.ReleaseMouseCapture();
            }

            AdornerLayer adornerLayer = this.Parent as AdornerLayer;
            if (adornerLayer != null)
            {
                adornerLayer.Remove(this);

                if (Operation != null)
                {
                    double left = Math.Min(this.startPoint.Value.X, this.endPoint.Value.X);
                    double top = Math.Min(this.startPoint.Value.Y, this.endPoint.Value.Y);

                    double width = Math.Abs(this.startPoint.Value.X - this.endPoint.Value.X);
                    double height = Math.Abs(this.startPoint.Value.Y - this.endPoint.Value.Y);

                    Operation.DragFinished(new Rect(left, top, width, height));
                }
            }
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            this.adornerCanvas.Arrange(new Rect(arrangeBounds));
            return arrangeBounds;
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.visuals[index];
        }

        private void UpdateRubberband()
        {
            double left = Math.Min(this.startPoint.Value.X, this.endPoint.Value.X);
            double top = Math.Min(this.startPoint.Value.Y, this.endPoint.Value.Y);

            double width = Math.Abs(this.startPoint.Value.X - this.endPoint.Value.X);
            double height = Math.Abs(this.startPoint.Value.Y - this.endPoint.Value.Y);

            this.rubberband.Width = width;
            this.rubberband.Height = height;
            Canvas.SetLeft(this.rubberband, left);
            Canvas.SetTop(this.rubberband, top);
        }

        private void UpdateSelection()
        {
            Rect rubberBand = new Rect(this.startPoint.Value, this.endPoint.Value);
            foreach (FrameworkElement control in this.designerCanvas.Canvas.Children)
            {
                Rect itemRect = VisualTreeHelper.GetDescendantBounds(control);
                Rect itemBounds = control.TransformToAncestor(designerCanvas.Canvas).TransformBounds(itemRect);

                var item = control.DataContext as IEditorItem;

                if (item == null)
                    continue;

                if (rubberBand.Contains(itemBounds))
                {
                    item.IsSelected = true;
                }
                else
                {
                    item.IsSelected = false;
                }
            }
        }
    }
}
