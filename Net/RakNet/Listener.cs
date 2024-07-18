using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using Simple.RakNet.Net.Packets;
using Simple.RakNet.Packets;

namespace Simple.RakNet.Net
{
    public class Listener
    {
        private UdpClientCreater Sock;
        private long Server_Guid;
        public ConcurrentDictionary<IPEndPoint, Session> Client = new ConcurrentDictionary<IPEndPoint, Session>();
        public Listener(IPEndPoint i_BindPoint)
        {
            Packets.Manager.InitPacketsList();
            Sock = new UdpClientCreater(i_BindPoint)
            {
             Recv_Action = new Action<EndPoint, byte[]>(OnReceivePacket)
            };
            byte[] buffer = new byte[8];
            new Random().NextBytes(buffer);
            Server_Guid = BitConverter.ToInt64(buffer, 0);
             
        }
        private void OnReceivePacket(EndPoint i_EndPoint,byte[] packets)
        {
            switch ((RakNetProtocol)packets[0])
            {
                case RakNetProtocol.UnconnectPing:
                    break;
                case RakNetProtocol.DetectLostConnections:
                    break;
                case RakNetProtocol.OpenConnectionRequest1:
                    HandleOpenConnectionRequest1(packets,i_EndPoint);
                    break;
                case RakNetProtocol.OpenConnectionRequest2:
                    break;
                default:
                     
                    break;
            }
        }
        private void HandleOpenConnectionRequest1(byte[] buffer,EndPoint i_endpoint)
        {
            OpenConnectionRequest1 pk = new OpenConnectionRequest1();
            pk.Decode(buffer);
            OpenConnectionReply1 reply = new OpenConnectionReply1();
            reply.Magic = Packet.MAGIC;
            reply.mtuSize = (short)UdpClientCreater.RAKNET_CLIENT_MTU;
            reply.serverGuid = Server_Guid;
            reply.serverHasSecurity = 0x00;
            SendPacket(i_endpoint,reply.Encode());
        }
        public void HandleOPenConnectRequest2(byte[] buffer,EndPoint i_endpoint)
        {
            OpenConnectionRequest2 pk = new OpenConnectionRequest2();
            pk.Decode(buffer);

        }
        public void SendPacket(EndPoint iPEnd, byte[] buffer)
        {
            Sock.Send(iPEnd,buffer);
        }
    }
}
