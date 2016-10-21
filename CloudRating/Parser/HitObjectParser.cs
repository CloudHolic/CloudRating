using System;
using System.Collections.Generic;
using System.IO;

using CloudRating.CustomExceptions;
using CloudRating.Structures;
using CloudRating.Processor;

namespace CloudRating.Parser
{
    public static class HitObjectParser
    {
        public static void Parse(string filename, out List<Note> notes, out List<LongNote> lns, int keys)
        {
            LineConverter converter = new LineConverter(keys);
            notes = new List<Note>();
            lns = new List<LongNote>();

            using (var reader = new StreamReader(filename))
            {
                string currentLine;

                //  Find HitObjects tag.
                while ((currentLine = reader.ReadLine()) != null)
                    if (currentLine == "[HitObjects]")
                        break;

                //  Parsing notes.
                while ((currentLine = reader.ReadLine()) != null)
                {
                    //  Split current line with ','.
                    var splitLine = currentLine.Split(',');

                    if (splitLine.Length != 6)
                        throw new InvalidBeatmapException("Wrong HitObject format.");

                    //  x, y, time, 1, hitsound, addition for simple note.
                    //  x, y, time, 5, hitsound, addition for the first simple note.
                    if (Convert.ToInt32(splitLine[3]) == 1 || Convert.ToInt32(splitLine[3]) == 5)
                    {
                        var temp = new Note
                        {
                            Line = converter.GetLine(Convert.ToInt32(splitLine[0])),
                            Time = Convert.ToInt32(splitLine[2])
                        };

                        notes.Add(temp);
                    }

                    //  x, y, time, 128, hitsound, endtime:addition for long note.
                    else if (Convert.ToInt32(splitLine[3]) == 128 || (Convert.ToInt32(splitLine[3]) == 132))
                    {
                        var temp = new LongNote
                        {
                            Line = converter.GetLine(Convert.ToInt32(splitLine[0])),
                            Time = Convert.ToInt32(splitLine[2]),
                            Endtime = Convert.ToInt32(splitLine[5].Split(':')[0])
                        };

                        lns.Add(temp);
                    }

                    //  Any other types are unkown notes.
                    else
                        throw new InvalidBeatmapException("Unknown note type.");
                }
            }
        }
    }
}