using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapter_5_Multiple_Objects_GPU
{
    abstract class Scene
    {
        
        /**
          Load textures, initialize shaders, etc.
          */
        public abstract void InitScene();

        /**
        This is called prior to every frame.  Use this
        to update your animation.
        */
        public abstract void Update(float t);

        /**
         Draw your scene.
         */
        public abstract void Render();
        /**
        Called when screen is resized
        */
        public abstract void Resize(int w, int h);
    }
}
