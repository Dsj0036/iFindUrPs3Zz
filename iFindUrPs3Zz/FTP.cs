using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System
{
    internal static class FTP
    {
        public static FtpStatusCode RenameFile(string filename, string newpath)
        {
            FtpWebRequest ftpRequest = null;
            FtpWebResponse ftpResponse = null;
            
                ftpRequest = (FtpWebRequest)WebRequest.Create(filename);
                ftpRequest.Credentials = new NetworkCredential("user", "pass");
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.Rename;
                ftpRequest.RenameTo = newpath;
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
            FtpStatusCode _stc = ftpResponse.StatusCode;
                ftpResponse.Close();
                ftpRequest = null;
            return _stc;
           
        }
        public static void CreateDirectory(string url)
        {

            try
            {
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(url.Replace("http", "ftp").Replace("https", "ftp"));
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                request.Credentials = new NetworkCredential("user", "pass");
                using (var resp = (FtpWebResponse)request.GetResponse())
                {
                    Console.WriteLine(resp.StatusCode);
                }
            }
            catch
            {

            }
        }
        public static string[] GetContents(string path, string usn = "", string pass = "")
        {
            FtpWebRequest webRequest = (FtpWebRequest)FtpWebRequest.Create(path.Replace("http", "ftp"));
            webRequest.Credentials = new NetworkCredential(usn, pass);
            webRequest.Method = WebRequestMethods.Ftp.ListDirectory;
            webRequest.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            try
            {
                FtpWebResponse response = (FtpWebResponse)webRequest.GetResponse();

                StreamReader streamReader = new StreamReader(response.GetResponseStream());
                List<string> directories = new List<string>();
                string line = streamReader.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    if (line.StartsWith(".") is false)
                    {
                        directories.Add(line);

                    }
                    line = streamReader.ReadLine();
                }
                streamReader.Close();
                return directories.ToArray();
            }
            catch
            {
                return null;
            }
        }
        public static string[] GetFiles(string path, string usn = "", string pass = "")
        {
            try
            {
                FtpWebRequest webRequest = (FtpWebRequest)FtpWebRequest.Create(path.Replace("http", "ftp"));
                webRequest.Credentials = new NetworkCredential(usn, pass);
                Debug.WriteLine("Request opening.", "ftp");
                webRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse response = (FtpWebResponse)webRequest.GetResponse();
                StreamReader streamReader = new StreamReader(response.GetResponseStream());
                List<string> files = new List<string>();
                string line = streamReader.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    if (line.StartsWith(".") is false & Path.GetExtension(line) != "")
                    {
                        files.Add(line);

                    }
                    line = streamReader.ReadLine();
                }
                Debug.WriteLine("Response closing with data length of " + files.Count);
                streamReader.Close();
                response.Close();
                return files.ToArray();
            }
            catch
            {
                return null;
            }
        }
        public static string[] GetDirectories(string path, string usn = "", string pass = "")
        {
            FtpWebRequest webRequest = (FtpWebRequest)WebRequest.Create(path);
            webRequest.Credentials = new NetworkCredential(usn, pass);
            webRequest.Method = WebRequestMethods.Ftp.ListDirectory;
            try
            {

                FtpWebResponse response = (FtpWebResponse)webRequest.GetResponse();
                StreamReader streamReader = new StreamReader(response.GetResponseStream());
                List<string> dirs = new List<string>();
                string line = streamReader.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    if (line.StartsWith(".") is false & Path.GetExtension(line) == "")
                    {
                        dirs.Add(line);
                    }
                    line = streamReader.ReadLine();
                }
                Debug.WriteLine("Response closing with data length of " + dirs.Count);

                streamReader.Close();
                response.Close();
                return dirs.ToArray();
            }
            catch { return null; }  
        }
        public static bool DeleteFile(Uri serverUri)
        {
            try
            {
                // The serverUri parameter should use the ftp:// scheme.
                // It contains the name of the server file that is to be deleted.
                // Example: ftp://contoso.com/someFile.txt.
                // 

                if (serverUri.Scheme != Uri.UriSchemeFtp)
                {
                    return false;
                }
                // Get the object used to communicate with the server.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(serverUri);
                request.Credentials = new NetworkCredential("","");
                request.Method = WebRequestMethods.Ftp.DeleteFile;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                //Console.WriteLine("Delete status: {0}", response.StatusDescription);
                response.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// todo: set web index checking before request <summary>
        /// todo: set web index checking before request
        /// </summary>
        /// <param name="ftp"></param>
        /// <param name="usn"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public static long GetLength(string ftp, string usn = "", string pass = "")
        {
            try
            {
                var dir = ftp.Substring(0, ftp.LastIndexOf('/'));
                var names = FTP.GetContents(dir).ToList();
                if (names.Contains(ftp.GetFileName()))
                {
                    FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(ftp.Replace("http", "ftp"));
                    request.Proxy = null;
                    request.Credentials = new NetworkCredential(usn, pass);
                    request.Method = WebRequestMethods.Ftp.GetFileSize;

                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    long size = response.ContentLength;
                    response.Close();
                    return size;
                }
                else return 0x00;
            }
            catch 
            {
                return 0x00;
            }

        }
        public static void Upload(string localfilename, string ftpDirectory, Action<Pair<long>> onProgression = null, ThreadPriority priority = ThreadPriority.Normal, EventHandler finished = null)
        {
            Thread p = new Thread(() =>
            {
                string path = $"{ftpDirectory.TrimEnd('/')}/{Path.GetFileName(localfilename).TrimStart('/')}";
                FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(path);
                ftpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;
                Pair<long> outto = new Pair<long>(0, 0);
                using (FileStream inputStream = File.OpenRead(localfilename))

                using (Stream outputStream = ftpWebRequest.GetRequestStream())
                {
                    byte[] buffer = new byte[1024 * 1024];
                    int totalReadBytesCount = 0;
                    int readBytesCount;
                    while ((readBytesCount = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        outputStream.Write(buffer, 0, readBytesCount);
                        totalReadBytesCount += readBytesCount;

                        double progress = totalReadBytesCount * 100.0 / inputStream.Length;
                        outto.SetX(totalReadBytesCount);
                        outto.SetY(inputStream.Length);
                        onProgression?.Invoke(outto);
                    }
                    finished?.Invoke(null, null);
                }

            })
            {
                Priority = priority,
                IsBackground = true
            };
            p.Start();

        }
        public static void Download(string downloadFileName, string ftpFileName, string usn, string pass, EventHandler finished, EventHandler<Pair<int>> OnProgression = null,  EventHandler<Exception> onExcept = null, ThreadPriority priority = ThreadPriority.Normal)
        {
            Thread p = new Thread(() =>
            {
                try
                {
                    NetworkCredential credentials = new NetworkCredential(usn, pass);

                    // Query size of the file to be downloaded
                    WebRequest sizeRequest = WebRequest.Create(ftpFileName);
                    sizeRequest.Credentials = credentials;
                    sizeRequest.Method = WebRequestMethods.Ftp.GetFileSize;
                    int size = (int)sizeRequest.GetResponse().ContentLength;

                    // Download the file
                    WebRequest request = WebRequest.Create(ftpFileName);
                    request.Credentials = credentials;
                    request.Method = WebRequestMethods.Ftp.DownloadFile;
                    Pair<int> progress = new Pair<int>(0, 0);
                    using (Stream ftpStream = request.GetResponse().GetResponseStream())
                    using (Stream fileStream = File.Create(downloadFileName))
                    {
                        byte[] buffer = new byte[10240];
                        int read;
                        while ((read = ftpStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fileStream.Write(buffer, 0, read);
                            if (OnProgression != null)
                            {
                                int position = (int)fileStream.Position;
                                progress.SetX(position);
                                progress.SetY(size);
                                OnProgression?.Invoke(null, progress);
                            }
                        }
                    }
                    finished(null, null);
                }
                catch (Exception ex) 
                {
                    if (Debugger.IsAttached)
                    {
                        throw;
                    }
                    else
                    {
                        onExcept?.Invoke(Debugger.DefaultCategory, ex);
                    }
                }
               
            })
            {
                Priority = priority,
                IsBackground = true
            };
            p.Start();
        }
    }
}
