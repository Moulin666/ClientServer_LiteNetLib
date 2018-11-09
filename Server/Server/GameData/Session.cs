using System;
using System.Collections.Generic;
using System.Timers;
using LiteNetLib;
using LiteNetLib.Utils;
using NetCommon.Codes;


namespace Server.GameData
{
    public class Session : IDisposable
    {
        public byte[] Id { get; set; }

        public Dictionary<long, Client> Players;

        public Session (byte[] id, float startSessionTime)
        {
            Id = id;

            Players = new Dictionary<long, Client>();

            var timer = new Timer(startSessionTime);
            timer.Elapsed += (src, args) => StartSession(src, args);
            timer.AutoReset = false;
            timer.Start();
        }

        public void StartSession (object src, ElapsedEventArgs args)
        {
            if (Players.Count != 2)
                SessionCache.Instance.DeleteSession(Id);

            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)NetOperationCode.StartSession);

            SendToAll(writer, DeliveryMethod.ReliableOrdered);
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
            lock (Players)
            {
                if (Players.ContainsKey(player.NetPeer.Id) || Players.Count >= 2)
                    return false;

                Players[player.NetPeer.Id] = player;
                return true;
            }
        }

        public void Leave(long playerId)
        {
            lock (Players)
            {
                if (Players.ContainsKey(playerId))
                    Players.Remove(playerId);
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
