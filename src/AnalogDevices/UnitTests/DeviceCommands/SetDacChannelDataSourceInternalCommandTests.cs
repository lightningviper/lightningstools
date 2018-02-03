using System;
using System.Collections;
using AnalogDevices.DeviceCommands;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class SetDacChannelDataSourceInternalCommandTests
    {
        [Test]
        public void ShouldThrowExceptionIfArgumentsAreOutOfRange(
            [Values((int) (ChannelAddress.Dac0 - 1), (int) (ChannelAddress.Dac39 + 1))] ChannelAddress channelAddress,
            [Values((int) (DacChannelDataSource.DataValueA - 1), (int) (DacChannelDataSource.DataValueB + 1))]
            DacChannelDataSource dacChannelDataSource)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => WhenSetDacChannelDataSourceInternal(channelAddress, dacChannelDataSource));
        }

        [Test]
        [TestCaseSource(nameof(TestData1))]
        public void ShouldUpdateCorrectABSelectRegister(
            ChannelAddress channelAddress,
            DacChannelDataSource dacChannelDataSource,
            int registerNum
        )
        {
            WhenSetDacChannelDataSourceInternal(channelAddress, dacChannelDataSource);
            ThenABSelectRegisterUpdatedShouldBe(registerNum);
        }


        [Test]
        [TestCaseSource(nameof(TestData2))]
        public void ShouldApplyExistingABSelectRegisterValuesWhenComputingNewValuesForSingleChannel(
            int registerNum,
            byte existingABSelectRegisterBits,
            ChannelAddress channelAddress,
            DacChannelDataSource dacChannelDataSource,
            byte newABSelectRegisterBits
        )
        {
            GivenExistingABSelectRegisterValues(registerNum, (ABSelectRegisterBits) existingABSelectRegisterBits);
            WhenSetDacChannelDataSourceInternal(channelAddress, dacChannelDataSource);
            ThenNewABSelectRegisterBitsShouldBe((ABSelectRegisterBits) newABSelectRegisterBits);
        }

        [Test]
        [TestCaseSource(nameof(TestData2))]
        public void ShouldUpdateABSelectRegisterCacheInDeviceState(
            int registerNum,
            byte existingABSelectRegisterBits,
            ChannelAddress channelAddress,
            DacChannelDataSource dacChannelDataSource,
            byte newABSelectRegisterBits
        )
        {
            GivenExistingABSelectRegisterValues(registerNum, (ABSelectRegisterBits) existingABSelectRegisterBits);
            WhenSetDacChannelDataSourceInternal(channelAddress, dacChannelDataSource);
            ThenABSelectRegisterCacheInDeviceStateShouldBe(registerNum, (ABSelectRegisterBits) newABSelectRegisterBits);
        }

        private void GivenExistingABSelectRegisterValues(int registerNum, ABSelectRegisterBits abSelectRegisterBits)
        {
            Mock.Get(_fakeReadbackABSelectRegistersCommand)
                .Setup(x => x.ReadbackABSelectRegister(registerNum))
                .Returns(abSelectRegisterBits)
                .Verifiable();
        }

        private void WhenSetDacChannelDataSourceInternal(ChannelAddress channelAddress,
            DacChannelDataSource dacChannelDataSource)
        {
            _setDacChannelDataSourceInternalCommand.SetDacChannelDataSourceInternal(channelAddress,
                dacChannelDataSource);
        }

        private void ThenABSelectRegisterUpdatedShouldBe(int registerNum)
        {
            _specialFunctionCodeSent.Should().Be(SpecialFunctionCode.WriteToABSelectRegister0 + registerNum);
        }

        private void ThenNewABSelectRegisterBitsShouldBe(ABSelectRegisterBits abSelectRegisterBits)
        {
            _dataSent.Should().Be((ushort) abSelectRegisterBits);
        }

        private void ThenABSelectRegisterCacheInDeviceStateShouldBe(int registerNum,
            ABSelectRegisterBits abSelectRegisterBits)
        {
            _deviceState.ABSelectRegisters[registerNum].Should().Be(abSelectRegisterBits);
        }


        [SetUp]
        public void SetUp()
        {
            _fakeEvalBoard = Mock.Of<IDenseDacEvalBoard>();
            _deviceState = new DeviceState();
            Mock.Get(_fakeEvalBoard)
                .SetupGet(x => x.DeviceState)
                .Returns(_deviceState);

            _fakeReadbackABSelectRegistersCommand = Mock.Of<IReadbackABSelectRegisters>();

            _fakeSendSpecialFunctionCommand = Mock.Of<ISendSpecialFunction>();
            Mock.Get(_fakeSendSpecialFunctionCommand)
                .Setup(x => x.SendSpecialFunction(It.IsAny<SpecialFunctionCode>(), It.IsAny<ushort>()))
                .Callback<SpecialFunctionCode, ushort>(
                    (a, b) =>
                    {
                        _specialFunctionCodeSent = a;
                        _dataSent = b;
                    }
                )
                .Verifiable();

            _setDacChannelDataSourceInternalCommand = new SetDacChannelDataSourceInternalCommand(_fakeEvalBoard,
                _fakeReadbackABSelectRegistersCommand, _fakeSendSpecialFunctionCommand);
        }

        public static IEnumerable TestData1
        {
            get
            {
                for (var i = 0; i < 40; i++)
                {
                    yield return new TestCaseData(
                        (ChannelAddress) ((int) ChannelAddress.Dac0 + i),
                        DacChannelDataSource.DataValueA,
                        i / 8
                    );
                    yield return new TestCaseData(
                        (ChannelAddress) ((int) ChannelAddress.Dac0 + i),
                        DacChannelDataSource.DataValueB,
                        i / 8
                    );
                }
            }
        }

        public static IEnumerable TestData2
        {
            get
            {
                for (var i = 0; i < 40; i++)
                {
                    yield return new TestCaseData(
                        i / 8,
                        byte.MaxValue,
                        (ChannelAddress) ((int) ChannelAddress.Dac0 + i),
                        DacChannelDataSource.DataValueA,
                        (byte) (byte.MaxValue & ~(1 << (byte) (i % 8)))
                    );
                    yield return new TestCaseData(
                        i / 8,
                        byte.MinValue,
                        (ChannelAddress) ((int) ChannelAddress.Dac0 + i),
                        DacChannelDataSource.DataValueB,
                        (byte) (byte.MinValue | (1 << (byte) (i % 8)))
                    );
                }
            }
        }

        private IDenseDacEvalBoard _fakeEvalBoard;
        private DeviceState _deviceState;
        private IReadbackABSelectRegisters _fakeReadbackABSelectRegistersCommand;
        private ISendSpecialFunction _fakeSendSpecialFunctionCommand;
        private ISetDacChannelDataSourceInternal _setDacChannelDataSourceInternalCommand;
        private SpecialFunctionCode _specialFunctionCodeSent;
        private ushort _dataSent;
    }
}