﻿using System.Collections;
using System.IO;
using Basic;
using EntityNetwork;
using ProtoBuf.Meta;
using TypeAlias;
using UnityEngine;
using UnityEngine.Networking;

public class EntityNetworkClient : NetworkBehaviour
{
    public static int LocalClientId;

    private int _clientId;
    private ClientZone _zone;
    private ProtobufChannelToClientZoneInbound _zoneChannel;

    public int ClientId { get { return _clientId; } }

    // Registration

    public override void OnStartClient()
    {
        // In OnStartClient, ClientRpc is not called.
        // To workaround this limitation, use coroutine
        StartCoroutine(AddClientToZone());
    }

    private void OnDestroy()
    {
        EntityNetworkManager.Instance.RemoveClientToZone(_clientId);
    }

    private IEnumerator AddClientToZone()
    {
        yield return null;
        if (hasAuthority == false)
            yield break;

        var channel = new ProtobufChannelToServerZoneOutbound();
        channel.OutboundChannel = new EntityNetworkChannelToServerZone { NetworkClient = this };
        channel.TypeTable = EntityNetworkManager.TypeTable;
        channel.TypeModel = EntityNetworkManager.ProtobufTypeModel;

        _clientId = (int)netId.Value;
        _zone = new ClientZone(ClientEntityFactory.Default, channel);
        _zoneChannel = new ProtobufChannelToClientZoneInbound
        {
            TypeTable = EntityNetworkManager.TypeTable,
            TypeModel = EntityNetworkManager.ProtobufTypeModel,
            InboundClientZone = _zone
        };
        LocalClientId = _clientId;

        CmdAddClientToZone();
    }

    [Command]
    public void CmdAddClientToZone()
    {
        _clientId = (int)netId.Value;

        var result = EntityNetworkManager.Instance.AddClientToZone(_clientId, this);
        RpcAddClientToZoneDone(result);
    }

    [ClientRpc]
    public void RpcAddClientToZoneDone(bool added)
    {
        if (hasAuthority == false)
            return;

        Debug.LogFormat("RpcAddClientToZoneDone({0})", added);
        if (added)
        {
            CmdAddClientToZoneDone();
        }
        else
        {
            _clientId = 0;
            _zone = null;
            _zoneChannel = null;
            LocalClientId = 0;
        }
    }

    [Command]
    public void CmdAddClientToZoneDone()
    {
        Debug.LogFormat("CmdAddClientToZoneDone({0})", _clientId);

        EntityNetworkManager.Instance.Zone.RunAction(zone =>
        {
            zone.Spawn(typeof(ISpaceShip), _clientId, EntityFlags.Normal);
        });
    }

    // Zone Channel

    [Command]
    public void CmdBuffer(byte[] bytes)
    {
        EntityNetworkManager.Instance.ZoneChannel.Write(bytes);
    }

    [ClientRpc]
    public void RpcBuffer(byte[] bytes)
    {
        if (hasAuthority == false)
            return;

        _zoneChannel.Write(bytes);
    }

    public override void OnNetworkDestroy()
    {
        Debug.LogFormat("OnNetworkDestroy()");
    }
}
