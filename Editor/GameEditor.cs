using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Windows.Forms;

namespace Editor
{
    public class GameEditor : Game
    {
        private GraphicsDeviceManager m_graphics;
        private FormEditor m_parent;
        private Camera m_camera;
        private Models m_teapot;
        private Effect m_myShader;
        private Texture m_metalTexture;

        public GameEditor()
        {
            m_graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
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
            m_camera = new Camera(new Vector3(0, 1, 1), m_graphics.GraphicsDevice.Viewport.AspectRatio);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            m_myShader = Content.Load<Effect>("MyShader");
            m_metalTexture = Content.Load<Texture>("Metal");
            m_teapot = new Models(Content.Load<Model>("obj/Teapot"), m_metalTexture, Vector3.Zero, 1);
            m_teapot.SetShader(m_myShader);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        } //

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            m_teapot.Render(m_camera.View, m_camera.Projection);

            base.Draw(gameTime);
        }
    }
}
