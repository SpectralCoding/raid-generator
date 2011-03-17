using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RAIDGenerator {
	class Program {
		static void Main(string[] args) {
			RaidController RaidController;

			Console.WriteLine("Welcome to the SpectralCoding RAID Generator!");
			Console.WriteLine("\"Disk\" to be converted to raid: .\\Workspace\\utorrent.exe");
			Console.WriteLine("Drive Bay: .\\Workspace\\Disks\\\n");

			Console.Write("Please enter a RAID Mode: ");

			RaidController = new RaidController(Console.ReadLine());

			Console.WriteLine("\nRaid Mode Set: " + RaidController.RaidMode.Description);


			Console.Write("\nPress ENTER to exit... ");
			Console.ReadLine();
		}
	}
}
