using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Windows.Forms;
using PhotoViewer.Element.PieMenu;

namespace PhotoViewer.Input.PointingDev
{
    public class PointingDevice : ICloneable
    {
        // コピーのために浅いコピーオブジェクトを返す
        public object Clone()
        {
            return MemberwiseClone();
        }

        // ポインティングデバイスの種類を示す
        public enum DeviceType
        {
            Unknown,
            Mouse,
            EyeTracker,
            Touch,
        }

        public enum State
        {
            Cross,
            Curosr,
        }

        // デバイスのヘッダ
        private int header_ = 0;
        // デバイスの種類
        private DeviceType type_ = DeviceType.Unknown;
        // スクリーン座標系でのカーソル位置
        private Vector2 position_ = Vector2.Zero;
        private Vector2 leftDownPosition_ = Vector2.Zero;
        private Vector2 rightDownPosition_ = Vector2.Zero;
        private Vector2 middleDownPosition_ = Vector2.Zero;
        // ボタンの状態
        private Microsoft.Xna.Framework.Input.ButtonState leftButton_ = Microsoft.Xna.Framework.Input.ButtonState.Released;
        private Microsoft.Xna.Framework.Input.ButtonState rightButton_ = Microsoft.Xna.Framework.Input.ButtonState.Released;
        //private Microsoft.Xna.Framework.Input.ButtonState middleButton_ = Microsoft.Xna.Framework.Input.ButtonState.Released;
        // デバイスの無入力時間を保持するカウンタ
        private int timeCounter_ = 0;
        public int state
        {
            get;
            set;
        }
        // コンストラクタ
        public PointingDevice(int header, Vector2 position)
        {
            header_ = header;
            //type_ = type;
            position_ = position;
            oldLeftButton = Microsoft.Xna.Framework.Input.ButtonState.Released;
            oldRightButton = Microsoft.Xna.Framework.Input.ButtonState.Released;
            MiddleButton = Microsoft.Xna.Framework.Input.ButtonState.Released;
            state = (int)PointingDevice.State.Curosr;
        }

        public bool showTouchPie = false;
        public int countTouchTime = 0;
        public int rightTouchCount = 0;
        public int rightMenuAppearCount = 0;


        public void DrawCursor()
        {
            Cursor.Position = new System.Drawing.Point((int)this.position_.X, (int)this.position_.Y);
        }

        PieMenu pieMenu = new PieMenu();

        //public void drawPieMenu()
        //{
        //    pieMenu.Render(this.GamePosition);
        //}

        public PieMenu getPieMenu()
        {
            return pieMenu;
        }


        public void PositionAdd(Vector2 vectorFromLast)
        {
            position_ += vectorFromLast;
            //Console.WriteLine(this.position_ + "s");
            //Console.WriteLine(Cursor.Position);
        }

        #region property
        public int Header
        {
            get
            {
                return header_;
            }
            set
            {
                header_ = value;
            }
        }
        public DeviceType Type
        {
            get
            {
                return type_;
            }
            set
            {
                type_ = value;
            }
        }
        public Vector2 Position
        {
            get
            {
                return position_;
            }
            set
            {
                position_ = value;
            }
        }
        
        public Vector2 GamePosition
        {
            get
            {
                return position_ - Browser.Instance.clientBounds.Min;
            }
        }

        public Vector2 OldGamePosition
        {
            get;
            private set;
        }

        public Vector2 LeftDownPosition
        {
            get
            {
                return leftDownPosition_;
            }
            set
            {
                leftDownPosition_ = value;
            }
        }
        public Vector2 GameLeftDownPosition
        {
            get
            {
                return leftDownPosition_ - Browser.Instance.clientBounds.Min;
            }
        }
        public Vector2 RightDownPosition
        {
            get
            {
                return rightDownPosition_;
            }
            set
            {
                rightDownPosition_ = value;
            }
        }
        public Vector2 GameRightDownPosition
        {
            get
            {
                return rightDownPosition_ - Browser.Instance.clientBounds.Min;
            }
            set
            {
                GameRightDownPosition = value;
            }
        }
        public Vector2 MiddleDownPosition
        {
            get
            {
                return middleDownPosition_;
            }
            set
            {
                middleDownPosition_ = value;
            }
        }
        public int TimeCounter
        {
            get
            {
                return timeCounter_;
            }
        }

        public Microsoft.Xna.Framework.Input.ButtonState LeftButton
        {
            get
            {
                return leftButton_;
            }
            set
            {
                leftButton_ = value;
            }
        }

        public Microsoft.Xna.Framework.Input.ButtonState oldLeftButton
        {
            get;
            set;
        }

        public Microsoft.Xna.Framework.Input.ButtonState RightButton
        {
            get
            {
                return rightButton_;
            }
            set
            {
                rightButton_ = value;
            }
        }

        public Microsoft.Xna.Framework.Input.ButtonState oldRightButton
        {
            get;
            set;
        }
        public Microsoft.Xna.Framework.Input.ButtonState MiddleButton
        {
            get;
            set;
        }

        public int MiddleValue
        {
            get;
            set;
        }

        public void update()
        {
            oldLeftButton = LeftButton;
            oldRightButton = RightButton;
            OldGamePosition = GamePosition;
        }

       #endregion
    }
}
