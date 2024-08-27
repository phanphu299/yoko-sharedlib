using System;
using System.Data.Common;

namespace AHI.Infrastructure.AzureCoapTriggerExtension.Bindings
{
    public class CoapConnectionString
    {
        private const int DetaultCoapPort = 5683;
        private const string KeyForServer = nameof(Server);
        private const string KeyForUsername = nameof(Username);
        private const string KeyForPassword = nameof(Password);
        private const string KeyForQos = nameof(Qos);

        private readonly DbConnectionStringBuilder _connectionStringBuilder;

        public CoapConnectionString(string connectionString)
        {
            _connectionStringBuilder = new DbConnectionStringBuilder()
            {
                ConnectionString = connectionString
            };
            ParseAndSetServer();
        }

        public string Server { get; private set; }

        public string Username => _connectionStringBuilder.TryGetValue(KeyForUsername, out var userNameValue)
                ? userNameValue.ToString()
                : null;

        public string Password => _connectionStringBuilder.TryGetValue(KeyForPassword, out var passwordValue)
                ? passwordValue.ToString()
                : null;

        public string Qos => _connectionStringBuilder.TryGetValue(KeyForQos, out var qosValue)
               ? qosValue.ToString()
               : null;

        private void ParseAndSetServer()
        {
            Server = _connectionStringBuilder.TryGetValue(KeyForServer, out var serverValue)
                ? serverValue.ToString()
                : throw new Exception($"No server hostname configured for connection, please which connectionstring to use via the CoapTriggerAttribute, using the application settings via the Azure Portal or using the local.settings.json and then include the 'Server=' part in the connectionstring.");
        }
    }
}
