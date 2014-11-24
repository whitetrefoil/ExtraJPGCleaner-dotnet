using System;
using System.Collections.Generic;
using System.IO;

namespace ExtraJPGCleaner.Helpers
{
    public class FileOperationResult
    {
        public bool IsSuccessful { get; private set; }
        public Exception RawException { get; private set; }
        public FileOperationResult(bool isSuccessful, Exception rawException)
        {
            IsSuccessful = isSuccessful;
            RawException = rawException;
        }
    }

    static class FileOperator
    {
        public static FileOperationResult Delete(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch(DirectoryNotFoundException ex)
            {
                return new FileOperationResult(true, ex);
            }
            catch(Exception ex)
            {
                if (ex is PathTooLongException || ex is UnauthorizedAccessException || ex is IOException)
                {
                    return new FileOperationResult(false, ex);
                }
                else
                {
                    throw;
                }
            }
            return new FileOperationResult(true, null);
        }

        /// <summary>
        /// Find for any JPG that has corresponding RAW.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="extensions">Search for these RAW extensions.</param>
        /// <returns>A Dict contains the JPGs and theirs RAW extensions.</returns>
        public static Dictionary<string, string[]> Search(string location, string extensions)
        {
            return Search(location, ConvertStringToArray(extensions));
        }

        /// <summary>
        /// Find for any JPG that has corresponding RAW.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="extensions">Search for these RAW extensions.</param>
        /// <returns>A Dict contains the JPGs and theirs RAW extensions.</returns>
        public static Dictionary<string, string[]> Search(string location, string[] extensions)
        {
            var jpgFilesHaveRaw = new Dictionary<string, string[]>();
            var jpgFiles = Directory.GetFiles(location, "*.jpg", SearchOption.AllDirectories);

            foreach (string jpg in jpgFiles)
            {
                var raw = CheckExistingRaw(jpg, extensions);
                if (raw.Length > 0)
                {
                    jpgFilesHaveRaw.Add(jpg, raw);
                }
            }

            return jpgFilesHaveRaw;
        }

        /// <summary>
        /// Convert strings like `"a,b,c"` to `["a","b","c"]`
        /// </summary>
        /// <param name="extensions">The string to convert.</param>
        /// <returns></returns>
        private static string[] ConvertStringToArray(string extensions)
        {
            var splitted = extensions.Split(new Char[] { ',', '.' }, StringSplitOptions.RemoveEmptyEntries);
            string[] cleaned = new string[splitted.Length];

            for (int _i = 0; _i < splitted.Length; _i++)
            {
                cleaned[_i] = splitted[_i].Trim();
            }

            return cleaned;
        }

        /// <summary>
        /// Check if the relative RAW files exist.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="extensions"></param>
        /// <returns>An array contains extensions of all existing RAW files.</returns>
        private static string[] CheckExistingRaw(string path, string[] extensions)
        {
            List<string> existingRaws = new List<string>();
            var pathWithoutExtension = Path.ChangeExtension(path, null);

            foreach (string ext in extensions)
            {
                if (File.Exists(pathWithoutExtension + "." + ext))
                {
                    existingRaws.Add(ext);
                }
            }

            return existingRaws.ToArray();
        }
    }
}
