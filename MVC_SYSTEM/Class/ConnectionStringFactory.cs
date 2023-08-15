using System;
using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.Class
{
    public class ConnectionStringFactory
    {
        internal static string BuildModelConnectionString(string connectionString)
        {
            var builder = new EntityConnectionStringBuilder
            {
                Provider = "System.Data.SqlClient",
                Metadata = @"your metadata string",
                ProviderConnectionString = connectionString
            };
            return builder.ConnectionString;
        }
    }
}