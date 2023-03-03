using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_1___Synchronization_Algorithm
{
    /// <summary>
    /// Yield points
    /// </summary>
    class Synchronizer
    {
        private readonly List<DataSource> _sources; 

        public Synchronizer(List<DataSource> sources)
        {
            _sources = sources;
        }

        public IEnumerable<Point> GetNextPoint()
        {
            DataSource earliestStream = null;
            while (true)
            {
                //Find the stream with earliest time
                earliestStream = _sources.Where(stream => stream.Generated.Count != 0)
                .OrderBy(stream => stream.Generated.First().Time).FirstOrDefault();
                if (earliestStream == null)
                {
                  //All Generated queues are empty, so break the loop
                  yield break;
                }
                //Return the point and undo from the queue.
                yield return earliestStream.Generated.Dequeue();
            }
        }
    }
}
