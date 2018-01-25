using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DaemonProcess
{
    public class PipeServer : Pipe, IPipe
    {
        public int ThreadDelay { set; get; }

        private NamedPipeServerStream _pipeServer;

        public PipeServer(string _pipeName, Encoding _encoding, int _pipeInBufferSize = 4096, int _pipeOutBufferSize = 65535)
        {
            pipeName = _pipeName;
            PipeEncoding = _encoding;
            pipeInBufferSize = _pipeInBufferSize;
            pipeOutBufferSize = _pipeOutBufferSize;

            pipe = new NamedPipeServerStream(
                pipeName,
                PipeDirection.InOut,
                1,
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous | PipeOptions.WriteThrough,
                pipeInBufferSize,
                pipeOutBufferSize
                );

            ((NamedPipeServerStream)pipe).BeginWaitForConnection(WaitForConnectionCallback, (NamedPipeServerStream)pipe);
            _pipeServer = (NamedPipeServerStream)pipe;

        }

        public AsyncCallback WaitForConnectionCallback1;

        public override void Connect()
        {
            if (!pipe.IsConnected || pipe == null)
            {
                pipe = new NamedPipeServerStream(
                pipeName,
                PipeDirection.InOut,
                1,
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous | PipeOptions.WriteThrough,
                pipeInBufferSize,
                pipeOutBufferSize
                );

                if (WaitForConnectionCallback1 != null)
                    ((NamedPipeServerStream)pipe).BeginWaitForConnection(WaitForConnectionCallback1, (NamedPipeServerStream)pipe);
                else
                    ((NamedPipeServerStream)pipe).BeginWaitForConnection(WaitForConnectionCallback, (NamedPipeServerStream)pipe);
            }
            _pipeServer = (NamedPipeServerStream)pipe;
        }

        private void WaitForConnectionCallback(IAsyncResult ar)
        {
            var pipeServer = (NamedPipeServerStream)ar.AsyncState;

            pipeServer.EndWaitForConnection(ar);
            WaitForConnectionCallback1(ar);
            // 开始Thread



            //var data = new byte[pipeInBufferSize];

            //var count = pipeServer.Read(data, 0, pipeInBufferSize);

            //if (count > 0)
            //{
            //    string message = PipeEncoding.GetString(data, 0, count);
            //    if (message.Equals(PipeEnmus.PIPE_CONNECTED))
            //    {
            //        // 连接成功
            //    }

            //    //MethodInfo method = type.GetMethod("WriteString");
            //    //this.BeginInvoke(new Action(() =>
            //    //{
            //    //    tblRecMsg.Text = message;
            //    //}));
            //}
        }

        public override void Disconnect()
        {
            if (_pipeServer.IsConnected)
                _pipeServer.Disconnect();
        }

        public void CheckPipe(object ar)
        {
            var pipe = (NamedPipeServerStream)(ar as IAsyncResult).AsyncState;

            while (true)
            {
                string message = ReadFromPipe();
                Thread.Sleep(ThreadDelay);
            }

        }
    }
}