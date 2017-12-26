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


namespace SceneParticles
{
    using static Program;
    class SceneParticles : Scene
    {
        IShader renderProg, computeProg;

        int width, height;
        ivec3 nParticles;
        uint totalParticles;

        float time, deltaT, speed, angle;
        uint particlesVao;
        uint bhVao, bhBuf;  // black hole VAO and buffer
        vec4 bh1, bh2;

        mat4 view, model, projection;

        void CompileAndLinkShader()
        {
            renderProg = new Shader("shaders/particles.vert", "shaders/particles.frag");
            computeProg = new ComputeShader("shaders/particles.comp");
        }

        void InitBuffers()
        {
            // Initial positions of the particles
            ArrayList arrPos = new ArrayList();
            float[] initVel = new float[totalParticles * 4];
            for (int i=0;i<initVel.Length;i++)
            {
                initVel[i] =0.0f;
            }
            vec4 p = new vec4(0.0f, 0.0f, 0.0f, 1.0f);
            float dx = 2.0f / (nParticles.x - 1),
                    dy = 2.0f / (nParticles.y - 1),
                    dz = 2.0f / (nParticles.z - 1);
            // We want to center the particles at (0,0,0)
            mat4 transf = mat4.Identity;
            transf *= mat4.Translate(new vec3(-1, -1, -1));
            for (int i = 0; i < nParticles.x; i++)
            {
                for (int j = 0; j < nParticles.y; j++)
                {
                    for (int k = 0; k < nParticles.z; k++)
                    {
                        p.x = dx * i;
                        p.y = dy * j;
                        p.z = dz * k;
                        p.w = 1.0f;
                        p = transf * p;
                        arrPos.Add(p.x);
                        arrPos.Add(p.y);
                        arrPos.Add(p.z);
                        arrPos.Add(p.w);
                    }
                }
            }
            // We need buffers for position , and velocity.
            uint[] bufs = new uint[2];
            Gl.GenBuffers(bufs);
            uint posBuf = bufs[0];
            uint velBuf = bufs[1];
            
            uint bufSize = totalParticles * 4 * sizeof(float);
            float[] initPos = (float[]) arrPos.ToArray(typeof(float));
            // The buffers for positions
            Gl.BindBufferBase(BufferTarget.ShaderStorageBuffer, 0, posBuf);
            Gl.BufferData(BufferTarget.ShaderStorageBuffer, bufSize,initPos, BufferUsage.DynamicDraw);
            // Velocities
            Gl.BindBufferBase(BufferTarget.ShaderStorageBuffer, 1, velBuf);
            Gl.BufferData(BufferTarget.ShaderStorageBuffer, bufSize, initVel, BufferUsage.DynamicDraw);

            uint[] buf = new uint[1];
            
            // Set up the VAO
            Gl.GenVertexArrays(buf);
            particlesVao = buf[0];
            Gl.BindVertexArray(particlesVao);

            Gl.BindBuffer(BufferTarget.ArrayBuffer, posBuf);
            Gl.VertexAttribPointer(0, 4, VertexAttribType.Float, false, 0, IntPtr.Zero);
            Gl.EnableVertexAttribArray(0);

            Gl.BindVertexArray(0);
            
            // Set up a buffer and a VAO for drawing the attractors (the "black holes")
            Gl.GenBuffers(buf);
            bhBuf = buf[0];
            Gl.BindBuffer(BufferTarget.ArrayBuffer, bhBuf);
            float[] data = { bh1.x, bh1.y, bh1.z, bh1.w, bh2.x, bh2.y, bh2.z, bh2.w };
            Gl.BufferData(BufferTarget.ArrayBuffer, 8 * sizeof(float), data,BufferUsage.DynamicDraw);

            Gl.GenVertexArrays(buf);
            bhVao = buf[0];
            Gl.BindVertexArray(bhVao);

            Gl.BindBuffer(BufferTarget.ArrayBuffer, bhBuf);
            Gl.VertexAttribPointer(0, 4, VertexAttribType.Float, false, 0, IntPtr.Zero);
            Gl.EnableVertexAttribArray(0);

            Gl.BindVertexArray(0);
        }


        void SetMatrices()
        {
            renderProg.Use();
            mat4 mv = view * model;
            mat3 norm = new mat3(new vec3(mv[0]), new vec3(mv[1]), new vec3(mv[2]));

            renderProg.SetMat4("ModelViewMatrix", mv);
            renderProg.SetMat3("NormalMatrix", norm);
            renderProg.SetMat4("MVP", projection * mv);
        }

        public SceneParticles() 
        {
            width = 800;
            height = 600;
            nParticles = new ivec3(100, 100, 100);
            time = 0.0f;
            deltaT = 0.0f;
            speed = 35.0f;
            angle = 0.0f;
            bh1 = new vec4(5, 0, 0, 1);
            bh2 = new vec4(-5, 0, 0, 1);
            totalParticles = (uint)(nParticles.x * nParticles.y * nParticles.z);
        }

        public override void InitScene()
        {
            CompileAndLinkShader();
            InitBuffers();

            Gl.ClearColor(1, 1, 1, 1);

            projection = mat4.Perspective(glm.Radians(50.0f), (float)width / height, 1.0f, 100.0f);

            Gl.Enable(EnableCap.Blend);
            Gl.BlendFunc(BlendingFactor.SrcAlpha,BlendingFactor.OneMinusSrcAlpha);

        }

        public override void Update(float t)
        {
            if (time == 0.0f)
            {
                deltaT = 0.0f;
            }
            else
            {
                deltaT = t - time;
            }
            time = t;
            if (Animate)
            {
                angle += speed * deltaT;
                if (angle > 360.0f)
                {
                    angle -= 360.0f;
                }
            }
        }

        public override void Render()
        {
            // Rotate the attractors ("black holes")
            //mat4 rot = new mat4(1.0f);
            mat4 rot = mat4.Identity;
            rot *= mat4.Rotate(glm.Radians(angle), new vec3(0, 0, 1));
            vec3 att1 = new vec3(rot * bh1);
            vec3 att2 = new vec3(rot * bh2);

            // Execute the compute shader
            computeProg.Use();
            computeProg.SetVec3("BlackHolePos1", att1);
            computeProg.SetVec3("BlackHolePos2", att2);
            Gl.DispatchCompute(totalParticles / 1000, 1, 1);
            Gl.MemoryBarrier(MemoryBarrierMask.ShaderStorageBarrierBit);

            // Draw the scene
            renderProg.Use();
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            view = mat4.LookAt(new vec3(2, 0, 20),new vec3(0, 0, 0), new vec3(0, 1, 0));
            model = mat4.Identity;
            SetMatrices();

            // Draw the particles
            Gl.PointSize(1.0f);
            renderProg.SetVec4("Color", new vec4(0, 0, 0, 0.2f));
            Gl.BindVertexArray(particlesVao);
            Gl.DrawArrays(Gl.POINTS, 0, (int)totalParticles);
            Gl.BindVertexArray(0);

            // Draw the attractors
            Gl.PointSize(5.0f);
            float[] data = { att1.x, att1.y, att1.z, 1.0f, att2.x, att2.y, att2.z, 1.0f };
            Gl.BindBuffer(BufferTarget.ArrayBuffer, bhBuf);
            //////////////////////////Marshaling///////////////////////////////////////////
            IntPtr p = Marshal.AllocHGlobal(data.Length * sizeof(float));
            Marshal.Copy(data, 0, p, data.Length);
            Gl.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, 8 * sizeof(float), p);
            Marshal.FreeHGlobal(p);
            //////////////////////////////////////////////////////////////////////////////
            
            renderProg.SetVec4("Color", new vec4(1, 1, 0, 1.0f));
            Gl.BindVertexArray(bhVao);
            Gl.DrawArrays(PrimitiveType.Points, 0, 2);
            Gl.BindVertexArray(0);
        }

        public override void Resize(int w, int h)
        {
            Gl.Viewport(0, 0, w, h);
            width = w;
            height = h;
        }
    }
}
