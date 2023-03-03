using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_1___Synchronization_Algorithm
{
    /// <summary>
    /// Collection of randomly generated datapoints.
    /// </summary>
    class DataSource
    {
        /// <summary>
        /// Name of this virtual data
        /// </summary>
        public string Name;
        
        /// <summary>
        /// List of generated points for this virtual data source.
        /// </summary>
        public Queue<Point> Generated;

        /// <summary>
        /// Create the data source.
        /// </summary>
        public DataSource(int name, TimeSpan period, TimeSpan seedDensity)
        {
            Name = Guid.NewGuid().ToString();
            Generated = new Queue<Point>();

            //Generate asset starting price:
            var seedAssetPrice = Convert.ToDecimal(100*(new Random().NextDouble()));

            //Generate the data:
            var now = DateTime.MinValue;
            var start = new DateTime(2000, 1, 1);
            var randomGenerator = new Random(name);

            while (now < start + period)
            {
                //Seed first jump if this is the first point:
                if (now == DateTime.MinValue)
                {
                    var randomStartOffset = period.Ticks / randomGenerator.Next(5,15);
                    start += new TimeSpan(randomStartOffset);
                    now = start;
                }

                //Increment now by variable time period:
                now += new TimeSpan((int)(seedDensity.Ticks* randomGenerator.NextDouble()));

                //Generate Random Value Walk:
                var walkStep = seedAssetPrice * 0.01m;
                var plusMinus = randomGenerator.NextDouble() < 0.5 ? -1m : 1m;
                var step = walkStep*plusMinus*Convert.ToDecimal(randomGenerator.NextDouble());
                seedAssetPrice += step;

                //Model jumps in weekends:
                if (now.DayOfWeek == DayOfWeek.Friday && now.Hour > 15)
                {
                    //Jump from Friday 4pm to Monday 9.30am -- NYSE market hours.
                    now += TimeSpan.FromHours(8 + 24 + 24 + 9.5);
                }

                //Early exit terms
                if (seedAssetPrice <= 0.001m)
                {
                    break;
                }

                //Stock split: 10-1 split.
                if (seedAssetPrice > 5000)
                {
                    seedAssetPrice /= 10;
                }

                Generated.Enqueue(new Point( now, seedAssetPrice));
            }

            Console.WriteLine("DataSource {0} started {1} ended on {2} with {3} points.", name, start.ToString("u"), now.ToString("u"), Generated.Count);
        }
    }
}
