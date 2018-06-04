using System.Threading;
using NDesk.DBus;

namespace Saturnin
{
    class Program
    {
        static void Main(string[] args)
        {
            Saturnin saturnin = new Saturnin(Bus.System);
            saturnin.ReadMessages();
            saturnin.WatchScheduledMessages();
            saturnin.WatchDpmbSubscribers();
        }
    }


}