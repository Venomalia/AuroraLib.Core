using AuroraLib.Core.Collections;
using AuroraLib.Core.Format.Identifier;
using AuroraLib.Core.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

namespace AuroraLib.Core.Format
{

    /// <summary>
    /// Represents a collection of format information, allowing lookups by MIME type,
    /// and enabling format detection based on stream content and file extensions.
    /// </summary>
    [DebuggerDisplay("{Count}")]
    public sealed class FormatDictionary : ILinkedDictionary<string, IFormatInfo>
    {
        private readonly Dictionary<string, IFormatInfo> Formats;

        // Accelerates the recognition of formats.
        private readonly Dictionary<IIdentifier, IFormatInfo> IdentifierLUT;
        private readonly List<IFormatInfo> Remaining;

        /// <inheritdoc/>
        public int Count => Formats.Count;

        /// <inheritdoc/>
        public IEnumerable<string> Keys => Formats.Keys;

        /// <inheritdoc/>
        public IEnumerable<IFormatInfo> Values => Formats.Values;

        /// <inheritdoc/>
        public IFormatInfo this[string key] => Formats[key];

        bool ICollection<IFormatInfo>.IsReadOnly => false;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatDictionary"/> class.
        /// </summary>
        public FormatDictionary()
        {
            Formats = new Dictionary<string, IFormatInfo>();
            IdentifierLUT = new Dictionary<IIdentifier, IFormatInfo>();
            Remaining = new List<IFormatInfo>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatDictionary"/> class by copying data from another instance.
        /// </summary>
        /// <param name="dictionary">The <see cref="FormatDictionary"/> instance to copy data from.</param>
        public FormatDictionary(FormatDictionary dictionary)
        {
            Formats = new Dictionary<string, IFormatInfo>(dictionary.Formats);
            IdentifierLUT = new Dictionary<IIdentifier, IFormatInfo>(dictionary.IdentifierLUT);
            Remaining = new List<IFormatInfo>(dictionary.Remaining);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatDictionary"/> class using a collection of <see cref="IFormatInfo"/> formats.
        /// </summary>
        /// <param name="formats">A collection of <see cref="IFormatInfo"/> formats to add to the dictionary.</param>
        public FormatDictionary(IEnumerable<IFormatInfo> formats) : this()
        {
            foreach (IFormatInfo format in formats)
            {
                Add(format);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatDictionary"/> class using available formats from the specified <paramref name="assemblies"/>.
        /// </summary>
        /// <param name="assemblies">Assemblies to search for format information providers.</param>
        public FormatDictionary(params Assembly[] assemblies) : this(GetAvailableFormats(assemblies))
        { }

        /// <summary>
        /// Scans the provided <paramref name="assemblies"/> for types implementing <see cref="IFormatInfoProvider"/> and returns their associated <see cref="IFormatInfo"/>.
        /// </summary>
        /// <param name="assemblies">Assemblies to scan for format providers.</param>
        /// <returns>An enumerable collection of <see cref="IFormatInfo"/> objects found in the provided assemblies.</returns>
        public static IEnumerable<IFormatInfo> GetAvailableFormats(params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                // Iterate through all types defined in the current assembly
                foreach (var type in assembly.DefinedTypes)
                {
                    // Check if the type implements IFormatInfoProvider, is not an interface or abstract, and has a parameterless constructor
                    if (typeof(IFormatInfoProvider).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract && type.GetConstructor(Type.EmptyTypes) != null)
                    {
                        // Create an instance of the type and return the associated format information
                        yield return ((IFormatInfoProvider)Activator.CreateInstance(type.AsType())!).Info;
                    }
                }
            }
        }

        /// <summary>
        /// Identifies the format of the provided stream based on its content and or file extension.
        /// </summary>
        /// <param name="stream">The data stream to analyze for format identification.</param>
        /// <param name="fileNameAndExtension">The file name and extension (optional).</param>
        /// <param name="format"> When this method returns, contains the identified format if successful, otherwise <c>null</c>.</param>
        /// <returns><c>true</c> if a matching format was found; otherwise, <c>false</c>.</returns>
#if NET6_0_OR_GREATER
        public bool Identify(Stream stream, ReadOnlySpan<char> fileNameAndExtension, [MaybeNullWhen(false)] out IFormatInfo format)
#else
        public bool Identify(Stream stream, ReadOnlySpan<char> fileNameAndExtension, out IFormatInfo? format)
#endif
        {
            ThrowIf.Null(stream);

            if (stream.Length >= 0x10)
            {
                stream.Seek(0, SeekOrigin.Begin);
                Identifier64 identifier = stream.Peek<Identifier64>();

                if (IdentifierLUT.TryGetValue(identifier, out format) && Match(format, stream, fileNameAndExtension))
                    return true;

                if (IdentifierLUT.TryGetValue(identifier.Lower, out format) && Match(format, stream, fileNameAndExtension))
                    return true;
            }

            foreach (IFormatInfo item in Remaining)
            {
                if (Match(item, stream, fileNameAndExtension))
                {
                    format = item;
                    return true;
                }
            }
            format = null;
            return false;
        }

        /// <inheritdoc cref="Identify(Stream, ReadOnlySpan{char}, out IFormatInfo)"/>
#if NET6_0_OR_GREATER
        public bool Identify(Stream stream, [MaybeNullWhen(false)] out IFormatInfo format)
#else
        public bool Identify(Stream stream, out IFormatInfo? format)
#endif
            => Identify(stream, default, out format);

        private static bool Match(IFormatInfo format, Stream stream, ReadOnlySpan<char> fileNameAndExtension)
        {
            try
            {
                if (format.IsMatch(stream, fileNameAndExtension))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Format match failed for format {format.MIMEType}: {ex}");
            }
            finally
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
            return false;
        }

        /// <inheritdoc/>
        public bool TryAdd(IFormatInfo item)
        {
            ThrowIf.Null(item);
            string mime = item.MIMEType.ToString();

            if (!Formats.TryAdd(mime, item))
                return false;

            if (item.IdentifierOffset != 0 || !(item.Identifier is Identifier32 || item.Identifier is Identifier64) || !IdentifierLUT.TryAdd(item.Identifier, item))
            {
                Remaining.Add(item);
            }
            return true;
        }

        /// <inheritdoc/>
        public void Add(IFormatInfo item)
        {
            if (!TryAdd(item))
                throw new InvalidOperationException($"Item with MIME type '{item.MIMEType}' already exists.");
        }

        /// <inheritdoc/>
        public void Clear()
        {
            IdentifierLUT.Clear();
            Formats.Clear();
            Remaining.Clear();
        }

        /// <inheritdoc/>
        public bool Contains(IFormatInfo item) => ContainsKey(item.MIMEType.ToString());

        /// <inheritdoc/>
        public bool ContainsKey(string mimeTyp) => Formats.ContainsKey(mimeTyp);

        /// <inheritdoc/>
#if NET6_0_OR_GREATER
        public bool TryGetValue(string mimeTyp, [MaybeNullWhen(false)] out IFormatInfo value)
#else
        public bool TryGetValue(string mimeTyp, out IFormatInfo value)
#endif
            => Formats.TryGetValue(mimeTyp, out value);

        /// <inheritdoc/>
        public bool Remove(IFormatInfo item) => Formats.Remove(item.MIMEType.ToString(), out _);

        /// <inheritdoc/>
        public bool Remove(string mime) => Remove(mime, out _);

        /// <inheritdoc/>
#if NET6_0_OR_GREATER
        public bool Remove(string mime, [MaybeNullWhen(false)] out IFormatInfo item)
#else
        public bool Remove(string mime, out IFormatInfo item)
#endif
        {
            if (Formats.Remove(mime, out item))
            {
                if (item.IdentifierOffset == 0 && item.Identifier != null && IdentifierLUT.TryGetValue(item.Identifier, out IFormatInfo? info))
                {
                    if (item.Equals(info))
                    {
                        IdentifierLUT.Remove(item.Identifier);
                    }
                }
                Remaining.Remove(item);
                return true;
            }
            return false;
        }

        /// <inheritdoc/>
        public IEnumerator<IFormatInfo> GetEnumerator() => Formats.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerator<KeyValuePair<string, IFormatInfo>> IEnumerable<KeyValuePair<string, IFormatInfo>>.GetEnumerator() => Formats.GetEnumerator();

        void ICollection<IFormatInfo>.CopyTo(IFormatInfo[] array, int arrayIndex)
        {
            ThrowIf.Null(array);
            ThrowIf.Negative(arrayIndex);

            int i = arrayIndex;
            foreach (IFormatInfo node in this)
            {
                array[i++] = node;
            }
        }
    }
}
