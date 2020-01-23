using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProcessNote
{
    public partial class Form1 : Form
    {
        List<Process> Processes { get; set; }
        ListBox listBox;
        Label infoLabel;
        Font defaultFont = new Font("Arial", 20);
        public Form1()
        {
            Processes = new List<Process>();
            CreateComponents();
            InitializeComponent();
        }

        void CreateComponents()
        {
            FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel();
            flowLayoutPanel.AutoSize = true;
            flowLayoutPanel.Anchor = AnchorStyles.Left;
            flowLayoutPanel.Dock = DockStyle.Fill;

            listBox = new ListBox();
            listBox.Size = new System.Drawing.Size(this.Width, this.Height);
            listBox.MouseDoubleClick += listBox_MouseDoubleClick;
            listBox.MouseDown += listBox_MouseClick;

            Button button = new Button();
            button.Click += button_Click;   //event
            button.Font = defaultFont;
            button.AutoSize = true;
            button.Text = "Refresh";

            infoLabel = new Label();
            infoLabel.Text = "Process info";
            infoLabel.Size = new System.Drawing.Size(this.Width, (int)(this.Height * 1.5));
            infoLabel.Font = defaultFont;

            flowLayoutPanel.Controls.Add(listBox);
            flowLayoutPanel.Controls.Add(infoLabel);
            flowLayoutPanel.Controls.Add(button);
            this.Controls.Add(flowLayoutPanel);
        }

        void LoadProcesses()
        {
            Processes.Clear();
            listBox.Items.Clear();
            foreach (var process in Process.GetProcesses())
            {
                Processes.Add(process);
                listBox.Items.Add(String.Format("{0}: {1}", process.Id, process.ProcessName));
            }
        }

        void button_Click(object sender, EventArgs e)
        {
            LoadProcesses();
            //MessageBox.Show("Refreshed!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void listBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listBox.Items.Count != 0)
                RefreshAttributes();
        }

        private void listBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (listBox.Items.Count != 0)
                RefreshAttributes();
        }

        void RefreshAttributes()
        {
            Process selectedProcess = Processes[listBox.SelectedIndex];
            PerformanceCounter CPUCounter = new PerformanceCounter("Process", "% Processor Time", selectedProcess.ProcessName);
            PerformanceCounter MEMCounter = new PerformanceCounter("Process", "Working Set", selectedProcess.ProcessName);
            TimeSpan runTime = new TimeSpan();
            CPUCounter.NextValue();
            Thread.Sleep(1000);
            float processUsage = CPUCounter.NextValue() / Environment.ProcessorCount;
            float memoryUsage = MEMCounter.NextValue() / 1024 / 1024;
            try
            {
                runTime = DateTime.Now - selectedProcess.StartTime;
                infoLabel.Text = $"Process ID: {selectedProcess.Id}\nProcess name: {selectedProcess.ProcessName}\nStart time: {selectedProcess.StartTime.ToShortTimeString()}\nRunning time: {String.Format("{0:00}:{1:00}:{2:00}", runTime.Hours, runTime.Minutes, runTime.Seconds)}\nCPU usage: {processUsage.ToString("0.0")}%\nMemory usage: {memoryUsage.ToString("0.000")} MB";
            }
            catch
            {
                infoLabel.Text = "Process is protected.";
            }
            //MessageBox.Show(String.Format($"Process ID: {selectedProcess.Id}\nProcess name: {selectedProcess.ProcessName}\nStart time: {selectedProcess.StartTime.ToShortTimeString()}\nRunning time: {String.Format("{0:00}:{1:00}:{2:00}", runTime.Hours, runTime.Minutes, runTime.Seconds)}\nCPU usage: {processUsage.ToString("0.0")}%\nMemory usage: {memoryUsage.ToString("0.000")} MB"), "Process info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
