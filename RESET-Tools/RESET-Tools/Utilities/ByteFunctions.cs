﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RESET_Tools.Utilities
{
    public static class ByteFunctions
    {
        public static byte[] GetBytes(string stringValue)
        {
            //create byte array of the length of the string
            byte[] stringBytes = new byte[stringValue.Length];

            //for the length of the string, get each byte value
            for (int i = 0; i < stringBytes.Length; i++)
            {
                stringBytes[i] = Convert.ToByte(stringValue[i]);
            }

            //return it
            return stringBytes;
        }

        public static uint Convert_String_To_UInt32(string sValue)
        {
            //convert the bytes into a value
            uint parsedValue = BitConverter.ToUInt32(GetBytes(sValue), 0);

            //return it
            return parsedValue;
        }

        public static int Convert_String_To_Int32(string sValue)
        {
            //create byte array of the length of the string
            byte[] stringBytes = new byte[sValue.Length];

            //for the length of the string, get each byte value
            for (int i = 0; i < stringBytes.Length; i++)
            {
                stringBytes[i] = Convert.ToByte(sValue[i]);
            }

            //convert the bytes into a value
            int parsedValue = BitConverter.ToInt32(stringBytes, 0);

            //return it
            return parsedValue;
        }

        public static byte[] ModifyBytes(byte[] source, byte[] newBytes, uint indexOffset)
        {
            if (source.Length >= indexOffset)
                return source;

            //run a loop and begin going through for the lenght of the bytes
            for (int i = 0; i < newBytes.Length; i++)
            {
                //assign the value from the source byte array with the offset
                source[indexOffset + i] = newBytes[i];
            }

            //return the final byte array
            return source;
        }

        public static byte[] ModifyBytes(byte[] source, int newValue, uint indexOffset)
        {
            return ModifyBytes(source, BitConverter.GetBytes(newValue), indexOffset);
        }

        public static byte[] ModifyBytes(byte[] source, uint newValue, uint indexOffset)
        {
            return ModifyBytes(source, BitConverter.GetBytes(newValue), indexOffset);
        }

        public static byte[] ModifyBytes(byte[] source, float newValue, uint indexOffset)
        {
            return ModifyBytes(source, BitConverter.GetBytes(newValue), indexOffset);
        }

        public static byte[] ModifyBytes(byte[] source, bool newValue, uint indexOffset)
        {
            return ModifyBytes(source, BitConverter.GetBytes(newValue), indexOffset);
        }

        public static byte[] ModifyBytes(byte[] source, short newValue, uint indexOffset)
        {
            return ModifyBytes(source, BitConverter.GetBytes(newValue), indexOffset);
        }

        public static byte[] ModifyBytes(byte[] source, ushort newValue, uint indexOffset)
        {
            return ModifyBytes(source, BitConverter.GetBytes(newValue), indexOffset);
        }

        public static byte[] ModifyBytes(byte[] source, string newValue, uint indexOffset)
        {
            return ModifyBytes(source, GetBytes(newValue), indexOffset);
        }

        /// <summary>
        /// Combines two byte arrays into one.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static byte[] Combine(byte[] first, byte[] second)
        {
            //allocate a byte array with both total lengths combined to accomodate both
            byte[] bytes = new byte[first.Length + second.Length];

            //copy the data from the first array into the new array
            Buffer.BlockCopy(first, 0, bytes, 0, first.Length);

            //copy the data from the second array into the new array (offset by the total length of the first array)
            Buffer.BlockCopy(second, 0, bytes, first.Length, second.Length);

            //return the final byte array
            return bytes;
        }

        /// <summary>
        /// Allocates a byte array of fixed length. 
        /// <para>Depending on 'size' it allocates 'size' amount of bytes from 'sourceByteArray' offset by 'offsetLocation'</para>
        /// </summary>
        /// <param name="size"></param>
        /// <param name="sourceByteArray"></param>
        /// <param name="offsetLocation"></param>
        /// <returns></returns>
        public static byte AllocateByte(byte[] sourceByteArray, uint offsetLocation)
        {
            return sourceByteArray[offsetLocation];
        }

        /// <summary>
        /// Allocates a byte array of fixed length. 
        /// <para>Depending on 'size' it allocates 'size' amount of bytes from 'sourceByteArray' offset by 'offsetLocation'</para>
        /// </summary>
        /// <param name="size"></param>
        /// <param name="sourceByteArray"></param>
        /// <param name="offsetLocation"></param>
        /// <returns></returns>
        public static byte[] AllocateBytes(int size, byte[] sourceByteArray, uint offsetLocation)
        {
            //allocate byte array of fixed length
            byte[] byteArray = new byte[size];

            if (offsetLocation + size > sourceByteArray.Length)
                return byteArray;

            //run a loop and begin gathering values
            for (int i = 0; i < size; i++)
            {
                //assign the value from the source byte array with the offset
                byteArray[i] = sourceByteArray[offsetLocation + i];
            }

            //return the final byte array
            return byteArray;
        }

        public static byte[] AllocateBytes(byte[] bytes, byte[] destinationBytes, uint offset)
        {
            //for the length of the byte array, assign each byte value to the destination byte array values with the given offset
            for (int i = 0; i < bytes.Length; i++)
            {
                destinationBytes[offset + i] = bytes[i];
            }

            //return the result
            return destinationBytes;
        }

        public static byte[] AllocateBytes(string stringValue, byte[] destinationBytes, uint offset)
        {
            //for the length of the byte array, assign each byte value to the destination byte array values with the given offset
            for (int i = 0; i < stringValue.Length; i++)
            {
                destinationBytes[offset + i] = Convert.ToByte(stringValue[i]);
            }

            //return the result
            return destinationBytes;
        }

        public static string ReadWideString(BinaryReader reader)
        {
            string value = "";
            int stringLength = reader.ReadInt32();

            for (int i = 0; i < stringLength; i++)
            {
                short twoByte = reader.ReadInt16();
                value += Char.ConvertFromUtf32((int)twoByte);
            }

            return value;
        }

        public static string ReadString(BinaryReader reader)
        {
            int stringLength = reader.ReadInt32();
            string value = "";

            for(int i = 0; i < stringLength; i++)
            {
                value += reader.ReadChar();
            }

            return value;
        }

        public static void WriteString(BinaryWriter writer, string value)
        {
            writer.Write(value.Length);
            
            for(int i = 0; i < value.Length; i++)
            {
                writer.Write(value[i]);
            }
        }

        public static string WriteWideString(BinaryWriter writer, string value)
        {
            writer.Write(value.Length);

            //byte[] encodedString = Encoding.UTF32.GetBytes(value);
            //writer.Write(encodedString);

            for(int i = 0; i < value.Length; i++)
            {
                writer.Write(value[i]);
                writer.Write(byte.MinValue);
            }

            return value;
        }

        /// <summary>
        /// Checks if we have reached the end of the file.
        /// </summary>
        /// <param name="bytePointerPosition"></param>
        /// <param name="fileSize"></param>
        public static void ReachedEndOfFile(uint bytePointerPosition, uint fileSize)
        {
            if (bytePointerPosition != fileSize)
            {
                ConsoleFunctions.SetConsoleColor(ConsoleColor.Black, ConsoleColor.Red);
                Console.WriteLine("(End Of File Check) Didn't reach the end of the file!");
                Console.WriteLine("(End Of File Check) Left off at = {0}", bytePointerPosition);
                Console.WriteLine("(End Of File Check) File Size = {0}", fileSize);
            }
            else
            {
                ConsoleFunctions.SetConsoleColor(ConsoleColor.Black, ConsoleColor.Green);
                Console.WriteLine("(End Of File Check) Reached end of file!");
                Console.WriteLine("(End Of File Check) Left off at = {0}", bytePointerPosition);
                Console.WriteLine("(End Of File Check) File Size = {0}", fileSize);
            }

            ConsoleFunctions.SetConsoleColor(ConsoleColor.Black, ConsoleColor.White);
        }

        /// <summary>
        /// Checks if we have reached a specific offset in the file.
        /// </summary>
        /// <param name="bytePointerPosition"></param>
        /// <param name="offsetPoint"></param>
        public static void ReachedOffset(uint bytePointerPosition, uint offsetPoint)
        {
            if (bytePointerPosition != offsetPoint)
            {
                ConsoleFunctions.SetConsoleColor(ConsoleColor.Black, ConsoleColor.Red);
                Console.WriteLine("(Offset Check) Didn't reach the offset!");
                Console.WriteLine("(Offset Check) Left off at = {0}", bytePointerPosition);
                Console.WriteLine("(Offset Check) Offset = {0}", offsetPoint);
            }
            else
            {
                ConsoleFunctions.SetConsoleColor(ConsoleColor.Black, ConsoleColor.Green);
                Console.WriteLine("(Offset Check) Reached the offset!");
                Console.WriteLine("(Offset Check) Left off at = {0}", bytePointerPosition);
                Console.WriteLine("(Offset Check) Offset = {0}", offsetPoint);
            }

            ConsoleFunctions.SetConsoleColor(ConsoleColor.Black, ConsoleColor.White);
        }
    }
}
