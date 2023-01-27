using System;

namespace Examples.AspNetMvcCode.CodeUtility.JsLibrariesUtility
{
    /// <summary>
    /// Format exception
    /// </summary>
    public class UnsupportedFormatException : Exception
    {
        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="message"></param>
        public UnsupportedFormatException(string message) : base(message)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        public UnsupportedFormatException()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public UnsupportedFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}