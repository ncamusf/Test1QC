using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_1___Synchronization_Algorithm
{
    /// <summary>
    /// Point in time.
    /// </summary>
    class Point
    {
        public DateTime Time;
        public decimal Value;

        public Point(DateTime time, decimal value)
        {
            Time = time;
            Value = value;
        }
    }
}
