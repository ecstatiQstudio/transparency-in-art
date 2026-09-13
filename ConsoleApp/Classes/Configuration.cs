using System.Collections.Generic;

namespace Classes
{
    public class Configuration
    {
        public class Track
        {
            public int number { get; set; }
        }

        public class Instrument
        {
            public string name { get; set; }
            public List<Track> tracks { get; set; }
            public string pathToInputGltfFile { get; set; }
            public float gltfTargetSize { get; set; }
            public float gltfXRotationDegrees { get; set; }
            public float gltfYRotationDegrees { get; set; }
            public float gltfZRotationDegrees { get; set; }
            public Classes.GltfLoader gltfLoader { get; set; }
        }

        public string pathToInputMidiFile { get; set; }
        public string pathToInputTimedLyricsFile { get; set; }
        public List<Instrument> instruments { get; set; }
    }
}