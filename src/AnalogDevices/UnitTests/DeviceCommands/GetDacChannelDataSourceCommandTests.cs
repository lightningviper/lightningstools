using System;
using System.Collections;
using AnalogDevices.DeviceCommands;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class GetDacChannelDataSourceCommandTests
    {
        [Test]
        public void ShouldThrowExceptionIfChannelNumberIsOutOfRange(
            [Values((int) (ChannelAddress.Dac0 - 1), (int) (ChannelAddress.Dac39 + 1))] ChannelAddress channel)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => WhenGetDacChannelDataSource(channel));
        }

        [Test]
        [TestCaseSource(nameof(ABSelectRegisterCombinations))]
        public void ShouldReturnCorrectDacChannelDataSource(
            int abSelectRegisterNumber,
            uint? abSelectRegisterValue,
            ChannelAddress dacChannelAddress,
            DacChannelDataSource expectedDacChannelDataSource)
        {
            GivenABSelectRegisterValue(abSelectRegisterNumber, (ABSelectRegisterBits?) abSelectRegisterValue);
            WhenGetDacChannelDataSource(dacChannelAddress);
            ThenDacChannelDataSourceShouldBe(expectedDacChannelDataSource);
        }

        [Test]
        public void ShouldLock(
            [Values((int) ChannelAddress.Dac0)] ChannelAddress channel)
        {
            WhenGetDacChannelDataSource(channel);
            ThenLockWasEnteredFirstAndExitedAfter();
        }

        private void GivenABSelectRegisterValue(int registerNum, ABSelectRegisterBits? value)
        {
            _deviceState.ABSelectRegisters[registerNum] = value;
        }

        private void WhenGetDacChannelDataSource(ChannelAddress channelAddress)
        {
            _dacChannelDataSource = _getDacChannelDataSourceCommand.GetDacChannelDataSource(channelAddress);
        }

        private void ThenDacChannelDataSourceShouldBe(DacChannelDataSource dacChannelDataSource)
        {
            _dacChannelDataSource.Should().Be(dacChannelDataSource);
        }

        private void ThenLockWasEnteredFirstAndExitedAfter()
        {
            _lockEnteredCallOrder.Should().BeLessThan(_rbCallOrder);
            _lockExitedCallOrder.Should().BeGreaterThan(_rbCallOrder);
        }


        [SetUp]
        public void SetUp()
        {
            _fakeEvalBoard = Mock.Of<IDenseDacEvalBoard>();
            _deviceState = new DeviceState();
            Mock.Get(_fakeEvalBoard)
                .SetupGet(x => x.DeviceState)
                .Returns(_deviceState);

            _rb = Mock.Of<IReadbackABSelectRegisters>();
            Mock.Get(_rb)
                .Setup(x => x.ReadbackABSelectRegister(It.IsAny<int>()))
                .Callback(() => _rbCallOrder = _callOrder++)
                .Returns(new Func<int, ABSelectRegisterBits>(
                    registerNum => _deviceState.ABSelectRegisters[registerNum] ?? ABSelectRegisterBits.AllChannelsA));


            _fakeLockFactory = Mock.Of<ILockFactory>();
            Mock.Get(_fakeLockFactory)
                .Setup(x => x.GetLock(LockType.CommandLock))
                .Returns(new ObservableLock(
                    new object(),
                    () => _lockEnteredCallOrder = _callOrder++,
                    () => _lockExitedCallOrder = _callOrder++)
                );

            _getDacChannelDataSourceCommand = new GetDacChannelDataSourceCommand(_fakeEvalBoard, _rb, _fakeLockFactory);
        }

        public static IEnumerable ABSelectRegisterCombinations
        {
            get
            {
                for (var dacChannelNum = 0; dacChannelNum < 40; dacChannelNum++)
                {
                    var abSelectRegisterNumber = dacChannelNum / 8;
                    var offset = dacChannelNum % 8;
                    yield return new TestCaseData(
                        abSelectRegisterNumber,
                        (uint) (ABSelectRegisterBits.AllChannelsB & ~(ABSelectRegisterBits) (1 << offset)),
                        (ChannelAddress) (dacChannelNum + 8),
                        DacChannelDataSource.DataValueA
                    );
                    yield return new TestCaseData(
                        abSelectRegisterNumber,
                        (uint) (ABSelectRegisterBits.AllChannelsA | (ABSelectRegisterBits) (1 << offset)),
                        (ChannelAddress) (dacChannelNum + 8),
                        DacChannelDataSource.DataValueB
                    );
                }
            }
        }

        private IDenseDacEvalBoard _fakeEvalBoard;
        private DeviceState _deviceState;
        private IReadbackABSelectRegisters _rb;
        private IGetDacChannelDataSource _getDacChannelDataSourceCommand;
        private DacChannelDataSource _dacChannelDataSource;
        private ILockFactory _fakeLockFactory;
        private int _callOrder;
        private int _lockEnteredCallOrder;
        private int _lockExitedCallOrder;
        private int _rbCallOrder;
    }
}