using LiteNetLib;
using NetCommon;


namespace Server
{
    public class NetPlayer
    {
        public NetPeer NetPeer { get; set; }

        public PlayerData PlayerData { get; set; }

        public NetPlayer(NetPeer peer)
        {
            NetPeer = peer;

            PlayerData = new PlayerData
            {
                Id = peer.ConnectId,
                X = 0,
                Y = 5,
                Z = 0,
                IsMine = false,
                Moved = false
            };
        }

        public NetPlayer(NetPeer peer, PlayerData playerData)
        {
            NetPeer = peer;
            PlayerData = playerData;
        }
    }
}
