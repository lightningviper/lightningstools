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
        private readonly Queue<UsbCommandOrQuery> _commandAndQueryQueue = new Queue<UsbCommandOrQuery>();
        private System.Timers.Timer _commandSendingTimer;
        private readonly bool _useCommandSendingTimer = true;
        private object _commandAndQueryQueueLockObj = new object();
        private int _minimumCommandSendingIntervalMilliseconds = 50;

        private struct UsbCommandOrQuery
        {
            public byte Subaddress { get; set; }
            public byte? Data { get; set; }
            public bool UsePseudoCobs { get; set; }

            public int? BytesToRead { get; set; }

            public Action<byte[]> Callback { get; set; }
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
            SendCommandOrQueryInternal(subaddress: subaddress, data: data, usePseudoCOBS: usePseudoCOBS);
        }
        public void SendQuery(byte subaddress, byte? data = null, int bytesToRead = 0, bool sendChecksumAndDelimiter = false, Action<byte[]> callback = null, bool invokeSynchronously = false)
        {
            SendCommandOrQueryInternal(subaddress: subaddress, data: data, bytesToRead: bytesToRead, usePseudoCOBS: sendChecksumAndDelimiter, callback: callback, invokeSynchronously: invokeSynchronously);
        }

        private void SendCommandOrQueryInternal(byte subaddress, byte? data = null, int? bytesToRead = null, bool usePseudoCOBS = false, Action<byte[]> callback = null, bool invokeSynchronously = false)
        {
            if (_useCommandSendingTimer && !invokeSynchronously)
            {
                var usbCommand = new UsbCommandOrQuery() { Subaddress = subaddress, Data = data, UsePseudoCobs = usePseudoCOBS, BytesToRead = bytesToRead, Callback = callback };
                lock (_commandAndQueryQueueLockObj)
                {
                    _commandAndQueryQueue.Enqueue(usbCommand);
                }
            }
            else
            {
                SendCommandOrQuerySynchronously(subaddress: subaddress, data: data, bytesToRead: bytesToRead, usePseudoCOBS: usePseudoCOBS, callback: callback);
            }
        }

        private void CommandSendingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            UsbCommandOrQuery? nextCommandOrQuery = null;
            nextCommandOrQuery = GetNextCommandOrQueryFromQueue();
            if (nextCommandOrQuery != null)
            {
                try
                {
                    SendCommandOrQuerySynchronously(subaddress: nextCommandOrQuery.Value.Subaddress, data: nextCommandOrQuery.Value.Data, bytesToRead: nextCommandOrQuery.Value.BytesToRead, usePseudoCOBS: nextCommandOrQuery.Value.UsePseudoCobs, callback: nextCommandOrQuery.Value.Callback);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception in command sending:{ex}");
                }
            }
        }

        private UsbCommandOrQuery? GetNextCommandOrQueryFromQueue()
        {
            lock (_commandAndQueryQueueLockObj)
            {
                UsbCommandOrQuery? laterCommandOrQuery = null;
                var nextCommandOrQuery =  _commandAndQueryQueue.Count > 0 ? _commandAndQueryQueue.Dequeue() : (UsbCommandOrQuery?)null;
                if (nextCommandOrQuery != null)
                {
                    do
                    {
                        laterCommandOrQuery = null;
                        laterCommandOrQuery = LaterQueuedCommandInSameCommandGroupAs(nextCommandOrQuery.Value);
                        if (laterCommandOrQuery != null)
                        {
                            nextCommandOrQuery = laterCommandOrQuery;
                            _commandAndQueryQueue.Dequeue();
                        }
                    }
                    while (laterCommandOrQuery != null);
                }
                return nextCommandOrQuery;
            }
        }

        private UsbCommandOrQuery? LaterQueuedCommandInSameCommandGroupAs(UsbCommandOrQuery commandOrQueryToEvaluate)
        {
            lock (_commandAndQueryQueueLockObj)
            {
                if (_commandAndQueryQueue.Count == 0 || CommandSubaddresses == null) return null;

                var commandGroup = GetCommandGroup(commandOrQueryToEvaluate.Subaddress);
                if (commandGroup == null) return null;

                var commands = _commandAndQueryQueue.ToList();
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
        
        private void SendCommandOrQuerySynchronously(byte subaddress, byte? data=null, int? bytesToRead = null, bool usePseudoCOBS = false, Action<byte[]> callback = null)
        {
            if (SerialPortConnection != null)
            {
                    if (bytesToRead.HasValue && bytesToRead.Value > 0)
                    {
                        SerialPortConnection.DiscardInputBuffer();
                    }

                    if (!usePseudoCOBS)
                    {
                        //if (SerialPortConnection.COMPort == "COM2" || SerialPortConnection.COMPort == "COM3")
                        //{
                        //    Console.WriteLine($"{DateTime.Now:O}: Writing command with subAddress:{subaddress} with value byte:{data ?? 0x00} to {SerialPortConnection.COMPort}");
                        //}
                        SerialPortConnection.Write(new[] { subaddress, data ?? 0x00 }, 0, 2);
                        //if (SerialPortConnection.COMPort == "COM2" || SerialPortConnection.COMPort == "COM3")
                        //{
                        //    Console.WriteLine($"{DateTime.Now:O}: Wrote command with subAddress:{subaddress} with value byte:{data ?? 0x00} to {SerialPortConnection.COMPort}");
                        //}
                }
                else
                    {
                        var checksum = (byte)((subaddress + (data ?? 0x00)) & 0x00FF);
                        var delimiter = (byte)0xFF;
                        //if (SerialPortConnection.COMPort == "COM2" || SerialPortConnection.COMPort == "COM3")
                        //{
                        //    Console.WriteLine($"{DateTime.Now:O}: Writing command with subAddress:{subaddress} with value byte:{data ?? 0x00}, checksum:{checksum}, delimiter:{delimiter} to {SerialPortConnection.COMPort}");
                        //}
                        SerialPortConnection.Write(new[] { subaddress, data ?? 0x00, checksum, delimiter }, 0, 4);
                        //if (SerialPortConnection.COMPort == "COM2" || SerialPortConnection.COMPort == "COM3")
                        //{
                        //    Console.WriteLine($"{DateTime.Now:O}: Wrote command with subAddress:{subaddress} with value byte:{data ?? 0x00}, checksum:{checksum}, delimiter:{delimiter} to {SerialPortConnection.COMPort}");
                        //}
                }

                if (bytesToRead.HasValue && bytesToRead.Value > 0)
                    {
                        var readBuffer = new byte[bytesToRead.Value];
                        SerialPortConnection.Read(readBuffer, 0, bytesToRead.Value);
                        //Console.WriteLine("Bytes read:");
                        //for (var i = 0; i < readBuffer.Length; i++) {
                        //    Console.WriteLine($"[{i}]:{readBuffer[i]:x}");
                        //}
                        callback(readBuffer);
                    }
            }
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
