using Raspberry.IO.GeneralPurpose;
using System.Threading;

namespace coffee_roaster
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var led1 = ConnectorPin.P1Pin07.Output();

            var connection = new GpioConnection(led1);

            for (int i = 0; i < 100; i++)
            {
                connection.Toggle(led1);
                Thread.Sleep(250);
            }

            connection.Close();
        }
    }
}