using pickture.Utilities.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class RegionRectData : IPositionable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlAttribute]
        public int X
        {
            get { return _x; }
            set
            {
                _x = value;
                PropertyChanged.Notify(() => X);
            }
        }
        private int _x;

        [XmlAttribute]
        public int Y
        {
            get { return _y; }
            set
            {
                _y = value;
                PropertyChanged.Notify(() => Y);
            }
        }
        private int _y;

        [XmlAttribute]
        public int Width { get; set; }

        [XmlAttribute]
        public int Height { get; set; }

        [XmlAttribute]
        public int Id = -1;
    }
}
