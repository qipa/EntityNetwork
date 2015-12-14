﻿using System;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;
using ProtoBuf.Meta;
using TrackableData;
using TypeAlias;

namespace EntityNetwork.Tests
{
    internal class DummyChannelToServerZoneInbound : IByteChannel
    {
        public ProtobufChannelToServerZoneInbound Channel;

        public void Write(byte[] bytes)
        {
            ((ServerZone)Channel.InboundServerZone).RunAction(_ => { Channel.Write(bytes); });
        }
    }

    internal class TestZoneSet
    {
        public TestServerZone ServerZone;
        public TestClientZone[] ClientZones;
        public Dictionary<int, TestClientZone> ClientZoneMap;

        private readonly TypeAliasTable _typeTable;
        private readonly TypeModel _typeModel;
        private int _lastClientId;

        public TestZoneSet(TypeAliasTable typeTable, TypeModel typeModel)
        {
            ServerZone = new TestServerZone(EntityFactory.Default);
            ClientZones = new TestClientZone[0];
            ClientZoneMap = new Dictionary<int, TestClientZone>();

            _typeTable = typeTable;
            _typeModel = typeModel;
        }

        public KeyValuePair<int, TestClientZone> AddClient()
        {
            var clientId = ++_lastClientId;

            var channelUp = new ProtobufChannelToServerZoneOutbound
            {
                TypeTable = _typeTable,
                TypeModel = _typeModel,
                OutboundChannel = new DummyChannelToServerZoneInbound
                {
                    Channel = new ProtobufChannelToServerZoneInbound
                    {
                        TypeTable = _typeTable,
                        TypeModel = _typeModel,
                        ClientId = clientId,
                        InboundServerZone = ServerZone,
                    }
                }
            };

            var clientZone = new TestClientZone(EntityFactory.Default, channelUp);

            var channelDown = new ProtobufChannelToClientZoneOutbound
            {
                TypeTable = _typeTable,
                TypeModel = _typeModel,
                OutboundChannel = new ProtobufChannelToClientZoneInbound()
                {
                    TypeTable = _typeTable,
                    TypeModel = _typeModel,
                    InboundClientZone = clientZone,
                }
            };

            ServerZone.AddClient(clientId, channelDown);

            ClientZoneMap.Add(clientId, clientZone);
            ClientZones = ClientZoneMap.Values.ToArray();

            return new KeyValuePair<int, TestClientZone>(clientId, clientZone);
        }

        public void RemoveClient(int clientId)
        {
            if (ClientZoneMap.Remove(clientId) == false)
                return;

            ServerZone.RemoveClient(clientId);
            ClientZones = ClientZoneMap.Values.ToArray();
        }
    }

    internal static class TestZoneBuilder
    {
        public const int ClientZoneCount = 2;

        public static TestZoneSet Build()
        {
            return Build(ClientZoneCount);
        }

        public static TestZoneSet Build(int clientZoneCount)
        {
            var typeTable = new TypeAliasTable();

            var typeModel = TypeModel.Create();
            typeModel.Add(typeof(TrackablePocoTracker<ISpaceShipData>), false)
                     .SetSurrogate(typeof(TrackableSpaceShipDataTrackerSurrogate));

            var zoneSet = new TestZoneSet(typeTable, typeModel);
            for (var i = 0; i < clientZoneCount; i++)
                zoneSet.AddClient();
            return zoneSet;
        }
    }
}
