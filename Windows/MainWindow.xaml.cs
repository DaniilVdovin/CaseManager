﻿using CaseManager.RecordSystem;
using CaseManager.UI.AI;
using CaseManager.UI.BPMN;
using CaseManager.Windows;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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

            IOCore.openSpace = Op_Sp;
            IOCore.main = this;

            this.MaxHeight = SystemParameters.WorkArea.Height + SystemParameters.WorkArea.Top + 12;
            this.MaxWidth = SystemParameters.WorkArea.Width + SystemParameters.WorkArea.Left + 12;

            
            status_progress.Visibility= Visibility.Hidden;

            mm_ui_person.Click += (s, e) => Op_Sp.Add_Element(new PersonUI());
            mm_ui_image.Click += (s, e) => Op_Sp.Add_Element(new ImageHolderUI());
            mm_ui_doc.Click += (s, e) => Op_Sp.Add_Element(new Calendar());
            mm_ui_bpmn_rect.Click += (s, e) => Op_Sp.Add_Element(new BPMN_Rect());
            mm_ui_bpmn_ask.Click += (s, e) => Op_Sp.Add_Element(new BPMN_ask());
            mm_ui_ai_block.Click += (s, e) => Op_Sp.Add_Element(new AI_blockUI());
            mm_ui_clear.Click += (s, e) => Op_Sp.ClearAll();
            mm_ui_line.Click += (s, e) => Op_Sp.Add_Constrain();

            mm_ui_imageprocessing.Click += (s, e) => new ImageProcessing().Show();

            Windows_close.Click += (s, e) => Close();
            Windows_min.Click += (s, e) => this.WindowState = (this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Minimized);
            Windows_max.Click += (s, e) => this.WindowState = (this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized);

            mm_ui_home.Click += (s, e) => { 
                project_manager.Visibility = Visibility.Visible;
                mm_ui.IsEnabled = false;
            };
            mm_ui_loadproject.Click += (s, e) => { IOCore.LoadProject(); };
            mm_ui_saveproject.Click += (s, e) => { IOCore.SaveProject(); };
            mm_ui_saveasproject.Click += (s, e) => { IOCore.SaveAaProject(); };

            project_manager.Loaded += (s, e) =>
            {
                mm_ui.IsEnabled = false;
            };

            project_manager.Close += (s, e) =>
            {
                project_manager.Visibility = Visibility.Collapsed;
                mm_ui.IsEnabled = true;
            };
            project_manager.Open += (s,e) =>
            {
                IOCore.LoadProject(() =>{project_manager.Close.Invoke(s, e);});
            };
            project_manager.Create += (s, e) =>
            {
                Op_Sp.ClearAll();
            };
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length == 2)
            {
                if (args[1].Contains(".cmp"))
                    IOCore.Load(args[1], () => {
                        project_manager.Close.Invoke(null, null);
                    });
                else
                    notifManager.Add(3, "Не верный формат файла");
            }
            else
            {
                project_manager.Visibility = Visibility.Visible;
            }
        }
    }
}
