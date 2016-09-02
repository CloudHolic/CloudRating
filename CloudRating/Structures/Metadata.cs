namespace CloudRating.Structures
{
    public struct Metadata
    {
        public Metadata(double bpm, double hp, double od, int key, string title, string artist, string creator, string diff)
        {
            Bpm = bpm;
            Hp = hp;
            Od = od;
            Keys = key;

            Title = title;
            Artist = artist;
            Creator = creator;
            Diff = diff;
        }

        public double Bpm { get; set; }

        public double Hp { get; set; }

        public double Od { get; set; }

        public int Keys { get; set; }

        public string Title { get; set; }

        public string Artist { get; set; }

        public string Creator { get; set; }

        public string Diff { get; set; }
    }
}
