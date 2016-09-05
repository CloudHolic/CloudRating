using CloudRating.CustomExceptions;

namespace CloudRating.Processor
{
    public class LineConverter
    {
        public LineConverter(int k)
        {
            if (k < 4 || k > 8)
                throw new InvalidBeatmapException("Only 4~8k maps are allowed.");
            Key = k;
        }

        public int Key { get; }

        public int GetLine(int coordinate)
        {
            int result;

            if (Key == 4)
            {
                switch (coordinate)
                {
                    case 36:
                    case 64:
                    case 109:
                        result = 0;
                        break;
                    case 182:
                    case 192:
                        result = 1;
                        break;
                    case 256:
                    case 320:
                    case 329:
                        result = 2;
                        break;
                    case 402:
                    case 448:
                    case 475:
                        result = 3;
                        break;
                    default:
                        throw new InvalidBeatmapException("Unknown x-coordinate detected.");
                }
            }
            else if (Key == 5)
            {
                if (coordinate < 100)
                    result = 0;
                else if (coordinate > 100 && coordinate < 200)
                    result = 1;
                else if (coordinate > 200 && coordinate < 300)
                    result = 2;
                else if (coordinate > 300 && coordinate < 400)
                    result = 3;
                else if (coordinate > 400)
                    result = 4;
                else
                    throw new InvalidBeatmapException("Unknown x-coordinate detected.");
            }
            else if (Key == 6)
            {
                switch (coordinate)
                {
                    case 42:
                        result = 0;
                        break;
                    case 128:
                        result = 1;
                        break;
                    case 213:
                        result = 2;
                        break;
                    case 298:
                        result = 3;
                        break;
                    case 383:
                    case 384:
                        result = 4;
                        break;
                    case 469:
                        result = 5;
                        break;
                    default:
                        throw new InvalidBeatmapException("Unknown x-coordinate detected.");
                }
            }
            else if (Key == 7)
            {
                switch (coordinate)
                {
                    case 36:
                        result = 0;
                        break;
                    case 109:
                        result = 1;
                        break;
                    case 182:
                        result = 2;
                        break;
                    case 256:
                        result = 3;
                        break;
                    case 320:
                    case 329:
                        result = 4;
                        break;
                    case 402:
                        result = 5;
                        break;
                    case 448:
                    case 475:
                        result = 6;
                        break;
                    default:
                        throw new InvalidBeatmapException("Unknown x-coordinate detected.");
                }
            }
            else if (Key == 8)
            {
                switch (coordinate)
                {
                    case 32:
                        result = 0;
                        break;
                    case 96:
                        result = 1;
                        break;
                    case 160:
                        result = 2;
                        break;
                    case 224:
                        result = 3;
                        break;
                    case 288:
                        result = 4;
                        break;
                    case 352:
                        result = 5;
                        break;
                    case 416:
                        result = 6;
                        break;
                    case 480:
                        result = 7;
                        break;
                    default:
                        throw new InvalidBeatmapException("Unknown x-coordinate detected.");
                }
            }
            else
                throw new InvalidBeatmapException("Only 4~8k maps are allowed.");

            return result;
        }
    }
}