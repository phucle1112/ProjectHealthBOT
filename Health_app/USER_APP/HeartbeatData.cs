using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Health_app
{
    class HeartbeatData
    {
        public HeartbeatData()
        { }
        public void hearBeat()
        {           
            String username = login.username;
            Database db = new Database();
            MySqlCommand cmd = null;
            db.initConnection();
            int bpm;
            Random temp = new Random();
            bpm = temp.Next(50, 120);                                            
            String query = "Insert into heartbeat_data (username, timePeriod, BPM) VALUES (@username, @timePeriod, @BPM)";
            cmd = new MySqlCommand(query, db.GetConnection());
            cmd.Parameters.Add("@username", MySqlDbType.VarChar).Value = username;
            cmd.Parameters.Add("@timePeriod", MySqlDbType.DateTime).Value = DateTime.Now;                              
            cmd.Parameters.Add("@BPM", MySqlDbType.Int16).Value = bpm;
            cmd.ExecuteNonQuery(); 
            db.stopConnection();
        }
        
    }
}
