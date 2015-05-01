using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pickture.Utilities
{
    public static class ScaleUtilities
    {
        public static void FitInto(ref double src_w, ref double src_h, double target_w, double target_h)
        {
            if (src_w < target_w && src_h < target_h)
            {
                var w_ratio = target_w / src_w;
                var h_ratio = target_h / src_h;

                // the height is the limiting dimension
                if (w_ratio > h_ratio)
                {
                    var scale = target_h / src_h;
                    src_w *= scale;
                    src_h *= scale;
                }
                // the width is the limiting dimension
                else
                {
                    var scale = target_w / src_w;
                    src_w *= scale;
                    src_h *= scale;
                }

                return;
            }

            if (src_w > target_w)
            {
                var scale = target_w / src_w;
                src_w *= scale;
                src_h *= scale;
            }

            if (src_h > target_h)
            {
                var scale = target_h / src_h;
                src_w *= scale;
                src_h *= scale;
            }
        }
    }
}
