using pickture.Utilities.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace pickture
{
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
        public List<RegionRectData> Regions = new List<RegionRectData>();
    }

    [XmlType]
    public class RegionRectData : DependencyObject, IPositionable, INotifyPropertyChanged, IEditorItem
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
          DependencyProperty.Register("IsSelected", typeof(bool),
                                      typeof(RegionRectData),
                                      new FrameworkPropertyMetadata(false));


        [XmlAttribute]
        public double X
        {
            get { return _x; }
            set
            {
                _x = value;
                PropertyChanged.Notify(() => X);
            }
        }
        private double _x;

        [XmlAttribute]
        public double Y
        {
            get { return _y; }
            set
            {
                _y = value;
                PropertyChanged.Notify(() => Y);
            }
        }
        private double _y;

        [XmlAttribute]
        public double Width
        {
            get { return _width; }
            set
            {
                _width = value;
                PropertyChanged.Notify(() => Width);
            }
        }
        private double _width;

        [XmlAttribute]
        public double Height
        {
            get { return _height; }
            set
            {
                _height = value;
                PropertyChanged.Notify(() => Height);
            }
        }
        private double _height;

        [XmlAttribute]
        public int Id = -1;
    }
}
