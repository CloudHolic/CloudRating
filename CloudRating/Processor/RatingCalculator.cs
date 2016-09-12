using System;

using CloudRating.Beatmap;

namespace CloudRating.Processor
{
    public static class RatingCalculator
    {
        public static double CalcRating(BeatmapInfo map)
        {
            //  Call PatternAnalyzer to find Jacks / Spams.
            var analyzer = new PatternAnalyzer(map.Notes, map.LNs, map.Data.Keys);
            var jacks = analyzer.GetJackRatio();
            var spams = analyzer.GetSpamRatio();


            //  Jack correction.


            //  Spam correction.
            //  Deduct 'Corrected Max Density' and 'Corrected Average Density' by exponential function.
            var corMaxDen = map.CorMaxDen - map.CorMaxDen * (Math.Pow(76, spams) - 1) / 100;
            var corAvgDen = map.CorAvgDen - map.CorAvgDen * (Math.Pow(76, spams) - 1) / 100;


            //  Start from 'Corrected Max Density'
            var result = corMaxDen;
            

            //  Density correction.
            //  If 'Corrected Average Density' is really close with 'Corrected Max Density',
            if (corMaxDen <= corAvgDen * 1.1)
                //  then it means it's quite hard compared with its max density.
                //  Get bonus score.
                result += corAvgDen * 0.1;

            //  If 'Corrected Average Density' is too low than 'Corrected Max Density',
            else if (corMaxDen >= 3 && corMaxDen >= corAvgDen * 1.5)
                //  then it means it's quite easy compared with its max density.
                //  Deduct rating score.
                result -= corMaxDen * 0.1;

            //  General correction for density.
            var correction = Math.Pow(10, Math.Log(corMaxDen, corAvgDen) - 1) - 1;
            result -= result * correction / 10;


            //  Key correction.
            //  Standard: 6k.
            //  Other key modes' rating value is set to 6k like 'key / 6'.
            result *= (double)map.Data.Keys / 6;

            //  Multiply it only for convenience.
            result = result * 1.2;

            return result;
        }
    }
}
