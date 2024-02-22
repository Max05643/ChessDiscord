using System.Diagnostics;
using System.IO;

namespace StockfishWrapper
{
    /// <summary>
    /// Represents a running instance of stockfish engine
    /// </summary>
    internal class StockfishProcess : IDisposable
    {
        private readonly Process stockfishInstance;


        public StockfishProcess(string pathToExecutable)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = pathToExecutable,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };

            stockfishInstance = new Process() { StartInfo = startInfo};

            stockfishInstance.Start();
        }


        public void WriteLine(string line)
        {
            stockfishInstance.StandardInput.WriteLine(line);
            stockfishInstance.StandardInput.Flush();
        }
        public string ReadLine()
        {
            return stockfishInstance.StandardOutput.ReadLine() ?? throw new InvalidOperationException("Process has already closed stdout");
        }


        void IDisposable.Dispose()
        {
            stockfishInstance.Kill();
            stockfishInstance.Dispose();
        }
    }
}