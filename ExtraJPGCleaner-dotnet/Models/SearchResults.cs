using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraJPGCleaner.Models
{
    public class SearchResult
    {
        public SearchResults Parent { get; set; }

        string path;
        public string Path
        {
            get
            {
                if (Parent.BasePath == null)
                {
                    return FullPath;
                }
                else
                {
                    return GetRelativePath(FullPath, Parent.BasePath);
                }
            }
        }

        public string FullPath
        {
            get
            {
                return System.IO.Path.GetFullPath(path);
            }
        }

        string[] extenstions;
        public string Extensions
        {
            get
            {
                return String.Join(",", extenstions);
            }
        }

        public bool IsFailed { get; set; }
        public string FailReason { get; set; }

        public SearchResult(SearchResults results, string jpgPath, string[] rawExtensions)
        {
            Parent = results;
            path = jpgPath;
            extenstions = rawExtensions;
            IsFailed = false;
            FailReason = null;
        }

        #region Private helpers

        /// <summary>
        /// Get relative path.
        /// </summary>
        /// <see cref="http://stackoverflow.com/questions/703281/getting-path-relative-to-the-current-working-directory"/>
        /// <param name="filespec"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        private string GetRelativePath(string filespec, string folder)
        {
            Uri pathUri = new Uri(filespec);
            // Folders must end in a slash
            if (!folder.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
            {
                folder += System.IO.Path.DirectorySeparatorChar;
            }
            Uri folderUri = new Uri(folder);
            return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', System.IO.Path.DirectorySeparatorChar));
        }

        #endregion
    }


    public class SearchResults : ObservableCollection<SearchResult>
    {
        /// <summary>
        /// The base path which will be used when output the relative path.
        /// </summary>
        public string BasePath { get; set; }

        public void ResetItems(IDictionary<string, string[]> list)
        {
            ClearItems();
            AddItems(list);
        }

        public void ResetItems(IEnumerable<SearchResult> results)
        {
            ClearItems();
            AddItems(results);
        }

        public void AddItems(IDictionary<string, string[]> list)
        {
            foreach (var item in list)
            {
                Add(new SearchResult(this, item.Key, item.Value));
            }
        }

        public void AddItems(IEnumerable<SearchResult> results)
        {
            foreach (var res in results)
            {
                res.Parent = this;
                Add(res);
            }
        }

        public SearchResults() { }
    }
}
