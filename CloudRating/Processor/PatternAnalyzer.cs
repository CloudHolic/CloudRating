using System;
using System.Collections.Generic;
using System.Linq;

using CloudRating.Structures;

// ReSharper disable InconsistentNaming
namespace CloudRating.Processor
{
    public class PatternAnalyzer
    {
        public List<Note>[] Notes { get; }

        public List<LongNote>[] LNs { get; }

        public Dictionary<int, List<int>> Times { get; }

        public Dictionary<int, List<Tuple<int, int>>> LN_Times { get; }

        public int Key { get; }

        public int Count { get; }

        public PatternAnalyzer(List<Note> notes, List<LongNote> lns, int key)
        {
            Key = key;
            Count = notes.Count + lns.Count;

            Notes = new List<Note>[key];
            LNs = new List<LongNote>[key];
            Times = new Dictionary<int, List<int>>();
            LN_Times = new Dictionary<int, List<Tuple<int, int>>>();

            for (var i = 0; i < key; i++)
            {
                Notes[i] = new List<Note>();
                LNs[i] = new List<LongNote>();
            }

            foreach (var cur in notes)
            {
                Notes[cur.Line].Add(cur);

                if(!Times.ContainsKey(cur.Time))
                    Times.Add(cur.Time, new List<int>());
                Times[cur.Time].Add(cur.Line);
            }

            foreach (var cur in lns)
            {
                LNs[cur.Line].Add(cur);

                if (!LN_Times.ContainsKey(cur.Time))
                    LN_Times.Add(cur.Time, new List<Tuple<int, int>>());
                LN_Times[cur.Time].Add(Tuple.Create(cur.Line, cur.Endtime));
            }
        }

        public double GetJackRatio()
        {
            var jackCount = 0;
            var sectionList = new List<int>();

            //  Jack: 3 or more notes which is in same line with gap <= 107ms.
            for (var i = 0 ; i < Notes.Length ; i++)
            {
                for (var j = 0; j < Notes[i].Count - 2; j++)
                {
                    if (j + 2 > Notes[i].Count)
                        break;

                    //  Find a new jack section.
                    var gap1 = Notes[i][j + 1].Time - Notes[i][j].Time;
                    var gap2 = Notes[i][j + 2].Time - Notes[i][j + 1].Time;

                    if (gap1 > 107 || gap2 > 107)
                        continue;

                    sectionList.Clear();
                    sectionList.Add(Notes[i][j].Time);
                    sectionList.Add(Notes[i][j + 1].Time);
                    sectionList.Add(Notes[i][j + 2].Time);
                    for (var k = j + 2; k < Notes[i].Count - 1; k++)
                    {
                        if (Notes[i][k + 1].Time - Notes[i][k].Time > 107)
                        {
                            j = k;
                            break;
                        }

                        sectionList.Add(Notes[i][k + 1].Time);
                    }

                    //  Check if there're any other notes except jack
                    var onlyJack = true;
                    for (var k = 0; k < Notes.Length; k++)
                    {
                        if (k == i)
                            continue;

                        for (var l = 0; l < Notes[k].Count; l++)
                        {
                            if (Notes[k][l].Time < sectionList[0])
                                continue;

                            if (Notes[k][l].Time > sectionList[0])
                            {
                                onlyJack = onlyJack && Notes[k][l].Time > sectionList[sectionList.Count - 1];
                                break;
                            }

                            var temp = true;
                            for (var m = 1; m < sectionList.Count; m++)
                            {
                                if ((l + m < Notes[k].Count) && (Notes[k][l + m].Time == sectionList[m]))
                                    continue;

                                temp = false;
                                break;
                            }

                            onlyJack = onlyJack && temp;
                            break;
                        }

                        for (var l = 0; l < LNs[k].Count; l++)
                        {
                            if (LNs[k][l].Endtime <= sectionList[0])
                                continue;
                            onlyJack = onlyJack && LNs[k][l].Time > sectionList[sectionList.Count - 1];
                            break;
                        }
                    }

                    //  If onlyJack, then 107ms => 94ms.
                    if (onlyJack)
                    {
                        for (var k = 0; k < sectionList.Count - 2; k++)
                        {
                            if (k + 2 > sectionList.Count)
                                break;

                            var newGap1 = sectionList[k + 1] - sectionList[k];
                            var newGap2 = sectionList[k + 2] - sectionList[k + 1];

                            if (newGap1 > 94 || newGap2 > 94)
                                continue;

                            jackCount++;
                            for (var l = k + 2; l < sectionList.Count - 1; l++)
                            {
                                if (sectionList[l + 1] - sectionList[l] > 94)
                                {
                                    k = l;
                                    break;
                                }

                                jackCount++;
                            }
                        }
                    }
                    else
                        jackCount += sectionList.Count - 2;
                }
            }

            return (double)jackCount / Count * 100;
        }

        public double GetSpamRatio()
        {
            return 0;
        }
    }
}