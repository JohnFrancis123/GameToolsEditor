using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Editor
{
    internal class Level
    {
        // Accessors
        public Camera GetCamera() { return m_camera; }

        // Members
        private List<Models> m_models = new();
        private Camera m_camera = new(new Vector3(0, 2, 2), 16.0f / 9.0f); //my resolution is 16x10

        public Level()
        {
        }

        public void LoadContent(ContentManager _content)
        {
            Models teapot = new(_content.Load<Model>("obj/Teapot"), _content.Load<Texture>("Metal"), Vector3.Zero, 1.0f);
            teapot.SetShader(_content.Load<Effect>("MyShader"));
            AddModel(teapot);
        }

        public void AddModel(Models _model)
        {
            m_models.Add(_model);
        }

        public void Render()
        {
            foreach (Models m in m_models)
            {
                m.Render(m_camera.View, m_camera.Projection);
            }
        }

    }
}
