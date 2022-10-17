#include <Arduino.h>
#include <map>

struct CustomKeyBoardData
{
    uint8_t address;
    uint16_t data;
};

struct KeyBoardInputData
{
    uint8_t type;
    CustomKeyBoardData data;
};

/// @brief 按键表
std::map<byte, byte> keyBoardValue{
    {0x00, 0x00},
    {0x07, 0x01},
    {0x0E, 0x02},
    {0x15, 0x03},
    {0x1C, 0x04},

    {0x01, 0x10},
    {0x08, 0x11},
    {0x0F, 0x12},
    {0x16, 0x13},
    {0x1D, 0x14},

    {0x02, 0x20},
    {0x09, 0x21},
    {0x10, 0x22},
    {0x17, 0x23},
    {0x1E, 0x24}};

/// @brief  编码器按键表
std::map<byte, byte> encoderValue{
    {0x03, 0x00},
    {0x0A, 0x01},
    {0x11, 0x02},

    {0x04, 0x05},
    {0x0B, 0x06},
    {0x12, 0x07}};

/// @brief 输入类型
enum CustomEventType
{
    None = 0,
    Key = 1,
    Encoder = 2,
    Potentiometer = 3
};

/// @brief 键值表
enum CustomKey
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
};

/// @brief 按键动作
enum CustomKeyEvent
{
    KeyNormal = 0,
    KeyDown = 1,
    KeyUp = 2
};

/// @brief 编码器表
enum CustomEncoder
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
};

/// @brief 编码器事件
enum CustomEncoderEvent
{
    EncoderNormal = 0,

    /// @brief 顺时针 加
    EncoderAdd = 1,

    /// @brief 逆时针 减
    EncoderReduce = 2,

    /// @brief 按下
    EncoderDown = 3,

    /// @brief 抬起
    EncoderUp = 4
};

/// @brief 电位器表
enum CustomPotentiometer
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
};
