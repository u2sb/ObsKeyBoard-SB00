using System;
using OBSWebsocketDotNet;

namespace ObsKeyBoardClient.Utils;

internal class ObsWsHelp : SingletonClass<ObsWsHelp>
{
    public ObsWsHelp()
    {
        Obs = new OBSWebsocket();
    }

    public OBSWebsocket Obs { get; }

    public void Connect(int port = 4444, string ip = "127.0.0.1", string password = "")
    {
        try
        {
            Obs.Connect($"ws://{ip}:{port}", password);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void DisConnect()
    {
        Obs.Disconnect();
    }
}