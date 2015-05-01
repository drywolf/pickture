using pickture.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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

            InitWindowCommandBindings();

            var args = Environment.GetCommandLineArgs();

            if (args.Length < 2)
            {
                ExitWithMessage("No image path specified, closing...");
                return;
            }

            var first_time_window = LoadWindowPlacement();

            // Very quick and dirty - but it does the job
            if (Properties.Settings.Default.Maximized)
            {
                WindowState = WindowState.Maximized;
            }

            TryOpenImage(args[1], first_time_window);
        }

        #region Window Commands
        private void InitWindowCommandBindings()
        {
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, OnMaximizeWindow, OnCanResizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, OnMinimizeWindow, OnCanMinimizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, OnRestoreWindow, OnCanResizeWindow));
        }

        private void OnCanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ResizeMode == ResizeMode.CanResize || ResizeMode == ResizeMode.CanResizeWithGrip;
        }

        private void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ResizeMode != ResizeMode.NoResize;
        }

        private void OnCloseWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void OnMaximizeWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        private void OnMinimizeWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void OnRestoreWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }
        #endregion // Window Commands

        #region Window Placement
        private bool LoadWindowPlacement()
        {
            var first_time_window = Properties.Settings.Default.Left < 0 || Properties.Settings.Default.Top < 0 || Properties.Settings.Default.Width < 0 || Properties.Settings.Default.Height < 0;

            Left = Properties.Settings.Default.Left;
            Top = Properties.Settings.Default.Top;

            Width = Properties.Settings.Default.Width;
            Height = Properties.Settings.Default.Height;

            return first_time_window;
        }

        private void SaveWindowPlacement()
        {
            if (WindowState == WindowState.Maximized)
            {
                // Use the RestoreBounds as the current values will be 0, 0 and the size of the screen
                Properties.Settings.Default.Left = RestoreBounds.Left;
                Properties.Settings.Default.Top = RestoreBounds.Top;

                Properties.Settings.Default.Width = RestoreBounds.Width;
                Properties.Settings.Default.Height = RestoreBounds.Height;

                Properties.Settings.Default.Maximized = true;
            }
            else
            {
                Properties.Settings.Default.Left = Left;
                Properties.Settings.Default.Top = Top;

                Properties.Settings.Default.Width = Width;
                Properties.Settings.Default.Height = Height;

                Properties.Settings.Default.Maximized = false;
            }

            Properties.Settings.Default.Save();
        }
        #endregion // Window Placement

        private void TryOpenImage(string path, bool fit_to_screen)
        {
            SaveFrames();

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

            canvas_border.MaxWidth = img_w_px;
            canvas_border.MaxHeight = img_h_px;

            LoadFrames();
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

            var new_frame = AddFrame();

            new_frame.Width = new_frame.Height = 250;

            Canvas.SetLeft(new_frame, pos.X - new_frame.Width * 0.5);
            Canvas.SetTop(new_frame, pos.Y - new_frame.Height * 0.5);
        }

        void frame_CopyToClipboardPixels(object sender, EventArgs e)
        {
            var frame = sender as PickFrame;

            if (frame == null)
                return;

            var frame_item = GetFrameItem(frame);

            var raw_bmp = BitmapUtils.ConvertToBmp(bmp);

            var frame_bmp = BitmapUtils.Copy(raw_bmp, new System.Drawing.Rectangle(frame_item.X, frame_item.Y, frame_item.Width, frame_item.Height));

            var frame_source = BitmapUtils.ConvertToSource(frame_bmp);

            Clipboard.SetImage(frame_source);

            raw_bmp.Dispose();
            frame_bmp.Dispose();
        }

        void frame_CopyToClipboardFile(object sender, SaveImageArgs e)
        {
            var frame = sender as PickFrame;

            if (frame == null)
                return;

            var frame_item = GetFrameItem(frame);

            var raw_bmp = BitmapUtils.ConvertToBmp(bmp);

            var frame_bmp = BitmapUtils.Copy(raw_bmp, new System.Drawing.Rectangle(frame_item.X, frame_item.Y, frame_item.Width, frame_item.Height));

            var temp_dir = System.IO.Path.GetTempPath();
            var extension = BitmapUtils.GetFileExtension(e.Format).ToLower();
            var temp_filename = string.Format("{0}_{1}{2}", System.IO.Path.GetFileNameWithoutExtension(ImageFilename), frame_item.Id, extension);
            var temp_image_path = System.IO.Path.Combine(temp_dir, temp_filename);

            frame_bmp.Save(temp_image_path, e.Format);

            System.Collections.Specialized.StringCollection files = new System.Collections.Specialized.StringCollection();
            files.Add(temp_image_path);

            Clipboard.SetFileDropList(files);

            raw_bmp.Dispose();
            frame_bmp.Dispose();
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.Key != Key.Left && e.Key != Key.Right)
                // NOTE: nothing to do here
                return;

            try
            {
                var image_directory_path = System.IO.Path.GetDirectoryName(image_path);

                var jpgs = Directory.GetFiles(image_directory_path, "*.jpg").ToList();

                if (jpgs.Count < 2)
                    // NOTE: no other images to load
                    return;

                var index = jpgs.IndexOf(image_path);

                var next_index = index;

                if (e.Key == Key.Left)
                    --next_index;

                else if (e.Key == Key.Right)
                    ++next_index;

                if (next_index < 0)
                    next_index = jpgs.Count - 1;

                else if (next_index >= jpgs.Count)
                    next_index = 0;

                var next_image_path = jpgs[next_index];
                TryOpenImage(next_image_path, false);
            }
            catch (Exception ex)
            {
                // TODO: where to log silent errors to ?!?!
            }
        }

        private PickFrameItem GetFrameItem(PickFrame frame)
        {
            var item = frame.FrameItem;

            item.X = (int)Math.Round(Canvas.GetLeft(frame));
            item.Y = (int)Math.Round(Canvas.GetTop(frame));

            item.Width = (int)Math.Round(frame.ActualWidth);
            item.Height = (int)Math.Round(frame.ActualHeight);

            return item;
        }

        private IEnumerable<PickFrame> Frames
        {
            get
            {
                var frames = picker_canvas
                                .Children
                                .Cast<UIElement>()
                                .Where(e => e is PickFrame)
                                .Cast<PickFrame>();

                return frames;
            }
        }

        private PicktureDocument BuildDocument(string origin_filename)
        {
            var doc = new PicktureDocument(origin_filename, img_w_px, img_h_px);

            foreach (var frame in Frames)
            {
                var item = GetFrameItem(frame);
                doc.Frames.Add(item);
            }

            return doc;
        }

        private void LoadDocument(PicktureDocument doc)
        {
            foreach (var frame in doc.Frames)
            {
                var new_frame = AddFrame(frame);

                new_frame.Width = frame.Width;
                new_frame.Height = frame.Height;

                Canvas.SetLeft(new_frame, frame.X);
                Canvas.SetTop(new_frame, frame.Y);
            }
        }

        private PickFrame AddFrame(PickFrameItem frame_item = null)
        {
            if (frame_item == null)
            {
                frame_item = new PickFrameItem();

                var used_ids = Frames.Select(f => f.FrameItem.Id).OrderBy(id => id).ToArray();

                for (var i = 0; i < used_ids.Length; i++)
                {
                    if (used_ids[i] != i)
                    {
                        // eat up free id's from the back
                        var free_id = used_ids[i] - 1;
                        frame_item.Id = free_id;
                        break;
                    }
                }

                // no free id was found, create a new one
                if (frame_item.Id < 0)
                    frame_item.Id = used_ids.Length;
            }

            var new_frame = new PickFrame();
            new_frame.DataContext = frame_item;

            new_frame.CopyToClipboardPixels -= frame_CopyToClipboardPixels;
            new_frame.CopyToClipboardPixels += frame_CopyToClipboardPixels;

            new_frame.CopyToClipboardFile -= frame_CopyToClipboardFile;
            new_frame.CopyToClipboardFile += frame_CopyToClipboardFile;

            picker_canvas.Children.Add(new_frame);

            return new_frame;
        }

        private void RemoveFrame(PickFrame frame)
        {
            frame.CopyToClipboardPixels -= frame_CopyToClipboardPixels;
            frame.CopyToClipboardFile -= frame_CopyToClipboardFile;

            picker_canvas.Children.Remove(frame);
        }

        private void RemoveAllFrames()
        {
            foreach (var frame in Frames)
            {
                frame.CopyToClipboardPixels -= frame_CopyToClipboardPixels;
                frame.CopyToClipboardFile -= frame_CopyToClipboardFile;
            }

            picker_canvas.Children.Clear();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            SaveFrames();

            SaveWindowPlacement();
        }

        private void SaveFrames()
        {
            if (string.IsNullOrEmpty(image_path))
                return;

            var xs = new XmlSerializer(typeof(PicktureDocument));
            var pck_path = System.IO.Path.ChangeExtension(image_path, ".pck");

            try
            {
                // don't save if no frames were placed
                if (picker_canvas.Children.Count == 0)
                {
                    // just delete the pck file if there is one
                    if (File.Exists(pck_path))
                        File.Delete(pck_path);

                    return;
                }

                var doc = BuildDocument(ImageFilename);

                using (var fs = new FileStream(pck_path, FileMode.Create))
                using (var gz = new GZipStream(fs, CompressionLevel.Optimal))
                {
                    xs.Serialize(gz, doc);
                }
            }
            catch (Exception ex)
            {
                // TODO: where to log silent errors to ?!?!
            }
        }

        private void LoadFrames()
        {
            if (string.IsNullOrEmpty(image_path))
                return;

            RemoveAllFrames();

            var xs = new XmlSerializer(typeof(PicktureDocument));
            var pck_path = System.IO.Path.ChangeExtension(image_path, ".pck");

            if (!File.Exists(pck_path))
                return;

            try
            {
                using (var fs = new FileStream(pck_path, FileMode.Open))
                using (var gz = new GZipStream(fs, CompressionMode.Decompress))
                {
                    var doc = xs.Deserialize(gz) as PicktureDocument;
                    LoadDocument(doc);
                }
            }
            catch (Exception ex)
            {
                // TODO: where to log silent errors to ?!?!
            }
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
    }

    [XmlRoot]
    public class PicktureDocument
    {
        public PicktureDocument() { }

        public PicktureDocument(string origin_filename, int origin_width, int origin_height)
        {
            OriginFilename = origin_filename;
            OriginWidth = origin_width;
            OriginHeight = origin_height;
        }

        [XmlAttribute]
        public string OriginFilename;

        [XmlAttribute]
        public int OriginWidth;

        [XmlAttribute]
        public int OriginHeight;

        [XmlArray]
        public List<PickFrameItem> Frames = new List<PickFrameItem>();
    }

    [XmlType]
    public class PickFrameItem
    {
        [XmlAttribute]
        public int X;

        [XmlAttribute]
        public int Y;

        [XmlAttribute]
        public int Width;

        [XmlAttribute]
        public int Height;

        [XmlAttribute]
        public int Id = -1;
    }
}
