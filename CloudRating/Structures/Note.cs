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

    //  Represents a simple note, including LN-counter.
    public class NoteCount
    {
        public NoteCount(int time, int line, int lns)
        {
            Time = time;
            Line = line;
            LNs = lns;
        }

        public int Time { get; set; }

        public int Line { get; set; }

        public int LNs { get; set; }
    }
}
