using System;
using OBSWebsocketDotNet;

namespace OBSKeyBoard_SB00.Utils;

public class ObsWsHelp : Singleton<ObsWsHelp>
{
    public ObsWsHelp()
    {
        ObsWs = new OBSWebsocket();
    }

    public OBSWebsocket ObsWs { get; }

    public void Connect(int port = 4444, string ip = "127.0.0.1", string password = "")
    {
        try
        {
            ObsWs.ConnectAsync($"ws://{ip}:{port}", password);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void DisConnect()
    {
        ObsWs.Disconnect();
    }
}