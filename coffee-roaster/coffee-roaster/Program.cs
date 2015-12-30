using System;

namespace coffee_roaster
{
    public class Program
    {
        public static int memFileDescriptor; // mem_fed

        public static void Main(string[] args)
        {
            setup_io();
        }

        public static void setup_io()
        {
            memFileDescriptor = Libc.Open("/dev/mem", Libc.FileAccessMode.O_RDWR | Libc.FileAccessMode.O_SYNC);

            if (memFileDescriptor < 0)
            {
                Console.WriteLine("can't open /dev/mem");
                Environment.Exit(-1);
            }

            Libc.Close(memFileDescriptor);
        }
    }
}