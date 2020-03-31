using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ESCDA
{
    public partial class Form1 : Form
    {
        private int q = 11;
        private int A = 3;
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
            while (true)
            {
                double y = Math.Sqrt((Math.Pow(x, 3) + A * x + B) % q);
                if (y == Math.Truncate(y))
                {
                    return new int[2] { x, (int)y };
                }
                x++;
            }
        }
        private void genKey()
        {
            P = elCurve();
            Random rD = new Random();
            x = rD.Next(1, q - 1);
            Q = new int[2] { P[0] * x, P[1] * x };
        }
        private void signature(int h)
        {
            choiceK:
            Random rD = new Random();
            int k = rD.Next(1, q - 1);
            curvePoint = new int[2] { P[0] * k, P[1] * k };
            int r = curvePoint[0] % q;
            if (r == 0) goto choiceK;

            double kInv = Math.Pow(k, -1);
            int s = Convert.ToInt32(kInv * (h + x * r));
            if (s == 0) goto choiceK;

            textBox3.Text = r.ToString();
            textBox4.Text = s.ToString();
            textBox6.Text = r.ToString();
            textBox7.Text = s.ToString();
        }
        private void checkSignature(int h,int r, int s)
        {
            if (true)
            {
                int u1 = Convert.ToInt32((Math.Pow(s, -1) * h) % q);
                int u2 = Convert.ToInt32((Math.Pow(s, -1) * r) % q);

                int[] uP = new int[2] { P[0] * u1, P[1] * u1 };
                int[] uQ = new int[2] { Q[0] * u2, Q[1] * u2 };
                int[] point = new int[2] { uP[0] + uQ[0], uP[1] + uQ[1] };

                int u = point[0] % q;
                if(u == r)
                {
                    textBox8.Text = "Подпись верна";
                } else
                {
                    textBox8.Text = "Подпись ошибочна";
                }
            }
        }
        private int hashFunc(string str)
        {
            int outInt = 0;
            for(int i = 0; i < str.Length; i++)
            {
                outInt += (int)str[i] *71;
                outInt *= 100;
                outInt %= 65535;
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
    }
}
