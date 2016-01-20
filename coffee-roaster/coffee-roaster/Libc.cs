using System;
using System.Runtime.InteropServices;

namespace coffee_roaster
{
    public class Libc
    {
        [Flags]
        public enum FileAccessMode
        {
            /// <summary>
            /// The open for read only
            /// </summary>
            O_RDONLY = 0,
            /// <summary>
            /// The open for write only
            /// </summary>
            O_WRONLY = 1,
            /// <summary>
            /// The Open for read/write
            /// </summary>
            O_RDWR = 2,
            /// <summary>
            /// Write I/O operations on the file descriptor shall complete as defined by synchronized I/O data integrity completion
            /// </summary>
            O_DSYNC = 1000,
            /// <summary>
            /// Write I/O operations on the file descriptor shall complete as defined by synchronized I/O file integrity completion
            /// </summary>
            O_SYNC = 4000000 | O_DSYNC
        }

        [DllImport("libc", EntryPoint = "open")]
        private static extern int open(string filename, int oflag);

        public static int Open(string filename, FileAccessMode oflag)
        {
            return open(filename, (int)oflag);
        }

        [DllImport("libc", EntryPoint = "close")]
        private static extern int close(int fildes);

        public static int Close(int fildes)
        {
            return close(fildes);
        }
    }
}