using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace App
{
    class Program
    {
        static void Main(string[] args)
        {
            var configText = System.IO.File.ReadAllText(@"Configs\app.config.json");
            var config = JsonConvert.DeserializeObject<AppConfig>(configText);
            StartService(config.AppPort);
            ConnectToDb(config.DatabaseConnection);
        }

        static void StartService(ushort port)
        {
            Console.WriteLine($"Service 'App' is started on port: '{port}'");
        }

        static void ConnectToDb(AppConfig.DatabaseConnectionSection connection)
        {
            Console.WriteLine($"Connected to database'{connection.Instance}' with user '{connection.User}'");
        }
    }
}
