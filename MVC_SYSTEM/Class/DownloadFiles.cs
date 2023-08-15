﻿using MVC_SYSTEM.TrickModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Web;

namespace MVC_SYSTEM.Class
{
    public class DownloadFiles
    {
        public class NetworkConnection : IDisposable
        {
            string _networkName;

            public NetworkConnection(string networkName,
                NetworkCredential credentials)
            {
                _networkName = networkName;

                var netResource = new NetResource()
                {
                    Scope = ResourceScope.GlobalNetwork,
                    ResourceType = ResourceType.Disk,
                    DisplayType = ResourceDisplaytype.Share,
                    RemoteName = networkName
                };

                var userName = string.IsNullOrEmpty(credentials.Domain)
                    ? credentials.UserName
                    : string.Format(@"{0}\{1}", credentials.Domain, credentials.UserName);

                var result = WNetAddConnection2(
                    netResource,
                    credentials.Password,
                    userName,
                    0);

                if (result != 0)
                {
                    throw new System.ComponentModel.Win32Exception(result, "Error connecting to remote share" + credentials.Password.ToString() + "--" + credentials.Domain + credentials.UserName);
                }
            }

            ~NetworkConnection()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                WNetCancelConnection2(_networkName, 0, true);
            }

            [DllImport("mpr.dll")]
            private static extern int WNetAddConnection2(NetResource netResource,
                string password, string username, int flags);

            [DllImport("mpr.dll")]
            private static extern int WNetCancelConnection2(string name, int flags,
                bool force);
        }

        [StructLayout(LayoutKind.Sequential)]
        public class NetResource
        {
            public ResourceScope Scope;
            public ResourceType ResourceType;
            public ResourceDisplaytype DisplayType;
            public int Usage;
            public string LocalName;
            public string RemoteName;
            public string Comment;
            public string Provider;
        }

        public enum ResourceScope : int
        {
            Connected = 1,
            GlobalNetwork,
            Remembered,
            Recent,
            Context
        };

        public enum ResourceType : int
        {
            Any = 0,
            Disk = 1,
            Print = 2,
            Reserved = 8,
        }

        public enum ResourceDisplaytype : int
        {
            Generic = 0x0,
            Domain = 0x01,
            Server = 0x02,
            Share = 0x03,
            File = 0x04,
            Group = 0x05,
            Network = 0x06,
            Root = 0x07,
            Shareadmin = 0x08,
            Directory = 0x09,
            Tree = 0x0a,
            Ndscontainer = 0x0b
        }
        public class FileDownloads
        {
            public List<DownLoadFileInformation> GetFiles(string path)
            {

                List<DownLoadFileInformation> listFiles = new List<DownLoadFileInformation>();

                //NetworkCredential NCredentials = new NetworkCredential("ashahri.as", "ashahri123", "felhqr");
                //userName   --> Serevr User Name
                //passWord   --> Server Password
                //DomainName --> Yourdomain Name

                // If server your server not in domain user below line
                //NetworkCredential NCredentials = new NetworkCredential("userNmae","passWord");

                //Path For download From Network Path.

                //using (new NetworkConnection(path, NCredentials))
                //{

                    string fileSavePath = Path.GetDirectoryName(path);
                    DirectoryInfo dirInfo = new DirectoryInfo(fileSavePath);

                    int i = 0;
                    foreach (var item in dirInfo.GetFiles())
                    {
                        listFiles.Add(new DownLoadFileInformation()
                        {

                            FileId = i + 1,
                            FileName = item.Name,
                            FilePath = dirInfo.FullName + @"\" + item.Name

                        });
                        i = i + 1;
                    }
                //}
                return listFiles;

            }

        }
    }
}