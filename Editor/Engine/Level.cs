using Editor.Engine.Interfaces;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Editor.Engine
{
    internal class Level : ISerializable
    { //beginning lab 4
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
            Models teapot = new(_content, "obj/Teapot", "Metal", "MyShader", Vector3.Zero, 1.0f);
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

        public void Serialize(BinaryWriter _stream)
        {
            _stream.Write(m_models.Count);
            foreach (var model in m_models)
            {
                model.Serialize(_stream);
            }
            m_camera.Serialize(_stream);
        }

        public void Deserialize(BinaryReader _stream, ContentManager _content)
        {
            int modelCount = _stream.ReadInt32();
            for (int count = 0; count < modelCount; count++) 
            {
                Models m = new();
                m.Deserialize(_stream, _content);
                m_models.Add(m);
            }
            m_camera.Deserialize(_stream, _content);
        }
    }
}
