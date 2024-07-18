#pragma warning disable 
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Simple.RakNet.Utils;
using Simple.RakNet.Utils.IO;
using System.Collections.Concurrent;
using Microsoft.IO;
namespace Simple.RakNet.Packets
{
    public abstract partial class Packet
    {
        public ReadOnlyMemory<byte> Buffer { get; set; }
        public required bool IsMcpe { get; set; }
        public required int Id { get; set; }
        protected private Stream? _buffer;
        private BinaryWriter? _writer;
        protected MemoryStreamReader? _reader;
        public static Byte[] MAGIC = new byte[] { 0x00, 0xff, 0xff, 0x00, 0xfe, 0xfe, 0xfe, 0xfe, 0xfd, 0xfd, 0xfd, 0xfd, 0x12, 0x34, 0x56, 0x78 };
        public bool Is_used = false;
        private object _encodeSync = new object();
        private byte[]? _encodedMessage;
        private static RecyclableMemoryStreamManager _streamManager = new RecyclableMemoryStreamManager();
        private static ConcurrentDictionary<int, bool> _isLob = new ConcurrentDictionary<int, bool>();
        public void Write(sbyte value)
        {
            _writer?.Write(value);
        }
        public void Write(object a)
        {

        }
        public object? Read(object a)
        {
            return null;
        }
        public sbyte? ReadSByte()
        {
            return (sbyte?)_reader?.ReadByte();
        }

        public void Write(byte value)
        {
            _writer?.Write(value);
        }

        public byte ReadByte()
        {
            return (byte)_reader?.ReadByte();
        }

        public void Write(bool value)
        {
            Write((byte)(value ? 1 : 0));
        }

        public bool ReadBool()
        {
            return _reader?.ReadByte() != 0;
        }

        public void Write(Memory<byte> value)
        {
            Write((ReadOnlyMemory<byte>)value);
        }

        public void Write(ReadOnlyMemory<byte> value)
        {

            _writer?.Write(value.Span);
        }

        public void Write(byte[] value)
        {


            _writer?.Write(value);
        }

        public ReadOnlyMemory<byte> Slice(int count)
        {
            return _reader.Read(count);
        }

        public ReadOnlyMemory<byte> ReadReadOnlyMemory(int count, bool slurp = false)
        {

            if (!slurp && count == 0) return Memory<byte>.Empty;

            if (count == 0)
            {
                count = (int)(_reader?.Length - _reader?.Position);
            }

            ReadOnlyMemory<byte> readBytes = _reader.Read(count);
            if (readBytes.Length != count) throw new ArgumentOutOfRangeException($"Expected {count} bytes, only read {readBytes.Length}.");
            return readBytes;
        }

        public byte[] ReadBytes(int count, bool slurp = false)
        {
            if (!slurp && count == 0) return new byte[0];

            if (count == 0)
            {
                count = (int)(_reader.Length - _reader.Position);
            }
            ReadOnlyMemory<byte> readBytes = _reader.Read(count);
            if (readBytes.Length != count) throw new ArgumentOutOfRangeException($"Expected {count} bytes, only read {readBytes.Length}.");
            return readBytes.ToArray(); //TODO: Replace with ReadOnlyMemory<byte> return
        }
        public void WriteByteArray(byte[] value)
        {
            if (value == null)
            {
                WriteLength(0);
                return;
            }

            WriteLength(value.Length);

            if (value.Length == 0) return;

            _writer?.Write(value, 0, value.Length);
        }

        public byte[] ReadByteArray(bool slurp = false)
        {
            var len = ReadLength();
            var bytes = ReadBytes(len, slurp);
            return bytes;
        }

        public void Write(ulong[] value)
        {
            if (value == null)
            {
                WriteLength(0);
                return;
            }

            WriteLength(value.Length);

            if (value.Length == 0) return;
            for (int i = 0; i < value.Length; i++)
            {
                ulong val = value[i];
                Write(val);
            }
        }

        public ulong[] ReadUlongs(bool slurp = false)
        {
            var len = ReadLength();
            var ulongs = new ulong[len];
            for (int i = 0; i < ulongs.Length; i++)
            {
                ulongs[i] = ReadUlong();
            }
            return ulongs;
        }

        public void Write(short value, bool bigEndian = false)
        {
            if (bigEndian) _writer?.Write(BinaryPrimitives.ReverseEndianness(value));
            else _writer?.Write(value);
        }

        public short ReadShort(bool bigEndian = false)
        {
            if (_reader.Position == _reader.Length) return 0;

            if (bigEndian) return BinaryPrimitives.ReverseEndianness(_reader.ReadInt16());

            return _reader.ReadInt16();
        }

        public void Write(ushort value, bool bigEndian = false)
        {
            if (bigEndian) _writer?.Write(BinaryPrimitives.ReverseEndianness(value));
            else _writer?.Write(value);
        }

        public ushort ReadUshort(bool bigEndian = false)
        {
            if (_reader.Position == _reader.Length) return 0;

            if (bigEndian) return BinaryPrimitives.ReverseEndianness(_reader.ReadUInt16());

            return _reader.ReadUInt16();
        }

        public void WriteBe(short value)
        {
            _writer?.Write(BinaryPrimitives.ReverseEndianness(value));
        }

        public short ReadShortBe()
        {
            if (_reader.Position == _reader.Length) return 0;

            return BinaryPrimitives.ReverseEndianness(_reader.ReadInt16());
        }

        public void Write(Int24 value)
        {
            _writer?.Write(value.GetBytes());
        }

        public Int24 ReadLittle()
        {
            return new Int24(_reader.Read(3).Span);
        }

        public void Write(int value, bool bigEndian = false)
        {
            if (bigEndian) _writer?.Write(BinaryPrimitives.ReverseEndianness(value));
            else _writer?.Write(value);
        }

        public int ReadInt(bool bigEndian = false)
        {
            if (bigEndian) return BinaryPrimitives.ReverseEndianness(_reader.ReadInt32());

            return _reader.ReadInt32();
        }

        public void WriteBe(int value)
        {
            _writer?.Write(BinaryPrimitives.ReverseEndianness(value));
        }

        public int ReadIntBe()
        {
            return BinaryPrimitives.ReverseEndianness(_reader.ReadInt32());
        }

        public void Write(uint value)
        {
            _writer?.Write(value);
        }

        public uint ReadUint()
        {
            return _reader.ReadUInt32();
        }


        public void WriteVarInt(int value)
        {
            VarInt.WriteInt32(_buffer, value);
        }

        public int ReadVarInt()
        {
            return VarInt.ReadInt32(_reader);
        }

        public void WriteSignedVarInt(int value)
        {
            VarInt.WriteSInt32(_buffer, value);
        }

        public int ReadSignedVarInt()
        {
            return VarInt.ReadSInt32(_reader);
        }

        public void WriteUnsignedVarInt(uint value)
        {
            VarInt.WriteUInt32(_buffer, value);
        }

        public uint ReadUnsignedVarInt()
        {
            return VarInt.ReadUInt32(_reader);
        }

        public int ReadLength()
        {
            return (int)VarInt.ReadUInt32(_reader);
        }

        public void WriteLength(int value)
        {
            VarInt.WriteUInt32(_buffer, (uint)value);
        }

        public void WriteVarLong(long value)
        {
            VarInt.WriteInt64(_buffer, value);
        }

        public long ReadVarLong()
        {
            return VarInt.ReadInt64(_reader);
        }

        public void WriteEntityId(long value)
        {
            WriteSignedVarLong(value);
        }

        public void WriteSignedVarLong(long value)
        {
            VarInt.WriteSInt64(_buffer, value);
        }

        public long ReadSignedVarLong()
        {
            return VarInt.ReadSInt64(_reader);
        }

        public void WriteRuntimeEntityId(long value)
        {
            WriteUnsignedVarLong(value);
        }

        public void WriteUnsignedVarLong(long value)
        {
            // Need to fix this to ulong later
            VarInt.WriteUInt64(_buffer, (ulong)value);
        }

        public long ReadUnsignedVarLong()
        {
            // Need to fix this to ulong later
            return (long)VarInt.ReadUInt64(_reader);
        }

        public void Write(long value)
        {
            _writer?.Write(BinaryPrimitives.ReverseEndianness(value));
        }

        public long ReadLong()
        {
            return BinaryPrimitives.ReverseEndianness(_reader.ReadInt64());
        }

        public void Write(ulong value)
        {
            _writer?.Write(value);
        }

        public ulong ReadUlong()
        {
            return _reader.ReadUInt64();
        }

        public void Write(float value)
        {
            _writer?.Write(value);

            //byte[] bytes = BitConverter.GetBytes(value);
            //_writer?.Write(bytes[3]);
            //_writer?.Write(bytes[2]);
            //_writer?.Write(bytes[1]);
            //_writer?.Write(bytes[0]);
        }

        public float ReadFloat()
        {
            //byte[] buffer = _reader.ReadBytes(4);
            //return BitConverter.ToSingle(new[] {buffer[3], buffer[2], buffer[1], buffer[0]}, 0);
            return _reader.ReadSingle();
        }

        public void Write(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                WriteLength(0);
                return;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(value);

            WriteLength(bytes.Length);
            Write(bytes);
        }

        public string ReadString()
        {
            if (_reader.Position == _reader.Length) return string.Empty;
            int len = ReadLength();
            if (len <= 0) return string.Empty;
            return Encoding.UTF8.GetString(ReadBytes(len));
        }

        public void WriteFixedString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Write((short)0, true);
                return;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(value);

            Write((short)bytes.Length, true);
            Write(bytes);
        }

        public string ReadFixedString()
        {
            if (_reader.Position == _reader.Length) return string.Empty;
            short len = ReadShort(true);
            if (len <= 0) return string.Empty;
            return Encoding.UTF8.GetString(_reader.Read(len).Span);
        }

        public void Write(IPEndPoint endpoint)
        {
            if (endpoint.AddressFamily == AddressFamily.InterNetwork)
            {
                Write((byte)4);
                var parts = endpoint.Address.ToString().Split('.');
                foreach (var part in parts)
                {
                    Write((byte)~byte.Parse(part));
                }
                Write((short)endpoint.Port, true);
            }
        }

        public IPEndPoint ReadIPEndPoint()
        {
            byte ipVersion = ReadByte();

            IPAddress address = IPAddress.Any;
            int port = 0;

            if (ipVersion == 4)
            {
                string ipAddress = $"{(byte)~ReadByte()}.{(byte)~ReadByte()}.{(byte)~ReadByte()}.{(byte)~ReadByte()}";
                address = IPAddress.Parse(ipAddress);
                port = (ushort)ReadShort(true);
            }
            else if (ipVersion == 6)
            {
                ReadShort(); // Address family
                port = (ushort)ReadShort(true); // Port
                ReadLong(); // Flow info
                var addressBytes = ReadBytes(16);
                address = new IPAddress(addressBytes);
            }
            else
            {

            }

            return new IPEndPoint(address, port);
        }

        public void Write(IPEndPoint[] endpoints)
        {
            foreach (var endpoint in endpoints)
            {
                Write(endpoint);
            }
        }

        public IPEndPoint[] ReadIPEndPoints(int count)
        {
            if (count == 20 && _reader.Length < 120) count = 10;
            var endPoints = new IPEndPoint[count];
            for (int i = 0; i < endPoints.Length; i++)
            {
                endPoints[i] = ReadIPEndPoint();
            }

            return endPoints;
        }

        public virtual void Reset()
        {
            ResetPacket();
            _encodedMessage = null;
            Buffer = null;
            _writer?.Close();
            _reader?.Close();
            _buffer?.Close();
            _writer = null;
            _reader = null;
            _buffer = null;
        }
        protected virtual void ResetPacket()
        {

        }
        public virtual byte[] Encode()
        {
            byte[] cache = _encodedMessage;
            if (cache != null) return cache;

            lock (_encodeSync)
            {
                // This construct to avoid unnecessary contention and double encoding.
                if (_encodedMessage != null) return _encodedMessage;

                // Dynamic pooling. If this packet has been registered as a large object in previous
                // runs, we use the pooled stream for it instead to avoid LOB allocations
                bool isLob = _isLob.ContainsKey(Id);
                _buffer = isLob ? _streamManager.GetStream() : new MemoryStream();
                using (_writer = new BinaryWriter(_buffer, Encoding.UTF8, true))
                {
                    EncodePacket();

                    _writer.Flush();
                    // This WILL allocate LOB. Need to convert this to work with array segment and pool it.
                    // then we will use GetBuffer instead.
                    // Also remember to move dispose entirely to Reset (dispose) when that happens.
                    var buffer = (MemoryStream)_buffer;
                    _encodedMessage = buffer.ToArray();
                    if (!isLob && _encodedMessage.Length >= 85_000)
                    {
                        _isLob.TryAdd(Id, true);
                        //Log.Warn($"LOB {GetType().Name} {_encodedMessage.Length}, IsLOB={_isLob}");
                    }
                }
                _buffer.Dispose();

                _writer = null;
                _buffer = null;
                return _encodedMessage;
            }
        }
        public virtual Packet Decode(ReadOnlyMemory<byte> buffer)
        {
            Buffer = buffer;
            _reader = new MemoryStreamReader(buffer);
            DecodePacket();
            _reader.Dispose();
            _reader = null;
            return this;
        }
        protected virtual void EncodePacket()
        {
            _buffer.Position = 0;
            if (IsMcpe) WriteVarInt(Id);
            else Write((byte)Id);
        }
        protected virtual void DecodePacket()
        {
            Id = IsMcpe ? ReadVarInt() : ReadByte();
        }
        //TODO:rewrite this ⬇
        private static string HexDump(ReadOnlySpan<byte> bytes, in int bytesPerLine, in bool printLineCount)
        {
            var sb = new StringBuilder();
            for (int line = 0; line < bytes.Length; line += bytesPerLine)
            {
                byte[] lineBytes = bytes.Slice(line).ToArray().Take(bytesPerLine).ToArray();
                if (printLineCount) sb.AppendFormat("{0:x8} ", line);
                sb.Append(string.Join(" ", lineBytes.Select(b => b.ToString("x2"))
                        .ToArray())
                    .PadRight(bytesPerLine * 3));
                sb.Append(" ");
                sb.Append(new string(lineBytes.Select(b => b < 32 ? '.' : (char)b)
                    .ToArray()));
                sb.AppendLine();
            }
            return sb.ToString();
        }

    }
}
