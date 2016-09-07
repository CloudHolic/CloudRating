using System;
using System.Collections.Generic;

using CloudRating.Structures;

// ReSharper disable InconsistentNaming
namespace CloudRating.Processor
{
    public class PatternAnalyzer
    {
        public List<Note>[] Notes { get; }

        public List<LongNote> LNs { get; }

        public int Key { get; }

        public int Count { get; }

        public PatternAnalyzer(List<Note> notes, List<LongNote> lns, int key)
        {
            Key = key;
            LNs = lns;
            Count = notes.Count + lns.Count;

            Notes = new List<Note>[key];

            for (var i = 0 ; i < Notes.Length ; i++)
                Notes[i] = new List<Note>();

            foreach (var cur in notes)
                Notes[cur.Line].Add(cur);
        }

        public double GetJackRatio()
        {
            var jackCount = 0;

            //  Jack: 3 or more notes which is in same line with gap <= 107ms.
            foreach (var curLine in Notes)
            {
                for(var j = 0 ; j < curLine.Count - 2 ; j++)
                {
                    //  Find a new jack section.
                    var gap1 = curLine[j + 1].Time - curLine[j].Time;
                    var gap2 = curLine[j + 2].Time - curLine[j + 1].Time;

                    if (gap1 > 107 || gap2 > 107)
                        continue;

                    jackCount++;
                    for (var k = j + 2 ; k < curLine.Count - 2 ; k++)
                    {
                        if (curLine[k + 1].Time - curLine[k].Time > 107)
                        {
                            j = k;
                            break;
                        }

                        jackCount++;
                    }
                }
            }

            return Math.Round((double)jackCount / Count * 100, 2);
        }

        public double GetAntijackRatio()
        {
            return 0;
        }
    }
}