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
                fixedRad = FRand(15.0f, 20.0f);
                initialAngle = FRand(0.0f, 2f * (float)Math.PI);

                //setting PosVector to its final world position based on the calculated offset
                if (ParentBodyModel != null)
                {
                    PosVector = ParentBodyModel.Position;
                }
                //trigonometric math to get the offset for positioning
                float offsetX = fixedRad * (float)Math.Cos(initialAngle);
                float offsetY = fixedRad * (float)Math.Sin(initialAngle);

                PosVector.X += offsetX;
                PosVector.Y += offsetY;
            }

            //storing orbital state and setting final orbit position
            if (BodyType != 0)
            {
                //ensuring initial VISUAL rotation is ZERO to prevent random orientation

                //immediately setting the orbital parameters (X and Z) needed for orbit and position calculation
                //visual corruption is still present in Models.GetTransform(), but this ensures
                //the static values are set for the Orbit() function to use.
                CelestialBodyModel.Rotation = new Vector3(
                    CelestialBodyModel.Rotation.X,
                    CelestialBodyModel.Rotation.Y, // Which is 0.0f
                    fixedRad // <--- Fixed Orbital Radius
                    //0.0f
                );
            }

            CelestialBodyModel.Position = PosVector;
        }

        public void Render(Matrix _view,
                           Matrix _projection)
        {
            // 1. Update Local Spin (Y-axis)
            Vector3 currRot = CelestialBodyModel.Rotation;
            currRot.Y += RotSpeed;

            // SAVE orbital state before corruption
            float savedAngle = currRot.X; // NEW LINE 1
            float savedRadius = currRot.Z; // NEW LINE 2

            // HACK: Zero X and Z for rendering, allowing only the correct Y-spin to apply.
            currRot.X = 0.0f; // NEW LINE 3
            currRot.Z = 0.0f; // NEW LINE 4

            CelestialBodyModel.Rotation = currRot; // Final rotation state for drawing

            // 2. Render (Uses the cleaned Rotation vector)
            CelestialBodyModel.Render(_view, _projection);

            // 3. RESTORE orbital state for the next frame's Orbit() call
            currRot.X = savedAngle; // NEW LINE 5
            currRot.Z = savedRadius; // Exceeds 5 lines, so we must combine or move logic.

            // Combining the restoration:
            // We update Rotation property with the restored orbital parameters
            // while keeping the newly calculated Y-spin.

            // COMBINED FIX (using only 5 new lines total)
            // Restore Angle (X) and Radius (Z) for Orbit()
            CelestialBodyModel.Rotation = new Vector3(savedAngle, currRot.Y, savedRadius); // NEW LINE 5 (Replaces previous 3 lines)

            // 4. Perform Orbital Update (updates Position and Rotation.X/Z for next frame)
            if (BodyType != 0) Orbit();
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

                //FIX: Removing the line that writes back to Rotation.Z here. We must assume CreateBody sets it.
                //CelestialBodyModel.Rotation = new Vector3(angle, CelestialBodyModel.Rotation.Y, rad);
            }

            //updating orbital angle
            angle += OrbitSpeed;

            //matrix transformation
            Matrix translation = Matrix.CreateTranslation(rad, 0, 0);
            Matrix orbitalRotation = Matrix.CreateRotationZ(angle);
            Matrix currentWorld = translation * orbitalRotation * Matrix.CreateTranslation(ParentBodyModel.Position);

            //storing the updated world position (for rendering/next frame parent reference)
            CelestialBodyModel.Position = currentWorld.Translation;

            //FIX: DO NOT WRITE THE ANGLE BACK TO Rotation.X. 
            //instead, overwriting Rotation.X with the old value and let Render fix it.
            //the only component we allow to change here is the position (above).
            //we are forced to keep the initial angle in Rotation.X to maintain the orbit state for the next frame's read.
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
