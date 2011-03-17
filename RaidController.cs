using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RAIDGenerator {
	class RaidController {

		private RaidMode m_RaidMode;
		private int m_DisksInArray;
		private int m_DiskSize;

		public RaidMode RaidMode {
			get { return m_RaidMode; }
		}
		public int DisksInArray {
			get { return m_DisksInArray; }
		}
		public int DiskSize {
			get { return m_DiskSize; }
		}

		public RaidController(string inputRaidMode, int inputDisksInArray, int DiskSizeMB) {
			m_RaidMode = new RaidMode(inputRaidMode);
			m_DisksInArray = inputDisksInArray;
			m_DiskSize = DiskSizeMB * 8388608;
		}

		public void GenerateArray(string inputSourceDiskFilename) {
			bool[] SourceDisk;
			bool[][] DiskArray;
			SourceDisk = FileToBoolArray(inputSourceDiskFilename);
			DiskArray = SplitIntoDisks(SourceDisk);
			WriteDisks(DiskArray);
		}

		public void RecombineArray() {
			switch (m_RaidMode.Title) {
				case "RAID0":
					#region RAID0
					break;
					#endregion
			}
		}

		private bool[][] SplitIntoDisks(bool[] inputDisk) {
			bool[][] returnBoolArray = null;
			switch (m_RaidMode.Title) {
				case "RAID0":
					#region RAID0
					int UsedDisks = m_DisksInArray;
					returnBoolArray = new bool[UsedDisks][];
					for (int i = 0; i < returnBoolArray.Length; i++) {
						returnBoolArray[i] = new bool[m_DiskSize];
					}
					int CurrentDisk = 0;
					int CurrentBlock = 0;
					for (int i = 0; i < inputDisk.Length; i++) {
						returnBoolArray[CurrentDisk][CurrentBlock] = inputDisk[i];
						CurrentDisk++;
						if (CurrentDisk == UsedDisks) {
							CurrentDisk = 0;
							CurrentBlock++;
						}
					}
					break;
					#endregion
			}
			return returnBoolArray;
		}

		private void WriteDisks(bool[][] inputDiskArray) {
			for (int i = 0; i < inputDiskArray.Length; i++) {
				BoolArrayToFile(inputDiskArray[i], @"C:\Users\Administrator\Desktop\RAID Generator\Workspace\Disks\Disk" + i + ".dsk");
			}
		}

		public bool[] FileToBoolArray(string inputSourceDiskFilename) {
			Console.WriteLine("Reading {0}...", Path.GetFileName(inputSourceDiskFilename));
			byte[] tempHexDisc = null;
			string tempBinaryStr = String.Empty;
			int discPos = 0;
			FileStream tempFS = new FileStream(inputSourceDiskFilename, FileMode.Open, FileAccess.Read);
			BinaryReader tempBR = new BinaryReader(tempFS);
			long fileLength = new FileInfo(inputSourceDiskFilename).Length;
			tempHexDisc = tempBR.ReadBytes((int)fileLength);
			bool[] returnBoolArr = new bool[tempHexDisc.Length * 8];
			for (int i = 0; i < tempHexDisc.Length; i++) {
				tempBinaryStr = Convert.ToString(tempHexDisc[i], 2).PadLeft(8, '0'); ;
				for (int j = 0; j < 8; j++) {
					if (tempBinaryStr.Substring(j, 1) == "1") {
						returnBoolArr[discPos] = true;
					} else {
						returnBoolArr[discPos] = false;
					}
					discPos++;
				}
			}
			return returnBoolArr;
		}

		private void BoolArrayToFile(bool[] inputBoolArr, string inputOutputFile) {
			Console.WriteLine("Writing {0}...", Path.GetFileName(inputOutputFile));
			FileStream fs = File.Create(inputOutputFile);
			UTF8Encoding utf8 = new UTF8Encoding();
			BinaryWriter bw = new BinaryWriter(fs, utf8);
			string tempBinaryStr = String.Empty;
			for (int i = 0; i < inputBoolArr.Length; i += 8) {
				for (int j = i; j < i + 8; j++) {
					if (inputBoolArr[j]) {
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
