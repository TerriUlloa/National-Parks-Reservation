using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace Capstone.Exceptions
{
    /// <summary>
    /// Specifies that a park was not found
    /// </summary>
    public class ParkNotFoundException : Exception
    {
        /// <summary>
        /// The constructor needed to create custom exception
        /// </summary>
        /// <param name="message">Custom error message for the exception</param>
        public ParkNotFoundException(string message = "") : base(message)
        {

        }
    }
}
