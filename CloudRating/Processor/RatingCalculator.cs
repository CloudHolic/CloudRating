﻿using System;

using CloudRating.Beatmap;

namespace CloudRating.Processor
{
    public static class RatingCalculator
    {
        public static double CalcRating(BeatmapInfo map)
        {
            //  Start from 'Corrected Max Density'
            var result = map.CorMaxDen;

            //  Density correction.
            //  If 'Corrected Average Density' is really close with 'Corrected Max Density',
            if (map.CorMaxDen <= map.CorAvgDen * 1.1)
                //  then it means it's quite hard compared with its max density.
                //  Get bonus score.
                result += map.CorAvgDen * 0.1;

            //  If 'Corrected Average Density' is too low than 'Corrected Max Density',
            else if (map.CorMaxDen >= map.CorAvgDen * 1.5)
                //  then it means it's quite easy compared with its max density.
                //  Deduct rating score.
                result -= map.CorMaxDen * 0.1;

            //  General correction for density.
            var correction = Math.Pow(10, Math.Log(map.CorMaxDen, map.CorAvgDen) - 1);
            result -= correction;


            //  Jack correction.
            //  Call PatternAnalyzer to find Jacks.
            var jacks = PatternAnalyzer.FindJacks(map.Notes, map.LNs, map.Data.Keys);


            //  Key correction.
            //  Standard: 6k.
            //  Other key modes' rating value is set to 6k like 'key / 6'.
            result *= (double)map.Data.Keys / 6;

            //  Double the score, and round it only for convenience.
            result = Math.Round(result * 2, 2);

            return result;
        }
    }
}
