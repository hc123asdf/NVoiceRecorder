using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Lame;
using NAudio.Wave;
namespace VoiceRecoder
{
    class NAudioRecordHelper
    {
        public enum RecordType
        {
            loudspeaker = 0, // 扬声器
            microphone = 1 //麦克风
        }

        //录制的类型
        RecordType t = RecordType.microphone;
        public DateTime recoderTime;
        //录制麦克风的声音
        WaveInEvent waveIn = null; //new WaveInEvent();
        //录制扬声器的声音
        WasapiLoopbackCapture capture = null; //new WasapiLoopbackCapture();
        //生成音频文件的对象
        WaveFileWriter writer = null;
        LameMP3FileWriter mp3writer = null;
        string audioFile = "";

        int deviceNo;

        public NAudioRecordHelper(RecordType x, string filePath,int deviceNo)
        {
            t = x;
            audioFile = filePath;
           this. deviceNo = deviceNo;
        }

        /// <summary>
        /// 开始录制
        /// </summary>
        public void StartRecordAudio()
        {
            try
            {
                if (audioFile == "")
                {
                    System.Windows.Forms.MessageBox.Show("请设置录制文件的路径！");
                    return;
                }
                recoderTime = DateTime.Now;
                if (t == RecordType.microphone)
                {
                    waveIn = new WaveInEvent();
                    waveIn.DeviceNumber = this.deviceNo;
                    writer = new WaveFileWriter(audioFile, waveIn.WaveFormat);
                    //开始录音，写数据
                    waveIn.DataAvailable += (s, a) =>
                    {
                        writer.Write(a.Buffer, 0, a.BytesRecorded);
                    };

                    //结束录音
                    waveIn.RecordingStopped += (s, a) =>
                    {
                        writer.Dispose();
                        writer = null;
                        waveIn.Dispose();

                    };


                    waveIn.StartRecording();
                }
                else
                {
                      capture = new WasapiLoopbackCapture();

                    mp3writer = new LameMP3FileWriter(audioFile, capture.WaveFormat, 32);
               
                    var s1 = capture.WaveFormat;
                    
                    capture.DataAvailable += (s, a) =>
                    {
                        mp3writer.Write(a.Buffer, 0, a.BytesRecorded);
                      //  writer.Write(a.Buffer, 0, a.BytesRecorded);
                    };
                    //结束录音
                    capture.RecordingStopped += (s, a) =>
                    {
                        mp3writer.Dispose();
                        mp3writer = null;
                        capture.Dispose();
                    };



                    capture.StartRecording();
                }

            }
            catch (Exception ex)
            {
            }
        }


        //结束录制
        public void StopRecordAudio()
        {
            if (t == RecordType.microphone)
                waveIn.StopRecording();
            else
                capture.StopRecording();
        }
    }
}
