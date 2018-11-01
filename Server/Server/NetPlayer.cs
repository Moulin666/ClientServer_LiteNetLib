using LiteNetLib;
using System;


namespace Server
{
    public class NetPlayer
    {
        public NetPeer NetPeer { get; set; }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Boolean Moved;

        public NetPlayer(NetPeer peer)
        {
            NetPeer = peer;

            X = 0f;
            Y = 5f;
            Z = 0f;

            Moved = false;
        }

        public NetPlayer(NetPeer peer, Boolean isMine)
        {
            NetPeer = peer;

            X = 0f;
            Y = 5f;
            Z = 0f;

            Moved = false;
        }
    }
}
