using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DaemonProcessTest
{
    /// <summary>
    /// ClientWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ClientWindow : Window
    {

        private const string PipeName = "PipeSample";

        private Encoding encoding = Encoding.UTF8;

        private NamedPipeClientStream _pipe;

        private bool _starting = false;
        public ClientWindow()
        {
            InitializeComponent();
            
            

        }

        public Thread PipeCheckerThread { set; get; }

        public void CheckPipe(object ar)
        {
            var test = ar as IAsyncResult;
            var pipe = (NamedPipeClientStream)test.AsyncState;
            var data = new byte[65535];
            Console.WriteLine("要开始Read了 client");
            while (true)
            {
                var count = pipe.Read(data, 0, data.Length);

                if (count > 0)
                {
                    string message = encoding.GetString(data, 0, count);

                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        tblRecMsg.Text = message;
                    }));
                }
                Thread.Sleep(1000);
            }

        }


        private void OnConnect(object sender, RoutedEventArgs e)
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
                    PipeName,
                    PipeDirection.InOut,
                    PipeOptions.Asynchronous | PipeOptions.WriteThrough
                );

                _pipe.Connect();

                _pipe.ReadMode = PipeTransmissionMode.Message;

                string message = "Connected!";

                byte[] data = encoding.GetBytes(message);

                _pipe.BeginWrite(data, 0, data.Length, PipeWriteCallback, _pipe);
                Console.WriteLine("beginwrite 完了 client");
                message = "Again!";

                data = encoding.GetBytes(message);
                //_pipe.BeginWrite(data, 0, data.Length, PipeWriteCallback, _pipe);
                //Console.WriteLine("又beginwrite 完了 client");
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
            Console.WriteLine("要开始Read了 client");
            var count = pipe.Read(data, 0, data.Length);

            if (count > 0)
            {
                string message = encoding.GetString(data, 0, count);

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    tblRecMsg.Text = message;
                }));
            }
            PipeCheckerThread = new Thread(new ParameterizedThreadStart(CheckPipe));
            PipeCheckerThread.Priority = ThreadPriority.Normal;
            PipeCheckerThread.Start(ar);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_pipe.IsConnected)
            {
                try
                {
                    string message = this.testtext.Text;

                    byte[] data = encoding.GetBytes(message);

                    _pipe.Write(data, 0, data.Length);
                    _pipe.Flush();
                    _pipe.WaitForPipeDrain();
                }
                catch { }
            }
        }
    }
}
