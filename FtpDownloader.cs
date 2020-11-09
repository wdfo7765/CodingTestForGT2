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
        bool IDownloader.DoDownload(out string resultMsg)
        {
            string address, id, password,remotePath,localFolderPath;
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

            List<string> downLoadList = new List<string>();
            string[] downloadFiles;
            StringBuilder result = new StringBuilder();
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(address + "/" + remotePath));
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

                downloadFiles =result.ToString().Split('\n');
                for (int i = 0; i < downloadFiles.Length; i++)
                {
                    Console.WriteLine(downloadFiles[i]);
                }
            }
            catch
            {
                downloadFiles = null;
            }


            for (int i = 0; i < downloadFiles.Length; i++)
            {

                //다운로드 시작
                Uri sourceFileUri = new Uri(address+downloadFiles[i]);
                FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(sourceFileUri);

                ftpWebRequest.Credentials = new NetworkCredential(id, password);
                ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;

                FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse(); //여기서 문제생기네
                Stream sourceStream = ftpWebResponse.GetResponseStream();

                FileStream targetFileStream = new FileStream(localFolderPath+"/"+downloadFiles[i], FileMode.Create, FileAccess.Write);


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

                downLoadList.Add(downloadFiles[i]);
            }


            /*
            //다운로드 시작
            Uri sourceFileUri = new Uri(address);
            FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(sourceFileUri) ;

            ftpWebRequest.Credentials = new NetworkCredential(id, password);
            ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;

            FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse() ; //여기서 문제생기네
            Stream sourceStream = ftpWebResponse.GetResponseStream();

            FileStream targetFileStream = new FileStream(localFolderPath, FileMode.Create, FileAccess.Write);


            int length = 2048;
            Byte[] buffer = new Byte[length];
            int bytesRead = sourceStream.Read(buffer, 0, length);
            while(bytesRead >0)
            {
                targetFileStream.Write(buffer, 0, length);
                bytesRead = sourceStream.Read(buffer, 0, length);
            }

            */

            /*byte[] bufferByteArray = new byte[1024];
            while (true)

            {
                int byteCount = sourceStream.Read(bufferByteArray, 0, bufferByteArray.Length);
                if (byteCount == 0)
                {
                    break;
                }
                targetFileStream.Write(bufferByteArray, 0, byteCount);
            }
            */



            
     

            for(int i = 0; i<downloadFiles.Length;i++)
            {
                Console.WriteLine(downLoadList[i]);
            }
            resultMsg = "";
            return false;
        }
    }
}
