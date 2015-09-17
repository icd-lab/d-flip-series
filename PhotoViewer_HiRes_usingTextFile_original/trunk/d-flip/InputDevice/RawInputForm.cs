using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PhotoViewer;
using System.Diagnostics;

using Windows7.Multitouch;
using Windows7.Multitouch.Window;

namespace PhotoViewer.InputDevice
{
    public partial class RawInputForm : Form
    {
        private PointingDeviceCollection pointingDevices = new PointingDeviceCollection();
        //private BoundingBox2D screenBounds_ = new BoundingBox2D();
        Dictionary<int, PointingDevice> touchMap = new Dictionary<int, PointingDevice>();
        List<int> touchRemoveList = new List<int>();
        readonly TouchHandler _touchHandler;

        public PointingDeviceCollection pdCollection
        {
            get
            {
                return pointingDevices;
            }
        }

        public RawInputForm()
        {
            InitializeComponent();

            int size = Marshal.SizeOf(typeof(SRawInputDevice));
            SRawInputDevice[] devices = new SRawInputDevice[1];

            // UsagePage=1,Usage=2 �Ń}�E�X�f�o�C�X���w��
            devices[0].UsagePage = 1;
            devices[0].Usage = 2;

            //WM_INPUT ���󂯎��E�B���h�E
            devices[0].Target = this.Handle;

            //WM_INPUT ��L���ɂ���f�o�C�X�Q�Cdevices �̐��CRawInputDevice �̍\���̃T�C�Y
            RegisterRawInputDevices(devices, 1, size);

            //touchMap = 
            //pointingDevices = pdCollection;
            #region touchDevice

            if (Windows7.Multitouch.TouchHandler.DigitizerCapabilities.IsMultiTouchReady)
            {
                _touchHandler = Factory.CreateHandler<TouchHandler>(SystemParameter.control);
                _touchHandler.TouchDown += new EventHandler<TouchEventArgs>(_touchHandler_TouchDown);
                _touchHandler.TouchUp += new EventHandler<TouchEventArgs>(_touchHandler_TouchUp);
                _touchHandler.TouchMove += new EventHandler<TouchEventArgs>(_touchHandler_TouchMove);
                //touchPoints = new List<PointingDevice>();
            }
            #endregion
        }

        public void _touchHandler_TouchMove(object sender, TouchEventArgs e)
        {
            //throw new NotImplementedException();
            if (touchMap.ContainsKey(e.Id))
            {
                PointingDevice p = touchMap[e.Id];
                p.Position = new Vector2(e.Location.X, e.Location.Y);
                //p.oldLeftButton = Microsoft.Xna.Framework.Input.ButtonState.Pressed;
            }
            //Debug.WriteLine(p.Position);
        }

        public void _touchHandler_TouchUp(object sender, TouchEventArgs e)
        {
            
                touchRemoveList.Add(e.Id);
                //p.oldLeftButton = Microsoft.Xna.Framework.Input.ButtonState.Pressed;
            
            
        }

        public void _touchHandler_TouchDown(object sender, TouchEventArgs e)
        {
            PointingDevice p = new PointingDevice(e.Id, new Vector2(e.Location.X, e.Location.Y));
            touchMap[e.Id] = p;
            p.Type = PointingDevice.DeviceType.Touch;
            pdCollection.add(p);
            p.LeftButton = Microsoft.Xna.Framework.Input.ButtonState.Pressed;
            p.oldLeftButton = Microsoft.Xna.Framework.Input.ButtonState.Released;
            p.Position = new Vector2(e.Location.X, e.Location.Y);
            //Debug.WriteLine(p.Position);

        }

        public List<int> TouchRemoveList
        {
            get
            {
                return touchRemoveList;
            }
        }
        public void removeTouchMap()
        {
            if (touchRemoveList.Count > 0)
            {
                for (int i = 0; i < touchRemoveList.Count; i++)
                {
                    if (touchMap.ContainsKey(touchRemoveList[i]))
                    {
                        PointingDevice p = touchMap[touchRemoveList[i]];
                        //p.Position = new Vector2(e.Location.X, e.Location.Y);
                        if (p.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                        {
                            p.LeftButton = Microsoft.Xna.Framework.Input.ButtonState.Released;
                            continue;
                        }
                        pdCollection.remove(p);
                        touchMap.Remove(touchRemoveList[i]);
                        touchRemoveList.Remove(touchRemoveList[i]);
                        i--;
                    }
                }
                //touchRemoveList.Clear();
            }
        }
        // �}�E�X�̃��b�Z�[�W��RawInput���󂯎�邽�т�ProcessInputKey�����s
        private void ProcessInputKey(ref Message m)
        {
            const int RidInput = 0x10000003;
            int headerSize = Marshal.SizeOf(typeof(SRawInputHeader));
            int size = Marshal.SizeOf(typeof(SRawInput));

            SRawInput input;
            GetRawInputData(m.LParam, RidInput, out input, ref size, headerSize);
            SRawMouse tempMouse = input.Mouse;

            // ���m�F�̃w�b�_�����f�o�C�X���}�E�X�Ƃ��ēo�^
            if (input.Header.Device != IntPtr.Zero)//so maybe it is touch device, process in another way
            {
                bool headerFlag = false;
                for (PointingDevice pointingDevice = pointingDevices.first(); pointingDevice != null; pointingDevice = pointingDevices.next())
                {
                    if (pointingDevice.Header == (int)input.Header.Device)
                    {
                        headerFlag = true;
                        break;
                    }
                }
                if (headerFlag == false)
                {
                    pointingDevices.add(new PointingDevice((int)input.Header.Device, new Vector2(Cursor.Position.X, Cursor.Position.Y)));
                    //Console.WriteLine(Cursor.Position);
                    
                }

                // �w�b�_����v����f�o�C�X�̏����X�V
                for (PointingDevice pointingDevice = pointingDevices.first(); pointingDevice != null; pointingDevice = pointingDevices.next())
                {
                    if (pointingDevice.Header == (int)input.Header.Device)
                    {
                        // �ʒu���̍X�V
                        Vector2 tempAdded = new Vector2(tempMouse.LastX, tempMouse.LastY);
                        pointingDevice.PositionAdd(tempAdded);
                        Cursor.Position = new System.Drawing.Point((int)pointingDevice.Position.X, (int)pointingDevice.Position.Y);
                        //if (true || screenBounds_.Contains(pointingDevice.Position + tempAdded) == ContainmentType.Contains)
                        //{
                        //    pointingDevice.PositionAdd(tempAdded);
                        //    Cursor.Position = new System.Drawing.Point((int)pointingDevice.Position.X, (int)pointingDevice.Position.Y);
                        //}
                        //else if (screenBounds_.Contains(new Vector2(pointingDevice.Position.X + tempAdded.X, pointingDevice.Position.Y)) == ContainmentType.Contains)
                        //{
                        //    pointingDevice.PositionAdd(new Vector2(tempAdded.X, 0f));
                        //    Cursor.Position = new System.Drawing.Point((int)pointingDevice.Position.X, (int)pointingDevice.Position.Y);
                        //}
                        //else if (screenBounds_.Contains(new Vector2(pointingDevice.Position.X, pointingDevice.Position.Y + tempAdded.Y)) == ContainmentType.Contains)
                        //{
                        //    pointingDevice.PositionAdd(new Vector2(0f, tempAdded.Y));
                        //    Cursor.Position = new System.Drawing.Point((int)pointingDevice.Position.X, (int)pointingDevice.Position.Y);
                        //}

                        //pointingDevice.oldLeftButton = pointingDevice.LeftButton;
                        //pointingDevice.oldRightButton = pointingDevice.RightButton;

                        //Debug.WriteLine(pointingDevice.Position);
                        // �{�^�����̍X�V
                        if (tempMouse.ButtonData != 0)
                        {
                            short tempbd = tempMouse.ButtonData;
                            for (int i = 1024; i > 0; i /= 2)
                            {
                                if (i <= tempbd)
                                {
                                    if ((RawMouseButtons)i == RawMouseButtons.LeftDown)
                                    {
                                        pointingDevice.LeftButton = Microsoft.Xna.Framework.Input.ButtonState.Pressed;
                                        pointingDevice.LeftDownPosition = pointingDevice.Position;
                                    }
                                    if ((RawMouseButtons)i == RawMouseButtons.LeftUp)
                                    {
                                        pointingDevice.LeftButton = Microsoft.Xna.Framework.Input.ButtonState.Released;
                                    }
                                    if ((RawMouseButtons)i == RawMouseButtons.RightDown)
                                    {
                                        pointingDevice.RightButton = Microsoft.Xna.Framework.Input.ButtonState.Pressed;
                                        pointingDevice.RightDownPosition = pointingDevice.Position;
                                    }
                                    if ((RawMouseButtons)i == RawMouseButtons.RightUp)
                                    {
                                        pointingDevice.RightButton = Microsoft.Xna.Framework.Input.ButtonState.Released;
                                    }
                                    if ((RawMouseButtons)i == RawMouseButtons.MiddleDown)
                                    {
                                        pointingDevice.MiddleButton = Microsoft.Xna.Framework.Input.ButtonState.Pressed;
                                        pointingDevice.MiddleDownPosition = pointingDevice.Position;
                                    }
                                    if ((RawMouseButtons)i == RawMouseButtons.MiddleUp)
                                    {
                                        pointingDevice.MiddleButton = Microsoft.Xna.Framework.Input.ButtonState.Released;
                                    }
                                    tempbd -= (short)i;
                                    if (tempbd == 0)
                                    {
                                        break;
                                    }
                                }
                            }
                        }

                        // �����͎��ԃJ�E���^�̃��Z�b�g
                        //pointingDevice.TimeCounterReset();
                    }
                }

                // ���{�^�����������܂܂�RawMouse���J�[�\����ێ�
                // �ŏ��ɔF�����ꂽRawMouse���D��I�ɃJ�[�\����ێ�
                /*if (pointingDevices_[0].LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                {
                    Cursor.Position = new System.Drawing.Point((int)pointingDevices_[0].Position.X, (int)pointingDevices_[0].Position.Y);
                }
                else
                {
                    for (PointingDevice pointingDevice = pointingDevices.first(); pointingDevice != null; pointingDevice = pointingDevices.next())
                    {
                        if (pointingDevice.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                        {
                            Cursor.Position = new System.Drawing.Point((int)pointingDevice.Position.X, (int)pointingDevice.Position.Y);
                        }
                    }
                }*/
            }
        }

        // �w�肵���w�b�_�̃f�o�C�X���폜
        public void RemovePointingDevice(int header)
        {
            PointingDevice removed = null;
            for (PointingDevice pointingDevice = pointingDevices.first(); pointingDevice != null; pointingDevice = pointingDevices.next())
            {
                if (pointingDevice.Header == header)
                {
                    removed = pointingDevice;
                    break;
                }
            }
            if (removed != null)
            {
                pointingDevices.remove(removed);
            }
        }

        // �󂯎�������b�Z�[�W��RawInput�̂��̂����m�F���đ�����I��
        protected override void WndProc(ref Message m)
        {
            const int WmInput = 0xFF;
            if (m.Msg == WmInput)
            {
                this.ProcessInputKey(ref m);
            }
            else
            {
                
                base.WndProc(ref m);
            }
        }

        // ���f�[�^�������₷�������H
        [DllImport("user32.dll")]
        private static extern int RegisterRawInputDevices(SRawInputDevice[] devices, int number, int size);
        [DllImport("user32.dll")]
        private static extern int GetRawInputData(IntPtr rawInput, int command, out SRawInput data, ref int size, int headerSize);

        #region �v���p�e�B
        /*public List<PointingDevice> PointingDevices
        {
            get
            {
                return pointingDevices_;
            }
        }*/

        public List<PointingDevice> touchDevices
        {
            get {
               return new List<PointingDevice>(touchMap.Values);
            }
        }
        /*public BoundingBox2D ScreenBounds
        {
            set
            {
                screenBounds_ = value;
            }
        }*/
        #endregion
    }
}
