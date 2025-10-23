using Editor.Engine.Interfaces;
using Editor.Engine.Lights;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using Editor.Editor;
using System.Windows.Forms;

namespace Editor.Engine
{
    internal class Level : ISerializable
    { //beginning lab 4
        // Accessors
        public Camera GetCamera() { return m_camera; }

        // Members
        private List<Models> m_models = new();
        private Camera m_camera = new(new Vector3(0.1f, 354.0f, 541.0f), 16.0f / 9.0f); //my resolution is 16x10
        private Light m_light = new()
        {
            Position = new(0, 400, -500),
            Color = new(0.9f, 0.9f, 0.9f)
        };
        private Effect m_terrainEffect = null;
        private Terrain m_terrain = null;

        public Level()
        {
        }

        public void LoadContent(GameEditor _game)
        {
            //m_terrain = new(_game.DefaultEffect, _game.DefaultHeightMap,
            //                _game.DefaultGrass, 200, _game.GraphicsDevice);
        }

        public void LoadContent(GraphicsDevice _device, ContentManager _content)
        {
            //m_terrainEffect = _content.Load<Effect>("TerrainEffect");
            //m_terrain = new(_content.Load<Texture2D>("JohnFrancisHeightMap"), _content.Load<Texture2D>("zuck"), 200, _device);
        }

        public void AddModel(Models _model)
        {
            m_models.Add(_model);
        }

        public void ClearSelectedModels()
        {
            foreach(var model in m_models)
            {
                model.Selected = false;
            }
            if(m_terrain != null)
            {
                m_terrain.Selected = false;
            }
        }

        public List<ISelectable> GetSelectedModels()
        {
            List<ISelectable> models = new List<ISelectable>();
            foreach(var model in m_models)
            {
                if (model.Selected) models.Add(model);
            }
            if(m_terrain != null)
            {
                if (m_terrain.Selected) models.Add(m_terrain);
            }

            return models;
        }
        public void Render()
        {
            Renderer r = Renderer.Instance;
            r.Camera = m_camera;
            r.Light = m_light;
            foreach (Models m in m_models)
            {
                r.Render(m);
            }
            if (m_terrain != null)
            {
                r.Render(m_terrain);
                //m_terrain.Draw(m_terrainEffect, m_camera.View, m_camera.Projection);
            }
        }

        private void HandleTranslate()
        {
            InputController ic = InputController.Instance;
            Vector3 translate = Vector3.Zero;
            if (ic.IsKeyDown(Keys.Left)) translate.X += -10;
            if (ic.IsKeyDown(Keys.Right)) translate.X += 10;
            if (ic.IsKeyDown(Keys.Menu))
            {
                if (ic.IsKeyDown(Keys.Up)) translate.Z += 1;
                if (ic.IsKeyDown(Keys.Down)) translate.Z += -1;
            }
            else
            {
                if (ic.IsKeyDown(Keys.Up)) translate.Y += 10;
                if (ic.IsKeyDown(Keys.Down)) translate.Y += -10;
            }
            if (ic.IsButtonDown(MouseButtons.Middle))
            {
                Vector2 dir = ic.MousePosition - ic.LastPosition;
                translate.X = dir.X;
                translate.Y = -dir.Y;
            }
            if (ic.GetWheel() != 0)
            {
                translate.Z = ic.GetWheel() * 2;
            }

            if (translate != Vector3.Zero)
            {
                bool modelTranslated = false;
                foreach(Models model in m_models)
                {
                    if (model.Selected)
                    {
                        modelTranslated = true;
                        model.Translate(translate / 1000, m_camera);
                    }
                }
                if (!modelTranslated)
                {
                    m_camera.Translate(translate * 0.001f);
                }
            }
        }

        private void HandleRotate(float _delta)
        {
            InputController ic = InputController.Instance;
            if (ic.IsButtonDown(MouseButtons.Right) && (!ic.IsKeyDown(Keys.Menu)))
            {
                Vector2 dir = ic.MousePosition - ic.LastPosition;
                if (dir != Vector2.Zero)
                {
                    Vector3 movement = new Vector3(dir.Y, dir.X, 0) * _delta;
                    bool modelRotated = false;
                    foreach (Models model in m_models)
                    {
                        if (model.Selected)
                        {
                            modelRotated = true;
                            model.Rotation += movement;
                        }
                        if (!modelRotated)
                        {
                            m_camera.Rotate(movement);
                        }
                    }
                }
            }
        }

        private void HandleScale(float _delta)
        {
            InputController ic = InputController.Instance;
            if((ic.IsButtonDown(MouseButtons.Right)) && (ic.IsKeyDown(Keys.Menu)))
            {
                float l = ic.MousePosition.Y - ic.LastPosition.Y;
                if(l != 0)
                {
                    l *= _delta;
                    foreach(Models model in m_models)
                    {
                        if (model.Selected)
                        {
                            model.Scale += l;
                        }
                    }
                }
            }
        }

        internal ISelectable HandlePick(bool _select = true) {
            float? f;
            Matrix transform = Matrix.Identity;
            InputController ic = InputController.Instance;
            if ((ic.IsButtonDown(MouseButtons.Left)) || (!_select))
            {
                Ray r = HelpMath.GetPickRay(ic.MousePosition, m_camera);
                foreach(Models model in m_models)
                {
                    if (_select) model.Selected = false;
                    transform = model.GetTransform();
                    foreach(ModelMesh mesh in model.Mesh.Meshes)
                    {
                        BoundingSphere s = mesh.BoundingSphere;
                        s.Transform(ref transform, out s);
                        f = r.Intersects(s);
                        if (f.HasValue)
                        {
                            f = HelpMath.PickTriangle(in mesh, ref r, ref transform);
                            if (f.HasValue)
                            {
                                if (!_select) return model;
                                model.Selected = true;
                            }
                        }
                    }
                }

                // Check Terrain
                if (m_terrain != null)
                {
                    transform = Matrix.Identity;
                    f = HelpMath.PickTriangle(in m_terrain, ref r, ref transform);

                    m_terrain.Selected = false;
                    if (f.HasValue)
                    {
                        if (!_select) return m_terrain;
                        m_terrain.Selected = true;
                    }
                }
            }

            return null;
        }

        private void HandleAudio()
        {
            foreach(Models m in m_models)
            {
                if((Models.SelectedDirty) &&
                    m.Selected)
                {
                    var sfi = m.SoundEffects[(int)SoundEffectTypes.OnSelect];
                    if(sfi?.Instance.State == SoundState.Stopped)
                    {
                        sfi.Instance.Play();
                    }
                }
            }
        }
        public void Update(float _delta)
        {
            HandleTranslate();
            HandleRotate(_delta);
            HandleScale(_delta);
            HandlePick();
            HandleAudio();
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

        public void Deserialize(BinaryReader _stream, GameEditor _game)
        {
            //m_models.Clear(); //new. May need to remove.

            int modelCount = _stream.ReadInt32();
            for (int count = 0; count < modelCount; count++) 
            {
                Models m = new();
                m.Deserialize(_stream, _game);
                m_models.Add(m);
            }
            m_camera.Deserialize(_stream, _game);
        }

        public override string ToString()
        {
            string s = string.Empty;
            foreach(Models m in m_models)
            {
                if (m.Selected)
                {
                    s += "\nModel: Pos: " + m.Position.ToString() +
                        " Rot: " + m.Rotation.ToString();
                }
            }
            return m_camera.ToString() + s;
        }
    }
}
