using System;
using CloudRating.Beatmap;

namespace CloudRating.Processor
{
    public static class RatingCalculator
    {
        public static double CalcRating(BeatmapInfo map)
        {
            //  Call PatternAnalyzer to find Jacks / Spams.
            var analyzer = new PatternAnalyzer(map.Notes, map.LNs, map.Data.Keys, map.Data.SpecialStyle);
            var jacks = analyzer.GetJackRatio();
            var spams = analyzer.GetSpamRatio();
            
            //  Spam correction.
            //  Deduct 'Corrected Max Density' and 'Corrected Average Density' by exponential function.
            var corMaxDen = map.CorMaxDen * (1 - (Math.Pow(78, spams) - 1) / 100);
            var corAvgDen = map.CorAvgDen * (1 - (Math.Pow(78, spams) - 1) / 100);
            
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
            var correction = Math.Pow(6, Math.Log(corMaxDen, corAvgDen + corMaxDen)) - 1;
            result *= 1 - correction / 10;
            
            //  Jack correction.
            //  Increase Rating by exponential function.
            result *= 1 + (Math.Pow(101, jacks) - 1) / 100;
            
            //  Key correction.
            //  Standard: 6k.
            //  Other key modes' rating value is set to 6k like '(key + 1) / 7'.
            //  If it's special style, than key -= 1.
            var specialStyle = map.Data.Keys == 8 && (double)(analyzer.Notes[0].Count + analyzer.LNs[0].Count) / analyzer.Count < 0.06;
            result *= (double)((specialStyle ? map.Data.Keys - 1 : map.Data.Keys) + 1) / 7;

            //  Multiply it only for convenience.
            result *= 1.6;

            return result;
        }
    }
}