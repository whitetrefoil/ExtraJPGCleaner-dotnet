using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ExtraJPGCleaner.Models
{
    public class SearchResult
    {
        public SearchResultCollection Parent { get; set; }

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
                    return GetRelativePath();
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

        public bool IsDeleted { get; set; }
        public bool IsFailed { get; set; }
        public string FailReason { get; set; }

        public string DisplayText
        {
            get
            {
                if (IsFailed)
                {
                    return "(" + FailReason + ") " + Path;
                }
                else
                {
                    return Path;
                }
            }
        }

        public SearchResult(SearchResultCollection results, string jpgPath, string[] rawExtensions)
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
        /// <returns></returns>
        private string GetRelativePath()
        {
            Uri pathUri = new Uri(FullPath);
            // Folders must end in a slash
            if (!Parent.BasePath.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
            {
                Parent.BasePath += System.IO.Path.DirectorySeparatorChar;
            }
            Uri folderUri = new Uri(Parent.BasePath);
            return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', System.IO.Path.DirectorySeparatorChar));
        }

        #endregion
    }


    public class SearchResultCollection : ObservableCollection<SearchResult>
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

        public void CleanupItems()
        {
            var newList = new List<SearchResult>();
            foreach (var item in this)
            {
                if (!item.IsDeleted)
                {
                    newList.Add(item);
                }
            }
            ResetItems(newList);
        }

        public SearchResultCollection() { }
    }
}
