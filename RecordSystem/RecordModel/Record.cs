using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CaseManager.RecordSystem.RecordModel
{
    [XmlType("PointRecord")]
    public class PointRecord
    {
        [XmlElement("X")]
        public double X { get; set; }
        [XmlElement("Y")]
        public double Y { get; set; }
    }
    [XmlType("PropertyRecord")]
    public class PropertyRecord
    {
        [XmlElement("Index")]
        public int Index { get; set; }
        [XmlElement("Value")]
        public object Value { get; set; }
    }
    [XmlType("ElementRecord")]
    [XmlInclude(typeof(PropertyRecord)), XmlInclude(typeof(PointRecord))]
    public class ElementRecord
    {
        [XmlElement("Name")]
        public string Name { get; set; }
        [XmlElement("Point")]
        public PointRecord Point { get; set; }
        [XmlElement("Type")]
        public string Type { get; set; }
        [XmlArray("PropertyRecord")]
        [XmlArrayItem("Property")]
        public List<PropertyRecord> Property { get; set; }
        /*
        [XmlElement("Image")]
        public byte[] ImageBuffer
        {
            get
            {
                byte[] imageBuffer = null;

                if (Image != null)
                {
                    using (var stream = new MemoryStream())
                    {
                        var encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(Image));
                        encoder.Save(stream);
                        imageBuffer = stream.ToArray();
                    }
                }

                return imageBuffer;
            }
            set
            {
                if (value == null)
                {
                    Image = null;
                }
                else
                {
                    using (var stream = new MemoryStream(value))
                    {
                        var decoder = BitmapDecoder.Create(stream,
                            BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                        Image = decoder.Frames[0];
                    }
                }
            }
        }*/
    }
    [XmlType("ConstrainRecord")]
    public class ConstrainRecord
    {
        [XmlElement("StartIndex")]
        public int StartIndex { get; set; }
        [XmlElement("EndIndex")]
        public int EndIndex { get; set; }
    }
    //, XmlInclude(typeof(PropertyRecord)), XmlInclude(typeof(PointRecord))
    [XmlType("Record")]
    [XmlInclude(typeof(ElementRecord))] 
    public class Record
    {
        [XmlElement("Name")]
        public string Name { get; set; }
        [XmlElement("Version")]
        public string Version { get; set; }
        [XmlElement("Path")]
        public string Path { get; set; }
        [XmlArray("Elements")]
        [XmlArrayItem("ElementRecord")]
        public List<ElementRecord> elements { get; set; }
        [XmlArray("Constrains")]
        [XmlArrayItem("ConstrainRecord")]
        public List<ConstrainRecord> constrains { get; set; }
    }

}
