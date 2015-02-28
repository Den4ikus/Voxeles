using SharpDX;

namespace VoxelEngine
{
    public static class VectorExtesion
    {
        public static Vector3 ToVector3(this Vector4 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        public static Vector4 ToVector4(this Vector3 v)
        {
            return new Vector4(v, 1f);
        }
    }
}
