using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace pickture
{
    public class AppZoomUtils
    {
        public AppZoomUtils(Canvas picker_canvas, Viewbox view_box, ScaleTransform scale_transform)
        {
            this.picker_canvas = picker_canvas;
            this.view_box = view_box;
            this.scale_transform = scale_transform;
        }

        public void OnMouseWheel(MouseWheelEventArgs e, double img_w_px, double img_h_px)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                var delta = e.Delta / 1200d;
                const double min_scale = 1;
                const double max_scale = 10;

                scale_transform.ScaleX += delta * scale_transform.ScaleX;
                scale_transform.ScaleY = scale_transform.ScaleX;

                if (scale_transform.ScaleX < min_scale)
                    scale_transform.ScaleX = scale_transform.ScaleY = min_scale;
                else if (scale_transform.ScaleX > max_scale)
                    scale_transform.ScaleX = scale_transform.ScaleY = max_scale;

                var mouse_pos = e.GetPosition(picker_canvas);
                //var scale_center = new System.Windows.Point(scale_center.X, scale_center.Y);

                var dist = new Vector(mouse_pos.X - scale_center.X, mouse_pos.Y - scale_center.Y);
                var dist_len = dist.Length;
                dist.Normalize();

                var new_center = scale_center + dist * dist_len * 0.5;

                scale_center.X = new_center.X;
                scale_center.Y = new_center.Y;

                scale_transform.CenterX = scale_center.X * view_box.ActualWidth / img_w_px;
                scale_transform.CenterY = scale_center.Y * view_box.ActualHeight / img_h_px;

                //scale_transform.CenterX = 0.5 * canvas_border.ActualWidth;
                //scale_transform.CenterY = 0.5 * canvas_border.ActualHeight;

                picker_canvas.InvalidateVisual();
            }
        }

        public void ResetZoom()
        {
            scale_center = new System.Windows.Point();
            scale_transform.ScaleX = scale_transform.ScaleY = 1;
        }

        public void AdjustSize(double img_w_px, double img_h_px)
        {
            // adjust scale center
            scale_transform.CenterX = scale_center.X * view_box.ActualWidth / img_w_px;
            scale_transform.CenterY = scale_center.Y * view_box.ActualHeight / img_h_px;
        }

        private Canvas picker_canvas;
        private Viewbox view_box;
        private ScaleTransform scale_transform;
        private System.Windows.Point scale_center;
    }
}
