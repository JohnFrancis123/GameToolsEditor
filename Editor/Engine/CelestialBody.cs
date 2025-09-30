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


        public CelestialBody() 
        {

        }

        public CelestialBody(Models _celestialBodyModel, byte _bodyType, Models _parentBodyModel = null)
        {
            ParentBodyModel = _parentBodyModel;
            CelestialBodyModel = _celestialBodyModel;
            BodyType = _bodyType;
        }


        public void CreateBody()
        {
            Vector3 posVector = Vector3.Zero;
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
                posVector.X = FRand(-150.0f, 150.0f);
                posVector.Y = FRand(-90.0f, 90.0f);

                //calculating the fixed radius and angle based on the random position in radians
                fixedRad = new Vector2(posVector.X, posVector.Y).Length();
                initialAngle = (float)Math.Atan2(posVector.Y, posVector.X);
            }
            else if (BodyType == 2) //moon
            {
                RotSpeed = FRand(0.005f, 0.01f);
                OrbitSpeed = FRand(0.01f, 0.02f);
                CelestialBodyModel.Scale = FRand(0.2f, 0.4f);

                //radius of 20 and angle of 0 gives us a position at (20, 0, 0).
                fixedRad = 20.0f;
                initialAngle = 0.0f;
                

                //setting PosVector to its final world position based on the calculated offset
                posVector = ParentBodyModel.Position;
            }

            //storing orbital state and setting final orbit position
            if (BodyType != 0)
            {
                //sneaking the angle and radius into our Rotation vector for our Orbit function
                CelestialBodyModel.Rotation = new Vector3(
                    initialAngle, 
                    0.0f,
                    fixedRad // <--- Fixed Orbital Radius to save
                );
            }

            CelestialBodyModel.Position = posVector;
        }

        public void Render(Matrix _view,
                           Matrix _projection)
        {
            //updating local spin on Y axis for rotation effect
            Vector3 currRot = CelestialBodyModel.Rotation;
            currRot.Y += RotSpeed;

            //saving orbital state in case of corruption (technically not needed when always zero, but just for safety)
            float tempAngle = currRot.X; 
            float tempRadius = currRot.Z; 

            //zeroing X and Z for rendering, allowing only the correct Y-spin to apply.
            currRot.X = 0.0f; 
            currRot.Z = 0.0f; 

            CelestialBodyModel.Rotation = currRot; //final rotation state for drawing

            //rendering using the cleaned Rotation vector
            CelestialBodyModel.Render(_view, _projection);

            //restoring orbital state for the next frame's Orbit() call
            currRot.X = tempAngle; 
            currRot.Z = tempRadius;

            //saving the radius and angle for later
            CelestialBodyModel.Rotation = new Vector3(tempAngle, currRot.Y, tempRadius); 

            //performing the orbit using the angle and radius that we snuck into the Rotation vector
            if (BodyType != 0) Orbit();
        }

        private float FRand(float min, float max)
        {
            double range = max - min;
            return (float)(m_rand.NextDouble() * range + min);
        }

        private void Orbit()
        {

            //retrieving our angle and radius from the Rotation values.
            //rotation X and rotation Z are ALWAYS set to 0 during rendering.

            float angle = CelestialBodyModel.Rotation.X; //current orbital angle
            float rad = CelestialBodyModel.Rotation.Z;   //fixed orbital radius

            //Y rotation gives us our Y spin.
            float currYSpin = CelestialBodyModel.Rotation.Y;

            //fallback if radius was not initialized (for first frame stability)
            if (rad == 0.0f && ParentBodyModel != null)
            {
                rad = new Vector2(CelestialBodyModel.Position.X - ParentBodyModel.Position.X,
                                     CelestialBodyModel.Position.Y - ParentBodyModel.Position.Y).Length();

                //FIX: Removing the line that writes back to Rotation.Z here. We must assume CreateBody sets it.
                //CelestialBodyModel.Rotation = new Vector3(angle, CelestialBodyModel.Rotation.Y, rad);
            }

            //updating orbital angle
            angle += OrbitSpeed;

            //matrix transformation
            Matrix translation = Matrix.CreateTranslation(rad, 0, 0);
            Matrix orbitalRotation = Matrix.CreateRotationZ(angle); //rotation Z for our angle
            //taking advantage of matrix math to rotate around a point
            Matrix currentWorld = translation * orbitalRotation * Matrix.CreateTranslation(ParentBodyModel.Position);

            //storing the updated world position (for rendering/next frame parent reference)
            CelestialBodyModel.Position = currentWorld.Translation;

            //updating the rotation of our celestial body.
            CelestialBodyModel.Rotation = new Vector3(angle, currYSpin, rad);
        }

        public void Serialize(BinaryWriter _stream)
        {
            CelestialBodyModel.Serialize(_stream);

            bool hasParent = ParentBodyModel != null;
            _stream.Write(hasParent);

            if (hasParent)
            {
                //using the CelestialBodyModel's Rotation.X property to store 
                //the Parent's index just before serializing the body.
                //this is set by the Level class before calling Serialize.
                _stream.Write((int)CelestialBodyModel.Rotation.X);
            }

            _stream.Write(RotSpeed);
            _stream.Write(OrbitSpeed);
            _stream.Write(BodyType);
        }

        public int Deserialize(BinaryReader _stream, ContentManager _content)
        {
            Models m = new();
            m.Deserialize(_stream, _content);
            CelestialBodyModel = m;

            int parentIndex = -1; //defaulting to -1 (no parent)

            bool hasParent = _stream.ReadBoolean();

            if (hasParent)
            {
                //reading the saved parent index
                parentIndex = _stream.ReadInt32();
                ParentBodyModel = null; //essential: Starts unlinked
            }
            else
            {
                ParentBodyModel = null;
            }

            RotSpeed = _stream.ReadSingle();
            OrbitSpeed = _stream.ReadSingle();
            BodyType = _stream.ReadByte();

            return parentIndex; //returning the Parent Index for the Level class to use
        }
    }
}
