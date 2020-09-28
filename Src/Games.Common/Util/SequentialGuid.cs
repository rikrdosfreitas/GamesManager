using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Games.Common.Util
{
    public static class SequentialGuid
    {
        private static readonly object sync = new object();
        private static readonly bool Enabled;

        private static readonly long Epoc;
        private static readonly uint Node;

        private static uint Counter;


        static SequentialGuid()
        {
            Enabled = InitializeHighPrecisionDateTime();
            Epoc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
            Node = GenerateNodeValue();
        }

        public static Guid NewGuid()
        {
            return NewGuid(GuidUseType.SqlServer);
        }

        public static string NewString()
        {
            return NewGuid(GuidUseType.String).ToString();
        }

        public static Guid NewBinary()
        {
            return NewGuid(GuidUseType.Binary);
        }


        #region Helper Methods
        private static Guid NewGuid(GuidUseType guidType)
        {
            lock (sync)
            {
                unchecked
                {
                    Counter++;
                }
            }

            var msb = GetMSB(); // 8...2,1,0
            var lsb = GetLSB(); // 8...2.1.0

            var sqlGuid = BuildSqlGuid(msb, lsb, guidType);

            return new Guid(sqlGuid);
        }

        private static byte[] BuildSqlGuid(byte[] msb, byte[] lsb, GuidUseType guidType)
        {
            // Build SQL binary Guid
            var guid = new byte[16];

            // MMLLLLLL.LLMMMMMM
            switch (guidType)
            {
                case GuidUseType.Binary:
                case GuidUseType.String:
                    Buffer.BlockCopy(msb, 0, guid, 0, 8);
                    Buffer.BlockCopy(lsb, 0, guid, 8, 8);

                    // The Guid is stored as Int32.Int16.Int16.Byte[8]
                    if (guidType == GuidUseType.String && BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(guid, 0, 4);
                        Array.Reverse(guid, 4, 2);
                    }

                    break;

                case GuidUseType.SqlServer:
                    Buffer.BlockCopy(msb, 6, guid, 0, 2);
                    Buffer.BlockCopy(lsb, 0, guid, 2, 8);
                    Buffer.BlockCopy(msb, 0, guid, 10, 6);
                    break;
            }

            return guid;
        }

        private static byte[] GetLSB()
        {
            var lsb = new byte[8];
            var node = BitConverter.GetBytes(Node);     // lsb...msb
            AjustEndianness(node);                      // msb...lsb

            var count = BitConverter.GetBytes(Counter); // lsb...msb
            AjustEndianness(count);                     // msb...lsb

            // Build LSB 
            Buffer.BlockCopy(node, 0, lsb, 0, 4);       // MMMM.0000
            Buffer.BlockCopy(count, 0, lsb, 4, 4);      // MMMM.LLLL

            return lsb;
        }

        private static byte[] GetMSB()
        {
            var resolution = GetHighPrecisionUtcNow().Ticks - Epoc;
            var msb = BitConverter.GetBytes(resolution);    // lsb...msb

            // Transform endianness from machine architecture to binary order (MSB-LSB)
            AjustEndianness(msb);                           // MMMM.LLLL

            return msb;
        }

        private static void AjustEndianness(byte[] array)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(array);
            }
        }

        private static DateTime GetHighPrecisionUtcNow()
        {
            if (!Enabled)
            {
                return DateTime.UtcNow;
            }

            GetSystemTimePreciseAsFileTime(out long filetime);

            return DateTime.FromFileTimeUtc(filetime);
        }

        private static UInt32 GenerateNodeValue()
        {
            var rngCsp = new RNGCryptoServiceProvider();
            var node = new byte[sizeof(UInt32)];
            rngCsp.GetNonZeroBytes(node);

            return BitConverter.ToUInt32(node, 0);
        }

        private static bool InitializeHighPrecisionDateTime()
        {
            try
            {
                GetSystemTimePreciseAsFileTime(out long filetime);
                return true;
            }
            catch (EntryPointNotFoundException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi)]
        private static extern void GetSystemTimePreciseAsFileTime(out long filetime);

        private enum GuidUseType
        {
            Binary = 0,
            SqlServer = 1,
            String = 2
        }
        #endregion
    }
}
