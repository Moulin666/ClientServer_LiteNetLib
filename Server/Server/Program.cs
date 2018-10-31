using LiteNetLib;
using LiteNetLib.Utils;
using System;


namespace Server
{
    public class Program
    {
        private NetManager _netServer { get; set; }
        private NetPeer _ourPeer { get; set; }
        private NetDataWriter _dataWriter { get; set; }

        public static void Main(string[] args)
        {
            Console.WriteLine("Start application.");

            INetEventListener server = new ServerNetEventListener();

            Console.ReadKey();
        }
    }
}
