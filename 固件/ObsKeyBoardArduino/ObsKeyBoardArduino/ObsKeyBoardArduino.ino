/*
 Name:		ObsKeyBoardArduino.ino
 Created:	2022/5/9 15:17:28
 Author:	MonoLogueChi
*/

#include <USB.h>
#include <bc_key_scan.h>
#include <SoftwareSerial.h>
#include <ESPRotary.h>
#include <map>


#if ARDUINO_USB_CDC_ON_BOOT
#define HWSerial Serial0
#define USBSerial Serial
#else
#define HWSerial Serial
USBCDC USBSerial;
#endif

SoftwareSerial swSerial(1, 2);

BcKeyScan Keypad(swSerial);

ESPRotary r00 = ESPRotary(3, 4);
ESPRotary r01 = ESPRotary(5, 6);
ESPRotary r02 = ESPRotary(7, 8);
ESPRotary r10 = ESPRotary(13, 14);
ESPRotary r11 = ESPRotary(15, 16);
ESPRotary r12 = ESPRotary(17, 18);

/**
 * \brief 按键表
 */
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
	{0x1E, 0x24}
};

/**
 * \brief 编码器按键表
 */
std::map<byte, byte> encoderValue{
	{0x03, 0x00},
	{0x0A, 0x01},
	{0x011, 0x02},

	{0x04, 0x10},
	{0x0B, 0x11},
	{0x12, 0x12}
};

/**
 * \brief 输出键值
 * \param keyValue 
 */
void outKey(byte keyValue)
{
	byte a[] = {0xFF, 0x00, 0x00, 0x00, 0x00, 0x0D, 0x0A};
	byte keyValue0;

	if (keyValue < 0x80)
	{
		keyValue0 = keyValue;
		a[4] = 0x01;
	}
	else
	{
		keyValue0 = keyValue - 0x80;
		a[4] = 0x02;
	}

	auto it = keyBoardValue.find(keyValue0);

	if (it != keyBoardValue.end())
	{
		//设置类型为按键
		a[2] = 0x01;
		//设置按键位
		a[3] = keyBoardValue[keyValue0];
	}
	else
	{
		it = encoderValue.find(keyValue0);
		if (it != encoderValue.end())
		{
			//设置类型为编码器 0x02
			a[2] = 0x02;

			//设置编码器位
			a[3] = encoderValue[keyValue0];

			//设置编码器值
			a[4] = a[4] + 0x02;
		}
		else
		{
			return;
		}
	}
	USBSerial.write(a, sizeof(a));
	HWSerial.write(a, sizeof(a));
}

/**
 * \brief 输出编码器值
 * \param direction 方向
 * \param encoderValue 编码器位置
 */
void outEncoder(byte direction, byte encoderValue)
{
	byte a[] = { 0xFF, 0x00, 0x02, encoderValue, 0x00, 0x0D, 0x0A };
	switch (direction)
	{
	case RE_LEFT:
		a[4] = 0x02;
		break;
	case RE_RIGHT:
		a[4] = 0x01;
		break;
	default:
		return;
	}
	USBSerial.write(a, sizeof(a));
	HWSerial.write(a, sizeof(a));
}

void r00RotationHandler(ESPRotary& r)
{
	outEncoder(r.getDirection(), 0x00);
}
void r01RotationHandler(ESPRotary& r)
{
	outEncoder(r.getDirection(), 0x01);
}
void r02RotationHandler(ESPRotary& r)
{
	outEncoder(r.getDirection(), 0x02);
}
void r10RotationHandler(ESPRotary& r)
{
	outEncoder(r.getDirection(), 0x10);
}
void r11RotationHandler(ESPRotary& r)
{
	outEncoder(r.getDirection(), 0x11);
}
void r12RotationHandler(ESPRotary& r)
{
	outEncoder(r.getDirection(), 0x12);
}

// the setup function runs once when you press reset or power the board
void setup()
{
	HWSerial.begin(115200);
	HWSerial.setDebugOutput(true);

	USBSerial.begin();
	USB.begin();

	swSerial.begin(9600);
	Keypad.setDetectMode(1);

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

// the loop function runs over and over again until power down or reset
void loop()
{
	Keypad.checkChanges();
	if (Keypad.isKeyChanged())
	{
		outKey(Keypad.getKeyValue());
	}

	r00.loop();
	r01.loop();
	r02.loop();
	r10.loop();
	r11.loop();
	r12.loop();
}
