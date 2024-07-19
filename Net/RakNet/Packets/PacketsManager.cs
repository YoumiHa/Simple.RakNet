using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Intrinsics.Wasm;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using Simple.RakNet.Packets;
namespace Simple.RakNet.Net.Packets
{
    public class Manager
    {
        public static readonly ConcurrentDictionary<PacketRegister, Type> Packets = new ConcurrentDictionary<PacketRegister, Type>();
        public static void InitPacketsList()
        {
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetAssembly(typeof(PacketRegister));
            System.Type[] types = asm.GetExportedTypes();
           // List<PacketRegister> registers = new List<PacketRegister>();
            Func<Attribute[], bool> IsMyAttribute = o =>
            {
                foreach (Attribute a in o)
                {
                    if (a is PacketRegister)
                       // registers.Add((PacketRegister)a);
                        return true;
                }
                return false;
            };

            System.Type[] cosType = types.Where(o =>
            {
                return IsMyAttribute(System.Attribute.GetCustomAttributes(o, true));
            }
            ).ToArray();
            foreach (var i in cosType)
            {
                PacketRegister attribute = i.GetCustomAttribute<PacketRegister>();
                Packets.TryAdd(attribute,i);
            }
        }
        public static Packet CreatePacket(PacketRegister register, byte[] bytes)
        {
            Type pk_type;
            var bo = Packets.TryGetValue(register, out pk_type);
            if (bo == false)
            {
                return null;
            }
            var da = Activator.CreateInstance(pk_type);
            ((Packet)da).Id = register.m_id;
            ((Packet)da).Buffer = bytes;
            return (Packet)da;
        }
    }
    public class PacketRegister : Attribute 
    { 
        public int m_id { get;private set; }
        public bool m_ismc { get; private set; }
        public PacketRegister(int id,bool IsMc) 
        {
            m_id = id;
            m_ismc = IsMc;
        }
    }
}
