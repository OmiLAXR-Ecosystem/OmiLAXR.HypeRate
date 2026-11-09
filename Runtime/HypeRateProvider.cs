using OmiLAXR.Actors.HeartRate;
using OmiLAXR.HypeRate.SDK;
using UnityEngine;
using HR = OmiLAXR.HypeRate.SDK.HypeRate;

namespace OmiLAXR.HypeRate
{
    /// <summary>
    /// Heart rate provider that receives the learner's heart rate live from the HypeRate service over a websocket.
    /// The learner shares it from the HypeRate app; enter its session ID and your HypeRate API key.
    /// </summary>
    public class HypeRateProvider : HeartRateProvider
    {
        public override int GetHeartRate() => heartRate;

        [ReadOnly, Tooltip("Latest heart rate in beats per minute received from HypeRate.")]
        public int heartRate = 0;
        
        [Header("Get it here https://www.hyperate.io/api.")]
        [Tooltip("Your HypeRate API key (request it at https://www.hyperate.io/api).")]
        public string apiKey = "<Request your API key>"; 
        [Header("Get it from your HypeRate App.")]
        [Tooltip("Session ID shown in the learner's HypeRate app.")]
        public string sessionId = "internal-testing";
        [Tooltip("Websocket address of the HypeRate service.")]
        public string hyperateURL = "wss://app.hyperate.io/socket/websocket";

        /// <summary>
        /// Socket delivering heart rates. Defaults to the HypeRate websocket; assign before Start to replace it, e.g. in tests.
        /// </summary>
        public IHeartRateSocket Socket { get; set; }

        private IHeartRateSocket _hypeRateSocket;

        private async void Start()
        {
            _hypeRateSocket = Socket ?? HR.GetInstance();

            _hypeRateSocket.HeartRateUpdated += hr =>
            {
                heartRate = hr;
            }; 
            
            await _hypeRateSocket.ConnectToServer(apiKey, hyperateURL);
            await _hypeRateSocket.JoinHeartbeatChannel(sessionId);
            
        }

        private async void OnApplicationQuit()
        {
            if (_hypeRateSocket == null)
                return;
            await _hypeRateSocket.CloseConnection();
        }
    }
}