namespace ObsKeyBoardClient.Utils.CustomKeyBoard;

public static class CustomKeyBoardEvent
{
    /// <summary>
    ///     分析数据
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static byte[]? AnalysisData(byte[] data)
    {
        //判断头部是否符合要求
        if (data.Length != 7 || data[0] != 0xFF || data[1] != 0x00 || data[5] != 0x0D || data[6] != 0x0A) return null;

        var b = new byte[3];

        b[0] = data[2];
        b[1] = data[3];
        b[2] = data[4];

        return b;
    }

    /// <summary>
    ///     获取类型
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static CustomEventType GetType(byte[]? data)
    {
        if (data == null || data.Length != 3 || data[0] > 3) return CustomEventType.None;

        return (CustomEventType) data[0];
    }

    /// <summary>
    ///     获取按键
    /// </summary>
    /// <param name="data"></param>
    /// <param name="keyEvent"></param>
    /// <returns></returns>
    public static CustomKey GetKey(byte[]? data, out CustomKeyEvent keyEvent)
    {
        keyEvent = (CustomKeyEvent) data[2];
        return (CustomKey) data[1];
    }

    /// <summary>
    ///     获取编码器
    /// </summary>
    /// <param name="data"></param>
    /// <param name="encoderEvent"></param>
    /// <returns></returns>
    public static CustomEncoder GetEncoder(byte[]? data, out CustomEncoderEvent encoderEvent)
    {
        encoderEvent = (CustomEncoderEvent) data[2];
        return (CustomEncoder) data[1];
    }

    /// <summary>
    ///     获取电位器
    /// </summary>
    /// <param name="data"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static CustomPotentiometer GetPotentiometer(byte[]? data, out float value)
    {
        value = (float) data[2] / 0xFF;
        return (CustomPotentiometer) data[1];
    }
}

/// <summary>
///     类型
/// </summary>
public enum CustomEventType
{
    None = 0,
    Key = 1,
    Encoder = 2,
    Potentiometer = 3
}

/// <summary>
///     键值表
/// </summary>
public enum CustomKey
{
    K00 = 0x00,
    K01 = 0x01,
    K02 = 0x02,
    K03 = 0x03,
    K04 = 0x04,
    K05 = 0x05,
    K06 = 0x06,
    K07 = 0x07,

    K10 = 0x10,
    K11 = 0x11,
    K12 = 0x12,
    K13 = 0x13,
    K14 = 0x14,
    K15 = 0x15,
    K16 = 0x16,
    K17 = 0x17,

    K20 = 0x20,
    K21 = 0x21,
    K22 = 0x22,
    K23 = 0x23,
    K24 = 0x24,
    K25 = 0x25,
    K26 = 0x26,
    K27 = 0x27,

    K30 = 0x30,
    K31 = 0x31,
    K32 = 0x32,
    K33 = 0x33,
    K34 = 0x34,
    K35 = 0x35,
    K36 = 0x36,
    K37 = 0x37,

    K40 = 0x40,
    K41 = 0x41,
    K42 = 0x42,
    K43 = 0x43,
    K44 = 0x44,
    K45 = 0x45,
    K46 = 0x46,
    K47 = 0x47,

    K50 = 0x50,
    K51 = 0x51,
    K52 = 0x52,
    K53 = 0x53,
    K54 = 0x54,
    K55 = 0x55,
    K56 = 0x56,
    K57 = 0x57,

    K60 = 0x60,
    K61 = 0x61,
    K62 = 0x62,
    K63 = 0x63,
    K64 = 0x64,
    K65 = 0x65,
    K66 = 0x66,
    K67 = 0x67,

    K70 = 0x70,
    K71 = 0x71,
    K72 = 0x72,
    K73 = 0x73,
    K74 = 0x74,
    K75 = 0x75,
    K76 = 0x76,
    K77 = 0x77
}

/// <summary>
///     按键事件
/// </summary>
public enum CustomKeyEvent
{
    Normal = 0,
    Down = 1,
    Up = 2
}

/// <summary>
///     编码器表
/// </summary>
public enum CustomEncoder
{
    E00 = 0x00,
    E01 = 0x01,
    E02 = 0x02,
    E03 = 0x03,
    E04 = 0x04,
    E05 = 0x05,
    E06 = 0x06,
    E07 = 0x07,
    E08 = 0x08,
    E09 = 0x09
}

/// <summary>
///     编码器事件
/// </summary>
public enum CustomEncoderEvent
{
    Normal = 0,

    /// <summary>
    ///     定义 顺时针 加
    /// </summary>
    Add = 1,

    /// <summary>
    ///     定义 逆时针 减
    /// </summary>
    Reduce = 2,

    /// <summary>
    ///     按下
    /// </summary>
    Down = 3,

    /// <summary>
    ///     抬起
    /// </summary>
    Up = 4
}

/// <summary>
///     电位器表
/// </summary>
public enum CustomPotentiometer
{
    P00 = 0x00,
    P01 = 0x01,
    P02 = 0x02,
    P03 = 0x03,
    P04 = 0x04,
    P05 = 0x05,
    P06 = 0x06,
    P07 = 0x07,
    P08 = 0x08,
    P09 = 0x09
}