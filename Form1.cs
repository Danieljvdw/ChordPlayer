using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Synthesis;

namespace ChordPlayer
{
    public partial class Form1 : Form
    {
        private bool propertiesLoaded = false;

        private bool active = false;
        private int counter = 0;
        private Random r = new Random();

        private System.Windows.Forms.Timer Timer = null;

        private string[] avaliableChords = { "A", "B", "C", "D", "E", "F", "G", "a", "b", "c", "d", "e", "f", "g" };

        private string currentChord = "";
        public string CurrentChord
        {
            get
            {
                return currentChord;
            }
            set
            {
                currentChord = value;
                label3.Text = currentChord;
            }
        }

        SpeechSynthesizer synth = new SpeechSynthesizer();
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (active)
            {
                button1.Text = "Start";
                active = false;
            }
            else
            {
                button1.Text = "Stop";
                active = true;
            }
        }

        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!propertiesLoaded)
                return;

            if (Properties.Settings.Default.Chords == null)
            {
                Properties.Settings.Default.Chords = new System.Collections.Specialized.StringCollection();
            }
            if (Properties.Settings.Default.ChordsChecked == null)
            {
                Properties.Settings.Default.ChordsChecked = new System.Collections.Specialized.StringCollection();
            }

            Properties.Settings.Default.Chords.Clear();

            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                if (dataGridView1.Rows[i].Cells[0].Value == null)
                    dataGridView1.Rows[i].Cells[0].Value = false;
                if (dataGridView1.Rows[i].Cells[1].Value == null)
                    dataGridView1.Rows[i].Cells[1].Value = "";

                Properties.Settings.Default.ChordsChecked.Add(dataGridView1.Rows[i].Cells[0].Value.ToString());
                Properties.Settings.Default.Chords.Add(dataGridView1.Rows[i].Cells[1].Value.ToString());
            }

            Properties.Settings.Default.Save();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            bool propertiesPopulated = false;

            if (Properties.Settings.Default.Chords != null)
            {
                int i = 0;

                for (i = 0; i < Properties.Settings.Default.Chords.Count; i++)
                {
                    bool boxChecked = (Properties.Settings.Default.ChordsChecked[i] == "True");
                    string chordLabel = Properties.Settings.Default.Chords[i];

                    dataGridView1.Rows.Add(boxChecked, chordLabel);
                }

                if(i > 0)
                {
                    propertiesPopulated = true;
                }
            }

            if(!propertiesPopulated)
            {
                loadDefaultProperties();
            }

            propertiesLoaded = true;

            if (Timer == null)
            {
                Timer = new Timer();
                Timer.Interval = 1000;
                Timer.Tick += Timer_Tick;
                Timer.Start();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!active)
                return;

            counter++;

            if(counter >= numericUpDown1.Value)
            {
                pickChord();
                counter = 0;
            }
        }

        private void pickChord()
        {
            int numberOfChords = 0;
            int chordChosen = 0;

            foreach(DataGridViewRow r in dataGridView1.Rows)
            {
                if(r.Cells[0].Value != null && (bool)r.Cells[0].Value)
                {
                    numberOfChords++;
                }
            }

            if (numberOfChords == 0)
                return;

            chordChosen = r.Next(numberOfChords) + 1;

            foreach (DataGridViewRow r in dataGridView1.Rows)
            {
                if (r.Cells[0].Value != null && (bool)r.Cells[0].Value)
                {
                    chordChosen--;
                }

                if(chordChosen == 0)
                {
                    string c = "";

                    try
                    {
                        c = (string)r.Cells[1].Value;
                    }
                    catch
                    {
                        c = Convert.ToString(r.Cells[1].Value);
                    }

                    CurrentChord = c;
                    break;
                }
            }

            Task.Run(() => synth.Speak(CurrentChord));
        }

        private void loadDefaultProperties()
        {
            dataGridView1.Rows.Clear();

            foreach (string c in avaliableChords)
            {
                dataGridView1.Rows.Add(true, c);
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            loadDefaultProperties();
        }
    }
}
