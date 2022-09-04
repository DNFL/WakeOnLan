using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Extension
{
	/*		
	 *		C类：192.168.0.0 - 192.168.255.255
	 *		B类：172.16.0.0 - 172.31.255.255		#小型的局域网
	 *		A类：10.0.0.0 - 10.255.255.255		#一般大型局域网用的
	*/
	public static class LocalAreaNetworkForIPv4
	{
		public const uint AMask = 0x_0000_00FF;

		public const uint ALAN = 0x_0000_000A;

		public const uint BMask = 0x_0000_F0FF;

		public const uint BLAN = 0x_0000_10AC;

		public const uint CMask = 0x_0000_FFFF;

		public const uint CLAN = 0x_0000_A8C0;

		public const uint DefaultMask = CMask;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="IPAddress"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentException"></exception>
		public static bool IsLocalAreaNetworkForIPv4(this IPAddress IPAddress)
		{
			if (IPAddress == null)
				throw new ArgumentNullException("IPAddress");
			if (IPAddress.AddressFamily != Sockets.AddressFamily.InterNetwork)
				throw new ArgumentException("The 'AddressFamily' of parameter 'IPAddress' is not 'InterNetwork' which is IPv4. ");
#pragma warning disable CS0618 // 类型或成员已过时
			uint specified = (uint)IPAddress.Address & DefaultMask;
#pragma warning restore CS0618 // 类型或成员已过时
			if (specified == CLAN) 
				return true;
			if ((specified & BMask) == BLAN)
				return true;
			if ((specified & AMask) == ALAN)
				return true;

			return false;
		}

		//public static IEnumerable<IPAddress> EnumClassC()
		//{
		//	const long CSpan = 0x_0001_0000;
		//	const int CCount = 0x_FF;
		//	const long CStart = 0x_FF00_A8C0;
		//	const long CEnd = CStart + CSpan * CCount;
		//	//const long BStart = 0x_FFFF_10AC;
		//	//const long BEnd = 0x_FFFF_1FAC;
		//	//const long AStart = 

		//	long now = CStart;
		//	while (now <= CEnd)
		//	{
		//		yield return new IPAddress(now);
		//		now += CSpan;
		//	}
		//}
	}
}
