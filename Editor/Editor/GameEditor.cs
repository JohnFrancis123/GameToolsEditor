using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Windows.Forms;
using Editor;
using GUI.Editor;
using System;
using System.ComponentModel;
using Editor.Engine;
using System.Linq;
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

        RasterizerState m_rasterState = new RasterizerState();
        DepthStencilState m_depthStencilState = new DepthStencilState();

        private bool m_dirty;

        private Models[] m_selected = Array.Empty<Models>();

        private void UpdateSelectionAndModels(Models[] selection)
        {
            // --- 1. Unsubscribe from the OLD selection ---
            foreach (var model in m_selected)
            {
                // CORRECT: Remove handler from old selection
                model.PropertyChanged -= OnModelPropertyChanged;
            }

            // --- 2. Update the PropertyGrid selection (LOGIC FIX) ---
            if (selection == null || selection.Length == 0)
            {
                // Case: No selection
                m_parent.propertyGrid.SelectedObject = null;
            }
            else if (selection.Length == 1)
            {
                // Case: Single selection
                m_parent.propertyGrid.SelectedObject = selection[0];
            }
            else // selection.Length > 1
            {
                // Case: Multiple selection (Requires SelectedObjects and an object array)
                // You MUST use SelectedObjects for multiple items.
                m_parent.propertyGrid.SelectedObjects = selection.Cast<object>().ToArray();
            }

            // --- 3. Subscribe to the NEW selection (SUBSCRIPTION FIX) ---
            foreach (var model in selection)
            {
                // FIX: You must ADD the handler to the new selection.
                model.PropertyChanged += OnModelPropertyChanged;
            }

            // --- 4. Store the new selection for the next cycle ---
            // Note: Do this *after* all event handling logic is complete.
            m_selected = selection;
        }

        private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //checking if on the wrong thread (MonoGame thread).
            if (m_parent.propertyGrid.InvokeRequired)
            {
                //calling back to the UI thread.
                m_parent.propertyGrid.Invoke(new Action<object, PropertyChangedEventArgs>(OnModelPropertyChanged),
                                             sender,
                                             e);
                return;
            }

            //execution continues here ONLY on the safe UI thread

            //setting the dirty flag (original logic)
            if (e.PropertyName != "Selected" && sender is Models)
            {
                m_dirty = true;
            }

            //forcing the Property Grid to update its display.
            //the Property Grid *should* update automatically, but if it fails, 
            //explicitly telling it to Refresh() is the definitive fix.
            //we only need to refresh if a property *other* than 'Selected' changed.
            if (e.PropertyName != "Selected")
            {
                m_parent.propertyGrid.Refresh();
            }
        }

        public GameEditor()
        {
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
            m_spriteBatch = new SpriteBatch(GraphicsDevice); ;
            m_fonts = new();
            m_fonts.LoadContent(Content);
        }

        //setting the selected property of the property grid every tick
        protected override void Update(GameTime _gameTime)
        {
            if(Project != null)
            {
                Project.Update((float)(_gameTime.ElapsedGameTime.TotalMilliseconds / 1000));
                InputController.Instance.Clear();
                //

                var models = Project.CurrentLevel.GetSelectedModels().ToArray();

                if (!m_selected.SequenceEqual(models))
                {
                    UpdateSelectionAndModels(models);
                }

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
            if(Project == null) return;
            Camera c = Project.CurrentLevel.GetCamera();
            c.Viewport = m_graphics.GraphicsDevice.Viewport;
            c.Update(c.Position, m_graphics.GraphicsDevice.Viewport.AspectRatio);
        }
    }
}
