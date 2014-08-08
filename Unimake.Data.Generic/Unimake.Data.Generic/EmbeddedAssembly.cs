using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Security.Cryptography;

/* 
 * fonte: http://www.codeproject.com/Articles/528178/Load-DLL-From-Embedded-Resource
 * Revisado por: Marcelo
 */


namespace Unimake.Data.Generic
{
    internal class EmbeddedAssembly
    {
        // Version 1.3

        /// <summary>
        /// Load Assembly, DLL from Embedded Resources into memory.
        /// </summary>
        /// <param name="embeddedResource">Embedded Resource string. Example: WindowsFormsApplication1.SomeTools.dll</param>
        /// <param name="fileName">File Name. Example: SomeTools.dll</param>
        public static void Load(string embeddedResource, string fileName)
        {
            byte[] ba = null;
            Assembly curAsm = Assembly.GetExecutingAssembly();

            using(Stream stm = curAsm.GetManifestResourceStream(embeddedResource))
            {
                // Either the file is not existed or it is not mark as embedded resource
                if(stm == null)
                    throw new Exception(embeddedResource + " is not found in Embedded Resources.");

                // Get byte[] from the file from embedded resource
                ba = new byte[(int)stm.Length];
                stm.Read(ba, 0, (int)stm.Length);
            }

            bool fileOk = false;
            string tempFile = "";

            using(SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
            {
                // Get the hash value from embedded DLL/assembly
                string fileHash = BitConverter.ToString(sha1.ComputeHash(ba)).Replace("-", string.Empty);

                // Define the temporary storage location of the DLL/assembly
                tempFile = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + fileName;

                // Determines whether the DLL/assembly is existed or not
                if(File.Exists(tempFile))
                {
                    // Get the hash value of the existed file
                    byte[] bb = File.ReadAllBytes(tempFile);
                    string fileHash2 = BitConverter.ToString(sha1.ComputeHash(bb)).Replace("-", string.Empty);

                    // Compare the existed DLL/assembly with the Embedded DLL/assembly
                    if(fileHash == fileHash2)
                    {
                        // Same file
                        fileOk = true;
                    }
                    else
                    {
                        // Not same
                        fileOk = false;
                    }
                }
                else
                {
                    // The DLL/assembly is not existed yet
                    fileOk = false;
                }
            }

            // Create the file on disk
            if(!fileOk)
            {
                FileInfo fi = new FileInfo(tempFile);
                fi.Directory.Create();
                fi = null;
                try
                {
                    System.IO.File.WriteAllBytes(tempFile, ba);
                }
                catch(IOException)
                { 
                    //nada
                }
            }
        }
    }
}
