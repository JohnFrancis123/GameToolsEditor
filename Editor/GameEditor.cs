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
            base.Initialize();
        }

        protected override void LoadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        } //

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }
    }
}
