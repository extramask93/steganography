using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace steg_2.lib
{
    class RemRandom
    {
        HashSet<int> usedPositions = new HashSet<int>();
        private Random rng_;
        private int max_;
        public RemRandom(int seed, int max)
        {
            rng_ = new Random(seed);
            max_ = max;
        }
        public RemRandom(string seed, int max)
        {
            var arr = Encoding.ASCII.GetBytes(seed);
            int sum = 0;
            foreach (var item in arr)
            {
                sum += item;
            }
            rng_ = new Random(sum);
            max_ = max;
        }
        public int Next()
        {
            int next;
            do
            {
                next = rng_.Next(0, max_);
            } while (usedPositions.Contains(next));
            usedPositions.Add(next);
            return next;
           
        }
  
        
    }
}
