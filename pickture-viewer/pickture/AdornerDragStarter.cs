using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace pickture
{
    public interface IAdornerDragOperation
    {
        void DragFinished(Rect r);
    }

    public class AdornerDragStarter
    {
        public AdornerDragStarter(Canvas canvas, Func<IAdornerDragOperation> drag_operation, Func<Point?, IAdornerDragOperation, Adorner> create_adorner)
        {
            Canvas = canvas;
            CreateAdorner = create_adorner;
            DragOperation = drag_operation;
        }

        public Canvas Canvas;
        public Func<IAdornerDragOperation> DragOperation;

        public Func<Point?, IAdornerDragOperation, Adorner> CreateAdorner;

        private Point? dragStartPoint = null;

        public void PrepareDrag(Point drag_start_position)
        {
            dragStartPoint = new Point?(drag_start_position);
        }

        public void FinishDrag()
        {
            dragStartPoint = null;
        }

        public bool StartDrag()
        {
            if (dragStartPoint.HasValue)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(Canvas);
                if (adornerLayer != null)
                {
                    var adorner = CreateAdorner(dragStartPoint, DragOperation());
                    if (adorner != null)
                    {
                        adornerLayer.Add(adorner);
                    }
                }

                return true;
            }

            return false;
        }
    }
}
