using System;

namespace SimLinkup.HardwareSupport.ArduinoSeat
{
    internal class ArduinoSeatCommunicationProtocolHeaders
    {
        [Flags]
        internal enum ArduinoSeatPacketFields : byte
        {
            MOTOR_STATES = 0x01,
            MOTOR_1_SPEED = 0x10,
            MOTOR_2_SPEED = 0x20,
            MOTOR_3_SPEED = 0x40,
            MOTOR_4_SPEED = 0x80
        };

        [Flags]
        internal enum MotorBits : byte
        {
            ALL_OFF = 0x00,
            MOTOR_1_STATE = 0x01,
            MOTOR_2_STATE = 0x02,
            MOTOR_3_STATE = 0x04,
            MOTOR_4_STATE = 0x08,
        }
    }
}
