using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Exceptions
{
    public class InvalidSiteException : Exception
    {
        /// <summary>
        /// The constructor needed to create custom exception
        /// </summary>
        /// <param name="message">Custom error message for the exception</param>
        public InvalidSiteException(string message = "") : base(message)
        {

        }
    }
}
