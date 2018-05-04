using System;
using System.Collections.Generic;
using System.Text;

namespace UnitOfWorks.Abstractions
{
    public interface IUnitOfWorkResult
    {
        /// <summary>
        /// Contains an error, if an error has occured in trying to
        /// save the unit of work.
        /// </summary>
        UnitOfWorkException Error { get; }

        /// <summary>
        /// Specifies if the unit of work save, where 
        /// successfull.
        /// </summary>
        /// <returns>True if successfully save, and false if not.</returns>
        bool IsSuccessfull();
    }
}
