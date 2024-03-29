﻿using CaseManager.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace CaseManager
{
    /// <summary>
    /// Логика взаимодействия для PersonUI.xaml
    /// </summary>
    public partial class ImageHolderUI : UserControl, IElement
    {
        public bool CanDelite { get; set; }
        public bool[] ControlPoints { get; set; }
        public List<OpenSpace_Propertis.Property> properties { get; set; }
        private readonly ImageSource image_default;
        public ImageHolderUI()
        {
            InitializeComponent();
            image_default = i_image.Source;
            CanDelite = true;
            
            ControlPoints = new bool[4];
            for (int i = 0; i<4; i++) ControlPoints[i] = true;
            properties = new List<OpenSpace_Propertis.Property>
            {
                new OpenSpace_Propertis.Property("Основное","Изображение", "-","File",(v)=>{LoadImage(v.ToString());})
            };
        }
        public void LoadImage(string path)
        {
            if (path != "" && path != "-")
            {
                try
                {
                    i_image.Source = new BitmapImage(new Uri(path));
                }
                catch
                {
                    i_image.Source = image_default;
                }
            }
        }

        public void Clear()
        {
            
        }
    }
}
