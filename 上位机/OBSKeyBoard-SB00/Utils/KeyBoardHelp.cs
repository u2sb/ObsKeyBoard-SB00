using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using OBSKeyBoard_SB00.Models;

namespace OBSKeyBoard_SB00.Utils;

public class KeyBoardHelp : Singleton<KeyBoardHelp>
{
    private readonly List<string> _serialPortDatas = new();
    private readonly SerialPortHelp _serialPortHelp = SerialPortHelp.Instance;
    private readonly UTF8Encoding _utf8 = new();
    public Action<CustomEncoder, CustomEncoderEvent>? OnCustomEncoder;
    public Action<CustomKey, CustomKeyEvent>? OnCustomKey;
    public Action<CustomPotentiometer, float>? OnCustomPotentiometer;
    public Action<KeyBoardInputData>? OnDataReceived;

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
            var sRaw = _utf8.GetString(bytes);
            var i = 0;
            for (;;)
            {
                var si = sRaw.IndexOf("}{", i, StringComparison.Ordinal);

                if (si < 0)
                {
                    _serialPortDatas.Add(sRaw.Substring(i));
                    break;
                }

                _serialPortDatas.Add(sRaw.Substring(i, si - i + 1));

                i = si + 1;
            }

            foreach (var s in _serialPortDatas)
            {
                var data = JsonSerializer.Deserialize<KeyBoardInputData>(s, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

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

            _serialPortDatas.Clear();
        }
    }
}