#include <Arduino.h>
#include <USB.h>
#include <bc_key_scan.h>
#include <ESPRotary.h>
#include <SoftwareSerial.h>

#include "model/KeyBoardData.h"

#if ARDUINO_USB_CDC_ON_BOOT
#define HWSerial Serial0
#define USerial Serial
#else
#define HWSerial Serial
USBCDC USerial;
#endif

SoftwareSerial swSerial(1, 2);
BcKeyScan Keypad(swSerial);

ESPRotary r00 = ESPRotary(3, 4);
ESPRotary r01 = ESPRotary(5, 6);
ESPRotary r02 = ESPRotary(7, 8);
ESPRotary r10 = ESPRotary(13, 14);
ESPRotary r11 = ESPRotary(15, 16);
ESPRotary r12 = ESPRotary(17, 18);

CustomKeyBoardData KeyBoardData;
KeyBoardInputData InputData;

byte send_index = 0x1C;

void KeyDataOutput()
{
  MsgPacketizer::send(USerial, send_index, InputData);
}

void BcKeyInit()
{
  swSerial.begin(9600);
  Keypad.setDetectMode(1);
}

void BcKeyUpdate()
{
  Keypad.checkChanges();
  if (Keypad.isKeyChanged())
  {
    byte keyValue = Keypad.getKeyValue();
    byte keyValue0 = keyValue;

    if (keyValue < 0x80)
    {
      keyValue0 = keyValue;
      KeyBoardData.data = KeyDown;
    }
    else
    {
      keyValue0 = keyValue - 0x80;
      KeyBoardData.data = KeyUp;
    }

    auto it = keyBoardValue.find(keyValue0);

    if (it != keyBoardValue.end())
    {
      // 设置类型为按键
      InputData.type = Key;
      // 设置按键位
      KeyBoardData.address = keyBoardValue[keyValue0];
    }
    else
    {
      it = encoderValue.find(keyValue0);
      if (it != encoderValue.end())
      {
        // 设置类型为编码器 0x02
        InputData.type = Encoder;

        // 设置编码器位
        KeyBoardData.address = encoderValue[keyValue0];

        // 设置编码器值
        KeyBoardData.data = KeyBoardData.data + 0x02;
      }
      else
      {
        return;
      }
    }

    InputData.data = KeyBoardData;
    KeyDataOutput();
  }
}

/// @brief 编码器输出
void outEncoder(byte direction, byte encoderValue)
{
  switch (direction)
  {
  case RE_LEFT:
    KeyBoardData.data = EncoderReduce;
    break;
  case RE_RIGHT:
    KeyBoardData.data = EncoderAdd;
    break;
  default:
    return;
  }

  KeyBoardData.address = encoderValue;
  InputData.type = Encoder;
  InputData.data = KeyBoardData;
  KeyDataOutput();
}

void r00RotationHandler(ESPRotary &r)
{
  outEncoder(r.getDirection(), 0x00);
}

void r01RotationHandler(ESPRotary &r)
{
  outEncoder(r.getDirection(), 0x01);
}
void r02RotationHandler(ESPRotary &r)
{
  outEncoder(r.getDirection(), 0x02);
}
void r10RotationHandler(ESPRotary &r)
{
  outEncoder(r.getDirection(), 0x05);
}
void r11RotationHandler(ESPRotary &r)
{
  outEncoder(r.getDirection(), 0x06);
}
void r12RotationHandler(ESPRotary &r)
{
  outEncoder(r.getDirection(), 0x07);
}

void RotaryInit()
{
  r00.setStepsPerClick(4);
  r00.setLeftRotationHandler(r00RotationHandler);
  r00.setRightRotationHandler(r00RotationHandler);

  r01.setStepsPerClick(4);
  r01.setLeftRotationHandler(r01RotationHandler);
  r01.setRightRotationHandler(r01RotationHandler);

  r02.setStepsPerClick(4);
  r02.setLeftRotationHandler(r02RotationHandler);
  r02.setRightRotationHandler(r02RotationHandler);

  r10.setStepsPerClick(4);
  r10.setLeftRotationHandler(r10RotationHandler);
  r10.setRightRotationHandler(r10RotationHandler);

  r11.setStepsPerClick(4);
  r11.setLeftRotationHandler(r11RotationHandler);
  r11.setRightRotationHandler(r11RotationHandler);

  r12.setStepsPerClick(4);
  r12.setLeftRotationHandler(r12RotationHandler);
  r12.setRightRotationHandler(r12RotationHandler);
}

void RotaryUpdate()
{
  r00.loop();
  r01.loop();
  r02.loop();
  r10.loop();
  r11.loop();
  r12.loop();
}

void setup()
{
  HWSerial.begin(115200);
  HWSerial.setDebugOutput(true);

  USerial.begin();
  USB.begin();

  BcKeyInit();
  RotaryInit();
}

uint8_t buffer[128];
size_t message_length;
bool status;

void loop()
{
  BcKeyUpdate();
  RotaryUpdate();
  MsgPacketizer::update();
}