using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaseManager.RecordSystem.RecordModel
{
    [Serializable]
    public class PointRecord
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
    [Serializable]
    public class PropertyRecord
    {
        public int Index { get; set; }
        public int Value { get; set; }
    }
    [Serializable]
    public class ElementRecord
    {
        public string Name { get; set; }
        public PointRecord Point { get; set; }
        public string Type { get; set; }
        public List<PropertyRecord> Property { get; set; }
    }
    [Serializable]
    public class Record
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Path { get; set; }
    }

}
