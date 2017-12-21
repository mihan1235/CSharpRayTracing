using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutputAnImage
{
    class Program
    {
        static void Main(string[] args)
        {
            int nx = 1280;
            int ny = 760;
            Console.WriteLine("P3\n{0} {1}\n255",nx,ny);
            for(int j = ny - 1; j >= 0; j--)
            {
                for(int i = 0; i < nx; i++)
                {
                    float r = (float)i / nx;
                    float g = (float)j / ny;
                    float b = 0.2f;
                    int ir = (int)(255 * r);
                    int ig = (int)(255 * g);
                    int ib = (int)(255 * b);
                    Console.WriteLine("{0} {1} {2}",ir,ig,ib);
                }
            }
        }
    }
}
