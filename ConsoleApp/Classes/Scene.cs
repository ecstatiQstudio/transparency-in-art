using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;

namespace Classes
{
    public class Scene
    {
        public Scene(Classes.Configuration configuration, Classes.Configuration.Instrument instrument)
        {
            _configuration = configuration;
            _instrument = instrument;
            _objects = new List<Renderable>();
            CreateModel();
        }

        public List<Classes.Renderable> Objects { get { return _objects; } }

        private Classes.Mesh CreateBackWallMesh()
        {
            float size = 50f;
            float height = 50f;
            float z = -size;
            float[] vertices = [
                // positions        // normals      // uvs
                -size, 0, z,        0, 0, 1,        0, 0,
                size, 0, z,         0, 0, 1,        10, 0,
                size, height, z,    0, 0, 1,        10, 10,
                -size, height, z,   0, 0, 1,        0, 10
            ];
            uint[] indices = [
                0, 1, 2,
                0, 2, 3
            ];
            int texture = Classes.GltfLoader.CreateGenericTexture();

            Classes.Mesh result = new Mesh(System.Numerics.Vector4.One, 0.2f, 1f, true, texture, texture, false, texture, vertices, indices);

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
                0, 1, 2,
                0, 2, 3
            ];
            int texture = Classes.GltfLoader.CreateGenericTexture();

            Classes.Mesh result = new Mesh(System.Numerics.Vector4.One, 0.2f, 1f, true, texture, texture, false, texture, vertices, indices);

            return result;
        }

        private Classes.Mesh CreateLeftWallMesh()
        {
            float size = 50f;
            float height = 50f;
            float[] vertices = [
                // positions            // normals      // uvs
                -size, 0, -size,        1, 0, 0,        0, 0,
                -size, height, -size,   1, 0, 0,        0, 1,
                -size, height, size,    1, 0, 0,        1, 1,
                -size, 0, size,         1, 0, 0,        1, 0
            ];
            uint[] indices = [
                0, 2, 1,
                0, 3, 2
            ];
            int texture = Classes.GltfLoader.CreateGenericTexture();

            Classes.Mesh result = new Mesh(System.Numerics.Vector4.One, 0.2f, 1f, true, texture, texture, false, texture, vertices, indices);

            return result;
        }

        private Classes.Mesh CreateRightWallMesh()
        {
            float size = 50f;
            float height = 50f;
            float[] vertices = [
                // positions            // normals      // uvs
                size, 0, -size,         -1, 0, 0,        0, 0,
                size, height, -size,    -1, 0, 0,        0, 1,
                size, height, size,     -1, 0, 0,        1, 1,
                size, 0, size,          -1, 0, 0,        1, 0
            ];
            uint[] indices = [
                0, 2, 1,
                0, 3, 2
            ];
            int texture = Classes.GltfLoader.CreateGenericTexture();

            Classes.Mesh result = new Mesh(System.Numerics.Vector4.One, 0.2f, 1f, true, texture, texture, false, texture, vertices, indices);

            return result;
        }

        private void CreateInstrumentObjects()
        {
            Classes.Shader shader = new Shader(Classes.Shaders.VS, Classes.Shaders.FS);
            OpenTK.Mathematics.Matrix4[] instances = new OpenTK.Mathematics.Matrix4[1];

            instances[0] = OpenTK.Mathematics.Matrix4.CreateScale(0.5f) * OpenTK.Mathematics.Matrix4.CreateTranslation(0f, 0f, 0f);

            for (int i = 0; i < _instrument.gltfLoader.Meshes.Count; i++)
            {
                if (_objects.Count < 4 + i + 1)
                {
                    // first 4 are floor and 3 walls
                    _objects.Add(null);
                }

                _objects[4 + i] = new Classes.Renderable(_instrument.gltfLoader.Meshes[i], shader, instances);
            }
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

            _instrument.gltfLoader = new Classes.GltfLoader(_instrument.pathToInputGltfFile, _instrument.gltfTargetSize, _instrument.gltfXRotationDegrees, _instrument.gltfYRotationDegrees, _instrument.gltfZRotationDegrees);

            CreateInstrumentObjects();
        }

        private Classes.Configuration _configuration { get; set; }
        private Classes.Configuration.Instrument _instrument { get; set; }
        private List<Classes.Renderable> _objects { get; set; }
    }
}