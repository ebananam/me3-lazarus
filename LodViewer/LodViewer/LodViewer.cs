using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Gibbed.MassEffect3.FileFormats.Save;
using System.IO;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Threading;
using Gibbed.IO;
using Gibbed.MassEffect3.FileFormats;

namespace LodViewer
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class LodViewer : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        List<Vector> input;
        List<Gibbed.MassEffect2.FileFormats.Save.Vector> input2;

        VertexPositionColor[] data, data2;
        VertexBuffer vb, vb2;
        BasicEffect effect;
        Matrix world, world2, view, proj;
        Vector3 camPos;
        float t, t2;
        SpriteFont font1, font2;

        Viewport viewPort, viewPort2;

        public LodViewer(List<Vector> input, List<Gibbed.MassEffect2.FileFormats.Save.Vector> input2)
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferMultiSampling = true;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            //this.input = input;
            //this.input2 = input2;

            this.input = LoadSaveFromStream(File.OpenRead("C:\\Users\\Pedro Madeira\\Documents\\BioWare\\Mass Effect 3\\Save\\Naomi_12_Sentinel_210312_015c769\\AutoSave.pcsav")).Player.Appearance.MorphHead.Lod0Vertices;
            this.input2 = Gibbed.MassEffect2.FileFormats.SaveFile.Load(File.OpenRead("C:\\Users\\Pedro Madeira\\Documents\\BioWare\\Mass Effect 2\\Save\\Naomi_12_Sentinel_150212\\AutoSave.pcsav")).PlayerRecord.Appearance.MorphHead.LOD0Vertices;
            

            t = 0;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font1 = Content.Load<SpriteFont>("SpriteFont1");
            font2 = Content.Load<SpriteFont>("SpriteFont2");

            viewPort = viewPort2 = GraphicsDevice.Viewport;
            viewPort.Width = viewPort2.Width /= 2;
            viewPort.X = viewPort2.Width;

            float delta = .1f;
            float cdelta = 0f;
            // TODO: use this.Content to load your game content here
            {
                vb = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), input.Count, BufferUsage.None);
                List<VertexPositionColor> data = new List<VertexPositionColor>();

                foreach (Vector v in input)
                {
                    Vector3 v2 = new Vector3(v.X, v.Y, v.Z);

                    Color c = new Color(1 - cdelta, 0f, cdelta);

                    data.Add(new VertexPositionColor(v2 + Vector3.UnitZ * delta, c));
                    data.Add(new VertexPositionColor(v2 - Vector3.UnitZ * delta - Vector3.UnitX * delta, c));
                    data.Add(new VertexPositionColor(v2 - Vector3.UnitZ * delta + Vector3.UnitX * delta, c));

                    data.Add(new VertexPositionColor(v2 + Vector3.UnitZ * delta, c));
                    data.Add(new VertexPositionColor(v2 - Vector3.UnitZ * delta - Vector3.UnitY * delta, c));
                    data.Add(new VertexPositionColor(v2 - Vector3.UnitZ * delta + Vector3.UnitY * delta, c));

                    cdelta += 1 / (float)input.Count;
                }
                this.data = data.ToArray();
            }

            cdelta = 0f;
            {
                vb2 = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), input2.Count, BufferUsage.None);
                List<VertexPositionColor> data2 = new List<VertexPositionColor>();

                foreach (Gibbed.MassEffect2.FileFormats.Save.Vector v in input2)
                {
                    Vector3 v2 = new Vector3(v.X, v.Y, v.Z);

                    Color c = new Color(1 - cdelta, 0f, cdelta);

                    data2.Add(new VertexPositionColor(v2 + Vector3.UnitZ * delta, c));
                    data2.Add(new VertexPositionColor(v2 - Vector3.UnitZ * delta - Vector3.UnitX * delta, c));
                    data2.Add(new VertexPositionColor(v2 - Vector3.UnitZ * delta + Vector3.UnitX * delta, c));

                    data2.Add(new VertexPositionColor(v2 + Vector3.UnitZ * delta, c));
                    data2.Add(new VertexPositionColor(v2 - Vector3.UnitZ * delta - Vector3.UnitY * delta, c));
                    data2.Add(new VertexPositionColor(v2 - Vector3.UnitZ * delta + Vector3.UnitY * delta, c));

                    cdelta += 1 / (float)input.Count;
                }
                this.data2 = data2.ToArray();
            }

            effect = new BasicEffect(GraphicsDevice);
            effect.VertexColorEnabled = true;
            effect.AmbientLightColor = Color.White.ToVector3();
            effect.DiffuseColor = Color.White.ToVector3();
            proj = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), GraphicsDevice.Viewport.AspectRatio/2, 1f, 1000f);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            t += 0.00f;
            t2 += 0.01f;

            camPos = new Vector3();
            camPos.X = (float)Math.Sin((double)t);
            camPos.Y = (float)Math.Cos((double)t);
            camPos.Z = 0;
            camPos *= 50;

            world = Matrix.CreateRotationZ(t2) * Matrix.CreateWorld(new Vector3(0, 0, -165), -Vector3.UnitZ, Vector3.UnitX);
            world2 = Matrix.CreateRotationZ(t2) * Matrix.CreateWorld(new Vector3(0, 0, -165), -Vector3.UnitZ, Vector3.UnitX);
            view = Matrix.CreateLookAt(camPos, Vector3.Zero, Vector3.UnitZ);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            GraphicsDevice.RasterizerState = new RasterizerState() { CullMode = CullMode.None };
            GraphicsDevice.Viewport = viewPort;
            effect.World = world;
            effect.View = view;
            effect.Projection = proj;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                //GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, vb.VertexCount);
                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, data, 0, data.Length / 3);
            }
            spriteBatch.Begin();
            spriteBatch.DrawString(font1, "Mass Effect 3", new Vector2(viewPort.Width / 2, 40), Color.White,
                0, font1.MeasureString("Mass Effect 3") / 2, 1.0f, SpriteEffects.None, 0.5f);
            spriteBatch.DrawString(font2, "LOD0 Vertices: " + input.Count, new Vector2(viewPort.Width / 2, viewPort.Height - 40), Color.White,
                0, font2.MeasureString("LOD0 Vertices: " + input.Count) / 2, 1.0f, SpriteEffects.None, 0.5f);
            spriteBatch.End();

            GraphicsDevice.RasterizerState = new RasterizerState() { CullMode = CullMode.None };
            GraphicsDevice.Viewport = viewPort2;
            effect.World = world2;
            effect.View = view;
            effect.Projection = proj;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                //GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, vb.VertexCount);
                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, data2, 0, data2.Length / 3);
            }
            spriteBatch.Begin();
            spriteBatch.DrawString(font1, "Mass Effect 2", new Vector2(viewPort.Width / 2, 40), Color.White,
                0, font1.MeasureString("Mass Effect 2") / 2, 1.0f, SpriteEffects.None, 0.5f);
            spriteBatch.DrawString(font2, "LOD0 Vertices: " + input2.Count, new Vector2(viewPort.Width / 2, viewPort.Height - 40), Color.White,
                0, font2.MeasureString("LOD0 Vertices: " + input2.Count) / 2, 1.0f, SpriteEffects.None, 0.5f);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private SFXSaveGameFile LoadSaveFromStream(Stream stream)
        {
            if (stream.ReadValueU32(Endian.Big) == 0x434F4E20)
            {
                return null;
            }
            stream.Seek(-4, SeekOrigin.Current);

            Gibbed.MassEffect3.FileFormats.SFXSaveGameFile saveFile;
            try
            {
                saveFile = Gibbed.MassEffect3.FileFormats.SFXSaveGameFile.Read(stream);
            }
            catch (Exception e)
            {
                return null;
            }

            return saveFile;
        }
    }
}
