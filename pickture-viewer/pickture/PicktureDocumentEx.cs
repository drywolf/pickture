using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace pickture
{
    public static class PicktureDocumentEx
    {
        public static PicktureDocument BuildDocument(RegionManager region_manager, string origin_filename, int img_w_px, int img_h_px)
        {
            var doc = new PicktureDocument(origin_filename, img_w_px, img_h_px);

            foreach (var region in region_manager.Regions)
            {
                var region_rect = RegionManager.GetRegionRect(region);
                doc.Regions.Add(region_rect);
            }

            return doc;
        }

        public static void LoadDocument(RegionManager region_manager, PicktureDocument doc)
        {
            foreach (var region_rect in doc.Regions)
            {
                var new_region = region_manager.AddRegion(region_rect);

                new_region.Width = region_rect.Width;
                new_region.Height = region_rect.Height;

                Canvas.SetLeft(new_region, region_rect.X);
                Canvas.SetTop(new_region, region_rect.Y);
            }
        }
    }
}
