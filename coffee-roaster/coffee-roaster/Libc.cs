using System;
using System.Runtime.InteropServices;

namespace coffee_roaster
{
    public class Libc
    {
        [Flags]
        public enum FileAccessMode
        {
            O_RDONLY = 0,
            O_WRONLY = 1,
            O_RDWR = 2,
            O_DSYNC = 1000,
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