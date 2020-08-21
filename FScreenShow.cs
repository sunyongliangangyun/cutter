using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ScreenCapturer
{
    public partial class FScreenShow : Form
    {
        public static int TouchScreenID = -1;
        /// <summary>
        /// 遮罩层画刷
        /// </summary>
        private Brush brush_mask = new SolidBrush(Color.FromArgb(125, Color.Black));
        /// <summary>
        /// 文字背景画刷
        /// </summary>
        private Brush brush_info = new SolidBrush(Color.FromArgb(200, 0, 0, 0));
        /// <summary>
        /// 截图信息类
        /// </summary>
        private CaptureRectangle captureRect = new CaptureRectangle();
        /// <summary>
        /// 屏幕序号
        /// </summary>
        private int ScreenIndex = 0;
        /// <summary>
        /// 当前屏幕基础截图
        /// </summary>
        public Bitmap ScreenCapture;
        /// <summary>
        /// 是否固定截图大小(固定后不允许调整截图大小)
        /// </summary>
        public bool FixRect = true;
        public int FixWidth = 200;
        public int FixHeight = 200;
        /// <summary>
        /// 是否保存到剪切板
        /// </summary>
        public bool SaveToClipboard = true;
        /// <summary>
        /// 是否保存到目录
        /// </summary>
        public bool SaveToFolder = false;
        public string Folder;
        /// <summary>
        /// 是否需要在保存时调整截图大小
        /// </summary>
        public bool Transform = false;
        public int TransformWidth = 0;
        public int TransformHeight = 0;
        /// <summary>
        /// 截图时的标线颜色
        /// </summary>
        public Color LineColor = Color.Red;
        /// <summary>
        /// 当前鼠标点
        /// </summary>
        private Point now = Point.Empty;
        private Point PointLT
        {
            get => captureRect.PLT;
            set => captureRect.PLT = value;
        }
        private Point PointRB
        {
            get => captureRect.PRB;
            set => captureRect.PRB = value;
        }
        private int CaptureWidth => captureRect.CaptureWidth;
        private int CaptureHeight => captureRect.CaptureHeight;
        private bool InDrawMode;
        private CaptrueHitType hitType = CaptrueHitType.None;

        private int prex, prey, endx, endy = 0;

        #region wat
        private const uint WS_EX_LAYERED = 0x80000;
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int GWL_STYLE = (-16);
        private const int GWL_EXSTYLE = (-20);
        private const int LWA_ALPHA = 0;


        [DllImport("user32", EntryPoint = "SetWindowLong")]
        private static extern uint SetWindowLong(
       IntPtr hwnd,
       int nIndex,
       uint dwNewLong
       );

        [DllImport("user32", EntryPoint = "GetWindowLong")]
        private static extern uint GetWindowLong(
        IntPtr hwnd,
        int nIndex
        );

        [DllImport("user32", EntryPoint = "SetLayeredWindowAttributes")]
        private static extern int SetLayeredWindowAttributes(
        IntPtr hwnd,
        int crKey,
        int bAlpha,
        int dwFlags
        );

        /// <summary>
        　　/// 设置窗体具有鼠标穿透效果
        　　/// </summary>
        　　/// <param name="flag">true穿透，false不穿透</param>
        public void SetPenetrate(bool flag = true)
        {
            uint style = GetWindowLong(this.Handle, GWL_EXSTYLE);
            if (flag)
                SetWindowLong(this.Handle, GWL_EXSTYLE, style | WS_EX_TRANSPARENT | WS_EX_LAYERED);
            else
                SetWindowLong(this.Handle, GWL_EXSTYLE, style & ~(WS_EX_TRANSPARENT | WS_EX_LAYERED));
            SetLayeredWindowAttributes(this.Handle, 0, 100, LWA_ALPHA);
        }
        #endregion

        public static void ClearTouchID()
        {
            TouchScreenID = -1;
        }
        public FScreenShow(int index,int prex,int prey,int endx,int endy)
        {
            InitializeComponent();
            ScreenIndex = index;
            this.prex = prex;
            this.prey = prey;
            this.endx = endx;
            this.endy = endy;
            CopyScreen();
        }
        private void FScreen_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void Draw(Graphics graphics = null)
        {
            using (Bitmap bmp = (Bitmap)ScreenCapture.Clone())
            {
                using(Graphics g = Graphics.FromImage(bmp))
                {
                    // 画透明遮罩层
                    g.FillRectangle(brush_mask, 0, 0, bmp.Width, bmp.Height);

                    //if (captureRect.CanDraw)
                    //{
                        // 没有异或笔,只能重画一次被框选的范围, 覆盖遮罩层
                        g.DrawImage(ScreenCapture, captureRect.CaptureRect, PointLT.X, PointLT.Y, PointRB.X-PointLT.X, PointRB.Y-PointLT.Y, GraphicsUnit.Pixel);
                        // 画矩形
                        captureRect.Draw(g, LineColor);
                    //}
                }
                if (graphics == null)
                {
                    using(Graphics g = this.CreateGraphics())
                    {
                        g.DrawImage(bmp, 0, 0);
                    }
                }
                else
                {
                    graphics.DrawImage(bmp, 0, 0);
                }
            }
        }
        private void FScreen_Load(object sender, EventArgs e)
        {
            this.Left = Screen.AllScreens[ScreenIndex].WorkingArea.X;
            this.WindowState = FormWindowState.Maximized;
            PointLT = new Point(prex, prey);
            PointRB = new Point(endx, endy);
            SetPenetrate();

        }
        private void CopyScreen()
        {
             ScreenCapture = new Bitmap(Screen.AllScreens[ScreenIndex].Bounds.Width, Screen.AllScreens[ScreenIndex].Bounds.Height);
           // ScreenCapture = new Bitmap(200, 500);
            using (Graphics g = Graphics.FromImage(ScreenCapture))
            {
                g.CopyFromScreen(new Point(Screen.AllScreens[ScreenIndex].WorkingArea.X, 0), new Point(0, 0), Screen.AllScreens[ScreenIndex].Bounds.Size);
            }
        }

        private void SaveClipboard(Bitmap image)
        {
            if (Transform && TransformWidth > 0 && TransformHeight > 0)
            { 
                using(var target = new Bitmap(image, TransformWidth, TransformHeight))
                {
                    Clipboard.SetImage(target);
                }
            }
            else
            {
                Clipboard.SetImage(image);
            }
        }
        private void FScreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            brush_mask.Dispose();
            brush_info.Dispose();
        }
        private void FScreen_Paint(object sender, PaintEventArgs e)
        {
            Draw(e.Graphics);
         
        }
    }    
}
