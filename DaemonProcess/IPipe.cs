using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonProcess
{
    interface IPipe
    {
        void WriteToPipe(string _message);
        string ReadFromPipe();

    }
}
