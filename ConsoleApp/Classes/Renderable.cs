using System;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace Classes
{
    public class Renderable
    {
        public Renderable(Classes.Mesh mesh, Classes.Shader shader, OpenTK.Mathematics.Matrix4[] instances)
        {
            _mesh = mesh;
            _shader = shader;
            _instances = instances;

            _vbo = GL.GenBuffer();
            
            GL.BindVertexArray(_mesh.Vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _instances.Length * Marshal.SizeOf<OpenTK.Mathematics.Matrix4>(), instances, BufferUsageHint.StaticDraw);

            int offset = 4 * sizeof(float);
            int stride = offset * 4;

            for (int i = 0; i < 4; i++)
            {
                GL.EnableVertexAttribArray(2 + i);
                GL.VertexAttribPointer(2 + i, 4, VertexAttribPointerType.Float, false, stride, i * offset);
                GL.VertexAttribDivisor(2 + i, 1);
            }
        }

        public void Draw(Classes.Camera camera)
        {
            _shader.Use();

            GL.Uniform1(GL.GetUniformLocation(_shader.Handle, "texture0"), 0);
            GL.Uniform1(GL.GetUniformLocation(_shader.Handle, "albedoTex"), 0);
            GL.Uniform1(GL.GetUniformLocation(_shader.Handle, "mrTex"), 1);

            OpenTK.Mathematics.Matrix4 view = camera.GetView();
            OpenTK.Mathematics.Matrix4 projection = camera.GetProjection();

            GL.UniformMatrix4(GL.GetUniformLocation(_shader.Handle, "uView"), false, ref view);
            GL.UniformMatrix4(GL.GetUniformLocation(_shader.Handle, "uProj"), false, ref projection);
            GL.Uniform3(GL.GetUniformLocation(_shader.Handle, "uCameraPos"), camera.Position);
            GL.Uniform3(GL.GetUniformLocation(_shader.Handle, "uColor"), OpenTK.Mathematics.Vector3.One);
            GL.Uniform1(GL.GetUniformLocation(_shader.Handle, "uMetallic"), 0f);
            GL.Uniform1(GL.GetUniformLocation(_shader.Handle, "uRoughness"), 1f);
            
            GL.BindVertexArray(_mesh.Vao);
            
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _mesh.AlbedoTexture);

            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, _mesh.MRTexture);

            GL.DrawElementsInstanced(PrimitiveType.Triangles, _mesh.IndexCount, DrawElementsType.UnsignedInt, IntPtr.Zero, _instances.Length);
        }
        
        private Classes.Mesh _mesh { get; set; }
        private Classes.Shader _shader { get; set; }
        private OpenTK.Mathematics.Matrix4[] _instances { get; set; }
        private int _vbo { get; set; }
    }
}