//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Editor
{
    internal class Camera //probably not supposed to be internal. Check if errors.
    {
        public Vector3 Position { get; set; } = new Vector3 (0, 0 ,0);
        public Matrix View {  get; set; } = Matrix.Identity;
        public Matrix Projection { get; set; } = Matrix.Identity;
        public float NearPlane { get; set; } = 0.1f;
        public float FarPlane { get; set; } = 1000f;

        public Camera(Vector3 _position, float _aspectRatio)
        {
            Update(_position, _aspectRatio);
        }
        public void Update(Vector3 _position, float _aspectRatio)
        {
            Position = _position;
            View = Matrix.CreateLookAt(Position, 
                             new Vector3(0, 0, 0), 
                             Vector3.Up);
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45),
                                                               _aspectRatio,
                                                               NearPlane,
                                                               FarPlane);
        }
    }
}
