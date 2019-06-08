using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
namespace Invisiblehand
{
    public partial class CalibrationForm : Form
    {
        private const int NumberOfPoint = 9; // 버튼 갯수
        private const int NumberOfCalibrationData = 6; // 캘리브레이션 데이터 갯수
        private static int[,] ReferencePoint = new int[NumberOfPoint, 2]; // 이상적인 포인팅 위치
        private static int[,] SamplePoint = new int[NumberOfPoint, 2]; // 실제 포인팅 위치
        private static double KX1, KX2, KX3, KY1, KY2, KY3; // 캘리브레이션 알고리즘에서 사용됨
        private int SettingCount = 0; // 현재 클릭된 버튼 갯수
        private Button[] array; // 버튼을 배열로 사용하기 위함

        public CalibrationForm()
        {
            InitializeComponent();
            InitalizePoint();
            InitalizeArray();
            InitalizeButton();
        }

        // 캘리브레이션 이니셜라이징
        void InitalizePoint()
        {
            for (int i = 0; i < NumberOfPoint; i++)
            {
                ReferencePoint[i, 0] = 0;
                ReferencePoint[i, 1] = 0;
                SamplePoint[i, 0] = 0;
                SamplePoint[i, 1] = 0;
            }
            KX1 = 0;
            KX2 = 0;
            KX3 = 0;
            KY1 = 0;
            KY2 = 0;
            KY3 = 0;
        }

        void InitalizeArray()
        {
            // 버튼을 배열로 사용(처음에 잘못짬)
            array = new Button[NumberOfPoint + 1];
            array[1] = button1;
            array[2] = button2;
            array[3] = button3;
            array[4] = button4;
            array[5] = button5;
            array[6] = button6;
            array[7] = button7;
            array[8] = button8;
            array[9] = button9;
        }

        void InitalizeButton()
        {
            // 설정된 버튼의 수 초기화
            SettingCount = 0;

            // 버튼 1번 부터 유도하기
            array[1].BackColor = Color.DodgerBlue;
            array[1].Text = "1";
            array[1].Image = null;

            // 버튼 2번 부터는 White로 설정
            for (int i = 2; i <= NumberOfPoint; i++)
            {
                array[i].BackColor = Color.White;
                array[i].Text = i.ToString();
                array[i].Image = null;
                
            }
        }

        // 캘리브레이션 클릭
        private void CalibrationClick(object sender, EventArgs e)
        {
            Button triggeredButton = (Button)sender;

            // DodgerBlue 버튼이 눌려야 하는 버튼이다.
            if (triggeredButton.BackColor == Color.DodgerBlue)
            {
                // 눌린 버튼의 색깔을 바꿈
                triggeredButton.BackColor = SystemColors.ControlDark;
                triggeredButton.Image = Properties.Resources.hi_3;

                // 현재 눌린 버튼이 번호
                int i = Convert.ToInt32(triggeredButton.Name.Substring(6));

                // 다음 버튼 색깔을 유도함
                if (i < NumberOfPoint)
                    array[i + 1].BackColor = Color.DodgerBlue;

                // 눌린 버튼의 - 1 (위의 일부 데이터들때문)
                int j = i - 1;

                // 이상 위치
                Point buttonPoint = triggeredButton.Parent.PointToScreen(triggeredButton.Location);
                ReferencePoint[j, 0] = (buttonPoint.X + triggeredButton.Size.Width / 2);
                ReferencePoint[j, 1] = (buttonPoint.Y + triggeredButton.Size.Height / 2);

                // 실제 위치
                int CursorX = Cursor.Position.X;
                int CursorY = Cursor.Position.Y;
                SamplePoint[j, 0] = CursorX;
                SamplePoint[j, 1] = CursorY;

                // 디버그용 
                string idealine = "Ideal Point : " + ReferencePoint[j, 0].ToString() + ", " + ReferencePoint[j, 1].ToString();
                Debug.WriteLine(idealine);
                string sampleline = "sample Point : " + CursorX.ToString() + ", " + CursorY.ToString();
                Debug.WriteLine(sampleline);

                //세팅 수 증가
                SettingCount++;
            }
            else
               
            ShowFailPopup();
            
            
            // 9개가 모두 선택되면 팝업 호출
            if (SettingCount >= NumberOfPoint)
                ShowPopup();

        }

        private void ShowPopup()
        {
            // 팝업 호출
            CalibrationPopupForm popup = new CalibrationPopupForm();

            // 팝업을 정 가운데로 호출
            popup.StartPosition = FormStartPosition.Manual;
            popup.Location = new Point(this.Width / 2 - popup.Width / 2, this.Height / 2 - popup.Height / 2);

            // 팝업 호출
            DialogResult dialogresult = popup.ShowDialog();
            bool result = false;
            if (dialogresult == DialogResult.OK)
            {
                Console.WriteLine("You clicked OK");
                result = true;
                GetCalibrationCoefficient();
                DebugCalibration();
                WriteCalibrationFile();
            }
            else if (dialogresult == DialogResult.Cancel)
            {
                Console.WriteLine("You clicked Cancel or X button in the top right corner");
                result = false;
                InitalizePoint();
                InitalizeButton();
            }
            popup.Dispose();

            if (result)
                this.Dispose();
        }

        private void ShowFailPopup()
        {
            for (int k = 1; k <= SettingCount; k++)
            {
                array[k].Image = Properties.Resources.no_1;
            }

            // 실패 팝업
            CalibrationFailForm popup = new CalibrationFailForm();

            // 팝업을 정 가운데로 호출
            popup.StartPosition = FormStartPosition.Manual;
            popup.Location = new Point(this.Width / 2 - popup.Width / 2, this.Height / 2 - popup.Height / 2);

            DialogResult dialogresult = popup.ShowDialog();

            // 실패 버튼 클릭
            if (dialogresult == DialogResult.Cancel)
            {
                Console.WriteLine("Calibration Fail. Please Retry.");
                InitalizePoint();
                InitalizeButton();
            }
        }
        // Write CSV
        void WriteCalibrationFile()
        {
            Debug.WriteLine("WriteCalibrationFile");

            // String 이어 붙이기
            var csv = new StringBuilder();
            csv.AppendLine(string.Format("{0}", KX1.ToString()));
            csv.AppendLine(string.Format("{0}", KX2.ToString()));
            csv.AppendLine(string.Format("{0}", KX3.ToString()));
            csv.AppendLine(string.Format("{0}", KY1.ToString()));
            csv.AppendLine(string.Format("{0}", KY2.ToString()));
            csv.AppendLine(string.Format("{0}", KY3.ToString()));

            // 파일 쓰기
            File.WriteAllText("calibration.csv", csv.ToString());

        }

        void ReadCalibrationFile()
        {
            Debug.WriteLine("ReadCalibrationFile");

            string strFile = "calibration.csv";
            string[] row = new string[NumberOfCalibrationData];

            using (FileStream fs = new FileStream(strFile, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8, false))
                {
                    string strLineValue = null;
                    int i = 0;
                    while ((strLineValue = sr.ReadLine()) != null)
                    {
                        row[i] = strLineValue;
                        i++;
                    }
                }
            }

            // Read CSV
            KX1 = Convert.ToDouble(row[0]);
            KX2 = Convert.ToDouble(row[1]);
            KX3 = Convert.ToDouble(row[2]);
            KY1 = Convert.ToDouble(row[3]);
            KY2 = Convert.ToDouble(row[4]);
            KY3 = Convert.ToDouble(row[5]);

        }

        // Do Calibration
        void DoCalibration(ref int Px, ref int Py)
        {
            // do calibration for point
            // (Px, Py) using the calculated coefficients
            Px = (int)(KX1 * (Px) + KX2 * (Py) + KX3 + 0.5);
            Py = (int)(KY1 * (Px) + KY2 * (Py) + KY3 + 0.5);
        }

        // Get Calibration
        int GetCalibrationCoefficient() // calculate the coefficients for calibration
        {
            //algorithm: KX1, KX2, KX3, KY1, KY2, KY3
            int i;
            int Points = NumberOfPoint;
            double[] a = new double[3];
            double[] b = new double[3];
            double[] c = new double[3];
            double[] d = new double[3];

            double k;
            if (Points < 3)
            {
                return 0;
            }
            else
            {
                if (Points == 3)
                {
                    for (i = 0; i < Points; i++)
                    {
                        a[i] = (double)(SamplePoint[i, 0]);
                        b[i] = (double)(SamplePoint[i, 1]);
                        c[i] = (double)(ReferencePoint[i, 0]);
                        d[i] = (double)(ReferencePoint[i, 1]);
                    }
                }
                else if (Points > 3)
                {
                    for (i = 0; i < 3; i++)
                    {
                        a[i] = 0;
                        b[i] = 0;
                        c[i] = 0;
                        d[i] = 0;
                    }
                    for (i = 0; i < Points; i++)

                    {
                        a[2] = a[2] + (double)(SamplePoint[i, 0]);
                        b[2] = b[2] + (double)(SamplePoint[i, 1]);
                        c[2] = c[2] + (double)(ReferencePoint[i, 0]);
                        d[2] = d[2] + (double)(ReferencePoint[i, 1]);
                        a[0] = a[0] + (double)(SamplePoint[i, 0]) * (double)(SamplePoint[i, 0]);
                        a[1] = a[1] + (double)(SamplePoint[i, 0]) * (double)(SamplePoint[i, 1]);
                        b[0] = a[1];
                        b[1] = b[1] + (double)(SamplePoint[i, 1]) * (double)(SamplePoint[i, 1]);
                        c[0] = c[0] + (double)(SamplePoint[i, 0]) * (double)(ReferencePoint[i, 0]);
                        c[1] = c[1] + (double)(SamplePoint[i, 1]) * (double)(ReferencePoint[i, 0]);
                        d[0] = d[0] + (double)(SamplePoint[i, 0]) * (double)(ReferencePoint[i, 1]);
                        d[1] = d[1] + (double)(SamplePoint[i, 1]) * (double)(ReferencePoint[i, 1]);
                    }
                    a[0] = a[0] / a[2];
                    a[1] = a[1] / b[2];
                    b[0] = b[0] / a[2];
                    b[1] = b[1] / b[2];
                    c[0] = c[0] / a[2];
                    c[1] = c[1] / b[2];
                    d[0] = d[0] / a[2];
                    d[1] = d[1] / b[2];
                    a[2] = a[2] / Points;
                    b[2] = b[2] / Points;
                    c[2] = c[2] / Points;
                    d[2] = d[2] / Points;
                }
                k = (a[0] - a[2]) * (b[1] - b[2]) - (a[1] - a[2]) * (b[0] - b[2]);
                KX1 = ((c[0] - c[2]) * (b[1] - b[2]) - (c[1] - c[2]) * (b[0] - b[2])) / k;
                KX2 = ((c[1] - c[2]) * (a[0] - a[2]) - (c[0] - c[2]) * (a[1] - a[2])) / k;
                KX3 = (b[0] * (a[2] * c[1] - a[1] * c[2]) + b[1] * (a[0] * c[2] - a[2] * c[0]) + b[2] * (a[1] * c[0] -
               a[0] * c[1])) / k;
                KY1 = ((d[0] - d[2]) * (b[1] - b[2]) - (d[1] - d[2]) * (b[0] - b[2])) / k;
                KY2 = ((d[1] - d[2]) * (a[0] - a[2]) - (d[0] - d[2]) * (a[1] - a[2])) / k;
                KY3 = (b[0] * (a[2] * d[1] - a[1] * d[2]) + b[1] * (a[0] * d[2] - a[2] * d[0]) + b[2] * (a[1] * d[0] -
               a[0] * d[1])) / k;
                return Points;
            }
        }

        // Debug Calibration Data
        void DebugCalibration()
        {
            Debug.WriteLine(
                "KX1 : " + KX1.ToString() +
                "KX2 : " + KX2.ToString() +
                "KX3 : " + KX3.ToString() +
                "KY1 : " + KY1.ToString() +
                "KY2 : " + KY2.ToString() +
                "KY3 : " + KY3.ToString());
        }
    }
}
