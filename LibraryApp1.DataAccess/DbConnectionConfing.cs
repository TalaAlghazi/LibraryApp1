namespace LibraryApp1.DataAccess
{
    public static class DbConnectionConfig
    {
        private const string EnvironmentVariableName = "LIBRARYAPP1_CONNECTION_STRING";

        private const string DefaultConnectionString =
            @"Server=(localdb)\MSSQLLocalDB;Database=LibraryApp1;Trusted_Connection=True;TrustServerCertificate=True;";

        public static string GetConnectionString()
        {
            var fromEnvironment = Environment.GetEnvironmentVariable(EnvironmentVariableName);
            return string.IsNullOrWhiteSpace(fromEnvironment) ? DefaultConnectionString : fromEnvironment;
        }
    }
}
