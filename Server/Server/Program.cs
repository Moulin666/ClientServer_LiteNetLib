using System;


namespace Server
{
    public class Program
    {
        private static ServerNetEventListener _server;

        private static void Main(string[] args)
        {
            Console.WriteLine("Start application.");

            var operation = "start";

            while (operation == "start")
            {
                _server = new ServerNetEventListener();
                _server.Start(15000);

                Console.ReadKey();

                _server.Stop();

                operation = Console.ReadLine();
            }
        }
    }
}
