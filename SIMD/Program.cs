using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SIMD
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var rnd = new Random();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            // 1GB array contains random number less then 0x40000000
            var testAry = new int[268435456];
            for (int i = 0; i < testAry.Length; i++)
            {
                testAry[i] = rnd.Next(0x40000000);
            }

            sw.Stop();
            Console.WriteLine($"Test data generated in {sw.Elapsed}");

            // each element add 1 to 256, total 255 times
            var addAry = new int[255];
            for (int i = 0; i < addAry.Length; i++)
            {
                addAry[i] = i + 1;
            }

            Console.WriteLine("Single thread basic add");
            sw.Restart();
            for (int i = 0; i < testAry.Length; i++)
            {
                for (int j = 0; j < addAry.Length; j++)
                {
                    testAry[i] += addAry[j];
                }
            }

            sw.Stop();
            Console.WriteLine($"Single thread basic add in {sw.Elapsed}");

            Console.WriteLine($"Threads count: {Environment.ProcessorCount}");

            Console.WriteLine("Multi threads basic add");
            sw.Restart();
            Parallel.For(0, testAry.Length, i =>
            {
                for (int j = 0; j < addAry.Length; j++)
                {
                    testAry[i] += addAry[j];
                }
            });
            sw.Stop();
            Console.WriteLine($"Multi threads basic add in {sw.Elapsed}");

            Console.WriteLine($"Current runtime: {(Environment.Is64BitProcess ? "x64" : "x86")}");
            Console.WriteLine($"CPU SIMD HardwareAccelerated: {Vector.IsHardwareAccelerated}");

            if (!Environment.Is64BitProcess || !Vector.IsHardwareAccelerated)
            {
                Console.WriteLine("Please run at x64 release build to enable CPU SIMD HardwareAccelerated");
                goto GPU;
            }

            var SIMDIntBatchCount = Vector<int>.Count;

            Console.WriteLine($"SIMD int batch size: {SIMDIntBatchCount}");

            Console.WriteLine("Single thread SIMD add");
            sw.Restart();
            for (int i = 0; i < testAry.Length; i += SIMDIntBatchCount)
            {
                var vector = new Vector<int>(testAry, i);
                for (int j = 0; j < addAry.Length; j++)
                {
                    var increment = new Vector<int>(addAry[j]);
                    vector += increment;
                }

                vector.CopyTo(testAry, i);
            }

            sw.Stop();
            Console.WriteLine($"Single thread SIMD add in {sw.Elapsed}");

            Console.WriteLine("Multi threads SIMD add");
            sw.Restart();
            Parallel.For(0, testAry.Length / SIMDIntBatchCount, i =>
            {
                i *= SIMDIntBatchCount;
                var vector = new Vector<int>(testAry, i);
                for (int j = 0; j < addAry.Length; j++)
                {
                    var increment = new Vector<int>(addAry[j]);
                    vector += increment;
                }

                vector.CopyTo(testAry, i);
            });
            sw.Stop();
            Console.WriteLine($"Multi threads SIMD add in {sw.Elapsed}");

        GPU:
            Console.WriteLine("GPU SIMD add");
            sw.Restart();
            GPUAdd(testAry, testAry.Length, addAry, addAry.Length);

            sw.Stop();
            Console.WriteLine($"GPU SIMD add in {sw.Elapsed}");
        }

        [DllImport("CppAMP")]
        private static extern void GPUAdd(int[] data, int dataSize, int[] additions, int additionsCount);
    }
}