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
            foreach (var doc_rect in doc.Regions)
            {
                var new_region = region_manager.AddRegion(doc_rect);

                new_region.Rect.Width = doc_rect.Width;
                new_region.Rect.Height = doc_rect.Height;

                new_region.Rect.X = doc_rect.X;
                new_region.Rect.Y = doc_rect.Y;
            }
        }
    }
}
