using SharpDX.DirectInput;

namespace VoxelEngine
{
    internal sealed class DirectInputFactory : IFactory<DirectInput>
    {
        private DirectInput _instance = null;

        public DirectInput Create()
        {
            return _instance ?? (_instance = new DirectInput());
        }
    }
}
