using OpenTK.Graphics.OpenGL4;

namespace Classes
{
    public class Mesh
    {
        public Mesh(System.Numerics.Vector4 color, float metallicFactor, float roughnessFactor, bool hasMetallicRoughnessTexture, int mrTexture, int albedoTexture, bool hasNormalTexture, int normalTexture, float[] vertices, uint[] indices)
        {
            _color = color;
            _metallicFactor = metallicFactor;
            _roughnessFactor = roughnessFactor;
            _hasMetallicRoughnessTexture = hasMetallicRoughnessTexture;
            _mrTexture = mrTexture;
            _albedoTexture = albedoTexture;
            _hasNormalTexture = hasNormalTexture;
            _normalTexture = normalTexture;
            _vertices = vertices;
            _indices = indices;
            
            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            int vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            int ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            // size of 8 for: 3 pos, 3 normal, 2 uv
            int stride = 8 * sizeof(float);
            // pos
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
            GL.EnableVertexAttribArray(0);
            // normal
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
            // uv
            GL.VertexAttribPointer(6, 2, VertexAttribPointerType.Float, false, stride, 6 * sizeof(float));
            GL.EnableVertexAttribArray(6);
        }

        public int AlbedoTexture { get { return _albedoTexture; } }
        public int IndexCount { get { return _indices.Length; } }
        public int MRTexture { get { return _mrTexture; } }
        public int Vao { get { return _vao; } }

        private System.Numerics.Vector4 _color { get; set; }
        private float _metallicFactor { get; set; }
        private float _roughnessFactor { get; set; }
        private bool _hasMetallicRoughnessTexture { get; set; }
        private int _mrTexture { get; set; }
        private int _albedoTexture { get; set; }
        private bool _hasNormalTexture { get; set; }
        private int _normalTexture { get; set; }
        private float[] _vertices { get; set; }
        private uint[] _indices { get; set; }
        private int _vao { get; set; }
    }
}