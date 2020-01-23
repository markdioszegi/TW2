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
using System.Timers;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace ProcessNote
{
    public partial class Form1 : Form
    {
        List<Process> Processes { get; set; }
        ListBox listBox;
        DataGridView dataGridView;
        CheckBox checkBox;
        CheckBox modeCheckBox;
        Label infoLabel;
        Button refreshButton;
        Timer refreshTimer;
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

            /* listBox = new ListBox();
            listBox.Size = new Size(this.Width, this.Height);
            listBox.MouseDoubleClick += listBox_MouseDoubleClick;
            listBox.MouseDown += listBox_MouseClick; */

            dataGridView = new DataGridView();
            dataGridView.Size = new Size(this.Width * 2, this.Height);
            dataGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            dataGridView.RowHeadersVisible = false;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.MultiSelect = false;
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.AllowUserToResizeColumns = false;
            dataGridView.ReadOnly = true;
            dataGridView.ColumnCount = 3;
            dataGridView.Columns[0].Name = "Process ID";
            dataGridView.Columns[1].Name = "Process Name";
            dataGridView.Columns[2].Name = "Comment";
            dataGridView.CellClick += dataGridView_CellClick;

            refreshButton = new Button();
            refreshButton.Click += refreshButton_Click;   //event
            refreshButton.Font = defaultFont;
            refreshButton.AutoSize = true;
            refreshButton.Text = "Refresh";

            checkBox = new CheckBox();
            checkBox.Font = defaultFont;
            checkBox.Text = "Always on top";
            checkBox.AutoSize = true;
            checkBox.CheckedChanged += checkBox_Checked;

            modeCheckBox = new CheckBox();
            //modeCheckBox.Appearance = Appearance.Button;
            modeCheckBox.Font = defaultFont;
            modeCheckBox.Text = "Online mode";
            modeCheckBox.AutoSize = true;
            modeCheckBox.CheckedChanged += modeCheckBox_Checked;


            infoLabel = new Label();
            infoLabel.Text = "Process info";
            infoLabel.Size = new System.Drawing.Size(this.Width, (int)(this.Height * 1.5));
            infoLabel.Font = defaultFont;

            flowLayoutPanel.Controls.Add(dataGridView);
            //flowLayoutPanel.Controls.Add(listBox);
            flowLayoutPanel.Controls.Add(infoLabel);
            flowLayoutPanel.Controls.Add(refreshButton);
            flowLayoutPanel.Controls.Add(checkBox);
            flowLayoutPanel.Controls.Add(modeCheckBox);
            this.Controls.Add(flowLayoutPanel);
        }

        void LoadProcesses()
        {
            Processes.Clear();
            dataGridView.Rows.Clear();
            //listBox.Items.Clear();
            foreach (var process in Process.GetProcesses())
            {
                Processes.Add(process);
                dataGridView.Rows.Add(process.Id, process.ProcessName);
                //listBox.Items.Add(String.Format("{0}: {1}", process.Id, process.ProcessName));
            }
        }

        void refreshButton_Click(object sender, EventArgs e)
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

        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //MessageBox.Show("rindex: " + e.RowIndex);
            if (e.RowIndex >= 0 && Processes.Count > 0)
                RefreshAttributes();
        }

        void RefreshAttributes()
        {
            //Process selectedProcess = Processes[dataGridView.SelectedCells[0].Value];
            Process selectedProcess = Processes.Find(i => i.Id == (int)dataGridView.SelectedRows[0].Cells[0].Value);
            PerformanceCounter CPUCounter = new PerformanceCounter("Process", "% Processor Time", selectedProcess.ProcessName);
            PerformanceCounter MEMCounter = new PerformanceCounter("Process", "Working Set", selectedProcess.ProcessName);
            TimeSpan runTime = new TimeSpan();
            CPUCounter.NextValue();
            //Thread.Sleep(1000);
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
            //MessageBox.Show($"{dataGridView.SelectedCells[0].OwningColumn.Name}", "info");
        }
        void checkBox_Checked(object sender, EventArgs e)
        {
            if (checkBox.Checked)
            {
                MessageBox.Show("Always on top!");
                this.TopMost = true;
            }
            else
            {
                this.TopMost = false;
            }
        }
        void modeCheckBox_Checked(object sender, EventArgs e)
        {
            if (modeCheckBox.Checked)
            {
                refreshTimer = new Timer();
                refreshTimer.Interval = 1500;
                refreshTimer.Tick += refreshTimerElapsed;
                refreshTimer.Start();
            }
            else
            {
                refreshTimer.Stop();
            }
        }

        void refreshTimerElapsed(object sender, EventArgs e)
        {
            LoadProcesses();
            RefreshAttributes();
        }
    }
}
