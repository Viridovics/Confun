namespace App
{
    class AppConfig
    {
        public ushort AppPort { get; set; }

        public DatabaseConnectionSection DatabaseConnection {get; set;}

        internal class DatabaseConnectionSection
        {
            public string Instance  { get; set; }
            public string User  { get; set; }
            public string Pwd  { get; set; }

        }
    }
}