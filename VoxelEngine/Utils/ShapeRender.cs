using System;
using System.Collections.Generic;
using MDV.Graphics.Common;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace MDV.Graphics.Utils
{
    public static class ShapeRender
    {
        private class Shape
        {
            public VertexPositionColor[] Vertices;
            public int LineCount;
            public float LifeTime;
        }

        private static GraphicsDevice device;
        private static LineEffect effect;

        private static List<Shape> cachedShapes = new List<Shape>(32);
        private static List<Shape> activeShapes = new List<Shape>(32);

        private static VertexPositionColor[] buffer = new VertexPositionColor[128];
        private static Vector3[] corners = new Vector3[8];

        private static Vector3[] spherePoints;
        private const int sphereResolution = 20;
        private const int sphereLineCount = (sphereResolution + 1) * 3;

        private RenderMode _currentRenderMode = RenderMode.None;

        public void BeginDraw(RenderMode mode)
        {
            _currentRenderMode = mode;
        }

        public void EndDraw()
        {
            if (_currentRenderMode == RenderMode.None) throw new InvalidOperationException();


            _currentRenderMode = RenderMode.None;
        }

        public static void Initialize(ContentManager manager)
        {
            Effect e = manager.Load<Effect>("LineEffect");
            effect = new LineEffect(e);
            effect.World = Matrix.Identity;

            device = GEngine.Graphics.GraphicsDevice;

            GenerateSpherePoints();
        }

        public static void AddLine(Vector3 a, Vector3 b, Color color)
        {
            AddLine(a, b, color, 0f);
        }

        public static void AddLine(Vector3 a, Vector3 b, Color color, float lifeTime)
        {
            Shape shape = GetShape(1, lifeTime);

            shape.Vertices[0] = new VertexPositionColor(a, color);
            shape.Vertices[1] = new VertexPositionColor(b, color);
        }

        public static void AddTriangle(Vector3 a, Vector3 b, Vector3 c, Color color)
        {
            AddTriangle(a, b, c, color, 0f);
        }

        public static void AddTriangle(Vector3 a, Vector3 b, Vector3 c, Color color, float lifeTime)
        {
            Shape shape = GetShape(3, lifeTime);

            shape.Vertices[0] = new VertexPositionColor(a, color);
            shape.Vertices[1] = new VertexPositionColor(b, color);
            shape.Vertices[2] = new VertexPositionColor(b, color);
            shape.Vertices[3] = new VertexPositionColor(c, color);
            shape.Vertices[4] = new VertexPositionColor(c, color);
            shape.Vertices[5] = new VertexPositionColor(a, color);
        }

        public static void AddBox(BoundingBox b, Color color)
        {
            AddBox(b, color, 0f);
        }

        public static void AddBox(BoundingBox b, Color color, float lifeTime)
        {
            Shape shape = GetShape(12, lifeTime);

            b.GetCorners(corners);

            // Верхняя сторона
            shape.Vertices[0] = new VertexPositionColor(corners[0], color);
            shape.Vertices[1] = new VertexPositionColor(corners[1], color);
            shape.Vertices[2] = new VertexPositionColor(corners[1], color);
            shape.Vertices[3] = new VertexPositionColor(corners[2], color);
            shape.Vertices[4] = new VertexPositionColor(corners[2], color);
            shape.Vertices[5] = new VertexPositionColor(corners[3], color);
            shape.Vertices[6] = new VertexPositionColor(corners[3], color);
            shape.Vertices[7] = new VertexPositionColor(corners[0], color);

            //Нижняя сторона
            shape.Vertices[8] = new VertexPositionColor(corners[4], color);
            shape.Vertices[9] = new VertexPositionColor(corners[5], color);
            shape.Vertices[10] = new VertexPositionColor(corners[5], color);
            shape.Vertices[11] = new VertexPositionColor(corners[6], color);
            shape.Vertices[12] = new VertexPositionColor(corners[6], color);
            shape.Vertices[13] = new VertexPositionColor(corners[7], color);
            shape.Vertices[14] = new VertexPositionColor(corners[7], color);
            shape.Vertices[15] = new VertexPositionColor(corners[4], color);

            //Вертикальные стороны
            shape.Vertices[16] = new VertexPositionColor(corners[0], color);
            shape.Vertices[17] = new VertexPositionColor(corners[4], color);
            shape.Vertices[18] = new VertexPositionColor(corners[1], color);
            shape.Vertices[19] = new VertexPositionColor(corners[5], color);
            shape.Vertices[20] = new VertexPositionColor(corners[2], color);
            shape.Vertices[21] = new VertexPositionColor(corners[6], color);
            shape.Vertices[22] = new VertexPositionColor(corners[3], color);
            shape.Vertices[23] = new VertexPositionColor(corners[7], color);
        }

        public static void AddFrustum(BoundingFrustum f, Color color)
        {
            AddFrustum(f, color, 0f);
        }

        public static void AddFrustum(BoundingFrustum f, Color color, float lifeTime)
        {
            Shape shape = GetShape(12, lifeTime);

            f.GetCorners(corners);

            // Верхняя сторона
            shape.Vertices[0] = new VertexPositionColor(corners[0], color);
            shape.Vertices[1] = new VertexPositionColor(corners[1], color);
            shape.Vertices[2] = new VertexPositionColor(corners[1], color);
            shape.Vertices[3] = new VertexPositionColor(corners[2], color);
            shape.Vertices[4] = new VertexPositionColor(corners[2], color);
            shape.Vertices[5] = new VertexPositionColor(corners[3], color);
            shape.Vertices[6] = new VertexPositionColor(corners[3], color);
            shape.Vertices[7] = new VertexPositionColor(corners[0], color);

            //Нижняя сторона
            shape.Vertices[8] = new VertexPositionColor(corners[4], color);
            shape.Vertices[9] = new VertexPositionColor(corners[5], color);
            shape.Vertices[10] = new VertexPositionColor(corners[5], color);
            shape.Vertices[11] = new VertexPositionColor(corners[6], color);
            shape.Vertices[12] = new VertexPositionColor(corners[6], color);
            shape.Vertices[13] = new VertexPositionColor(corners[7], color);
            shape.Vertices[14] = new VertexPositionColor(corners[7], color);
            shape.Vertices[15] = new VertexPositionColor(corners[4], color);

            //Вертикальные стороны
            shape.Vertices[16] = new VertexPositionColor(corners[0], color);
            shape.Vertices[17] = new VertexPositionColor(corners[4], color);
            shape.Vertices[18] = new VertexPositionColor(corners[1], color);
            shape.Vertices[19] = new VertexPositionColor(corners[5], color);
            shape.Vertices[20] = new VertexPositionColor(corners[2], color);
            shape.Vertices[21] = new VertexPositionColor(corners[6], color);
            shape.Vertices[22] = new VertexPositionColor(corners[3], color);
            shape.Vertices[23] = new VertexPositionColor(corners[7], color);
        }

        public static void AddSphere(BoundingSphere s, Color color)
        {
            AddSphere(s, color, 0f);
        }

        public static void AddSphere(BoundingSphere s, Color color, float lifeTime)
        {
            Shape shape = GetShape(sphereLineCount, lifeTime);

            for (int i = 0; i < spherePoints.Length; i++)
            {
                Vector3 vertex = spherePoints[i] * s.Radius + s.Center;
                shape.Vertices[i] = new VertexPositionColor(vertex, color);
            }
        }

        public static void Render(IGameTime time)
        {
            Matrix view = GEngine.Camera.View;
            Matrix projection = GEngine.Camera.Projection;

            int totalVertCount = 0;
            for (int i = 0; i < activeShapes.Count; i++)
            {
                totalVertCount += activeShapes[i].Vertices.Length;
            }

            if (totalVertCount > 0)
            {
                if (buffer.Length < totalVertCount)
                {
                    buffer = new VertexPositionColor[totalVertCount * 2];
                }

                int totalLineCount = 0;
                int bufferIndex = 0;
                for (int i = 0; i < activeShapes.Count; i++)
                {
                    Shape s = activeShapes[i];
                    totalLineCount += s.LineCount;
                    int vertCount = s.LineCount * 2;
                    for (int j = 0; j < vertCount; j++)
                    {
                        buffer[bufferIndex++] = s.Vertices[j];
                    }
                }

                device.VertexDeclaration = new VertexDeclaration(device, VertexPositionColor.VertexElements);

                effect.View = view;
                effect.Projection = projection;
                effect.Apply();

                effect.Begin();
                effect.BeginPass(0);

                device.DrawUserPrimitives(PrimitiveType.LineList, buffer, 0, totalLineCount);

                effect.EndPass(0);
                effect.End();

                bool needSort = false;
                for (int i = 0; i < activeShapes.Count; i++)
                {
                    Shape s = activeShapes[i];
                    s.LifeTime -= (float)time.ElapsedGameTime.TotalSeconds;
                    if (s.LifeTime <= 0)
                    {
                        activeShapes.RemoveAt(i);
                        cachedShapes.Add(s);
                        needSort = true;
                    }
                }

                if (needSort) cachedShapes.Sort(CachedListComparator);
            }
        }

        private static int CachedListComparator(Shape x, Shape y)
        {
            return x.Vertices.Length - y.Vertices.Length;
        }

        private static Shape GetShape(int lineCount, float lifeTime)
        {
            Shape shape = null;

            int vertCount = lineCount * 2;
            for (int i = 0; i < cachedShapes.Count; i++)
            {
                if (cachedShapes[i].Vertices.Length >= vertCount)
                {
                    shape = cachedShapes[i];
                    cachedShapes.RemoveAt(i);
                    break;
                }
            }

            if (shape == null)
            {
                shape = new Shape() { Vertices = new VertexPositionColor[vertCount] };
            }

            shape.LineCount = lineCount;
            shape.LifeTime = lifeTime;
            activeShapes.Add(shape);

            return shape;
        }

        private static void GenerateSpherePoints()
        {
            spherePoints = new Vector3[sphereLineCount * 2];

            float stepAngle = MathHelper.TwoPi / sphereResolution;

            int index = 0;
            //Круг в плоскости XY
            for (float a = 0f; a < MathHelper.TwoPi; a += stepAngle)
            {
                spherePoints[index++] = new Vector3((float)Math.Cos(a), (float)Math.Sin(a), 0f);
                spherePoints[index++] = new Vector3((float)Math.Cos(a + stepAngle), (float)Math.Sin(a + stepAngle), 0f);
            }
            //Круг в плоскости XZ
            for (float a = 0f; a < MathHelper.TwoPi; a += stepAngle)
            {
                spherePoints[index++] = new Vector3((float)Math.Cos(a), 0f, (float)Math.Sin(a));
                spherePoints[index++] = new Vector3((float)Math.Cos(a + stepAngle), 0f, (float)Math.Sin(a + stepAngle));
            }
            //Круг в плоскости YZ
            for (float a = 0f; a < MathHelper.TwoPi; a += stepAngle)
            {
                spherePoints[index++] = new Vector3(0f, (float)Math.Cos(a), (float)Math.Sin(a));
                spherePoints[index++] = new Vector3(0f, (float)Math.Cos(a + stepAngle), (float)Math.Sin(a + stepAngle));
            }
        }
    }
}
