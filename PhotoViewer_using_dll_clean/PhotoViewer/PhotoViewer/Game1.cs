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
using dflip;
using System.Windows.Forms;
using System.Reflection;
namespace PhotoViewer
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public static string Title = "D-Flip: Dynamic & Flexible Interactive Photoshow";
        GraphicsDeviceManager graphics;
        //SpriteBatch spriteBatch;
        KeyboardDevice keyboard = new KeyboardDevice();
        dflip.SystemParameter mainProcess;
        string profilePath;

        public Game1(string[] args)
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1366;
            graphics.PreferredBackBufferHeight = 768;

            graphics.PreferMultiSampling = true;
            this.Window.Title = Title;
            this.Window.AllowUserResizing = true;
            this.IsMouseVisible = false;

            graphics.ApplyChanges();

            dflip.SystemParameter.clientBounds = new BoundingBox2D(new Vector2(Window.ClientBounds.Left, Window.ClientBounds.Top), new Vector2(Window.ClientBounds.Right, Window.ClientBounds.Bottom), 0f);
            dflip.SystemParameter.ClientHeight = (int)this.Window.ClientBounds.Height;
            dflip.SystemParameter.ClientWidth = (int)this.Window.ClientBounds.Width;
            dflip.SystemParameter.control = System.Windows.Forms.Control.FromHandle(this.Window.Handle);

            this.Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);

            mainProcess = new dflip.SystemParameter();
            if (args.Length > 0)
            {
                profilePath = args[0];
            }
            else profilePath = "profile.ini";

        }

        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            mainProcess.windowSizeChanged = true;
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
            mainProcess.Initialize(profilePath);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            SamplerState d = new SamplerState();

            d.Filter = TextureFilter.Point;
            d.MaxMipLevel = 4;
            d.MaxAnisotropy = 4;
            d.MipMapLevelOfDetailBias = 4;
            GraphicsDevice.PresentationParameters.PresentationInterval = PresentInterval.Two;
            GraphicsDevice.SamplerStates[0] = d;
            dflip.SystemParameter.graphicsDevice = GraphicsDevice;
            
            dflip.SystemParameter.batch_ = new SpriteBatch(GraphicsDevice);
            
            //spriteBatch = new SpriteBatch(GraphicsDevice);
            ResourceManager.content_ = new ContentManager(this.Services);

            //make sure font file
            if (!System.IO.File.Exists(@"Content\Font.xnb"))
            {
                System.Windows.Forms.MessageBox.Show("Font.xnb not found.");
                return;
            }
            ResourceManager.font_ = Content.Load<SpriteFont>("Content\\Font");
            Assembly assembly = Assembly.GetExecutingAssembly();

            for (int i = 0; i < ResourceManager.iconNumber_; i++)
            {
                ResourceManager.texture_.Add(Texture2D.FromStream(GraphicsDevice, assembly.GetManifestResourceStream("PhotoViewer.Resources.icon" + i.ToString() + ".png")));
            }
            ResourceManager.fukiTex_ = Texture2D.FromStream(GraphicsDevice, assembly.GetManifestResourceStream("PhotoViewer.Resources.fuki.png"));

            //read shadow
            ResourceManager.shadowSquare_ = Texture2D.FromStream(GraphicsDevice, assembly.GetManifestResourceStream("PhotoViewer.Resources.shadow_square.png"));
            // frame
            ResourceManager.frameSquare_ = Texture2D.FromStream(GraphicsDevice, assembly.GetManifestResourceStream("PhotoViewer.Resources.dot.png"));

            // mouse
            //if (IsMouseVisible == false)
            {
                ResourceManager.cursor_ = Texture2D.FromStream(GraphicsDevice, assembly.GetManifestResourceStream("PhotoViewer.Resources.cursor1.png"));
            }

            // stroke & x
            ResourceManager.stroke_ = Texture2D.FromStream(GraphicsDevice, assembly.GetManifestResourceStream("PhotoViewer.Resources.stroke.png"));
            ResourceManager.batsuTex_ = Texture2D.FromStream(GraphicsDevice, assembly.GetManifestResourceStream("PhotoViewer.Resources.batsu.png"));
            //pie menu
            ResourceManager.pieTexDef_ = Texture2D.FromStream(GraphicsDevice, assembly.GetManifestResourceStream("PhotoViewer.Resources.pie.png"));
            for (int i = 0; i < ResourceManager.pieMenuNumber; ++i)
            {
                ResourceManager.pieTexs_.Add(Texture2D.FromStream(GraphicsDevice, assembly.GetManifestResourceStream("PhotoViewer.Resources.pie" + (i + 1).ToString() + ".png")));
            }
            // time slider
            ResourceManager.sBarTex1_ = Texture2D.FromStream(GraphicsDevice, assembly.GetManifestResourceStream("PhotoViewer.Resources.scrollBar1.png"));
            ResourceManager.sBarTex2_ = Texture2D.FromStream(GraphicsDevice, assembly.GetManifestResourceStream("PhotoViewer.Resources.scrollBar2.png"));

            // 
            //fukiTex_ = Texture2D.FromStream(Browser.Instance.GraphicsDevice, assembly.GetManifestResourceStream("PhotoViewer.Resources.fuki.png"));

            // map

            ResourceManager.mapTex_ = Texture2D.FromStream(GraphicsDevice, assembly.GetManifestResourceStream("PhotoViewer.Resources.worldmap2.png"));
            ResourceManager.mapTex_tohoku = Texture2D.FromStream(GraphicsDevice, assembly.GetManifestResourceStream("PhotoViewer.Resources.map_tohoku.png"));

            // dock

            ResourceManager.icon_light_ = Texture2D.FromStream(GraphicsDevice, assembly.GetManifestResourceStream("PhotoViewer.Resources.icon_light.png"));
            ResourceManager.shadowCircle_ = Texture2D.FromStream(GraphicsDevice, assembly.GetManifestResourceStream("PhotoViewer.Resources.shadow_circle.png"));

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            ResourceManager.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            dflip.SystemParameter.clientBounds = new BoundingBox2D(new Vector2(Window.ClientBounds.Left, Window.ClientBounds.Top), new Vector2(Window.ClientBounds.Right, Window.ClientBounds.Bottom), 0f);
            dflip.SystemParameter.ClientHeight = this.Window.ClientBounds.Height;
            dflip.SystemParameter.ClientWidth = this.Window.ClientBounds.Width;

            
            mainProcess.update();
            this.Window.Title = Title + " - " + dflip.SystemParameter.title;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here
            mainProcess.draw();
            base.Draw(gameTime);
        }
    }
}
