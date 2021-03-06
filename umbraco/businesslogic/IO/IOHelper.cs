﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Web;
using umbraco.BusinessLogic;
using System.Text.RegularExpressions;
using umbraco.businesslogic.Exceptions;

namespace umbraco.IO
{
    public static class IOHelper
    {
        private static string m_rootDir = "";
        // static compiled regex for faster performance
        private readonly static Regex _resolveUrlPattern = new Regex("(=[\"\']?)(\\W?\\~(?:.(?![\"\']?\\s+(?:\\S+)=|[>\"\']))+.)[\"\']?", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

        public static char DirSepChar
        {
            get
            {
                return Path.DirectorySeparatorChar;
            }
        }

        //helper to try and match the old path to a new virtual one
        public static string FindFile(string virtualPath)
        {
            string retval = virtualPath;

            if (virtualPath.StartsWith("~"))
                retval = virtualPath.Replace("~", SystemDirectories.Root);

            if (virtualPath.StartsWith("/") && !virtualPath.StartsWith(SystemDirectories.Root))
                retval = SystemDirectories.Root + "/" + virtualPath.TrimStart('/');

            return retval;
        }

        //Replaces tildes with the root dir
        public static string ResolveUrl(string virtualPath)
        {
            if (virtualPath.StartsWith("~"))
                return virtualPath.Replace("~", SystemDirectories.Root).Replace("//", "/");
            else
                return VirtualPathUtility.ToAbsolute(virtualPath, MultiPlatformHelper.EnsureRootAppPath(SystemDirectories.Root));
        }


        public static string ResolveUrlsFromTextString(string text)
        {
            if (UmbracoSettings.ResolveUrlsFromTextString)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Debug.WriteLine("Start: " + sw.ElapsedMilliseconds);

                // find all relative urls (ie. urls that contain ~)
                MatchCollection tags =
                    _resolveUrlPattern.Matches(text);
                Debug.WriteLine("After regex: " + sw.ElapsedMilliseconds);
                foreach (Match tag in tags)
                {
                    Debug.WriteLine("-- inside regex: " + sw.ElapsedMilliseconds);
                    string url = "";
                    if (tag.Groups[1].Success)
                        url = tag.Groups[1].Value;

                    // The richtext editor inserts a slash in front of the url. That's why we need this little fix
                    //                if (url.StartsWith("/"))
                    //                    text = text.Replace(url, ResolveUrl(url.Substring(1)));
                    //                else
                    if (!String.IsNullOrEmpty(url))
                    {
                        Debug.WriteLine("---- before resolve: " + sw.ElapsedMilliseconds);
                        string resolvedUrl = (url.Substring(0, 1) == "/") ? ResolveUrl(url.Substring(1)) : ResolveUrl(url);
                        Debug.WriteLine("---- after resolve: " + sw.ElapsedMilliseconds);
                        Debug.WriteLine("---- before replace: " + sw.ElapsedMilliseconds);
                        text = text.Replace(url, resolvedUrl);
                        Debug.WriteLine("---- after replace: " + sw.ElapsedMilliseconds);
                    }

                }

                Debug.WriteLine("total: " + sw.ElapsedMilliseconds);
                sw.Stop();
                System.Web.HttpContext.Current.Trace.Write("Resolve Urls", sw.ElapsedMilliseconds.ToString());

            }
            return text;
        }

        public static string MapPathBase(string path, bool useHttpContext)
        {
            // Check if the path is already mapped
            if (path.Length >= 2 && path[1] == Path.VolumeSeparatorChar)
                return path;

            if (useHttpContext)
            {
                //string retval;
                if (!string.IsNullOrEmpty(path) && (path.StartsWith("~") || path.StartsWith(SystemDirectories.Root)))
                    return System.Web.Hosting.HostingEnvironment.MapPath(path);
                else
                    return System.Web.Hosting.HostingEnvironment.MapPath("~/" + path.TrimStart('/'));
            }
            else
            {
                string _root = (!String.IsNullOrEmpty(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath)) ? System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath.TrimEnd(IOHelper.DirSepChar) : getRootDirectorySafe();

                string _path = path.TrimStart('~', '/').Replace('/', IOHelper.DirSepChar);

                string retval = _root + IOHelper.DirSepChar.ToString() + _path;

                return retval;
            }
        }

        public static string MapPath(string path, bool useHttpContext)
        {

			if (IO.MultiPlatformHelper.IsWindows)
				return MapPathBase(path, useHttpContext);            

			return IO.MultiPlatformHelper.MapUnixPath(path, useHttpContext);
        }

        public static string MapPath(string path)
        {

			if (IO.MultiPlatformHelper.IsWindows)
				return MapPathBase(path, true);            

			return IO.MultiPlatformHelper.MapUnixPath(path);
        }

        //use a tilde character instead of the complete path
        public static string returnPath(string settingsKey, string standardPath, bool useTilde)
        {
            string retval = ConfigurationManager.AppSettings[settingsKey];

            if (string.IsNullOrEmpty(retval))
                retval = standardPath;

            return retval.TrimEnd('/');
        }


        public static string returnPath(string settingsKey, string standardPath)
        {
            return returnPath(settingsKey, standardPath, false);

        }


        /// <summary>
        /// Validates if the current filepath matches a directory where the user is allowed to edit a file
        /// </summary>
        /// <param name="filePath">filepath </param>
        /// <param name="validDir"></param>
        /// <returns>true if valid, throws a FileSecurityException if not</returns>
        public static bool ValidateEditPath(string filePath, string validDir)
        {
            if (!filePath.StartsWith(MapPath(SystemDirectories.Root)))
                filePath = MapPath(filePath);
            if (!validDir.StartsWith(MapPath(SystemDirectories.Root)))
                validDir = MapPath(validDir);

            if (!filePath.StartsWith(validDir))
                throw new FileSecurityException(String.Format("The filepath '{0}' is not within an allowed directory for this type of files", filePath.Replace(MapPath(SystemDirectories.Root), "")));

            return true;
        }

        public static bool ValidateFileExtension(string filePath, List<string> validFileExtensions)
        {
            if (!filePath.StartsWith(MapPath(SystemDirectories.Root)))
                filePath = MapPath(filePath);
            FileInfo f = new FileInfo(filePath);


            if (!validFileExtensions.Contains(f.Extension.Substring(1)))
                throw new FileSecurityException(String.Format("The extension for the current file '{0}' is not of an allowed type for this editor. This is typically controlled from either the installed MacroEngines or based on configuration in /config/umbracoSettings.config", filePath.Replace(MapPath(SystemDirectories.Root), "")));

            return true;
        }


        /// <summary>
        /// Returns the path to the root of the application, by getting the path to where the assembly where this
        /// method is included is present, then traversing until it's past the /bin directory. Ie. this makes it work
        /// even if the assembly is in a /bin/debug or /bin/release folder
        /// </summary>
        /// <returns></returns>
        private static string getRootDirectorySafe()
        {
            if (!String.IsNullOrEmpty(m_rootDir))
            {
                return m_rootDir;
            }

            string baseDirectory =
                System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Substring(8));
            m_rootDir = baseDirectory.Substring(0, baseDirectory.LastIndexOf("bin") - 1);

			//changed for tests ck, 9/9/12
			if (MultiPlatformHelper.IsUnix && !m_rootDir.StartsWith(IOHelper.DirSepChar.ToString()))
				m_rootDir = IOHelper.DirSepChar + m_rootDir;

            return m_rootDir;

        }

    }
}
