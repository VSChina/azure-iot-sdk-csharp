// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Azure.Devices.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Devices.E2ETests
{
    [TestClass]
    public class DeviceTokenRefreshE2ETests
    {
        private const string DevicePrefix = "E2E_Message_TokenRefresh_";

        [TestMethod]
        public async Task DeviceClient_TokenIsRefreshed_Ok_Http()
        {
            await DeviceClient_TokenIsRefreshed_Ok_Internal(Client.TransportType.Http1);
        }

        [TestMethod]
        public async Task DeviceClient_TokenIsRefreshed_Ok_Amqp()
        {
            await DeviceClient_TokenIsRefreshed_Ok_Internal(Client.TransportType.Amqp);
        }

        [TestMethod]
        public async Task DeviceClient_TokenIsRefreshed_Ok_Mqtt()
        {
            await DeviceClient_TokenIsRefreshed_Ok_Internal(Client.TransportType.Mqtt);
        }

        private async Task DeviceClient_TokenIsRefreshed_Ok_Internal(Client.TransportType transport)
        {
            var builder = IotHubConnectionStringBuilder.Create(Configuration.IoTHub.ConnectionString);

            RegistryManager rm = await TestUtil.GetRegistryManagerAsync(DevicePrefix);
            int ttl = 2;
            int buffer = 50;

            try
            {
                Device device = await CreateDeviceClientAsync(rm);
                                
                var refresher = new TestTokenRefresher(
                    device.Id, 
                    device.Authentication.SymmetricKey.PrimaryKey, 
                    ttl, 
                    buffer);

                DeviceClient deviceClient = 
                    DeviceClient.Create(builder.HostName, refresher, transport);

                var message = new Client.Message(Encoding.UTF8.GetBytes("Hello"));

                // Create the first Token.
                Debug.WriteLine("OpenAsync");
                await deviceClient.OpenAsync();
                Debug.WriteLine("SendEventAsync (1)");
                await deviceClient.SendEventAsync(message);
                Assert.AreEqual(1, refresher.SafeCreateNewTokenCallCount);

                Debug.WriteLine($"Waiting {ttl} seconds.");
                // Wait for the Token to expire.
                await Task.Delay(ttl * 1000);
                
                Debug.WriteLine("SendEventAsync (2)");
                await deviceClient.SendEventAsync(message);

                // Ensure that the token was refreshed.
                if (transport != Client.TransportType.Mqtt)
                {
                    // This is not currently supported for MQTT.
                    Assert.AreEqual(2, refresher.SafeCreateNewTokenCallCount);
                }

                Debug.WriteLine("CloseAsync");
                await deviceClient.CloseAsync();
            }
            finally
            {
                await TestUtil.UnInitializeEnvironment(rm);
            }
        }

        private Task<Device> CreateDeviceClientAsync(RegistryManager registryManager)
        {
            string deviceName = DevicePrefix + Guid.NewGuid();
            Debug.WriteLine($"Creating device {deviceName}");
            return registryManager.AddDeviceAsync(new Device(deviceName));
        }

        private class TestTokenRefresher : DeviceAuthenticationWithTokenRefresh
        {
            private int _callCount = 0;
            private string _key;

            public int SafeCreateNewTokenCallCount
            {
                get
                {
                    return _callCount;
                }
            }

            public TestTokenRefresher(string deviceId, string key) : base(deviceId)
            {
                _key = key;
            }

            public TestTokenRefresher(
                string deviceId, 
                string key, 
                int suggestedTimeToLive, 
                int timeBufferPercentage) 
                : base(deviceId, suggestedTimeToLive, timeBufferPercentage)
            {
                _key = key;
            }

            protected override Task<string> SafeCreateNewToken(string iotHub, int suggestedTimeToLive)
            {
                Debug.WriteLine($"Refresher: Creating new token {_callCount}");

                var builder = new SharedAccessSignatureBuilder()
                {
                    Key = _key,
                    TimeToLive = TimeSpan.FromSeconds(suggestedTimeToLive),
                    Target = string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}/devices/{1}",
                        iotHub,
                        WebUtility.UrlEncode(DeviceId)),
                };

                _callCount++;

                string token = builder.ToSignature();
                Debug.WriteLine($"Token: {token}");
                return Task.FromResult(token);
            }
        }
    }
}
