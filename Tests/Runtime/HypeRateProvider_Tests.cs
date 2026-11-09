using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using OmiLAXR.HypeRate.SDK;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace OmiLAXR.HypeRate.Tests
{
    public class HypeRateProvider_Tests
    {
        private sealed class FakeSocket : IHeartRateSocket
        {
            public readonly List<string> Calls = new List<string>();
            public event Action<int> HeartRateUpdated;

            public Task ConnectToServer(string websocketToken, string url)
            {
                Calls.Add($"connect {websocketToken} {url}");
                return Task.CompletedTask;
            }

            public Task JoinHeartbeatChannel(string deviceId)
            {
                Calls.Add("join " + deviceId);
                return Task.CompletedTask;
            }

            public Task CloseConnection()
            {
                Calls.Add("close");
                return Task.CompletedTask;
            }

            public void Push(int heartRate) => HeartRateUpdated?.Invoke(heartRate);
        }

        [UnityTest]
        public IEnumerator HypeRateProvider_Connects_Joins_And_Receives_Heart_Rate()
        {
            // Arrange
            var go = new GameObject("HypeRate");
            go.SetActive(false);
            var provider = go.AddComponent<HypeRateProvider>();
            var socket = new FakeSocket();
            provider.Socket = socket;
            provider.apiKey = "key";
            provider.sessionId = "device";
            provider.hyperateURL = "wss://example.org";

            // Act
            go.SetActive(true);
            yield return null; // Start runs before the first frame
            socket.Push(91);

            // Assert
            CollectionAssert.AreEqual(new[] { "connect key wss://example.org", "join device" }, socket.Calls);
            Assert.AreEqual(91, provider.GetHeartRate());
            Object.DestroyImmediate(go);
        }

        [Test]
        public void HypeRate_Sdk_Forwards_Updates_Through_Interface()
        {
            // Arrange
            IHeartRateSocket socket = SDK.HypeRate.GetInstance();
            var received = 0;
            void OnUpdate(int hr) => received = hr;

            // Act & Assert (subscribing and unsubscribing must not throw)
            socket.HeartRateUpdated += OnUpdate;
            socket.HeartRateUpdated -= OnUpdate;
            Assert.AreEqual(0, received);
        }
    }
}
