using System;
using System.Windows.Forms;
using SharpDX;
using SharpDX.DirectInput;
using SharpDX.Windows;

namespace VoxelEngine.Input
{
    public static class Mouse
    {
        private sealed class MouseMonitor : IDisposable
        {
            private const int KeysCount = 3;

            private static SharpDX.DirectInput.Mouse _mouse;
            private static float[] _timingKeys = new float[KeysCount];
            private static MouseState _currentState;
            private static MouseState _previousState;

            public Vector2 Position { get; private set; }
            public Vector2 PrevPosition { get; private set; }
            public Vector2 PositionDelta { get; private set; }

            public MouseMonitor(DirectInput directInput, RenderForm applicationForm)
            {
                _mouse = new SharpDX.DirectInput.Mouse(directInput);
                _mouse.Properties.BufferSize = KeysCount;
                _mouse.Properties.AxisMode = DeviceAxisMode.Relative;
                _mouse.SetCooperativeLevel(applicationForm, CooperativeLevel.Foreground | CooperativeLevel.NonExclusive);
                //_mouse.Properties.AutoCenter = true;
            }

            public bool ButtonDown(MouseButtons button)
            {
                if (_currentState == null || _previousState == null) return false;
                return _currentState.Buttons[(int) button] && !_previousState.Buttons[(int) button];
            }

            public bool ButtonUp(MouseButtons button)
            {
                if (_currentState == null || _previousState == null) return false;
                return !_currentState.Buttons[(int) button] && _previousState.Buttons[(int) button];
            }

            public bool ButtonHold(MouseButtons button)
            {
                if (_currentState == null || _previousState == null) return false;
                return _currentState.Buttons[(int) button] && _previousState.Buttons[(int) button];
            }

            public bool ButtonFree(MouseButtons button)
            {
                if (_currentState == null || _previousState == null) return false;
                return !_currentState.Buttons[(int) button] && !_previousState.Buttons[(int) button];
            }

            public bool ButtonHold(MouseButtons button, out float holdTime)
            {
                holdTime = _timingKeys[(int) button];
                return ButtonHold(button);
            }

            public bool ButtonFree(MouseButtons button, out float freeTime)
            {
                freeTime = _timingKeys[(int) button];
                return ButtonFree(button);
            }

            public void Update(IGameTime time)
            {
                _previousState = _currentState;

                try
                {
                    _currentState = _mouse.GetCurrentState();

                    PrevPosition = Position;
                    Position = new Vector2(_currentState.X, _currentState.Y);
                    PositionDelta = new Vector2(_currentState.X, _currentState.Y);//Position - PrevPosition;

                    if (_previousState == null) return;
                    for (int i = 0; i < KeysCount; i++)
                    {
                        if (_currentState.Buttons[i] != _previousState.Buttons[i])
                        {
                            _timingKeys[i] = 0f;
                            continue;
                        }
                        _timingKeys[i] += time.ElapsedTime;
                    }
                }
                catch (SharpDXException ex)
                {
                    if (ex.Descriptor == ResultCode.InputLost || ex.Descriptor == ResultCode.NotAcquired)
                    {
                        _mouse.Acquire();
                    }
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
                    _mouse.Unacquire();
                    _mouse.Dispose();
                    _mouse = null;
                }

                _disposed = true;
            }

            #endregion
        }

        private static MouseMonitor _mouse;
        private static BaseGame _game;

        public static Vector2 PositionDelta
        {
            get { return _mouse.PositionDelta; }
        }

        public static IDisposable Initialize(BaseGame game)
        {
            _game = game;
            return (_mouse = new MouseMonitor(game.DirectInput, game.Form));
        }

        public static bool ButtonDown(MouseButtons button)
        {
            return _mouse.ButtonDown(button);
        }

        public static bool ButtonUp(MouseButtons button)
        {
            return _mouse.ButtonUp(button);
        }

        public static bool ButtonHold(MouseButtons button)
        {
            return _mouse.ButtonHold(button);
        }

        public static bool ButtonFree(MouseButtons button)
        {
            return _mouse.ButtonFree(button);
        }

        internal static void Update(IGameTime time)
        {
            // если текущая форма активна пытаемся обновить состояние
            // проверка нужна т.к. если форма не активна захватить устройство ввода не получится
            if (Form.ActiveForm == _game.Form)
            {
                _mouse.Update(time);
            }
        }
    }
}
