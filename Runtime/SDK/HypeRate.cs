using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace HypeRate.SDK
{
    public delegate void HeartRateUpdateHandler(int heartRate);
    public class HypeRate
    {
        private HypeRate() { }

        private static HypeRate _instance;

        private ClientWebSocket _webSocket = new ClientWebSocket();

        private Channels _channels = new();

        public event HeartRateUpdateHandler OnHeartRateUpdate;
        public event Action<HypeRateDataPackage> OnMessage;

        public static HypeRate GetInstance()
        {
            if (_instance == null)
            {
                _instance = new HypeRate();
            }
            return _instance;
        }

        public async Task ConnectToServer(string websocketToken, string hyperateURL = "wss://app.hyperate.io/socket/websocket")
        {
            var serverUri = new Uri(hyperateURL + "?token=" + websocketToken);
            await _webSocket.ConnectAsync(serverUri, CancellationToken.None);

            Debug.Log("[HypeRate] Connection open!");
            // try to join all channels that where defined before. This is the case on a reconnect
            foreach (var channelName in _channels.GetChannelsToJoin())
            {
                var refArg = _channels.AddJoiningChannel(channelName);
                await SendMessage(Network.GetJoinPacket(channelName, refArg));
            }
            CreateMessageReciever();
        }

        private void CreateMessageReciever()
        {
            _ = Task.Run(async () =>
            {
                while (_webSocket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult result;
                    var buffer = new ArraySegment<byte>(new byte[1024]);
                    do
                    {
                        result = await _webSocket.ReceiveAsync(buffer, CancellationToken.None);
                        var message = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);
                        
                        var dataPackage = JsonUtility.FromJson<HypeRateDataPackage>(message);

                        OnMessage?.Invoke(dataPackage);
                        
                        if (dataPackage.@event == "hr_update")
                        {
                            var hr = int.Parse(dataPackage.payload.hr);
                            OnHeartRateUpdate?.Invoke(hr);
                        }
                    }
                    while (!result.EndOfMessage);
                }
            });
        }

        public async Task CloseConnection()
        {
            await _webSocket.CloseAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None);
            _channels.HandleReconnect();
            Debug.Log("[HypeRate] Connection closed!");
        }

        // Send a message to the WebSocket server
        public async Task SendMessage(string message)
        {
            var sendBuffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
            await _webSocket.SendAsync(sendBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
            Debug.Log("[HypeRate] Sent message: " + message);
        }

        public async Task JoinHeartbeatChannel(string deviceId)
        {
            var channelName = string.Format("hr:{0}", deviceId);
            var refArg = _channels.AddJoiningChannel(channelName);

            await SendMessage(Network.GetJoinPacket(channelName, refArg));
        }

        public async Task LeaveHeartbeatChannel(string deviceId)
        {
            var channelName = string.Format("hr:{0}", deviceId);
            var refArg = _channels.AddLeavingChannel(channelName);

            await SendMessage(Network.GetLeavePacket(channelName, refArg));
        }

        public async Task JoinClipsChannel(string deviceId)
        {
            var channelName = string.Format("clips:{0}", deviceId);
            var refArg = _channels.AddJoiningChannel(channelName);

            await SendMessage(Network.GetJoinPacket(channelName, refArg));
        }

        public async Task LeaveClipsChannel(string deviceId)
        {
            var channelName = string.Format("clips:{0}", deviceId);
            var refArg = _channels.AddLeavingChannel(channelName);

            await SendMessage(Network.GetLeavePacket(channelName, refArg));
        }

        public static ChannelType DetermineChannelType(string input)
        {
            if (input.StartsWith("hr:"))
            {
                return ChannelType.Heartrate;
            }

            return input.StartsWith("clips:") ? ChannelType.Clip : ChannelType.Unknown;
        }

        public static string ExtractDeviceIdFromChannelName(string input)
        {
            if (input.StartsWith("hr:"))
            {
                return input[3..];
            }

            return input.StartsWith("clips:") ? input[6..] : input;
        }
    }
}