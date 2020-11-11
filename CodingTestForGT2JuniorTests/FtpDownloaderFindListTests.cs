using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodingTestForGT2Junior;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTestForGT2Junior
{
    [TestClass()]
    public class FtpDownloaderTests
    {
        FtpDownloader testDownLoader = new FtpDownloader();
        string address = "ftp://ftp.iers.org/products/eop/bulletinb/format_2009/";
        string[] downloadFiles;
        string remotePath = "";
        string id = "";
        string password = "";
        string localFolderPath = "/testDownLoad";


        [TestMethod()]
        public void findListTest()
        {
            bool istrue = false;

            downloadFiles = testDownLoader.findList(address, remotePath, id, password, localFolderPath);

            foreach (string str in downloadFiles)
            {
                if (str == "bulletinb-369.txt")
                {
                    istrue = true;
                }
            }
            Assert.IsTrue(istrue);
        }

        [TestMethod()]
        public void doDownloadTest()
        {
            downloadFiles = testDownLoader.findList(address, remotePath, id, password, localFolderPath);
            testDownLoader.doDownLoad(address, id, password, localFolderPath, downloadFiles[0]);
            FileInfo fi = new FileInfo(localFolderPath + "/" + downloadFiles[0]);
            Assert.IsTrue(fi.Exists);
        }

    }
}