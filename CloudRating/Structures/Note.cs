namespace CloudRating.Structures
{
    //  Represents a simple note, not LN.
    public struct Note
    {
        public Note(int time, int line)
        {
            Time = time;
            Line = line;
        }

        public int Time { get; set; }

        public int Line { get; set; }
    }
}
