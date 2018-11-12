using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using LiteNetLib;
using LiteNetLib.Utils;
using NetCommon;
using NetCommon.Codes;
using Server.GameData;


namespace Server.GameLogic.Session
{
    public class Session : IDisposable
    {
        public byte[] Id { get; private set; }

        public bool IsStarted { get; private set; }

        public Dictionary<int, Unit> Units { get; set; }

        public Dictionary<long, Client> Players;

        public Session (byte[] id, Dictionary<int, Unit> units, float startSessionTime)
        {
            Id = id;
            Units = units;

            IsStarted = false;

            Players = new Dictionary<long, Client>();

            var timer = new Timer(startSessionTime);
            timer.Elapsed += (src, args) => StartSession(src, args);
            timer.AutoReset = false;
            timer.Start();
        }

        public void StartSession (object src, ElapsedEventArgs args)
        {
            if (IsStarted)
                return;

            if (Players.Count != 2)
            {
                SessionCache.Instance.DeleteSession(Id);
                return;
            }

            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)NetOperationCode.StartSession);

            writer.Put(Players.ElementAt(1).Value.Units.Count);
            foreach (var u in Players.ElementAt(1).Value.Units)
                writer.Put(MessageSerializerService.SerializeObjectOfType(u.Value.UnitData));

            Players.ElementAt(0).Value.NetPeer.Send(writer, DeliveryMethod.ReliableOrdered);

            writer.Reset();
            writer.Put((byte)NetOperationCode.StartSession);

            writer.Put(Players.ElementAt(0).Value.Units.Count);
            foreach (var u in Players.ElementAt(0).Value.Units)
                writer.Put(MessageSerializerService.SerializeObjectOfType(u.Value.UnitData));

            Players.ElementAt(1).Value.NetPeer.Send(writer, DeliveryMethod.ReliableOrdered);

            IsStarted = true;
        }
        
        public void Dispose()
        {
            lock (Players)
            {
                foreach (var player in Players.Values)
                    player.NetPeer.Disconnect();

                Players.Clear();
            }

            // Send result to API Server.
        }

        public bool Join(Client player)
        {
            if (IsStarted)
                return false;

            lock (Players)
            {
                if (Players.ContainsKey(player.NetPeer.Id) || Players.Count >= 2)
                    return false;

                player.CurrentSessionId = Id;

                if (Players.Count == 0)
                    for (int i = 0; i < 6; i++)
                        player.Units[i] = Units[i];
                else
                    for (int i = 6; i < 12; i++)
                        player.Units[i] = Units[i];

                Players[player.NetPeer.Id] = player;
                return true;
            }
        }

        public void Leave(long playerId)
        {
            lock (Players)
            {
                if (Players.ContainsKey(playerId))
                {
                    var player = Players[playerId];
                    player.CurrentSessionId = null;
                    player.Units.Clear();

                    Players.Remove(playerId);

                    IsStarted = false;
                }
            }
        }
       
        public void SendToAll (NetDataWriter dataWriter, DeliveryMethod deliveryMethod)
        {
            lock (Players)
                foreach (var p in Players)
                    p.Value.NetPeer.Send(dataWriter, deliveryMethod);
        }
    }
}
