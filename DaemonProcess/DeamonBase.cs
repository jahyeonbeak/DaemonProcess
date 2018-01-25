using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DaemonProcess
{
    public class DeamonBase
    {
        private static DeamonBase m_instance = null; //动态接口

        public NamedPipeClientStream PipeStream { set; get; } // 命名通信管道

        public Thread PipeOpenCheckerThread { set; get; }
        private bool isPipeOpenCheckerRun = false;

        public Thread PipeCheckerThread { set; get; }




        private DeamonBase()
        {
            PipeOpenCheckerThread = new Thread(new ThreadStart(CheckPipeOpen));
            PipeOpenCheckerThread.Priority = ThreadPriority.Lowest;
            PipeCheckerThread = new Thread(new ThreadStart(CheckPipe));
            PipeCheckerThread.Priority = ThreadPriority.Normal;
            isPipeOpenCheckerRun = false;

        }

        public static DeamonBase Instance()
        {
            if (m_instance == null)
            {
                m_instance = new DeamonBase();
            }
            return m_instance;
        }

        public void CreatePipe(string _pipeName)
        {
            PipeStream = new NamedPipeClientStream(_pipeName);
        }

        public void StartMonitor()
        {
            isPipeOpenCheckerRun = true;
            PipeOpenCheckerThread.Start();
        }

        private void CheckPipeOpen()
        {
            while (isPipeOpenCheckerRun)
            {
                if (!PipeStream.IsConnected)
                {
                    PipeStream.Connect();
                }
                else
                {
                    PipeCheckerThread.Start();
                    isPipeOpenCheckerRun = false;
                    PipeOpenCheckerThread.Abort();
                }
                Thread.Sleep(2000);
            }
        }

        public void CheckPipe()
        {
            Decoder decoder = Encoding.UTF8.GetDecoder();
            Byte[] bytes = new Byte[10];
            Char[] chars = new Char[10];

            PipeStream.ReadMode = PipeTransmissionMode.Byte;
            int numBytes;
            do
            {
                string message = "";

                do
                {
                    numBytes = PipeStream.Read(bytes, 0, bytes.Length);
                    int numChars = decoder.GetChars(bytes, 0, numBytes, chars, 0);
                    message += new String(chars, 0, numChars);
                } while (!PipeStream.IsMessageComplete);

                decoder.Reset();
                Console.WriteLine(message);
            } while (numBytes != 0);

        }
        
    }
}
