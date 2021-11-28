#pragma once
#pragma warning (disable : 26495)
#ifndef DTSCARDNATIVE_H
#define DTSCARDNATIVE_H

#ifndef WIN32_LEAN_AND_MEAN
#define WIN32_LEAN_AND_MEAN
#endif
#include <windows.h>
#include <iostream>

#define DTS_SUCCESS				   0
#define DTS_ERR_UNDEFINED		   1
#define DTS_ERR_NO_CALLBACK	   2
#define DTS_ERR_SEND_PENDING	   3
#define DTS_ERR_CARD_NOT_FOUND   4
#define DTS_ERR_WIN_ERROR        5
#define DTS_ERR_TIMEOUT          6

class DTSCardNative {
private:
    HANDLE io_handles[2];
    OVERLAPPED recv_ov, send_ov;
    int recv_success, send_success;
    unsigned char InputPacketBuffer[65];
    unsigned char outputBuffer[5];
    HANDLE WriteHandle, ReadHandle;
    DWORD BytesRead, BytesWritten;

    unsigned int valueS1, valueS3;

    std::wstring serial;
    std::wstring errorMsg;

    void unicode2ascii(const wchar_t* strin, char* strout, const unsigned int maxLen);
    int sendValues();

public:
    DTSCardNative() {}

    // Used to set the serial number of the board that the object should control.
    void setSerial(const wchar_t* serial);

    /* Call this after setSerial() to connect to the board.
     * Will return DTS_ERR_UNDEFINED on error or DTS_SUCCESS
     * on success. */
    int init();

    /* Call regularly. This handles re-connect and transfer of data.
     * Will return DTS_ERR_UNDEFINED on error or DTS_SUCCESS on success. */
    int update();

    // Call before throwing away object.
    void clean();

    /* Return the Windows error message from a DTS_ERR_WIN_ERROR */
    const wchar_t* winError();

    /* This is to set the values of the D/A-converters on the board directly (0-4095).
     * Will fail with DTS_ERR_SEND_PENDING if the last transmission isn't finished.
     * Other return values are DTS_ERR_UNDEFINED and DTS_SUCCESS (indicates success). */
    int setValues(unsigned int valueS1, unsigned int valueS3);

    /* These are for setting the values of the D/A-converters individually.
     * Return values are the same as for setValues() */
    int setValueS1(unsigned int valueS1);
    int setValueS3(unsigned int valueS3);

    /* This is the one that should be used to transmit a synchro signal, the angle is in degrees.
     * Return values are the same as for setValues() */
    int setAngle(double angle);
};

#endif