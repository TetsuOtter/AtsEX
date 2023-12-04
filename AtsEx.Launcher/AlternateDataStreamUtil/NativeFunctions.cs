using System;
using System.Runtime.InteropServices;

namespace AtsEx.Launcher.AlternateDataStreamUtil
{
    using DWORD = UInt64;

    internal enum StreamInfoLevels
    {
        FindStreamInfoStandard,
        FindStreamInfoMaxInfoLevel
    }

    // ref: https://stackoverflow.com/questions/683491/how-to-declarate-large-integer-in-c-sharp
    [StructLayout(LayoutKind.Explicit, Size = 8)]
    internal struct LARGE_INTEGER
    {
        [FieldOffset(0)]
        public Int64 QuadPart;
        [FieldOffset(0)]
        public UInt32 LowPart;
        [FieldOffset(4)]
        public Int32 HighPart;
    }

    // ref: https://learn.microsoft.com/ja-jp/windows/win32/api/fileapi/ns-fileapi-win32_find_stream_data
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct WIN32_FIND_STREAM_DATA
    {
        public LARGE_INTEGER StreamSize;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 296)]
        public string cStreamName;
    }

    internal static class NativeFunctions
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr FindFirstStreamW(string lpFileName, StreamInfoLevels InfoLevel, out WIN32_FIND_STREAM_DATA lpFindStreamData, DWORD dwFlags);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool FindNextStreamW(IntPtr hFindStream, out WIN32_FIND_STREAM_DATA lpFindStreamData);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool FindClose(IntPtr hFindFile);
    }
}
