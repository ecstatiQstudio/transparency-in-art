using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;

namespace Classes
{
    public class Scene
    {
        public Scene(Classes.Configuration configuration)
        {
            _configuration = configuration;
            _objects = new List<Renderable>();
            CreateModel();
        }

        public List<Classes.Renderable> Objects { get { return _objects; } }

        private Classes.Mesh CreateBackWallMesh()
        {
            float size = 50f;
            float height = 100f;
            float z = -size;
            float[] vertices = [
                // positions        // normals      // uvs
                -size, 0, z,        0, 0, 1,        0, 0,
                size, 0, z,         0, 0, 1,        1, 0,
                size, height, z,    0, 0, 1,        1, 1,
                -size, height, z,   0, 0, 1,        0, 1
            ];
            uint[] indices = [
                0, 1, 2,
                0, 2, 3
            ];
            int texture = CreateGenericTexture();

            Classes.Mesh result = new Mesh(texture, texture, vertices, indices);

            return result;
        }

        private Classes.Mesh CreateFloorMesh()
        {
            float size = 50f;
            float[] vertices = [
                // positions        // normals      // uvs
                -size, 0, -size,    0, 1, 0,        0, 0,
                size, 0, -size,     0, 1, 0,        10, 0,
                size, 0, size,      0, 1, 0,        10, 10,
                -size, 0, size,     0, 1, 0,        0, 10
            ];
            uint[] indices = [
                0, 2, 1,
                0, 3, 2
            ];
            int texture = CreateGenericTexture();

            Classes.Mesh result = new Mesh(texture, texture, vertices, indices);

            return result;
        }

        private Classes.Mesh CreateLeftWallMesh()
        {
            float size = 50f;
            float height = 100f;
            float[] vertices = [
                // positions            // normals      // uvs
                -size, 0, -size,        1, 0, 0,        0, 0,
                -size, height, -size,   1, 0, 0,        0, 1,
                -size, height, size,    1, 0, 0,        1, 1,
                -size, 0, size,         1, 0, 0,        1, 0
            ];
            uint[] indices = [
                0, 1, 2,
                0, 2, 3
            ];
            int texture = CreateGenericTexture();

            Classes.Mesh result = new Mesh(texture, texture, vertices, indices);

            return result;
        }

        private Classes.Mesh CreateRightWallMesh()
        {
            float size = 50f;
            float height = 100f;
            float[] vertices = [
                // positions            // normals      // uvs
                size, 0, -size,         -1, 0, 0,        0, 0,
                size, height, -size,    -1, 0, 0,        0, 1,
                size, height, size,     -1, 0, 0,        1, 1,
                size, 0, size,          -1, 0, 0,        1, 0
            ];
            uint[] indices = [
                0, 1, 2,
                0, 2, 3
            ];
            int texture = CreateGenericTexture();

            Classes.Mesh result = new Mesh(texture, texture, vertices, indices);

            return result;
        }

        private void CreateModel()
        {
            // floor
            Classes.Mesh floorMesh = CreateFloorMesh();
            Classes.Shader floorShader = new Classes.Shader(Shaders.VS, Shaders.FS);
            OpenTK.Mathematics.Matrix4[] floorInstances = new OpenTK.Mathematics.Matrix4[1];

            floorInstances[0] = OpenTK.Mathematics.Matrix4.CreateScale(0.5f) * OpenTK.Mathematics.Matrix4.CreateTranslation(0f, 0f, 0f);
            _objects.Add(new Classes.Renderable(floorMesh, floorShader, floorInstances));

            // back wall
            Classes.Mesh backWallMesh = CreateBackWallMesh();
            Classes.Shader backWallShader = new Classes.Shader(Shaders.VS, Shaders.FS);
            OpenTK.Mathematics.Matrix4[] backWallInstances = new OpenTK.Mathematics.Matrix4[1];

            backWallInstances[0] = OpenTK.Mathematics.Matrix4.CreateScale(0.5f) * OpenTK.Mathematics.Matrix4.CreateTranslation(0f, 0f, 0f);
            _objects.Add(new Classes.Renderable(backWallMesh, backWallShader, backWallInstances));

            // left wall
            Classes.Mesh leftWallMesh = CreateLeftWallMesh();
            Classes.Shader leftWallShader = new Classes.Shader(Shaders.VS, Shaders.FS);
            OpenTK.Mathematics.Matrix4[] leftWallInstances = new OpenTK.Mathematics.Matrix4[1];

            leftWallInstances[0] = OpenTK.Mathematics.Matrix4.CreateScale(0.5f) * OpenTK.Mathematics.Matrix4.CreateTranslation(0f, 0f, 0f);
            _objects.Add(new Classes.Renderable(leftWallMesh, leftWallShader, leftWallInstances));

            // right wall
            Classes.Mesh rightWallMesh = CreateRightWallMesh();
            Classes.Shader rightWallShader = new Classes.Shader(Shaders.VS, Shaders.FS);
            OpenTK.Mathematics.Matrix4[] rightWallInstances = new OpenTK.Mathematics.Matrix4[1];

            rightWallInstances[0] = OpenTK.Mathematics.Matrix4.CreateScale(0.5f) * OpenTK.Mathematics.Matrix4.CreateTranslation(0f, 0f, 0f);
            _objects.Add(new Classes.Renderable(rightWallMesh, rightWallShader, rightWallInstances));
        }

        private int CreateGenericTexture()
        {
            int handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, handle);

            byte[] pixel = { 255, 255, 255, 255 };

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 1, 1, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixel);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ((int)TextureMinFilter.Nearest));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ((int)TextureMagFilter.Nearest));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, ((int)TextureWrapMode.Repeat));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, ((int)TextureWrapMode.Repeat));

            return handle;
        }

        private Classes.Configuration _configuration { get; set; }
        private List<Classes.Renderable> _objects { get; set; }
    }
}