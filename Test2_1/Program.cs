using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;

namespace Test2_1
{
    class Program
    {
        static void Main(string[] args)
        {
            string zipDestinationPath = ConfigurationManager.AppSettings["ZipDestinationPath"];
            using (FileStream zipToOpen = new FileStream($@"{zipDestinationPath}\{DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss")}.zip", FileMode.Create))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    var imageSourcePath = ConfigurationManager.AppSettings["ImageSourcePath"];
                    ZipArchiveEntry readmeEntry;
                    DirectoryInfo d = new DirectoryInfo(imageSourcePath);
                    FileInfo[] Files = d.GetFiles("*");

                    List<MetaInfo> metainfos = new List<MetaInfo>();
                    foreach (FileInfo file in Files)
                    {
                        var imageRoute = $@"{GetCurrentDayOfWeek()}\{file.Name}";                       
                        readmeEntry = archive.CreateEntryFromFile($@"{imageSourcePath}\{file.Name}", imageRoute);
                        metainfos.Add(new MetaInfo { 
                            Id = GetCurrentJulianDate(),
                            Creationdate = DateTime.Now,
                            ImageRoute = imageRoute
                        });
                    }

                    ZipArchiveEntry readmeEntryMeta = archive.CreateEntry("info.meta");
                    using (StreamWriter writer = new StreamWriter(readmeEntryMeta.Open()))
                    {
                        string delimiter = ConfigurationManager.AppSettings["MetaInfoDelimiter"];
                        writer.WriteLine($"      ID      {delimiter}      Creation Date      {delimiter} Image route");
                        int i = 1;
                        foreach (var item in metainfos) {
                            writer.WriteLine($"{item.Id}{i.ToString("D5")}{delimiter}{item.Creationdate.ToString("yyyy/MM/dd HH:mm:ss")}{delimiter}{item.ImageRoute}");
                            i++;
                        }
                    }
                }
            }
        }

        private static string GetCurrentDayOfWeek() {
            DateTime date = new DateTime();
            return date.DayOfWeek.ToString();
        }

        private static int GetCurrentJulianDate()
        {
            DateTime date = new DateTime();
            return (date.Year % 100) * 1000 + date.DayOfYear;
        }
    }
}
