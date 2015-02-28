using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.DirectInput;
using SharpDX.Windows;
using Device = SharpDX.Direct3D11.Device;
using Keyboard = VoxelEngine.Input.Keyboard;
using Mouse = VoxelEngine.Input.Mouse;

namespace VoxelEngine
{
    public abstract class BaseGame : IDisposable
    {
        protected RenderForm _form;
        protected GameTimeManager _gameTimeManager = new GameTimeManager();
        protected FPSCounter _fpsCounter = new FPSCounter();

        protected Device device;
        protected DeviceContext context;
        private SwapChain swapChain;
        private RenderTargetView renderView;
        private Texture2D backBuffer;
        private Factory factory;

        private List<IDisposable> _disposableList = new List<IDisposable>(); 

        public RenderForm Form
        {
            get { return _form; }
        }

        private DirectInput _directInputInstance = null;
        internal DirectInput DirectInput
        {
            get
            {
                if (_directInputInstance == null) _disposableList.Add(_directInputInstance = new DirectInputFactory().Create());
                return _directInputInstance;
            }
        }

        protected BaseGame()
        {
            _form = new RenderForm("SharpDX Direct3D 11 Sample")
                {
                    Width = 800,
                    Height = 600
                };
            _disposableList.Add(_form);

            //_form.FormClosing += delegate(object sender, FormClosingEventArgs args)
            //    {
            //        Unload(args.CloseReason);
            //    };
            //_form.KeyDown += (sender, args) => { if (args.Alt)  };
        }

        protected virtual void Initialize()
        {
            _disposableList.Add(Mouse.Initialize(this));
            _disposableList.Add(Keyboard.Initialize(this));
            
            
            // SwapChain description
            var desc = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(
                    _form.ClientSize.Width,
                    _form.ClientSize.Height,
                    new Rational(60, 1),
                    Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = _form.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            // Create Device and SwapChain
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, desc, out device, out swapChain);
            context = device.ImmediateContext;

            // Ignore all windows events
            factory = swapChain.GetParent<Factory>();
            factory.MakeWindowAssociation(_form.Handle, WindowAssociationFlags.IgnoreAll);

            // New RenderTargetView from the backbuffer
            backBuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);
            renderView = new RenderTargetView(device, backBuffer);

            context.Rasterizer.SetViewport(new Viewport(0, 0, _form.ClientSize.Width, _form.ClientSize.Height, 0.0f, 1.0f));
            context.OutputMerger.SetTargets(renderView);
        }

        protected virtual void Update(IGameTime time)
        {
            Mouse.Update(time);
            Keyboard.Update(time);
        }

        protected abstract void Load();
        protected abstract void Unload(CloseReason reason);
        protected abstract void Draw(IGameTime time);

        public void Run()
        {
            this.Initialize();
            this.Load();

            RenderLoop.Run(_form, () =>
            {
                //Thread.Sleep(16);
                _gameTimeManager.Reset();
                _fpsCounter.Frame(_gameTimeManager.ElapsedTime);

                //context.ClearRenderTargetView(renderView, Color.Black);
                Update(_gameTimeManager);
                Draw(_gameTimeManager);

                swapChain.Present(0, PresentFlags.None);
            });
        }

        public void Exit()
        {
            _form.Close();
        }

        #region IDisposable

        protected bool _disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(_disposed) return;

            if (disposing)
            {
                renderView.Dispose();
                backBuffer.Dispose();
                context.ClearState();
                context.Flush();
                device.Dispose();
                context.Dispose();
                swapChain.Dispose();
                factory.Dispose();

                foreach (var item in _disposableList)
                {
                    item.Dispose();
                }
            }

            _disposed = true;
        }

        #endregion
    }
}
