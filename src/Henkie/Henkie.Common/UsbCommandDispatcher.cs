using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Henkie.Common
{
    public class UsbCommandDispatcher: ICommandDispatcher
    {

        private bool _isDisposed = false;
        private ISerialPortConnection SerialPortConnection { get; set; }
        private readonly Queue<UsbCommand> _commandQueue = new Queue<UsbCommand>();
        private System.Timers.Timer _commandSendingTimer;
        private readonly bool _useCommandSendingTimer = true;
        private object _lockObj = new object();
        private int _minimumCommandSendingIntervalMilliseconds = 20;

        private struct UsbCommand
        {
            public byte Subaddress { get; set; }
            public byte? Data { get; set; }
            public bool UsePseudoCobs { get; set; }
        }

        private Type CommandSubaddresses { get; set; } = null;
        public UsbCommandDispatcher(ISerialPortConnection serialPortConnection)
        {
            SerialPortConnection = serialPortConnection;
            StartCommandSendingTimer();
        }

        public UsbCommandDispatcher(string COMPort, Type commandSubaddresses = null, bool useCommandSendingTimer = true)
        {
            if (commandSubaddresses != null)
            {
                CommandSubaddresses = commandSubaddresses;
                _useCommandSendingTimer = true;
            }
            SerialPortConnection = new SerialPortConnection(COMPort);
            _useCommandSendingTimer = useCommandSendingTimer;
            if (_useCommandSendingTimer)
            {
                StartCommandSendingTimer();
            }
        }
        private void StartCommandSendingTimer()
        {
            if (_useCommandSendingTimer)
            {
                _commandSendingTimer = new System.Timers.Timer(_minimumCommandSendingIntervalMilliseconds)
                {
                    AutoReset = true,
                };
                _commandSendingTimer.Elapsed += CommandSendingTimer_Elapsed;
                _commandSendingTimer.Enabled = true;
                _commandSendingTimer.Start();
            }
        }
        public void SendCommand(byte subaddress, byte? data = null, bool usePseudoCOBS = false)
        {
            if (_useCommandSendingTimer)
            {
                var usbCommand = new UsbCommand() { Subaddress = subaddress, Data = data, UsePseudoCobs = usePseudoCOBS };
                lock (_lockObj)
                {
                    _commandQueue.Enqueue(usbCommand);
                }
            }
            else
            {
                lock (_lockObj)
                {
                    SendCommandInternal(subaddress, data, usePseudoCOBS);
                }
            }
        }
        
        private void CommandSendingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            UsbCommand? nextCommand = null;
            nextCommand = GetNextCommandFromQueue();
            if (nextCommand != null)
            {
                lock (_lockObj)
                {
                    SendCommandInternal(nextCommand.Value.Subaddress, nextCommand.Value.Data, nextCommand.Value.UsePseudoCobs);
                }
            }
        }

        private UsbCommand? GetNextCommandFromQueue()
        {
            lock (_lockObj)
            {
                UsbCommand? laterCommand = null;
                var nextCommand =  _commandQueue.Count > 0 ? _commandQueue.Dequeue() : (UsbCommand?)null;
                if (nextCommand != null)
                {
                    do
                    {
                        laterCommand = null;
                        laterCommand = LaterQueuedCommandInSameCommandGroupAs(nextCommand.Value);
                        if (laterCommand != null)
                        {
                            nextCommand = laterCommand;
                            _commandQueue.Dequeue();
                        }
                    }
                    while (laterCommand != null);
                }
                return nextCommand;
            }
        }

        private UsbCommand? LaterQueuedCommandInSameCommandGroupAs(UsbCommand commandToEvaluate)
        {
            lock (_lockObj)
            {
                if (_commandQueue.Count == 0 || CommandSubaddresses == null) return null;

                var commandGroup = GetCommandGroup(commandToEvaluate.Subaddress);
                if (commandGroup == null) return null;

                var commands = _commandQueue.ToList();
                foreach (var command in commands)
                {
                    var thisItemCommandGroup = GetCommandGroup(command.Subaddress);
                    if (thisItemCommandGroup == null) continue;
                    if (thisItemCommandGroup == commandGroup) return command;
                }
                return null;
            }
        }
        
        private string GetCommandGroup(byte subAddress)
        {
            if (!(Enum.ToObject(CommandSubaddresses, subAddress) is Enum enumMember)) return null;
            var commandGroupAttribute = enumMember.GetAttribute<CommandGroupAttribute>();
            if (commandGroupAttribute == null) return null;
            return commandGroupAttribute.CommandGroupName;
        }
        
        private void SendCommandInternal(byte subaddress, byte? data=null, bool usePseudoCOBS = false)
        {
            if (SerialPortConnection != null)
            {
                lock (_lockObj)
                {
                    if (!usePseudoCOBS)
                    {
                        SerialPortConnection.Write(new[] { subaddress, data ?? 0x00 }, 0, 2);
                        //Console.WriteLine($"Writing command with subAddress:{subaddress} with value byte:{data.Value} to {SerialPortConnection.COMPort}");
                    }
                    else
                    {
                        var checksum = (byte)((subaddress + (data ?? 0x00)) & 0x00FF);
                        var delimiter = (byte)0xFF;
                        SerialPortConnection.Write(new[] { subaddress, data ?? 0x00, checksum, delimiter }, 0, 4);
                        //Console.WriteLine($"{DateTime.Now:O}: Writing command with subAddress:{subaddress} with value byte:{data ?? 0x00}, checksum:{checksum}, delimiter:{delimiter} to {SerialPortConnection.COMPort}");
                        //Console.WriteLine($"{DateTime.Now.ToString("O")}: Writing {(subaddress <4 ? "BEARING " :subaddress >=4 && subaddress <=7 ? "HEADING " : null) } command with subAddress:{subaddress} with value byte:{data.Value}, checksum:{checksum}, delimiter:{delimiter} to {SerialPortConnection.COMPort}");
                    }
                }
            }
        }
        
        public byte[] SendQuery(byte subaddress, byte? data = null, int bytesToRead = 0, bool usePsuedoCOBS = false)
        {
            if (bytesToRead <0)
            {
                throw new ArgumentOutOfRangeException(nameof(bytesToRead));
            }
            if (SerialPortConnection != null)
            {
                lock (_lockObj)
                {
                    if (bytesToRead > 0)
                    {
                        SerialPortConnection.DiscardInputBuffer();
                    }
                    SendCommandInternal(subaddress, data, usePsuedoCOBS);
                    if (bytesToRead > 0)
                    {
                        var readBuffer = new byte[bytesToRead];
                        SerialPortConnection.Read(readBuffer, 0, bytesToRead);
                        //Console.WriteLine("Bytes read:");
                        //for (var i = 0; i < readBuffer.Length; i++) {
                        //    Console.WriteLine($"[{i}]:{readBuffer[i]:x}");
                        //}
                        return readBuffer;
                    }
                }
            }
            return null;
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~UsbCommandDispatcher()
        {
            Dispose();
        }

        private void Dispose(bool disposing)
        {
            if (disposing && !_isDisposed)
            {
                SerialPortConnection?.Dispose();
                _commandSendingTimer?.Dispose();
            }
            _isDisposed = true;
        }
    }
}
