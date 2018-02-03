namespace AnalogDevices.DeviceCommands
{
    internal interface ISendSpecialFunction
    {
        void SendSpecialFunction(SpecialFunctionCode specialFunctionCode, ushort data);
    }

    internal class SendSpecialFunctionCommand : ISendSpecialFunction
    {
        private readonly ILockFactory _lockFactory;
        private readonly ISendSPI _sendSPICommand;

        public SendSpecialFunctionCommand(
            IDenseDacEvalBoard evalBoard,
            ISendSPI sendSPICommand = null,
            ILockFactory lockFactory = null)
        {
            _sendSPICommand = sendSPICommand ?? new SendSPICommand(evalBoard);
            _lockFactory = lockFactory ?? new LockFactory();
        }

        public void SendSpecialFunction(SpecialFunctionCode specialFunctionCode, ushort data)
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                var value = (uint) ((((byte) specialFunctionCode & (byte) BasicMasks.SixBits) << 16) | data);
                _sendSPICommand.SendSPI(value);
            }
        }
    }
}