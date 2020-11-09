using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTestForGT2Junior
{
    public interface IDownloader
    {
        bool DoDownload(out string resultMsg);
    }
}
