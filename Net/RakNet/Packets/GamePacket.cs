using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.RakNet.Packets
{
    public partial class GamePacket : Packet
    {
        public ReadOnlyMemory<byte> payload; // = null;


        partial void AfterEncode()
        {
            Write(payload);
        }

        partial void AfterDecode()
        {
            payload = ReadReadOnlyMemory(0, true);
        }
    }
}
