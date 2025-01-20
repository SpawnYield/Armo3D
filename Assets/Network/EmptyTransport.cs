using Unity.Netcode;
using UnityEngine;
using System;

public class EmptyTransport : NetworkTransport
{
    private bool isServerStarted = false;
    private bool isClientStarted = false;

    public override void Initialize(NetworkManager networkManager = null)
    {
        Debug.Log("EmptyTransport Initialized.");
    }

    public override void Shutdown()
    {
        Debug.Log("EmptyTransport Shutdown.");
    }

    public override ulong ServerClientId => 0;

    public override void Send(ulong clientId, ArraySegment<byte> data, NetworkDelivery delivery)
    {
        // Пустая отправка данных
    }

    public override NetworkEvent PollEvent(out ulong clientId, out ArraySegment<byte> payload, out float receiveTime)
    {
        clientId = 0;
        payload = default;
        receiveTime = 0;
        return NetworkEvent.Nothing;
    }

    public override void DisconnectLocalClient() { }
    public override void DisconnectRemoteClient(ulong clientId) { }

    public override ulong GetCurrentRtt(ulong clientId) => 0;

    // Реализуем начало работы сервера
    public override bool StartServer()
    {
        if (!isServerStarted)
        {
            isServerStarted = true;
            Debug.Log("Server started.");
            return true;
        }
        return false;
    }

    // Реализуем начало работы клиента
    public override bool StartClient()
    {
        if (!isClientStarted)
        {
            isClientStarted = true;
            Debug.Log("Client started.");
            return true;
        }
        return false;
    }
}
