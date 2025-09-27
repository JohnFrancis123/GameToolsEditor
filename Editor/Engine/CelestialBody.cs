using Editor.Engine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;
using System;
using System.IO;

namespace Editor.Engine
{
    internal class CelestialBody
    {
        public Models CelestialBodyModel { get; set; }
        public byte BodyType { get; set; } //0 for sun, 1 for planet, 2 for moon

        public CelestialBody() 
        {

        }

        public CelestialBody(Models _celestialBodyModel, byte _bodyType)
        {
            CelestialBodyModel = _celestialBodyModel;
            BodyType = _bodyType;
        }

        public void Render(Matrix _view,
                           Matrix _projection)
        {
            if(BodyType == 0)
            {
                Vector3 currRot = CelestialBodyModel.Rotation;
                currRot.Y += 0.005f;
                CelestialBodyModel.Rotation = currRot;
            }

            CelestialBodyModel.Render(_view, _projection);
        }

        private void Orbit()
        {

        }

        public void Serialize(BinaryWriter _stream)
        {
            CelestialBodyModel.Serialize(_stream);
            //_stream.Write(BodyType.ToString());
            _stream.Write(BodyType);
        }

        public void Deserialize(BinaryReader _stream, ContentManager _content)
        {
            Models m = new();
            m.Deserialize(_stream, _content);
            CelestialBodyModel = m;

            //string bodyType = _stream.ReadString();
            //BodyType = ((byte)bodyType[0]);

            BodyType = _stream.ReadByte();
        }
    }
}
