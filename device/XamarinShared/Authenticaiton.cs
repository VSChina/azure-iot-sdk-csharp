using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XamarinShared
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
