using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmSharp;

namespace Chapter_3_Rays
{
    class Program
    {
        static vec3 Color(Ray r)
        {
            vec3 unit_direction = r.Direction;
            float t = 0.5f * (unit_direction.y + 1.0f);
            return (1.0f - t) * new vec3(1.0f, 1.0f, 1.0f) + t * new vec3(0.5f,0.7f,1.0f);
        }

        static void Main(string[] args)
        {
            int nx = 1280;
            int ny = 760;
            Console.WriteLine("P3\n{0} {1}\n255", nx, ny);
            //Camera
            vec3 lower_left_corner = new vec3(-2.0f,-1.0f,-1.0f);
            vec3 horizontal = new vec3(4.0f,0.0f,0.0f);
            vec3 origin = new vec3(0.0f,0.0f,0.0f);
            vec3 vertical = new vec3(0.0f,2.0f,0.0f);
            for (int j = ny - 1; j >= 0; j--)
            {
                for (int i = 0; i < nx; i++)
                {
                    float u = (float)i / nx;
                    float v = (float)j / ny;
                    vec3 color = Color(new Ray(origin,lower_left_corner+ u *horizontal+v*vertical));
                    int ir = (int)(255 * color[0]);
                    int ig = (int)(255 * color[1]);
                    int ib = (int)(255 * color[2]);
                    Console.WriteLine("{0} {1} {2}", ir, ig, ib);
                }
            }
        }
    }
}
