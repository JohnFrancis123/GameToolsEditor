using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Windows.Forms;
using Editor;
using GUI.Editor;
using System;
using System.ComponentModel;
using Editor.Engine;
using System.Collections.Generic;

namespace Editor.Editor
{
    public class GameEditor : Game
    {
        internal Project Project { get; set; }

        private GraphicsDeviceManager m_graphics;
        private FormEditor m_parent;
        private SpriteBatch m_spriteBatch;
        private FontController m_fonts;
        //private VertexBuffer m_vertexBuffer;

        private List<Models> m_selected; 

        RasterizerState m_rasterState = new RasterizerState();
        DepthStencilState m_depthStencilState = new DepthStencilState();

        private bool m_dirty;

        public GameEditor()
        {
            m_selected = new();

            m_graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            m_rasterState = new RasterizerState();
            m_rasterState.CullMode = CullMode.None;
            m_depthStencilState = new DepthStencilState();
            m_depthStencilState.DepthBufferEnable = true;

            m_dirty = false;
        }

        public GameEditor(FormEditor _parent) : this()
        {
            m_parent = _parent;
            Form gameForm = Control.FromHandle(Window.Handle) as Form;
            gameForm.TopLevel = false;
            gameForm.Dock = DockStyle.Fill;
            gameForm.FormBorderStyle = FormBorderStyle.None;
            m_parent.splitContainer.Panel1.Controls.Add(gameForm);
        }

        protected override void Initialize()
        {
            //RasterizerState state = new RasterizerState();
            //state.CullMode = CullMode.None;
            //GraphicsDevice.RasterizerState = state;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            m_spriteBatch = new SpriteBatch(GraphicsDevice);
            m_fonts = new();
            m_fonts.LoadContent(Content);
        }

        private void UpdSelectionAndModels(List<Models> selection)
        {
            //unsubscribing from the models
            foreach (Models model in m_selected) 
            {
                model.PropertyChanged -= OnPropertyChanged;
            }

            if (selection.Count == 0)
            {
                m_parent.propertyGrid.SelectedObject = null;
                m_dirty = true;
            }
            else if (selection.Count > 1 && m_dirty) 
            {
                m_parent.propertyGrid.SelectedObjects = selection.ToArray();
                m_dirty = false;
            }
            else if (selection.Count == 1 && m_dirty)
            {
                m_parent.propertyGrid.SelectedObject = selection[0];
                m_dirty = false;
            }

            foreach(var model in m_selected)
            {
                model.PropertyChanged += OnPropertyChanged;
            }

            m_selected = selection;

        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (m_parent.propertyGrid.InvokeRequired)
            {
                m_parent.propertyGrid.Invoke(new Action<object, PropertyChangedEventArgs>(OnPropertyChanged), sender, e);
                return;
            }

            if(sender is Models)
            {
                if(e.PropertyName == "Appearance")
                {
                    for (int i = 0; i < m_selected.Count; i++)
                    {
                        m_selected[i].UpdateTex(Content);
                    }
                }
                //if a property is changed externally to the property grid, it's dirty.
                if (e.PropertyName != "Selected")
                {
                    m_dirty = true;
                }
            }
        }

        //can be called on the FormEditor thread
        public void HandleTexChange(Models model)
        {
            if (m_parent.propertyGrid.InvokeRequired)
            {
                m_parent.propertyGrid.Invoke(new Action<Models>(HandleTexChange), model);
                return;
            }
            model.UpdateTex(Content);
            m_dirty = true;
        }


        //setting the selected property of the property grid every tick
        protected override void Update(GameTime _gameTime)
        {
            if (Project != null)
            {
                Project.Update((float)(_gameTime.ElapsedGameTime.TotalMilliseconds / 1000));
                InputController.Instance.Clear();
                //
                //if(!m_selected.SequenceEqual(models))
                var models = Project.CurrentLevel.GetSelectedModels();

                UpdSelectionAndModels(models);
                
            }

            base.Update(_gameTime);
        } //

        private void HandleSelections()
        {

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (Project != null)
            {
                GraphicsDevice.RasterizerState = m_rasterState;
                GraphicsDevice.DepthStencilState = m_depthStencilState;
                Project.Render();
                m_spriteBatch.Begin();
                m_fonts.Draw(m_spriteBatch, 20, InputController.Instance.ToString(), new Vector2(20, 20), Color.White);
                m_fonts.Draw(m_spriteBatch, 16, Project.CurrentLevel.ToString(), new Vector2(20, 80), Color.Yellow);
                m_spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        public void AdjustAspectRatio()
        {
            if (Project == null) return;
            Camera c = Project.CurrentLevel.GetCamera();
            c.Viewport = m_graphics.GraphicsDevice.Viewport;
            c.Update(c.Position, m_graphics.GraphicsDevice.Viewport.AspectRatio);
        }
    }
}
