using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Simple.RakNet.Net
{
    public class Listener
    {
        private UdpClientCreater Sock;
        public ConcurrentDictionary<IPEndPoint, Session> Client = new ConcurrentDictionary<IPEndPoint, Session>();
        public Listener(IPEndPoint i_BindPoint)
        {
          
            Sock = new UdpClientCreater(i_BindPoint)
            {
             Recv_Action = new Action<EndPoint, ReadOnlyMemory<byte>>(OnReceivePacket)
            };
        }
        private void OnReceivePacket(EndPoint i_EndPoint,ReadOnlyMemory<byte> packets)
        {

        }

    }
}
