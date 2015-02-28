namespace VoxelEngine
{
    public sealed class FPSCounter
    {
        private float _timeCounter = 0f;
        private int _fpsCounter = 0;
        private float _period = 1f;
        private float _fpsValue = 0f;

        public float FPS
        {
            get { return _fpsValue; }
        }

        public float Period
        {
            get { return _period; }
            set
            {
                _period = value;
                _fpsCounter = 0;
                _timeCounter = 0f;
            }
        }

        public FPSCounter(float period = 0.25f)
        {
            _period = period;
        }

        public void Frame(float elapsedTime)
        {
            _timeCounter += elapsedTime;
            if (_timeCounter < _period) _fpsCounter++;
            else
            {
                _fpsValue = (_fpsValue + (_fpsCounter / _period)) * 0.5f;
                _timeCounter = 0f;
                _fpsCounter = 0;
            }
        }
    }
}
