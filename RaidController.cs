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
			m_DiskSize = DiskSizeMB * 1024 * 1024 * 8;
		}

		public void GenerateArray(string SourceDiskFilename) {
			bool[] SourceDisk;
			bool[][] DiskArray;
			SourceDisk = FileToBoolArray(SourceDiskFilename);
			DiskArray = SplitIntoDisks(SourceDisk);
			WriteDisks(DiskArray);
		}

		public void RecombineArray(string OutputFileName, int startPoint, int Length) {
			bool UnrecoverableError = false;
			bool[] trimmedOutputFileBoolArr = new bool[Length * 8];
			bool[] outputFileBoolArr = new bool[m_DisksInArray * m_DiskSize];
			bool[][] DiskArray = new bool[m_DisksInArray][];
			int outputFilePos = 0;
			int i = 0;
			FileInfo[] fileInfoList = new DirectoryInfo(@"C:\Users\Administrator\Desktop\RAID Generator\Workspace\Disks\").GetFiles("Disk*.dsk");
			foreach(FileInfo currentFile in fileInfoList) {
				DiskArray[i] = FileToBoolArray(currentFile.FullName);
				i++;
			}
			switch (m_RaidMode.Title) {
				case "RAID0":
					#region RAID0 - Block-level striping without parity or mirroring.
					for (i = 0; i < m_DiskSize; i++) {
						for (int j = 0; j < m_DisksInArray; j++) {
							outputFileBoolArr[outputFilePos] = DiskArray[j][i];
							outputFilePos++;
						}
					}
					break;
					#endregion
				case "RAID1":
					#region RAID1 - Mirroring without parity or striping.
					for (i = 0; i < m_DiskSize; i++) {
						for (int j = 0; j < m_DisksInArray; j++) {
							if (DiskArray[0][i] != DiskArray[j][i]) {
								UnrecoverableError = true;
								Console.WriteLine("Unrecoverable error at Block[{0}]!", i);
								break;
							}
						}
						if (!UnrecoverableError) {
							outputFileBoolArr[outputFilePos] = DiskArray[0][i];
							outputFilePos++;
						} else {
							break;
						}
					}
					break;
					#endregion
			}
			if (!UnrecoverableError) {
				Array.Copy(outputFileBoolArr, trimmedOutputFileBoolArr, trimmedOutputFileBoolArr.Length);
				BoolArrayToFile(trimmedOutputFileBoolArr, OutputFileName);
			}

		}

		private bool[][] SplitIntoDisks(bool[] inputDisk) {
			bool[][] returnBoolArray = null;
			int UsedDisks = 0;
			int CurrentDisk = 0;
			int CurrentBlock = 0;
			switch (m_RaidMode.Title) {
				case "RAID0":
					#region RAID0 - Block-level striping without parity or mirroring.
					UsedDisks = m_DisksInArray;
					returnBoolArray = new bool[UsedDisks][];
					for (int i = 0; i < returnBoolArray.Length; i++) {
						returnBoolArray[i] = new bool[m_DiskSize];
					}
					for (int i = 0; i < inputDisk.Length; i++) {
						returnBoolArray[CurrentDisk][CurrentBlock] = inputDisk[i];
						//Console.Write("Disk[{0}].Block[{1}] = Source[{2}]", CurrentDisk, CurrentBlock, i); Console.ReadLine();
						CurrentDisk++;
						if (CurrentDisk == UsedDisks) {
							CurrentDisk = 0;
							CurrentBlock++;
						}
					}
					break;
					#endregion
				case "RAID1":
					#region RAID1 - Mirroring without parity or striping.
					UsedDisks = m_DisksInArray;
					returnBoolArray = new bool[UsedDisks][];
					for (int i = 0; i < returnBoolArray.Length; i++) {
						returnBoolArray[i] = new bool[m_DiskSize];
					}
					for (int i = 0; i < inputDisk.Length; i++) {
						for (CurrentDisk = 0; CurrentDisk < UsedDisks; CurrentDisk++) {
							returnBoolArray[CurrentDisk][CurrentBlock] = inputDisk[i];
							//Console.Write("Disk[{0}].Block[{1}] = Source[{2}]", CurrentDisk, CurrentBlock, i); Console.ReadLine();
						}
						CurrentBlock++;
					}
					break;
					#endregion
				case "RAID4":
					#region RAID4 - Block-level striping with dedicated parity.
					UsedDisks = m_DisksInArray;
					bool tempXOR = false;
					returnBoolArray = new bool[UsedDisks][];
					for (int i = 0; i < returnBoolArray.Length; i++) {
						returnBoolArray[i] = new bool[m_DiskSize];
					}
					for (int i = 0; i < inputDisk.Length; i += UsedDisks - 1) {
						for (CurrentDisk = 0; CurrentDisk < UsedDisks - 1; CurrentDisk++) {
							returnBoolArray[CurrentDisk][CurrentBlock] = inputDisk[i + CurrentDisk];
							//Console.WriteLine("Disk[{0}].Block[{1}] = Source[{2}] = {3}", CurrentDisk, CurrentBlock, i + CurrentDisk, inputDisk[i + CurrentDisk]);
							if (CurrentDisk == 0) {
								tempXOR = inputDisk[i];
							} else {
								tempXOR ^= inputDisk[i + CurrentDisk];
							}
						}
						returnBoolArray[CurrentDisk][CurrentBlock] = tempXOR;
						//Console.Write("Disk[{0}].Block[{1}] = XOR = {2}", CurrentDisk, CurrentBlock, tempXOR); Console.ReadLine();
					}
					break;
					#endregion
			}
			return returnBoolArray;
		}

		private void WriteDisks(bool[][] inputDiskArray) {
			for (int i = 0; i < inputDiskArray.Length; i++) {
				BoolArrayToFile(inputDiskArray[i], String.Format(@"C:\Users\Administrator\Desktop\RAID Generator\Workspace\Disks\Disk{0:d2}.dsk", i));
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
