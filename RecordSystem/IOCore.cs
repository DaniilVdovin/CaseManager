using CaseManager.RecordSystem.RecordModel;
using CaseManager.UI;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml.Serialization;
using static CaseManager.Canvas_Object_Manager;

namespace CaseManager.RecordSystem
{
    public static class IOCore
    {
        public static OpenSpace OpenSpace;
        
        public static string CurrentProjectPath = "/../";
        public static string CurrentProjectName = "CaseManagerProject";
        internal static Type[] personTypes = { typeof(Record), typeof(ElementRecord), typeof(PointRecord), typeof(PropertyRecord) };
        internal static XmlSerializer xmlSerializer = new XmlSerializer(typeof(Record), personTypes);
        public static void SaveProject() => Save(CurrentProjectPath + "/" + CurrentProjectName);
        public static void SaveAaProject()
        {
            SaveFileDialog openFileDialog = new SaveFileDialog()
            {
                FileName = IOCore.CurrentProjectName,
                DefaultExt = ".cmp"
            };
            openFileDialog.FileOk += (b, i) =>
            {
                if (openFileDialog.FileName != "")
                {
                    IOCore.Save(openFileDialog.FileName);
                }
            };
            openFileDialog.ShowDialog();
        }
        public static void LoadProject() => LoadProject(()=>{});
        public static void LoadProject(Action complite)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "CM Project (*.cmp)|*.cmp"
            };
            openFileDialog.FileOk += (b, i) =>
            {
                if (openFileDialog.FileName != "")
                {
                    IOCore.Load(openFileDialog.FileName, complite);
                }
            };
            openFileDialog.ShowDialog();
        }
        internal static List<ConstrainRecord> GetConstrains()
        {
            List<ConstrainRecord> elements = new List<ConstrainRecord>();
            foreach (Canvas_Constrain constrain in OpenSpace.constrain_Manager.Constrains)
            {
                var p = GetConstrainConnectIndex(constrain);
                elements.Add(new ConstrainRecord()
                {
                    StartIndex = p.Item1,
                    EndIndex = p.Item2
                });
            }
            return elements;
        }
        internal static (int,int) GetConstrainConnectIndex(Canvas_Constrain constrain)
        {
            int start=0, end=0;
            for (int i = 0; i < OpenSpace.canvas_Object_Manager.ObjectItems.Count; i++)
            {
                if (OpenSpace.canvas_Object_Manager.ObjectItems[i].UI_Item == constrain.start) start = i;
                if (OpenSpace.canvas_Object_Manager.ObjectItems[i].UI_Item == constrain.end) end = i;
            }
            return (start, end);
        }
        internal static List<ElementRecord> GetElements()
        {
            List<ElementRecord> elements = new List<ElementRecord>();
            foreach(Canvas_Object_Manager.ObjectItem objectItem in OpenSpace.canvas_Object_Manager.ObjectItems)
            {
                if (objectItem.UI_Item.GetType() == typeof(AI_NodeUI)) continue;
                elements.Add(new ElementRecord() {
                    Name = objectItem.UI_Item.Uid,
                    Point = new PointRecord()
                    {
                        X = Canvas.GetLeft(objectItem.UI_Item),
                        Y = Canvas.GetTop(objectItem.UI_Item)
                    },
                    Type = objectItem.UI_Item.GetType().ToString(),
                    Property = GetPropertiesFromUI(objectItem.UI_Item as IElement)
                });
            }
            return elements;
        }
        internal static List<PropertyRecord> GetPropertiesFromUI(IElement ui)
        {
            List<PropertyRecord> elements = new List<PropertyRecord>();
            for (int i = 0; i < ui.properties.Count; i++)
            {
                elements.Add(new PropertyRecord()
                {
                    Index = i,
                    Value = ui.properties[i].Value
                });
            }
            return elements;
        }
        internal static void Save(string ProjectFile)
        {
            Record record = new Record();
            record.Name = CurrentProjectName;
            record.Path = CurrentProjectPath;
            record.Version = "v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            record.elements = GetElements();
            record.constrains = GetConstrains();
            using (FileStream fs = new FileStream(ProjectFile, FileMode.Create))
            {
                xmlSerializer.Serialize(fs, record);
                Console.WriteLine("Object has been serialized");
            }
        }
        internal static void Load(string ProjectFile, Action complite)
        {
            using (FileStream fs = new FileStream(ProjectFile, FileMode.Open))
            {
                Record record = xmlSerializer.Deserialize(fs) as Record;
                Console.WriteLine($"Object has been deserialized");
                OpenSpace.LoadFromFile(record);
                complite.Invoke();
            }
        }
        internal static void Create(string ProjectFolder,string ProjectName)
        {
            Record record = new Record();
            record.Name = CurrentProjectName = ProjectName;
            record.Path = CurrentProjectPath = ProjectFolder;
            record.Version = "v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            using (FileStream fs = new FileStream(ProjectFolder+"/"+ProjectName, FileMode.Create))
            {
                xmlSerializer.Serialize(fs, record);
                Console.WriteLine("Object has been serialized");
            }
        }
    }
}
