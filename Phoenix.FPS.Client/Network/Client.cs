using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.FPS.Client.Network
{
    internal static class Client
    {
        public static Riptide.Client RiptideClient;
        public static long Tick = 0;
        static Game _game;
        public static void Init(Game game)
        {
            _game = game;
            RiptideClient = new();

            var ip = "192.168.1.40:7777";
            RiptideClient.Connect(ip, 10);
            RiptideClient.ConnectionFailed += ConnectionFailed;
            RiptideClient.Connected += Connected;
            RiptideClient.Disconnected += Disconnected;
            
        }

        private static void Connected(object? sender, EventArgs e)
        {
            Console.WriteLine("sending id");
            MessagesOut.SendPlayerId();
        }

        private static void Disconnected(object? sender, Riptide.DisconnectedEventArgs e)
        {
            Console.WriteLine("dc");
            Console.WriteLine(e.Reason);
            if(e.Message is not null)
                Console.WriteLine(e.Message.GetString());

            //m.get
        }
        private static void ConnectionFailed(object? sender, Riptide.ConnectionFailedEventArgs e)
        {
        }
        static double lastT = 0; 
        public static void Update()
        {
            var t = _game.Graphics.Time;
            var tdiff = t - lastT;
            
            RiptideClient.Update();
            if (tdiff < 0.005)
            {
                return;
            }
            lastT = t;

            if (RiptideClient.IsConnected)
            {
                MessagesOut.SendData();
            }
        }
    }
}
