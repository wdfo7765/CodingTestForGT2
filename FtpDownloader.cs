using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTestForGT2Junior
{
    public class FtpDownloader : IDownloader
    {
        bool IDownloader.DoDownload(out string resultMsg)
        {
            resultMsg = "FTP Downloader is not yet implemented";

            return false;
        }
    }
}
