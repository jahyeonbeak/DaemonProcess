using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonProcess
{
    public abstract class Pipe : IPipe
    {
        internal string pipeName;
        internal Encoding PipeEncoding { set; get; }
        internal int pipeInBufferSize = 4096;
        internal int pipeOutBufferSize = 65535;
        internal PipeStream pipe;

        public abstract void Connect();
        public abstract void Disconnect();
        public virtual void Close()
        {
            pipe.Close();
        }
        public virtual void WriteToPipe(string _message)
        {

            if (pipe.IsConnected)
            {
                try
                {
                    byte[] writeBytes = StringToBytes(_message);
                    pipe.Write(writeBytes, 0, writeBytes.Length);
                    pipe.Flush();
                    pipe.WaitForPipeDrain();
                }
                catch { }
            }

        }
        public virtual string ReadFromPipe()
        {
            var data = new byte[pipeInBufferSize];
            var count = pipe.Read(data, 0, pipeInBufferSize);
            if (count > 0)
            {
                string message = BytesToString(data, 0, count);
                return message;
            }
            return null;
        }
        public virtual byte[] StringToBytes(string _instring)
        {
            byte[] data = PipeEncoding.GetBytes(_instring);
            return data;
        }
        public virtual string BytesToString(byte[] _inbytes, int _index, int _count)
        {
            string str = PipeEncoding.GetString(_inbytes, 0, _count);
            return str;
        }
    }
}
