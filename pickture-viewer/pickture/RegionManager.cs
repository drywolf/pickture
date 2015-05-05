using pickture.Utilities.WPF;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
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

        public FrameworkElement AddRegion(RegionRectData region_item = null)
        {
            if (region_item == null)
            {
                region_item = new RegionRectData();

                var used_ids = Regions.Select(f => f.Rect.Id).OrderBy(id => id).ToArray();

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

            // NOTE: deprecated
            MoveThumb mt = new MoveThumb();
            //var ct = new ControlTemplate();
            //ct.VisualTree = new FrameworkElementFactory(typeof(ImageRegion));
            //var tr = new Trigger()
            //{
            //    Property = RegionRectData.IsSelectedProperty,
            //    Value = true
            //};

            ParserContext parserContext = new ParserContext();
            parserContext.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            parserContext.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
            //parserContext.XmlnsDictionary.Add("s", "clr-namespace:pickture");
            parserContext.XamlTypeMapper = new XamlTypeMapper(new string[] { });
            parserContext.XamlTypeMapper.AddMappingProcessingInstruction("s_x", "pickture", "pickture");
            parserContext.XmlnsDictionary.Add("s", "s_x");


            string template =
            @"
            <ControlTemplate TargetType=""{x:Type Control}"">
                <Grid Background=""Transparent"" DataContext=""{Binding RelativeSource={RelativeSource TemplatedParent}, Path=.}"">
                    <!--<s:MoveThumb x:Name=""PART_MoveThumb"" Cursor=""SizeAll"" Background=""Transparent"">
                        <s:MoveThumb.Template>
                            <ControlTemplate TargetType=""{x:Type s:MoveThumb}"">
                                <Rectangle Fill=""Transparent"" />
                            </ControlTemplate>
                        </s:MoveThumb.Template>
                    </s:MoveThumb>-->
                    <ContentPresenter x:Name=""PART_ContentPresenter""
                                        Content=""{TemplateBinding ContentControl.Content}"" />
                    <s:ResizeDecorator x:Name=""PART_DesignerItemDecorator"" ShowDecorator=""True"" />
                </Grid>
            </ControlTemplate>
            ";

            mt.Template = (ControlTemplate)XamlReader.Parse(template, parserContext);

            var st = new Style(typeof(Control));
            st.Setters.Add(new Setter { Property = Canvas.LeftProperty, Value = new Binding("X") });
            st.Setters.Add(new Setter { Property = Canvas.TopProperty, Value = new Binding("Y") });
            mt.Style = st;

            //mt.Template = ct;
            mt.DataContext = region_item;

            mt.Loaded += (s, e) =>
            {
                var sender = s as Control;

                ContentPresenter content = sender.Template.FindName("PART_ContentPresenter", sender) as ContentPresenter;

                var new_region = new ImageRegion();

                content.Content = new_region;
                new_region.DataContext = region_item;

                new_region.CopyToClipboardPixels -= region_CopyToClipboardPixels;
                new_region.CopyToClipboardPixels += region_CopyToClipboardPixels;

                new_region.CopyToClipboardFile -= region_CopyToClipboardFile;
                new_region.CopyToClipboardFile += region_CopyToClipboardFile;

                EditorItemEx.ActivateItem(new_region);
            };

            picker_canvas.Children.Add(mt);

            return mt;
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

            var region_bmp = BitmapUtils.Copy(raw_bmp, new System.Drawing.Rectangle((int)region_item.X, (int)region_item.Y, (int)region_item.Width, (int)region_item.Height));

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

            var region_bmp = BitmapUtils.Copy(raw_bmp, new System.Drawing.Rectangle((int)region_item.X, (int)region_item.Y, (int)region_item.Width, (int)region_item.Height));

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
            var item = region.Rect;

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
                var regions = Find.VisualChildren<ImageRegion>(picker_canvas);
                return regions;
            }
        }

        private Canvas picker_canvas;
        private Func<BitmapImage> get_bmp;
        private Func<string> get_img_filename;
    }
}
