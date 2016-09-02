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
    public class BeatmapInfo
    {
        private List<Note> Notes;
        private List<LongNote> LNs;

        public Metadata Data { get; set; }

        public BeatmapInfo(string filename)
        {
            LoadFile(filename);
        }

        public Tuple<double, double> GetBeatmapDensities()
        {
            return DensityCalculator.GetDensity(ref Notes, ref LNs, Data.Keys);
        }

        private void LoadFile(string filename)
        {
            if (File.Exists(filename))
            {
                Data = MetadataParser.Parse(filename);
                HitObjectParser.Parse(filename, out Notes, out LNs, Data.Keys);
            }
            else
                throw new FileNotFoundException();
        }
    }
}