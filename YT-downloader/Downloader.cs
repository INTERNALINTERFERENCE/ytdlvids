using FFmpeg.NET;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace YT_downloader
{
    public class Downloader
    {
        private string newPath = null;
        private string newLink = $@"{Environment.CurrentDirectory}\Temp";
        private YoutubeClient ytClient = new YoutubeClient();

        private void Manifest(string path)
        {
            var pos = path.LastIndexOf('=') + 1;
            newPath = path.Substring(pos);
        }
        
        public async void DownloadVideo720P(string path, ProgressBar progressBar, string link)
        {
            var progressHandler = new Progress<double>(p => progressBar.Value = (int)(p * 100));
            Manifest(path);
            var streamManifest = await ytClient.Videos.Streams.GetManifestAsync(newPath);

            var streamInfo = streamManifest.GetMuxed().WithHighestVideoQuality();
            if (streamInfo != null)
                await ytClient.Videos.Streams.DownloadAsync(streamInfo, Path.Combine(link, $"video.{streamInfo.Container.Name}"), progressHandler);
        }

        public async void DownloadVideo1080P(string path, ProgressBar progressBar, string link)
        {
            var progressHandler = new Progress<double>(p => progressBar.Value = (int)(p * 100));
            Manifest(path);           
            var streamManifest = await ytClient.Videos.Streams.GetManifestAsync(newPath);
            var video = await ytClient.Videos.GetAsync(path);


            var videoInfo = streamManifest.GetVideoOnly()
                .Where(s => s.Container == Container.Mp4).WithHighestVideoQuality();

            var audioInfo = streamManifest.GetAudioOnly().WithHighestBitrate();
           
            if (videoInfo != null && audioInfo != null)
            {
                await ytClient.Videos.Streams.DownloadAsync(audioInfo, Path.Combine(newLink, $"audio.wav"));
                await ytClient.Videos.Streams.DownloadAsync(videoInfo, Path.Combine(newLink, $"video.mp4"), progressHandler);
                await Task.Run(() => MergeAudioVideo(link));
            }
        }           

        private async void MergeAudioVideo(string link)
        {
            var ffmpeg = new Engine($@"{Environment.CurrentDirectory}\Engine\ffmpeg.exe");
            await ffmpeg.ExecuteAsync($@"-i {Environment.CurrentDirectory}\Temp\video.mp4  -i {Environment.CurrentDirectory}\Temp\audio.wav -c:v copy -c:a aac -map 0:v:0 -map 1:a:0 -shortest {link}\video.mp4");
            ClearTemp();
        }

        private void ClearTemp()
        {
            var folder = Directory.GetFiles($@"{Environment.CurrentDirectory}\Temp\");
            foreach(var files in folder)
                File.Delete(files);
        }
    }
}
