using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Lab2_ip
{
    enum ForkType { Left = 1, Right };

    class Fork
    {
        public int index { private set; get; }
        public Philosopher owner { private set; get; }

        public string Name
        {
            get
            {
                return "Fork" + index;
            }
        }

        public Fork(int index)
        {
            this.index = index;
            owner = null;
        }

        public void Take(Philosopher desirousPh)
        {
            owner = desirousPh;
        }
        public void Release()
        {
            owner = null;
        }

        public string Info
        {
            get
            {
                if (owner == null)
                    return Name + " is not taken.\n";

                return Name + " is taken by " + owner.Name + "\rn";
            }

        }
    }
}
