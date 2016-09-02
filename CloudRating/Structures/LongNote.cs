namespace CloudRating.Structures
{
    //  Represents an LN.
    public struct LongNote
    {
        public LongNote(int time, int endtime, int line)
        {
            Time = time;
            Endtime = endtime;
            Line = line;
        }

        public int Time { get; set; }

        public int Endtime { get; set; }

        public int Line { get; set; }
    }
}