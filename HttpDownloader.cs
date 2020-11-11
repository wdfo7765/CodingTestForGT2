using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CodingTestForGT2Junior
{
    public class HttpDownloader : IDownloader
    {
    
        /// <summary>
        /// Http 다운로드 함수
        /// </summary>
        /// <param name="resultMsg"></param>
        /// <returns></returns>
        public bool DoDownload(out string resultMsg)
        {
            //변수 선언
            string url, downlaodLocalFolderPath;

            //프린트 출력 및 입력 함수
            string[] inputInfo = printHelp();
            url = inputInfo[0];
            downlaodLocalFolderPath = inputInfo[1];

            //파일이 존재하는지 확인
            isExist(downlaodLocalFolderPath);

            //타겟파일 경로 오류 체크
            if(!Directory.CreateDirectory(Path.GetDirectoryName(downlaodLocalFolderPath)).Exists)
            {
                resultMsg = "Please, check the target file path.";
                return false;
            }

            //파일 다운로드
            downloadFile(url, downlaodLocalFolderPath);
            
            if (File.Exists(downlaodLocalFolderPath))
            {
                resultMsg = "Succeeded to download data from URL.";
                return true;
            }
            else
            {
                resultMsg = "Failed to download data form URL.";
                return false;
            }
        }
        /// <summary>
        /// 안내문 출력 및 정보 입력받기
        /// </summary>
        public string[] printHelp()
        {
            string url, downlaodLocalFolderPath;
            Console.WriteLine();
            Console.WriteLine("<HTTP Data Downloader>");
            Console.Write("Input URL to download data: ");
            url = Console.ReadLine();
            Console.WriteLine();
            Console.Write("Input target file path: ");
            downlaodLocalFolderPath = Console.ReadLine();
            Console.WriteLine();

            string[] info = { url, downlaodLocalFolderPath };
            return info;
        }
        /// <summary>
        /// 파일이 존재하는지 확인, 존재한다면 삭제
        /// </summary>
        public void isExist(string downlaodLocalFolderPath)
        {
            if (File.Exists(downlaodLocalFolderPath))
            {
                FileInfo file = new FileInfo(downlaodLocalFolderPath);
                file.IsReadOnly = false;
                File.Delete(downlaodLocalFolderPath);
            }
        }

        /// <summary>
        /// 파일 다운로드
        /// </summary>
        /// <param name="url"></param>
        /// <param name="downlaodLocalFolderPath"></param>
        public void downloadFile(string url, string downlaodLocalFolderPath)
        {
            //http://www.celestrak.com/NORAD/elements/tle-new.txt
            using (var client = new WebClient())
            {
                client.DownloadFile(url, downlaodLocalFolderPath);
            }
        }

    }
}
