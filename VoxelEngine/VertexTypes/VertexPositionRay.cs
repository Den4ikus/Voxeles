using System.Runtime.InteropServices;
using SharpDX;

namespace VoxelEngine
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionRay
    {
        public Vector2 Position;
        public Vector3 RayStart;
        public Vector3 RayEnd;

        public VertexPositionRay(Vector2 position, Vector3 rayStart, Vector3 rayEnd)
        {
            Position = position;
            RayStart = rayStart;
            RayEnd = rayEnd;
        }
    }
}
