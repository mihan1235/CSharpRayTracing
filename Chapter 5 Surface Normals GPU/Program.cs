using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapter_5_Surface_Normals
{
    using glfw3;
    using static glfw3.glfw3;
    using static glfw3.KeyMacros;
    using static glfw3.State;
    using OpenGL;
    using System.Runtime.InteropServices;

    class Program
    {
        static int width = 1600;
        static int hight = 900;
        static Scene scene = new SceneRayTracing(width,hight);

        static int Main(string[] args)
        {

            GlfwInit();
            GlfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 4);
            GlfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
            GlfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
            GlfwWindowHint(GLFW_RESIZABLE, GLFW_FALSE);

            Gl.Initialize();
            GLFWwindow window = GlfwCreateWindow(width, hight, "LearnOpenGL", null, null);
            if (window == null)
            {
                Console.WriteLine("Failed to create GLFW window");
                GlfwTerminate();
                return -1;
            }
            GlfwMakeContextCurrent(window);
            GlfwSetKeyCallback(window, key_callback);
            GLFWframebuffersizefun a = new GLFWframebuffersizefun(framebuffer_size_callback);
            GlfwSetFramebufferSizeCallback(window, a);
            Gl.ClearColor(0.5f, 0.0f, 0.0f, 1.0f);
            scene.InitScene();
            scene.Resize(width, hight);

            const int samples = 50;
            float[] time = new float[samples];
            int index = 0;

            while ((GlfwWindowShouldClose(window) != GLFW_TRUE) && (GlfwGetKey(window, GLFW_KEY_ESCAPE) != GLFW_TRUE))
            {
                scene.Update((float)GlfwGetTime());
                scene.Render();
                //Gl.ClearColor(0.5f, 0.0f, 0.0f, 1.0f);
               // Gl.Clear(ClearBufferMask.ColorBufferBit);
                GlfwSwapBuffers(window);
                GlfwPollEvents();

                // Update FPS
                time[index] = (float)GlfwGetTime();
                index = (index + 1) % samples;

                if (index == 0)
                {
                    float sum = 0.0f;
                    for (int i = 0; i < samples - 1; i++)
                        sum += time[i + 1] - time[i];
                    float fps = samples / sum;

                    String strm = "Chapter 10 -- " + " (fps: " + fps + ")";
                    GlfwSetWindowTitle(window, strm);
                }
            }

            // glfw: terminate, clearing all previously allocated GLFW resources.
            // ------------------------------------------------------------------
            GlfwTerminate();
            return 0;
        }

        // process all input: query GLFW whether relevant keys are pressed/released this frame and react accordingly
        // ---------------------------------------------------------------------------------------------------------
        static void processInput(GLFWwindow window)
        {
            if (GlfwGetKey(window, GLFW_KEY_ESCAPE) == GLFW_PRESS)
                GlfwSetWindowShouldClose(window, GLFW_TRUE);
        }

        // glfw: whenever the window size changed (by OS or user resize) this callback function executes
        // ---------------------------------------------------------------------------------------------
        static void framebuffer_size_callback(IntPtr window, int width, int height)
        {
            // make sure the viewport matches the new window dimensions; note that width and 
            // height will be significantly larger than specified on retina displays.
            //glViewport(0, 0, width, height);
            Gl.Viewport(0, 0, width, height);
            //Console.WriteLine("glViewport(0, 0, {0}, {1});",width,hight);
        }

        static void key_callback(IntPtr window, int key, int scancode, int action, int mods)
        {

        }
    }
}
