using SharpDX;
using VoxelEngine.Cameras.Controllers;

namespace VoxelEngine.Cameras
{
    public abstract class BaseCamera
    {
        protected Vector3 position;
        protected Vector3 forward;
        protected Vector3 up;
        protected Vector3 right;
        protected Matrix view;
        protected Matrix projection;
        protected BoundingFrustum frustum;
        protected float nearPlane;
        protected float farPlane;
        protected bool needUpdateView;
        protected bool needUpdateProjection;
        protected bool needUpdateFrustum;

        public Vector3 Position
        {
            get { return this.position; }
            set
            {
                position = value;
                needUpdateView = true;
                needUpdateFrustum = true;
            }
        }

        public Vector3 Forward
        {
            get { return this.forward; }
            set
            {
                forward = value;
                needUpdateView = true;
                needUpdateFrustum = true;
            }
        }

        public Vector3 Up
        {
            get { return this.up; }
            set
            {
                up = value;
                needUpdateView = true;
                needUpdateFrustum = true;
            }
        }

        public Vector3 Right
        {
            get { return this.right; }
            set
            {
                right = value;
                needUpdateView = true;
                needUpdateFrustum = true;
            }
        }

        public Matrix View
        {
            get
            {
                if (needUpdateView) UpdateView();
                return this.view;
            }
        }

        public Matrix Projection
        {
            get
            {
                if (needUpdateProjection) UpdateProjection();
                return this.projection;
            }
        }

        public BoundingFrustum Frustum
        {
            get
            {
                if (needUpdateFrustum)
                {
                    // т.к. Frustum зависит от матриц вида и проекции нужно их обновить если требуется
                    if (needUpdateView) UpdateView();
                    if (needUpdateProjection) UpdateProjection();
                    UpdateFrustum();
                }
                return this.frustum;
            }
        }

        public float NearPlane
        {
            get { return nearPlane; }
            set
            {
                nearPlane = value;
                needUpdateProjection = true;
                needUpdateFrustum = true;
            }
        }

        public float FarPlane
        {
            get { return farPlane; }
            set
            {
                farPlane = value;
                needUpdateProjection = true;
                needUpdateFrustum = true;
            }
        }

        public ICameraController Controller { get; set; }

        protected virtual void UpdateView()
        {
            needUpdateView = false;
        }

        protected virtual void UpdateProjection()
        {
            needUpdateProjection = false;
        }

        protected virtual void UpdateFrustum()
        {
            needUpdateFrustum = false;
        }

        public virtual void Update(IGameTime time)
        {
            if (Controller != null)
                Controller.Update(this, time);
        }
    }
}
