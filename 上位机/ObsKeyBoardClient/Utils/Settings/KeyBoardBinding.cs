using System.Collections.Generic;
using ObsKeyBoardClient.Utils.CustomKeyBoard;

namespace ObsKeyBoardClient.Utils.Settings;

public class KeyBoardBinding
{
    public Dictionary<CustomKey, string?>? KeyBinding { get; set; }
    public Dictionary<CustomEncoder, string?>? EncoderBinding { get; set; }
    public Dictionary<CustomPotentiometer, string?>? PotentiometerBinding { get; set; }
}