using System;

namespace VoxelEngine
{
    public sealed class GameTimeManager : IGameTime
    {
        private long _resetTime = DateTime.UtcNow.Ticks;
        private float _elapsedTime = 0f;

        public float ElapsedTime
        {
            get { return _elapsedTime; }
        }

        public void Reset()
        {
            _elapsedTime = (float) (DateTime.UtcNow.Ticks - _resetTime)/TimeSpan.TicksPerSecond;
            _resetTime = DateTime.UtcNow.Ticks;
        }
    }
}
