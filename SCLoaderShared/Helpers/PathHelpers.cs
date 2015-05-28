using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SCLoaderShared.Helpers
{
    public class PathHelpers
    {

        /// <summary>
        /// Combines path parts with respect to the current platform (Win/Linux etc.)
        /// Fixes mixed slashes and back-slashes
        /// Removes illegal characters from filename
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static string GetWorkingCombinedPath(params string[] paths)
        {

            var length = paths.Length;

            string[] fixedPaths = new string[length];

            for (var i = 0; i < length; i++)
            {
                if (paths[i].Contains("."))
                {
                    // Filename -> no trailing slash
                    fixedPaths[i] = FixPathSeparators(CleanIllegalFilenameCharacters(paths[i]), false);
                }
                else
                {
                    // Directory -> add trailing slash
                    fixedPaths[i] = FixPathSeparators(paths[i], true);
                }

            }

            return Path.Combine(fixedPaths);

        }

        /// <summary>
        /// Fixes mixed usage of path separator to the platform-specific form
        /// </summary>
        /// <param name="path"></param>
        /// <param name="trailingSlash"></param>
        /// <returns></returns>
        public static string FixPathSeparators(string path, bool trailingSlash)
        {

            path = path.Trim();

            if (path.Length > 0)
            {
                path = path.Replace('\\', Path.DirectorySeparatorChar);
                path = path.Replace('/', Path.DirectorySeparatorChar);
                path = path.TrimEnd(Path.DirectorySeparatorChar);

                if (trailingSlash)
                {
                    path += Path.DirectorySeparatorChar;
                }

            }


            return path;

        }


        /// <summary>
        /// Removes illegal characters from filename
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string CleanIllegalFilenameCharacters(string fileName)
        {

            string regexSearch = new string(Path.GetInvalidFileNameChars());

            Regex regex = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));

            return regex.Replace(fileName, "");

        }

    }
}
