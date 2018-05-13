using FluentFTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ftpClient.Core
{
    public class FtpFileObject
    {
        public string Name { get; set; }

        public long Size { get; set; }

        public DateTime ModifyDate { get; set; }

        public bool IsFolder { get; set; }

        public FtpFileObject(string name, long size, DateTime modifyDate, bool isFolder)
        {
            this.Name = name;
            this.Size = size;
            this.ModifyDate = modifyDate;
            this.IsFolder = isFolder;
        }
    }
}
