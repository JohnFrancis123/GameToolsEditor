using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Engine
{
    internal class Material
    {
        public Material() { }

        public Texture Diffuse { get; set; }
        public Effect Effect { get; set; }
    }
}
