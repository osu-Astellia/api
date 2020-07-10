namespace AstelliaAPI
{
    public interface IScore
    {
        public int id { get; set; }

        public string beatmap_md5 { get; set; }
        public int userid { get; set; }

        public int score { get; set; }
        public int max_combo { get; set; }

        public int mods { get; set; }

        public int c300 { get; set; }

        public int c100 { get; set; }

        public int c50 { get; set; }
        public int cGeki { get; set; }

        public int cKatu { get; set; }


        public int cMiss { get; set; }

        public string time { get; set; }

        public sbyte play_mode { get; set; }

        public double accuracy { get; set; }

        public float pp { get; set; }
    }
}