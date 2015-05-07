using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xam.Plugin.DownloadManager.Abstractions
{
    public interface IDownloadManager
    {
        /// <summary>
        /// Do Download
        /// </summary>
        /// <param name="uri">Full Url to Download</param>
        /// <param name="filename">Name of File</param>
        void Download(string uri, string filename);

    }
}
