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

                //  This program is valid only at v14.
                if (reader.ReadLine() != "osu file format v14")
                    throw new InvalidBeatmapException("It's not a v14 format.");

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
                        data.Title = currentLine.Split(':')[1];

                    //  Artist
                    if (currentLine.StartsWith("ArtistUnicode:"))
                        data.Artist = currentLine.Split(':')[1];

                    //  Creator
                    if (currentLine.StartsWith("Creator:"))
                        data.Creator = currentLine.Split(':')[1];

                    //  Difficulty
                    if (currentLine.StartsWith("Version:"))
                        data.Diff = currentLine.Split(':')[1];

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
                        currentLine = reader.ReadLine();

                        //  Osu stores BPM as 'Miliseconds/Beat'.
                        var msPerBeat = Convert.ToDouble(currentLine?.Split(',')[1]);

                        data.Bpm = Math.Round(60000 / msPerBeat, 2);

                        //  The rest part doesn't have any metadata.
                        break;
                    }
                }
            }

            return data;
        }
    }
}
