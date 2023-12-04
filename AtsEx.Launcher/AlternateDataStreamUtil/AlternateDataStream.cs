using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace AtsEx.Launcher.AlternateDataStreamUtil
{
    internal class AlternateDataStream
    {
        public IReadOnlyList<WIN32_FIND_STREAM_DATA> Streams { get; }

        public AlternateDataStream(FileInfo fileInfo)
        {
            List<WIN32_FIND_STREAM_DATA> streams = new List<WIN32_FIND_STREAM_DATA>();

            IntPtr ptr = IntPtr.Zero;
            try
            {
                ptr = NativeFunctions.FindFirstStreamW(fileInfo.FullName, StreamInfoLevels.FindStreamInfoStandard, out WIN32_FIND_STREAM_DATA streamData, 0);

                if (ptr == IntPtr.Zero)
                {
                    int err = Marshal.GetLastWin32Error();
                    if (err != 0)
                        throw new ExternalException($"Failed to call {nameof(NativeFunctions.FindFirstStreamW)}", err);
                }
                else
                {
                    streams.Add(streamData);
                    while (NativeFunctions.FindNextStreamW(ptr, out streamData))
                        streams.Add(streamData);
                }
            }
            finally
            {
                if (ptr != IntPtr.Zero && !NativeFunctions.FindClose(ptr))
                {
                    int err = Marshal.GetLastWin32Error();
                    if (err != 0)
                        throw new ExternalException("", err);
                }
            }

            Streams = streams;
        }
    }
}
