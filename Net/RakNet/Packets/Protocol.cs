#pragma warning disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Simple.RakNet.Packets
{
    public enum RakNetProtocol 
    {
       UnconnectPing = 0x01,
       UnconnectPong = 0x1c,
       ConnectedPing = 0x00,
       ConnectedPong = 0x03,
       DetectLostConnections = 0x04,
       OpenConnectionRequest1 = 0x05,
       OpenConnectionReply1 = 0x06,
       OpenConnectionRequest2 = 0x07,
       OpenConnectionReply2 = 0x08,
        ConnectionRequest = 0x09,
        ConnectionRequestAccepted = 0x10,
        NewIncomingConnection = 0x13,
        NoFreeIncomingConnections = 0x14,
        DisconnectionNotification = 0x15,
        ConnectionBanned = 0x17,
        IpRecentlyConnected = 0x1a,
    }

    public partial class UnconnectPing : Packet
    {
        public long Time { get; set; }
        public byte[] Magic { get; set; }
        public long Client_id { get; set; }
        public UnconnectPing()
        {
            IsMcpe = false;
            Id = 0x01;
        }
        protected override void EncodePacket()
        {
            base.EncodePacket();
            BeforeEncode();
            Write(Time);
            Write(Magic);
            Write(Client_id);
            AfterEncode();
        }
        partial void BeforeEncode();
        partial void AfterEncode();
        protected override void DecodePacket()
        {
            base.DecodePacket();
            BeforeDecode();
            Time = ReadLong();
            ReadBytes(Magic.Length);
            Client_id = ReadLong();
            AfterDecode();
        }
        partial void BeforeDecode();
        partial void AfterDecode();
        protected override void ResetPacket()
        {
            base.ResetPacket();
            Time = default(long);
            Magic = default(byte[]);
            Client_id = default(long);
        }

    }
    public partial class UnconnectPong : Packet
    {
        public long Time { get; set; }
        public long Server_guid { get; set; }
        public byte[] Magic { get; set; }
        public string Server_id_String { get; set; }
        public UnconnectPong()
        {
            IsMcpe = false;
            Id = 0x1c;
        }
        partial void BeforeEncode();
        partial void AfterEncode();
        partial void BeforeDecode();
        partial void AfterDecode();
        protected override void EncodePacket()
        {
            base.EncodePacket();
            BeforeEncode();
            Write(Time);
            Write(Server_guid);
            Write(Magic);
            Write(Server_id_String);
            AfterEncode();
        }
        protected override void DecodePacket()
        {
            base.DecodePacket();
            BeforeDecode();
            Time = ReadLong();
            Server_guid = ReadLong();
            ReadBytes(Magic.Length);
            Server_id_String = ReadFixedString();
            AfterDecode();
        }
        protected override void ResetPacket()
        {
            base.ResetPacket();
            Time = default(long);
            Server_guid = default(long);
            Magic = default(byte[]);
            Server_id_String = default(string);
        }
    }
    public partial class ConnectedPing : Packet
    {

        public long sendpingtime; // = null;

        public ConnectedPing()
        {
            Id = 0x00;
            IsMcpe = false;
        }

        protected override void EncodePacket()
        {
            base.EncodePacket();

            BeforeEncode();

            Write(sendpingtime);

            AfterEncode();
        }

        partial void BeforeEncode();
        partial void AfterEncode();

        protected override void DecodePacket()
        {
            base.DecodePacket();

            BeforeDecode();

            sendpingtime = ReadLong();

            AfterDecode();
        }

        partial void BeforeDecode();
        partial void AfterDecode();

        protected override void ResetPacket()
        {
            base.ResetPacket();

            sendpingtime = default(long);
        }

    }
    public partial class ConnectedPong : Packet
    {

        public long sendpingtime; // = null;
        public long sendpongtime; // = null;

        public ConnectedPong()
        {
            Id = 0x03;
            IsMcpe = false;
        }

        protected override void EncodePacket()
        {
            base.EncodePacket();

            BeforeEncode();

            Write(sendpingtime);
            Write(sendpongtime);

            AfterEncode();
        }

        partial void BeforeEncode();
        partial void AfterEncode();

        protected override void DecodePacket()
        {
            base.DecodePacket();

            BeforeDecode();

            sendpingtime = ReadLong();
            sendpongtime = ReadLong();

            AfterDecode();
        }

        partial void BeforeDecode();
        partial void AfterDecode();

        protected override void ResetPacket()
        {
            base.ResetPacket();

            sendpingtime = default(long);
            sendpongtime = default(long);
        }

    }
    public partial class DetectLostConnections : Packet
    {


        public DetectLostConnections()
        {
            Id = 0x04;
            IsMcpe = false;
        }

        protected override void EncodePacket()
        {
            base.EncodePacket();

            BeforeEncode();


            AfterEncode();
        }

        partial void BeforeEncode();
        partial void AfterEncode();

        protected override void DecodePacket()
        {
            base.DecodePacket();

            BeforeDecode();


            AfterDecode();
        }

        partial void BeforeDecode();
        partial void AfterDecode();

        protected override void ResetPacket()
        {
            base.ResetPacket();

        }

    }
    public partial class OpenConnectionRequest1 : Packet
    {

        public byte[] Magic;
        public byte raknetProtocolVersion; // = null;

        public OpenConnectionRequest1()
        {
            Id = 0x05;
            IsMcpe = false;
        }

        protected override void EncodePacket()
        {
            base.EncodePacket();

            BeforeEncode();

            Write(Magic);
            Write(raknetProtocolVersion);

            AfterEncode();
        }

        partial void BeforeEncode();
        partial void AfterEncode();

        protected override void DecodePacket()
        {
            base.DecodePacket();

            BeforeDecode();

            ReadBytes(Magic.Length);
            raknetProtocolVersion = ReadByte();

            AfterDecode();
        }

        partial void BeforeDecode();
        partial void AfterDecode();

        protected override void ResetPacket()
        {
            base.ResetPacket();

            raknetProtocolVersion = default(byte);
        }

    }
    public partial class OpenConnectionReply1 : Packet
    {

        public byte[] Magic;

        public long serverGuid; // = null;
        public byte serverHasSecurity; // = null;
        public short mtuSize; // = null;

        public OpenConnectionReply1()
        {
            Id = 0x06;
            IsMcpe = false;
        }

        protected override void EncodePacket()
        {
            base.EncodePacket();

            BeforeEncode();

            Write(Magic);
            Write(serverGuid);
            Write(serverHasSecurity);
            WriteBe(mtuSize);

            AfterEncode();
        }

        partial void BeforeEncode();
        partial void AfterEncode();

        protected override void DecodePacket()
        {
            base.DecodePacket();

            BeforeDecode();

            ReadBytes(Magic.Length);
            serverGuid = ReadLong();
            serverHasSecurity = ReadByte();
            mtuSize = ReadShortBe();

            AfterDecode();
        }

        partial void BeforeDecode();
        partial void AfterDecode();

        protected override void ResetPacket()
        {
            base.ResetPacket();

            serverGuid = default(long);
            serverHasSecurity = default(byte);
            mtuSize = default(short);
        }

    }
    public partial class OpenConnectionRequest2 : Packet
    {

        public byte[] Magic;

        public IPEndPoint remoteBindingAddress; // = null;
        public short mtuSize; // = null;
        public long clientGuid; // = null;

        public OpenConnectionRequest2()
        {
            Id = 0x07;
            IsMcpe = false;
        }

        protected override void EncodePacket()
        {
            base.EncodePacket();

            BeforeEncode();

            Write(Magic);
            Write(remoteBindingAddress);
            WriteBe(mtuSize);
            Write(clientGuid);

            AfterEncode();
        }

        partial void BeforeEncode();
        partial void AfterEncode();

        protected override void DecodePacket()
        {
            base.DecodePacket();

            BeforeDecode();

            ReadBytes(Magic.Length);
            remoteBindingAddress = ReadIPEndPoint();
            mtuSize = ReadShortBe();
            clientGuid = ReadLong();

            AfterDecode();
        }

        partial void BeforeDecode();
        partial void AfterDecode();

        protected override void ResetPacket()
        {
            base.ResetPacket();

            remoteBindingAddress = default(IPEndPoint);
            mtuSize = default(short);
            clientGuid = default(long);
        }

    }
    public partial class OpenConnectionReply2 : Packet
    {

        public byte[] Magic;
        public long serverGuid; // = null;
        public IPEndPoint clientEndpoint; // = null;
        public short mtuSize; // = null;
        public byte[] doSecurityAndHandshake; // = null;

        public OpenConnectionReply2()
        {
            Id = 0x08;
            IsMcpe = false;
        }

        protected override void EncodePacket()
        {
            base.EncodePacket();

            BeforeEncode();

            Write(Magic);
            Write(serverGuid);
            Write(clientEndpoint);
            WriteBe(mtuSize);
            Write(doSecurityAndHandshake);

            AfterEncode();
        }

        partial void BeforeEncode();
        partial void AfterEncode();

        protected override void DecodePacket()
        {
            base.DecodePacket();

            BeforeDecode();

            ReadBytes(Magic.Length);
            serverGuid = ReadLong();
            clientEndpoint = ReadIPEndPoint();
            mtuSize = ReadShortBe();
            doSecurityAndHandshake = ReadBytes(0, true);

            AfterDecode();
        }

        partial void BeforeDecode();
        partial void AfterDecode();

        protected override void ResetPacket()
        {
            base.ResetPacket();

            serverGuid = default(long);
            clientEndpoint = default(IPEndPoint);
            mtuSize = default(short);
            doSecurityAndHandshake = default(byte[]);
        }

    }
    public partial class ConnectionRequest : Packet
    {

        public long clientGuid; // = null;
        public long timestamp; // = null;
        public byte doSecurity; // = null;

        public ConnectionRequest()
        {
            Id = 0x09;
            IsMcpe = false;
        }

        protected override void EncodePacket()
        {
            base.EncodePacket();

            BeforeEncode();

            Write(clientGuid);
            Write(timestamp);
            Write(doSecurity);

            AfterEncode();
        }

        partial void BeforeEncode();
        partial void AfterEncode();

        protected override void DecodePacket()
        {
            base.DecodePacket();

            BeforeDecode();

            clientGuid = ReadLong();
            timestamp = ReadLong();
            doSecurity = ReadByte();

            AfterDecode();
        }

        partial void BeforeDecode();
        partial void AfterDecode();

        protected override void ResetPacket()
        {
            base.ResetPacket();

            clientGuid = default(long);
            timestamp = default(long);
            doSecurity = default(byte);
        }

    }
    public partial class ConnectionRequestAccepted : Packet
    {

        public IPEndPoint systemAddress; // = null;
        public short systemIndex; // = null;
        public IPEndPoint[] systemAddresses; // = null;
        public long incomingTimestamp; // = null;
        public long serverTimestamp; // = null;

        public ConnectionRequestAccepted()
        {
            Id = 0x10;
            IsMcpe = false;
        }

        protected override void EncodePacket()
        {
            base.EncodePacket();

            BeforeEncode();

            Write(systemAddress);
            WriteBe(systemIndex);
            Write(systemAddresses);
            Write(incomingTimestamp);
            Write(serverTimestamp);

            AfterEncode();
        }

        partial void BeforeEncode();
        partial void AfterEncode();

        protected override void DecodePacket()
        {
            base.DecodePacket();

            BeforeDecode();

            systemAddress = ReadIPEndPoint();
            systemIndex = ReadShortBe();
            systemAddresses = ReadIPEndPoints(20);
            incomingTimestamp = ReadLong();
            serverTimestamp = ReadLong();

            AfterDecode();
        }

        partial void BeforeDecode();
        partial void AfterDecode();

        protected override void ResetPacket()
        {
            base.ResetPacket();

            systemAddress = default(IPEndPoint);
            systemIndex = default(short);
            systemAddresses = default(IPEndPoint[]);
            incomingTimestamp = default(long);
            serverTimestamp = default(long);
        }

    }
    public partial class NewIncomingConnection : Packet
    {

        public IPEndPoint clientendpoint; // = null;
        public IPEndPoint[] systemAddresses; // = null;
        public long incomingTimestamp; // = null;
        public long serverTimestamp; // = null;

        public NewIncomingConnection()
        {
            Id = 0x13;
            IsMcpe = false;
        }

        protected override void EncodePacket()
        {
            base.EncodePacket();

            BeforeEncode();

            Write(clientendpoint);
            Write(systemAddresses);
            Write(incomingTimestamp);
            Write(serverTimestamp);

            AfterEncode();
        }

        partial void BeforeEncode();
        partial void AfterEncode();

        protected override void DecodePacket()
        {
            base.DecodePacket();

            BeforeDecode();

            clientendpoint = ReadIPEndPoint();
            systemAddresses = ReadIPEndPoints(20);
            incomingTimestamp = ReadLong();
            serverTimestamp = ReadLong();

            AfterDecode();
        }

        partial void BeforeDecode();
        partial void AfterDecode();

        protected override void ResetPacket()
        {
            base.ResetPacket();

            clientendpoint = default(IPEndPoint);
            systemAddresses = default(IPEndPoint[]);
            incomingTimestamp = default(long);
            serverTimestamp = default(long);
        }

    }
    public partial class NoFreeIncomingConnections : Packet
    {

        public byte[] Magic;
        public long serverGuid; // = null;

        public NoFreeIncomingConnections()
        {
            Id = 0x14;
            IsMcpe = false;
        }

        protected override void EncodePacket()
        {
            base.EncodePacket();

            BeforeEncode();

            Write(Magic);
            Write(serverGuid);

            AfterEncode();
        }

        partial void BeforeEncode();
        partial void AfterEncode();

        protected override void DecodePacket()
        {
            base.DecodePacket();

            BeforeDecode();

            ReadBytes(Magic.Length);
            serverGuid = ReadLong();

            AfterDecode();
        }

        partial void BeforeDecode();
        partial void AfterDecode();

        protected override void ResetPacket()
        {
            base.ResetPacket();

            serverGuid = default(long);
        }

    }
    public partial class DisconnectionNotification : Packet
    {


        public DisconnectionNotification()
        {
            Id = 0x15;
            IsMcpe = false;
        }

        protected override void EncodePacket()
        {
            base.EncodePacket();

            BeforeEncode();


            AfterEncode();
        }

        partial void BeforeEncode();
        partial void AfterEncode();

        protected override void DecodePacket()
        {
            base.DecodePacket();

            BeforeDecode();


            AfterDecode();
        }

        partial void BeforeDecode();
        partial void AfterDecode();

        protected override void ResetPacket()
        {
            base.ResetPacket();

        }

    }
    public partial class ConnectionBanned : Packet
    {

        public byte[] Magic;
        public long serverGuid; // = null;

        public ConnectionBanned()
        {
            Id = 0x17;
            IsMcpe = false;
        }

        protected override void EncodePacket()
        {
            base.EncodePacket();

            BeforeEncode();

            Write(Magic);
            Write(serverGuid);

            AfterEncode();
        }

        partial void BeforeEncode();
        partial void AfterEncode();

        protected override void DecodePacket()
        {
            base.DecodePacket();

            BeforeDecode();

            ReadBytes(Magic.Length);
            serverGuid = ReadLong();

            AfterDecode();
        }

        partial void BeforeDecode();
        partial void AfterDecode();

        protected override void ResetPacket()
        {
            base.ResetPacket();

            serverGuid = default(long);
        }

    }
    public partial class IpRecentlyConnected : Packet
    {

        public byte[] Magic;

        public IpRecentlyConnected()
        {
            Id = 0x1a;
            IsMcpe = false;
        }

        protected override void EncodePacket()
        {
            base.EncodePacket();

            BeforeEncode();

            Write(Magic);

            AfterEncode();
        }

        partial void BeforeEncode();
        partial void AfterEncode();

        protected override void DecodePacket()
        {
            base.DecodePacket();

            BeforeDecode();

            ReadBytes(Magic.Length);

            AfterDecode();
        }

        partial void BeforeDecode();
        partial void AfterDecode();

        protected override void ResetPacket()
        {
            base.ResetPacket();

        }

    }
    public partial class GamePacket : Packet
    {

        public GamePacket()
        {
            Id = 0xfe;
            IsMcpe = false;
        }

        protected override void EncodePacket()
        {
            base.EncodePacket();

            BeforeEncode();


            AfterEncode();
        }

        partial void BeforeEncode();
        partial void AfterEncode();

        protected override void DecodePacket()
        {
            base.DecodePacket();

            BeforeDecode();


            AfterDecode();
        }

        partial void BeforeDecode();
        partial void AfterDecode();

        protected override void ResetPacket()
        {
            base.ResetPacket();

        }

    }
}
