using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Model
{
    internal class Message
    {
        public string nickname;

        public DateTime date = DateTime.Now;

        public string msgText;
    }
}
