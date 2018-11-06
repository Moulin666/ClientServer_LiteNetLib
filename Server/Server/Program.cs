using System;


namespace Server
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Start application.");

            ServerNetEventListener server = new ServerNetEventListener(100, "TestServer");
            server.Start(15000);
        }
    }
}
