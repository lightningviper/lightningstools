﻿using System;
using AnalogDevices.DeviceCommands;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace AnalogDevices.UnitTests.DeviceCommands
{
    [TestFixture]
    public class GetDacChannelDataValueACommandTests
    {
        [Test]
        public void ShouldGetCorrectDacChannelDataValueA(
            [Values(DacPrecision.SixteenBit, DacPrecision.FourteenBit)] DacPrecision dacPrecision,
            [Range((int) ChannelAddress.Dac0, (int) ChannelAddress.Dac39)] int channelAddress,
            [Values(0, 1, 1 << 1, 1 << 2, 1 << 3, 1 << 4, 1 << 5, 1 << 6, 1 << 7, 1 << 8, 1 << 9, 1 << 10, 1 << 11,
                1 << 12, 1 << 13, 1 << 14, 1 << 15, (2 ^ 16) - 1)] int x1aRegisterValue)
        {
            GivenDacPrecision(dacPrecision);
            GivenX1ARegisterBits((ChannelAddress) channelAddress, (ushort) x1aRegisterValue);
            WhenGetDacChannelDataValueA((ChannelAddress) channelAddress);
            ThenValueReturnedShouldBe((ushort) (x1aRegisterValue &
                                                (dacPrecision == DacPrecision.SixteenBit
                                                    ? (ushort) BasicMasks.SixteenBits
                                                    : (ushort) BasicMasks.HighFourteenBits)));
        }

        [Test]
        public void ShouldThrowExceptionIfChannelNumberIsOutOfRange(
            [Values((int) (ChannelAddress.Dac0 - 1), (int) (ChannelAddress.Dac39 + 1))] ChannelAddress channelAddress)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => WhenGetDacChannelDataValueA(channelAddress));
        }

        private void GivenDacPrecision(DacPrecision dacPrecision)
        {
            _deviceState.Precision = dacPrecision;
        }

        private void GivenX1ARegisterBits(ChannelAddress channelAddress, ushort x1aRegisterValue)
        {
            Mock.Get(_fakeRb)
                .Setup(x => x.ReadbackX1ARegister(channelAddress))
                .Returns((ushort) (x1aRegisterValue & (_deviceState.Precision == DacPrecision.SixteenBit
                                       ? (ushort) BasicMasks.SixteenBits
                                       : (ushort) BasicMasks.HighFourteenBits)));
        }

        private void WhenGetDacChannelDataValueA(ChannelAddress channelAddress)
        {
            _dacChannelDataValueA = _getDacChannelDataValueACommand.GetDacChannelDataValueA(channelAddress);
        }

        private void ThenValueReturnedShouldBe(ushort expectedValue)
        {
            _dacChannelDataValueA.Should().Be(expectedValue);
        }

        [SetUp]
        public void SetUp()
        {
            _fakeEvalBoard = Mock.Of<IDenseDacEvalBoard>();
            _deviceState = new DeviceState();
            Mock.Get(_fakeEvalBoard)
                .SetupGet(x => x.DeviceState)
                .Returns(_deviceState);

            _fakeRb = Mock.Of<IReadbackX1ARegister>();

            _getDacChannelDataValueACommand = new GetDacChannelDataValueACommand(_fakeEvalBoard, _fakeRb);
        }

        private IGetDacChannelDataValueA _getDacChannelDataValueACommand;
        private IReadbackX1ARegister _fakeRb;
        private IDenseDacEvalBoard _fakeEvalBoard;
        private DeviceState _deviceState;
        private ushort _dacChannelDataValueA;
    }
}