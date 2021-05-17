using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AForge.Video;
using AForge.Video.DirectShow;
using NAudio.Wave;
using NetMQMedia.Models;
//using NAudio.Wave;

namespace NetMQMedia.Services
{
    public delegate void CameraStreaming(string data);
    public delegate void AudioStreaming(string data, WaveFormat format);
    public class MediaService
    {
        public event CameraStreaming CameraStreaming;
        public event AudioStreaming AudioStreaming;
        private FilterInfoCollection filterVedio;
        private FilterInfoCollection filterAudio;
        private VideoCaptureDevice capture;
        private WaveIn wave;
        private const int bitsPerSample = 16;
        private int sampleRate = 44100;
        //private WaveIn wave
        //private Captu
        public Camera SelectedCamera { get; set; }
        public Audio SelectedAudio { get; set; }
        public MediaService()
        {
            filterAudio = new FilterInfoCollection(FilterCategory.AudioInputDevice);
            filterVedio = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            wave = new WaveIn();            
        }
        public void GetMediaInputs<T>(ref List<T> medias)
        {
            if (typeof(T) == typeof(Camera))
            {
                medias = filterVedio.Cast<FilterInfo>()
                    .Select(s => new Camera
                    { 
                        MonikerString = s.MonikerString, 
                        Name = s.Name 
                    }).ToList() as List<T>;
            }
            else if (typeof(T) == typeof(Audio))
            {
                var audios = new List<Audio>();
                for (int i = 0; i < WaveIn.DeviceCount; i++)
                {
                    var input = WaveIn.GetCapabilities(i);
                    audios.Add(new Audio
                    {
                        DevNumber = i,
                        Name=input.ProductName,
                        MonikerString= $"@device:cm:{{{input.ProductGuid}}}\\wave:{{{input.ManufacturerGuid}}}",
                        Channels = input.Channels,
                    }) ;
                }
                medias = audios as List<T>;
            }
        }
        public void StartCameraStream(Camera camera)
        {
            if (capture != null)
            {
                capture.Stop();
            }
            capture = new VideoCaptureDevice(camera.MonikerString);                    
            capture.NewFrame += Capture_NewFrame;            
            capture.Start();
        }
        public void StopCameraStream()
        {
            if (capture != null)            
                capture.Stop();            
        }
        public void StartAudioStream(Audio audio)
        {
            wave.DeviceNumber = audio.DevNumber;            
            int blockAlign = (audio.Channels * (bitsPerSample / 8));            
            int averageBytesPerSecond = sampleRate * blockAlign;
            var waveFormat = WaveFormat.CreateCustomFormat(WaveFormatEncoding.Pcm, sampleRate, audio.Channels, averageBytesPerSecond, blockAlign, bitsPerSample);
            wave.WaveFormat = waveFormat;
            wave.DataAvailable += Wave_DataAvailable;
            wave.StartRecording();
        }
        public void StopAudioStream()
        {
            if(wave!=null)
                wave.StopRecording();
        }

        //Events
        private void Wave_DataAvailable(object sender, WaveInEventArgs e)
        {
            var str = Convert.ToBase64String(e.Buffer);
            AudioStreaming(str,wave.WaveFormat);
        }
        private void Capture_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {            
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            eventArgs.Frame.Save(ms, ImageFormat.Jpeg);
            byte[] byteImage = ms.ToArray();
            var SigBase64 = Convert.ToBase64String(byteImage);
            CameraStreaming(SigBase64);
        }
    }
}
