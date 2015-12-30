using System;

namespace coffee_roaster
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int mem_fd = Libc.Open("/dev/mem", Libc.FileAccessMode.O_RDWR | Libc.FileAccessMode.O_SYNC);

            if(mem_fd < 0)
            {
                Console.WriteLine("can't open /dev/mem");
                Environment.Exit(-1);
            }
        }
    }
}