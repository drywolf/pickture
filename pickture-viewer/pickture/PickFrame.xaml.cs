using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
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

namespace pickture
{
    /// <summary>
    /// Interaction logic for PickFrame.xaml
    /// </summary>
    public partial class PickFrame : UserControl
    {
        public PickFrame()
        {
            InitializeComponent();
        }

        private void CopyToClipboardPixel_Click(object sender, RoutedEventArgs e)
        {
            if (CopyToClipboardPixels != null)
                CopyToClipboardPixels(this, EventArgs.Empty);
        }

        private void CopyToClipboardJPEG_Click(object sender, RoutedEventArgs e)
        {
            if (CopyToClipboardFile != null)
                CopyToClipboardFile(this, new SaveImageArgs(ImageFormat.Jpeg));
        }

        private void CopyToClipboardPNG_Click(object sender, RoutedEventArgs e)
        {
            if (CopyToClipboardFile != null)
                CopyToClipboardFile(this, new SaveImageArgs(ImageFormat.Png));
        }

        public PickFrameItem FrameItem
        {
            get
            {
                var item = DataContext as PickFrameItem;
                return item;
            }
        }

        public event EventHandler CopyToClipboardPixels;
        public event EventHandler<SaveImageArgs> CopyToClipboardFile;
    }

    public class SaveImageArgs
    {
        public SaveImageArgs(ImageFormat format)
        {
            Format = format;
        }

        public ImageFormat Format;
    }
}
