using System;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Simple.RakNet.Utils.IO
{
	public class MemoryStreamReader : Stream
	{
		public ReadOnlyMemory<byte> _buffer;

		public override long Position { get; set; }
		public override long Length => _buffer.Length;
		public override bool CanWrite => false;
		public override bool CanRead => true;
		public override bool CanSeek => true;

		public bool Eof => Position >= _buffer.Length;


		public MemoryStreamReader(ReadOnlyMemory<byte> buffer)
		{
			_buffer = buffer;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			if (offset > Length) throw new ArgumentOutOfRangeException(nameof(offset), "offset longer than stream");

			long tempPosition = Position;
			switch (origin)
			{
				case SeekOrigin.Begin:
					tempPosition = offset;
					break;
				case SeekOrigin.Current:
					tempPosition += offset;
					break;
				case SeekOrigin.End:
					tempPosition = Length + offset;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(origin), origin, null);
			}

			if (tempPosition < 0) throw new IOException("Seek before beginning of stream");

			Position = (int) tempPosition;

			return Position;
		}


		public override int ReadByte()
		{
			return _buffer.Span[(int) Position++];
		}

		public override void SetLength(long value)
		{
			if (value > _buffer.Length) throw new IOException("Can't set length beyond size of buffer");
			_buffer = _buffer.Slice(0, (int) value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
				throw new NotImplementedException("This stream can't be used for write operations");
		}

		public short ReadInt16()
		{
			var val = BinaryPrimitives.ReadInt16LittleEndian(_buffer.Span.Slice((int) Position, 2));
			Position += 2;
			return val;
		}

		public ushort ReadUInt16()
		{
			var val = BinaryPrimitives.ReadUInt16LittleEndian(_buffer.Span.Slice((int) Position, 2));
			Position += 2;
			return val;
		}

		public int ReadInt32()
		{
			int val = BinaryPrimitives.ReadInt32LittleEndian(_buffer.Span.Slice((int) Position, 4));
			Position += 4;
			return val;
		}

		public uint ReadUInt32()
		{
			uint val = BinaryPrimitives.ReadUInt32LittleEndian(_buffer.Span.Slice((int) Position, 4));
			Position += 4;
			return val;
		}

		public long ReadInt64()
		{
			long val = BinaryPrimitives.ReadInt64LittleEndian(_buffer.Span.Slice((int) Position, 8));
			Position += 8;
			return val;
		}

		public ulong ReadUInt64()
		{
			ulong val = BinaryPrimitives.ReadUInt64LittleEndian(_buffer.Span.Slice((int) Position, 8));
			Position += 8;
			return val;
		}

		public float ReadSingle()
		{
			float val = ReadSingleLittleEndian(_buffer.Span.Slice((int) Position, 4));
			Position += 4;
			return val;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float ReadSingleLittleEndian(ReadOnlySpan<byte> source)
		{
			float f = !BitConverter.IsLittleEndian ? BitConverter.Int32BitsToSingle(BinaryPrimitives.ReverseEndianness(MemoryMarshal.Read<int>(source))) : MemoryMarshal.Read<float>(source);
			return f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double ReadDoubleLittleEndian(ReadOnlySpan<byte> source)
		{
			double d = !BitConverter.IsLittleEndian ? BitConverter.Int64BitsToDouble(BinaryPrimitives.ReverseEndianness(MemoryMarshal.Read<long>(source))) : MemoryMarshal.Read<double>(source);
			return d;
		}

		//public uint ReadVarInt()
		//{
		//	return ReadVarIntInternal();
		//}

		//public int ReadSignedVarInt()
		//{
		//	return DecodeZigZag32((uint) ReadVarIntInternal());
		//}

		private static int DecodeZigZag32(uint n)
		{
			return (int) (n >> 1) ^ -(int) (n & 1);
		}

		public ulong ReadVarLong()
		{
			return ReadVarLongInternal();
		}

		public string ReadLengthPrefixedString()
		{
			return Encoding.UTF8.GetString(ReadLengthPrefixedBytes().Span);
		}

		public ReadOnlyMemory<byte> ReadLengthPrefixedBytes()
		{
			var length = ReadVarLongInternal();
			return Read(length);
		}

		private ulong ReadVarLongInternal()
		{
			ulong result = 0;
			for (int shift = 0; shift <= 63; shift += 7)
			{
				ulong b = _buffer.Span[(int) Position++];

				// add the lower 7 bits to the result
				result |= ((b & 0x7f) << shift);

				// if high bit is not set, this is the last byte in the number
				if ((b & 0x80) == 0)
				{
					return result;
				}
			}

			throw new Exception("last byte of variable length int has high bit set");
		}

		//public ReadOnlyMemory<byte> Read(ulong offset, ulong count)
		//{
		//	long n = Length - Position;
		//	if (n > (long) count)
		//		n = (long) count;
		//	if (n <= 0)
		//		return ReadOnlyMemory<byte>.Empty;

		//	Span<byte> result = new byte[offset + count];

		//	Read(n).CopyTo(result.Slice((int) offset, (int) n));

		//	return result;
		//}

		public ReadOnlyMemory<byte> Read(ulong length)
		{
			return Read((int) length);
		}

		public ReadOnlyMemory<byte> Read(long length)
		{
			return Read((int) length);
		}

		public ReadOnlyMemory<byte> Read(int length, bool boundCheck = true)
		{
			if (boundCheck && length > Length - Position) throw new ArgumentOutOfRangeException(nameof(length), length, $"Value outside of range: {Length - Position}");

			int readLen = (int) Math.Min(length, Length - Position);
			ReadOnlyMemory<byte> buffer = _buffer.Slice((int) Position, readLen);
			Position += buffer.Length;

			return buffer;
		}

		public override void Flush()
		{
			// DO NOTHING
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			ReadOnlyMemory<byte> bytes = Read(count, false);
			bytes.CopyTo(new Memory<byte>(buffer).Slice(offset, count));

			return bytes.Length;
		}

		public override void Close()
		{
			// DO NOTHING
		}
	}
}