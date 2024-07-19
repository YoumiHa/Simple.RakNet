using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Simple.RakNet.Net
{
    public class Session
    {

        public IPEndPoint? Self_IPEndPoint { get; set; }
        
        public Session(IPEndPoint endPoint)
        {
            Self_IPEndPoint = endPoint;
        }

    }
}
