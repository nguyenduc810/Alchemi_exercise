
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Threading;
using Alchemi.Core;
using Alchemi.Core.Owner;
using static System.Net.Mime.MediaTypeNames;
using System.Data.Common;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;

namespace SquareRoot
{
    class SquareRoot : GApplication
    {
        public static GApplication App = new GApplication();
        private static DateTime start;
        private static long value;
        private static long nDigits;
        private static long NumberOfThread;
        private static long NumberOfElementThreads;
        private static long NumberOfLoop;
        private static long count = 0;
        public static double first = 0;
        public static double last = 0;
        public static int round = 0;
        public static bool wait = false;

        public static bool ExitCondition { get; set; }

        [STAThread]
        static void Main(string[] args)
        {
            List<List<int>> matrix;
            string host;
            Console.Write("Host[localhost]: ");
            host = Console.ReadLine();
            if (host.Length < 1)
                host = "localhost";
            Console.Write("\nEnter the number to calculate the cubic root: ");
            value = Convert.ToInt64(Console.ReadLine());
            last = value;
            Console.Write("\nEnter the number of digits to be calculated exactly: ");
            nDigits = Convert.ToInt64(Console.ReadLine());
            Console.Write("\nEnter the number of threads: ");
            NumberOfThread = Convert.ToInt64(Console.ReadLine());

            NumberOfElementThreads = NumberOfLoop / NumberOfThread;

           

            // Lấy giá trị của exitCondition
            

            while (!ExitCondition)
            {
                round++;
                //Split range in to multiple parts
                double step = (last - first) / NumberOfThread;

                //Basic init
                //TODO: Reuse 
                App = new GApplication();
                App.Connection = new GConnection(host, 9000, "user", "user");
                App.Manifest.Add(new ModuleDependency(typeof(CubicSearch).Module));
                App.ThreadFinish += new GThreadFinish(App_ThreadFinish);
                App.ApplicationFinish += new GApplicationFinish(App_ApplicationFinish);


                //each thread take care of 1 part
                for (int i = 0; i < NumberOfThread; i++)
                {
                    App.Threads.Add(new CubicSearch(first + i * step, first + (i + 1) * step, value));

                }

                Console.WriteLine(round);
                
                App.Start();
                wait = true;
                while (wait)
                {
                    Thread.Sleep(16);
                }
                if (round > 50  )
                {
                    //TODO: Edit exit condition to satisfy accuracy
                    ExitCondition = true;
                }
            }


            start = DateTime.Now;
            Console.WriteLine("Thread started!");
            Console.WriteLine("\n----------------------------------------------------------\n");
            Console.ReadLine();
        }

      

        private static void App_ThreadFinish(GThread thread)
        {
            Console.WriteLine("{0}th thread completed. Round {1}", count, round);
            Console.WriteLine();
        }
        private static void App_ApplicationFinish()
        {
            Console.WriteLine("----------------------------------------------------------");
          
           

            

            foreach (GThread i in App.Threads)
            {
                CubicSearch threadObj = (CubicSearch)i;
                if (threadObj.result)
                {
                    first = threadObj.first;
                    last = threadObj.last;
                }
            }

            if (last - first > Math.Pow(10, -nDigits))
            {
                Console.WriteLine("Round {0}, first: {1}, last: {2}, diff[last - first] = {3}, saiso = {4}", round, first, last, last - first, Math.Pow(10, -nDigits));

            }
            else
            {
                

                // Thay đổi giá trị của exitCondition
                ExitCondition = true;
                Console.WriteLine("Cube root of {0} ========== {1} || Time executor in  = {2}", value, first, DateTime.Now- start);

            }

            wait = false;
        }
    }


    [Serializable]
    class CubicSearch : GThread
    {
        public double target;
        public double first;
        public double last;
        public bool result = false;

        /// <summary>
        /// Use this constructor to pass data to GThread<br/>
        /// Find if cubic root of our target exist in range [first, last)
        /// </summary>
        /// <param name="first">Start number of the range we search, number included</param>
        /// <param name="last">Stop number of the range we search, number excluded</param>
        /// <param name="target">The number that we want to find cubic root</param>
        public CubicSearch(double first, double last, long target)
        {
            this.target = target;
            this.first = first;
            this.last = last;
        }

        public override void Start()
        {
            if (this.first * this.first * this.first <= this.target && this.target < this.last * this.last * this.last)
            {
                this.result = true;
            }
        }
    }
}