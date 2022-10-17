/******************************************************************************
*  longpress.ino
*  BC6xxx & BC759x Key Scan Interface Library Example Code
*
*  This code uses SofewareSerial to connect a BC6xxx or BC759x chip. it can
*  detect key presses.
*  The hardware Serial is also used to communicate with computer. When a key is
*  pressed, the key value is printed to the hardware Serial port and thus can be
*  seen using the Serial Monitor
*  The SoftwareSerial uses digital I/O pin 11 as Rx and pin 12 as Tx. (Tx not
*  used in this example.)
*  The Key-Scan Library can use hardware serial such as Serial,Serial1, or 
*  Serial2 too.
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
*     V2.0 May 2021.  Changed to use Serial to print key value
*
*  License:
*     MIT license. It can be used for both open source and commercial projects.
******************************************************************************/
#include <bc_key_scan.h>

#include <SoftwareSerial.h>

SoftwareSerial swSerial(11, 12);          // creating SoftwareSerial instance, using pin 11 as Rx, 12 as Tx (Tx not used in this example)
BcKeyScan      Keypad(swSerial);        // creating BcKeyScan instance using software serial port swSerial


void setup() {
  Serial.begin(9600);           // Initialize Serial
  swSerial.begin(9600);        // Initialize swSerial
}

void loop() {
    // put your main code here, to run repeatedly:
    Keypad.checkChanges();            // let the key scan library to update key status
    if (Keypad.isKeyChanged() == true) {       // if there is any detectable key change
      Serial.println(Keypad.getKeyValue());   // print key value on Serial (use Serial Monitor to see it!)
    }
    delay(10);                     // delay 10ms
}
