using Simple.RakNet.Packets;
namespace Simple.RakNet.Packets
{
	public partial class OpenConnectionRequest1
	{
		public short mtuSize;

		partial void AfterEncode()
		{
			Write(new byte[mtuSize - _buffer.Position - 28]);
		}

		partial void AfterDecode()
		{
			mtuSize = (short) (_reader.Length + 28);
			ReadBytes((int) (_reader.Length - _reader.Position));
		}
	}
}