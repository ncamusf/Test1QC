using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Test_1___Synchronization_Algorithm
{
    class Program
    {
        public static bool Exited;
        public static long StartingMb;
        public static long PointsCount;
        public static DateTime TestStart;
        public static int ProcessedPoints;

        static void Main(string[] args)
        {
            Console.WriteLine("Test1.Main(): Generating data... ");
            //Parameters:
            var streamCount = 10;
            var period = TimeSpan.FromDays(365);
            var seedDensity = TimeSpan.FromSeconds(60);

            //Create a few data sources:
            var streams = new List<DataSource>();
            Console.WriteLine("Test1.Main(): Created streams...");
            for (var i = 0; i < streamCount; i++)
            {
                streams.Add(new DataSource(i, period, seedDensity));
            }

            Console.WriteLine("Test1.Main(): Data ready=========");
            Console.WriteLine("Test1.Main(): Starting Memory Used: " + StartingMb + "Mb");
            Console.WriteLine("Test1.Main(): Synchronizing streams...");

            StartingMb = (GC.GetTotalMemory(false)/1024/1024);
            PointsCount = streams.Select(x => x.Generated.Count).Sum();
            TestStart = DateTime.Now;
            
            var memoryLogger = new Thread(Logging); memoryLogger.Start();
            var timer = Stopwatch.StartNew();

            // *******************************************************************************
            // Add streams[i].Generated to output synchonized.
            /*
             * Synchronize 40 data streams using an ONLINE algorithm. No batch processing.
             * 
             * See example below using **batch processing**. Works for 3-4 streams, but runs out of memory for 100+ streams.
             * 
             * LEAN relies heavily on well design algorithmic engines and must be able to process 1000 stocks in parallel at once. 
             * This is possible via well designed online algorithms. Test mimics this scenario.
             */

            // This is an example of using batch processing (doing everything at once) and requires too much memory for practical use.
            // We require results as we go.





            // ==============================================================================
            // >>>>> YOUR CODE GOES INTO THE SYNCHRONIZER CLASS.
            var synchronizer = new Synchronizer(streams);
            // ==============================================================================





            // Do not edit:
            // TEST 1: Ordered data:
            var headTime = DateTime.MinValue;
            foreach (var point in synchronizer.GetNextPoint())
            {
                if (point.Time < headTime)
                {
                    throw new Exception("Test1.Main(): TEST FAILED - NOT SYNCHRONIZED");
                }
                headTime = point.Time;
                ProcessedPoints++;
            }

            // TEST 2: Total data is the same:
            if (ProcessedPoints != PointsCount)
            {
                throw new Exception("Test1.Main(): TEST FAILED: MISSING DATA");
            }
            
            timer.Stop();
            Console.WriteLine("Test1.Main(): Synchronization completed in " + timer.Elapsed + " for " + PointsCount + " points: " 
                + Math.Round((double)PointsCount / timer.ElapsedMilliseconds,0) + "k/sec");
            
            Console.WriteLine("Test1.Main(): Test Pass.");
            Exited = true;
            Console.ReadKey();
        }


        /// <summary>
        /// Output memory usage over the course of the sort.
        /// </summary>
        public static void Logging()
        {
            var lastProcessedPoints = 0;
            while (!Exited)
            {
                Thread.Sleep(500);
                var lapsed = (DateTime.Now - TestStart);
                var currentUsage = (GC.GetTotalMemory(false)/1024/1024);
                var processedDelta = Math.Round( (ProcessedPoints - lastProcessedPoints) * 2.0 / 1000 );
                lastProcessedPoints = ProcessedPoints;

                Console.WriteLine("Test1.Main(): Time Elapsed {0}, Memory Used: {1}Mb, Aprox KPoints/Sec: {2}", lapsed, currentUsage, processedDelta);

                if (currentUsage > StartingMb*1.1)
                {
                    throw new Exception("Test1.Main(): TEST FAILED: EXCESS MEMORY USAGE");
                }

                if ( (PointsCount / lapsed.TotalMilliseconds) < 500)
                {
                    throw new Exception("Test1.Main(): TEST FAILED: TOO SLOW");
                }
            }
        }
    }
}
