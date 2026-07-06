using System.Collections.Generic;

namespace Classes
{
    public class Lyrics
    {
        public class Timed
        {
            public class Line
            {
                public float begin { get; set; }
                public float end { get; set; }
                public string content { get; set; }
            }

            public List<Line> line { get; set; }
        }

        public Timed timed { get; set; }
    }
}