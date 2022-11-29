using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaseManager.RecordSystem.RecordModel
{
    internal class PointRecord
    {
        public int X;
        public int Y;
    }
    internal class PropertyRecord
    {
        public int Index;
        public object Value;
    }
    internal class ElementRecord
    {
        public string Name { get; set; }
        public PointRecord Point { get; set; }
        public string Type { get; set; }

    }
}
