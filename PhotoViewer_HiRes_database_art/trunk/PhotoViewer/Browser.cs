// define するシンボル（プロジェクトのプロパティで設定）
//  ON: WINDOWS,CALC_FPS,MOUSE_UNDEAD,NO_ROTATION
// OFF: JAPANESE_MAP,LABEL_JAPANESE,NO_DRAW,NoEyeTrack,STRICT,STROKE_DEBUG

#region Using Statements
using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PhotoViewer.Input.KeyboardDev;
using PhotoViewer.Supplement;
using PhotoViewer.Element.Dock;
using PhotoViewer.Manager.Resource;
using PhotoViewer.Attractor.Manager;
using PhotoViewer.Manager.Time;
using PhotoViewer.Manager.Interaction;
using PhotoViewer.Input.PointingDev;
using PhotoViewer.Manager.PhotoDis;
using PhotoViewer.Input.Raw.Process;
using PhotoViewer.Element.StrokeTextbox;
using Windows7.Multitouch.Window;
using Windows7.Multitouch;
using PhotoViewer.Element.Label;
#endregion

namespace PhotoViewer
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /// 
    public class Browser : Microsoft.Xna.Framework.Game
    {
        #region 基本設定
        // タイトルテキスト
        public static string Title = "D-Flip: Dynamic & Flexible Interactive Photoshow";
        public static Browser Instance;
        // （モニタの解像度が 1024×768 のとき，(1024, 768) - (6, 32) → (1018, 736) とするとウインドウが画面いっぱいになる）
        // ムービー撮影時は，ウインドウ枠を除く領域の 横:縦 を 64:48 の倍数にする（アスペクト比 4:3，圧縮時のノイズを減らすため 16 の倍数）．
        public int ClientWidth 
        {
            get;
            private set;
        }// 1274;// 1018;

        public int ClientHeight 
        {
            get;
            private set;
        }// 992;// 736;

        // 合适的图像区域
        public const int MAXX = 256;
        public const int MAXY = 256;
        // 白色边框的厚度
        public const int MAR = 10;//5

        public System.Windows.Forms.Control control
        {
            get;
            private set;
        }
        
        KeyboardDevice keyboard = new KeyboardDevice();
        public Wall wall
        {
            get;
            private set;
        }

        // 每一幅图片的最大最小缩放值
        public static float MinPhotoScale(int windowX, int windowY, int photoX, int photoY, int photoCount)
        {
            int wArea = windowX * windowY;
            //int pArea = photoX * photoY;
            //int pCount = (1 + (int)Math.Sqrt((double)photoCount)) * (1 + (int)Math.Sqrt((double)photoCount));
            //return (float)Math.Sqrt((float)wArea / (float)(pCount * pArea)) * 0.25f;
            return (float)Math.Sqrt((float)wArea / photoCount) * 0.125f;
        }
        public static float MaxPhotoScale(int windowX, int windowY, int photoX, int photoY, int photoCount)
        {
            int wArea = windowX * windowY;
            int pArea = photoX * photoY;
            //return (float)Math.Sqrt((float)wArea / (float)pArea) * 0.5f;
            float result = (float)Math.Sqrt((float)wArea / photoCount) * 4f;
            if (result > Math.Sqrt(wArea) / 4)
                return (float)Math.Sqrt(wArea) / 4;
            return result;
        }
        
        // 自动载入图像周期[ms]
        public const int AutoLoadSpan = 5000;
        // 采样过滤器周期[ms]
        public const int FilterSamplingTime = 10;//1000;
        // 入力がないポインティングデバイスのポインタを削除するまでの時間[frame]
#if MOUSE_UNDEAD
        private readonly int RAW_MOUSE_REMOVE = int.MaxValue;// 30 * 60
#else
        private readonly int RAW_MOUSE_REMOVE = 30 * 60;
#endif
        #endregion

        #region 成员
        
        // 各种窗体
        private SystemState systemState;
        private PhotoDisplay photoDisplay;// new Viewer();
#if NoEyeTrack
#else
        private EyeTrackingForm eyeTrackingForm_ = new EyeTrackingForm();
#endif
        #endregion

        #region 描画用成员和纹理
        // 描画用グラフィックデバイスマネージャ
        GraphicsDeviceManager graphics_;
        // 描画用スプライトバッチ
        public SpriteBatch batch_
        {
            get;
            private set;
        }
        #endregion

        #region ControlPanel のメンバに置き換える等して消す予定
        // アトラクター選択用のアイコン（ドック風）
        private Dock dock_ = null;
        
        // 存储stroke的列表
        
        private StrokeBoxCollection strokeGroup = new StrokeBoxCollection();
        // pie menu 列表
        //private List<PieMenu> pieMenus_ = new List<PieMenu>();
        
        //Time state resource
        private TimeSliderManager timeStateManager = new TimeSliderManager();
        #endregion

        // 加载照片的日志
        //private List<PhotoLog> photoLog_ = new List<PhotoLog>();
        // 加载人物标记, added by Gengdai
        //private List<PeopleTags> peopleTags = new List<PeopleTags>();
        

        MouseController inputController;
        RawInputForm rawInput;
        PointingDeviceCollection pdCollection;
        bool BoxPosUpdate = true;
        public GraphicsAdapter graphicsAdapter = null;
        
        // 构造函数
        public Browser()
        {
            // 图形和内容管理器实例化
            graphics_ = new GraphicsDeviceManager(this);
            // 抗锯齿有效
            graphics_.PreferMultiSampling = true;
            
            // 设置窗口大小
            ClientHeight = 768 - 32;
            ClientWidth = 1366 - 6;
            graphics_.PreferredBackBufferWidth = ClientWidth;
            graphics_.PreferredBackBufferHeight = ClientHeight;
            // 设置窗口名称
            this.Window.Title = Title;
            // 可以调整窗口大小
            this.Window.AllowUserResizing = true;
            // 窗口不显示鼠标
            this.IsMouseVisible = false;
            // 是否固定60 FPS（詳細はXNAの仕様を参照 http://blogs.msdn.com/ito/archive/2007/03/08/2-update.aspx）
#if CALC_FPS
            this.IsFixedTimeStep = false;
#else
            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d);
#endif
            graphics_.ApplyChanges();
            // 读取设定文件（图像日志）
            //controlPanel_.ReadPhotoLogs(photoLog_);
            //controlPanel_.ReadPeopleLogs(peopleTags);
            IntPtr hWnd = this.Window.Handle;
            control = System.Windows.Forms.Control.FromHandle(hWnd);
            this.Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);
            this.control.Move += new EventHandler(control_Move);
            //batch_ = new SpriteBatch(this.GraphicsDevice);
            //provide global access to game instance, so other classes can access graphicsdevice
            Instance = this;
            
            clientBounds = new BoundingBox2D(new Vector2(Window.ClientBounds.Left, Window.ClientBounds.Top), new Vector2(Window.ClientBounds.Right, Window.ClientBounds.Bottom), 0f);
            rawInput = new RawInputForm();
            pdCollection = rawInput.pdCollection;
            //one more pointing device, store touch data
            //pdCollection.add(new PointingDevice(0, new Vector2(6000, 6000)));
            systemState = new SystemState();
            wall = new Wall();

            graphicsAdapter = graphics_.GraphicsDevice.Adapter;
        }

        void control_Move(object sender, EventArgs e)
        {
            BoxPosUpdate = true;
        }

        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            ClientHeight = this.Window.ClientBounds.Height;
            ClientWidth = this.Window.ClientBounds.Width;
            BoxPosUpdate = true;
        }

        // 丢弃
        protected override void Dispose(bool disposing)
        {
            // 丢弃，释放资源
            if (disposing)
            {
                rawInput.Dispose();
                rawInput = null;
#if NoEyeTrack
#else
                eyeTrackingForm_.Dispose();
                eyeTrackingForm_ = null;
#endif
            }
            base.Dispose(disposing);
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
            dock_ = new Dock(ResourceManager.iconNumber_);
            photoDisplay = new PhotoDisplay(systemState, dock_, timeStateManager.sBar, strokeGroup);
            inputController = new MouseController(pdCollection, dock_, timeStateManager.sBar, systemState, photoDisplay, strokeGroup, keyboard);
            
        }
        /*public Stream GenerateStreamFromString(string s)
{
    MemoryStream stream = new MemoryStream();
    StreamWriter writer = new StreamWriter(stream);
    writer.Write(s);
    writer.Flush();
    stream.Position = 0;
    return stream;
}*/


        /// <summary>
        /// Load your graphics_ content.  If loadAllContent is true, you should
        /// load content from both ResourceManagementMode pools.  Otherwise, just
        /// load ResourceManagementMode.Manual content.
        /// </summary>
        /// <param name="loadAllContent">Which type of content to load.</param>
        [Obsolete("LoadGraphicsContentは古い形式")]
        protected override void LoadContent()
        {
            batch_ = new SpriteBatch(graphics_.GraphicsDevice);
            ResourceManager.LoadContent();
        }
        List<PointingDevice> touchDevices = new List<PointingDevice>();
        private List<FloatTextBox> ftBoxes_ = new List<FloatTextBox>();
        
        
        /// <summary>
        /// Unload your graphics_ content.  If unloadAllContent is true, you should
        /// unload content from both ResourceManagementMode pools.  Otherwise, just
        /// unload ResourceManagementMode.Manual content.  Manual content will get
        /// Disposed by the GraphicsDevice during a Reset.
        /// </summary>
        /// <param name="unloadAllContent">Which type of content to unload.</param>
        [Obsolete("UnloadGraphicsContentは古い形式")]
        protected override void UnloadContent()
        {
            if (true)
            {
                // TODO: Unload any ResourceManagementMode.Automatic content
                batch_.Dispose(); // NOTE: 也不知道是否是必要的

                //if (createPhoto != null)
                //{
                //    createPhoto.UnloadPhoto();
                //    createPhoto.UnloadPeopleTag();
                //}
                //peopleTags.Clear();
                ResourceManager.Unload();
                //content_.Unload();
            }

            // TODO: Unload any ResourceManagementMode.Manual content
        }
        //Dictionary<Photo, int> touchPhotoMap = new Dictionary<Photo,int>(); 
        public BoundingBox2D clientBounds = new BoundingBox2D();
        bool focus = false;
        bool deactivated = false;
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
#if CALC_FPS
            experimentForm_.LogUpdateTime(gameTime, photos_.Count);
#endif
            
#if NoEyeTrack
#else
            eyeTrackingForm_.ScreenBounds = screenBounds;
#endif
            if ((this.control.Focused && focus) || !deactivated)
            {
                clientBounds = new BoundingBox2D(new Vector2(Window.ClientBounds.Left, Window.ClientBounds.Top), new Vector2(Window.ClientBounds.Right, Window.ClientBounds.Bottom), 0f);
               
                #region 指点设备状态更新

                //touchDevices.Clear();
                pdCollection.initialize();
                inputController.trigerDock();
                if (BoxPosUpdate)
                {
                    strokeGroup.PositionUpdate();
                    wall.UpdateWall();
                }
                
                // アイトラッカをポインティングデバイスとして追加
#if NoEyeTrack
#else
            if (eyeTrackingForm_.IsTracking)
            {
                // 両目の視点の平均を利用   利用视点的平均值
                pointingDevices.Add(new PointingDevice(-1, PointingDevice.DeviceType.EyeTracker, eyeTrackingForm_.GazePosition - clientBounds.Min));
                draggedPhotos_.Add(new List<SDraggedPhoto>());
                //// 左右の視点を別々に利用
                //pointingDevices.Add(new PointingDevice(-2, PointingDevice.DeviceType.EyeTracker, eyeTrackingForm_.LeftGazePosition - clientBounds.Min));
                //pointingDevices.Add(new PointingDevice(-3, PointingDevice.DeviceType.EyeTracker, eyeTrackingForm_.RightGazePosition - clientBounds.Min));
            }
#endif
                #endregion
                ///
                /// 用户的操作 
                ///
                #region  keyboard
                keyboard.getKeyboardState();
                if (keyboard.altKey && keyboard.ctrlKey && keyboard.isKeyDown(Keys.Z))
                {
                    photoDisplay.Stop = false;
                }
                if (keyboard.altKey && keyboard.ctrlKey && keyboard.isKeyDown(Keys.X))
                {
                    photoDisplay.Stop = true;
                }
                #endregion

                //(window is not focused and deactivated is false) indicates focus is in textboxes

                photoDisplay.photoInitialize();
                inputController.check();
                photoDisplay.photoBehavior();
                BoxPosUpdate = false;
            }

            else
            {
                inputController.clearStrokeUnderDrawing();
            }

            //if deactivated is true, the app is deactivated
            if (deactivated && this.control.Focused)
            {
                //    bool flag = false;
                //    for (int i = 0; i < strokeGroup.textboxList.Count; i++)
                //    {
                //        if (strokeGroup.textboxList[i] != null && strokeGroup.textboxList[i].IfFocused())
                //        {
                //            flag = true;
                //            break;
                //        }
                //    }
                deactivated = false;
                for (int i = 0; i < strokeGroup.textboxList.Count; i++)
                {
                    if (strokeGroup.textboxList[i] != null && strokeGroup.textboxList[i].IsShown)
                    {
                        strokeGroup.textboxList[i].Show();
                    }
                }

            }

            //if window lost focus && focus is not in textbox or wall form, 
            //then we guess the focus is in other threads, so textbox's topmost should set to false
            //if (wall.IfFocused())
            //    Console.WriteLine("true");
            if (!focus && !this.control.Focused && !wall.IfFocused())
            {
                //Console.WriteLine("coming");
                bool flag = false;
                for (int i = 0; i < strokeGroup.textboxList.Count; i++)
                {
                    if (strokeGroup.textboxList[i] != null && strokeGroup.textboxList[i].IfFocused())
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    for (int i = 0; i < strokeGroup.textboxList.Count; i++)
                    {
                        if (strokeGroup.textboxList[i] != null)
                        {
                            strokeGroup.textboxList[i].Hide();
                        }
                    }
                    //photoDisplay.clearActive();
                    deactivated = true;
                }
            }
            //if (deactivated)
            //    Console.WriteLine("what!");

            rawInput.removeTouchMap();//remove not-existing touch point; if remove in touchup function, can't detect tap.
            
            focus = this.control.Focused;

            base.Update(gameTime);
        }

        Vector3 tempColor;
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
#if NO_DRAW
        }
#else
#if CALC_FPS
            experimentForm_.LogDrawTime(gameTime, photos_.Count);
#endif


#if NoEyeTrack
            {
#else
            if (!eyeTrackingForm_.IsCalibrating)
            {
#endif
                graphics_.GraphicsDevice.Clear(Color.White);
                batch_.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                // 描绘地图
                //double mot = -1d;
                if (systemState.curState == SystemState.ATTRACTOR_GEOGRAPH)
                {
                    Color halfWhite = new Color(255, 255, 255, (byte)122);
                    Vector2 mapScale = new Vector2((float)Window.ClientBounds.Width / (float)ResourceManager.mapTex_.Width, (float)Window.ClientBounds.Height / (float)ResourceManager.mapTex_.Height);
                    //batch_.Begin();
                    batch_.Draw(ResourceManager.mapTex_, Vector2.Zero, null, halfWhite, 0f, Vector2.Zero, mapScale, SpriteEffects.None, 1f);
                    //batch_.End();

                }
                // render stroke
                strokeGroup.renderStatic();
                
                
                //batch_.End();
                
                // 渲染画像
                //batch_.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                photoDisplay.drawPhoto();
                //batch_.End();

                //batch_.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                // 渲染dock
                dock_.Render(batch_, Window.ClientBounds.Height, ResourceManager.shadowCircle_, ResourceManager.icon_light_);
                pdCollection.drawPieMenu();
                strokeGroup.renderDynamic();
                strokeGroup.renderTags();
                // 渲染滚动条下的dock
                if (systemState.curState == SystemState.ATTRACTOR_TIME)
                {
                    timeStateManager.render();
                }
                //batch_.End();
                              

                // 渲染光标
#if NoEyeTrack
                if (!IsMouseVisible)
                {
                    
#else
                if (IsMouseVisible == false && eyeTrackingForm_.IsEyeTrackingVisible)
                {
#endif
                    //float alpha = (float)Math.Abs((gameTime.TotalGameTime.Milliseconds % 511) - 255) / 255f;
                    //batch_.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                    // 计算鼠标的颜色
#if CALC_FPS
                float hueSpeed = 5f;
#else
                    float hueSpeed = 5f;// / ((float)(this.TargetElapsedTime.Milliseconds) * 0.03f);
#endif
                    Vector3 cursorColor = Vector3.Zero;
                    float hu = (float)(gameTime.TotalGameTime.TotalMilliseconds) * hueSpeed / 10000f;
                    if (hu > 1f)
                        hu -= (int)hu;
                    tempColor = new Vector3(hu, 1f, 1f);
                    ResourceManager.hsv2rgb(ref tempColor, out cursorColor);
                    pdCollection.drawMouse(new Color(cursorColor));
                    
                    
                }
            }
            batch_.End();
            base.Draw(gameTime);
        }
#endif
    }
}