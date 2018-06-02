using System.Threading;
using NDesk.DBus;

namespace Saturnin
{
    class Program
    {
        static void Main(string[] args)
        {
            //new Saturnin().SendMessage("test", "+420608828650", null);
            new Saturnin(Bus.System).ReadMessages();

            //Thread.Sleep(60 * 1000);
        }
    }


}