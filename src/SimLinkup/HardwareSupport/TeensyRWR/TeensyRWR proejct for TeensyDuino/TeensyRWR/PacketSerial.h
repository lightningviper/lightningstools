#pragma once
#include <Arduino.h>
#include "COBS.h"

template<typename EncoderType, uint8_t PacketMarker = 0, size_t BufferSize = 256>
class PacketSerial_
{
  public:
    typedef void (*PacketHandlerFunction)(const uint8_t* buffer, size_t size);
    PacketSerial_(): _receiveBufferIndex(0), _stream(nullptr), _onPacketFunction(nullptr),
    {
    }
    ~PacketSerial_()
    {
    }

    void begin(unsigned long speed)
    {
      Serial.begin(speed);
      while (!Serial);
      setStream(&Serial);
    }

    void setStream(Stream* stream)
    {
      _stream = stream;
    }


    void update()
    {
      if (_stream == nullptr) return;

      while (_stream->available() > 0)
      {
        uint8_t data = _stream->read();

        if (data == PacketMarker)
        {
          if (_onPacketFunction )
          {
            uint8_t _decodeBuffer[_receiveBufferIndex];
            size_t numDecoded = EncoderType::decode(_receiveBuffer, _receiveBufferIndex, _decodeBuffer);

            if (_onPacketFunction)
            {
              _onPacketFunction(_decodeBuffer, numDecoded);
            }
          }

          _receiveBufferIndex = 0;
        }
        else
        {
          if ((_receiveBufferIndex + 1) < BufferSize)
          {
            _receiveBuffer[_receiveBufferIndex++] = data;
          }
        }
      }
    }

    void send(const uint8_t* buffer, size_t size) const
    {
      if (_stream == nullptr || buffer == nullptr || size == 0) return;
      uint8_t _encodeBuffer[EncoderType::getEncodedBufferSize(size)];
      size_t numEncoded = EncoderType::encode(buffer, size, _encodeBuffer);
      _stream->write(_encodeBuffer, numEncoded);
      _stream->write(PacketMarker);
    }

    void setPacketHandler(PacketHandlerFunction onPacketFunction)
    {
      _onPacketFunction = onPacketFunction;
    }

  private:
    PacketSerial_(const PacketSerial_&);
    PacketSerial_& operator = (const PacketSerial_&);

    uint8_t _receiveBuffer[BufferSize];
    size_t _receiveBufferIndex = 0;

    Stream* _stream = nullptr;

    PacketHandlerFunction _onPacketFunction = nullptr;
};


typedef PacketSerial_<COBS> PacketSerial;
typedef PacketSerial_<COBS> COBSPacketSerial;
