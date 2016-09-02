using System;
using System.Collections.Generic;
using System.Linq;

using CloudRating.Structures;

namespace CloudRating.Processor
{
    public static class DensityCalculator
    {
        //  Returns a tuple of (Max Density, Average Density)
        public static Tuple<double, double> GetDensity(ref List<Note> notes, ref List<LongNote> lns, int key)
        {
            List<int> denList;

            //  Get the list of densities.
            CalcDensityList(ref notes, ref lns, key, out denList);

            //  Check if the dnesity list is empty.
            if(denList.Count == 0)
                throw new InvalidOperationException("Empty list");

            //  Find the maximum density.
            var maxDens = denList.Max();

            //  Remove the least 25% densities.
            var count = denList.Count / 4;
            for (var i = 0; i < count; i++)
                denList.Remove(denList.Min());

            //  Calculate the average density.
            var average = denList.Aggregate(0.0, (current, cur) => current + cur);
            average /= denList.Count;

            return Tuple.Create((double)maxDens, average);
        }

        private static void CalcDensityList(ref List<Note> notes, ref List<LongNote> lns, int key, out List<int> density)
        {
            density = new List<int>();

            var startTiming = Math.Min(notes[0].Time, lns[0].Time);
            var endTiming = Math.Max(notes[notes.Count - 1].Time, lns[lns.Count - 1].Time);

            var startPeriod = startTiming - startTiming % 250;
            var endPeriod = endTiming + (250 - endTiming % 250);

            for (var i = startPeriod; i < endPeriod - 1000; i += 250)
            {
                int simpleNotes = 0, longNotes = 0;

                //  Count the number of simple notes.
                simpleNotes += notes.Count(cur => cur.Time >= i && cur.Time <= i + 1000);

                //  Count the number of long notes.
                longNotes += lns.Count(cur => (cur.Time >= i && cur.Time <= i + 1000) || (cur.Endtime >= i && cur.Endtime <= i + 1000) || (cur.Time <= i && cur.Endtime >= i + 1000));

                density.Add(simpleNotes + longNotes);
            }
        }
    }
}
