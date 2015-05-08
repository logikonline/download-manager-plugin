using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.UI.Popups;
using Xam.Plugin.DownloadManager.Abstractions;
using Xamarin.Forms;

[assembly: Dependency(typeof(Xam.Plugin.DownloadManager.WinPhone.DownloadManager))]
namespace Xam.Plugin.DownloadManager.WinPhone
{
    public class DownloadManager : IDownloadManager
    {

        private CancellationTokenSource cts;

        public static void Init()
        {
        }
        public DownloadManager()
        {
            cts = new CancellationTokenSource();
        }

        public async void Download(string uri, string filename)
        {
            Uri source;
            if (!Uri.TryCreate(uri, UriKind.Absolute, out source))
            {
                return;
            }

            string destination = filename.Trim();

            if (string.IsNullOrWhiteSpace(destination))
            {
                return;
            }

            StorageFile destinationFile;
            try
            {
                destinationFile = await KnownFolders.PicturesLibrary.CreateFileAsync(
                    destination, CreationCollisionOption.GenerateUniqueName);
            }
            catch (FileNotFoundException ex)
            {
                return;
            }

            BackgroundDownloader downloader = new BackgroundDownloader();
            DownloadOperation download = downloader.CreateDownload(source, destinationFile);


            download.Priority = BackgroundTransferPriority.Default;

            List<DownloadOperation> requestOperations = new List<DownloadOperation>();
            requestOperations.Add(download);

            // If the app isn't actively being used, at some point the system may slow down or pause long running 
            // downloads. The purpose of this behavior is to increase the device's battery life. 
            // By requesting unconstrained downloads, the app can request the system to not suspend any of the 
            // downloads in the list for power saving reasons. 
            // Use this API with caution since it not only may reduce battery life, but it may show a prompt to 
            // the user. 
            UnconstrainedTransferRequestResult result;
            try
            {
                result = await BackgroundDownloader.RequestUnconstrainedDownloadsAsync(requestOperations);
            }
            catch (NotImplementedException)
            {
                return;
            }


            await HandleDownloadAsync(download, true);
        }

        private async Task HandleDownloadAsync(DownloadOperation download, bool start)
        {
            try
            {
                Progress<DownloadOperation> progressCallback = new Progress<DownloadOperation>(DownloadProgress);

                if (start)
                {
                    // Start the download and attach a progress handler. 
                    await download.StartAsync().AsTask(cts.Token, progressCallback);
                }
                else
                {
                    // The download was already running when the application started, re-attach the progress handler. 
                    await download.AttachAsync().AsTask(cts.Token, progressCallback);
                }

                ResponseInformation response = download.GetResponseInformation();

            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
        }

        private void DownloadProgress(DownloadOperation download)
        {
            double percent = 100;
            if (download.Progress.TotalBytesToReceive > 0)
            {
                percent = download.Progress.BytesReceived * 100 / download.Progress.TotalBytesToReceive;
            }

            if (percent == 100)
            {
                MessageDialog msgbox = new MessageDialog("Download Complete");
                //Calling the Show method of MessageDialog class  
                //which will show the MessageBox  
                msgbox.ShowAsync();
            }

            if (download.Progress.HasRestarted)
            {
                MessageDialog msgbox = new MessageDialog("DownloadRestart");
                //Calling the Show method of MessageDialog class  
                //which will show the MessageBox  
                msgbox.ShowAsync();
            }

            if (download.Progress.HasResponseChanged)
            {
                // We've received new response headers from the server. 
                MessageDialog msgbox = new MessageDialog(" - Response updated; Header count: " + download.GetResponseInformation().Headers.Count);
                msgbox.ShowAsync();

            }


        }
    }
}
