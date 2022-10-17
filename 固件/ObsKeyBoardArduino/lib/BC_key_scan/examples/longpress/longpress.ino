/******************************************************************************
*  longpress.ino
*  BC6xxx & BC759x Key Scan Interface Library Example Code
*
*  This code uses SofewareSerial to connect a BC6xxx or BC759x chip. Besides
*  normal single key pressing, it can also detect the long-press of particular
*  keys.
*  Arduino built-in hardware Serial port is used to print the keypad detection 
*  results.
*  This software monitors keypad actions and print key values on Serial. When key 
*  1 or key 5 is pressed for more than 3s, a new key event with user defined key 
*  value of 120(0x78) or 121(0x79) is generated.
*  The SoftwareSerial uses digital I/O pin 11 as Rx and pin 12 as Tx. The Tx line
*  is not used in this example.
*  The SoftwareSerial can be replaced by hardware serial such as Serial1, or Serial2.
*  This code runs on any Arduino compatible boards.
*
*  Dependencies:
*     This code depends on the following libraries:
*        Arduino Software Serial Library
*        BitCode BC_key_scan Library
*
*  Author:
*     This software is written by BitCode. https://bitcode.com.cn
*
*  Version:
*     V1.0 March 2021
*     V2.0 May 2021, changed to use hardware Serial to print key values.
*
*  License:
*     MIT license. It can be used for both open source and commercial projects.
******************************************************************************/
#include <bc_key_scan.h>

#include <SoftwareSerial.h>

SoftwareSerial swSerial(11, 12);        // creating SoftwareSerial instance, using pin 11 as Rx, 12 as Tx
BcKeyScan      Keypad(swSerial);        // creating BcKeyScan instance using software serial port swSerial

// Definition of long-press keys
const unsigned char  lp1[2]    = { 1, 120 };          // Longpress key 1, detecting long-pressing of key 1 and use 120 as the user defined key value
const unsigned char  lp2[2]    = { 5, 121 };          // Longpress key 2, detecting long-pressing of key 5 and use 121 as the user defined key value
const unsigned char* LPList[2] = { lp1, lp2 };        // Longpress key list

void setup() {
  Serial.begin(9600);          // Initialize Serial
  swSerial.begin(9600);        // Initialize swSerial
  Keypad.defLongpressKey(LPList, 2);        // Define long-press keys
  Keypad.setLongpressCount(300);            // Set long-press time to 3s (10ms * 300)
}

void loop() {
    // put your main code here, to run repeatedly:
    Keypad.checkChanges();                // let the key scan library to update key status
    if (Keypad.isKeyChanged() == true) {  // if there is any detectable key change
      Serial.println(Keypad.getKeyValue());   // print key value on Serial (use Serial Monitor to see it!)
    }
    Keypad.longpressTick();        // Tick the long-press detector in BC_key_scan library
    delay(10);                     // delay 10ms
}
