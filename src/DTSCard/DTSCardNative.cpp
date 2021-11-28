#pragma once
#define _USE_MATH_DEFINES
#include <cmath>
#include "DTSCardNative.h"


#include <string>
#include <WinIoCtl.h>
#include <Setupapi.h>
#include <vector>
#include <algorithm>
#include <ctime>
#define INITGUID
#include <DEVPKEY.H>

using std::string;
using std::vector;

#pragma comment (lib, "Setupapi.lib")

#define HID_CTL_CODE(id) \
   CTL_CODE (FILE_DEVICE_KEYBOARD, (id), METHOD_NEITHER, FILE_ANY_ACCESS)
#define HID_BUFFER_CTL_CODE(id) \
   CTL_CODE (FILE_DEVICE_KEYBOARD, (id), METHOD_BUFFERED, FILE_ANY_ACCESS)
#define HID_IN_CTL_CODE(id) \
   CTL_CODE (FILE_DEVICE_KEYBOARD, (id), METHOD_IN_DIRECT, FILE_ANY_ACCESS)
#define HID_OUT_CTL_CODE(id) \
   CTL_CODE (FILE_DEVICE_KEYBOARD, (id), METHOD_OUT_DIRECT, FILE_ANY_ACCESS)


#define IOCTL_GET_PHYSICAL_DESCRIPTOR         HID_OUT_CTL_CODE(102)
#define IOCTL_HID_FLUSH_QUEUE                 HID_CTL_CODE(101)
#define IOCTL_HID_GET_COLLECTION_DESCRIPTOR   HID_CTL_CODE(100)
#define IOCTL_HID_GET_COLLECTION_INFORMATION  HID_BUFFER_CTL_CODE(106)
#define IOCTL_HID_GET_FEATURE                 HID_OUT_CTL_CODE(100)
#define IOCTL_HID_GET_HARDWARE_ID             HID_OUT_CTL_CODE(103)
#define IOCTL_HID_GET_INDEXED_STRING          HID_OUT_CTL_CODE(120)
#define IOCTL_HID_GET_INPUT_REPORT            HID_OUT_CTL_CODE(104)
#define IOCTL_HID_GET_MANUFACTURER_STRING     HID_OUT_CTL_CODE(110)
#define IOCTL_GET_NUM_DEVICE_INPUT_BUFFERS    HID_BUFFER_CTL_CODE(104)
#define IOCTL_HID_GET_POLL_FREQUENCY_MSEC     HID_BUFFER_CTL_CODE(102)
#define IOCTL_HID_GET_PRODUCT_STRING          HID_OUT_CTL_CODE(111)
#define IOCTL_HID_GET_SERIALNUMBER_STRING     HID_OUT_CTL_CODE(112)
#define IOCTL_HID_SET_FEATURE                 HID_IN_CTL_CODE(100)
#define IOCTL_SET_NUM_DEVICE_INPUT_BUFFERS    HID_BUFFER_CTL_CODE(105)
#define IOCTL_HID_SET_OUTPUT_REPORT           HID_IN_CTL_CODE(101)
#define IOCTL_HID_SET_POLL_FREQUENCY_MSEC     HID_BUFFER_CTL_CODE(103)

#define MY_DEVICE_ID  "Vid_04d8&Pid_f64e"

void DTSCardNative::setSerial(const wchar_t* serial)
{
    this->serial = serial;
}

int DTSCardNative::init() {
    /*
    Before we can "connect" our application to our USB embedded device, we must first find the device.
    A USB bus can have many devices simultaneously connected, so somehow we have to find our device, and only
    our device.  This is done with the Vendor ID (VID) and Product ID (PID).  Each USB product line should have
    a unique combination of VID and PID.

    Microsoft has created a number of functions which are useful for finding plug and play devices.  Documentation
    for each function used can be found in the MSDN library.  We will be using the following functions:

    SetupDiGetClassDevs()					//provided by setupapi.dll, which comes with Windows
    SetupDiEnumDeviceInterfaces()			//provided by setupapi.dll, which comes with Windows
    GetLastError()							//provided by kernel32.dll, which comes with Windows
    SetupDiDestroyDeviceInfoList()			//provided by setupapi.dll, which comes with Windows
    SetupDiGetDeviceInterfaceDetail()		//provided by setupapi.dll, which comes with Windows
    SetupDiGetDeviceRegistryProperty()		//provided by setupapi.dll, which comes with Windows
    malloc()								//part of C runtime library, msvcrt.dll?
    CreateFile()							//provided by kernel32.dll, which comes with Windows

    We will also be using the following unusual data types and structures.  Documentation can also be found in
    the MSDN library:

    PSP_DEVICE_INTERFACE_DATA
    PSP_DEVICE_INTERFACE_DETAIL_DATA
    SP_DEVINFO_DATA
    HDEVINFO
    HANDLE
    GUID

    The ultimate objective of the following code is to call CreateFile(), which opens a communications
    pipe to a specific device (such as a HID class USB device endpoint).  CreateFile() returns a "handle"
    which is needed later when calling ReadFile() or WriteFile().  These functions are used to actually
    send and receive application related data to/from the USB peripheral device.

    However, in order to call CreateFile(), we first need to get the device path for the USB device
    with the correct VID and PID.  Getting the device path is a multi-step round about process, which
    requires calling several of the SetupDixxx() functions provided by setupapi.dll.
    */

    WriteHandle = ReadHandle = recv_ov.hEvent = send_ov.hEvent = INVALID_HANDLE_VALUE;

    //Globally Unique Identifier (GUID) for HID class devices.  Windows uses GUIDs to identify things.
    GUID InterfaceClassGuid = { 0x4d1e55b2, 0xf16f, 0x11cf, 0x88, 0xcb, 0x00, 0x11, 0x11, 0x00, 0x00, 0x30 };
    //GUID InterfaceClassGuid;
    //HidD_GetHidGuid(&InterfaceClassGuid);

    HDEVINFO DeviceInfoTable = INVALID_HANDLE_VALUE;
    PSP_DEVICE_INTERFACE_DATA InterfaceDataStructure = new SP_DEVICE_INTERFACE_DATA;
    PSP_DEVICE_INTERFACE_DETAIL_DATA DetailedInterfaceDataStructure = 0;
    SP_DEVINFO_DATA DevInfoData;

    DWORD InterfaceIndex = 0;
    DWORD StatusLastError = 0;
    DWORD dwRegType;
    DWORD dwRegSize;
    DWORD StructureSize = 0;
    PBYTE PropertyValueBuffer;
    size_t MatchFound;
    DWORD ErrorStatus;

    string DeviceIDToFind(MY_DEVICE_ID);


    //First populate a list of plugged in devices (by specifying "DIGCF_PRESENT"), which are of the specified class GUID. 
    DeviceInfoTable = SetupDiGetClassDevs(&InterfaceClassGuid, NULL, NULL, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);

    //Now look through the list we just populated.  We are trying to see if any of them match our device. 
    while (true)
    {
        InterfaceDataStructure->cbSize = sizeof(SP_DEVICE_INTERFACE_DATA);
        if (!SetupDiEnumDeviceInterfaces(DeviceInfoTable, NULL, &InterfaceClassGuid, InterfaceIndex, InterfaceDataStructure))
        {
            ErrorStatus = GetLastError();
            if (ERROR_NO_MORE_ITEMS == ErrorStatus)	//Did we reach the end of the list of matching devices in the DeviceInfoTable?
            {	//Cound not find the device.  Must not have been attached.
                SetupDiDestroyDeviceInfoList(DeviceInfoTable);	//Clean up the old structure we no longer need.
                delete InterfaceDataStructure;
                return DTS_ERR_CARD_NOT_FOUND;
            }
            else
            {
                LPVOID lpMsgBuf;

                FormatMessage(
                    FORMAT_MESSAGE_ALLOCATE_BUFFER |
                    FORMAT_MESSAGE_FROM_SYSTEM |
                    FORMAT_MESSAGE_IGNORE_INSERTS,
                    NULL,
                    ErrorStatus,
                    MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
                    (LPTSTR)&lpMsgBuf,
                    0, NULL);
                errorMsg = (wchar_t*)lpMsgBuf;
                LocalFree(lpMsgBuf);

                SetupDiDestroyDeviceInfoList(DeviceInfoTable);	//Clean up the old structure we no longer need.
                delete InterfaceDataStructure;
                return DTS_ERR_WIN_ERROR;
            }
        }

        //Now retrieve the hardware ID from the registry.  The hardware ID contains the VID and PID, which we will then 
        //check to see if it is the correct device or not.

        //Initialize an appropriate SP_DEVINFO_DATA structure.  We need this structure for SetupDiGetDeviceRegistryProperty().
        DevInfoData.cbSize = sizeof(SP_DEVINFO_DATA);
        SetupDiEnumDeviceInfo(DeviceInfoTable, InterfaceIndex, &DevInfoData);

        //First query for the size of the hardware ID, so we can know how big a buffer to allocate for the data.
        SetupDiGetDeviceRegistryProperty(DeviceInfoTable, &DevInfoData, SPDRP_HARDWAREID, &dwRegType, NULL, 0, &dwRegSize);

        //Allocate a buffer for the hardware ID.
        PropertyValueBuffer = (BYTE*)malloc(dwRegSize);
        if (PropertyValueBuffer == NULL)	//if null, error, couldn't allocate enough memory
        {	//Can't really recover from this situation, just exit instead.
            SetupDiDestroyDeviceInfoList(DeviceInfoTable);	//Clean up the old structure we no longer need.
            delete InterfaceDataStructure;
            return DTS_ERR_UNDEFINED;
        }

        //Retrieve the hardware IDs for the current device we are looking at.  PropertyValueBuffer gets filled with a 
        //REG_MULTI_SZ (array of null terminated strings).  To find a device, we only care about the very first string in the
        //buffer, which will be the "device ID".  The device ID is a string which contains the VID and PID, in the example 
        //format "Vid_04d8&Pid_003f".

        SetupDiGetDeviceRegistryProperty(DeviceInfoTable, &DevInfoData, SPDRP_HARDWAREID, &dwRegType, PropertyValueBuffer, dwRegSize, NULL);

        char* PropertyValueBufferASCII = new char[dwRegSize + 1];
        unicode2ascii((const wchar_t*)PropertyValueBuffer, PropertyValueBufferASCII, dwRegSize);

        //Now check if the first string in the hardware ID matches the device ID of my USB device.
        string DeviceIDFromRegistry(PropertyValueBufferASCII);

        free(PropertyValueBuffer);		//No longer need the PropertyValueBuffer, free the memory to prevent potential memory leaks
        PropertyValueBuffer = 0;
        delete[] PropertyValueBufferASCII;
        PropertyValueBufferASCII = 0;

        SetupDiGetDeviceRegistryProperty(DeviceInfoTable, &DevInfoData, SPDRP_PHYSICAL_DEVICE_OBJECT_NAME, &dwRegType, NULL, 0, &dwRegSize);
        //Allocate a buffer for the device object name.
        PropertyValueBuffer = (BYTE*)malloc(dwRegSize);
        if (PropertyValueBuffer == NULL)	//if null, error, couldn't allocate enough memory
        {	//Can't really recover from this situation, just exit instead.
            SetupDiDestroyDeviceInfoList(DeviceInfoTable);	//Clean up the old structure we no longer need.
            delete InterfaceDataStructure;
            return DTS_ERR_UNDEFINED;
        }
        SetupDiGetDeviceRegistryProperty(DeviceInfoTable, &DevInfoData, SPDRP_PHYSICAL_DEVICE_OBJECT_NAME, &dwRegType, PropertyValueBuffer, dwRegSize, NULL);

        PropertyValueBufferASCII = new char[dwRegSize + 1];
        unicode2ascii((const wchar_t*)PropertyValueBuffer, PropertyValueBufferASCII, dwRegSize);

        string DeviceObjectNameFromRegistry(PropertyValueBufferASCII);

        free(PropertyValueBuffer);		//No longer need the PropertyValueBuffer, free the memory to prevent potential memory leaks
        PropertyValueBuffer = 0;
        delete[] PropertyValueBufferASCII;
        PropertyValueBufferASCII = 0;

        DEVPROPTYPE PropertyType;
        DWORD size;
        SetupDiGetDeviceProperty(DeviceInfoTable, &DevInfoData, &DEVPKEY_Device_InstanceId, &PropertyType, NULL, 0, &size, 0);
        //Allocate a buffer for the hardware ID.
        PropertyValueBuffer = (BYTE*)malloc(size);
        if (PropertyValueBuffer == NULL)	//if null, error, couldn't allocate enough memory
        {	//Can't really recover from this situation, just exit instead.
            SetupDiDestroyDeviceInfoList(DeviceInfoTable);	//Clean up the old structure we no longer need.
            delete InterfaceDataStructure;
            return DTS_ERR_UNDEFINED;
        }
        SetupDiGetDeviceProperty(DeviceInfoTable, &DevInfoData, &DEVPKEY_Device_InstanceId, &PropertyType, PropertyValueBuffer, size, NULL, 0);

        PropertyValueBufferASCII = new char[size + 1];
        unicode2ascii((const wchar_t*)PropertyValueBuffer, PropertyValueBufferASCII, size);

        string DeviceInstanceIdentifier(PropertyValueBufferASCII);

        free(PropertyValueBuffer);		//No longer need the PropertyValueBuffer, free the memory to prevent potential memory leaks
        PropertyValueBuffer = 0;
        delete[] PropertyValueBufferASCII;
        PropertyValueBufferASCII = 0;

        //Convert both strings to lower case.  This makes the code more robust/portable accross OS Versions
        transform(DeviceIDFromRegistry.begin(), DeviceIDFromRegistry.end(), DeviceIDFromRegistry.begin(), tolower);
        transform(DeviceIDToFind.begin(), DeviceIDToFind.end(), DeviceIDToFind.begin(), tolower);

        //Now check if the hardware ID we are looking at contains the correct VID/PID
        MatchFound = DeviceIDFromRegistry.find(DeviceIDToFind);
        if (MatchFound != string::npos)
        {
            //Device must have been found.  Open read and write handles.  In order to do this, we will need the actual device path first.
            //We can get the path by calling SetupDiGetDeviceInterfaceDetail(), however, we have to call this function twice:  The first
            //time to get the size of the required structure/buffer to hold the detailed interface data, then a second time to actually 
            //get the structure (after we have allocated enough memory for the structure.)

            //First call populates "StructureSize" with the correct value
            SetupDiGetDeviceInterfaceDetail(DeviceInfoTable, InterfaceDataStructure, NULL, NULL, &StructureSize, NULL);
            DetailedInterfaceDataStructure = (PSP_DEVICE_INTERFACE_DETAIL_DATA)(malloc(StructureSize));		//Allocate enough memory
            if (DetailedInterfaceDataStructure == NULL)	//if null, error, couldn't allocate enough memory
            {	//Can't really recover from this situation, just exit instead.
                SetupDiDestroyDeviceInfoList(DeviceInfoTable);	//Clean up the old structure we no longer need.
                delete InterfaceDataStructure;
                return DTS_ERR_UNDEFINED;
            }
            DetailedInterfaceDataStructure->cbSize = sizeof(SP_DEVICE_INTERFACE_DETAIL_DATA);
            //Now call SetupDiGetDeviceInterfaceDetail() a second time to receive the goods.  
            SetupDiGetDeviceInterfaceDetail(DeviceInfoTable, InterfaceDataStructure, DetailedInterfaceDataStructure, StructureSize, NULL, NULL);

            //We now have the proper device path, and we can finally open read and write handles to the device.
            //We store the handles in the global variables "WriteHandle" and "ReadHandle", which we will use later to actually communicate.
            WriteHandle = CreateFile((DetailedInterfaceDataStructure->DevicePath), GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, NULL, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, 0);

            ErrorStatus = GetLastError();
            if (ErrorStatus != ERROR_SUCCESS) {
                LPVOID lpMsgBuf;

                FormatMessage(
                    FORMAT_MESSAGE_ALLOCATE_BUFFER |
                    FORMAT_MESSAGE_FROM_SYSTEM |
                    FORMAT_MESSAGE_IGNORE_INSERTS,
                    NULL,
                    ErrorStatus,
                    MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
                    (LPTSTR)&lpMsgBuf,
                    0, NULL);
                errorMsg = (wchar_t*)lpMsgBuf;
                LocalFree(lpMsgBuf);

                delete InterfaceDataStructure;
                free(DetailedInterfaceDataStructure);
                return DTS_ERR_WIN_ERROR;
            }

            //Check if the serial number is correct
            wchar_t cardSerial[126];
            DWORD bytesReturned;
            ErrorStatus = DeviceIoControl(WriteHandle,                       // device to be queried
                IOCTL_HID_GET_SERIALNUMBER_STRING, // operation to perform
                NULL, 0,                       // no input buffer
                cardSerial, sizeof(cardSerial),            // output buffer
                &bytesReturned,                         // # bytes returned, cannot be NULL when lpOverlapped is NULL, but contains no useful value after the function returns
                (LPOVERLAPPED)NULL);          // synchronous I/O

            if (!ErrorStatus) {
                ErrorStatus = GetLastError();
                LPVOID lpMsgBuf;

                FormatMessage(
                    FORMAT_MESSAGE_ALLOCATE_BUFFER |
                    FORMAT_MESSAGE_FROM_SYSTEM |
                    FORMAT_MESSAGE_IGNORE_INSERTS,
                    NULL,
                    ErrorStatus,
                    MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
                    (LPTSTR)&lpMsgBuf,
                    0, NULL);
                errorMsg = (wchar_t*)lpMsgBuf;
                LocalFree(lpMsgBuf);

                delete InterfaceDataStructure;
                free(DetailedInterfaceDataStructure);
                CloseHandle(WriteHandle);
                return DTS_ERR_WIN_ERROR;
            }

            // If the serial number is wrong, continue
            if (serial != cardSerial) {
                free(DetailedInterfaceDataStructure);
                CloseHandle(WriteHandle);
                InterfaceIndex++;
                continue;
            }

            // Open read handle too			
            ReadHandle = CreateFile((DetailedInterfaceDataStructure->DevicePath), GENERIC_READ, FILE_SHARE_READ | FILE_SHARE_WRITE, NULL, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, 0);

            ErrorStatus = GetLastError();
            if (ErrorStatus != ERROR_SUCCESS) {
                LPVOID lpMsgBuf;

                FormatMessage(
                    FORMAT_MESSAGE_ALLOCATE_BUFFER |
                    FORMAT_MESSAGE_FROM_SYSTEM |
                    FORMAT_MESSAGE_IGNORE_INSERTS,
                    NULL,
                    ErrorStatus,
                    MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
                    (LPTSTR)&lpMsgBuf,
                    0, NULL);
                errorMsg = (wchar_t*)lpMsgBuf;
                LocalFree(lpMsgBuf);

                CloseHandle(WriteHandle);
                delete InterfaceDataStructure;
                free(DetailedInterfaceDataStructure);
                return DTS_ERR_WIN_ERROR;
            }

            SetupDiDestroyDeviceInfoList(DeviceInfoTable);	//Clean up the old structure we no longer need.

            // Initialize variables
            recv_success = send_success = TRUE;

            recv_ov.Offset = 0;
            recv_ov.OffsetHigh = 0;

            send_ov.Offset = 0;
            send_ov.OffsetHigh = 0;

            // Create event handles
            recv_ov.hEvent = CreateEvent(NULL, TRUE, FALSE, NULL);
            send_ov.hEvent = CreateEvent(NULL, TRUE, FALSE, NULL);

            if (recv_ov.hEvent == NULL || send_ov.hEvent == NULL) {
                CloseHandle(ReadHandle);
                CloseHandle(WriteHandle);

                if (recv_ov.hEvent != NULL) {
                    CloseHandle(recv_ov.hEvent);
                }

                if (send_ov.hEvent != NULL) {
                    CloseHandle(send_ov.hEvent);
                }

                delete InterfaceDataStructure;
                free(DetailedInterfaceDataStructure);

                return DTS_ERR_UNDEFINED;
            }

            io_handles[0] = recv_ov.hEvent;
            io_handles[1] = send_ov.hEvent;

            delete InterfaceDataStructure;
            free(DetailedInterfaceDataStructure);

            return DTS_SUCCESS;
        }

        InterfaceIndex++;
        //Keep looping until we either find a device with matching VID and PID, or until we run out of items.
    }//end of while(true)
}

int DTSCardNative::update() {
    DWORD result;

    /*if (recv_success) {
       recv_success = ReadFile(ReadHandle, &InputPacketBuffer, 65, &BytesRead, &recv_ov);

       if (!recv_success) {
          int error = GetLastError();
          switch (error) {
          case ERROR_IO_PENDING:
             // This is what should happen if everything is working
             break;

          default:
             clean();

             return DTS_ERR_UNDEFINED;
             break;
          };
       }
    }*/

    result = WaitForMultipleObjects(2, io_handles, false, 0);

    switch (result) {
    case WAIT_OBJECT_0:
        recv_success = TRUE;
        GetOverlappedResult(ReadHandle, &recv_ov, &BytesRead, false);
        //ResetEvent(recv_ov.hEvent);
        break;

    case WAIT_OBJECT_0 + 1:
        send_success = TRUE;
        GetOverlappedResult(WriteHandle, &send_ov, &BytesWritten, false);
        ResetEvent(send_ov.hEvent);
        break;

    case WAIT_TIMEOUT:
        return DTS_ERR_TIMEOUT;// Don't have to do anything here
        break;

    case WAIT_FAILED:
        clean();
        return DTS_ERR_UNDEFINED;
        break;

    default:
        clean();
        return DTS_ERR_UNDEFINED;
        break;
    };

    /*if (recv_success) {
       vector<string> packets;
       string str;
       for (int i = 0; i < 64 && InputPacketBuffer[i + 1] != 0; i++) {
          if (InputPacketBuffer[i + 1] == '\n') {
             packets.push_back(str);
             str.empty();
          }
          else {
             str += InputPacketBuffer[i + 1];
          }
       }

       for (vector<string>::iterator it = packets.begin(); it != packets.end(); it++) {

       }
    }*/

    return DTS_SUCCESS;
}

void DTSCardNative::clean() {
    CloseHandle(WriteHandle);
    CloseHandle(ReadHandle);
    CloseHandle(recv_ov.hEvent);
    CloseHandle(send_ov.hEvent);

    WriteHandle = ReadHandle = INVALID_HANDLE_VALUE;
}

const wchar_t* DTSCardNative::winError() {
    return errorMsg.c_str();
}

int DTSCardNative::sendValues()
{
    if (send_success) {
        outputBuffer[0] = 0;
        outputBuffer[1] = (valueS1 >> 8) & 0x0F;
        outputBuffer[2] = valueS1 & 0xFF;
        outputBuffer[3] = (valueS3 >> 8) & 0x0F;
        outputBuffer[4] = valueS3 & 0xFF;
        send_success = WriteFile(WriteHandle, outputBuffer, 5, &BytesWritten, &send_ov);	//Blocking function, unless an "overlapped" structure is used

        if (!send_success) {
            DWORD ErrorStatus = GetLastError();
            switch (ErrorStatus) {
            case ERROR_IO_PENDING:
                // This is what should happen if everything is working
                return DTS_SUCCESS;
                break;

            default:
                clean();
                LPVOID lpMsgBuf;

                FormatMessage(
                    FORMAT_MESSAGE_ALLOCATE_BUFFER |
                    FORMAT_MESSAGE_FROM_SYSTEM |
                    FORMAT_MESSAGE_IGNORE_INSERTS,
                    NULL,
                    ErrorStatus,
                    MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
                    (LPTSTR)&lpMsgBuf,
                    0, NULL);
                errorMsg = (wchar_t*)lpMsgBuf;
                return DTS_ERR_WIN_ERROR;
                break;
            };
        }
        else {
            return DTS_SUCCESS;
        }
    }
    else {
        return DTS_ERR_SEND_PENDING;
    }
}

int DTSCardNative::setAngle(double angle) {
    valueS1 = (unsigned int)(sin(angle * M_PI / 180 - 2 * M_PI / 3) * 2048 + 2048);
    valueS3 = (unsigned int)(-sin(angle * M_PI / 180 + 2 * M_PI / 3) * 2048 + 2048);

    if (valueS1 > 4095)
        valueS1 = 4095;

    if (valueS3 > 4095)
        valueS3 = 4095;

    return sendValues();
}

int DTSCardNative::setValues(unsigned int valueS1, unsigned int valueS3) {
    this->valueS1 = valueS1;
    this->valueS3 = valueS3;

    return sendValues();
}

int DTSCardNative::setValueS1(unsigned int valueS1)
{
    this->valueS1 = valueS1;

    return sendValues();
}

int DTSCardNative::setValueS3(unsigned int valueS3)
{
    this->valueS3 = valueS3;

    return sendValues();
}

void DTSCardNative::unicode2ascii(const wchar_t* strin, char* strout, const unsigned int maxLen) {
    unsigned int i;

    for (i = 0; strin[i] != 0 && i < maxLen - 1; i++) {
        strout[i] = (char)strin[i];
    }

    strout[i] = 0;
}
