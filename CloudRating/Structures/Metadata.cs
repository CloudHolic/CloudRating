namespace CloudRating.Structures
{
    public struct Metadata
    {
        public Metadata(double maxbpm, double minbpm, double hp, double od, int key, bool style,
            string title, string artist, string creator, string diff)
        {
            MaxBpm = maxbpm;
            MinBpm = minbpm;
            Hp = hp;
            Od = od;
            Keys = key;
            SpecialStyle = style;

            Title = title;
            Artist = artist;
            Creator = creator;
            Diff = diff;
        }

        public double MaxBpm { get; set; }

        public double MinBpm { get; set; }

        public double Hp { get; set; }

        public double Od { get; set; }

        public int Keys { get; set; }

        public bool SpecialStyle { get; set; }

        public string Title { get; set; }

        public string Artist { get; set; }

        public string Creator { get; set; }

        public string Diff { get; set; }
    }
}
