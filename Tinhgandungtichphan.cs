using System;
using System.Collections.Generic;
using System.Text;
using Alchemi.Core;
using Alchemi.Core.Owner;


namespace TrapezoidRule
{
    class TrapezoidRule : GApplication
    {
        public static GApplication App = new GApplication();
        private static int[] matrix;//Mang chua so can kiem tra
        private static int NumPerThread;//So luong so trong 1 thread
        private static double kq;
        private static DateTime start;

        [STAThread]
        static void Main(string[] args)
        {
            int n;
            string host;
            Random random = new Random();
            Console.Write("Host[localhost]:");
            host = Console.ReadLine();
            if (host.Length < 1)
            {
                host = "localhost";
            }
            Console.Write("Dua vao can tren cua tich phan:"); //9
            n = Int32.Parse(Console.ReadLine());
            Console.Write("Dua vao so luong so phep tinh cho 1 thread:"); //3
            NumPerThread = Int32.Parse(Console.ReadLine());
            matrix = new int[n];

            int NumRemain = n; // NumRemain =  số luong số cần kiểm tra =9
            int NumCur = 0; // 
            while (NumRemain > 0)
            {
                int NumberOfThread;
                if (NumRemain > NumPerThread) // số lượng số cần check >  số lượng số của 1 luồng
                {
                    NumberOfThread = NumPerThread;
                }
                else
                {
                    NumberOfThread = NumRemain;
                }
                int[] Nums = new int[NumberOfThread];
                App.Threads.Add(new TrapezoidRuleCount(NumRemain, NumRemain - NumPerThread, NumPerThread)); // NumCur-NumperThread, NumCur-NumperThread + 1
                NumRemain -= NumberOfThread; //6 3
            }
            // thiết lập liên kết
            App.Connection = new GConnection("localhost", 9000, "user", "user");
            // Thêm vào các mô đun cần thiết
            App.Manifest.Add(new ModuleDependency(typeof(TrapezoidRuleCount).Module));
            // Thêm vào sự kiện threadFinish
            App.ThreadFinish += new GThreadFinish(App_ThreadFinish);

            App.ApplicationFinish += new GApplicationFinish(App_ApplicationFinish);
            start = DateTime.Now;
            Console.WriteLine("Thread started!");
            App.Start();
            Console.ReadLine();

        }

        private static void App_ThreadFinish(GThread thread)
        {
            TrapezoidRuleCount pnc = (TrapezoidRuleCount)thread;
            Console.WriteLine("Ket qua cua tung luong:");
            Console.WriteLine(pnc.ans);
            kq += pnc.ans;
            Console.WriteLine("So{0}-{1} hoan thanh", pnc.StartNums, pnc.EndNums);
            Console.WriteLine("-----------------------------------------");
        }

        private static void App_ApplicationFinish()
        {
            Console.WriteLine("Ket qua cua tich phan:");
            Console.WriteLine(kq);
            Console.WriteLine("Hoan thanh sau {0} seconds.", DateTime.Now - start);
        }
    }
    [Serializable]
    class TrapezoidRuleCount : GThread
    {
        public int StartNums;
        public int EndNums;
        public int n;
        public double ans;
        public TrapezoidRuleCount(int EndNums, int StartNums, int n)
        {
            this.StartNums = StartNums;
            this.EndNums = EndNums;
            this.n = n;
        }
        public override void Start()
        {
            double h = (double)((EndNums - StartNums) / n);

            double[] X = new double[n + 1];
            double[] Y = new double[n + 1];

            for (int i = 0; i < n + 1; i++)
            {
                X[i] = StartNums + i * h;
                Y[i] = Math.Pow(X[i], 2) + 1;
            }
            ans = 0;
            for (int i = 0; i < n + 1; i++)
            {
                if ((i == 0) || (i == n))
                {
                    ans = ans + Y[i];
                }
                else
                {
                    ans = ans + 2 * Y[i];
                }
            }
            ans = (ans * h) / 2;

        }
    }
}
