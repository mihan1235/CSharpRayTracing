using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmSharp;

namespace Vec3
{
    class Program
    {
        static void Main(string[] args)
        {
            int nx = 1280;
            int ny = 760;
            Console.WriteLine("P3\n{0} {1}\n255", nx, ny);
            for (int j = ny - 1; j >= 0; j--)
            {
                for (int i = 0; i < nx; i++)
                {
                    vec3 color = new vec3((float)i / nx, (float)j / ny, 0.2f);
                    int ir = (int)(255 * color[0]);
                    int ig = (int)(255 * color[1]);
                    int ib = (int)(255 * color[2]);
                    Console.WriteLine("{0} {1} {2}", ir, ig, ib);
                }
            }
        }
    }
}
