using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace PhotoViewer.Manager
{
    public static class ResourceManager
    {
        // 内容管理器
        public static ContentManager content_ = null;

        public static string homeDirectory_ = "C:\\Photoviewer";

        // 阴影图像的纹理
        public static Texture2D shadowSquare_ = null;
        // 白色边框的图像纹理
        public static Texture2D frameSquare_ = null;
        // 光标纹理
        public static Texture2D cursor_ = null;
        // dock的光纹理
        public static Texture2D icon_light_ = null;
        // dock图标栏的阴影
        public static Texture2D shadowCircle_ = null;
        // ×的纹理
        public static Texture2D batsuTex_ = null;

        // 顶部时间轴的纹理
        public static Texture2D sBarTex1_ = null;
        public static Texture2D sBarTex2_ = null;
        // 气球的纹理
        public static Texture2D fukiTex_ = null;
        // 世界地图的纹理
        public static Texture2D mapTex_ = null;
        public static Texture2D stroke_ = null;
        
        //piemenu
        public static Texture2D pieTexDef_ = null;
        public static readonly int pieMenuNumber = 6;
        public static List<Texture2D> pieTexs_ = new List<Texture2D>();

        public static List<Texture2D> texture_ = new List<Texture2D>();
        public static readonly int iconNumber_ = 4;
        public static SpriteFont font_;
        public static void LoadContent()
        {
            content_ = new ContentManager(Browser.Instance.Services);

            font_ = Browser.Instance.Content.Load<SpriteFont>("Content\\Font");
            Assembly assembly = Assembly.GetExecutingAssembly();

            for (int i = 0; i < iconNumber_; i++ )
            {
                texture_.Add(Texture2D.FromStream(Browser.Instance.GraphicsDevice, assembly.GetManifestResourceStream("PhotoViewer.Resources.icon" + i.ToString() + ".png")));
            }
            fukiTex_ = Texture2D.FromStream(Browser.Instance.GraphicsDevice, assembly.GetManifestResourceStream("PhotoViewer.Resources.fuki.png"));

            // 读取图像阴影纹理
            shadowSquare_ = Texture2D.FromStream(Browser.Instance.GraphicsDevice, assembly.GetManifestResourceStream("PhotoViewer.Resources.shadow_square.png"));
            // 读取白色边框纹理
            frameSquare_ = Texture2D.FromStream(Browser.Instance.GraphicsDevice, assembly.GetManifestResourceStream("PhotoViewer.Resources.dot.png"));

            // 读取光标纹理
            //if (IsMouseVisible == false)
            {
                cursor_ = Texture2D.FromStream(Browser.Instance.GraphicsDevice, assembly.GetManifestResourceStream("PhotoViewer.Resources.cursor1.png"));
            }

            // 读取stroke和x的纹理
            stroke_ = Texture2D.FromStream(Browser.Instance.GraphicsDevice, assembly.GetManifestResourceStream("PhotoViewer.Resources.stroke.png"));
            batsuTex_ = Texture2D.FromStream(Browser.Instance.GraphicsDevice, assembly.GetManifestResourceStream("PhotoViewer.Resources.batsu.png"));
            // 读取pie memu菜单纹理
            pieTexDef_ = Texture2D.FromStream(Browser.Instance.GraphicsDevice, assembly.GetManifestResourceStream("PhotoViewer.Resources.pie.png"));
            for (int i = 0; i < pieMenuNumber; ++i)
            {
                pieTexs_.Add(Texture2D.FromStream(Browser.Instance.GraphicsDevice, assembly.GetManifestResourceStream("PhotoViewer.Resources.pie" + (i + 1).ToString() + ".png")));
            }
            // 读取滚动条纹理
            sBarTex1_ = Texture2D.FromStream(Browser.Instance.GraphicsDevice, assembly.GetManifestResourceStream("PhotoViewer.Resources.scrollBar1.png"));
            sBarTex2_ = Texture2D.FromStream(Browser.Instance.GraphicsDevice, assembly.GetManifestResourceStream("PhotoViewer.Resources.scrollBar2.png"));

            // 读取对于图片移动时的纹理（气球？）
            //fukiTex_ = Texture2D.FromStream(Browser.Instance.GraphicsDevice, assembly.GetManifestResourceStream("PhotoViewer.Resources.fuki.png"));

            // 读取世界（日本）地图
#if JAPANESE_MAP
                mapTex_ = Texture2D.FromStream(graphics_.GraphicsDevice, assembly.GetManifestResourceStream("PhotoViewer.worldmap1.png"));
#else
            mapTex_ = Texture2D.FromStream(Browser.Instance.GraphicsDevice, assembly.GetManifestResourceStream("PhotoViewer.Resources.worldmap2.png"));
#endif

            // dock实例化并载入相关纹理

            icon_light_ = Texture2D.FromStream(Browser.Instance.GraphicsDevice, assembly.GetManifestResourceStream("PhotoViewer.Resources.icon_light.png"));
            shadowCircle_ = Texture2D.FromStream(Browser.Instance.GraphicsDevice, assembly.GetManifestResourceStream("PhotoViewer.Resources.shadow_circle.png"));

        }
        public static void Unload()
        {
            content_.Dispose();
            for (int i = 0; i < iconNumber_; i++ )
            {
                texture_[i].Dispose();
            }
            fukiTex_.Dispose();
            shadowSquare_.Dispose();
            // 读取白色边框纹理
            frameSquare_.Dispose();

            // 读取光标纹理
            cursor_.Dispose();

            // 读取stroke和x的纹理
            stroke_.Dispose();
            batsuTex_.Dispose();
            // 读取pie memu菜单纹理
            pieTexDef_.Dispose();
            for (int i = 0; i < pieMenuNumber; ++i)
            {
                pieTexs_[i].Dispose();
            }
            // 读取滚动条纹理
            sBarTex1_.Dispose();
            sBarTex2_.Dispose();

            // 读取对于图片移动时的纹理（气球？）
            fukiTex_.Dispose();
            // 读取世界（日本）地图
#if JAPANESE_MAP
                mapTex_.Dispose();
#else
            mapTex_.Dispose();
#endif

            // dock实例化并载入相关纹理

            icon_light_.Dispose();
            shadowCircle_.Dispose();
        }

        public static double HsvDist(Vector3 f1, Vector3 f2)
        {
            double x = (double)f1.Y * Math.Cos((double)(f1.X * Math.PI) / 180d);
            double y = (double)f1.Y * Math.Sin((double)(f1.X * Math.PI) / 180d);
            double z = (double)f1.Z * 0.7d;
            double xx = (double)f2.Y * Math.Cos((double)(f2.X * Math.PI) / 180d);
            double yy = (double)f2.Y * Math.Sin((double)(f2.X * Math.PI) / 180d);
            double zz = (double)f2.Z * 0.7d;
            return Math.Sqrt((x - xx) * (x - xx) + (y - yy) * (y - yy) + (z - zz) * (z - zz));
        }

        public static void rgb2hsv(ref Vector3 rgb, out Vector3 hsv)
        {
            // (r,g,b)は(1,1,1)，(h,s,v)は(360,1,1)
            float min = (float)Math.Min(Math.Min(rgb.X, rgb.Y), rgb.Z);
            float max = (float)Math.Max(Math.Max(rgb.X, rgb.Y), rgb.Z);

            if (max == min)
            {
                hsv.X = 0;
            }
            else
            {
                if (max == rgb.X)
                {
                    hsv.X = 60.0f * (rgb.Y - rgb.Z) / (max - min);
                }
                else if (max == rgb.Y)
                {
                    hsv.X = 60.0f * (rgb.Z - rgb.X) / (max - min) + 120.0f;
                }
                else
                {
                    hsv.X = 60.0f * (rgb.X - rgb.Y) / (max - min) + 240.0f;
                }
            }
            //hsv.Y  = ( max - min ) / max; // 円柱の色空間
            hsv.Y = max - min; // 円錐の色空間
            hsv.Z = max;
        }
        public static void hsv2rgb(ref Vector3 hsv, out Vector3 rgb)
        {
            // (r,g,b)は(1,1,1)，(h,s,v)は(360,1,1)
            float h = (hsv.X - (float)Math.Floor(hsv.X)) * 6;
            float s = hsv.Y;
            float v = hsv.Z;

            int i = (int)h;
            float f = h - i;

            float p = v * (1 - s);
            float q = v * (1 - s * (f));
            float t = v * (1 - s * (1 - f));

            switch (i)
            {
                case 6:
                case 0:
                    rgb.X = v;
                    rgb.Y = t;
                    rgb.Z = p;
                    break;
                case 1:
                    rgb.X = q;
                    rgb.Y = v;
                    rgb.Z = p;
                    break;
                case 2:
                    rgb.X = p;
                    rgb.Y = v;
                    rgb.Z = t;
                    break;
                case 3:
                    rgb.X = p;
                    rgb.Y = q;
                    rgb.Z = v;
                    break;
                case 4:
                    rgb.X = t;
                    rgb.Y = p;
                    rgb.Z = v;
                    break;
                case 5:
                    rgb.X = v;
                    rgb.Y = p;
                    rgb.Z = q;
                    break;
                default:
                    rgb = Vector3.Zero;
                    break;
            }
        }
    }

    
}
