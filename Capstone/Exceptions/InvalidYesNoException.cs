using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Exceptions
{
    public class InvalidYesNoException :Exception
    {
        /// <summary>
        /// The constructor needed to create custom exception
        /// </summary>
        /// <param name="message">Custom error message for the exception</param>
        public InvalidYesNoException(string message = "") : base(message)
        {

        }
    }
}
