using OpenTK.Graphics.OpenGL4;

namespace Classes
{
    public class Renderer
    {
        public void Render(Classes.Camera camera, Classes.Scene scene)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            foreach (Classes.Renderable renderable in scene.Objects)
            {
                renderable.Draw(camera);
            }
        }
    }
}