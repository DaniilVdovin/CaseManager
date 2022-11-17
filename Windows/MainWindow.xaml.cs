﻿using System;
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

            mm_ui_person.Click += (s, e) => Op_Sp.Add_Element(new PersonUI());

            Windows_close.Click += (s, e) => Close();
            Windows_min.Click   += (s, e) => this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Minimized;
            Windows_max.Click   += (s, e) => this.WindowState = this.WindowState == WindowState.Maximized?WindowState.Normal : WindowState.Maximized;
            Point lm = new Point();
            Window_move.MouseLeftButtonDown += (s, e) =>
            {
                base.OnMouseLeftButtonDown(e);
                this.DragMove();
                this.WindowState = WindowState.Normal;
            };
        }
    }
}
