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
            body.CreateBody();
            AddBody(body);
        }

        public void AddWorld(ContentManager _content)
        {
            Models world = new(_content, "obj/World", "obj/WorldDiffuse", "MyShader", Vector3.Zero, 1.0f);

            Models parent = new();
            world.SetShader(_content.Load<Effect>("MyShader"));

            int c = m_bodies.Count;

            for (int a = 0; a < c; a++)
            { //if we wanna make it impossible to make a world body without a sun. Completely optional.
                if (m_bodies[a].BodyType == 0)
                {
                    parent = m_bodies[a].CelestialBodyModel;

                    CelestialBody body = new CelestialBody(world, 1, parent);
                    body.CreateBody();
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
                Models moon = new(_content, "obj/moon", "obj/MoonDiffuse", "MyShader", Vector3.Zero, 1.0f);
                moon.SetShader(_content.Load<Effect>("MyShader"));

                parent = m_bodies[indices[i]].CelestialBodyModel;

                CelestialBody body = new CelestialBody(moon, 2, parent);
                body.CreateBody();
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
            foreach (CelestialBody c in m_bodies)
            {
                c.Render(m_camera.View, m_camera.Projection);
            }
        }

        public void Serialize(BinaryWriter _stream)
        {
            _stream.Write(m_bodies.Count);

            //pass 1: serializing body data and parent index
            for (int i = 0; i < m_bodies.Count; i++)
            {
                var body = m_bodies[i];

                //before serializing, finding the parent's index if one exists.
                int parentIndex = -1;
                if (body.ParentBodyModel != null)
                {
                    //finding the index of the parent in the m_bodies list.
                    //this is the core of the hierarchy serialization logic.
                    for (int p = 0; p < m_bodies.Count; p++)
                    {
                        //looking up the parent object by its Model reference.
                        if (m_bodies[p].CelestialBodyModel == body.ParentBodyModel)
                        {
                            parentIndex = p;
                            break;
                        }
                    }
                }

                //saving the correct Rotation state before corruption.
                Vector3 originalRotation = body.CelestialBodyModel.Rotation;

                //temporarily storing the parent index in a property the body serializes.
                //the CelestialBody.Serialize method will read this and write it to the stream.
                body.CelestialBodyModel.Rotation = new Vector3(
                    (float)parentIndex, //parent Index is cast to float and stored in Rotation.X
                    body.CelestialBodyModel.Rotation.Y,
                    body.CelestialBodyModel.Rotation.Z
                );

                //serializing the body (which now writes the parent index)
                body.Serialize(_stream);

                //restoring the correct Rotation state immediately.
                body.CelestialBodyModel.Rotation = originalRotation;

                //explicitly writing the correct Rotation vector to the stream.
                //this data will be used in Deserialize to correct the model's rotation.
                _stream.Write(originalRotation.X);
                _stream.Write(originalRotation.Y);
                _stream.Write(originalRotation.Z);
            }

            m_camera.Serialize(_stream);
        }

        public void Deserialize(BinaryReader _stream, ContentManager _content)
        {
            m_bodies.Clear(); //clearing any existing bodies
            List<int> parentIndices = new(); //temporary list to store parent indices

            int bodyCount = _stream.ReadInt32();

            //PASS 1: Loading all bodies and capturing parent indices
            for (int i = 0; i < bodyCount; i++)
            {
                CelestialBody c = new();

                //the modified Deserialize returns the parent index, which we capture.
                //NOTE: c.CelestialBodyModel.Rotation.X is now corrupted (contains the index float).
                int parentIndex = c.Deserialize(_stream, _content);

                //reading the correct rotation vector from the stream.**
                Vector3 correctRotation = new Vector3(
                    _stream.ReadSingle(),
                    _stream.ReadSingle(),
                    _stream.ReadSingle()
                );

                //overwriting the corrupted rotation with the correct, saved vector.
                c.CelestialBodyModel.Rotation = correctRotation;

                m_bodies.Add(c);
                parentIndices.Add(parentIndex);
            }

            //PASS 2: reconnecting the hierarchy
            for (int i = 0; i < m_bodies.Count; i++)
            {
                int parentIndex = parentIndices[i];

                if (parentIndex != -1)
                {
                    //the body's ParentBodyModel must point to the live CelestialBodyModel 
                    //property of the loaded parent object.
                    m_bodies[i].ParentBodyModel = m_bodies[parentIndex].CelestialBodyModel;
                }
            }

            m_camera.Deserialize(_stream, _content);
        }
    }
}
