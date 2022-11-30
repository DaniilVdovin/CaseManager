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
    }

}
