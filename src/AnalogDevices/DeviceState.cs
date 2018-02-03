namespace AnalogDevices
{
    public class DeviceState
    {
        private readonly ILockFactory _lockFactory;
        internal ABSelectRegisterBits?[] ABSelectRegisters = new ABSelectRegisterBits?[5];
        internal ushort?[] CRegisters = new ushort?[40];
        internal ushort?[] MRegisters = new ushort?[40];
        internal ushort?[] X1ARegisters = new ushort?[40];
        internal ushort?[] X1BRegisters = new ushort?[40];
        internal ushort? OFS0Register;
        internal ushort? OFS1Register;
        internal bool SPI_Initialized;

        internal DeviceState(ILockFactory lockFactory = null)
        {
            _lockFactory = lockFactory ?? new LockFactory();
        }

        internal ControlRegisterBits? ControlRegister { get; set; }

        public bool UseRegisterCache { get; set; } = false;
        public DacPrecision Precision { get; set; } = DacPrecision.SixteenBit;

        public void ClearRegisterCache()
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                ABSelectRegisters = new ABSelectRegisterBits?[5];
                ControlRegister = null;
                CRegisters = new ushort?[40];
                MRegisters = new ushort?[40];
                X1ARegisters = new ushort?[40];
                X1BRegisters = new ushort?[40];
                OFS0Register = null;
                OFS1Register = null;
                SPI_Initialized = false;
            }
        }
    }
}