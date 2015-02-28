using System;
using System.Windows.Forms;
using SharpDX.DirectInput;

namespace VoxelEngine.Input
{
    public static class Keyboard
    {
        private sealed class KeyboardMonitor : IDisposable
        {
            private const int KeysCount = 256;

            private SharpDX.DirectInput.Keyboard _keyboard;
            private float[] _timingKeys = new float[KeysCount];
            private KeyboardState _currentState;
            private KeyboardState _previousState;

            public KeyboardMonitor(DirectInput directInput, Form applicationForm)
            {
                _keyboard = new SharpDX.DirectInput.Keyboard(directInput);
                _keyboard.Properties.BufferSize = KeysCount;
                _keyboard.SetCooperativeLevel(applicationForm, CooperativeLevel.Foreground | CooperativeLevel.Exclusive);
            }

            public bool KeyDown(Key key)
            {
                if (_currentState == null || _previousState == null) return false;
                return _currentState.IsPressed(key) && !_previousState.IsPressed(key);
            }

            public bool KeyUp(Key key)
            {
                if (_currentState == null || _previousState == null) return false;
                return !_currentState.IsPressed(key) && _previousState.IsPressed(key);
            }

            public bool KeyHold(Key key)
            {
                if (_currentState == null || _previousState == null) return false;
                return _currentState.IsPressed(key) && _previousState.IsPressed(key);
            }

            public bool KeyFree(Key key)
            {
                if (_currentState == null || _previousState == null) return false;
                return !_currentState.IsPressed(key) && !_previousState.IsPressed(key);
            }

            public bool KeyHold(Key key, out float holdTime)
            {
                holdTime = _timingKeys[(int) key];
                return KeyHold(key);
            }

            public bool KeyFree(Key key, out float freeTime)
            {
                freeTime = _timingKeys[(int) key];
                return KeyFree(key);
            }

            public void Update(IGameTime time)
            {
                _previousState = _currentState;

                try
                {
                    _currentState = _keyboard.GetCurrentState();
                }
                catch (SharpDX.SharpDXException ex)
                {
                    if (ex.Descriptor == ResultCode.InputLost || ex.Descriptor == ResultCode.NotAcquired)
                    {
                        _keyboard.Acquire();
                    }
                }

                if (_previousState == null) return;
                for (int i = 0; i < _currentState.AllKeys.Count; i++)
                {
                    if (_currentState.AllKeys[i] != _previousState.AllKeys[i])
                    {
                        _timingKeys[i] = 0f;
                        continue;
                    }
                    _timingKeys[i] += time.ElapsedTime;
                }
            }

            #region IDisposable

            private bool _disposed = false;

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (_disposed) return;

                if (disposing)
                {
                    _keyboard.Unacquire();
                    _keyboard.Dispose();
                    _keyboard = null;
                }

                _disposed = true;
            }

            #endregion
        }

        private static KeyboardMonitor _keyboard;
        private static BaseGame _game;

        public static IDisposable Initialize(BaseGame game)
        {
            _game = game;
            return (_keyboard = new KeyboardMonitor(game.DirectInput, game.Form));
        }

        public static bool KeyDown(Key key)
        {
            return _keyboard.KeyDown(key);
        }

        public static bool KeyUp(Key key)
        {
            return _keyboard.KeyUp(key);
        }

        public static bool KeyHold(Key key)
        {
            return _keyboard.KeyHold(key);
        }

        public static bool KeyFree(Key key)
        {
            return _keyboard.KeyFree(key);
        }

        internal static void Update(IGameTime time)
        {
            // если текущая форма активна пытаемся обновить состояние
            // проверка нужна т.к. если форма не активна захватить устройство ввода не получится
            if (Form.ActiveForm == _game.Form)
            {
                _keyboard.Update(time);
            }
        }
    }
}
