#pragma once
#include "DTSCardNative.h"
#include <windows.h>
#include <vcclr.h>
#using <System.dll>

using namespace System;

namespace DTSCard
{
    public enum class DTSCardOperationStatus
    {
        Success=0,
        Undefined=1,
        NoCallback=2,
        SendPending=3,
        CardNotFound=4,
        WindowsError=5,
        Timeout=6
    };

    public ref class DTSCardManaged {
    public:
        // Allocate the native object on the C++ Heap via a constructor
        DTSCardManaged() : dtsCardNativeObject(new DTSCardNative) {}

        // Deallocate the native object on a destructor
        ~DTSCardManaged() {
            dtsCardNativeObject->clean();
            delete dtsCardNativeObject;
        }

    protected:
        // Deallocate the native object on the finalizer just in case no destructor is called
        !DTSCardManaged() {
            dtsCardNativeObject->clean();
            delete dtsCardNativeObject;
        }

    public:
        void SetSerial(String^ serial)
        {
            pin_ptr<const wchar_t> pinchars = PtrToStringChars(serial);
            return dtsCardNativeObject->setSerial(pinchars);
        }
        int Init() { return dtsCardNativeObject->init(); }
        int Update() { return dtsCardNativeObject->update(); }
        void Clean() { dtsCardNativeObject->clean(); }
        property String^ WinError{ String^ get() { return gcnew String(dtsCardNativeObject->winError()); }}
        int SetValueS1(unsigned int valueS1) { return dtsCardNativeObject->setValueS1(valueS1); }
        int SetValueS3(unsigned int valueS3) { return dtsCardNativeObject->setValueS3(valueS3); }
        int SetAngle(double angle) { return dtsCardNativeObject->setAngle(angle); }
        int SetValues(unsigned int valueS1, unsigned int valueS3) { return dtsCardNativeObject->setValues(valueS1, valueS3); }

    private:
        DTSCardNative* dtsCardNativeObject;
    };
}