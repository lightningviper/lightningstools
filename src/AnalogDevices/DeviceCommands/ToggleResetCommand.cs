using System.Threading;

namespace AnalogDevices.DeviceCommands
{
    internal interface IToggleReset
    {
        void ToggleReset();
    }

    internal class ToggleResetCommand : IToggleReset
    {
        private readonly IDenseDacEvalBoard _evalBoard;
        private readonly ILockFactory _lockFactory;
        private readonly ISetRESETPinHigh _setRESETPinHighCommand;
        private readonly ISetRESETPinLow _setRESETPinLowCommand;

        public ToggleResetCommand(
            IDenseDacEvalBoard evalBoard,
            ILockFactory lockFactory = null,
            ISetRESETPinHigh setResetPinHighCommand = null,
            ISetRESETPinLow setResetPinLowCommand = null)
        {
            _evalBoard = evalBoard;
            _lockFactory = lockFactory ?? new LockFactory();
            _setRESETPinHighCommand = setResetPinHighCommand ?? new SetRESETPinHighCommand(evalBoard);
            _setRESETPinLowCommand = setResetPinLowCommand ?? new SetRESETPinLowCommand(evalBoard);
        }

        public void ToggleReset()
        {
            using (_lockFactory.GetLock(LockType.CommandLock))
            {
                _evalBoard.DeviceState.ClearRegisterCache();
                _setRESETPinHighCommand.SetRESETPinHigh();
                Thread.Sleep(1000);
                _setRESETPinLowCommand.SetRESETPinLow();
            }
        }
    }
}