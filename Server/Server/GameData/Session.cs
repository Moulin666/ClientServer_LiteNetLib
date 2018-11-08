using System;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;


namespace Server.GameData
{
    public class Session : IDisposable
    {
        public byte[] Id { get; set; }

        public Dictionary<long, Client> Players;

        public Session (byte[] id)
        {
            Id = id;

            Players = new Dictionary<long, Client>();
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

        public void SendToPlayer(Client player, NetDataWriter dataWriter, DeliveryMethod deliveryMethod) => player.NetPeer.Send(dataWriter, deliveryMethod);

        public void SendToAll (NetDataWriter dataWriter, DeliveryMethod deliveryMethod)
        {
            lock (Players)
                foreach (var p in Players)
                    p.Value.NetPeer.Send(dataWriter, deliveryMethod);
        }
    }
}
