using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test_Program {
    class Program {
        static void Main(string[] args) {
            Program p = new Program();

            p.DoWork();

            for (int i = 0; i < 1000; i++) {
                Console.Write(".");
                Thread.Sleep(100);
            }


            Console.ReadLine();
        }


        async void DoWork() {
            await Task.Factory.StartNew(() => {
                for (int i = 0; i < 4; i++) {
                    Console.WriteLine("Working: " + i);
                    Thread.Sleep(1000);
                }
                Console.WriteLine("Done");
            });

        }

    }
}
