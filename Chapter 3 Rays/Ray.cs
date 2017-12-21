using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmSharp;

namespace Chapter_3_Rays
{
    //p(t)=A+tB - ray, where A is origin, B is direction, p is position in 3D space
    class Ray
    {
        public Ray(vec3 A, vec3 B)
        {
            this.A = A;
            this.B = B;
        }
        public vec3 Origin{
            get
            {
                return A;
            }
        }
        public vec3 Direction
        {
            get
            {
                return B;
            }
        }
        public vec3 Position(float t)
        {
            return A + t * B;
        }
        vec3 A;
        vec3 B;
    }
}
