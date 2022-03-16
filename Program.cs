using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Pastel;
using System.Drawing;

// Heavily based on the content of the PTX course

namespace ShellcodeEncryption
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Banner();
            // checks argument count
            if (args.Length == 0)
            {
                Console.WriteLine("[!] Usage: ShellcodeEncryption.exe FILE EncryptionPassword (Optional)");
                return;
            }
            byte[] shellcode = File.ReadAllBytes(args[0]);
            string outputFile = args[0] + "_enc";
            byte[] password = null;
            // if user put in second string for the password use that if not use default for iron injector 
            if (args.Length < 2)
            {
                password = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes("!r0nInj3ct0r123!"));
            }
            else
            {
                password = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(args[1]));
            }

            // writes out messages and encrypted shellcode as a byte array along with saving it to a file
            Console.WriteLine("[<] Shellcode Lenght: {0} Bytes", shellcode.Length);
            byte[] shellcodeEncrypted = AES_Encrypt(shellcode, password);
            Console.WriteLine("[+] Encrypted Shellcode Lenght: {0} Bytes", shellcodeEncrypted.Length);
            File.WriteAllBytes(outputFile, shellcodeEncrypted);
            Console.WriteLine("[>] Encrypted Shellcode Saved in {0}", outputFile);
            PrintShellcode(shellcodeEncrypted);
        }
        // does the AES encryption using the shellcode from a file, make sure if using msfveon or similiar to save it as raw foramt when outputting shellcode to a file
        public static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);
                    AES.Mode = CipherMode.CBC;
                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }
            return encryptedBytes;
        }
        public static void PrintShellcode(byte[] shellcodeBytes)
        {
            StringBuilder shellcode = new StringBuilder();
            shellcode.Append("byte[] shellcode = new byte[");
            shellcode.Append(shellcodeBytes.Length);
            shellcode.Append("] { ");
            for (int i = 0; i < shellcodeBytes.Length; i++)
            {
                shellcode.Append("0x");
                shellcode.AppendFormat("{0:x2}", shellcodeBytes[i]);
                if (i < shellcodeBytes.Length - 1)
                {
                    shellcode.Append(",");
                }
            }
            shellcode.Append(" };");
            Console.WriteLine(shellcode.ToString());
        }
        
        public static void Banner()
        {
            Console.WriteLine(
             "███████╗██╗  ██╗███████╗██╗     ██╗         ███████╗███╗   ██╗ ██████╗██████╗ ██╗   ██╗██████╗ ████████╗\n".Pastel(Color.FromArgb(231, 189, 66)) +
             "██╔════╝██║  ██║██╔════╝██║     ██║         ██╔════╝████╗  ██║██╔════╝██╔══██╗╚██╗ ██╔╝██╔══██╗╚══██╔══╝\n".Pastel(Color.FromArgb(231, 189, 66)) +
             "███████╗███████║█████╗  ██║     ██║         █████╗  ██╔██╗ ██║██║     ██████╔╝ ╚████╔╝ ██████╔╝   ██║   \n".Pastel(Color.FromArgb(231, 189, 66)) +
             "╚════██║██╔══██║██╔══╝  ██║     ██║         ██╔══╝  ██║╚██╗██║██║     ██╔══██╗  ╚██╔╝  ██╔═══╝    ██║   \n".Pastel(Color.FromArgb(231, 189, 66)) +
             "███████║██║  ██║███████╗███████╗███████╗    ███████╗██║ ╚████║╚██████╗██║  ██║   ██║   ██║        ██║   \n".Pastel(Color.FromArgb(231, 189, 66)) +
             "╚══════╝╚═╝  ╚═╝╚══════╝╚══════╝╚══════╝    ╚══════╝╚═╝  ╚═══╝ ╚═════╝╚═╝  ╚═╝   ╚═╝   ╚═╝        ╚═╝   \n".Pastel(Color.FromArgb(231, 189, 66)) +
               "\n" +
              "Author: Jon @QueenCityCyber \n".Pastel(Color.SkyBlue) +
              "https://github.com/Queen-City-Cyber \n".Pastel(Color.FromArgb(52, 152, 52))
               );
        }
    }
}
