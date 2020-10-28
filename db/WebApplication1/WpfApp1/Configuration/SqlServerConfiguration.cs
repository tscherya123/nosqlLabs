using Data.Settings;

namespace WpfApp1.Configuration
{
    public static class SqlServerConfiguration
    {
        private const string DataSource = "qweasdx.database.windows.net";
        private const string InitialCatalog = "DB";
        private const string UserID = "Admin_";
        private const string Password = "1234567890Aa]";

        public static SqlServerConntectionSettings Settings { 
            get
            {
                return new SqlServerConntectionSettings()
                {
                    DataSource = DataSource,
                    InitialCatalog = InitialCatalog,
                    UserID = UserID,
                    Password = Password
                };
            }
        }
    }
}
