// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32.SafeHandles
{
    /// <summary>
    ///     Holds a managed string marshaled as a LPCWSTR.
    /// </summary>
    internal sealed class SafeUnicodeStringHandle : SafeHandle
    {
        /// <summary>
        ///     Marshal a String to a native LPCWSTR. It is permitted to pass "null" for the string.
        /// </summary>
        public SafeUnicodeStringHandle(string s)
            : base(IntPtr.Zero, ownsHandle: true)
        {
            handle = NativeMemoryHelper.AllocStringUnicode(s);
        }

        public unsafe SafeUnicodeStringHandle(ReadOnlySpan<char> s)
            : base(IntPtr.Zero, ownsHandle: true)
        {
            // If s == default then the span represents the null string,
            // and handle should be IntPtr.Zero to match Marshal.StringToHGlobalUni.
            //
            // Since that was already done in the base ctor call, we only need to do
            // work when s != default.
            if (s != default)
            {
                int cch = checked(s.Length + 1);
                int cb = checked(cch * sizeof(char));
                handle = NativeMemoryHelper.Alloc(cb);

                Span<char> dest = new Span<char>(handle.ToPointer(), cch);
                s.CopyTo(dest);
                dest[s.Length] = (char)0;
            }
        }

        public sealed override bool IsInvalid
        {
            get
            {
                return (nint)handle == 0;
            }
        }

        protected sealed override bool ReleaseHandle()
        {
            NativeMemoryHelper.Free(handle);
            return true;
        }
    }
}
