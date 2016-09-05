using System;
using System.Collections.Generic;
using System.IO;

using CloudRating.Parser;
using CloudRating.Structures;
using CloudRating.Processor;

// ReSharper disable InconsistentNaming
namespace CloudRating.Beatmap
{
    //  Represents a beatmap. It contains all information about a single osu-file.
    public struct BeatmapInfo
    {
        public Metadata Data { get; }

        public double MaxDen { get; }

        public double AvgDen { get; }

        public double CorMaxDen { get; }

        public double CorAvgDen { get; }

        public BeatmapInfo(string filename)
        {
            List<Note> notes;
            List<LongNote> lns;

            //  Load, and parse.
            if (File.Exists(filename))
            {
                Data = MetadataParser.Parse(filename);
                HitObjectParser.Parse(filename, out notes, out lns, Data.Keys);
            }
            else
                throw new FileNotFoundException();

            //  Calculate densities.
            var orgDen = DensityCalculator.GetDensity(ref notes, ref lns);
            var corDen = DensityCalculator.GetCorrectedDensity(ref notes, ref lns, Data.Keys);

            MaxDen = orgDen.Item1;
            AvgDen = orgDen.Item2;
            CorMaxDen = corDen.Item1;
            CorAvgDen = corDen.Item2;

            //  TODO: Calculate Rating.
        }
    }
}