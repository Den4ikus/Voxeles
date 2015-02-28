using SharpDX;

namespace VoxelEngine.Cameras
{
    public class PerspectiveCamera : BaseCamera
    {
        protected float fieldOfView;

        public float FieldOfView
        {
            get { return this.fieldOfView; }
            set
            {
                fieldOfView = value;
                needUpdateProjection = true;
                needUpdateFrustum = true;
            }
        }

        public PerspectiveCamera()
        {
            this.needUpdateView = true;
            this.needUpdateProjection = true;
            this.needUpdateFrustum = true;
        }

        public PerspectiveCamera(Vector3 position, Vector3 forward, Vector3 up, float fieldOfView, float nearPlane, float farPlane)
            :this()
        {
            this.position = position;

            this.forward = forward;
            this.up = up;
            this.right = Vector3.Cross(this.forward, this.up);
            this.up = Vector3.Cross(this.right, this.forward);
            this.forward.Normalize();
            this.right.Normalize();
            this.up.Normalize();

            this.FieldOfView = fieldOfView;
            this.nearPlane = nearPlane;
            this.farPlane = farPlane;
        }

        protected override void UpdateView()
        {
            this.view = Matrix.LookAtRH(this.position, this.position + this.forward, this.up);
        }

        protected override void UpdateProjection()
        {
            this.projection = Matrix.PerspectiveFovRH(
                this.fieldOfView,
#warning костыль
                800f/600f,
                this.nearPlane,
                this.farPlane);
        }

        protected override void UpdateFrustum()
        {
            this.frustum = new BoundingFrustum(this.view * this.projection);
        }
    }
}
