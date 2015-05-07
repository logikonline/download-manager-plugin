using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using UIKit;
using Xam.Plugin.DownloadManager.Abstractions;
using Xamarin.Forms;

[assembly: Dependency(typeof(Xam.Plugin.DownloadManager.iOS.DownloadManager))]
namespace Xam.Plugin.DownloadManager.iOS
{
    public class DownloadManager : IDownloadManager
    {

        /// <summary>
        /// Use WebClient to do Download. Send Alert to user know its done
        /// </summary>
        /// <param name="uri">Full url to download</param>
        /// <param name="filename">File to download</param>
        public void Download(string uri, string filename)
        {
            var webClient = new WebClient();

            webClient.DownloadDataCompleted += (s, e) =>
            {
                var bytes = e.Result; // get the downloaded data
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string localFilename = filename;
                string localPath = Path.Combine(documentsPath, localFilename);
                File.WriteAllBytes(localPath, bytes); // writes to local storage   
            };

            var url = new Uri(uri);

            webClient.DownloadDataAsync(url);

            new UIAlertView("Done", "Download Done.", null, "OK", null).Show();
        }
    }
}
