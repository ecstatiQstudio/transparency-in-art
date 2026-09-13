using OpenTK.Windowing.Desktop;

namespace Classes
{
    public class Room
    {
        public Room(Classes.Configuration configuration, Classes.Configuration.Instrument instrument, int width, int height)
        {
            _configuration = configuration;
            _instrument = instrument;
            _width = width;
            _height = height;

            NativeWindowSettings nativeWindowSettings = new NativeWindowSettings
            {
                ClientSize = new OpenTK.Mathematics.Vector2i(_width, _height),
                Title = "Next Steps Dear",
                NumberOfSamples = 0
            };

            _game = new Classes.Game(GameWindowSettings.Default, nativeWindowSettings, _configuration, _instrument);
        }

        public void Run()
        {
            _game.Run();
        }

        public void Stop()
        {
            _game.Stop();
        }

        public bool IsReady { get { return _game.IsReady; } }

        private Classes.Configuration _configuration { get; set; }
        private Classes.Configuration.Instrument _instrument { get; set; }
        private int _width { get; set; }
        private int _height { get; set; }
        private Classes.Game _game { get; set; }
    }
}