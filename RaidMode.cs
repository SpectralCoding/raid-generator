using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RAIDGenerator {
	class RaidMode {

		public string Title = String.Empty;
		public string Description = String.Empty;
		public int MinimumDisks = 0;

		public RaidMode(string inputRaidMode) {
			switch (inputRaidMode) {
				case "0":
					Title = "RAID0";
					Description = "RAID0 - Block-level striping without parity or mirroring.";
					MinimumDisks = 2;
					break;
				case "1":
					Title = "RAID1";
					Description = "RAID1 - Mirroring without parity or striping.";
					MinimumDisks = 2;
					break;
				case "2":
					Title = "RAID2";
					Description = "RAID2 - Bit-level striping with dedicated Hamming-code parity.";
					MinimumDisks = 3;
					break;
				case "3":
					Title = "RAID3";
					Description = "RAID3 - Byte-level striping with dedicated parity.";
					MinimumDisks = 3;
					break;
				case "4":
					Title = "RAID4";
					Description = "RAID4 - Block-level striping with dedicated parity.";
					MinimumDisks = 3;
					break;
				case "5":
					Title = "RAID5";
					Description = "RAID5 - Block-level striping with distributed parity.";
					MinimumDisks = 3;
					break;
				case "6":
					Title = "RAID6";
					Description = "RAID6 - Block-level striping with double distributed parity.";
					MinimumDisks = 4;
					break;
			}

		}
	}
}
