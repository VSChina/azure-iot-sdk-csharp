using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;
using System;

namespace test
{
    class Program
    {

        static void Main(string[] args)
        {
            MainAsync(args);
        }
        static async void MainAsync(string[] args)
        {
            string connectionString = "HostName=jasminel-h1.azure-devices.net;DeviceId=DeviceId_10109;SharedAccessKey=P61ZA8kaPcJAEJVIpG9eMuEfZvPuHqatbet0spv5Fg8=;";

            var settings = new ITransportSettings[] {
                new MqttTransportSettings(TransportType.Mqtt_Tcp_Only)
            };

            var settings2 = new ITransportSettings[] {
                new AmqpTransportSettings(TransportType.Amqp_Tcp_Only)
            };

            var settings3 = new ITransportSettings[] {
                new AmqpTransportSettings(TransportType.Amqp_Tcp_Only)
                {
                    AmqpConnectionPoolSettings = new AmqpConnectionPoolSettings()
                    {
                        Pooling = true,
                        MaxPoolSize = 10
                    }
                }
            };

            var settings4 = new ITransportSettings[] {
                new AmqpTransportSettings(TransportType.Amqp_WebSocket_Only)
                {
                    AmqpConnectionPoolSettings = new AmqpConnectionPoolSettings()
                    {
                        Pooling = true,
                        MaxPoolSize = 10
                    }
                }
            };

            DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(connectionString, settings4);
            deviceClient.RetryPolicy = RetryPolicyType.Exponential_Backoff_With_Jitter;
            deviceClient.OpenAsync().Wait();
            Console.WriteLine("Opened a connection to IoT Hub");
            deviceClient.CloseAsync().Wait();
        }
    }
}