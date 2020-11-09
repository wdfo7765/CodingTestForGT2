using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace CodingTestForGT2Junior
{
    public class FtpDownloader : IDownloader
    {

        List<string> downLoadList = new List<string>();
        string[] downloadFiles;
        string address, id, password, remotePath, localFolderPath;

        StringBuilder result = new StringBuilder();
        FtpWebRequest reqFTP;

        bool IDownloader.DoDownload(out string resultMsg)
        {
            
            Console.WriteLine("<FTP Data Downloader>");
            Console.Write("Input Addrrss to download data: ");
            address = Console.ReadLine();
            Console.Write("Input ID to download data: ");
            id = Console.ReadLine();
            Console.Write("Input PassWord to download data: ");
            password = Console.ReadLine();
            Console.Write("Input RemotePath to download data: ");
            remotePath = Console.ReadLine();
            Console.WriteLine();
            Console.Write("Input LocalFolderPath file path: ");
            localFolderPath = Console.ReadLine();
            Console.WriteLine();

            string path = address + remotePath;
            string[] check = path.Split('/');
            if (check[check.Length-1].IndexOf('.') == -1)
            {
                //폴더일때
                downloadFiles = findList(address, remotePath);


            }
            else
            {
                address = "";
                for(int i=0; i<check.Length-1;i++)
                {
                    address += check[i];

                }
                downloadFiles = findList(address, "");
                for (int i = 0; i < downloadFiles.Length; i++)
                {
                    doDownLoad(address, id, password, localFolderPath, downloadFiles[i]);

                }


            }
            for(int i = 0; i<downloadFiles.Length;i++)
            {
               // Console.WriteLine(downLoadList[i]);
            }
            resultMsg = "";
            return false;
        }
        void doRecursion(string[] files,string address)
        {
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].IndexOf('.') == -1)
                {
                    doDownLoad(address +"/"+ files[i], id, password, localFolderPath, files[i]);
                }
                else
                {
                    doRecursion(findList(address + remotePath, downloadFiles[i]),address + remotePath + downloadFiles[i]);

                }


            }
        }
        string[] findList(string address,string remotePath)
        {
            string[] files;
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(address  + remotePath));
            reqFTP.UseBinary = true;
            reqFTP.Credentials = new NetworkCredential(id, password);
            reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
            WebResponse response = reqFTP.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());

            string line = reader.ReadLine();
            while (line != null)
            {
                result.Append(line);
                result.Append("\n");
                line = reader.ReadLine();
            }
            result.Remove(result.ToString().LastIndexOf('\n'), 1);
            reader.Close();
            response.Close();

            files = result.ToString().Split('\n');
            for (int i = 0; i < files.Length; i++)
            {
                Console.WriteLine(files[i]);
            }
            return files;
        }
        void doDownLoad(string address, string id, string password,string localFolderPath, string downloadFile)
        {
            if(downloadFile.IndexOf('.') ==-1)
            {
                return;
            }
            Uri sourceFileUri = new Uri(address + downloadFile);
            FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(sourceFileUri);

            ftpWebRequest.Credentials = new NetworkCredential(id, password);
            ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;

            FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse(); //여기서 문제생기네
            Stream sourceStream = ftpWebResponse.GetResponseStream();

            FileStream targetFileStream = new FileStream(localFolderPath + "/" + downloadFile, FileMode.Create, FileAccess.Write);


            int length = 2048;
            Byte[] buffer = new Byte[length];
            int bytesRead = sourceStream.Read(buffer, 0, length);
            while (bytesRead > 0)
            {
                targetFileStream.Write(buffer, 0, length);
                bytesRead = sourceStream.Read(buffer, 0, length);
            }
            targetFileStream.Close();
            sourceStream.Close();

            downLoadList.Add(downloadFile);
        }
    }
}
