using pickture.Adorners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace pickture
{
    public interface IEditorItem
    {
        bool IsSelected { get; set; }
    }

    public class EditorCanvasAspect
    {
        public EditorCanvasAspect(Canvas canvas, Func<IAdornerDragOperation> drag_operation)
        {
            Canvas = canvas;

            Canvas.MouseDown += OnMouseDown;
            Canvas.MouseMove += OnMouseMove;

            adorner_drag = new AdornerDragStarter(Canvas, drag_operation, (p, op) => new RubberbandAdorner(this, p, op));
        }

        public Canvas Canvas;
        private AdornerDragStarter adorner_drag;

        public void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source == Canvas)
            {
                adorner_drag.PrepareDrag(e.GetPosition(Canvas));
                DeselectAll();
                e.Handled = true;
            }
        }

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                adorner_drag.FinishDrag();
            }

            if (adorner_drag.StartDrag())
                e.Handled = true;
        }

        public void DeselectAll()
        {
            foreach (var item in SelectedItems)
            {
                item.IsSelected = false;
            }
        }

        public IEnumerable<UIElement> SelectedControls
        {
            get
            {
                var selected_controls = from ctrl in Canvas.Children.OfType<FrameworkElement>()
                                        where ctrl.DataContext is IEditorItem
                                        let item = ctrl.DataContext as IEditorItem
                                        where item.IsSelected
                                        select ctrl;

                return selected_controls;
            }
        }

        public IEnumerable<IEditorItem> SelectedItems
        {
            get
            {
                var selected_items = from ctrl in Canvas.Children.OfType<FrameworkElement>()
                                     where ctrl.DataContext is IEditorItem
                                     let item = ctrl.DataContext as IEditorItem
                                     where item.IsSelected
                                     select item;

                return selected_items;
            }
        }
    }
}
