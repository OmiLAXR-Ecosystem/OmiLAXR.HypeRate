/*
* SPDX-License-Identifier: AGPL-3.0-or-later
* Copyright (C) 2025 Sergej Görzen <sergej.goerzen@gmail.com>
* This file is part of OmiLAXR.HypeRate.
*/
using System;
using System.Threading.Tasks;

namespace OmiLAXR.HypeRate.SDK
{
    /// <summary>
    /// Connection that delivers heart rate updates (HypeRate websocket by default).
    /// </summary>
    public interface IHeartRateSocket
    {
        event Action<int> HeartRateUpdated;
        Task ConnectToServer(string websocketToken, string url);
        Task JoinHeartbeatChannel(string deviceId);
        Task CloseConnection();
    }
}
