using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Lab2_ip
{
    class Program
    {
        private const int philosophersCount = 10;

        static void Main(string[] args)
        {
            Waitor wtr = new Waitor(philosophersCount, true, true);

            wtr.Start();

            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(1500);
                wtr.SaveCurrentTableState();
            }
            
            wtr.Stop();

            Thread.Sleep(1000);
            wtr.SaveCurrentTableState();
        }
    }
}
