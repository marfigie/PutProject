using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PUT_Project.Logic
{
    class MessageArrivedEventArgs : EventArgs
    {

        public MessageArrivedEventArgs(string msg)
        {
            this.msg = msg;
        }

        public string msg;
    }
}
