using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace PPJoy
{
    /// <summary>
    ///   Represents an error that occurs when trying to obtain a reference to a PPJoy <see cref = "Device" /> that does not exist.
    /// </summary>
    [Serializable]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class DeviceNotFoundException : PPJoyException
    {
        #region Constructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "DeviceNotFoundException" /> class.
        /// </summary>
        public DeviceNotFoundException()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "DeviceNotFoundException" /> class.
        /// </summary>
        /// <param name = "message">The message that describes the error.</param>
        public DeviceNotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "DeviceNotFoundException" /> class.
        /// </summary>
        /// <param name = "message">The error message that explains the reason for the exception.</param>
        /// <param name = "inner">The exception that is the cause of the current exception, or <see langword = "null" /> if no inner exception is specified.</param>
        public DeviceNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "DeviceNotFoundException" /> class.
        /// </summary>
        /// <param name = "info">The <see cref = "System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the <see cref = "System.Exception" /> being thrown.</param>
        /// <param name = "context">The <see cref = "System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected DeviceNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}