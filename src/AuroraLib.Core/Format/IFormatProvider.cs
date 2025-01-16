using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraLib.Core.Format
{
    /// <summary>
    /// Defines a provider that supplies file format information.
    /// </summary>
    public interface IFormatProvider
    {
        /// <summary>
        /// Gets the file format information associated with the provider.
        /// </summary>
        IFormatInfo Info { get; }
    }
}
