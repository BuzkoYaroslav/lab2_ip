using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Lab2_ip
{
    class ActingPhilosopher
    {
        private const int rightForkWaitingTime = 10;

        private Thread phThread;
        private Philosopher ph;
        private bool isRunning;
        private Waitor waitor;

        public ActingPhilosopher(Philosopher ph, Waitor waitor)
        {
            this.ph = ph;
            this.waitor = waitor;
            phThread = new Thread(new ThreadStart(Run));
            phThread.Name = ph.Name;
            phThread.IsBackground = true;
            isRunning = true;
            if (waitor.IsEatingTakingTime)
                ph.StartStarvation();

            phThread.Start();
        }

        public void Stop()
        {
            isRunning = false;
        }

        private void Run()
        {
            while(isRunning)
            {
                Fork left = waitor.ForkForPhilosopher(ph, ForkType.Left),
                     right = waitor.ForkForPhilosopher(ph, ForkType.Right);
                Mutex leftF, rightF;
                if (!Mutex.TryOpenExisting(left.Name, out leftF))
                    leftF = new Mutex(false, left.Name);
                if (!Mutex.TryOpenExisting(right.Name, out rightF))
                    rightF = new Mutex(false, right.Name);

                leftF.WaitOne();

                if (waitor.CanTakeFork(left, ph))
                    ph.Take(left, ForkType.Left);
                else
                {
                    leftF.ReleaseMutex();
                    continue;
                }

                if (rightF.WaitOne(rightForkWaitingTime))
                {
                    if (waitor.CanTakeFork(right, ph))
                    {
                        ph.Take(right, ForkType.Right);

                        ph.Eat();
                        ph.Think();
                    }

                    rightF.ReleaseMutex();
                    leftF.ReleaseMutex();
                } else
                {
                    ph.ReleaseFork(ForkType.Left);

                    leftF.ReleaseMutex();
                }
            }
        }
    }
}
