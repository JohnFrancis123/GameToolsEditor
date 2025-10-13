//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using Editor.Engine.Interfaces;
using Editor.Engine.ModelAttribs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

//using SharpDX.Direct2D1;
using System.ComponentModel;
using System.IO;

namespace Editor.Engine
{
    //[TypeConverter(typeof(ExpandableObjectConverter))]
    public class Models : ISerializable, INotifyPropertyChanged
    {
        // Accessors
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Transformation Transformation { 
            get => m_transformation; 
            set
            {
                OnPropertyChanged("Transformation");
                m_transformation = value;
            }
        }
        public State State { 
            get => m_state; 
            set
            {
                OnPropertyChanged("Selected");
                m_state = value;
            }
        }

        public Appearance Appearance { get => m_appearance; 
            set 
            {
                if (m_appearance != value)
                {
                    m_appearance = value;
                    OnPropertyChanged("Appearance");
                }
            } 
        }

        //event for property changes
        public event PropertyChangedEventHandler? PropertyChanged;



        //private Vector3 m_position;
        //private Vector3 m_rotation;
        //private float m_scale;
        //private bool m_selected;
        private Effect m_shader;
        private Model m_mesh;
        private Texture m_texture;
        private Transformation m_transformation;
        private State m_state;
        private Appearance m_appearance;

        private List<string> m_textures;

        public Model GetMesh() { return m_mesh; }

        public Models()
        {
        }

        public Models(ContentManager _content, string _model, string _texture, string _effect, Vector3 _position, float _scale)
        {
            m_transformation = new();
            m_state = new();
            m_appearance = new();
            m_textures = new();
            //m_textures.Add("Grass");
            //m_textures.Add("HeightMap");
            //m_textures.Add("Metal");
            m_appearance.DiffuseTexture = _texture;

            Create(_content, _model, _texture, _effect, _position, _scale);
        }

        public void Create(ContentManager _content, string _model, string _texture, string _effect, Vector3 _position, float _scale) 
        {
            //m_transformation = new();
            m_mesh = _content.Load<Model>(_model);
            m_mesh.Tag = _model;


            m_texture = _content.Load<Texture>(_texture);
            Appearance.DiffuseTexture = _texture;
            m_texture.Tag = _texture;

            m_shader = _content.Load<Effect>(_effect);
            m_shader.Tag = _effect;
            SetShader(m_shader);
            //m_position = _position;
            //Scale = _scale;
            Transformation.Position = _position;
            Transformation.Scale = _scale;
            m_state.Selected = false;
        }

        public void UpdateTex(ContentManager _content)
        {
            string texName = Appearance.DiffuseTexture;
            m_texture = _content.Load<Texture>(texName); ;
        }

        public void SetShader(Effect _effect)
        {
            m_shader = _effect;
            foreach (ModelMesh mesh in m_mesh.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = m_shader;
                }
            }
        }

        public void Translate(Vector3 _translate, Camera _camera)
        {
            Vector3 zeroVec = new Vector3(0, 0, 0);
            if (_translate == zeroVec) return;

            OnPropertyChanged("Transformation");

            float distance = Vector3.Distance(_camera.Target, _camera.Position);
            Vector3 forward = _camera.Target - _camera.Position;
            forward.Normalize();
            Vector3 left = Vector3.Cross(forward, Vector3.Up);
            left.Normalize();
            Vector3 up = Vector3.Cross(left, forward);
            up.Normalize();
            Transformation.Position += left * _translate.X * distance;
            Transformation.Position += up * _translate.Y * distance;
            Transformation.Position += forward * _translate.Z * 100f;
        }

        public void Rotate(Vector3 _rotate)
        {
            Vector3 zeroVec = new Vector3(0, 0, 0);
            if (_rotate == zeroVec) return;

            OnPropertyChanged("Transformation");
            Transformation.Rotation += _rotate;
        }

        public void Scale(float _scale)
        {
            if (_scale == 0) return;
            OnPropertyChanged("Transformation");
            Transformation.Scale += _scale;
        }

        public Matrix GetTransform()
        {
            return Matrix.CreateScale(Transformation.Scale) *
                   Matrix.CreateFromYawPitchRoll(Transformation.Rotation.Y, Transformation.Rotation.X, Transformation.Rotation.Z) *
                   Matrix.CreateTranslation(Transformation.Position);
        }

        public void Render(Matrix _view, 
                           Matrix _projection)
        {
            //m_position.X += 0.001f;
            //m_rotation.Y += 0.005f;

            m_shader.Parameters["World"].SetValue(GetTransform());
            m_shader.Parameters["WorldViewProjection"].SetValue(GetTransform() * _view * _projection);
            m_shader.Parameters["Texture"].SetValue(m_texture);
            m_shader.Parameters["Tint"].SetValue(State.Selected);

            foreach (ModelMesh mesh in m_mesh.Meshes)
            {
                mesh.Draw();
            }
        }

        public void Serialize(BinaryWriter _stream)
        {
            _stream.Write(m_mesh.Tag.ToString());
            _stream.Write(m_texture.Tag.ToString());
            _stream.Write(m_shader.Tag.ToString());
            HelpSerialize.Vec3(_stream, Transformation.Position);
            HelpSerialize.Vec3(_stream, Transformation.Rotation);
            _stream.Write(Transformation.Scale);
        }

        public void Deserialize(BinaryReader _stream, ContentManager _content) 
        {
            Transformation = new();
            string mesh = _stream.ReadString();
            string texture = _stream.ReadString();
            string shader = _stream.ReadString();
            Transformation.Position = HelpDeserialize.Vec3(_stream);
            Transformation.Rotation = HelpDeserialize.Vec3(_stream);
            Transformation.Scale = _stream.ReadSingle();
            Create(_content, mesh, texture, shader, Transformation.Position, Transformation.Scale);
        }
    }
}
