using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using PhotoViewer.Supplement;
using PhotoViewer.Manager;
using PhotoViewer.Element;
using Microsoft.Xna.Framework;
using PhotoViewer.InputDevice;

namespace PhotoViewer
{
    public class SystemParameter
    {
        public static SpriteBatch batch_
        {
            get;
            set;
        }

        public static System.Windows.Forms.Control control
        {
            get;
            set;
        }

        public static int ClientWidth
        {
            get;
            set;
        }// 1274;// 1018;
        public static int ClientHeight
        {
            get;
            set;
        }// 992;// 736;

        public static BoundingBox2D clientBounds = new BoundingBox2D();

        public static GraphicsDevice graphicsDevice;

        KeyboardDevice keyboard = new KeyboardDevice();

        // 各种窗体
        private SystemState systemState;
        private PhotoDisplay photoDisplay;

        #region ControlPanel のメンバに置き換える等して消す予定
        // アトラクター選択用のアイコン（ドック風）
        private Dock dock_ = null;

        // 存储stroke的列表

        private StrokeBoxCollection strokeGroup = new StrokeBoxCollection();
        // pie menu 列表
        private List<PieMenu> pieMenus_ = new List<PieMenu>();

        //Time state resource
        private TimeSliderManager timeStateManager = new TimeSliderManager();
        #endregion

        MouseController inputController;
        RawInputForm rawInput;
        PointingDeviceCollection pdCollection;

        Vector3 tempColor;

        List<PointingDevice> touchDevices = new List<PointingDevice>();

        // maximum and minimum scale
        public static float MinPhotoScale(int windowX, int windowY, int photoX, int photoY, int photoCount)
        {
            int wArea = windowX * windowY;
            int pArea = photoX * photoY;
            int pCount = (1 + (int)Math.Sqrt((double)photoCount)) * (1 + (int)Math.Sqrt((double)photoCount));
            return (float)Math.Sqrt((float)wArea / (float)(pCount * pArea)) * 0.25f;
        }
        public static float MaxPhotoScale(int windowX, int windowY, int photoX, int photoY, int photoCount)
        {
            int wArea = windowX * windowY;
            int pArea = photoX * photoY;
            return (float)Math.Sqrt((float)wArea / (float)pArea) * 0.5f /*0.75f*/;
        }

        public SystemParameter()
        {
            rawInput = new RawInputForm();
            pdCollection = rawInput.pdCollection;
            systemState = new SystemState();
        }

        public void Initialize(string path)
        {
            dock_ = new Dock(ResourceManager.iconNumber_);
            //pdCollection.initialize();
            photoDisplay = new PhotoDisplay(systemState, dock_, timeStateManager.sBar, strokeGroup);
            inputController = new MouseController(pdCollection, dock_, timeStateManager.sBar, systemState, photoDisplay, strokeGroup, keyboard);
            inputController.InitialOpen(path);
        }

        public void update()
        {
            #region update input device

            touchDevices.Clear();

            inputController.trigerDock();
            #endregion

            //strokeGroup.remove();

            photoDisplay.photoInitialize();
            inputController.check();
            photoDisplay.photoBehavior();

            rawInput.removeTouchMap();//remove not-existing touch point; if remove in touchup function, can't detect tap.

        }

        int i = 0;

        public void draw()
        {
            float width = clientBounds.Max.X - clientBounds.Min.X;
            float height = clientBounds.Max.Y - clientBounds.Min.Y;

            batch_.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            // render map
            //double mot = -1d;
            if (systemState.curState == SystemState.ATTRACTOR_GEOGRAPH)
            {
                Color halfWhite = new Color(255, 255, 255, (byte)255);
                Vector2 mapScale = new Vector2(width / (float)ResourceManager.mapTex_.Width, height / (float)ResourceManager.mapTex_.Height);
                //batch_.Begin();
                batch_.Draw(ResourceManager.mapTex_, Vector2.Zero, null, halfWhite, 0f, Vector2.Zero, mapScale, SpriteEffects.None, 1f);
                //batch_.End();

            }
            // render stroke
            strokeGroup.renderStatic();


            //batch_.End();

            //render photos
            //batch_.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            photoDisplay.drawPhoto();
            //batch_.End();

            //batch_.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            // render dock
            dock_.Render(batch_, (int)height, ResourceManager.shadowCircle_, ResourceManager.icon_light_);
            pdCollection.drawPieMenu();
            strokeGroup.renderDynamic();
            strokeGroup.renderTags();
            // render time slider
            if (systemState.curState == SystemState.ATTRACTOR_TIME)
            {
                timeStateManager.render();
            }
            //batch_.End();


            // render mouse

            //float alpha = (float)Math.Abs((gameTime.TotalGameTime.Milliseconds % 511) - 255) / 255f;
            //batch_.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            float hueSpeed = 5f;// / ((float)(this.TargetElapsedTime.Milliseconds) * 0.03f);

            Vector3 cursorColor = Vector3.Zero;
            float hu = (float)(i++) * hueSpeed/ 10000f;
            if (hu > 1f)
                hu -= (int)hu;
            tempColor = new Vector3(hu, 1f, 1f);
            ResourceManager.hsv2rgb(ref tempColor, out cursorColor);
            pdCollection.drawMouse(new Color(cursorColor));

            batch_.End();


        }
    }
}
