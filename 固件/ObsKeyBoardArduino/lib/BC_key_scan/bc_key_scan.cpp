/******************************************************************************
*  bc_key_scan.cpp
*  BC6xxx & BC759x Key Matrix Interface Driver Library
*  This library can be used with the following chips:
*    BC6301  --  30-key 5x6 matrix keyboard interface
*    BC6040  --  40-key 5x8 matrix keyboard interface
*    BC6561  --  56-key 7x8 matrix keybaord interface
*    BC6088  --  88-key 11x8 matrix keyboard interface
*    BC7595  --  48-key 6x8 matrix keyboard with 48 segments LED display
*    BC7591  --  96-key 24x4 matrix keyboard with 256 segments LED display
*
*  Dependencies:
*     This Library depends on the Arduino Serial or Software Serial library.
*
*  Author:
*     This library is written by BitCode. https://bitcode.com.cn
*
*  Version:
*     V1.0 March 2021
*     V2.0 July 2021
*
*  License:
*     MIT license. It can be used for both open source and commercial projects.
******************************************************************************/

#include "bc_key_scan.h"

const uint8_t FullBits[8] = { 0x00, 0x01, 0x03, 0x07, 0x0f, 0x1f, 0x3f, 0x7f };

/******************************************************************************
* Constructor
* This library should be initialized by hardware or software serial. If using
* software serial, It's user's responsibility to create an instance of 
* software serial before creating the instance of BcKeyScan.
******************************************************************************/
BcKeyScan::BcKeyScan(Stream& SerialPort)
    : Uart(SerialPort)
{
}

/******************************************************************************
* Check if there is new key press needs to be processed. 
* Input Parameters: none
* Return Value: bool. To be true(1) if there is new key
******************************************************************************/
bool BcKeyScan::isKeyChanged()
{
    return NewKeyAvailable;
}

/******************************************************************************
* Get key Value
* When there is a new key event ( is_key_changed() returns true ), use this 
* function to get the key value. is_key_changed() will return false after calling
* this function. This function can be called multiple times.
* Input Parameters: none
* Return Value: uint8_t. The key value will be returned.
******************************************************************************/
uint8_t BcKeyScan::getKeyValue()
{
    NewKeyAvailable = false;
    return ReportedKey;
}

/******************************************************************************
* Set Key Detect Mode
* Decide whether this library will detect the key release event. When Mode=0,
* only key pressing will be detected, while whem Mode=1 both press and release
* will be detected as a key event. Only 0 and 1 2 modes are supported currently.
* Input Parameters: uint8_t Mode. Working mode.
* Return Value: none
******************************************************************************/
void BcKeyScan::setDetectMode(const uint8_t Mode)
{
    DetectKeyRelease = Mode;
}

/******************************************************************************
* Set long-press count(time)
* When using long-press detection, The long-press count determines how much time
* will be considered a long-press. Time is represented as ticks, user will call
* long_press_tick() repeatly and the value set by this function will be used as
* a threshold to trigger a long-press key event.
* Input Parameters: uint16_t CountLimit. Tick Counts as threshold
* Return Value: none
******************************************************************************/
void BcKeyScan::setLongpressCount(const uint16_t CountLimit)
{
    LongPressCount = CountLimit;
}

/******************************************************************************
* Define Combined Key(s)
* If detection of combined keys is needed, they have to be defined by this
* function first. A combined key is a combination of up to 7 keys being pressed
* simultaneously. 
* Input Parameters:
*     uint8_t** pLPKeyList  - An array of defined combined keys
*     uint8_t* pCBKeyMap    - An array provided by user for the library to use
*                             as a buffer to store the status of each keys in
*                             key combinations.
*     uint8_t CBKeyCount    - Number of combined keys.
* Return Value: none
******************************************************************************/
//void BcKeyScan::defCombinedKey(const uint8_t** pCBKeyList, uint8_t* pCBKeyMap, uint8_t CBKeyCount)
void BcKeyScan::defCombinedKey(const uint8_t** pCBKeyList, uint8_t CBKeyCount)
{
    pCombinedKeys           = pCBKeyList;
    CombinedKeyCount        = CBKeyCount & 0x07;
    pCombinedKeyStatusArray = new uint8_t[CBKeyCount];
//    pCombinedKeyStatusArray = pCBKeyMap;
    for (uint8_t i = 0; i < CBKeyCount; i++)
    {
        *(pCombinedKeyStatusArray + i) = 0;
    }
}

/******************************************************************************
* Define Long-press Keys(s)
* If detection of long-press keys is needed, they must be difined by this function
* first. 
* Input Parameters:
*     uint8_t** pLPKeyList  - An array of keys to be detected
*     uint8_t LpKeyCount    - Number of long-press keys to be detected
* Return Value: none
******************************************************************************/
void BcKeyScan::defLongpressKey(const uint8_t** pLPKeyList, uint8_t LPKeyCount)
{
    pLongPressKeys    = pLPKeyList;
    LongPressKeyCount = LPKeyCount;
}

/******************************************************************************
* Set Callback function
* Another way to precess key events other than inquiring the is_key_changed()
* User creates a callback function to process the key events with a uint8_t
* input parameter and void return value, and use this function to let the
* library know, then every time a new key event is happened the callback
* function will be called. The key value will be passed as parameter to the
* callback function.
* Input Parameters:
*     void (*pCallbackFunc)(uint8_t)  - a pointer to the callback function
* Return Value: none
******************************************************************************/
void BcKeyScan::setCallback(void (*pCallbackFunc)(uint8_t))
{
    pCallback = pCallbackFunc;
}

/******************************************************************************
* Long Press Tick
* User calls this function to perform a long-press tick. When the tick counts to
* the CountLimit in set_longpress_count() and the last pressed key was one of
* the long-press keys to be watched, a long-press key event is generated. The 
* count will be cleared every time a key is pressed or released.
* Input Parameters: none
* Return Value: none
******************************************************************************/
void BcKeyScan::longpressTick()
{
    TimeCounter++;
    if (TimeCounter > LongPressCount)
    {
        TimeCounter = 0;
        if (pLongPressKeys != NULL)
        {
            for (uint8_t i = 0; i < LongPressKeyCount; i++)
            {
                if ((LastKeyEvent == *(*(pLongPressKeys + i)))
                    || ((LastKeyEvent & 0x80) && (*(*(pLongPressKeys + i)) == 0xFF)))
                {
                    ReportedKey     = *(*(pLongPressKeys + i) + 1);
                    NewKeyAvailable = true;
                    if (pCallback != NULL)
                    {
                        pCallback(ReportedKey);
                        NewKeyAvailable = false;
                    }
                    break;
                }
            }
        }
    }
}

/******************************************************************************
* Check Key Status Changes
* This function should be called periodically or from serial event.
* When it's called periodically, the interval should be less than 100ms to keep
* a quick response of the keyboard.
* Input Parameters: none
* Return Value: none
******************************************************************************/
void BcKeyScan::checkChanges()
{
    while (Uart.available())
    {
        updateKeyStatus(Uart.read());
    }
}

void BcKeyScan::updateKeyStatus(uint8_t RxData)
{
    uint8_t        i, j;
    uint8_t        PreviousCBKeyStat;
    const uint8_t* pCurrentKeySet;
    uint8_t        NumOfKeys;

    LastKeyEvent = RxData;
    TimeCounter  = 0;
    if (DetectKeyRelease || !(LastKeyEvent & 0x80))
    {
        ReportedKey     = LastKeyEvent;
        NewKeyAvailable = true;
    }
    if (CombinedKeyCount != 0)
    {
        for (i = 0; i < CombinedKeyCount; i++)
        {
            PreviousCBKeyStat = *(pCombinedKeyStatusArray + i);
            pCurrentKeySet    = *(pCombinedKeys + i);
            NumOfKeys         = *(pCurrentKeySet);
            for (j = 0; j < NumOfKeys; j++)
            {
                if ((LastKeyEvent & 0x7f) == *(pCurrentKeySet + 2 + j))
                {
                    if (LastKeyEvent & 0x80)
                    {
                        *(pCombinedKeyStatusArray + i) &= ~(1 << j);
                        if (PreviousCBKeyStat == FullBits[NumOfKeys])
                        {
                            LastKeyEvent = (*(pCurrentKeySet + 1)) | 0x80;
                            if (DetectKeyRelease)
                            {
                                ReportedKey     = LastKeyEvent;
                                NewKeyAvailable = true;
                            }
                        }
                    }
                    else
                    {
                        *(pCombinedKeyStatusArray + i) |= (1 << j);
                        if (*(pCombinedKeyStatusArray + i) == FullBits[NumOfKeys])
                        {
                            LastKeyEvent    = *(pCurrentKeySet + 1);
                            ReportedKey     = LastKeyEvent;
                            NewKeyAvailable = true;
                        }
                    }
                    break;
                }
            }
        }
    }
    if (NewKeyAvailable && (pCallback != NULL))
    {
        pCallback(ReportedKey);
        NewKeyAvailable = false;
    }
}
