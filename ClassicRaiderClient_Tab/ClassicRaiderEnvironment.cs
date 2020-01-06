using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ClassicRaiderClient_Tab
{
    public class ClassicRaiderEnvironment
    {
        FileInfo lastUploadedFile;

        public ClassicRaiderEnvironment()
        {
            new Thread(() => // Catch wow closing.
            {
                while (true)
                {
                    var WowProcess = Process.GetProcessesByName("WowClassic").FirstOrDefault();
                    if (WowProcess != null)
                    {
                        WowProcess.WaitForExit();

                        if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.Path))
                        {
                            // wait for file to be uploaded.
                            while ((new FileInfo(Properties.Settings.Default.Path).LastWriteTime != lastUploadedFile.LastWriteTime))
                            {
                                Thread.Sleep(50);//wait 50ms to recheck if file has been uploaded..
                            }

                            // delete file.
                            using (var sw = new StreamWriter(Properties.Settings.Default.Path))
                            {
                                sw.Write("ClassicRaiderProfilePrefs = nil\naldb = {}\nscanningEnabled = true");
                            }
                            File.Delete(Properties.Settings.Default.Path + ".bak");
                        }
                    }
                    Thread.Sleep(1000);
                }
            }).Start();
        }

        FileSystemWatcher watcher = new FileSystemWatcher();
        public bool AllowHandling { get; set; } = false;


        public void Setup(string name)
        {
            Properties.Settings.Default.Path = name;
            Properties.Settings.Default.Save();
        }

        public string GetAddonPath()
        {
            var FD = new System.Windows.Forms.OpenFileDialog();
            if (FD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileToOpen = FD.FileName;

                System.IO.FileInfo File = new System.IO.FileInfo(FD.FileName);
                //OR

                System.IO.StreamReader reader = new System.IO.StreamReader(fileToOpen);
                //etc
            }
            else return null;

            if (FD.FileName.Contains("SavedVariables\\ClassicRaiderProfile.lua"))
            {
                return FD.FileName;
            }
            else
            {
                MessageBox.Show("Wrong File: please select the ClassicRaiderProfile.lua").ToString();
                return null;
            }
        }

        public void CreateFileWatcher(string path)
        {
            //file watcher ... watches after changes
            watcher.Path = Path.GetDirectoryName(path);
            watcher.Filter = Path.GetFileName(path);
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size | NotifyFilters.CreationTime;

            //event handlers
            watcher.Changed += new FileSystemEventHandler(OnChanged);

            // Begin watching.
            watcher.EnableRaisingEvents = true;
            Console.WriteLine(watcher.EnableRaisingEvents);
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (AllowHandling == true)
            {
                var thread = new Thread(() =>
                {
                    NameValueCollection nvc = new NameValueCollection();
                    nvc.Add("id", "file");
                    nvc.Add("class", "upload-input");
                    HttpUploadFile("https://classicraider.com/UploadData",
                         @e.FullPath, "file", "lua", nvc);
                    lastUploadedFile = new FileInfo(e.FullPath);
                });

                thread.Start();
            }
            else
            {
                FileSystemWatcher watcher = sender as FileSystemWatcher;
                if (watcher != null)
                {
                    watcher.EnableRaisingEvents = false;
                }
            }


        }

        private void HttpUploadFile(string url, string file, string paramName, string contentType, NameValueCollection nvc)
        {
            Console.WriteLine(string.Format("Uploading {0} to {1}", file, url));
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = System.Net.CredentialCache.DefaultCredentials;

            Stream rs = wr.GetRequestStream();
            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in nvc.Keys)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, nvc[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, paramName, file, contentType);
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                rs.Write(buffer, 0, bytesRead);
            }
            fileStream.Close();

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();



            WebResponse wresp = null;
            try
            {
                wresp = wr.GetResponse();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                Console.WriteLine(string.Format("File uploaded, server response is:"));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error uploading file", ex);
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }
        }
    }
}
