using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Drawing.Drawing2D;
using System.Threading;

namespace LoadingTest
{
    public partial class Loading : Form
    {
        public Loading()
        {
            InitializeComponent();
            lblShowState.Text = "";
            myDel = new MyDelegate(Cal);
        }

        private MyDelegate myDel = null;

        #region 等待界面
        private int count = -1;
        private ArrayList images = new ArrayList();
        public Bitmap[] bitmap = new Bitmap[8];
        private int _value = 1;
        private Color _circleColor = Color.Red;
        private float _circleSize = 0.8f;      

        private int width = 200;//设置圆的宽
        private int height = 200;////设置圆的高

        public Bitmap DrawCircle(int j)
        {
            const float angle = 360.0F / 8;
            Bitmap map = new Bitmap(150, 150);
            Graphics g = Graphics.FromImage(map);

            g.TranslateTransform(width / 2.0F, height / 2.0F);
            g.RotateTransform(angle * _value);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int[] a = new int[8] { 25, 50, 75, 100, 125, 150, 175, 200 };
            for (int i = 1; i <= 8; i++)
            {
                int alpha = a[(i + j - 1) % 8];
                Color drawColor = Color.FromArgb(alpha, _circleColor);
                using (SolidBrush brush = new SolidBrush(drawColor))
                {
                    float sizeRate = 3.5F / _circleSize;
                    float size = width / (6 * sizeRate);
                    float diff = (width / 10.0F) - size;
                    float x = (width / 80.0F) + diff;
                    float y = (height / 80.0F) + diff;
                    g.FillEllipse(brush, x, y, size, size);
                    g.RotateTransform(angle);
                }
            }
            return map;
        }

        public void Draw()
        {
            for (int j = 0; j < 8; j++)
            {
                bitmap[7 - j] = DrawCircle(j);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            SetNewSize();
            base.OnResize(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            SetNewSize();
            base.OnSizeChanged(e);
        }

        private void SetNewSize()
        {
            int size = Math.Max(width, height);
            pictureBox.Size = new Size(size, size);
        }

        public void set()
        {
            for (int i = 0; i < 8; i++)
            {
                Draw();
                Bitmap map = new Bitmap((bitmap[i]), new Size(120, 110));
                images.Add(map);
            }
            pictureBox.Image = (Image)images[0];
            pictureBox.Size = pictureBox.Image.Size;
        }
       
        private void Timer_Tick(object sender, EventArgs e)
        {
            set();
            count = (count + 1) % 8;
            pictureBox.Image = (Image)images[count];
        }
        
        private void StartWaiting()
        {
            timer1.Start();
            pictureBox.Visible = true;

            progressBar1.Visible = true;
            progressBar1.Enabled = true;
        }

        private void StopWaiting()
        {
            timer1.Stop();
            pictureBox.Visible = false;

            progressBar1.Visible = false;
            progressBar1.Enabled = false;
        }

        #endregion

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (btnStart.Text == "开始")
            {
                btnStart.Text = "中止";
                lblShowState.Text = "开始加载……";
                StartWaiting();
                IAsyncResult result = myDel.BeginInvoke(MyCallBack, null);               
            }
            else
            {              
                //Thread.CurrentThread.Abort();//中止了主线程
                btnStart.Text = "开始";
                lblShowState.Text = "中止加载！";
            }
        }
       
        private void MyCallBack(IAsyncResult result)
        {
            int res = myDel.EndInvoke(result);

            //异步显示结果：result.AsyncState字段用来封装回调时自定义的参数，object类型
            if (lblShowState.InvokeRequired)
            {
                // 当一个控件的InvokeRequired属性值为真时，说明有一个创建它以外的线程想访问它
                Action<string> actionDelegate = (x) => { this.lblShowState.Text = x.ToString(); };
                // 或者
                // Action<string> actionDelegate = delegate(string txt) { this.label2.Text = txt; };
                this.lblShowState.Invoke(actionDelegate, res.ToString());
            }           
            //StopWaiting();
            
        }

        private int Cal()
        {
            int result = 0;
            for (int i = 0; i < 10; i++)
            {
                result += i;
                Thread.Sleep(300);//模拟耗时任务              
            }
            return result;
        }

        private delegate int MyDelegate();      
       
        private void lblShowState_TextChanged(object sender, EventArgs e)
        {
            StopWaiting();
        }

        private void Loading_Load(object sender, EventArgs e)
        {

        }

    }
}
