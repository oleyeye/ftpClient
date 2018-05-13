using FluentFTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ftpClient.Core
{
    public sealed class Ftp
    {
        private static readonly Ftp ftp = new Ftp();

        private FtpClient client;


        private Ftp()
        {
            client = new FtpClient();
        }

        public static Ftp Instance
        {
            get
            {
                return ftp;
            }
        }

        public void Connect(string host, int port = 21, string userName = "anonymous", string password = "anonymous")
        {
            this.Disconnct();
            client.Host = host;
            client.Port = port;

            client.Credentials = new NetworkCredential(userName, password);
            client.Connect();

        }

        public void Disconnct()
        {
            if (client.IsConnected)
            {
                client.Disconnect();
            }
        }

        public IList<FtpFileObject> GetListing()
        {
            List<FtpFileObject> list = new List<FtpFileObject>();

            foreach (FtpListItem item in client.GetListing(client.GetWorkingDirectory(),
                    FtpListOption.Modify | FtpListOption.Size))
            {

                switch (item.Type)
                {
                    case FtpFileSystemObjectType.Directory:
                        var folder = new FtpFileObject(item.Name, item.Size, item.Modified, true);
                        list.Add(folder);
                        break;
                    case FtpFileSystemObjectType.File:
                        var file = new FtpFileObject(item.Name, item.Size, item.Modified, false);
                        list.Add(file);
                        break;
                    default:
                        break;
                }
            }

            return list;
        }

        public int UploadFile(string[] files)
        {
            var curDir = client.GetWorkingDirectory();
            return client.UploadFiles(files, curDir, FtpExists.Overwrite, true);
        }

        public bool DownloadFile(string remoteName, string savedName)
        {
            return client.DownloadFile(savedName, Path.Combine(client.GetWorkingDirectory(), remoteName));
        }

        public IList<FtpFileObject> GetListing(string folder)
        {
            client.SetWorkingDirectory(Path.Combine(client.GetWorkingDirectory(), folder));
            return GetListing();
        }

        public IList<FtpFileObject> GoToParentFolder()
        {
            var curDir = client.GetWorkingDirectory();

            var parentDir = curDir == "/" ? "/" : curDir.Substring(0, curDir.LastIndexOf('/') + 1);
            client.SetWorkingDirectory(parentDir);
            return GetListing();
        }
    }
}
