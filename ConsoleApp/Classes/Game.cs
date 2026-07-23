using System.Drawing;
using OpenTK.Graphics.ES11;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Classes
{
    public class Game : GameWindow
    {
        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, Classes.Configuration configuration)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            _configuration = configuration;
            _run = true;
        }

        public void Stop()
        {
            _run = false;
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.Enable(EnableCap.DepthTest);
            GL.Disable(EnableCap.CullFace);
            GL.ClearColor(0f, 0f, 0f, 1f);
            GL.Enable(EnableCap.FramebufferSrgb);

            _camera = new Classes.Camera(((float)Size.X) / ((float)Size.Y));
            _scene = new Classes.Scene(_configuration);
            _renderer = new Classes.Renderer();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            _renderer.Render(_camera, _scene);

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (!_run)
            {
                Close();
                return;
            }

            _isReady = true;
        }

        public bool IsReady { get { return _isReady; } }

        private Classes.Configuration _configuration { get; set; }
        private bool _run { get; set; }
        private bool _isReady { get; set; }
        private Classes.Camera _camera { get; set; }
        private Classes.Scene _scene { get; set; }
        private Classes.Renderer _renderer { get; set; }
    }
}