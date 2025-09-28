using Editor.Engine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Editor.Engine
{
    internal class Level : ISerializable
    {
        // Accessors
        public Camera GetCamera() { return m_camera; }

        // Members
        private List<Models> m_models = new();
        private List<CelestialBody> m_bodies = new();


        private Camera m_camera = new(new Vector3(0, 2, 2), 16.0f / 9.0f); //my resolution is 16x10

        public void AddSun(ContentManager _content)
        {
            for (int i = 0; i < m_bodies.Count; i++)
            {
                if (m_bodies[i].BodyType == 0)
                    return; //sun already exists, exit the method
            }

            //if the loop finishes without returning, create and add the new Sun
            Models sun = new(_content, "obj/Sun", "obj/SunDiffuse", "MyShader", Vector3.Zero, 1.0f);
            sun.SetShader(_content.Load<Effect>("MyShader"));

            CelestialBody body = new CelestialBody(sun, 0);

            AddBody(body);
        }
        //int i = 0;

        //List<int> indices = new List<int>();

        //for (int a = 0; a < m_bodies.Count; a++) 
        //{
        //    if (m_bodies[a].BodyType == 1)
        //    {
        //        i++;
        //        indices.Add(a);
        //    }
        //}

        //i = m_rand.Next(i);
        //m_bodies[indicies[i]];
        public void AddWorld(ContentManager _content)
        {
            Models world = new(_content, "obj/World", "obj/WorldDiffuse", "MyShader", Vector3.Zero, 1.0f);
            
            Models parent = new();
            world.SetShader(_content.Load<Effect>("MyShader"));

            int c = m_bodies.Count;

            for (int a = 0; a < c; a++) { //if we wanna make it impossible to make a world body without a sun. Completely optional.
                if (m_bodies[a].BodyType == 0)
                {
                    parent = m_bodies[a].CelestialBodyModel;

                    CelestialBody body = new CelestialBody(world, 1, parent);

                    AddBody(body);
                    return;
                }
            }
            //had to give it this name because the compiler is stupid.
            CelestialBody bod2 = new CelestialBody(world, 1);
            AddBody(bod2);
        }

        public void AddMoon(ContentManager _content)
        {
            Models moon = new(_content, "obj/moon", "obj/MoonDiffuse", "MyShader", Vector3.Zero, 1.0f);
            moon.SetShader(_content.Load<Effect>("MyShader"));
            Models parent = new();

            int c = m_bodies.Count;

            List<int> indices = new List<int>();

            for (int a = 0; a < c; a++)
            {
                if (m_bodies[a].BodyType == 1)
                {
                    indices.Add(a);
                }
            }

            for (int i = 0; i < indices.Count; i++)
            {
                parent = m_bodies[indices[i]].CelestialBodyModel;

                CelestialBody body = new CelestialBody(moon, 2, parent);

                AddBody(body);
                
            }
        }

        public Level()
        {
        }

        public void LoadContent(ContentManager _content)
        {
            //Models sun = new(_content, "obj/Sphere", "obj/worlddiffuse", "MyShader", Vector3.Zero, 2.0f);
            //sun.SetShader(_content.Load<Effect>("MyShader"));

            //CelestialBody body;

            //for (int a = 0; a < m_bodies.Count; a++)
            //{ //if we wanna make it impossible to make a world body without a sun. Completely optional.
            //    if (m_bodies[a].BodyType == 1)
            //    {
            //        body = new CelestialBody(sun, 2, m_bodies[a].CelestialBodyModel);

            //        AddBody(body);
            //    }
            //}
            //AddModel(sun);
        }

        public void AddBody(CelestialBody _body)
        {
            m_bodies.Add(_body);
        }
        public void AddModel(Models _model)
        {
            m_models.Add(_model);
        }

        public void Render()
        {
            //foreach (Models m in m_models)
            //{
            //    m.Render(m_camera.View, m_camera.Projection);
            //}

            foreach (CelestialBody c in m_bodies)
            {
                c.Render(m_camera.View, m_camera.Projection);
            }
        }

        public void Serialize(BinaryWriter _stream)
        {
            //_stream.Write(m_models.Count);
            //foreach (var model in m_models)
            //{
            //    model.Serialize(_stream);
            //}

            _stream.Write(m_bodies.Count);
            foreach (var body in m_bodies)
            {
                body.Serialize(_stream);
            }

            m_camera.Serialize(_stream);
        }

        public void Deserialize(BinaryReader _stream, ContentManager _content)
        {
            //int modelCount = _stream.ReadInt32();
            //for (int count = 0; count < modelCount; count++) 
            //{
            //    Models m = new();
            //    m.Deserialize(_stream, _content);
            //    m_models.Add(m);
            //}

            int bodyCount = _stream.ReadInt32();
            for (int count = 0; count < bodyCount; count++)
            {
                CelestialBody c = new();
                c.Deserialize(_stream, _content);
                m_bodies.Add(c);
            }
            m_camera.Deserialize(_stream, _content);

        }
    }
}
