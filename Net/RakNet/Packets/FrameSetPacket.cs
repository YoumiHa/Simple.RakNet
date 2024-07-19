using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Simple.RakNet.Utils;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Simple.RakNet.Packets
{
    public enum Reliability
    {
        Undefined = -1,
        Unreliable = 0,
        UnreliableSequenced = 1,
        Reliable = 2,
        ReliableOrdered = 3,
        ReliableSequenced = 4,
        UnreliableWithAckReceipt = 5,
        ReliableWithAckReceipt = 6,
        ReliableOrderedWithAckReceipt = 7
    }
    public partial class FrameSetPacket : Packet
    {
        public Int24 Sequence_number;
        public byte Flags;
        public ushort Length_IN_BITS;
        public Int24 Reliable_frame_index;
        public Int24 Sequenced_frame_index;
        public Int24 Ordered_frame_index;
        public byte Order_channel;
        public int Compound_size;
        public short Compound_ID;
        public int Index;
        public byte[] body;
        static readonly byte NEEDS_B_AND_AS_FLAG = 0x4;
        static readonly byte CONTINUOUS_SEND_FLAG = 0x8;
        public FrameSetPacket() 
        {
            Id = 0x80;
           IsMcpe = false;
        }
        partial void BeforeEncode();
        partial void AfterEncode();

        protected override void EncodePacket()
        {
            base.EncodePacket();
            BeforeEncode();
            byte id = (byte)(0x80 | NEEDS_B_AND_AS_FLAG);

            if ((Flags & 16) != 0 && Index != 0)
            {
                id |= NEEDS_B_AND_AS_FLAG;
            }
            Id = id;

            
            Write(Sequence_number);
            Write(Flags);
            Write(Length_IN_BITS,true);
            if (IsReliable())
            {
                Write(Reliable_frame_index);
            }
            if (IsSequenced())
            {
               Write(Sequenced_frame_index);
            }
            if (IsOrdered())
            {
                Write(Ordered_frame_index);
                Write(Order_channel);
            }
            if ((Flags & 16) != 0)
            {
                Write(Compound_size,true);
                Write(Compound_ID,true);
                Write(Index, true);
            }
            Write(body);
            AfterEncode();
        }
        partial void BeforeDecode();
        partial void AfterDecode();
        protected override void DecodePacket()
        {
            base.DecodePacket();
            BeforeDecode();
            AfterDecode();
        }
        protected override void ResetPacket()
        {
            base.ResetPacket();

        }
        public bool IsFragment()
        {
            return (Flags & 16) != 0;
        }

        public bool IsReliable()
        {
            Reliability r = (Reliability)((Flags & 224) >> 5);
            return r == Reliability.Reliable || r == Reliability.ReliableOrdered || r == Reliability.ReliableSequenced;
        }

        public bool IsOrdered()
        {
            Reliability r = (Reliability)((Flags & 224) >> 5);
            return r == Reliability.UnreliableSequenced || r == Reliability.ReliableOrdered || r == Reliability.ReliableSequenced;
        }

        public bool IsSequenced()
        {
            Reliability r = (Reliability)((Flags & 224) >> 5);
            return r == Reliability.UnreliableSequenced || r == Reliability.ReliableSequenced;
        }

        public Reliability GetReliability()
        {
            return (Reliability)((Flags & 224) >> 5);
        }
    }
}
