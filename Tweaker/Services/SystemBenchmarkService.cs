using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Tweaker.Services
{
    public class BenchmarkResult
    {
        public int CpuScore { get; set; }
        public int RamScore { get; set; }
        public int DiskScore { get; set; }
        public int TotalScore => CpuScore + RamScore + DiskScore;
    }

    public class SystemBenchmarkService
    {
        public async Task<BenchmarkResult> RunBenchmarkAsync()
        {
            var result = new BenchmarkResult();

            result.CpuScore = await RunCpuBenchmarkAsync();
            result.RamScore = await RunRamBenchmarkAsync();
            result.DiskScore = await RunDiskBenchmarkAsync();

            return result;
        }

        private async Task<int> RunCpuBenchmarkAsync()
        {
            return await Task.Run(() =>
            {
                var stopwatch = Stopwatch.StartNew();
                double result = 0;
                
                // Simple math operations to stress CPU
                for (int i = 0; i < 50000000; i++)
                {
                    result += Math.Sqrt(i) * Math.Sin(i);
                }

                stopwatch.Stop();
                // Higher score for faster completion
                // 10000 ms -> 0 score, 100 ms -> 9900 score
                int score = (int)(10000 - stopwatch.ElapsedMilliseconds);
                return Math.Max(0, score);
            });
        }

        private async Task<int> RunRamBenchmarkAsync()
        {
            return await Task.Run(() =>
            {
                var stopwatch = Stopwatch.StartNew();
                int size = 1024 * 1024 * 50; // 50 MB array
                int[] array = new int[size];

                // Write to RAM
                for (int i = 0; i < size; i++)
                {
                    array[i] = i;
                }

                // Read from RAM
                long sum = 0;
                for (int i = 0; i < size; i++)
                {
                    sum += array[i];
                }

                stopwatch.Stop();
                // 5000 ms -> 0 score, 10 ms -> 4990 score
                int score = (int)(5000 - stopwatch.ElapsedMilliseconds);
                return Math.Max(0, score);
            });
        }

        private async Task<int> RunDiskBenchmarkAsync()
        {
            return await Task.Run(() =>
            {
                var stopwatch = Stopwatch.StartNew();
                string tempFile = Path.Combine(Path.GetTempPath(), "tweaker_benchmark.tmp");
                byte[] data = new byte[1024 * 1024 * 10]; // 10 MB

                try
                {
                    // Write
                    File.WriteAllBytes(tempFile, data);
                    // Read
                    byte[] readData = File.ReadAllBytes(tempFile);
                }
                finally
                {
                    if (File.Exists(tempFile))
                        File.Delete(tempFile);
                }

                stopwatch.Stop();
                // 5000 ms -> 0 score, 10 ms -> 4990 score
                int score = (int)(5000 - stopwatch.ElapsedMilliseconds);
                return Math.Max(0, score);
            });
        }
    }
}
