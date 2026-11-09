using Newtonsoft.Json.Linq;
using NUnit.Framework;
using OmiLAXR.HypeRate.SDK;
using UnityEngine;

namespace OmiLAXR.HypeRate.Tests
{
    public class HypeRateSdk_Tests
    {
        [Test]
        public void Channels_Join_Tracks_Channel_Until_Joined()
        {
            // Arrange
            var channels = new Channels();

            // Act
            var joinRef = channels.AddJoiningChannel("hr:device");

            // Assert
            Assert.Greater(joinRef, 0);
            Assert.AreEqual(RefType.Join, channels.DetermineRefType(joinRef));
            Assert.AreEqual(-1, channels.AddJoiningChannel("hr:device"));
            CollectionAssert.AreEqual(new[] { "hr:device" }, channels.GetChannelsToJoin());

            // Act
            channels.HandleJoin(joinRef);

            // Assert
            Assert.AreEqual(RefType.Unknown, channels.DetermineRefType(joinRef));
            Assert.AreEqual(-1, channels.AddJoiningChannel("hr:device"));
            CollectionAssert.AreEqual(new[] { "hr:device" }, channels.GetChannelsToJoin());
        }

        [Test]
        public void Channels_HandleChannelJoin_Moves_Channel_To_Joined()
        {
            // Arrange
            var channels = new Channels();
            var joinRef = channels.AddJoiningChannel("clips:device");

            // Act
            channels.HandleChannelJoin(joinRef);
            channels.HandleChannelJoin(12345);

            // Assert
            StringAssert.Contains("joined: [ clips:device ]", channels.ToString());
        }

        [Test]
        public void Channels_Leaving_Channel_Is_Not_Rejoined()
        {
            // Arrange
            var channels = new Channels();
            channels.HandleJoin(channels.AddJoiningChannel("hr:a"));
            channels.HandleJoin(channels.AddJoiningChannel("hr:b"));

            // Act
            var leaveRef = channels.AddLeavingChannel("hr:a");

            // Assert
            Assert.AreEqual(RefType.Leave, channels.DetermineRefType(leaveRef));
            CollectionAssert.AreEqual(new[] { "hr:b" }, channels.GetChannelsToJoin());

            // Act
            channels.HandleLeave(leaveRef);

            // Assert
            Assert.AreEqual(RefType.Unknown, channels.DetermineRefType(leaveRef));
        }

        [Test]
        public void Channels_Joining_A_Leaving_Channel_Cancels_Leave()
        {
            // Arrange
            var channels = new Channels();
            var leaveRef = channels.AddLeavingChannel("hr:a");

            // Act
            var joinRef = channels.AddJoiningChannel("hr:a");

            // Assert
            Assert.AreEqual(RefType.Unknown, channels.DetermineRefType(leaveRef));
            Assert.AreEqual(RefType.Join, channels.DetermineRefType(joinRef));
        }

        [Test]
        public void Channels_HandleReconnect_Clears_All_State()
        {
            // Arrange
            var channels = new Channels();
            var joinRef = channels.AddJoiningChannel("hr:a");
            channels.AddLeavingChannel("hr:b");

            // Act
            channels.HandleReconnect();

            // Assert
            Assert.IsEmpty(channels.GetChannelsToJoin());
            Assert.AreEqual(RefType.Unknown, channels.DetermineRefType(joinRef));
            Assert.AreEqual("Channels { joining: [  ], joined: [  ], leaving: [  ]}", channels.ToString());
        }

        [Test]
        public void Network_Packets_Are_Valid_Phoenix_Messages()
        {
            // Act
            var join = JObject.Parse(Network.GetJoinPacket("hr:device", 7));
            var leave = JObject.Parse(Network.GetLeavePacket("hr:device", 8));
            var keepAlive = JObject.Parse(Network.GetKeepAlivePacket());

            // Assert
            Assert.AreEqual("hr:device", (string)join["topic"]);
            Assert.AreEqual("phx_join", (string)join["event"]);
            Assert.AreEqual(7, (int)join["ref"]);
            Assert.AreEqual("phx_leave", (string)leave["event"]);
            Assert.AreEqual(8, (int)leave["ref"]);
            Assert.AreEqual("phoenix", (string)keepAlive["topic"]);
            Assert.AreEqual("heartbeat", (string)keepAlive["event"]);
        }

        [Test]
        public void HypeRate_Channel_Name_Helpers()
        {
            // Assert
            Assert.AreEqual(ChannelType.Heartrate, SDK.HypeRate.DetermineChannelType("hr:abc"));
            Assert.AreEqual(ChannelType.Clip, SDK.HypeRate.DetermineChannelType("clips:abc"));
            Assert.AreEqual(ChannelType.Unknown, SDK.HypeRate.DetermineChannelType("other"));
            Assert.AreEqual("abc", SDK.HypeRate.ExtractDeviceIdFromChannelName("hr:abc"));
            Assert.AreEqual("abc", SDK.HypeRate.ExtractDeviceIdFromChannelName("clips:abc"));
            Assert.AreEqual("other", SDK.HypeRate.ExtractDeviceIdFromChannelName("other"));
        }

        [Test]
        public void HypeRate_Is_Singleton()
        {
            Assert.AreSame(SDK.HypeRate.GetInstance(), SDK.HypeRate.GetInstance());
        }

        [Test]
        public void HypeRateDataPackage_Parses_Heart_Rate_Update()
        {
            // Act
            var package = JsonUtility.FromJson<HypeRateDataPackage>(
                "{\"event\":\"hr_update\",\"payload\":{\"hr\":\"87\",\"status\":\"ok\"},\"topic\":\"hr:abc\"}");

            // Assert
            Assert.AreEqual("hr_update", package.@event);
            Assert.AreEqual("87", package.payload.hr);
            Assert.AreEqual("ok", package.payload.status);
        }

        [Test]
        public void HypeRateProvider_Returns_Last_Heart_Rate()
        {
            // Arrange (kept inactive so Start() does not open a network connection)
            var go = new GameObject("HypeRate");
            go.SetActive(false);
            var provider = go.AddComponent<HypeRateProvider>();

            // Act
            provider.heartRate = 72;

            // Assert
            Assert.AreEqual(72, provider.GetHeartRate());
            Object.DestroyImmediate(go);
        }
    }
}
