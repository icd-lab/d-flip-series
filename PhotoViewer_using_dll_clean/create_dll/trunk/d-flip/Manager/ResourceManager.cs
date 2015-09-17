using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace dflip
{
    public static class ResourceManager
    {
        // 
        public static ContentManager content_ = null;

        public static string homeDirectory_ = null;

        // 
        public static Texture2D shadowSquare_ = null;
        // 
        public static Texture2D frameSquare_ = null;
        // 
        public static Texture2D cursor_ = null;
        // 
        public static Texture2D icon_light_ = null;
        // 
        public static Texture2D shadowCircle_ = null;
        // 
        public static Texture2D batsuTex_ = null;

        // 
        public static Texture2D sBarTex1_ = null;
        public static Texture2D sBarTex2_ = null;
        // 
        public static Texture2D fukiTex_ = null;
        // 
        public static Boolean IfTohoku = false;
        public static Texture2D mapTex_tohoku = null;
        public static Texture2D mapTex_ = null;
        public static Texture2D stroke_ = null;
        
        //piemenu
        public static Texture2D pieTexDef_ = null;
        public static readonly int pieMenuNumber = 6;
        public static List<Texture2D> pieTexs_ = new List<Texture2D>();

        public static List<Texture2D> texture_ = new List<Texture2D>();
        public static readonly int iconNumber_ = 9;
        public static SpriteFont font_;

        //photo area
        public const int MAXX = 256;
        public const int MAXY = 256;
        // frame thickness
        public const int MAR = 10;//5
        
        public static void Unload()
        {
            content_.Dispose();
            for (int i = 0; i < iconNumber_; i++ )
            {
                texture_[i].Dispose();
            }
            fukiTex_.Dispose();
            shadowSquare_.Dispose();
            // white frame
            frameSquare_.Dispose();

            //mouse
            cursor_.Dispose();

            //
            stroke_.Dispose();
            batsuTex_.Dispose();
            // 
            pieTexDef_.Dispose();
            for (int i = 0; i < pieMenuNumber; ++i)
            {
                pieTexs_[i].Dispose();
            }
            // 
            sBarTex1_.Dispose();
            sBarTex2_.Dispose();

            // 
            fukiTex_.Dispose();
            //
#if JAPANESE_MAP
                mapTex_.Dispose();
#else
            mapTex_.Dispose();
#endif

            // 

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
