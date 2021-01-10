using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using NAudio.Wave;

namespace VoiceRecoder
{
    public partial class Form1 : Form
    {
        [DllImport("winmm.dll")]
        public static extern bool PlaySound(string szSound, IntPtr hMod, int flags);
        public string filePath = "file\\";
        NAudioRecordHelper nAudioRecordHelper = null;
        public int coolDown = 0;
        //  private SoundRecord recorder = null;//录音
        public Form1()
        {
            // recorder = new SoundRecord();
            filePath = AppDomain.CurrentDomain.BaseDirectory + filePath;
            if (!System.IO.Directory.Exists(filePath))
            {
                System.IO.Directory.CreateDirectory(filePath);
            }
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.SelectedItem == null)
            {
                MessageBox.Show("请选择录音设备");
                return;
            }
            var type = this.comboBox1.SelectedItem.ToString().IndexOf("系统内声音") != -1 ? 
                NAudioRecordHelper.RecordType.loudspeaker :
                   NAudioRecordHelper.RecordType.microphone;
            nAudioRecordHelper = new NAudioRecordHelper(type, filePath+DateTime.Now.ToString("yyyyMMddHHmmss") + ".mp3", (this.comboBox1.SelectedItem as Device).Value);
            timer1.Enabled = true;
            nAudioRecordHelper.StartRecordAudio();
            numericUpDown1.Enabled = false;
            coolDown =(int) numericUpDown1.Value;
            button1.Enabled = false;
            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            nAudioRecordHelper.StopRecordAudio();
            ResetTime();
            numericUpDown1.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = false;
        }

      

        private void timer1_Tick(object sender, EventArgs e)
        {

            labTime.Text = String.Format("{0:D2}:{1:D2}.{2:D2}",
              (DateTime.Now - nAudioRecordHelper.recoderTime).Hours, (DateTime.Now - nAudioRecordHelper.recoderTime).Minutes
              , (DateTime.Now - nAudioRecordHelper.recoderTime).Seconds);
            if((double)numericUpDown1.Value == -1)
            {
                return;
            }
            if ( (DateTime.Now - nAudioRecordHelper.recoderTime).TotalSeconds >(double) numericUpDown1.Value)
            {
                button2_Click(null, null);
            }
            label4.Text = "剩余" + coolDown;
            coolDown--;
        }

        public void ResetTime()
        {
            labTime.Text = "00:00:00";

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int n = 0; n < WaveIn.DeviceCount; n++)
            {
                this.comboBox1.Items.Add(new Device { Text = WaveIn.GetCapabilities(n).ProductName, Value = n });
            }
            var capture = new WasapiLoopbackCapture();
            this.comboBox1.Items.Add(new Device() { Text = "系统内声音-" + capture.WaveFormat.Channels, Value = 99 });
        }

        public class Device
        {
            public int Value { get; set; }
            public string Text { get; set; }
            public override String ToString()
            {
                return Text;
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", filePath);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (button2.Enabled)
            {
                button2_Click(null, null);
            }
        }
    }
}
