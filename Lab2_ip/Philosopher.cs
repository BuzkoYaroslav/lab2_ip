using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Lab2_ip
{
    class Philosopher
    {
        private Timer starvationTimer;
        private readonly int timerInterval = 1;

        public int index { private set; get; }
        private Fork left;
        private Fork right;

        public int timeOfEating { private set; get; }
        public int timeOfThinking { private set; get; }
        public int timeOfStarvation { private set; get; }

        public int platesEaten { private set; get; }

        public Philosopher(int index): this(index, 0, 0)
        {

        }
        public Philosopher(int index, int eatingTime, int thinkingTime)
        {
            this.index = index;

            timeOfEating = eatingTime;
            timeOfThinking = thinkingTime;
            timeOfStarvation = 0;
            platesEaten = 0;
        }

        public bool Eat()
        {
            if (left == null || right == null)
                return false;

            if (starvationTimer != null)
            {
                starvationTimer.Dispose();
                starvationTimer = null;
            }

            Thread.Sleep(timeOfEating);

            platesEaten += 1;
            ReleaseForks();

            return true;
        }
        public bool Think()
        {
            if (starvationTimer != null)
                return false;

            Thread.Sleep(timeOfThinking);

            if (timeOfEating != 0)
                ConfigureTimer();

            return true;
        }
        public bool Take(Fork fork, ForkType type)
        {
            switch (type)
            {
                case ForkType.Left:
                    if (left != null)
                        return false;
                    fork.Take(this);
                    left = fork;
                    break;
                case ForkType.Right:
                    if (right != null)
                        return false;
                    fork.Take(this);
                    right = fork;
                    break;
            }

            return true;
        }

        public string Name
        {
            get
            {
                return "Philosopher" + index;
            }
        }

        private void ReleaseForks()
        {
            ReleaseFork(ForkType.Left);
            ReleaseFork(ForkType.Right);
        }

        public void ReleaseFork(ForkType type)
        {
            switch(type)
            {
                case ForkType.Left:
                    if (left != null)
                    {
                        left.Release();
                        left = null;
                    }
                    break;
                case ForkType.Right:
                    if (right != null)
                    {
                        right.Release();
                        right = null;
                    }
                    break;
            }
        }


        private void ConfigureTimer()
        {
            TimerCallback tmCB = new TimerCallback(Starvate);
            starvationTimer = new Timer(tmCB, null, 0, timerInterval);
        }
        private void Starvate(object obj)
        {
            timeOfStarvation += timerInterval;
        }
        public void StartStarvation()
        {
            ConfigureTimer();
        }


        public string Info
        {
            get
            {
                lock (this)
                {
                    string result = "**Info about " + Name + "**\n";

                    if (left == null)
                        result += "Has no left fork.\n";
                    else
                        result += "Has " + left.Name + " as left fork.\n";

                    if (right == null)
                        result += "Has no right fork.\rn";
                    else
                        result += "Has " + right.Name + " as right fork.\n";

                    result += "Plates eaten = " + platesEaten + "\n";
                    result += "Eating time = " + timeOfEating + "\n";
                    result += "Thinking time = " + timeOfThinking + "\n";
                    result += "Starvation time = " + timeOfStarvation + "\n";

                    return result;
                }
            }
        }
    }
}
