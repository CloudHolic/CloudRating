using System;
using System.IO;

using CloudRating.CustomExceptions;
using CloudRating.Structures;

namespace CloudRating.Parser
{
    public static class MetadataParser
    {
        public static Metadata Parse(string filename)
        {
            var data = new Metadata();

            using (var reader = new StreamReader(filename))
            {
                string currentLine;

                while ((currentLine = reader.ReadLine()) != null)
                {
                    //  Get Mode. If not 3, it's not a mania mode.
                    if (currentLine.StartsWith("Mode:"))
                    {
                        if (currentLine.Split(' ')[1] != "3")
                            throw new InvalidModeException("It's not a Mania beatmap.");
                    }

                    //  Title
                    if (currentLine.StartsWith("TitleUnicode:"))
                        data.Title = currentLine.Substring(currentLine.IndexOf(':') + 1);

                    //  Artist
                    if (currentLine.StartsWith("ArtistUnicode:"))
                        data.Artist = currentLine.Substring(currentLine.IndexOf(':') + 1);

                    //  Creator
                    if (currentLine.StartsWith("Creator:"))
                        data.Creator = currentLine.Substring(currentLine.IndexOf(':') + 1);

                    //  Difficulty
                    if (currentLine.StartsWith("Version:"))
                        data.Diff = currentLine.Substring(currentLine.IndexOf(':') + 1);

                    //  HP
                    if (currentLine.StartsWith("HPDrainRate:"))
                        data.Hp = Convert.ToDouble(currentLine.Split(':')[1]);

                    //  Keys
                    if (currentLine.StartsWith("CircleSize:"))
                        data.Keys = Convert.ToInt32(currentLine.Split(':')[1]);

                    //  Od
                    if (currentLine.StartsWith("OverallDifficulty:"))
                        data.Od = Convert.ToDouble(currentLine.Split(':')[1]);

                    //  BPM
                    if (currentLine.StartsWith("[TimingPoints]"))
                    {
                        string cur;
                        double minBpm = 0, maxBpm = 0;

                        while ((cur = reader.ReadLine()) != null)
                        {
                            if (cur == "")
                                break;

                            //  Osu stores BPM as 'Miliseconds/Beat'.
                            var msPerBeat = Convert.ToDouble(cur.Split(',')[1]);
                            if (msPerBeat < 0)
                                continue;

                            var curBpm = Math.Round(60000/msPerBeat, 2);
                            if (Math.Abs(maxBpm) < 0.001 && Math.Abs(minBpm) < 0.001)
                                maxBpm = minBpm = curBpm;

                            else if (curBpm >= maxBpm)
                                maxBpm = curBpm;
                            else if (curBpm <= minBpm)
                                minBpm = curBpm;
                        }

                        data.MaxBpm = maxBpm;
                        data.MinBpm = minBpm;

                        //  The rest part doesn't have any metadata.
                        break;
                    }
                }
            }

            return data;
        }
    }
}
