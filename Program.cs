using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RAIDGenerator {
	class Program {
		static void Main(string[] args) {
			RaidController RaidController;
			bool AutoRun = true;

			Console.WriteLine("Welcome to the SpectralCoding RAID Generator!");
			Console.WriteLine("\"Disk\" to be converted to raid: .\\Workspace\\utorrent.exe");
			Console.WriteLine("Drive Bay: .\\Workspace\\Disks\\");

			if (AutoRun) {
				RaidController = new RaidController("4", 4, 1);
			} else {
				Console.Write("Please enter a RAID Mode: ");
				string RaidMode = Console.ReadLine();
				Console.Write("Please enter the number of disks in the array: ");
				int NumOfDisks = Convert.ToInt32(Console.ReadLine());
				Console.Write("Please enter the size of the disks (in MB): ");
				int DiskSize = Convert.ToInt32(Console.ReadLine());
				RaidController = new RaidController(RaidMode, NumOfDisks, DiskSize);
			}

			Console.WriteLine("\n" + RaidController.DisksInArray.ToString() + " Disk Raid Mode Set: " + RaidController.RaidMode.Description + "\n");

			RaidController.GenerateArray(@"C:\Users\Administrator\Desktop\RAID Generator\Workspace\drive.img");
			RaidController.RecombineArray(@"C:\Users\Administrator\Desktop\RAID Generator\Workspace\drive_regen.img", 0, 1048576);

			Console.Write("\nPress ENTER to exit... ");
			Console.ReadLine();
		}
	}
}
