using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Editor.Engine
{
    internal class HelpSerialize
    {
        public static void Vec3(BinaryWriter _stream, Vector3 _vector)
        {
            _stream.Write(_vector.X);
            _stream.Write(_vector.Y);
            _stream.Write(_vector.Z);
        }
    }

    internal class HelpDeserialize
    {
        public static Vector3 Vec3(BinaryReader _stream)
        {
            Vector3 v = Vector3.Zero;
            v.X = _stream.ReadSingle();
            v.Y = _stream.ReadSingle();
            v.Z = _stream.ReadSingle();
            return v;
        }
    }

    internal class HelpMath
    {
        public static Ray GetPickRay(Vector2 _mousePosition, Camera _camera)
        {
            Vector3 nearPoint = new Vector3(_mousePosition, 0);
            Vector3 farPoint = new Vector3(_mousePosition, 1);

            nearPoint = _camera.Viewport.Unproject(nearPoint, _camera.Projection, _camera.View, Matrix.Identity);
            farPoint = _camera.Viewport.Unproject(farPoint, _camera.Projection, _camera.View, Matrix.Identity);

            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            return new Ray(nearPoint, direction);
        }

        /// <summary>
        /// Checks whether a ray intersects a triangle. This uses the algorithm 
        /// developed by Tomas Moller and Ben Trumbore, which was published in the
        /// Journal of Graphics Tools, volume 2, "Fast, Minimum Storage Ray-Triangle Intersection".
        ///
        /// This method is implemented using the pass-by-reference version of the
        /// MonoGame math functions. Using these overloads is generally not recommended,
        /// because they make the code less readable than the normal versions.
        /// This method can be called very frequently in a tight inner loop; however, 
        /// so, in this particular case, the performance benefits from passing
        /// everything by reference outweighs the loss of readability.
        /// </summary>
        public static void RayIntersectsTriangle(ref Ray ray, ref Vector3 vertex1, ref Vector3 vertex2, ref Vector3 vertex3, out float? result)
        {
            // Compute vectors along two edges of the triangle.
            Vector3.Subtract(ref vertex2, ref vertex1, out Vector3 edge1);
            Vector3.Subtract(ref vertex3, ref vertex1, out Vector3 edge2);

            // Compute the determinant
            Vector3.Cross(ref ray.Direction, ref edge2, out Vector3 directionCrossEdge2);
            Vector3.Dot(ref edge1, ref directionCrossEdge2, out float determinant);

            // If the ray is parallel to the triangle plane, there is no collision.
            if (determinant > -float.Epsilon && determinant < float.Epsilon)
            {
                result = null;
                return;
            }

            // Calculate the U parameter of the intersection point.
            Vector3.Subtract(ref ray.Position, ref vertex1, out Vector3 distanceVector);
            Vector3.Dot(ref distanceVector, ref directionCrossEdge2, out float triangleU);
            float inverseDeterminant = 1.0f / determinant;
            triangleU *= inverseDeterminant;

            // Make sure it is inside the triangle.
            if (triangleU < 0 || triangleU > 1)
            {
                result = null;
                return;
            }

            // Calculate the V parameter of the intersection point.
            Vector3.Cross(ref distanceVector, ref edge1, out Vector3 distanceCrossEdge1);
            Vector3.Dot(ref ray.Direction, ref distanceCrossEdge1, out float triangleV);
            triangleV *= inverseDeterminant;

            // Make sure it is inside the triangle.
            if (triangleV < 0 || triangleU + triangleV > 1)
            {
                result = null;
                return;
            }

            // Compute the distance along the ray to the triangle.
            Vector3.Dot(ref edge2, ref distanceCrossEdge1, out float rayDistance);
            rayDistance *= inverseDeterminant;

            // Is the triangle behind the ray origin?
            if (rayDistance < 0)
            {
                result = null;
                return;
            }

            result = rayDistance;
        }

        //public static float? PickTriangle(in Terrain _terrain, ref Ray _ray, ref Matrix _transform)
        //{
        //    Vector3 pos1 = new(); Vector3 pos2 = new(); Vector3 pos3 = new();

        //    for (int i = 0; i < _terrain.Indices.Length; i += 3)
        //    {
        //        int index = _terrain.Indices[i];
        //        pos1 = _terrain.Vertices[index].Position;
        //        Vector3.Transform(ref pos1, ref _transform, out pos1);

        //        index = _terrain.Indices[i + 1];
        //        pos2 = _terrain.Vertices[index].Position;
        //        Vector3.Transform(ref pos2, ref _transform, out pos2);

        //        index = _terrain.Indices[i + 2];
        //        pos3 = _terrain.Vertices[index].Position;
        //        Vector3.Transform(ref pos3, ref _transform, out pos3);

        //        RayIntersectsTriangle(ref _ray, ref pos1, ref pos2, ref pos3, out float? res);
        //        if (res.HasValue)
        //        {
        //            return res;
        //        }
        //    }

        //    return null;
        //}

        public static float? PickTriangle(in Terrain _terrain, ref Ray _ray, ref Matrix _transform) //post-optimization fix
        {
            Vector3 pos1 = new(); Vector3 pos2 = new(); Vector3 pos3 = new();

            // Choose index source
            bool use16 = _terrain.Indices16 != null;
            var indices16 = _terrain.Indices16;
            var indices32 = _terrain.Indices32;

            int indexCount = _terrain.IndexCount;

            for (int i = 0; i < indexCount; i += 3)
            {
                int i0 = use16 ? indices16[i] : indices32[i];
                int i1 = use16 ? indices16[i + 1] : indices32[i + 1];
                int i2 = use16 ? indices16[i + 2] : indices32[i + 2];

                pos1 = _terrain.Positions[i0];
                Vector3.Transform(ref pos1, ref _transform, out pos1);

                pos2 = _terrain.Positions[i1];
                Vector3.Transform(ref pos2, ref _transform, out pos2);

                pos3 = _terrain.Positions[i2];
                Vector3.Transform(ref pos3, ref _transform, out pos3);

                RayIntersectsTriangle(ref _ray, ref pos1, ref pos2, ref pos3, out float? res);
                if (res.HasValue)
                {
                    return res;
                }
            }

            return null;
        }



        [ThreadStatic]
        private static short[] CachedIndices;
        [ThreadStatic]
        private static float[] CachedVertices;

        // ... GetPickRay and RayIntersectsTriangle methods remain the same ...

        // The method in question:
        public static float? PickTriangle(in ModelMesh _mesh, ref Ray _ray, ref Matrix _transform)
        {
            Vector3 pos1 = new(); Vector3 pos2 = new(); Vector3 pos3 = new();

            foreach (var part in _mesh.MeshParts)
            {
                int stride = part.VertexBuffer.VertexDeclaration.VertexStride / 4;

                // 1. Manage Index Buffer
                int indexCount = part.IndexBuffer.IndexCount;
                if (CachedIndices == null || CachedIndices.Length < indexCount)
                {
                    // Allocate a new array only if the existing one is too small.
                    CachedIndices = new short[indexCount];
                }
                // Use GetData with the cached buffer (up to indexCount elements)
                part.IndexBuffer.GetData<short>(0, CachedIndices, 0, indexCount);

                // 2. Manage Vertex Buffer
                int vertexFloatCount = part.VertexBuffer.VertexCount * stride;
                if (CachedVertices == null || CachedVertices.Length < vertexFloatCount)
                {
                    // Allocate a new array only if the existing one is too small.
                    CachedVertices = new float[vertexFloatCount];
                }
                // Use GetData with the cached buffer (up to vertexFloatCount elements)
                part.VertexBuffer.GetData<float>(0, CachedVertices, 0, vertexFloatCount);

                // Now, use CachedIndices and CachedVertices instead of the old local variables.
                // The rest of the logic is unchanged.
                for (int i = part.StartIndex; i < part.StartIndex + part.PrimitiveCount * 3; i += 3)
                {
                    // The indices array is now CachedIndices
                    int index = (part.VertexOffset + CachedIndices[i]) * stride;

                    // The vertices array is now CachedVertices
                    pos1.X = CachedVertices[index]; pos1.Y = CachedVertices[index + 1]; pos1.Z = CachedVertices[index + 2];
                    Vector3.Transform(ref pos1, ref _transform, out pos1);

                    index = (part.VertexOffset + CachedIndices[i + 1]) * stride;
                    pos2.X = CachedVertices[index]; pos2.Y = CachedVertices[index + 1]; pos2.Z = CachedVertices[index + 2];
                    Vector3.Transform(ref pos2, ref _transform, out pos2);

                    index = (part.VertexOffset + CachedIndices[i + 2]) * stride;
                    pos3.X = CachedVertices[index]; pos3.Y = CachedVertices[index + 1]; pos3.Z = CachedVertices[index + 2];
                    Vector3.Transform(ref pos3, ref _transform, out pos3);

                    RayIntersectsTriangle(ref _ray, ref pos1, ref pos2, ref pos3, out float? res);
                    if (res.HasValue)
                    {
                        return res;
                    }
                }
            }
            return null;
        }
    }

}