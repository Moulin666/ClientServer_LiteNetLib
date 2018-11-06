using LiteNetLib;
using NetCommon.MessageObjects;


namespace Server
{
    public class NetPlayer
    {
        public NetPeer NetPeer { get; set; }

        public PlayerData PlayerData { get; set; }

        public NetPlayer(NetPeer peer)
        {
            NetPeer = peer;

            // We can get this info from database.
            PlayerData = new PlayerData
            {
                Id = peer.Id,
                PositionData = new PositionData(0, 5, 0),
                Health = 100,
                AttackRadius = 10,
                Damage = 10,
                MoveSpeed = 5f
            };
        }

        public NetPlayer(NetPeer peer, PlayerData playerData)
        {
            NetPeer = peer;
            PlayerData = playerData;
        }
    }
}
