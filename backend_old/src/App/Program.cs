using System;
using ElectronCgi.DotNet;

namespace SmashTracker.App.Electron
{
    class Program
    {
        static void Main(string[] args)
        {
            var connection = new ConnectionBuilder()
                .WithLogging()
                .Build();

            connection.On<string, string>("greeting", (val) => $"Hello {val}");

            connection.Listen();
        }
    }
}
