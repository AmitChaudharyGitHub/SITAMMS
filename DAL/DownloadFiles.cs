using MMS.ViewModels.File;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace MMS.DAL
{
    public class DownloadFiles
    {
        public List<DownLoadFileInformation> GetFiles()
        {
            List<DownLoadFileInformation> lstFiles = new List<DownLoadFileInformation>();
            DirectoryInfo dirInfo = new DirectoryInfo(HostingEnvironment.MapPath("~/UploadedFiles"));

            int i = 0;
            foreach (var item in dirInfo.GetFiles())
            {
                lstFiles.Add(new DownLoadFileInformation()
                {

                    FileId = i + 1,
                    FileName = item.Name,
                    FilePath = dirInfo.FullName + @"\" + item.Name
                });
                i = i + 1;
            }
            return lstFiles;
        }

        public DownLoadFileInformation GetSelectedFile(int DocId,string filePath,string fileName)
        {
            List<DownLoadFileInformation> lstFiles = new List<DownLoadFileInformation>();
            DirectoryInfo dirInfo = new DirectoryInfo(HostingEnvironment.MapPath("~/UploadedFiles"));
            
            var file = new DownLoadFileInformation() {

                FileId = DocId,
                FileName = fileName,
                FilePath = filePath
            };


            return file;
        }



        public List<DownLoadFileInformation> GetFilesForGateEntry()
        {
            List<DownLoadFileInformation> lstFiles = new List<DownLoadFileInformation>();
            DirectoryInfo dirInfo = new DirectoryInfo(HostingEnvironment.MapPath("~/GateEntryUploadFiles"));

            int i = 0;
            foreach (var item in dirInfo.GetFiles())
            {
                lstFiles.Add(new DownLoadFileInformation()
                {

                    FileId = i + 1,
                    FileName = item.Name,
                    FilePath = dirInfo.FullName + @"\" + item.Name
                });
                i = i + 1;
            }
            return lstFiles;
        }

        public List<DownLoadFileInformation> GetSelectedFileForGateEntry(string FFname)
        {
            List<DownLoadFileInformation> lstFiles = new List<DownLoadFileInformation>();
            DirectoryInfo dirInfo = new DirectoryInfo(HostingEnvironment.MapPath("~/GateEntryUploadFiles"));
            int i = 0;
            foreach (var item in dirInfo.GetFiles())
            {
                if (item.Name.Equals(FFname, StringComparison.InvariantCultureIgnoreCase))
                {
                    lstFiles.Add(new DownLoadFileInformation()
                    {

                        FileId = i + 1,
                        FileName = item.Name,
                        FilePath = dirInfo.FullName + @"\" + item.Name
                    });
                }


                i = i + 1;
            }
            return lstFiles;
        }


        public List<DownLoadFileInformation> GetFilesForMRN()
        {
            List<DownLoadFileInformation> lstFiles = new List<DownLoadFileInformation>();
            DirectoryInfo dirInfo = new DirectoryInfo(HostingEnvironment.MapPath("~/MRNFileUpload"));

            int i = 0;
            foreach (var item in dirInfo.GetFiles())
            {
                lstFiles.Add(new DownLoadFileInformation()
                {

                    FileId = i + 1,
                    FileName = item.Name,
                    FilePath = dirInfo.FullName + @"\" + item.Name
                });
                i = i + 1;
            }
            return lstFiles;
        }

        public List<DownLoadFileInformation> GetSelectedFileForMRN(string FFname)
        {
            List<DownLoadFileInformation> lstFiles = new List<DownLoadFileInformation>();
            DirectoryInfo dirInfo = new DirectoryInfo(HostingEnvironment.MapPath("~/MRNFileUpload"));
            int i = 0;
            foreach (var item in dirInfo.GetFiles())
            {
                if (item.Name.Equals(FFname, StringComparison.InvariantCultureIgnoreCase))
                {
                    lstFiles.Add(new DownLoadFileInformation()
                    {

                        FileId = i + 1,
                        FileName = item.Name,
                        FilePath = dirInfo.FullName + @"\" + item.Name
                    });
                }


                i = i + 1;
            }
            return lstFiles;
        }

    }
}