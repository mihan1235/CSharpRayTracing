using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmSharp;
using System.Runtime.InteropServices;
using static glfw3.State;
using OpenGL;
using System.Collections;


namespace Chapter_5_Surface_Normals
{
    using static Program;
    class SceneRayTracing : Scene
    {
        IShader renderProg, computeProg;

        int width, height;
        //output texture
        uint imgTex;

        uint screen_VAO, screen_VBO;

        void CompileAndLinkShader()
        {
            renderProg = new Shader("shaders/render.vert", "shaders/render.frag");
            computeProg = new ComputeShader("shaders/ray_tracing.comp");
        }

        void InitBuffers()
        {
            //Init output texture

            uint[] buf = new uint[1];
            Gl.GenTextures(buf);
            imgTex = buf[0];
            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.Texture2d, imgTex);
            Gl.TexStorage2D(TextureTarget.Texture2d, 1,InternalFormat.Rgba8, width, height);
            Gl.BindImageTexture(0, imgTex, 0, false, 0, BufferAccess.ReadWrite,
            InternalFormat.Rgba8);

            //init VBO for rendering texture
            float[] vertices = {
		        // Vertex attributes for a quad that fills the entire screen in Normalized
		        // Device Coordinates.
		        // Positions   // TexCoords
		        -1.0f,  1.0f,  0.0f, 1.0f,
                -1.0f, -1.0f,  0.0f, 0.0f,
                1.0f, -1.0f,  1.0f, 0.0f,

                -1.0f,  1.0f,  0.0f, 1.0f,
                1.0f, -1.0f,  1.0f, 0.0f,
                1.0f,  1.0f,  1.0f, 1.0f
            };


            Gl.GenVertexArrays(buf);
            screen_VAO = buf[0];
            Gl.GenBuffers(buf);
            screen_VBO = buf[0];
            Gl.BindVertexArray(screen_VAO);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, screen_VBO);
            Gl.BufferData(BufferTarget.ArrayBuffer, (uint)(sizeof(float)*vertices.Length), vertices, BufferUsage.StaticDraw);
            Gl.EnableVertexAttribArray(0);
            Gl.VertexAttribPointer(0, 2, VertexAttribType.Float, false, 4 * sizeof(float), IntPtr.Zero);
            Gl.EnableVertexAttribArray(1);
            Gl.VertexAttribPointer(1, 2, VertexAttribType.Float, false, 4 * sizeof(float) , new IntPtr(2 * sizeof(float)));
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
            Gl.BindVertexArray(0);
        }


        void SetMatrices()
        {
            //renderProg.Use();
        }

        public SceneRayTracing(int width , int hight) 
        {
            this.width = width;
            this.height = hight;
        }

        public override void InitScene()
        {
            CompileAndLinkShader();
            InitBuffers();

            Gl.ClearColor(1, 1, 1, 1);

        }

        public override void Update(float t)
        {
            
        }

        public override void Render()
        {
            // Execute the compute shader
            computeProg.Use();
            computeProg.SetInt("width", width);
            computeProg.SetInt("height", height);
            //Console.WriteLine("{0} {1}", (uint)(width / 10), (uint)(height / 10));
            Gl.DispatchCompute((uint)(width / 10), (uint)(height / 10), 1);
            Gl.MemoryBarrier(MemoryBarrierMask.ShaderStorageBarrierBit);

            // Draw the scene
            renderProg.Use();
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            SetMatrices();
            Gl.BindVertexArray(screen_VAO);
            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.Texture2d, imgTex);
            // Draw the texture on the screen
            Gl.DrawArrays(PrimitiveType.Triangles, 0, 6);
            Gl.BindVertexArray(0);
            Gl.BindTexture(TextureTarget.Texture2d, 0);

        }

        public override void Resize(int w, int h)
        {
            Gl.Viewport(0, 0, w, h);
            width = w;
            height = h;
        }
    }
}
