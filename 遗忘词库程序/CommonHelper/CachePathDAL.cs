using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
namespace CommonHelper
{
    public class CachePathDAL
    {
        public static string GetCachePathFile(string root,string cacheName, string fileName)
        {
            string sCachePath = Path.GetFullPath(String.Format(@"{0}\{1}", GetWorkSpacePath(root), cacheName));
            if (!Directory.Exists(sCachePath)) Directory.CreateDirectory(sCachePath);
            return Path.GetFullPath(String.Format(@"{0}\{1}\{2}",GetWorkSpacePath(root),cacheName, fileName));
        }
        public static string GetCachePathFile(string cacheName, string fileName)
        {
            return Path.GetFullPath(String.Format(@"{0}\{1}", GetSubWorkSpacePath(cacheName), fileName));
        }
        public static string GetCachePathFile(string filename)
        {
            return Path.GetFullPath(String.Format(@"{0}\{1}", GetCacheSpacePath(), filename));
        }

        public static string GetCacheSpacePath()
        {
            return GetSubWorkSpacePath("Cache");
        }
        public static string GetSubWorkSpacePath(string root,string sSubFloder)
        {
            string sCachePath = Path.GetFullPath(String.Format(@"{0}\{1}", GetWorkSpacePath(root), sSubFloder));
            if (!Directory.Exists(sCachePath)) Directory.CreateDirectory(sCachePath);
            return sCachePath;
        }
        public static string GetSubWorkSpacePath(string sSubFloder)
        {
            string sCachePath = Path.GetFullPath(String.Format(@"{0}\{1}", GetWorkSpacePath(), sSubFloder));
            if (!Directory.Exists(sCachePath)) Directory.CreateDirectory(sCachePath);
            return sCachePath;
        }

        /// <summary>
        /// 获得默认工作目录
        /// </summary>
        /// <returns></returns>
        public static string GetWorkSpacePath()
        {
            return GetWorkSpacePath(String.Empty);
        }
        /// <summary>
        /// 获得工作目录
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public static string GetWorkSpacePath(string root)
        {
            if (!String.IsNullOrEmpty(root))
            {
                string sRootPath = Path.GetFullPath(root);
                if (Directory.Exists(sRootPath))
                {
                    return sRootPath;
                }
                else
                {
                    try
                    {
                        Directory.CreateDirectory(sRootPath);
                        return sRootPath;
                    }
                    catch
                    {
                    }
                }
            }
            string sExecPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //string sExecPath = System.Web.HttpContext.Current.Request.MapPath(HttpContext.Current.Request.ApplicationPath);
            //string sExecPath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath;
            string sDictPath = Path.GetFullPath(String.Format(@"{0}\{1}", sExecPath, "WorkSpace"));
            if (!Directory.Exists(sDictPath)) Directory.CreateDirectory(sDictPath);
            return sDictPath;
        }
    }
}
