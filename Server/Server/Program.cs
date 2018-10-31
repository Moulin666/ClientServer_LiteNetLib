using System;


namespace Server
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Start application.");

            ServerNetEventListener server = new ServerNetEventListener();
        }
    }
}
