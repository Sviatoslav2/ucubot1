using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using ucubot.Model;



namespace ucubot.Controllers
{
    [Route("api/[controller]")]
    public class LessonSignalEndpointController : Controller
    {
        private readonly IConfiguration _configuration;

        public LessonSignalEndpointController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IEnumerable<LessonSignalDto> ShowSignals()
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var query = "SELECT * FROM lesson_signal;";
            var conn = new MySqlConnection(connectionString); 
            var dataTable = new DataTable();
            var cmd = new MySqlCommand(query, conn);
        
            var da = new MySqlDataAdapter(cmd);
            try
            {
                conn.Open();
                da.Fill(dataTable);
                conn.Close();
            }
            catch
            {
                return null;
            }
            var enumerable = new List<LessonSignalDto>();            
            foreach(DataRow row in dataTable.Rows)
            {
                var obj = new LessonSignalDto
                {
                    Id = (int)row["id"],
                    UserId = (string)row["user_id"],
                    Type = (LessonSignalType)row["signal_type"],
                    Timestamp = (DateTime)row["time_stamp"]
                };
                enumerable.Add(obj);
            }
            return enumerable;
        }

        
        

        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var query = "SELECT * FROM lesson_signal WHERE id=@id;";
            var conn = new MySqlConnection(connectionString); 
            var dataTable = new DataTable();
            var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.Add("@id", id);
       
            var da = new MySqlDataAdapter(cmd);
            try
            {
                conn.Open();
                da.Fill(dataTable);
                conn.Close();
            }
            catch
            {
                return null;
            }
            var enumerable = new List<LessonSignalDto>();
            foreach(DataRow row in dataTable.Rows)
            {
                var obj = new LessonSignalDto
                {
                    Id = (int)row["id"],
                    UserId = (string)row["user_id"],
                    Type = (LessonSignalType)row["signal_type"],
                    Timestamp = (DateTime)row["time_stamp"]
                };
                enumerable.Add(obj);
            }
            if (enumerable.Count < 1)
            {
                return null;
            }
            
            return enumerable[0];
        }

        

        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();
            var connectionString = _configuration.GetConnectionString("BotDatabase");             
            string query = "insert into lesson_signal(signal_type, user_id, time_stamp) " +
                           "values(@signal_type, @user_id, @time_stamp);";

            var conn = new MySqlConnection(connectionString);        
            var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.Add("@signal_type", signalType);
            cmd.Parameters.Add("@user_id", userId);
        
            cmd.Parameters.Add("@time_stamp",DateTime.Now);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            return Accepted();
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            //TODO: add delete command to remove signal
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var query = "DELETE FROM lesson_signal WHERE id=@id;";
            var conn = new MySqlConnection(connectionString);
            var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("id", id);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            return Accepted();
        }
    }
}
