using MySql.Data.MySqlClient;
using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;
using System.Windows.Forms.DataVisualization.Charting;
using Series = System.Windows.Forms.DataVisualization.Charting.Series;
using System.Drawing;

namespace Health_app
{
    public partial class Main : Form
    {
        Database db = new Database();
        MySqlCommand cmd = null;
        MySqlDataAdapter adapter = new MySqlDataAdapter();
        //HeartbeatData heartbeat = new HeartbeatData();

        //private Thread bpmThread;
        //private double[] bpmArray = new double[30];

        public Main(String username)
        {
            InitializeComponent();
            textBox_username.Text = username;
            dataGridView_info_show();
            InitChart();
        }
        // Main loader
        private void Main_Load(object sender, EventArgs e)
        {
            timer1.Start();
            label1.Text = DateTime.Now.ToLongDateString();
            label2.Text = DateTime.Now.ToLongTimeString();
            myTimer.Start();
            myTimer.Tick += new EventHandler(BPM_show);           
            myTimer.Interval = 3000;
        }


        // Clock timer for live clock
        private void Timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = DateTime.Now.ToLongTimeString();
            timer1.Start();
        }
      
        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        void dataGridView_info_show()
        {
            String query = "SELECT user_firstname, user_lastname, user_email, user_birthday, user_address, user_phonenumber FROM healthbot.user_information WHERE user_user = '" + this.textBox_username.Text + "' ;";
            cmd = new MySqlCommand(query, db.GetConnection());
            try
            {
                adapter.SelectCommand = cmd;
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                BindingSource bSource = new BindingSource();

                bSource.DataSource = dataTable;
                dataGridView_info.DataSource = bSource;
                adapter.Update(dataTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        static System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
        private void BPM_show(Object myO, EventArgs myE)
        {
            //heartbeat.hearBeat();
            db.initConnection();
            String query = "SELECT BPM FROM healthbot.heartbeat_data ORDER BY timePeriod DESC LIMIT 1;";
            cmd = new MySqlCommand(query, db.GetConnection());
            MySqlDataReader reader = cmd.ExecuteReader();                     
            while (reader.Read())
            {
            richTextBPM.Text = reader.GetValue(0).ToString();
            }
            richTextBPM.TextChanged += null;
            db.stopConnection();
        }
        Timer chartTimer = new Timer();

        private void InitChart()
        {           
            chartTimer.Interval = 3000;
            chartTimer.Tick += chartTimer_Tick;
            chart1.DoubleClick += chartDemo_DoubleClick;

            Series series = chart1.Series[0];
            series.ChartType = SeriesChartType.Spline;

            chart1.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm:ss";
            chart1.ChartAreas[0].AxisX.ScaleView.Size = 5;
            
            chartTimer.Start();
        }

        void chartDemo_DoubleClick(object sender, EventArgs e)//show the scrollBar
        {
            chart1.ChartAreas[0].AxisX.ScaleView.Size = 5;
            chart1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chart1.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
        }

        void chartTimer_Tick(object sender, EventArgs e)
        {
            db.initConnection();
            String query = "SELECT timePeriod,BPM FROM healthbot.heartbeat_data ORDER BY timePeriod DESC LIMIT 1;";
            cmd = new MySqlCommand(query, db.GetConnection());
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int bpm = reader.GetUInt16(1);
                DateTime time = DateTime.Parse(reader.GetString(0));
                String stime = time.ToString("HH:mm:ss");
                var series = chart1.Series["BPM"];
                series.Points.AddXY(stime, bpm);
                chart1.ChartAreas[0].AxisX.ScaleView.Position = series.Points.Count - 5;
            }            
            db.stopConnection();           
        }

        private void buttonShow_Click(object sender, EventArgs e)
        {
            //chart2.Series["BPM"].Points.Clear();
            dataGridView_bpm_show();
        }

        void dataGridView_bpm_show()
        {
            int i;
            String startDay = monthCalendar.SelectionRange.Start.ToString("yyyy-MM-dd");
            String endDay = monthCalendar.SelectionRange.End.ToString("yyyy-MM-dd");
            textBox1.Text = startDay;
            textBox2.Text = endDay;
            String qr = "SELECT username,timePeriod, BPM FROM healthbot.heartbeat_data WHERE username ='" + this.textBox_username.Text + "'and date(timePeriod) between '" + startDay + "' and '" + endDay + "';";
            cmd = new MySqlCommand(qr, db.GetConnection());
            try
            {
                adapter.SelectCommand = cmd;
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                BindingSource bSource = new BindingSource();
                bSource.DataSource = dataTable;
                dataGridView1.DataSource = bSource;
                adapter.Update(dataTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }       
    }
}
