using System.Threading;

namespace botnet
{
    class Program
    {
        

        static void Main(string[] args)
        {
            DBus.Test();
            //var saturnin = new Saturnin();
            //Thread receiver = new Thread(new ThreadStart(saturnin.DoWork));
            //receiver.Start();
        }
    }

    public class Saturnin
    {
        public void DoWork()
        {
            while (true)
            {
                //SignalCliCommands.Receive();
                Thread.Sleep(5000);
            }
        }
    }
}