using System;
using System.Windows.Forms;
using System.Numerics;

namespace ESCDA
{
    public partial class Form1 : Form
    {
        private int q = 17;
        private int A = 2;
        private int B = 2;
        private int[] P;
        private int[] Q;
        private int x;
        private int[] curvePoint;
        public Form1()
        {
            InitializeComponent();

            int[] Q1 = add(new int[2] { 5,1}, new int[2] { 16, 4 });
            Console.WriteLine(Q1[0] + " " + Q1[1]);
        }

        private int[] elCurve()
        {
            int x = 0;
            while (true)
            {
                double y = Math.Sqrt(mod((int)(Math.Pow(x, 3) + A * x + B) , q));
                if (y == Math.Truncate(y) && x == 5)
                {
                    return new int[2] { x, (int)y };
                    //return new int[2] { 5, 1 };
                }
                x++;
            }
        }
        private void genKey()
        {
            P = elCurve();
            Random rD = new Random();
            x = rD.Next(1, q - 1);
            x = 5;
            Q = multiply(P, x);
            Console.WriteLine(Q[0] + " " + Q[1]);
        }
        private void signature(int h)
        {
            choiceK:
            Random rD = new Random();
            int k = rD.Next(1, q - 1);
            k = 3;
            //curvePoint = new int[2] { P[0] * k, P[1] * k };
            curvePoint = multiply(P, k);
            Console.WriteLine(curvePoint[0] + " " + curvePoint[1]);
            int r = mod(curvePoint[0] , q);
            if (r == 0) goto choiceK;

            //int s = Convert.ToInt32(Math.Pow(k, -1) * (h + x * r)) % q;
            int kInv = (int)(BigInteger.ModPow(k, q - 2, q));
            Console.WriteLine("kInv" + k +" "+ kInv * k % q);
            int s = mod(kInv * (h + x * r) , q);
            Console.WriteLine("s" + s);
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
                int w = (int)BigInteger.ModPow(s, (q - 2), q);

                Console.WriteLine("sInv" + s * w % q);
                //int u1 = Convert.ToInt32((Math.Pow(s, -1) * h)) % q;
                //int u2 = Convert.ToInt32((Math.Pow(s, -1) * r)) % q;

                //int u1 = mod((int)(Math.Pow(s, (q - 2)) * h) , q);
                //int u2 = mod((int)(Math.Pow(s, (q - 2)) * r) , q);
                int u1 = mod((w * h), q);
                int u2 = mod((w * r), q);
                Console.WriteLine("w " + w);
                Console.WriteLine("u1 - u2 "+u1 + " " + u2);

                int[] uP  = multiply(P, u1);
                int[] uQ = multiply(Q, u2);
                int[] point = add(uP, uQ);


                Console.WriteLine("uP "+ uP[0] + " " + uP[1]);
                Console.WriteLine("uQ " + uQ[0] + " " + uQ[1]);

                int u = mod(point[0] , q);
                Console.WriteLine("point " + point[0] + " " + point[1]);
                Console.WriteLine(u + " " + r);
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
        private int hashFunc(string str)
        {
            int outInt = 0;
            for(int i = 0; i < str.Length; i++)
            {
                outInt += (int)str[i];//*71
                //outInt *= 100;
                //outInt %= 65535;
            }
            return 8;
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
        public  int[] add(int[] p1, int[] p2)
        {
            int m;
            if (p1[0] == p2[0] && p1[1] == p2[1])
            {
                m = mod((int)((3 * Math.Pow(p1[0], 2) + A) * (int)(BigInteger.ModPow((2 * p1[1]), q - 2, q))) , q);
            } else
            {
                m = mod(((p2[1] - p1[1]) * (int)(BigInteger.ModPow((p2[0] - p1[0]), q - 2, q))) , q);
            }

            int xR = mod((int)(Math.Pow(m, 2) - p1[0] - p2[0]), q);
            //int yR = mod((int)((p1[1] + m * (xR - p1[0]))), q);

            int yR = mod((int)((-p1[1] + m * (p1[0] - xR))), q);
            //Console.WriteLine(xR + " " + yR + " " + m);
            return new int[2] { xR, yR };

        }
        public int[] multiply(int[] p1,int count)
        {
            if (count == 1) return p1;
            int[] p2 = add(p1, p1);
            for(int i = 1;i < count - 1; i++)
            {
                int[] p3 = add(p1, p2);
                p2 =new int[2] { p3[0], p3[1] };
                //Console.WriteLine(p2[0] + " " + p2[1]);
            }
            return p2;
        }
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
