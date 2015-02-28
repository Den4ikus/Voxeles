using SharpDX;
using SharpDX.DirectInput;

namespace VoxelEngine.Cameras.Controllers
{
    public sealed class PlayerCameraController : ICameraController
    {
        public void Update(BaseCamera camera, IGameTime time)
        {
            const float velocity = 5f;
            float elapsedTime = time.ElapsedTime;

            float currentVelocity;
            if (Input.Keyboard.KeyHold(Key.LeftShift)) currentVelocity = 2f * velocity;
            else currentVelocity = velocity;

            Vector3 camMove = new Vector3();
            if (Input.Keyboard.KeyHold(Key.W)) camMove += camera.Forward * currentVelocity * elapsedTime;
            if (Input.Keyboard.KeyHold(Key.S)) camMove -= camera.Forward * currentVelocity * elapsedTime;
            if (Input.Keyboard.KeyHold(Key.A)) camMove -= camera.Right * currentVelocity * elapsedTime;
            if (Input.Keyboard.KeyHold(Key.D)) camMove += camera.Right * currentVelocity * elapsedTime;

            if (!camMove.IsZero)
                camera.Position += camMove;

            Vector2 rotationAngle = Input.Mouse.PositionDelta * -1f * elapsedTime;
            if (!rotationAngle.IsZero)
            {
                Quaternion rotation =
                    Quaternion.RotationAxis(camera.Right, rotationAngle.Y) *
                    Quaternion.RotationAxis(camera.Up, rotationAngle.X);

                camera.Forward = Vector3.Transform(camera.Forward, rotation);
                camera.Up = Vector3.Transform(camera.Up, rotation);
                camera.Right = Vector3.Normalize(Vector3.Cross(camera.Forward, camera.Up));
            }
        }
    }
}
