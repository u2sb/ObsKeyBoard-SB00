﻿using Algorithm.Check;
using MessagePack;
using OBSKeyBoard_SB00.Models;

namespace OBSKeyBoard_SB00.Utils;

public class KeyBoardHelp : Singleton<KeyBoardHelp>
{
    private readonly SerialPortHelp _serialPortHelp = SerialPortHelp.Instance;
    public Action<CustomEncoder, CustomEncoderEvent>? OnCustomEncoder;
    public Action<CustomKey, CustomKeyEvent>? OnCustomKey;
    public Action<CustomPotentiometer, float>? OnCustomPotentiometer;

    public KeyBoardHelp()
    {
        _serialPortHelp.OnDataReceived += SerialPortDataReceived;
    }

    public void Open(string com, int b = 921600)
    {
        _serialPortHelp.Init(com, b);
        _serialPortHelp.Open();
    }

    public void Close()
    {
        _serialPortHelp.Close();
    }

    private void SerialPortDataReceived(byte[] bytes)
    {
        lock (Lock)
        {
            var d = COBS.Decode(bytes);
            var index = d[0];
            if (index != 0) return;
            var crc = d.Last();
            var mp = d.Skip(1).SkipLast(1).ToArray();
            if (crc != d.SkipLast(1).CRC8()) return;
            var data = MessagePackSerializer.Deserialize<KeyBoardInputData>(mp);
            if (data != null && data.Type != CustomEventType.None)
                switch (data.Type)
                {
                    case CustomEventType.Key:
                        OnCustomKey?.Invoke((CustomKey)data.Data.Address, (CustomKeyEvent)data.Data.Data);
                        break;
                    case CustomEventType.Encoder:
                        OnCustomEncoder?.Invoke((CustomEncoder)data.Data.Address,
                            (CustomEncoderEvent)data.Data.Data);
                        break;
                    case CustomEventType.Potentiometer:
                        OnCustomPotentiometer?.Invoke((CustomPotentiometer)data.Data.Address, data.Data.Data);
                        break;
                }
        }
    }
}