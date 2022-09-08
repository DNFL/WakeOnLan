using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;


namespace System.Net.Extension
{
	[StructLayout(LayoutKind.Explicit, Size = 6)]
	public unsafe partial struct MAC   //Media Access Control Address
	{
		/// <summary>
		/// 最大权重字节 (Most Significant Byte)
		/// </summary>
		[FieldOffset(0)]
		[MarshalAs(UnmanagedType.U1)]
		public readonly byte MSB = default; //Most Significant Byte： 最大权重字节
		/// <summary>
		/// 固定缓冲区 最高有效字节位于索引位置 0 
		/// </summary>
		[FieldOffset(0)]
		[MarshalAs(UnmanagedType.Struct)]
		internal fixed byte _bytes[6];  //最高有效字节位于索引位置 0
		#region 条件编译
#if DEBUG
		[FieldOffset(0)]
		[MarshalAs(UnmanagedType.U1)]
		private readonly byte byte0 = default;   //高端第一字节
		[FieldOffset(1)]
		[MarshalAs(UnmanagedType.U1)]
		private readonly byte byte1 = default;   //高端第二字节
		[FieldOffset(2)]
		[MarshalAs(UnmanagedType.U1)]
		private readonly byte byte2 = default;   //高端第三字节
		[FieldOffset(3)]
		[MarshalAs(UnmanagedType.U1)]
		private readonly byte byte3 = default;   //高端第四字节
		[FieldOffset(4)]
		[MarshalAs(UnmanagedType.U1)]
		private readonly byte byte4 = default;   //高端第五字节
		[FieldOffset(5)]
		[MarshalAs(UnmanagedType.U1)]
		private readonly byte byte5 = default;   //高端第六字节
#endif
		#endregion
		/// <summary>
		/// 
		/// </summary>
		/// <param name="address">is between 00-00-00-00-00-00 and FF-FF-FF-FF-FF-FF. </param>
		/// <exception cref="ArgumentException"></exception>
		public MAC(long address)
		{
			if (address > MaxValue)
				throw new ArgumentException("The parameter 'address' exceeds 48 bits. ");
			else if (address < MinValue)
				throw new ArgumentException("The parameter 'address' is negative. ");
			byte* ptr = (byte*)&address;
			_bytes[0] = ptr[0];
			_bytes[1] = ptr[1];
			_bytes[2] = ptr[2];
			_bytes[3] = ptr[3];
			_bytes[4] = ptr[4];
			_bytes[5] = ptr[5];
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="address">is an array with 6 bytes. </param>
		/// <exception cref="ArgumentException"></exception>
		public MAC(byte[] address)
		{
			if (address.Length != Size)
				throw new ArgumentException("The 'Length' of parameter byte array 'address' is not 6. ");
			_bytes[0] = address[0];
			_bytes[1] = address[1];
			_bytes[2] = address[2];
			_bytes[3] = address[3];
			_bytes[4] = address[4];
			_bytes[5] = address[5];
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="address">is represented as XX-XX-XX-xx-xx-xx or xx:xx:xx:XX:XX:XX or XXXX_XXxx_xxxx or so on. </param>
		/// <exception cref="ArgumentException"></exception>
		public MAC(string address)  //xx-xx-xx-xx-xx-xx, xx:xx:xx:xx:xx:xx, xxxx_xxxx_xxxx, start with hex numebr which between 0 and F
		{
			var temp = stackalloc byte[6];

			int index = 0;
			int times = 0;
			while (index < address.Length)
			{
				char ch = address[index];
				if (!((ch >= '0' && ch <= '9')
					|| (ch >= 'A' && ch <= 'F')
					|| (ch >= 'a' && ch <= 'f')))
					index++;

				if (!byte.TryParse(address.AsSpan(index, 2), Globalization.NumberStyles.HexNumber, null, out temp[times++]))
					throw new ArgumentException("The format of parameter 'address' is valid. ");
				index += 2;
			}
			if (times == 6)
			{
				_bytes[0] = temp[0];
				_bytes[1] = temp[1];
				_bytes[2] = temp[2];
				_bytes[3] = temp[3];
				_bytes[4] = temp[4];
				_bytes[5] = temp[5];
			}
			else throw new ArgumentException("The format of parameter 'address' is invalid. ");
		}

		/// <summary>
		/// The long value of the MAC address. 
		/// </summary>
		public long Address
		{
			get
			{
				fixed (byte* ptr = _bytes)
					return *(uint*)ptr | (long)*(ushort*)(ptr + 4) << 8 * 4;
			}
		}
		/// <summary>
		/// 组织唯一标识符
		/// </summary>
		public int OrganizationallyUniqueIdentifier
		{
			get
			{
				fixed (byte* ptr = _bytes)
				{
					return (int)(*(uint*)ptr & 0x_00_FF__FF_FF);
				}
			}
		}
		/// <summary>
		/// Abbreviation for Organizationally Unique Identifier
		/// </summary>
		public int OUI => OrganizationallyUniqueIdentifier;
		/// <summary>
		/// 网络接口控制器序列号
		/// </summary>
		public int NetworkInterfaceControllerSpecific
		{
			get
			{
				fixed (byte* ptr = _bytes)
					return (int)(*(uint*)(ptr + 2) >> 8 * 1);
			}
		}
		/// <summary>
		/// Abbreviation for Network Interface Controller Specific
		/// </summary>
		public int NIC => NetworkInterfaceControllerSpecific;
		/// <summary>
		/// 是否为默认：单播、全局 (is it Default: Unicast and Universal) 
		/// </summary>
		public bool IsDefault => (MACAttribute)(MSB & 0b_11) == MACAttribute.Default;
		/// <summary>
		/// 是否为单播 (is it Unicast)
		/// </summary>
		public bool IsUnicast => IsDefault;
		/// <summary>
		/// 是否为多播 (is it Multicast)
		/// </summary>
		public bool IsMulticast => ((MACAttribute)MSB & MACAttribute.Multicast) != 0;
		/// <summary>
		/// 是否为全局 (is it Universal)
		/// </summary>
		public bool IsUniversal => IsDefault;
		/// <summary>
		/// 是否为本地 (is it Local)
		/// </summary>
		public bool IsLocal => ((MACAttribute)MSB & MACAttribute.Local) != 0;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public byte this[int index]
		{
			get
			{
				if (index < 0 || index >= Size)
					throw new ArgumentOutOfRangeException();
				return _bytes[index];
			}
		}
		/// <summary>
		/// 转换为 byte 数组
		/// </summary>
		/// <returns>byte 数组 型 MAC 地址</returns>
		public byte[] ToBytes() => new byte[] { _bytes[5], _bytes[4], _bytes[3], _bytes[2], _bytes[1], _bytes[0] };
		/// <summary>
		/// 转换为 string
		/// </summary>
		/// <returns>string 型 MAC 地址 (FF-FF-FF-FF-FF-FF)</returns>
		public override string ToString()
			=> $"{_bytes[0]:X2}-{_bytes[1]:X2}-{_bytes[2]:X2}-{_bytes[3]:X2}-{_bytes[4]:X2}-{_bytes[5]:X2}";
	}
}
