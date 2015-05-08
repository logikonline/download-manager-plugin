using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xam.Plugin.DownloadManager.Abstractions;
using Xamarin.Forms;

[assembly: Dependency(typeof(Xam.Plugin.DownloadManager.Droid.DownloadManager))]
namespace Xam.Plugin.DownloadManager.Droid
{
    public class DownloadManager : IDownloadManager
    {
        public static void Init()
        {
        }
        /// <summary>
        /// Use DownloadManager of Android to Download. User Can se status on Action Bar
        /// </summary>
        /// <param name="uri">Full url to download</param>
        /// <param name="filename">File to download</param>
        public void Download(string uri, string filename)
        {
            Android.Net.Uri contentUri = Android.Net.Uri.Parse(uri);

            Android.App.DownloadManager.Request r = new Android.App.DownloadManager.Request(contentUri);


            r.SetDestinationInExternalPublicDir(Android.OS.Environment.DirectoryDownloads, filename);

            r.AllowScanningByMediaScanner();

            r.SetNotificationVisibility(Android.App.DownloadVisibility.VisibleNotifyCompleted);

            Android.App.DownloadManager dm = (Android.App.DownloadManager)Xamarin.Forms.Forms.Context.GetSystemService(Android.Content.Context.DownloadService);

            dm.Enqueue(r);
        }
    }
}
