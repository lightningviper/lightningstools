#region Using statements

using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

#endregion

namespace PPJoy
{
    /// <summary>
    ///   <see cref = "PPJoyException" /> is the base class <see cref = "System.Exception">Exception</see> for all custom <see cref = "System.Exception">Exception</see>s that occur within the PPJoy wrapper.
    /// </summary>
    [Serializable]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class PPJoyException : ApplicationException
    {
        #region Constructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "PPJoyException" /> class.
        /// </summary>
        public PPJoyException()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "PPJoyException" /> class.
        /// </summary>
        /// <param name = "message">The message that describes the error.</param>
        public PPJoyException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "PPJoyException" /> class.
        /// </summary>
        /// <param name = "message">The error message that explains the reason for the <see cref = "System.Exception">Exception</see>.</param>
        /// <param name = "inner">The <see cref = "System.Exception" /> that is the cause of the current <see cref = "System.Exception">Exception</see>, or <see langword = "null" /> if no inner <see cref = "System.Exception">Exception</see> is specified.</param>
        public PPJoyException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "PPJoyException" /> class.
        /// </summary>
        /// <param name = "info">The <see cref = "System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the <see cref = "System.Exception">Exception</see> being thrown.</param>
        /// <param name = "context">The <see cref = "System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected PPJoyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}