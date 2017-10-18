using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace Lab2_ip
{
    class Waitor
    {
        private readonly int maxEatingTime = 1;
        private readonly int maxThinkingTime = 1;
        private readonly int minEatingTime = 1;
        private readonly int minThinkingTime = 1;

        private Random rnd;

        private Fork[] forks;
        private Philosopher[] philosophers;
        private ActingPhilosopher[] actingPhilosophers;

        public bool IsEatingTakingTime { private set; get; }
        public bool IsThinkingTakingTime { private set; get; }

        public Waitor(int quantity, bool eating, bool thinking)
        { 
            IsEatingTakingTime = eating;
            IsThinkingTakingTime = thinking;
            if (IsThinkingTakingTime || IsEatingTakingTime)
                rnd = new Random();

            forks = new Fork[quantity];
            philosophers = new Philosopher[quantity];

            for (int i = 0; i < quantity; i++)
            {
                forks[i] = new Fork(i);
                philosophers[i] = CreatePhilosopher(i);
            }
        }

        private Philosopher CreatePhilosopher(int index)
        {
            if (IsEatingTakingTime && IsThinkingTakingTime)
                return new Philosopher(index, GetRandEatingTime(), GetRandThinkingTime());
            else if (IsEatingTakingTime)
                return new Philosopher(index, GetRandEatingTime(), 0);
            else if (IsThinkingTakingTime)
                return new Philosopher(index, 0, GetRandThinkingTime());
            else
                return new Philosopher(index);
        }

        private int GetRandEatingTime()
        {
            return rnd.Next(minEatingTime, maxEatingTime);
        }
        private int GetRandThinkingTime()
        {
            return rnd.Next(minThinkingTime, maxThinkingTime);
        }

        public void SaveCurrentTableState()
        {
            lock(this)
            {
                StreamWriter writer = new StreamWriter("current_state.txt", true);
                
                writer.WriteLine("***Info about table***");
                foreach (Fork f in forks)
                    writer.WriteLine(f.Info);
                foreach (Philosopher ph in philosophers)
                    writer.WriteLine(ph.Info);

                writer.Close();
            }
        }

        public void Start()
        {
            actingPhilosophers = new ActingPhilosopher[philosophers.Length];

            for (int i = 0; i < actingPhilosophers.Length; i++)
                actingPhilosophers[i] = new ActingPhilosopher(philosophers[i], this);
        }
        public void Stop()
        {
            foreach (var actPh in actingPhilosophers)
                actPh.Stop();
        }

        public Fork ForkForPhilosopher(Philosopher ph, ForkType type)
        {
            if (ph.index >= philosophers.Length ||
                ph.index < 0)
                return null;
            if (ph != philosophers[ph.index])
                return null;

            int index = ph.index;
            if (type == ForkType.Left)
                index--;

            if (index == -1)
                return forks[philosophers.Length - 1];

            return forks[index];
        }
        public bool CanTakeFork(Fork fork, Philosopher ph)
        {
            int fIndex = fork.index,
                phIndex = ph.index;

            if (!(Math.Abs(fIndex - phIndex) <= 1 ||
                  fIndex == philosophers.Length - 1 && phIndex == 0))
                return false;


            if (fork.owner == null)
            {
                Mutex mtx;
                if (!Mutex.TryOpenExisting(fork.Name, out mtx))
                    mtx = new Mutex(false, fork.Name);

                return true;
            } else
                return false;
        }
    }
}
