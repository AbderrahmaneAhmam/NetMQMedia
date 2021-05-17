using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NAudio.Wave;
//using NAudio.Wave;
using NetMQMedia.Models;
using NetMQMedia.Services;

namespace NetMQCameraAudio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MediaService service = new MediaService();
        WaveOut _waveOut = new WaveOut();
        RawSourceWaveStream provider;
        public MainWindow()
        {
            InitializeComponent();
            var cameras = new List<Camera>();
            var audios = new List<Audio>();            
            service.GetMediaInputs<Camera>(ref cameras);
            service.GetMediaInputs<Audio>(ref audios);
            service.StartCameraStream(cameras[0]);
            service.StartAudioStream(audios[0]);
            service.CameraStreaming += Service_CameraStreaming;
            service.AudioStreaming += Service_AudioStreaming;                        
        }

        private void Service_AudioStreaming(string data, WaveFormat format)
        {            
            var audioBuffer = Convert.FromBase64String(data);            
            provider = new RawSourceWaveStream(new MemoryStream(audioBuffer), format);
            _waveOut.Init(provider);
            _waveOut.Play();            
        }

        private void Service_CameraStreaming(string data)
        {            
            var bytes = Convert.FromBase64String(data);
            
            using (var stream = new MemoryStream(bytes))
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    imgStr.Source = BitmapFrame.Create(stream,
                    BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                });
            }
        }
    }
}
