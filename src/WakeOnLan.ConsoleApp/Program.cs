using static System.Console;

using System.Net.Extension;
using System.Net;
using static WakeOnLan.ConsoleApp.Utils;


switch (args.Length)
{
	case 0: WriteLine("No args! "); break;
	case 1: Handle(args[0]); break;
	case 2: Handle(args[0], args[1]); break;
	default: WriteLine(args[0]); break;
}

namespace WakeOnLan.ConsoleApp
{
	static class Utils
	{
		enum CommandType
		{
			Default = 0,
			IP = 1,
			HostName = 2,
			Help,
			Wrong,
		}
		static CommandType AnalyseCommandType(string command)
		{
			if (command.Length != 2)
				return CommandType.Wrong;
			if (!(command[0] == '/' || command[0] == '\\' || command[0] == '-'))
				return CommandType.Wrong;
			return command[1] switch
			{
				'a' => CommandType.IP,
				'n' => CommandType.HostName,
				'h' => CommandType.Help,
				_ => CommandType.Wrong,
			};
		}
		internal static void Handle(string command)
		{
			var type = AnalyseCommandType(command);
			if (type == CommandType.Wrong)
			{
				var mac = new MAC(command);
				Library.WakeOnLan.Wake(mac);
				WriteLine($"已向 {mac} 发送唤醒幻数据包");
			}
			else
			{
				WriteLine("使用WOL技术唤醒目标设备\n");
				WriteLine("${exeName} ${MAC address:like aa-bb-cc-dd-ee-ff or aabbccddeeff}");
				WriteLine("${exeName} -a ${IP address:like 192.168.1.2}");
				WriteLine("${exeName} -n ${Host name}");
			}
		}
		internal static void Handle(string command, string arg)
		{
			var type = AnalyseCommandType(command);
			switch (type)
			{
				case CommandType.IP:
					if (IPAddress.TryParse(arg, out IPAddress? address))
					{
						if (Library.WakeOnLan.Wake(address) == Library.WakeOnLan.SendARPReturnCode.NO_ERROR)
							WriteLine($"已向 {address} 发送唤醒幻数据包");
						else
							WriteLine($"未在当前局域网下找到 {address} ");
					}
					return;
				case CommandType.HostName:
					if (Library.WakeOnLan.Wake(arg) == Library.WakeOnLan.SendARPReturnCode.NO_ERROR)
						WriteLine($"已向 {arg} 发送唤醒幻数据包");
					else
						WriteLine($"未在当前局域网下找到 {arg} ");
					return;
				case CommandType.Wrong: WriteLine("Wrong command type!"); return;
				default: WriteLine("Wrong command type!"); return;
			}

		}
	}
}