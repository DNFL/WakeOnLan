using System;
using System.Net;
using System.Net.Extension;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace WakeOnLan.Library
{
	public static class WakeOnLan
	{
		/// <summary>
		/// WakeOnLan 默认端口
		/// </summary>
		public const short DefaultPort = 9;

		/// <summary>
		/// WakeOnLan 广播 IP 终结点
		/// </summary>
		public static IPEndPoint BroadcastIPEndPoint = new(IPAddress.Broadcast, DefaultPort);

		internal static UdpClient UdpClient;

		static WakeOnLan()
		{
			UdpClient = new(AddressFamily.InterNetwork);
			UdpClient.EnableBroadcast = true;
		}

		public const long LANBroadcastIPAddress = 0x_FF00_A8C0;

		public static void Wake(MAC address)
		{
			var packet = MagicPacketCreater.Create(address);
			var endPoint = BroadcastIPEndPoint;

			UdpClient.SendAsync(packet, packet.Length, endPoint);

			foreach (var NI in NetworkInterface.GetAllNetworkInterfaces())
			{
				foreach (var uniIP in NI.GetIPProperties().UnicastAddresses)
				{
					var IP = uniIP.Address;
					if (IP.AddressFamily == AddressFamily.InterNetwork && LocalAreaNetworkForIPv4.IsLocalAreaNetworkForIPv4(IP))
					{
						var addr = IP.GetAddressBytes();
						var mask = uniIP.IPv4Mask.GetAddressBytes();
						endPoint.Address = new(new byte[] { (byte)(addr[0] | ~mask[0]), (byte)(addr[1] | ~ mask[1]),
							(byte)(addr[2] | ~ mask[2]), (byte)(addr[3] | ~mask[3])});
						UdpClient.SendAsync(packet, packet.Length, endPoint);
					}
				}
			}
		}

		public static SendARPReturnCode Wake(IPAddress address)
		{
			MAC mac = MAC.BroacastAddress;
			int size = MAC.Size;

			if (address == null)
				throw new ArgumentNullException(nameof(address));
			if (address.AddressFamily != AddressFamily.InterNetwork)
				throw new ArgumentException($"The '{nameof(AddressFamily)}' of the parameter '{nameof(address)}' is not '{nameof(AddressFamily.InterNetwork)}' which is IPv4. ");
#pragma warning disable CS0618 // 类型或成员已过时
			var re = SendARP((uint)address.Address, 0, ref mac, ref size);
#pragma warning restore CS0618 // 类型或成员已过时
			if (re == SendARPReturnCode.NO_ERROR)
				Wake(mac);

			return re;
		}

		public static SendARPReturnCode Wake(string hostNameOrAddress)
		{ 
			IPAddress[] IPs;
			try
			{
				IPs = Dns.GetHostAddresses(hostNameOrAddress);
				foreach (var IP in IPs)
					return Wake(IP);
			}
			catch (Exception)
			{
				return SendARPReturnCode.ERROR_NOT_FOUND;
			}
			return SendARPReturnCode.NO_ERROR;
		}

		public enum SendARPReturnCode : short
		{
			NO_ERROR = 0,
			ERROR_BAD_NET_NAME = 67,			//The network name cannot be found.
			ERROR_BUFFER_OVERFLOW = 111,		//The file name is too long.
			ERROR_GEN_FAILURE = 31,				//A device attached to the system is not functioning.
			ERROR_INVALID_PARAMETER = 87,		//参数不正确
			ERROR_INVALID_USER_BUFFER = 1784,   //The supplied user buffer is not valid for the requested operation.
			ERROR_NOT_FOUND = 1168,				//Element not found.
			ERROR_NOT_SUPPORTED = 50,           //The request is not supported.
			OTHER = short.MaxValue,
		}
		[DllImport("Iphlpapi.dll")]
		private static extern SendARPReturnCode SendARP(uint dest, uint host, ref MAC mac, ref int length);
	}
}