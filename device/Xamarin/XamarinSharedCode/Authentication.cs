using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XamarinSharedCode
{
    public class Authentication
    {
        public TransportType transport;

        public Authentication(TransportType type)
        {
            transport = type;
        }
        public DeviceClient AuthViaConnectionString(string ConnectionString)
        {
            return DeviceClient.CreateFromConnectionString(ConnectionString, transport);
        }
        public DeviceClient AuthViaSaSToken(string ConnectionString)
        {
            return DeviceClient.CreateFromConnectionString(ConnectionString, transport);
        }

    }
}
