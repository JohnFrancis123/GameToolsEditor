using Editor.Engine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;
using SharpDX.MediaFoundation;
using System;
using System.Drawing.Text;
using System.IO;
using System.Runtime;

namespace Editor.Engine
{
    internal class CelestialBody
    {
        public Models CelestialBodyModel { get; set; }
        public byte BodyType { get; set; } //0 for sun, 1 for planet, 2 for moon
        public float RotSpeed { get; set; }
        public float OrbitSpeed { get; set; }

        private Random m_rand = new Random();

        public Models ParentBodyModel { get; set; }
        //public float Scale {  get; set; }


        public CelestialBody() 
        {

        }

        public CelestialBody(Models _celestialBodyModel, byte _bodyType, Models _parentBodyModel = null)
        {
            ParentBodyModel = _parentBodyModel;
            CelestialBodyModel = _celestialBodyModel;
            BodyType = _bodyType;

            CreateBody();

            //Vector3 PosVector = new Vector3(0, 0, 0);

            //if (ParentBodyModel != null)
            //    PosVector = ParentBodyModel.Position;

            //if (BodyType == 0) //sun
            //{
            //    RotSpeed = 0.005f;
            //    OrbitSpeed = 0.0f;

            //    CelestialBodyModel.Scale = 2.0f;
            //}
            //else if (BodyType == 1) //planet
            //{
            //    RotSpeed = FRand(0.02f, 0.03f);
            //    OrbitSpeed = FRand(0.001f, 0.002f);

            //    PosVector.X += FRand(-150.0f, 150.0f);
            //    PosVector.Y += FRand(-90.0f, 90.0f);
            //    CelestialBodyModel.Scale = 0.75f;
            //}
            //else if (BodyType == 2) //moon
            //{
            //    RotSpeed = FRand(0.005f, 0.01f);
            //    OrbitSpeed = FRand(0.01f, 0.02f);

            //    PosVector = ParentBodyModel.Position;

            //    //no instructions saying where the moons must spawn, so I estimated the position using the video
            //    PosVector.X += 75.0f; //the video itself showed all the moons spawning in the same spot, roughly a planet's radius away

            //    CelestialBodyModel.Scale = FRand(0.2f, 0.4f);
            //}
            //CelestialBodyModel.Position = PosVector;
        }


        private void CreateBody()
        {
            Vector3 PosVector = Vector3.Zero;
            float initialAngle = 0.0f;
            float fixedRad = 0.0f;

            if (BodyType == 0) //sun
            {
                RotSpeed = 0.005f;
                OrbitSpeed = 0.0f;
                CelestialBodyModel.Scale = 2.0f;
            }
            else if (BodyType == 1) //planet
            {
                RotSpeed = FRand(0.02f, 0.03f);
                OrbitSpeed = FRand(0.001f, 0.002f);
                CelestialBodyModel.Scale = 0.75f;

                //random X/Y position
                PosVector.X = FRand(-150.0f, 150.0f);
                PosVector.Y = FRand(-90.0f, 90.0f);

                //calculating the fixed radius and angle based on the random position
                fixedRad = new Vector2(PosVector.X, PosVector.Y).Length();
                initialAngle = (float)Math.Atan2(PosVector.Y, PosVector.X);
            }
            else if (BodyType == 2) //moon
            {
                RotSpeed = FRand(0.005f, 0.01f);
                OrbitSpeed = FRand(0.01f, 0.02f);
                CelestialBodyModel.Scale = FRand(0.2f, 0.4f);

                //calculating fixed radius and random angle (CRITICAL FIX for stacking)
                fixedRad = FRand(5.0f, 15.0f);
                initialAngle = FRand(0.0f, 2f * (float)Math.PI);

                //setting PosVector to its final world position based on the calculated offset
                if (ParentBodyModel != null)
                {
                    //NOTE: ParentBodyModel.Position is used here as a placeholder for the parent's current world pos.
                    PosVector = ParentBodyModel.Position;
                }
                //trigonometric math to get the 
                float offsetX = fixedRad * (float)Math.Cos(initialAngle);
                float offsetY = fixedRad * (float)Math.Sin(initialAngle);

                PosVector.X += offsetX;
                PosVector.Y += offsetY;
            }

            //storing orbital state and setting final orbit position
            if (BodyType != 0)
            {
                //storing orbital angle (Rotation.X) and fixed radius (Rotation.Z)
                CelestialBodyModel.Rotation = new Vector3(
                    initialAngle,
                    //CelestialBodyModel.Rotation.Y,
                    0.0f,
                    fixedRad // <--- Fixed Orbital Radius
                );
            }

            CelestialBodyModel.Position = PosVector;
        }
        public void Render(Matrix _view,
                           Matrix _projection)
        {
            Vector3 currRot = CelestialBodyModel.Rotation;
            currRot.Y += RotSpeed;
            CelestialBodyModel.Rotation = currRot; //updating the stored Y spin

            if(BodyType != 0) Orbit();


            CelestialBodyModel.Render(_view, _projection);
        }

        private float FRand(float min, float max)
        {
            double range = max - min;
            return (float)(m_rand.NextDouble() * range + min);
        }

        private void Orbit()
        {
            //retrieving the orbital state
            float angle = CelestialBodyModel.Rotation.X; //current orbital angle
            float rad = CelestialBodyModel.Rotation.Z;   //fixed orbital radius

            float currYSpin = CelestialBodyModel.Rotation.Y;

            //fallback if radius was not initialized (for first frame stability)
            if (rad == 0.0f && ParentBodyModel != null)
            {
                rad = new Vector2(CelestialBodyModel.Position.X - ParentBodyModel.Position.X,
                                  CelestialBodyModel.Position.Y - ParentBodyModel.Position.Y).Length();

                //storing the calculated radius back into Rotation.Z
                CelestialBodyModel.Rotation = new Vector3(angle, CelestialBodyModel.Rotation.Y, rad);
            }

            //updating orbital angle
            angle += OrbitSpeed;

            //matrix transformation

            //local translation matrix: Moves the body out by its fixed radius (rad, 0, 0)
            //this defines the body's fixed distance *from* its orbital center.
            Matrix translation = Matrix.CreateTranslation(rad, 0, 0);

            //orbital rotation matrix: Rotates the body around the Z-axis by the current angle
            //this defines the body's position *on* the orbit circle.
            Matrix orbitalRotation = Matrix.CreateRotationZ(angle);

            //world matrix: World = Translation * OrbitalRotation * ParentPositionMatrix
            //we compute the body's position relative to the origin, then shift the entire orbit to the parent's position.
            Matrix currentWorld = translation * orbitalRotation * Matrix.CreateTranslation(ParentBodyModel.Position);

            //storing the updated world position (for rendering/next frame parent reference)
            CelestialBodyModel.Position = currentWorld.Translation;

            //storing the updated orbital angle for the next frame
            CelestialBodyModel.Rotation = new Vector3(angle, currYSpin, rad);
        }
        public void Serialize(BinaryWriter _stream)
        {
            CelestialBodyModel.Serialize(_stream);

            bool hasParent = ParentBodyModel != null;
            _stream.Write(hasParent);

            
            if (hasParent)
            {
                ParentBodyModel.Serialize(_stream);
            }

            _stream.Write(RotSpeed);
            _stream.Write(OrbitSpeed);

            _stream.Write(BodyType);
        }

        public void Deserialize(BinaryReader _stream, ContentManager _content)
        {
            Models m = new();
            m.Deserialize(_stream, _content);
            CelestialBodyModel = m;

            bool hasParent = _stream.ReadBoolean();

            if (hasParent)
            {
                Models n = new();
                n.Deserialize(_stream, _content);
                ParentBodyModel = n;
            }
            else
            {
                ParentBodyModel = null;
            }

            RotSpeed = _stream.ReadSingle();
            OrbitSpeed = _stream.ReadSingle();

            BodyType = _stream.ReadByte();
        }
    }
}
