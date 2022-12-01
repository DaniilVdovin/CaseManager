using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace CaseManager.UI
{
    internal interface IElement
    {
        List<Canvas_Propertis.Property> properties { get; set; }
        bool CanDelite { get; set; }
        void Clear();
    }
}
