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
using System.ComponentModel;
using System.IO;

namespace Editor.Engine
{
    class Models : ISerializable, INotifyPropertyChanged
    {
        // Accessors
        public Model Mesh { get; set; }
        public Effect Shader { get; set; }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Transformation Transformation { get => m_transformation; 
            set
            {
                OnPropertyChanged("Transformation");
                m_transformation = value;
            }
        }
        #region transformations
        //public Vector3 Position { get => m_position; 
        //    set
        //    {
        //        if (m_position != value)
        //        {
        //            m_position = value;

        //            m_transformation.Position = value;
        //            //OnPropertyChanged("Position");
        //        }
        //    } 
        //}
        //public Vector3 Rotation { get => m_rotation; 
        //    set 
        //    {
        //        if (m_rotation != value)
        //        {
        //            m_rotation = value;
        //            m_transformation.Rotation = value;
        //            //OnPropertyChanged("Rotation");
        //        }
        //    } 
        //}
        //public float Scale { get => m_scale;
        //    set 
        //    {
        //        if (m_scale != value)
        //        {
        //            m_scale = value;
        //            m_transformation.Scale = value;
        //            //OnPropertyChanged("Scale");
        //        }
        //    }
        //}
        #endregion transformations



        public bool Selected
        {
            get => m_selected;
            set
            {
                if (m_selected != value)
                {
                    m_selected = value;
                    OnPropertyChanged("Selected");
                }
            }
        }
        // Texturing
        public Texture Texture
        {
            get => m_texture;
            set
            {
                if (m_texture != value)
                {
                    m_texture = value;
                    OnPropertyChanged("Texture");
                }
            }
        }

        //event for property changes
        public event PropertyChangedEventHandler? PropertyChanged;



        //private Vector3 m_position;
        //private Vector3 m_rotation;
        //private float m_scale;
        private bool m_selected;
        private Texture m_texture;
        private Transformation m_transformation;

        public Models()
        {
        }

        public Models(ContentManager _content, string _model, string _texture, string _effect, Vector3 _position, float _scale)
        {
            m_transformation = new();
            Create(_content, _model, _texture, _effect, _position, _scale);
        }

        public void Create(ContentManager _content, string _model, string _texture, string _effect, Vector3 _position, float _scale) 
        {
            //m_transformation = new();
            Mesh = _content.Load<Model>(_model);
            Mesh.Tag = _model;
            Texture = _content.Load<Texture>(_texture);
            Texture.Tag = _texture;
            Shader = _content.Load<Effect>(_effect);
            Shader.Tag = _effect;
            SetShader(Shader);
            //m_position = _position;
            //Scale = _scale;
            m_transformation.Position = _position;
            m_transformation.Scale = _scale;
            Selected = false;
        }

        public void SetShader(Effect _effect)
        {
            Shader = _effect;
            foreach (ModelMesh mesh in Mesh.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = Shader;
                }
            }
        }

        public void Translate(Vector3 _translate, Camera _camera)
        {
            Vector3 zeroVec = new Vector3(0, 0, 0);
            if (_translate == zeroVec) return;

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
            Transformation.Rotation += _rotate;
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

            Shader.Parameters["World"].SetValue(GetTransform());
            Shader.Parameters["WorldViewProjection"].SetValue(GetTransform() * _view * _projection);
            Shader.Parameters["Texture"].SetValue(Texture);
            Shader.Parameters["Tint"].SetValue(Selected);

            foreach (ModelMesh mesh in Mesh.Meshes)
            {
                mesh.Draw();
            }
        }

        public void Serialize(BinaryWriter _stream)
        {
            _stream.Write(Mesh.Tag.ToString());
            _stream.Write(Texture.Tag.ToString());
            _stream.Write(Shader.Tag.ToString());
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
