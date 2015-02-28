using System.Runtime.InteropServices;
using SharpDX;

namespace VoxelEngine
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionTexture
    {
        public Vector4 Position;
        public Vector2 TexCoord;

        public VertexPositionTexture(Vector3 position, Vector2 texCoord)
        {
            Position = position.ToVector4();
            TexCoord = texCoord;
        }
    }
}
