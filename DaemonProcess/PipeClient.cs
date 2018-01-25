using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonProcess
{
    public class PipeClient
    {
        private string pipeName = "";

        private Encoding encoding = Encoding.UTF8;

        private NamedPipeClientStream _pipe;

        private bool _starting = false;

        PipeClient()
        {
            
        }

        private void Connect()
        {
            if (_starting)
            {
                return;
            }

            try
            {
                _pipe = new NamedPipeClientStream
                (
                    ".",
                    pipeName,
                    PipeDirection.InOut,
                    PipeOptions.Asynchronous | PipeOptions.WriteThrough
                );

                _pipe.Connect();

                _pipe.ReadMode = PipeTransmissionMode.Message;

                string message = "Connected!";

                byte[] data = encoding.GetBytes(message);

                _pipe.BeginWrite(data, 0, data.Length, PipeWriteCallback, _pipe);

                _starting = true;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.StackTrace);
            }
        }
        private void PipeWriteCallback(IAsyncResult ar)
        {
            var pipe = (NamedPipeClientStream)ar.AsyncState;

            pipe.EndWrite(ar);
            pipe.Flush();
            pipe.WaitForPipeDrain();

            var data = new byte[65535];

            var count = pipe.Read(data, 0, data.Length);

            if (count > 0)
            {
                string message = encoding.GetString(data, 0, count);

                //Dispatcher.BeginInvoke(new Action(() =>
                //{
                //    tblRecMsg.Text = message;
                //}));
            }
        }

    }
}
