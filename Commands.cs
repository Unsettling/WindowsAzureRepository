namespace AzurePatterns
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Dynamic;

    using Microsoft.WindowsAzure;

    public static class Commands
    {
        private static string connectionString;

        private static string ConnectionString
        {
            get
            {
                return connectionString
                       ?? (connectionString = CloudConfigurationManager.GetSetting("SQLConnectionString"));
            }
        }

        public static void ExecuteNonQuery(string query)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    connection.Open();

                    command.CommandText = query;

                    command.ExecuteNonQuery();

                    connection.Close();
                }
            }
        }

        public static IEnumerable<dynamic> ExecuteReader(string query)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    connection.Open();

                    command.CommandText = query;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return SqlDataReaderExpando(reader);
                        }
                    }

                    connection.Close();
                }
            }
        }

        // ExecuteReader is set up to return an enumeration of dynamic types so 
        // a value can picked out easily or an aggregates can be returned.
        // Often a particular entity will be required which is when GetEntity is used.
        public static IEnumerable<T> GetEntity<T>(IEnumerable<dynamic> data) where T : new()
        {
            var properties = typeof(T).GetProperties();

            foreach (dynamic row in data)
            {
                var element = new T();

                foreach (var property in properties)
                {
                    var next = row[property.Name];

                    if (next.GetType() != typeof(DBNull))
                    {
                        property.SetValue(element, next, null);
                    }

                    yield return element;
                }
            }
        }

        private static dynamic SqlDataReaderExpando(SqlDataReader reader)
        {
            var expandoObject = new ExpandoObject() as IDictionary<string, object>;

            for (int i = 0; i < reader.FieldCount; i++)
            {
                expandoObject.Add(reader.GetName(i), reader[i]);
            }

            return expandoObject;
        }
    }
}
