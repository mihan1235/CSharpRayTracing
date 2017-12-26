using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmSharp;

namespace Chapter_1_OutputAnImage_GPU
{
    interface IShader
    {
        void Use();
        void SetMat4(String name, mat4 obj);
        void SetFloat(String name, float value);
        void SetInt(String name, int value);
        void SetBool(String name, bool value);
        void SetVec3(String name, vec3 value);
        void SetVec4(String name, vec4 value);
        void SetMat3(String name, mat3 value);
    }
}
