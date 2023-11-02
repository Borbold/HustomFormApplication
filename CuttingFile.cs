using System.Diagnostics;

namespace HustonRTEMS {
    internal class CuttingFile {
        public void ReadFileForCut(string pathFile, string cutFolder, int amountBytes) {
            FileStream fileStream = File.OpenRead(pathFile);
            BinaryReader binaryReader = new(fileStream);

            byte[] result = binaryReader.ReadBytes((int)fileStream.Length);

            DirectoryInfo directoryInfo = new(cutFolder);
            foreach(FileInfo file in directoryInfo.GetFiles())
                file.Delete();

            short countFile = (short)(result.Length / amountBytes);
            short numberFile = 1;
            while(countFile >= numberFile) {
                FileStream fileWrite = File.OpenWrite(cutFolder + "\\Test_" + numberFile.ToString());
                BinaryWriter binaryWriter = new(fileWrite);
                int start = (numberFile - 1) * (result.Length / countFile);
                int end = result.Length / (countFile - (numberFile - 1));
                for(int i = start; i < end; i++)
                    binaryWriter.Write(result[i]);
                binaryWriter.Close();
                fileWrite.Close();
                numberFile++;
            }

            FileStream crc = File.OpenWrite(cutFolder + "\\" + Crc16.ComputeCrc(result).ToString());
            crc.Close();

            binaryReader.Close();
            fileStream.Close();
        }
    }
}