using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoScout24
{
    class RandomGenerator
    {
        private Random Random;
        public RandomGenerator()
        {
            Random = new Random((int)DateTime.Now.Ticks);
        }

        public int Next(int min, int max, int step = 1)
        {
            if (step == 1) return Random.Next(min, max);
            int minK = min / step;
            int maxK = max / step;
            return Random.Next(minK, maxK) * step;
        }

        public decimal Next(int min, int max, int step, int divide)
        {
            int random = this.Next(min, max, step);
            if (divide == 1) return random;
            return ((decimal)random) / divide;
        }

    }
}
