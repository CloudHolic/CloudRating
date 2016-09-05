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

        public Tuple<double, double, double, double> GetBeatmapDensities()
        {

            var orgDen = DensityCalculator.GetDensity(ref Notes, ref LNs);
            var corDen = DensityCalculator.GetCorrectedDensity(ref Notes, ref LNs, Data.Keys);

            return Tuple.Create(orgDen.Item1, orgDen.Item2, corDen.Item1, corDen.Item2);
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