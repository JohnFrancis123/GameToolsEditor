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
            Vector3 PosVector = Vector3.Zero; // The final position starts at (0, 0, 0)
            float initialOrbitRadius = 0.0f; // Distance from parent
            float initialAngle = 0.0f;      // Random start angle for separation

            // Set speeds, scale, and initial positioning parameters
            if (BodyType == 0) // Sun
            {
                RotSpeed = 0.005f;
                OrbitSpeed = 0.0f;
                CelestialBodyModel.Scale = 2.0f;
                // Position remains (0, 0, 0)
            }
            else if (BodyType == 1) // Planet
            {
                RotSpeed = FRand(0.02f, 0.03f);
                OrbitSpeed = FRand(0.001f, 0.002f);
                CelestialBodyModel.Scale = 0.75f;

                // Planet position is purely random within the bounds (not relative to parent position)
                PosVector.X += FRand(-150.0f, 150.0f);
                PosVector.Y += FRand(-90.0f, 90.0f);
                PosVector.Z = 0.0f; // Assuming 0 for X-Y plane

                //CRITICAL FIX: Initializing the orbit angle(Rotation.X) randomly, so planets spawn properly.
                float initAngle = FRand(0.0f, 2.0f * (float)Math.PI);
                CelestialBodyModel.Rotation = new Vector3(
                    initAngle,
                    CelestialBodyModel.Rotation.Y, //preserving visual location 
                    0.0f //preserving unused Z location
                    );
            }
            else if (BodyType == 2) // Moon
            {
                RotSpeed = FRand(0.005f, 0.01f);
                OrbitSpeed = FRand(0.01f, 0.02f);
                CelestialBodyModel.Scale = FRand(0.2f, 0.4f);

                // 1. Get parent's world position as the base

                PosVector = ParentBodyModel.Position - ParentBodyModel.Rotation; // e.g., (100, 50, 0)

                // 2. Calculate random orbital parameters (Crucial for non-stacking)
                initialOrbitRadius = FRand(5.0f, 15.0f); // Random distance from planet
                initialAngle = FRand(0.0f, 2f * (float)Math.PI); // Full 360-degree angle (THE FIX)

                // 3. Calculate the offset based on the random angle
                float offsetX = initialOrbitRadius * (float)Math.Cos(initialAngle);
                float offsetY = initialOrbitRadius * (float)Math.Sin(initialAngle);

                // 4. Apply the offset to the parent's position
                PosVector.X += offsetX;
                PosVector.Y += offsetY;
                // PosVector.Z remains parent's Z (likely 0)

                // 5. Store the starting angle for the Orbit() method to use (THE FIX)
                CelestialBodyModel.Rotation = new Vector3(
                    initialAngle,
                    CelestialBodyModel.Rotation.Y,
                    CelestialBodyModel.Rotation.Z
                );
            }

            // 2. Final Position Calculation (Only for Moons now, as Planets are done above)
            if (BodyType == 2 && initialOrbitRadius > 0.0f)
            {
                // Calculate the initial offset in X-Y plane (relative to parent)
                float offsetX = initialOrbitRadius * (float)Math.Cos(initialAngle);
                float offsetY = initialOrbitRadius * (float)Math.Sin(initialAngle);

                // Add the offset to the parent's base position (which is PosVector)
                PosVector.X += offsetX;
                PosVector.Y += offsetY;

                // Store the starting angle for the Orbit() method to use
                CelestialBodyModel.Rotation = new Vector3(initialAngle, CelestialBodyModel.Rotation.Y, CelestialBodyModel.Rotation.Z);
            }

            // 3. Assign final position to the model
            CelestialBodyModel.Position = PosVector;
        }
        public void Render(Matrix _view,
                           Matrix _projection)
        {
            Vector3 currRot = CelestialBodyModel.Rotation;
            currRot.Y += RotSpeed;
            CelestialBodyModel.Rotation = currRot;

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
            //getting our positions
            Vector3 parentPos = new Vector3(0, 0, 0);
            Vector3 currentPos = CelestialBodyModel.Position;
            if (ParentBodyModel != null)
            {
                parentPos = ParentBodyModel.Position;
            }

            //getting the angle to update based on speed (radians)
            float angle = CelestialBodyModel.Rotation.X;
            //angle += OrbitSpeed;

            //getting the fixed orbital radius as distance scalar
            float rad = new Vector2(currentPos.X - parentPos.X, currentPos.Y - parentPos.Y).Length();

            angle += OrbitSpeed;

            //getting the new position using trigonometry
            float newX = parentPos.X + rad * (float)Math.Cos(angle);
            float newY = parentPos.Y + rad * (float)Math.Sin(angle);

            //updating the position and stored orbital angle for the next frame
            CelestialBodyModel.Position = new Vector3(newX, newY, 0);
            CelestialBodyModel.Rotation = new Vector3(angle, CelestialBodyModel.Rotation.Y, CelestialBodyModel.Rotation.Z);
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
