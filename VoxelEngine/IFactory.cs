using System;

namespace VoxelEngine
{
    interface IFactory<T> where T : IDisposable
    {
        T Create();
    }
}
