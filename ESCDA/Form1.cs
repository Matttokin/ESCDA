using System;
using System.Windows.Forms;
using System.Numerics;

namespace ESCDA
{
    public partial class Form1 : Form
    {
        private int q = 19;
        private int p = 17;
        private int A = 2;
        private int B = 2;
        private int[] P;
        private int[] Q;
        private int x;
        private int[] curvePoint;
        public Form1()
        {
            InitializeComponent();
        }

        private int[] elCurve()
        {
            int x = 0;
            //пока-что берем ближайшую точку , позже сделаю рандом
            while (true)
            {
                double y = Math.Sqrt(mod((int)(Math.Pow(x, 3) + A * x + B) , p));
                if (y == Math.Truncate(y))
                {
                    return new int[2] { x, (int)y };
                }
                x++;
            }
        }
        private void genKey()
        {
            P = elCurve(); //генерируем кривую
            Random rD = new Random();
            x = rD.Next(1, q - 1); //закрытый ключ
            Q = multiply(P, x); //открытый ключ
        }
        private void signature(int h)
        {
            //дальше расчет по формулам
            choiceK:
            Random rD = new Random();
            int k = rD.Next(1, q - 1);
            curvePoint = multiply(P, k);
            int r = mod(curvePoint[0] , q);
            if (r == 0) goto choiceK;

            int kInv = (int)(BigInteger.ModPow(k, q-2, q));
            int s = mod(kInv * (h + x * r) , q);
            if (s == 0) goto choiceK;

            textBox3.Text = r.ToString();
            textBox4.Text = s.ToString();
            textBox6.Text = r.ToString();
            textBox7.Text = s.ToString();
        }
        private void checkSignature(int h,int r, int s)
        {
            if ((1 <= r) && (r < q) && (1 <= s) && (s < q))
            {
                int w = (int)BigInteger.ModPow(s, (q - 2), q); // s^-1

                int u1 = mod((w * h), q);
                int u2 = mod((w * r), q);

                int[] uP  = multiply(P, u1);
                int[] uQ = multiply(Q, u2);
                int[] point = add(uP, uQ);

                int u = mod(point[0] , q);
                if(u == r)
                {
                    textBox8.Text = "Подпись верна";
                } else
                {
                    textBox8.Text = "Подпись ошибочна";
                }
            } else
            {
                MessageBox.Show("Неверные r или s");
            }
        }
        //самописная функция хеширования, ибо лень получать числа из md5 или искать готовую, которая возвращает число, а не строку
        private int hashFunc(string str)
        {
            int outInt = 0;
            for(int i = 0; i < str.Length; i++)
            {
                outInt += (int)(str[i] + 71)*10; //71 и 10 случайные числа, которые первыми "пришли в голову"

            }
            return outInt;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int h = hashFunc(textBox2.Text);
            if(h > 0)
            {
                signature(h);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            q = Convert.ToInt32(textBox1.Text);
            if(q > 3)
            {
                genKey();
                button1.Enabled = true;
                button2.Enabled = true;
            } else
            {
                MessageBox.Show("Значение должно быть больше 3 и простым числом!");
                button1.Enabled = false;
                button2.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int h = hashFunc(textBox5.Text);
            int r = Convert.ToInt32(textBox6.Text);
            int s = Convert.ToInt32(textBox7.Text);
            if ((h > 0) || (r > 0) || (s > 0))
            {
                checkSignature(h, r, s);
            }
        }
        //сложение 2х точек
        public  int[] add(int[] p1, int[] p2)
        {
            int m;
            if (p1[0] == p2[0] && p1[1] == p2[1])
            {
                m = (int)((3 * p1[0] * p1[0] + A) * (int)(BigInteger.ModPow((2 * p1[1]), p - 2, p))); //BigInteger.ModPow(ххх, p - 2, p) это xxx^-1 (возведение в степень по модулю)
            } else
            {
                m = ((p2[1] - p1[1]) * (int)(BigInteger.ModPow((p2[0] - p1[0]), p - 2, p)));
            }
            //расчеты по формулам
            int xR = mod((int)(Math.Pow(m, 2) - p1[0] - p2[0]), p);

            int yR = mod((int)((-p1[1] + m * (p1[0] - xR))), p);
            return new int[2] { xR, yR };

        }
        //умножение точки кривой на скаляр
        public int[] multiply(int[] p1,int count)
        {
            if (count == 1) return p1;
            int[] p2 = add(p1, p1);
            for(int i = 1;i < count - 1; i++)
            {
                int[] p3 = add(p1, p2);
                p2 =new int[2] { p3[0], p3[1] };
            }
            return p2;
        }
        //деление по модулю
        public int mod(int x,int y)
        {
            if(x >= 0)
            {
                return x % y;
            } else
            {
                return y + (x % y);
            }
        }
    }
}
