using pickture.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace pickture
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            zoom_utils = new AppZoomUtils(picker_canvas, view_box, canvas_scale);
            region_manager = new RegionManager(picker_canvas, () => bmp, () => ImageFilename);

            WindowCommands.Initialize(this);

            var args = Environment.GetCommandLineArgs();

            if (args.Length < 2)
            {
                ExitWithMessage("No image path specified, closing...");
                return;
            }

            var first_time_window = WindowPlacement.Load(this);

            TryOpenImage(args[1], first_time_window);

            command_model = new CommandModel();
            command_model.ToggleImage = new ImageToggleController(() => image_path, path => TryOpenImage(path, false));

            command_stack.DataContext = command_model;
        }

        private void TryOpenImage(string path, bool fit_to_screen)
        {
            RegionFileStorage.SaveRegions(region_manager, picker_canvas, image_path, ImageFilename, img_w_px, img_h_px);

            try
            {
                image_path = System.IO.Path.GetFullPath(path);
            }
            catch (Exception e)
            {
                ExitWithMessage("Error while normalizing image path: \r\n" + e.Message);
                return;
            }

            if (System.IO.Path.GetExtension(image_path) == ".pck")
                image_path = System.IO.Path.ChangeExtension(image_path, ".jpg");

            if (!File.Exists(image_path))
            {
                ExitWithMessage("Invalid image path specified, closing...");
                return;
            }
            
            try
            {
                bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(image_path);
                bmp.EndInit();

                image_preview.Source = bmp;
            }
            catch (Exception e)
            {
                ExitWithMessage("Error while opening image file: \r\n" + e.Message);
                return;
            }

            img_w_px = bmp.PixelWidth;
            img_h_px = bmp.PixelHeight;

            var img_w = (double)bmp.PixelWidth;
            var img_h = (double)bmp.PixelHeight;

            var screen_w = SystemParameters.FullPrimaryScreenWidth;
            var screen_h = SystemParameters.FullPrimaryScreenHeight;

            var scale_factor = 0.9d;

            var max_w = screen_w * scale_factor;
            var max_h = screen_h * scale_factor;

            ScaleUtilities.FitInto(ref img_w, ref img_h, max_w, max_h);

            if (fit_to_screen)
            {
                Width = img_w;
                Height = img_h;
            }

            picker_canvas.Width = img_w_px;
            picker_canvas.Height = img_h_px;

            Title = ImageFilename;

            // reset scale
            zoom_utils.ResetZoom();

            root_grid.MaxWidth = img_w_px;
            root_grid.MaxHeight = img_h_px;

            RegionFileStorage.LoadRegions(region_manager, image_path);
        }

        private string ImageFilename
        {
            get
            {
                var image_filename = System.IO.Path.GetFileName(image_path);
                return image_filename;
            }
        }

        private string image_path;
        private int img_w_px, img_h_px;

        private BitmapImage bmp;

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);

            var pos = e.GetPosition(picker_canvas);

            var new_region = region_manager.AddRegion();

            new_region.Rect.Width = new_region.Rect.Height = 250;

            new_region.Rect.X = (int)Math.Round(pos.X - new_region.Rect.Width * 0.5);
            new_region.Rect.Y = (int)Math.Round(pos.Y - new_region.Rect.Height * 0.5);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            
            if (e.Key == Key.Left)
                command_model.ToggleImage.PreviousImage.Execute(null);

            else if (e.Key == Key.Right)
                command_model.ToggleImage.NextImage.Execute(null);

            //try
            //{
            //    var image_directory_path = System.IO.Path.GetDirectoryName(image_path);

            //    var jpgs = Directory.GetFiles(image_directory_path, "*.jpg").ToList();

            //    if (jpgs.Count < 2)
            //        // NOTE: no other images to load
            //        return;

            //    var index = jpgs.IndexOf(image_path);

            //    var next_index = index;

            //    if (e.Key == Key.Left)
            //        --next_index;

            //    else if (e.Key == Key.Right)
            //        ++next_index;

            //    if (next_index < 0)
            //        next_index = jpgs.Count - 1;

            //    else if (next_index >= jpgs.Count)
            //        next_index = 0;

            //    var next_image_path = jpgs[next_index];
            //    TryOpenImage(next_image_path, false);
            //}
            //catch (Exception ex)
            //{
            //    // TODO: where to log silent errors to ?!?!
            //}
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            RegionFileStorage.SaveRegions(region_manager, picker_canvas, image_path, ImageFilename, img_w_px, img_h_px);

            WindowPlacement.Save(this);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            zoom_utils.AdjustSize(img_w_px, img_h_px);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            zoom_utils.OnMouseWheel(e, img_w_px, img_h_px);
        }

        private void ExitWithMessage(string message)
        {
            MessageBox.Show(message);
            Close();
        }

        private AppZoomUtils zoom_utils;
        private RegionManager region_manager;
        private CommandModel command_model;
    }
}
