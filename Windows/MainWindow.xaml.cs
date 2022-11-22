﻿using CaseManager.Windows;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CaseManager
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            this.MaxHeight = SystemParameters.WorkArea.Height + SystemParameters.WorkArea.Top + 12;
            this.MaxWidth = SystemParameters.WorkArea.Width + SystemParameters.WorkArea.Left + 12;

            mm_ui_person.Click += (s, e) => Op_Sp.Add_Element(new PersonUI());
            mm_ui_image.Click += (s, e) => Op_Sp.Add_Element(new ImageHolderUI());
            mm_ui_doc.Click += (s, e) => Op_Sp.Add_Element(new Calendar());
            mm_ui_line.Click += (s, e) => Op_Sp.Add_Constrain();

            mm_ui_imageprocessing.Click += (s, e) => new ImageProcessing().Show();

            Windows_close.Click += (s, e) => Close();
            Windows_min.Click += (s, e) => this.WindowState = (this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Minimized);
            Windows_max.Click += (s, e) => this.WindowState = (this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized);
        }
    }
}
