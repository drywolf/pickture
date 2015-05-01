using System;
using System.Collections.Generic;
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
    public class RegionRectData
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
