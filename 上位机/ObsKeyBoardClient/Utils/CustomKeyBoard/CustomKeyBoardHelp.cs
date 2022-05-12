using System;

namespace ObsKeyBoardClient.Utils.CustomKeyBoard;

public class CustomKeyBoardHelp : SingletonClass<CustomKeyBoardHelp>
{
    public Action<CustomEncoder, CustomEncoderEvent>? OnCustomEncoder;
    public Action<CustomKey, CustomKeyEvent>? OnCustomKey;
    public Action<CustomPotentiometer, float>? OnCustomPotentiometer;

    public void DataReceived(byte[] data)
    {
        if (data.Length % 7 == 0)
            for (var i = 0; i < data.Length / 7; i++)
            {
                var b = new byte[7];
                Array.ConstrainedCopy(data, i * 7, b, 0, 7);
                var c = CustomKeyBoardEvent.AnalysisData(data);
                var d = CustomKeyBoardEvent.GetType(c);

                switch (d)
                {
                    case CustomEventType.Key:
                        OnCustomKey?.Invoke(CustomKeyBoardEvent.GetKey(c, out var keyEvent), keyEvent);
                        return;
                    case CustomEventType.Encoder:
                        OnCustomEncoder?.Invoke(CustomKeyBoardEvent.GetEncoder(c, out var encoderEvent), encoderEvent);
                        return;
                    case CustomEventType.Potentiometer:
                        OnCustomPotentiometer?.Invoke(CustomKeyBoardEvent.GetPotentiometer(c, out var value), value);
                        return;
                }
            }
    }
}