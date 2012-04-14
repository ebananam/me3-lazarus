#region File Description
//-----------------------------------------------------------------------------
// SpinningTriangleControl.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using ME2Vector = Gibbed.MassEffect2.FileFormats.Save.Vector;
using ME3Vector = Gibbed.MassEffect3.FileFormats.Save.Vector;
using System;

#endregion

namespace Pixelmade.Lazarus
{
    /// <summary>
    /// Example control inherits from GraphicsDeviceControl, which allows it to
    /// render using a GraphicsDevice. This control shows how to draw animating
    /// 3D graphics inside a WinForms application. It hooks the Application.Idle
    /// event, using this to invalidate the control, which will cause the animation
    /// to constantly redraw.
    /// </summary>
    class LodViewerControl : GraphicsDeviceControl
    {
        public float POINT_SIZE = 0.08f, SELECTED_POINT_SIZE = 0.06f;
        BasicEffect effect;
        Stopwatch timer;
        VertexPositionColor[] vertexArray, thresholdArray, mapArray;
        float viewAngle, viewDistance, viewAngleVert = MathHelper.PiOver2;
        Vector3 lookAt = Vector3.Zero;
        Vector3 cameraOffset = Vector3.Zero;
        int minVertex;
        VertexMapping mapping;
        bool inverseMapping;
        string mode = "Normal";
        int pickX, pickY, pickIndex;
        List<int> lastPickSet = new List<int>();

        #region Properties
        public float ViewAngle
        {
            get { return viewAngle; }
            set
            {
                viewAngle = value % MathHelper.TwoPi;
            }
        }
        public float ViewDistance
        {
            get { return viewDistance; }
            set
            {
                viewDistance = value;
                if (viewDistance < 0.5f) viewDistance = 0.5f;
                else if (viewDistance > 50f) viewDistance = 50f;
            }
        }
        public float ViewAngleVert
        {
            get { return viewAngleVert; }
            set
            {
                viewAngleVert = value;
                if (viewAngleVert < 0.01f) viewAngleVert = 0.01f;
                else if (viewAngleVert > MathHelper.Pi - 0.01f) viewAngleVert = MathHelper.Pi - 0.01f;
            }
        }
        public int MinVertex
        {
            get { return minVertex; }
            set { minVertex = value; }
        }
        public Vector3 CameraOffset { get { return cameraOffset; } set { cameraOffset = value; } }
        public string Mode { get { return mode; } set { mode = value; } }
        #endregion

        public void SetData(List<ME2Vector> me2Data)
        {
            if (me2Data != null)
            {
                List<VertexPositionColor> vertices = new List<VertexPositionColor>();

                float color = 0f; float colorDelta = 1 / (float)me2Data.Count;
                foreach (ME2Vector vec in me2Data)
                {
                    Vector3 v = new Vector3(vec.X, vec.Y, vec.Z);
                    Color c = new Color(1 - color, 0f, color);
                    vertices.Add(new VertexPositionColor(v + Vector3.UnitZ * POINT_SIZE, c));
                    vertices.Add(new VertexPositionColor(v - Vector3.UnitZ * POINT_SIZE - Vector3.UnitX * POINT_SIZE, c));
                    vertices.Add(new VertexPositionColor(v - Vector3.UnitZ * POINT_SIZE + Vector3.UnitX * POINT_SIZE, c));
                    vertices.Add(new VertexPositionColor(v + Vector3.UnitZ * POINT_SIZE, c));
                    vertices.Add(new VertexPositionColor(v - Vector3.UnitZ * POINT_SIZE - Vector3.UnitY * POINT_SIZE, c));
                    vertices.Add(new VertexPositionColor(v - Vector3.UnitZ * POINT_SIZE + Vector3.UnitY * POINT_SIZE, c));
                    color += colorDelta;
                }
                vertexArray = vertices.ToArray();
            }
        }
        public void SetData(List<ME3Vector> me3Data)
        {
            if (me3Data != null)
            {
                List<VertexPositionColor> vertices = new List<VertexPositionColor>();

                float color = 0f; float colorDelta = 1 / (float)me3Data.Count;
                foreach (ME3Vector vec in me3Data)
                {
                    Vector3 v = new Vector3(vec.X, vec.Y, vec.Z);
                    Color c = new Color(1 - color, 0f, color);
                    vertices.Add(new VertexPositionColor(v + Vector3.UnitZ * POINT_SIZE, c));
                    vertices.Add(new VertexPositionColor(v - Vector3.UnitZ * POINT_SIZE - Vector3.UnitX * POINT_SIZE, c));
                    vertices.Add(new VertexPositionColor(v - Vector3.UnitZ * POINT_SIZE + Vector3.UnitX * POINT_SIZE, c));
                    vertices.Add(new VertexPositionColor(v + Vector3.UnitZ * POINT_SIZE, c));
                    vertices.Add(new VertexPositionColor(v - Vector3.UnitZ * POINT_SIZE - Vector3.UnitY * POINT_SIZE, c));
                    vertices.Add(new VertexPositionColor(v - Vector3.UnitZ * POINT_SIZE + Vector3.UnitY * POINT_SIZE, c));
                    color += colorDelta;
                }
                vertexArray = vertices.ToArray();
            }
        }
        public void SetMapData(VertexMapping mapping, bool inverse)
        {
            this.mapping = mapping;
            inverseMapping = inverse;

            if (!inverse)
            {
                // Threshold array creation..
                thresholdArray = (VertexPositionColor[])vertexArray.Clone();
                for (int i = 0; i < mapping.Thresholds.Length; i++)
                {
                    float t = (mapping.Thresholds[i] - mapping.MinThreshold) / mapping.MaxThreshold;
                    Color c = new Color(t, t, t);
                    SetVertexColor(thresholdArray, i, c);
                }
            }

            RefreshMapData();
        }
        public void RefreshMapData()
        {
            if (mapping == null) return;

            if (!inverseMapping)
            {
                for (int i = 0; i < mapping.Mapping.Length; i++)
                {
                    if (mapping.Ignore[i]) SetVertexIgnored(i, false);
                    else if (mapping.Verified[i]) SetVertexVerified(i, false);
                    else RollBackColor(i, false);
                }
            }
            else
            {
                for (int i = 0; i < vertexArray.Length / 6; i++) RollBackColor(i, false);
                for (int i = 0; i < mapping.Mapping.Length; i++)
                {
                    if (mapping.Ignore[i]) SetVertexIgnored(i, true);
                    else if (mapping.Verified[i]) SetVertexVerified(i, true);
                }
            }
        }
        public void RollBackColor(int index, bool inverse)
        {
            if (!inverse)
            {
                float color = index / (float)(vertexArray.Length / 6);

                SetVertexColor(vertexArray, index, new Color(1 - color, 0f, color));
            }
            else
            {
                foreach (int v in mapping.Mapping[index])
                {
                    float color = v / (float)(vertexArray.Length / 6);
                    SetVertexColor(vertexArray, v, new Color(1 - color, 0f, color));
                }
            }
        }
        public void SetMapSelection(int index, int range)
        {
            if (mapping == null) return;

            int start = index - range / 2;
            int end = index + range / 2 + 1;

            if (start < 0) start = 0;
            if (end > 2232) end = 2232;

            List<VertexPositionColor> vertices = new List<VertexPositionColor>();
            for (int i = start; i < end; i++)
            {
                if (!inverseMapping)
                {
                    Vector3 v = vertexArray[i * 6].Position;
                    v -= Vector3.UnitZ * POINT_SIZE;

                    vertices.Add(new VertexPositionColor(v + Vector3.UnitZ * SELECTED_POINT_SIZE, Color.Green));
                    vertices.Add(new VertexPositionColor(v - Vector3.UnitZ * SELECTED_POINT_SIZE - Vector3.UnitX * SELECTED_POINT_SIZE, Color.Green));
                    vertices.Add(new VertexPositionColor(v - Vector3.UnitZ * SELECTED_POINT_SIZE + Vector3.UnitX * SELECTED_POINT_SIZE, Color.Green));
                    vertices.Add(new VertexPositionColor(v + Vector3.UnitZ * SELECTED_POINT_SIZE, Color.Green));
                    vertices.Add(new VertexPositionColor(v - Vector3.UnitZ * SELECTED_POINT_SIZE - Vector3.UnitY * SELECTED_POINT_SIZE, Color.Green));
                    vertices.Add(new VertexPositionColor(v - Vector3.UnitZ * SELECTED_POINT_SIZE + Vector3.UnitY * SELECTED_POINT_SIZE, Color.Green));
                }
                else
                {
                    foreach (int j in mapping.Mapping[i])
                    {
                        Vector3 v = vertexArray[j * 6].Position;
                        v -= Vector3.UnitZ * POINT_SIZE;

                        vertices.Add(new VertexPositionColor(v + Vector3.UnitZ * SELECTED_POINT_SIZE, Color.Green));
                        vertices.Add(new VertexPositionColor(v - Vector3.UnitZ * SELECTED_POINT_SIZE - Vector3.UnitX * SELECTED_POINT_SIZE, Color.Green));
                        vertices.Add(new VertexPositionColor(v - Vector3.UnitZ * SELECTED_POINT_SIZE + Vector3.UnitX * SELECTED_POINT_SIZE, Color.Green));
                        vertices.Add(new VertexPositionColor(v + Vector3.UnitZ * SELECTED_POINT_SIZE, Color.Green));
                        vertices.Add(new VertexPositionColor(v - Vector3.UnitZ * SELECTED_POINT_SIZE - Vector3.UnitY * SELECTED_POINT_SIZE, Color.Green));
                        vertices.Add(new VertexPositionColor(v - Vector3.UnitZ * SELECTED_POINT_SIZE + Vector3.UnitY * SELECTED_POINT_SIZE, Color.Green));
                    }
                }
            }

            if (vertices.Count > 0) mapArray = vertices.ToArray();
            else mapArray = null;
        }
        public void SetVertexColor(VertexPositionColor[] array, int index, Color color)
        {
            array[index * 6].Color = color;
            array[index * 6 + 1].Color = color;
            array[index * 6 + 2].Color = color;
            array[index * 6 + 3].Color = color;
            array[index * 6 + 4].Color = color;
            array[index * 6 + 5].Color = color;
        }
        public void SetVertexIgnored(int index, bool inverse)
        {
            if (!inverse) SetVertexColor(vertexArray, index, Color.Gray);
            else
            {
                foreach (int v in mapping.Mapping[index])
                {
                    SetVertexColor(vertexArray, v, Color.Gray);
                }
            }
        }
        public void SetVertexVerified(int index, bool inverse)
        {
            float color = index / (float)(mapping.Mapping.Length);
            if (!inverse)
            {
                SetVertexColor(vertexArray, index, new Color(1f - color, 1f, color));
            }
            else
            {
                foreach (int v in mapping.Mapping[index])
                {
                    SetVertexColor(vertexArray, v, new Color(1f - color, 1f, color));
                }
            }
        }
        public int Pick(int x, int y)
        {
            if (vertexArray == null) return -1;

            Vector3 nearsource = new Vector3((float)x, (float)y, 0f);
            Vector3 farsource = new Vector3((float)x, (float)y, 1f);
            Matrix world = Matrix.CreateTranslation(0, 0, 0);
            Vector3 nearPoint = GraphicsDevice.Viewport.Unproject(nearsource, effect.Projection, effect.View, effect.World);
            Vector3 farPoint = GraphicsDevice.Viewport.Unproject(farsource, effect.Projection, effect.View, effect.World);
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            Ray ray = new Ray(nearPoint, direction);

            List<int> pickSet = new List<int>();
            for (int i = 0; i < vertexArray.Length / 6; i++)
            {
                Vector3 v = vertexArray[i * 6].Position;
                v -= Vector3.UnitZ * POINT_SIZE;

                float? dist = ray.Intersects(new BoundingSphere(v, POINT_SIZE));

                if (dist != null && dist < viewDistance * 1.75f) pickSet.Add(i);
            }

            if (pickSet.Count > 0)
            {
                if (pickSet.Count == lastPickSet.Count)
                {
                    bool same = true;
                    for (int i = 0; i < pickSet.Count; i++) if (pickSet[i] != lastPickSet[i]) { same = false; break; }

                    if (same)
                    {
                        return pickSet[++pickIndex % pickSet.Count];
                    }
                }
                pickX = x; pickY = y; pickIndex = 0;
                lastPickSet = pickSet;
                return pickSet[0];
            }
            else
            {
                lastPickSet = pickSet;
                return -1;
            }
        }

        public void OffsetView(float horizontal, float vertical)
        {
            Vector3 camDir = new Vector3((float)Math.Cos(viewAngle), (float)Math.Sin(viewAngle), 0);
            Vector3 sideDir = Vector3.Cross(camDir, Vector3.UnitZ);

            cameraOffset += sideDir * horizontal * viewDistance / 20f;
            cameraOffset.Z += vertical * viewDistance / 20f;
        }

        public void Focus(Vector3 relVertexPos)
        {
            cameraOffset = Vector3.Zero;
            lookAt = Vector3.Transform(relVertexPos, effect.World);
        }

        public void IncreasePointSize(List<ME2Vector> data)
        {
            POINT_SIZE *= 2f;
            SELECTED_POINT_SIZE *= 2f;

            SetData(data);
        }
        public void DecreasePointSize(List<ME2Vector> data)
        {
            POINT_SIZE /= 2f;
            SELECTED_POINT_SIZE /= 2f;

            SetData(data);
        }
        public void IncreasePointSize(List<ME3Vector> data)
        {
            POINT_SIZE *= 2f;
            SELECTED_POINT_SIZE *= 2f;

            SetData(data);
        }
        public void DecreasePointSize(List<ME3Vector> data)
        {
            POINT_SIZE /= 2f;
            SELECTED_POINT_SIZE /= 2f;

            SetData(data);
        }

        /// <summary>
        /// Initializes the control.
        /// </summary>
        protected override void Initialize()
        {
            // Create our effect.
            effect = new BasicEffect(GraphicsDevice);
            effect.VertexColorEnabled = true;

            effect.FogEnabled = true;
            effect.FogColor = Color.Black.ToVector3();

            // Start the animation timer.
            timer = Stopwatch.StartNew();

            // Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { Invalidate(); };
        }

        /// <summary>
        /// Draws the control.
        /// </summary>
        protected override void Draw()
        {
            GraphicsDevice.Clear(Color.Black);

            //float time = (float)timer.Elapsed.TotalSeconds / 10f;

            // Generate geometry.
            if (vertexArray == null) return;

            // Configure viewports
            float aspect = GraphicsDevice.Viewport.AspectRatio;

            // Set projection matrices
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), aspect, 0.1f, 100f);

            effect.FogStart = viewDistance;
            effect.FogEnd = viewDistance * 2;

            //Vector3 camOffset = new Vector3((float)Math.Cos(viewAngle), (float)Math.Sin(viewAngle), 0);
            Vector3 camOffset = new Vector3();
            camOffset.X = viewDistance * (float)Math.Cos(viewAngle) * (float)Math.Sin(viewAngleVert);
            camOffset.Y = viewDistance * (float)Math.Sin(viewAngle) * (float)Math.Sin(viewAngleVert);
            camOffset.Z = viewDistance * (float)Math.Cos(viewAngleVert);
            effect.View = Matrix.CreateLookAt(lookAt + camOffset + cameraOffset, lookAt + cameraOffset, Vector3.UnitZ);

            VertexPositionColor[] array;
            if (mode == "Thresholds" && !inverseMapping) array = thresholdArray;
            else array = vertexArray;

            // Draw Face
            if (array != null)
            {
                effect.World = Matrix.CreateWorld(new Vector3(0, 0, -165), -Vector3.UnitZ, Vector3.UnitY) * Matrix.CreateReflection(new Plane(0,1,0,0));
                GraphicsDevice.RasterizerState = RasterizerState.CullNone;

                effect.CurrentTechnique.Passes[0].Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, array, minVertex * 6, (array.Length - minVertex * 6) / 3);
            }

            if (mode == "Mapping" && mapArray != null)
            {
                GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.Black, float.MaxValue, 0);
                GraphicsDevice.RasterizerState = RasterizerState.CullNone;
                effect.CurrentTechnique.Passes[0].Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, mapArray, 0, mapArray.Length / 3);
            }
        }
    }
}