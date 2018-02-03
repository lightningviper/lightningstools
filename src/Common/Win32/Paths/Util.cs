using System;
using System.Text;

namespace Common.Win32.Paths
{
    public static class Util
    {
        /// Canonicalizes a path. This function allows the user 
        /// to specify what to remove from a path by inserting 
        /// special character sequences into the path. The ".." 
        /// sequence indicates to remove the path part from the 
        /// current position to the previous path part. The "." 
        /// sequence indicates to skip over the next path part 
        /// to the following path part. The root part of the path 
        /// cannot be removed.
        public static string Canonicalize(string path)
        {
            if (path.Length > NativeMethods.MAX_PATH)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(path), path, "path length must be less than or equal toNativeMethods.MAX_PATH"
                );
            }

            var buffer = new StringBuilder(NativeMethods.MAX_PATH, NativeMethods.MAX_PATH);
            return NativeMethods.PathCanonicalize(buffer, path) ? buffer.ToString() : null;
        }

        /// Truncates a path to fit within a certain number of 
        /// characters by replacing path components with ellipses.
        public static string Compact(string path, int maxlen)
        {
            if (path.Length > NativeMethods.MAX_PATH)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(path), path, "path length must be less than or equal toNativeMethods.MAX_PATH"
                );
            }

            if (maxlen > NativeMethods.MAX_PATH)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxlen), maxlen, "maxlen must be less than or equal toNativeMethods.MAX_PATH"
                );
            }

            var buffer = new StringBuilder(NativeMethods.MAX_PATH, NativeMethods.MAX_PATH);
            var success = NativeMethods.PathCompactPathEx(buffer, path, maxlen, 0);
            return success ? buffer.ToString() : null;
        }

        /// Determines if a given file name has one of a list of 
        /// suffixes. This function does a case-sensitive comparison. 
        /// The suffix must match exactly.
        public static bool ContainsExtension(string path, string[] extensions)
        {
            if (path.Length > NativeMethods.MAX_PATH)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(path), path, "path length must be less than or equal toNativeMethods.MAX_PATH"
                );
            }

            var ext = NativeMethods.PathFindSuffixArray(path, extensions, extensions.Length);
            return ext != null;
        }

        public static string GetShortPathName(string path)
        {
            var sbShortPath = new StringBuilder(NativeMethods.MAX_PATH);
            NativeMethods.GetShortPathName(path, sbShortPath, NativeMethods.MAX_PATH);
            return sbShortPath.ToString();
        }

        /// Determines if a file's registered content type matches 
        /// the specified content type. This function obtains the 
        /// content type for the specified file type and compares 
        /// that string with the pszContentType. The comparison is 
        /// not case sensitive.
        public static bool IsContentType(string path, string contenttype)
        {
            if (path.Length > NativeMethods.MAX_PATH)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(path), path, "path length must be less than or equal toNativeMethods.MAX_PATH"
                );
            }

            return NativeMethods.PathIsContentType(path, contenttype);
        }

        /// Converts a path to all lowercase characters to give 
        /// the path a consistent appearance. This function only 
        /// operates on paths that are entirely uppercase. For 
        /// example: C:\WINDOWS will be converted to c:\windows, 
        /// but c:\Windows will not be changed.
        public static string MakePretty(string path)
        {
            if (path.Length > NativeMethods.MAX_PATH)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(path), path, "path length must be less than or equal toNativeMethods.MAX_PATH"
                );
            }

            var buffer = new StringBuilder(NativeMethods.MAX_PATH, NativeMethods.MAX_PATH);
            buffer.Append(path);
            return NativeMethods.PathMakePretty(buffer) ? buffer.ToString() : null;
        }
    }
}