using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Extension
{
	public partial struct MAC
	{
		/// <summary>
		/// 字节数 (bytes count)
		/// </summary>
		public const int Size = 6;
		/// <summary>
		/// 最大有效整型值 (maximum valid integer value)
		/// </summary>
		public const long MaxValue = 0x_FF_FF_FF_FF_FF_FF;
		/// <summary>
		/// 最小有效整型值 (minimum valid integer value)
		/// </summary>
		public const long MinValue = 0;

		public static readonly MAC BroacastAddress = new(MaxValue);
		/// <summary>
		/// Classification of MAC addresses
		/// </summary>
		[Flags]
		public enum MACAttribute : byte
		{
			/// <summary>
			/// 默认：单播、全局 (Default: Unicast and Universal) 
			/// </summary>
			Default = 0b_00,    //Unicast and Universal
			/// <summary>
			/// 多播
			/// </summary>
			Multicast = 0b_01,
			/// <summary>
			/// 本地
			/// </summary>
			Local = 0b_10,
		}
	}
}
