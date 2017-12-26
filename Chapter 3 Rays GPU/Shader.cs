using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using OpenGL;
using GlmSharp;


namespace Chapter_3_Rays_GPU
{
    using static Program;
    public class Shader: IShader
    {
        public uint ID;
        String[] vertexShaderSource= new String[1];
        String[] fragmentShaderSource= new String[1];
        String[] geometryShaderSource = new String[1];

        String vertexPath, fragmentPath, geometryPath;

        uint vertex, fragment;
        uint geometry;

        // constructor generates the shader on the fly
        // ------------------------------------------------------------------------
        public Shader(String vertexPath, String fragmentPath, String 
                      geometryPath = null)
        {
            this.vertexPath = vertexPath;
            this.geometryPath = geometryPath;
            this.fragmentPath = fragmentPath;
            ReadSources();
            CompileShaders();
            CreateProgram();
            DeleteShaders();
        }

        public void Use()
        {
            Gl.UseProgram(ID);
        }

        void ReadSources()
        {
            try
            {
                vertexShaderSource[0] = File.ReadAllText(vertexPath);
                fragmentShaderSource[0] = File.ReadAllText(fragmentPath);
                if (geometryPath != null)
                {
                    geometryShaderSource[0] = File.ReadAllText(geometryPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR::SHADER::FILE_NOT_SUCCESFULLY_READ");
                Console.WriteLine("Message: {0}", ex.Message);
            }
        }

        void CompileShaders()
        {
            // 2. Compile shaders
            // Vertex Shader
            vertex = Gl.CreateShader(ShaderType.VertexShader);
            Gl.ShaderSource(vertex, vertexShaderSource);
            Gl.CompileShader(vertex);
            CheckCompileErrors(vertex, "VERTEX");
            // Fragment Shader
            fragment = Gl.CreateShader(ShaderType.FragmentShader);
            Gl.ShaderSource(fragment,fragmentShaderSource);
            Gl.CompileShader(fragment);
            CheckCompileErrors(fragment, "FRAGMENT");
            // If geometry shader is given, compile geometry shader
            if (geometryPath != null)
            {
                geometry = Gl.CreateShader(ShaderType.GeometryShader);
                Gl.ShaderSource(geometry,geometryShaderSource);
                Gl.CompileShader(geometry);
                CheckCompileErrors(geometry, "GEOMETRY");
            }
        }

        void CheckCompileErrors(uint shader, String type)
        {
            int[] success = new int[1];
            int length;
            StringBuilder infoLog = new StringBuilder(512);
            if (type != "PROGRAM")
            {
                Gl.GetShader(shader, ShaderParameterName.CompileStatus, success);
                if (success[0] != 1)
                {
                    Gl.GetShaderInfoLog(shader, 1024, out length, infoLog);
                    Console.WriteLine("ERROR::SHADER_COMPILATION_ERROR of type: {0}", type);
                    Console.WriteLine("{0}", infoLog);
                    Console.WriteLine(" -- --------------------------------------------------- -- ");
                }
            }
            else
            {
                Gl.GetProgram(shader, ProgramProperty.LinkStatus, success);
                if (success[0] != 1)
                {
                    Gl.GetProgramInfoLog(shader, 1024, out length, infoLog);
                    Console.WriteLine("ERROR::PROGRAM_LINKING_ERROR of type: {0}", type);
                    Console.WriteLine("{0}", infoLog);
                    Console.WriteLine(" -- --------------------------------------------------- -- ");
                }
            }
        }

        void CreateProgram()
        {
            // Shader Program
            ID = Gl.CreateProgram();
            Gl.AttachShader(ID, vertex);
            Gl.AttachShader(ID, fragment);
            if (geometryPath != null)
            {
                Gl.AttachShader(ID, geometry);
            }
            Gl.LinkProgram(ID);
            CheckCompileErrors(ID, "PROGRAM");
        }

        void DeleteShaders()
        {
            // Delete the shaders as they're linked into our program now and no longer necessery
            Gl.DeleteShader(vertex);
            Gl.DeleteShader(fragment);
            if (geometryPath != null)
            {
                Gl.DeleteShader(geometry);
            }
        }

        // utility uniform functions
        // ------------------------------------------------------------------------
        public void SetBool(String name, bool value)
        {
            switch (value){
                case true:
                    Gl.Uniform1(Gl.GetUniformLocation(ID, name),1);
                    break;
                case false:
                    Gl.Uniform1(Gl.GetUniformLocation(ID, name), 0);
                    break;
            }
        }
        // ------------------------------------------------------------------------
        public void SetInt(String name, int value)
        { 
            Gl.Uniform1(Gl.GetUniformLocation(ID, name), value); 
        }
        // ------------------------------------------------------------------------
        public void SetFloat(String name, float value)
        { 
            Gl.Uniform1(Gl.GetUniformLocation(ID, name), value);
        }

        public void SetMat4(String name, mat4 obj)
        {
            int transformLoc = Gl.GetUniformLocation(ID, name);
            Gl.UniformMatrix4(transformLoc, false, obj.Values1D);
        }

        public void SetVec3(String name, vec3 value)
        {
            int transformLoc = Gl.GetUniformLocation(ID, name);
            Gl.Uniform3(transformLoc, value.x, value.y, value.z);
        }

        public void SetVec4(String name, vec4 value)
        {
            int transformLoc = Gl.GetUniformLocation(ID, name);
            Gl.Uniform4(transformLoc, value.x, value.y, value.z, value.w);
        }

        public void SetMat3(String name, mat3 obj)
        {
            int transformLoc = Gl.GetUniformLocation(ID, name);
            Gl.UniformMatrix3(transformLoc, false, obj.Values1D);
        }
    }
}

