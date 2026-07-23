using OpenTK.Graphics.OpenGL4;

namespace Classes
{
    public class Shader
    {
        public Shader(string vs, string fs)
        {
            int vsHandle = Compile(ShaderType.VertexShader, vs);
            int fsHandle = Compile(ShaderType.FragmentShader, fs);

            _handle = GL.CreateProgram();
            GL.AttachShader(_handle, vsHandle);
            GL.AttachShader(_handle, fsHandle);
            GL.LinkProgram(_handle);
            GL.GetProgram(_handle, GetProgramParameterName.LinkStatus, out int success);

            // todo: error handling

            GL.DeleteShader(vsHandle);
            GL.DeleteShader(fsHandle);
        }

        public void Use()
        {
            GL.UseProgram(_handle);
        }

        public int Handle { get { return _handle; } }

        private int Compile(ShaderType shaderType, string shaderSource)
        {
            int result = GL.CreateShader(shaderType);

            GL.ShaderSource(result, shaderSource);
            GL.CompileShader(result);
            GL.GetShader(result, ShaderParameter.CompileStatus, out int success);

            // todo: error handling

            return result;
        }
        
        private int _handle { get; set; }
    }
}