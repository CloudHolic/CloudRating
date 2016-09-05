using System;
using System.Collections.Generic;
using System.Linq;

using CloudRating.Structures;

namespace CloudRating.Processor
{
    public static class DensityCalculator
    {
        //  Returns a tuple of (Max Density, Average Density)
        public static Tuple<double, double> GetDensity(ref List<Note> notes, ref List<LongNote> lns)
        {
            List<int> denList;

            //  Get the list of densities.
            CalcDensityList(ref notes, ref lns, out denList);

            //  Check if the density list is empty.
            if(denList.Count == 0)
                throw new InvalidOperationException("Empty list");

            //  Find the maximum density.
            var maxDens = denList.Max();

            //  Remove the least 25% densities.
            var count = (int)Math.Ceiling((double)denList.Count / 4);
            for (var i = 0; i < count; i++)
                denList.Remove(denList.Min());

            //  Calculate the average density.
            var average = denList.Aggregate(0.0, (current, cur) => current + cur);
            average = Math.Round(average / denList.Count, 2);

            return Tuple.Create((double)maxDens, average);
        }

        //  Returns a tuple of (Corrected Max Density, Corrected Average Density)
        public static Tuple<double, double> GetCorrectedDensity(ref List<Note> notes, ref List<LongNote> lns, int key)
        {
            List<double> denList;
            CalcCorrectedDensities(ref notes, ref lns, key, out denList);

            //  Check if the density list is empty.
            if(denList.Count == 0)
                throw new InvalidOperationException("Empty List");

            //  Find the highest 1% densities, and calculate the average.
            var maxCount = (int)Math.Ceiling((double)denList.Count / 100);
            double maxAverage = 0;
            for (var i = 0; i < maxCount; i++)
                maxAverage += denList.OrderByDescending(cur => cur).ToArray()[i];
            maxAverage = Math.Round(maxAverage / maxCount, 2);

            //  Remove the least 25% densities.
            var minCount = (int)Math.Ceiling((double)denList.Count / 4);
            for (var i = 0; i < minCount; i++)
                denList.Remove(denList.Min());

            //  Calculate the average density.
            var average = denList.Aggregate(0.0, (current, cur) => current + cur);
            average = Math.Round(average / denList.Count, 2);

            return Tuple.Create(maxAverage, average);
        }

        private static void CalcDensityList(ref List<Note> notes, ref List<LongNote> lns, out List<int> density)
        {
            int startTiming, endTiming;
            density = new List<int>();

            if (notes.Count == 0)
            {
                startTiming = lns[0].Time;
                endTiming = lns[lns.Count - 1].Time;
            }
            else if (lns.Count == 0)
            {
                startTiming = notes[0].Time;
                endTiming = notes[notes.Count - 1].Time;
            }
            else
            {
                startTiming = Math.Min(notes[0].Time, lns[0].Time);
                endTiming = Math.Max(notes[notes.Count - 1].Time, lns[lns.Count - 1].Time);
            }

            var startPeriod = startTiming - startTiming % 250;
            var endPeriod = endTiming + (250 - endTiming % 250);

            for (var i = startPeriod; i < endPeriod - 1000; i += 250)
            {
                var counts = 0;

                //  Count the number of simple notes.
                counts += notes.Count(cur => cur.Time >= i && cur.Time <= i + 1000);

                //  Count the number of long notes.
                counts += lns.Count(cur => (cur.Time >= i && cur.Time <= i + 1000)
                                    || (cur.Endtime >= i && cur.Endtime <= i + 1000) || (cur.Time <= i && cur.Endtime >= i + 1000));

                density.Add(counts);
            }
        }

        private static void CalcCorrectedDensities(ref List<Note> notes, ref List<LongNote> lns, int key, out List<double> density)
        {
            int startTiming, endTiming;
            var corNotes = new List<NoteCount>();
            var corLNs = new List<LongNoteCount>();
            var Base = new Dictionary<int, double>();

            density = new List<double>();

            //  Add Base info.
            Base.Add(4, 1.63864);
            Base.Add(5, 1.53141);
            Base.Add(6, 1.45851);
            Base.Add(7, 1.40523);
            Base.Add(8, 1.36435);

            if (notes.Count == 0)
            {
                startTiming = lns[0].Time;
                endTiming = lns[lns.Count - 1].Time;
            }
            else if (lns.Count == 0)
            {
                startTiming = notes[0].Time;
                endTiming = notes[notes.Count - 1].Time;
            }
            else
            {
                startTiming = Math.Min(notes[0].Time, lns[0].Time);
                endTiming = Math.Max(notes[notes.Count - 1].Time, lns[lns.Count - 1].Time);
            }

            var startPeriod = startTiming - startTiming % 250;
            var endPeriod = endTiming + (250 - endTiming % 250);

            for (var i = startPeriod; i < endPeriod - 1000; i += 250)
            {
                corNotes.Clear();
                corLNs.Clear();

                //  Get the notes in current period.
                corNotes.AddRange(from cur in notes where cur.Time >= i && cur.Time <= i + 1000
                                  select new NoteCount(cur.Time, cur.Line, 0));

                corLNs.AddRange(from cur in lns where (cur.Time >= i && cur.Time <= i + 1000)
                                                        || (cur.Endtime >= i && cur.Endtime <= i + 1000)
                                                        || (cur.Time <= i && cur.Endtime >= i + 1000)
                                select new LongNoteCount(cur.Time, cur.Endtime, cur.Line, 0));

                //  Count the LN-count for each notes.
                foreach (var cur in corLNs)
                {
                    foreach (var note in corNotes)
                    {
                        if (note.Time >= cur.Time && note.Time < cur.Endtime)
                            note.LNs++;
                    }

                    for (var j = corLNs.IndexOf(cur) + 1; j < corLNs.Count; j++)
                    {
                        if ((corLNs[j].Time >= cur.Time && corLNs[j].Time < cur.Endtime)
                            || (corLNs[j].Endtime >= cur.Time && corLNs[j].Endtime <= cur.Endtime)
                            || (corLNs[j].Time <= cur.Time && corLNs[j].Endtime >= cur.Endtime))
                            corLNs[j].LNs++;
                    }
                }

                //  Correct the density.
                var den = (corNotes.Sum(cur => (double) key/(key - cur.LNs)) + corLNs.Sum(cur => Math.Pow(Base[key], cur.LNs))) / key;

                density.Add(den);
            }
        }
    }
}
