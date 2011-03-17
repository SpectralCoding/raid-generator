using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RAIDGenerator {
	class RaidController {

		private RaidMode m_RaidMode;
		private bool[] m_SourceDisk;
		private bool[][] m_DiskArray;

		public RaidMode RaidMode {
			get { return m_RaidMode; }
		}

		public RaidController(string inputRaidMode) {
			m_RaidMode = new RaidMode(inputRaidMode);
			ReadSourceDisk(@"C:\Users\Administrator\Desktop\RAID Generator\Workspace\utorrent.exe");
			SplitIntoDisks();
			WriteDisks();
		}

		private void SplitIntoDisks() {
			switch (m_RaidMode.Title) {
				case "RAID0":

					break;
			}
		}

		private void WriteDisks() {

		}

		private void ReadSourceDisk(string inputSourceDiskFilename) {
			byte[] tempHexDisc = null;
			string tempBinaryStr = String.Empty;
			int discPos = 0;
			FileStream tempFS = new FileStream(inputSourceDiskFilename, FileMode.Open, FileAccess.Read);
			BinaryReader tempBR = new BinaryReader(tempFS);
			long fileLength = new FileInfo(inputSourceDiskFilename).Length;
			tempHexDisc = tempBR.ReadBytes((int)fileLength);
			m_SourceDisk = new bool[tempHexDisc.Length * 8];
			for (int i = 0; i < tempHexDisc.Length; i++) {
				tempBinaryStr = Convert.ToString(tempHexDisc[i], 2).PadLeft(8, '0'); ;
				for (int j = 0; j < 8; j++) {
					if (tempBinaryStr.Substring(j, 1) == "1") {
						m_SourceDisk[discPos] = true;
					} else {
						m_SourceDisk[discPos] = false;
					}
					discPos++;
				}
			}
		}

		private void BoolArrayToFile(string inputOutputFile) {
			FileStream fs = File.Create(inputOutputFile);
			UTF8Encoding utf8 = new UTF8Encoding();
			BinaryWriter bw = new BinaryWriter(fs, utf8);
			string tempBinaryStr = String.Empty;
			for (int i = 0; i < m_SourceDisk.Length; i += 8) {
				for (int j = i; j < i + 8; j++) {
					if (m_SourceDisk[j]) {
						tempBinaryStr += "1";
					} else {
						tempBinaryStr += "0";
					}
				}
				bw.Write(Convert.ToByte(Convert.ToInt32(tempBinaryStr, 2)));
				tempBinaryStr = String.Empty;
			}
			fs.Close();

		}

	}
}
