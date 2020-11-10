using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Configuration;
using System.Threading.Tasks;
using System.Xml;

namespace CodingTestForGT2Junior
{
    public class FtpDownloader : IDownloader
    {

        string[] downloadFiles;
        string address, id, password, remotePath, localFolderPath; //변수선언

        AppSettingsReader ar = new AppSettingsReader();

        StringBuilder result = new StringBuilder();
        FtpWebRequest reqFTP;
        string savePath;

        bool IDownloader.DoDownload(out string resultMsg)
        {

            savePath = (string)ar.GetValue("xmlPath", typeof(string));


            //FTP정보를 입력받음
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

            //입력한 정보가 파일인지 폴더인지 구분
            string path = address + remotePath;
            string[] check = path.Split('/');

            //입력한 정보가 폴더일 경우
            if (check[check.Length-1].IndexOf('.') == -1) 
            {
                string folderPath = "C:"+localFolderPath+"/"+check[check.Length-1];
                DirectoryInfo di = new DirectoryInfo(folderPath);
                //입력한 폴더가 LocalFolder 존재하지 않을경우 생성
                if(di.Exists==false)
                {
                    di.Create();
                }
 
                //다운로드 할 파일들의 목록
                downloadFiles = findList(address, remotePath);
            }

            //입력한 정보가 파일일 경우
            else
            {
                //파일인 경우 파일이 존재하는 폴더에 있는 파일들을 다운로드
                address.Substring(0, address.Length - check[check.Length - 1].Length);
                
                downloadFiles = findList(address, "");
            }

            //파일 목록을 이용하여 다운로드
            for (int i = 0; i < downloadFiles.Length; i++)
            {
                doDownLoad(address, id, password, localFolderPath, downloadFiles[i]);
            }

            resultMsg = "Succeeded to download data from FTP Server";
            return false;
        }

        /// <summary>
        /// FTP서버에 존재하는 파일 목록을 불러옴
        /// </summary>
        /// <param name="address"></param>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        public string[] findList(string address,string remotePath)
        {
            string[] files;
            saveXML();

            //FTP정보들로 FTP서버와 연결
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(address  + remotePath));
            reqFTP.UseBinary = true;
            reqFTP.Credentials = new NetworkCredential(id, password);
            reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
            WebResponse response = reqFTP.GetResponse(); //Response
            StreamReader reader = new StreamReader(response.GetResponseStream());


            //Stream을 line으로 저장
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
            //files에 Stream으로 받은 정보를 저장
            files = result.ToString().Split('\n');
            for (int i = 0; i < files.Length; i++)
            {
                //다운받을 파일 목록을 출력
                Console.WriteLine(files[i]);
            }
            return files;
        }
        /// <summary>
        /// FTP연결 정보를 XML파일에 저장
        /// </summary>
        public void saveXML()
        {
            //초기 변수선언
            FileInfo xmlFile = new FileInfo(savePath + "FTPInfo.xml");
            XmlDocument xml = new XmlDocument();
            XmlNode root;
            DateTime now = DateTime.Now;


            //XML파일이 존재할시
            if (xmlFile.Exists)
            {
                xml.Load(savePath + "FTPInfo.xml");
                root = xml.DocumentElement;
            }
            //XML파일이 존재하지 않을 경우
            else
            {
                root = xml.CreateElement("INFO");
            }
            XmlNode connect = xml.CreateElement("FTPConnect");


            //Node에 정보 작성
            XmlNode time = xml.CreateElement("Time");
            time.InnerText = now.ToString();
            connect.AppendChild(time);
            XmlNode ID = xml.CreateElement("ID");
            ID.InnerText = id;
            connect.AppendChild(ID);
            XmlNode PassWord = xml.CreateElement("PassWord");
            PassWord.InnerText = password;
            connect.AppendChild(PassWord);
            XmlNode LocalFolderPath = xml.CreateElement("LocalFolderPath");
            LocalFolderPath.InnerText = localFolderPath;
            connect.AppendChild(LocalFolderPath);
            XmlNode Address = xml.CreateElement("Address");
            Address.InnerText = address;
            connect.AppendChild(Address);
            XmlNode RemotePath = xml.CreateElement("RemotePath");
            RemotePath.InnerText = remotePath;
            connect.AppendChild(RemotePath);


            root.AppendChild(connect);
            //정보가 담긴 root 노드를 child로 추가
            xml.AppendChild(root);

            //xml 저장
            xml.Save(savePath + "FTPInfo.xml");
        }

        /// <summary>
        /// FTP서버에 존재하는 파일들을 다운로드
        /// </summary>
        /// <param name="address"></param>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <param name="localFolderPath"></param>
        /// <param name="downloadFile"></param>
        public void doDownLoad(string address, string id, string password,string localFolderPath, string downloadFile)
        {
            //다운로드 할 목록이 파일이 아닌경우 종료
            if(downloadFile.IndexOf('.') ==-1)
            {
                return;
            }

            //FTP 정보들로 서버와 연결
            Uri sourceFileUri = new Uri(address + downloadFile);
            FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(sourceFileUri);

            ftpWebRequest.Credentials = new NetworkCredential(id, password);
            ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;

            FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse(); //여기서 문제생기네
            Stream sourceStream = ftpWebResponse.GetResponseStream();

            FileStream targetFileStream = new FileStream(localFolderPath + "/" + downloadFile, FileMode.Create, FileAccess.Write);

            //최대 길이
            int length = 2048;
            Byte[] buffer = new Byte[length];
            int bytesRead = sourceStream.Read(buffer, 0, length);
            while (bytesRead > 0)
            {
                //Stream으로 받아온 정보를 파일에 Write
                targetFileStream.Write(buffer, 0, length);
                bytesRead = sourceStream.Read(buffer, 0, length);
            }
            targetFileStream.Close();
            sourceStream.Close();

        }
    }
}
