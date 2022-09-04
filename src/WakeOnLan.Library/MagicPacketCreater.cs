using System;
using System.Net.Extension;

namespace WakeOnLan.Library
{
	public static class MagicPacketCreater
	{
		/// <summary>
		/// 数据长度 ( 6 + 6 * 16 = 102)
		/// </summary>
		public const int Size = 6 + MAC.Size * 16;

		/// <summary>
		/// Create the MagicPacket which contains the MAC address of the target. 
		/// </summary>
		/// <param name="address">of the target</param>
		/// <returns></returns>
		public static byte[] Create(MAC address)
		{
			var data = new byte[Size];
			Span<byte> mac = stackalloc byte[MAC.Size] 
				{ address[0], address[1], address[2], address[3], address[4], address[5]};

			int index;

			for (index = 0; index < 6; index++)
				data[index] = 0x_FF;
			//data[0] = 0x_FF;
			//data[1] = 0x_FF;
			//data[2] = 0x_FF;
			//data[3] = 0x_FF;
			//data[4] = 0x_FF;
			//data[5] = 0x_FF;
			//index = 6;
			while (index < Size)
			{
				data[index++] = mac[0];
				data[index++] = mac[1];
				data[index++] = mac[2];
				data[index++] = mac[3];
				data[index++] = mac[4];
				data[index++] = mac[5];
			}
			return data;
		}
	}
}
