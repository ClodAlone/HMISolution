using System.Collections.Generic;

namespace MSZ.Interfaces
{
    public interface ILicenseKeyService
    {
        /// <summary>
        /// Read all licenses in set folder
        /// </summary>
        /// <param name="removeKeys">Keys already removed</param>
        /// <returns></returns>
        List<string> ReadAll(List<string> removeKeys = null);

        /// <summary>
        /// Encrypt and write code in set folder
        /// </summary>
        /// <param name="code">Encrypted code to write</param>
        void Write(string code);

        /// <summary>
        /// Clean all removed keys stored
        /// </summary>
        void CleanAll();

        /// <summary>
        /// Remove key from set folder
        /// </summary>
        /// <param name="value">Encrypted code to remove</param>
        /// <returns></returns>
        string Remove(string value);
    }
}