using CaseManager.RecordSystem.RecordModel;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Xml.Serialization;
namespace CaseManager.RecordSystem
{
    public static class IOCore
    {
        public static XmlSerializer xmlSerializer = new XmlSerializer(typeof(Record));
        public static string CurrentProjectPath;
        public static string CurrentProjectName;
        public static void SaveProject() => Save(CurrentProjectPath + "/" + CurrentProjectName);
        internal static void Save(string ProjectFile)
        {
            Record record = new Record();
            record.Name = CurrentProjectName;
            record.Path = CurrentProjectPath;
            record.Version = "v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();

            using (FileStream fs = new FileStream(ProjectFile, FileMode.OpenOrCreate))
            {
                xmlSerializer.Serialize(fs, record);
                Console.WriteLine("Object has been serialized");
            }
        }
        public static void LoadProject(string ProjectFile)
        {
            using (FileStream fs = new FileStream(ProjectFile, FileMode.OpenOrCreate))
            {
                Record record = xmlSerializer.Deserialize(fs) as Record;
                Console.WriteLine($"Object has been deserialized");
            }
        }
    }
}
