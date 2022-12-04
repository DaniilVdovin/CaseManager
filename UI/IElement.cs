using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace CaseManager.UI
{
    internal interface IElement
    {
        List<OpenSpace_Propertis.Property> properties { get; set; }
        bool CanDelite { get; set; }
        bool[] ControlPoints { get; set; }

        void Clear();
    }
}
