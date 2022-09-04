using System.Net.Extension;

using WakeOnLan.Library;

namespace WankOnLan.Test.WakeOnLan.Library.Test
{
	[TestClass]
	public class MagicPacketCreaterTests
	{
		[TestMethod]
		public void TestCreate()
		{
			MAC mac = new MAC("F1F2F3F4F5F6");

			Assert.IsTrue(MagicPacketCreater.Size == 102);

			var packet = MagicPacketCreater.Create(mac);

			Assert.IsTrue(packet.Length == MagicPacketCreater.Size);

			int index = 0;
			while (index < 6)
				Assert.IsTrue(packet[index++] == 0xFF);

			while (index < 102)
			{
				for (int i = 0; i < 6; i++)
				{
					Assert.IsTrue(packet[index++] == mac[i]);
				}
			}	
		}
	}
}
