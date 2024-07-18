using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Simple.RakNet.Net
{
    public class UdpClientCreater
    {
        public static readonly byte RAKNET_PROTOCOL_VERSION = 0xA;
        public static readonly ushort RAKNET_CLIENT_MTU = 1400;
        public static readonly int RECEIVE_TIMEOUT = 60000;
        public IPEndPoint m_IPEndPoint { get; private set; }
        private UdpClient m_Listener;
        public bool m_Connected { get; private set; } = false;
        public required Action<EndPoint,ReadOnlyMemory<byte>> Recv_Action { get; set; }
        public UdpClientCreater(IPEndPoint i_IPEndPoint)
        {
            this.m_IPEndPoint = i_IPEndPoint;
            m_Listener = Init_(i_IPEndPoint);
        }
        private UdpClient Init_(IPEndPoint i_EndPoint)
        {
            try
            {
                UdpClient listener = new UdpClient();

                if (Environment.OSVersion.Platform != PlatformID.MacOSX)
                {
                    listener.Client.ReceiveBufferSize = int.MaxValue;
                    listener.Client.SendBufferSize = int.MaxValue;
                }

                listener.DontFragment = false;
                listener.EnableBroadcast = true;

                if (Environment.OSVersion.Platform != PlatformID.Unix && Environment.OSVersion.Platform != PlatformID.MacOSX)
                {
                    uint IOC_IN = 0x80000000;
                    uint IOC_VENDOR = 0x18000000;
                    uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
                    listener.Client.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
                }

                listener.Client.Bind(i_EndPoint);
                return listener;
            }
            catch (Exception ex)
            {
                throw new Exception("new UdpClient Error by" + ex.Message);
            }
        }
        public void Start()
        {
            m_Connected = true;
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 0);
            Thread thread = new Thread(() =>
            {
                while (true)
                {
                    if (!m_Connected)
                        return;
                    byte[] recv_bytes = m_Listener.Receive(ref iPEndPoint);
                    Recv_Action(iPEndPoint,recv_bytes);
                }
            });
            thread.Start();
        }
        public void Stop() 
        {
            m_Connected = false;
            m_Listener.Dispose();
        }
        public void Send(IPEndPoint i_ClientEndPoint, byte[] packet_data)
        {
            m_Listener.Client.SendToAsync(packet_data,i_ClientEndPoint);
        }
    }
}
