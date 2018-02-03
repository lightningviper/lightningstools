using System;
using AnalogDevices.DeviceCommands;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class ReadbackABSelectRegisterCommandTests
    {
        [Test]
        public void ShouldReturnABSelectRegisterValueFromCache(
            [Values(0, 1, 2, 3, 4)] int registerNum,
            [Range(byte.MinValue, byte.MaxValue)] byte abSelectRegisterBits)
        {
            GivenRegisterCachesAreEnabled();
            GivenABSelectRegisterValueExistsInDeviceStateRegisterCache(registerNum,
                (ABSelectRegisterBits) abSelectRegisterBits);
            WhenReadbackABSelectRegister(registerNum);
            ThenTheValueFromCacheShouldBeTheValueReturned(registerNum);
        }

        [Test]
        public void ShouldReturnABSelectRegisterValueFromDeviceWhenCacheIsEmpty(
            [Values(0, 1, 2, 3, 4)] int registerNum,
            [Range(byte.MinValue, byte.MaxValue)] byte abSelectRegisterBits)
        {
            GivenRegisterCachesAreEnabled();
            GivenABSelectRegisterValueDoesNotExistInDeviceStateRegisterCache(registerNum);
            GivenABSelectRegisterValueInDeviceMemory((ABSelectRegisterBits) abSelectRegisterBits);
            WhenReadbackABSelectRegister(registerNum);
            ThenTheValueShouldBeReadFromTheDevice();
        }

        [Test]
        public void ShouldReturnABSelectRegisterValueFromDeviceWhenCacheIsDisabled(
            [Values(0, 1, 2, 3, 4)] int registerNum,
            [Range(byte.MinValue, byte.MaxValue)] byte abSelectRegisterBits)
        {
            GivenRegisterCachesAreDisabled();
            GivenABSelectRegisterValueExistsInDeviceStateRegisterCache(registerNum,
                (ABSelectRegisterBits) abSelectRegisterBits);
            GivenABSelectRegisterValueInDeviceMemory((ABSelectRegisterBits) abSelectRegisterBits);
            WhenReadbackABSelectRegister(registerNum);
            ThenTheValueShouldBeReadFromTheDevice();
        }

        [Test]
        public void ShouldCacheABSelectRegisterValueReturnedFromDevice(
            [Values(0, 1, 2, 3, 4)] int registerNum,
            [Range(byte.MinValue, byte.MaxValue)] byte abSelectRegisterBits)
        {
            GivenRegisterCachesAreEnabled();
            GivenABSelectRegisterValueDoesNotExistInDeviceStateRegisterCache(registerNum);
            GivenABSelectRegisterValueInDeviceMemory((ABSelectRegisterBits) abSelectRegisterBits);
            WhenReadbackABSelectRegister(registerNum);
            ThenTheValueReadShouldBeCached(registerNum);
        }

        [Test]
        public void ShouldThrowExceptionIfRegisterNumberIsOutOfRange(
            [Values(-1, 5)] int registerNum)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => WhenReadbackABSelectRegister(registerNum));
        }

        private void GivenRegisterCachesAreEnabled()
        {
            _deviceState.UseRegisterCache = true;
        }

        private void GivenRegisterCachesAreDisabled()
        {
            _deviceState.UseRegisterCache = false;
        }

        private void GivenABSelectRegisterValueExistsInDeviceStateRegisterCache(int registerNum,
            ABSelectRegisterBits? value)
        {
            _deviceState.ABSelectRegisters[registerNum] = value;
        }

        private void GivenABSelectRegisterValueDoesNotExistInDeviceStateRegisterCache(int registerNum)
        {
            _deviceState.ABSelectRegisters[registerNum] = null;
        }

        private void GivenABSelectRegisterValueInDeviceMemory(ABSelectRegisterBits abSelectRegisterBits)
        {
            Mock.Get(_fakeReadSPICommand)
                .Setup(x => x.ReadSPI())
                .Returns((ushort) abSelectRegisterBits);
        }

        private void WhenReadbackABSelectRegister(int registerNumber)
        {
            _resultOfReadback = _readbackABSelectRegisterCommand.ReadbackABSelectRegister(registerNumber);
        }

        private void ThenTheValueFromCacheShouldBeTheValueReturned(int registerNum)
        {
            _deviceState.ABSelectRegisters[registerNum].Should().Be(_resultOfReadback);
        }

        private void ThenTheValueShouldBeReadFromTheDevice()
        {
            Mock.Get(_fakeSendSpecialFunctionCommand)
                .Verify(x => x.SendSpecialFunction(
                    SpecialFunctionCode.SelectRegisterForReadback,
                    It.IsIn((ushort) AddressCodesForDataReadback.ABSelect0Register,
                        (ushort) AddressCodesForDataReadback.ABSelect1Register,
                        (ushort) AddressCodesForDataReadback.ABSelect2Register,
                        (ushort) AddressCodesForDataReadback.ABSelect3Register,
                        (ushort) AddressCodesForDataReadback.ABSelect4Register)), Times.Once);

            Mock.Get(_fakeReadSPICommand)
                .Verify(x => x.ReadSPI(), Times.Once);
        }

        private void ThenTheValueReadShouldBeCached(int registerNum)
        {
            _deviceState.ABSelectRegisters[registerNum].Should().Be(_resultOfReadback);
        }

        [SetUp]
        public void SetUp()
        {
            _deviceState = new DeviceState();

            _fakeEvalBoard = Mock.Of<IDenseDacEvalBoard>();
            Mock.Get(_fakeEvalBoard)
                .SetupGet(x => x.DeviceState)
                .Returns(_deviceState);

            _fakeSendSpecialFunctionCommand = Mock.Of<ISendSpecialFunction>();
            _fakeReadSPICommand = Mock.Of<IReadSPI>();

            _readbackABSelectRegisterCommand = new ReadbackABSelectRegisterCommand(_fakeEvalBoard,
                _fakeSendSpecialFunctionCommand, _fakeReadSPICommand);
        }

        private IReadbackABSelectRegisters _readbackABSelectRegisterCommand;
        private IDenseDacEvalBoard _fakeEvalBoard;
        private DeviceState _deviceState;
        private ISendSpecialFunction _fakeSendSpecialFunctionCommand;
        private IReadSPI _fakeReadSPICommand;
        private ABSelectRegisterBits _resultOfReadback;
    }
}