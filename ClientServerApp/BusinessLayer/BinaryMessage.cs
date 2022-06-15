using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class BinaryMessage
    {
        public byte[] Data { get; set; }
        public BinaryMessage()
        {
            this.Data = new byte[8192];
        }

        public BinaryMessage(int capacity)
        {
            this.Data = new byte[capacity];
        }
    }
}
