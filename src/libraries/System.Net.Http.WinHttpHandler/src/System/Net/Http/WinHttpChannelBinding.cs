// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using SafeWinHttpHandle = Interop.WinHttp.SafeWinHttpHandle;

#pragma warning disable CA1419 // TODO https://github.com/dotnet/roslyn-analyzers/issues/5232: not intended for use with P/Invoke

namespace System.Net.Http
{
    internal sealed class WinHttpChannelBinding : ChannelBinding
    {
        private int _size;
        private string? _cachedToString;

        internal unsafe WinHttpChannelBinding(SafeWinHttpHandle requestHandle)
        {
            void* data = null;
            uint dataSize = 0;

            if (!Interop.WinHttp.WinHttpQueryOption(requestHandle, Interop.WinHttp.WINHTTP_OPTION_SERVER_CBT, null, ref dataSize))
            {
                if (Marshal.GetLastWin32Error() == Interop.WinHttp.ERROR_INSUFFICIENT_BUFFER)
                {
                    data = NativeMemory.Alloc(dataSize);

                    if (Interop.WinHttp.WinHttpQueryOption(requestHandle, Interop.WinHttp.WINHTTP_OPTION_SERVER_CBT, data, ref dataSize))
                    {
                        SetHandle((nint)data);
                        _size = (int)dataSize;
                    }
                    else
                    {
                        NativeMemory.Free(data);
                    }
                }
            }
        }

        public override bool IsInvalid
        {
            get
            {
                return _size == 0;
            }
        }

        public override int Size
        {
            get
            {
                return _size;
            }
        }

        public override string? ToString()
        {
            if (_cachedToString == null && !IsInvalid)
            {
                var bytes = new byte[_size];
                Marshal.Copy(this.handle, bytes, 0, _size);
                _cachedToString = BitConverter.ToString(bytes).Replace('-', ' ');
            }

            return _cachedToString;
        }

        protected override bool ReleaseHandle()
        {
            Marshal.FreeHGlobal(this.handle);
            SetHandle(IntPtr.Zero);
            _size = 0;
            return true;
        }
    }
}
