using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace pickture
{
    public class RegionManager
    {
        public RegionManager(Canvas picker_canvas, Func<BitmapImage> get_bmp, Func<string> get_img_filename)
        {
            this.picker_canvas = picker_canvas;
            this.get_bmp = get_bmp;
            this.get_img_filename = get_img_filename;
        }

        public ImageRegion AddRegion(RegionRectData region_item = null)
        {
            if (region_item == null)
            {
                region_item = new RegionRectData();

                var used_ids = Regions.Select(f => f.RegionRect.Id).OrderBy(id => id).ToArray();

                for (var i = 0; i < used_ids.Length; i++)
                {
                    if (used_ids[i] != i)
                    {
                        // eat up free id's from the back
                        var free_id = used_ids[i] - 1;
                        region_item.Id = free_id;
                        break;
                    }
                }

                // no free id was found, create a new one
                if (region_item.Id < 0)
                    region_item.Id = used_ids.Length;
            }

            var new_region = new ImageRegion();
            new_region.DataContext = region_item;

            new_region.CopyToClipboardPixels -= region_CopyToClipboardPixels;
            new_region.CopyToClipboardPixels += region_CopyToClipboardPixels;

            new_region.CopyToClipboardFile -= region_CopyToClipboardFile;
            new_region.CopyToClipboardFile += region_CopyToClipboardFile;

            picker_canvas.Children.Add(new_region);

            return new_region;
        }

        public void RemoveRegion(ImageRegion region)
        {
            region.CopyToClipboardPixels -= region_CopyToClipboardPixels;
            region.CopyToClipboardFile -= region_CopyToClipboardFile;

            picker_canvas.Children.Remove(region);
        }

        public void RemoveAllRegions()
        {
            foreach (var region in Regions)
            {
                region.CopyToClipboardPixels -= region_CopyToClipboardPixels;
                region.CopyToClipboardFile -= region_CopyToClipboardFile;
            }

            picker_canvas.Children.Clear();
        }

        #region Clipboard Events
        void region_CopyToClipboardPixels(object sender, EventArgs e)
        {
            var region = sender as ImageRegion;

            if (region == null)
                return;

            var region_item = GetRegionRect(region);

            var raw_bmp = BitmapUtils.ConvertToBmp(get_bmp());

            var region_bmp = BitmapUtils.Copy(raw_bmp, new System.Drawing.Rectangle(region_item.X, region_item.Y, region_item.Width, region_item.Height));

            var region_source = BitmapUtils.ConvertToSource(region_bmp);

            Clipboard.SetImage(region_source);

            raw_bmp.Dispose();
            region_bmp.Dispose();
        }

        void region_CopyToClipboardFile(object sender, SaveImageArgs e)
        {
            var region = sender as ImageRegion;

            if (region == null)
                return;

            var region_item = GetRegionRect(region);

            var raw_bmp = BitmapUtils.ConvertToBmp(get_bmp());

            var region_bmp = BitmapUtils.Copy(raw_bmp, new System.Drawing.Rectangle(region_item.X, region_item.Y, region_item.Width, region_item.Height));

            var temp_dir = System.IO.Path.GetTempPath();
            var extension = BitmapUtils.GetFileExtension(e.Format).ToLower();
            var temp_filename = string.Format("{0}_{1}{2}", System.IO.Path.GetFileNameWithoutExtension(get_img_filename()), region_item.Id, extension);
            var temp_image_path = System.IO.Path.Combine(temp_dir, temp_filename);

            region_bmp.Save(temp_image_path, e.Format);

            System.Collections.Specialized.StringCollection files = new System.Collections.Specialized.StringCollection();
            files.Add(temp_image_path);

            Clipboard.SetFileDropList(files);

            raw_bmp.Dispose();
            region_bmp.Dispose();
        }
        #endregion // Clipboard Events

        public static RegionRectData GetRegionRect(ImageRegion region)
        {
            var item = region.RegionRect;

            item.X = (int)Math.Round(Canvas.GetLeft(region));
            item.Y = (int)Math.Round(Canvas.GetTop(region));

            item.Width = (int)Math.Round(region.ActualWidth);
            item.Height = (int)Math.Round(region.ActualHeight);

            return item;
        }

        public IEnumerable<ImageRegion> Regions
        {
            get
            {
                var regions = picker_canvas
                                .Children
                                .Cast<UIElement>()
                                .Where(e => e is ImageRegion)
                                .Cast<ImageRegion>();

                return regions;
            }
        }

        private Canvas picker_canvas;
        private Func<BitmapImage> get_bmp;
        private Func<string> get_img_filename;
    }
}
