using System;
using System.Collections.Generic;
using System.Text;
using Alchemi.Core;
using Alchemi.Core.Owner;


namespace PerfectSquare
{
    class PerfectSquare : GApplication
    {
        public static GApplication App = new GApplication();
        private static double[] matrix;//Mang chua so can kiem tra
        private static int NumPerThread;//So luong so trong 1 thread
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
            Console.Write("Dua vao so luong so can kiem tra:");
            n = Int32.Parse(Console.ReadLine());
            Console.Write("Dua vao so luong so cho 1 thread:");
            NumPerThread = Int32.Parse(Console.ReadLine());
            matrix = new double[n];
            for (int i = 0; i < n; i++)
            {
                matrix[i] = i + 1;
            }
            int NumRemain = n;
            int NumCur = 0;
            while (NumRemain > 0)
            {
                int NumberOfThread;
                if (NumRemain > NumPerThread)
                {
                    NumberOfThread = NumPerThread;
                }
                else
                {
                    NumberOfThread = NumRemain;
                }
                double[] Nums = new double[NumberOfThread];
                for (int i = 0; i < NumberOfThread; i++)
                {
                    Nums[i] = matrix[NumCur];
                    NumCur++;
                }
                App.Threads.Add(new PSNumberCheck(NumCur - NumPerThread, Nums));
                NumRemain -= NumberOfThread;
            }
            App.Connection = new GConnection("localhost", 9000, "user", "user");
            App.Manifest.Add(new ModuleDependency(typeof(PSNumberCheck).Module));
            App.ThreadFinish += new GThreadFinish(App_ThreadFinish);
            App.ApplicationFinish += new GApplicationFinish(App_ApplicationFinish);
            start = DateTime.Now;
            Console.WriteLine("Thread started!");
            App.Start();
            Console.ReadLine();
        }

        private static void App_ThreadFinish(GThread thread)
        {
            PSNumberCheck pnc = (PSNumberCheck)thread;
            Console.WriteLine("So {0}-{1} hoan thanh", pnc.StartNums, pnc.StartNums + pnc.n - 1);
            for (int i = 0; i < pnc.n; i++)
            {
                if (pnc.PerfectSquare_true[i] != -1)
                    Console.WriteLine("{0}, ", pnc.PerfectSquare_true[i]);
            }    
        }

        private static void App_ApplicationFinish()
        {
            Console.WriteLine("Hoan thanh sau {0} seconds.", DateTime.Now - start);
        }
    }
    [Serializable]
    class PSNumberCheck : GThread
    {
        public int StartNums;
        public double[] PerfectSquare;
        public double[] PerfectSquare_true;
        public int n;
        public PSNumberCheck(int StartNums, double[] PerfectSquare)
        {
            this.StartNums = StartNums;
            this.PerfectSquare = PerfectSquare;
            this.n = PerfectSquare.GetLength(0);
            this.PerfectSquare_true = new double[n];
            for (int i = 0; i < this.n; i++)
                PerfectSquare_true[i] = -1;
        }
        public bool PScheck(double n)
        {
            return Math.Sqrt(n) == (int)Math.Sqrt(n);
        }
        public override void Start()
        {
            for (int i = 0; i < n; i++)
            {
                if (this.PScheck(PerfectSquare[i]))
                    PerfectSquare_true[i] = PerfectSquare[i];
            }
        }
    }
}


