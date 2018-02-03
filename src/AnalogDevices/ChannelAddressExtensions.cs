namespace AnalogDevices
{
    internal static class ChannelAddressExtensions
    {
        internal static int ToChannelNumber(this ChannelAddress channelAddress)
        {
            return ((int)channelAddress) - 8;
        }

    }
}
