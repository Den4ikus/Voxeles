using System;
using System.Windows.Forms;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DirectInput;
using VoxelEngine;
using VoxelEngine.Cameras;
using VoxelEngine.Cameras.Controllers;
using Buffer = SharpDX.Direct3D11.Buffer;
using Keyboard = VoxelEngine.Input.Keyboard;
using MapFlags = SharpDX.Direct3D11.MapFlags;
using Resource = SharpDX.Direct3D11.Resource;

namespace Program
{
    public class MyGame : BaseGame
    {
        private ShaderBytecode vertexShaderByteCode;
        private ShaderBytecode pixelShaderByteCode;

        private VertexShader vertexShader;
        private PixelShader pixelShader;

        private Resource skybox;
        private ShaderResourceView skyboxResource;

        private InputLayout layout;
        private Buffer vertices;

        private PerspectiveCamera cam = new PerspectiveCamera(10 * Vector3.BackwardRH, Vector3.ForwardRH, Vector3.Up, (float)Math.PI / 4.0f, 1f, 100f);

        protected override void Initialize()
        {
            base.Initialize();

            cam.Controller = new PlayerCameraController();

            vertices = new Buffer(device, 32 * 4, ResourceUsage.Dynamic, BindFlags.VertexBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
        }

        protected override void Load()
        {
            vertexShaderByteCode = ShaderBytecode.CompileFromFile("MiniTri.hlsl", "VS", "vs_4_0");
            vertexShader = new VertexShader(device, vertexShaderByteCode);

            pixelShaderByteCode = ShaderBytecode.CompileFromFile("MiniTri.hlsl", "PS", "ps_4_0");
            pixelShader = new PixelShader(device, pixelShaderByteCode);

            //skyboxShaderByteCode = ShaderBytecode.CompileFromFile("MiniTri.hlsl", "PS_Skybox", "ps_4_0");
            //skyboxPS = new PixelShader(device, skyboxShaderByteCode);

            // Layout from VertexShader input signature
            layout = new InputLayout(
                device,
                ShaderSignature.GetInputSignature(vertexShaderByteCode),
                new[]
                    {
                        new InputElement("POSITION", 0, Format.R32G32_Float, 0, 0),
                        new InputElement("TEXCOORD", 0, Format.R32G32B32_Float, 8, 0),
                        new InputElement("TEXCOORD", 1, Format.R32G32B32_Float, 20, 0) 
                    });

            skybox = Texture2D.FromFile(device, "skybox.dds", new ImageLoadInformation()
                {
                    Width = -1,
                    Height = -1,
                    Depth = 1,
                    FirstMipLevel = 0,
                    MipLevels = 1,
                    Usage = ResourceUsage.Immutable,
                    BindFlags = BindFlags.ShaderResource,
                    CpuAccessFlags = CpuAccessFlags.None,
                    OptionFlags = ResourceOptionFlags.TextureCube,
                    Format = Format.R8G8B8A8_UNorm,
                    Filter = FilterFlags.None,
                    MipFilter = FilterFlags.None,
                    PSrcInfo = IntPtr.Zero
                });

            skyboxResource = new ShaderResourceView(device, skybox, new ShaderResourceViewDescription()
                {
                    Dimension = ShaderResourceViewDimension.TextureCube,
                    Format = Format.R8G8B8A8_UNorm,
                    TextureCube = new ShaderResourceViewDescription.TextureCubeResource()
                        {
                            MipLevels = 1,
                            MostDetailedMip = 0
                        }
                });

            // Prepare All the stages
            context.InputAssembler.InputLayout = layout;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertices, 32, 0));
            context.VertexShader.Set(vertexShader);
            context.PixelShader.SetShaderResource(0, skyboxResource);
        }

        protected override void Unload(CloseReason reason)
        {
        }

        protected override void Update(IGameTime time)
        {
            base.Update(time);

            cam.Update(time);

            if (Keyboard.KeyDown(Key.LeftAlt))
            {
                _form.Text = _fpsCounter.FPS.ToString();
                //System.Diagnostics.Debug.WriteLine(_fpsCounter.FPS.ToString());
            }

            if (Keyboard.KeyDown(Key.Escape))
            {
                Exit();
            }

        }

        protected override void Draw(IGameTime time)
        {
            DataStream dataStream;
            context.MapSubresource(vertices, MapMode.WriteDiscard, MapFlags.None, out dataStream);

            Vector3[] corners = cam.Frustum.GetCorners();
            dataStream.WriteRange(new []
                    {
                       new VertexPositionRay(new Vector2(-1f, 1f), cam.Position, corners[2]),
                       new VertexPositionRay(Vector2.One, cam.Position, corners[1]),
                       new VertexPositionRay(-Vector2.One, cam.Position, corners[3]),
                       new VertexPositionRay(new Vector2(1f, -1f), cam.Position, corners[0]),
                    });

            context.UnmapSubresource(vertices, 0);
            dataStream.Dispose();

            //context.PixelShader.Set(skyboxPS);
            //context.Draw(4, 0);
            context.PixelShader.Set(pixelShader);
            context.Draw(4, 0);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                layout.Dispose();

                pixelShaderByteCode.Dispose();
                pixelShader.Dispose();
                vertexShaderByteCode.Dispose();
                vertexShader.Dispose();

                vertices.Dispose();

                skyboxResource.Dispose();
                skybox.Dispose();
            }
        }
    }
}
