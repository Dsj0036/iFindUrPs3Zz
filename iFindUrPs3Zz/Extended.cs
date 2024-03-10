

using System.Collections.Generic;
using System;
using System.Drawing;
using System.Net;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Xml.Linq;
using System.Net.NetworkInformation;
namespace System
{
    internal static class ExtensionsTypes
    {
        public static List<char> Symbols = null;
        public static string FixFileName(this string filename)
        {
            var invalids = System.IO.Path.GetInvalidFileNameChars();
            return String.Join("_", filename.Split(invalids, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
        }
        public static void Clear(this MemoryStream source)
        {
            byte[] buffer = source.GetBuffer();
            Array.Clear(buffer, 0, buffer.Length);
            source.Position = 0;
            source.SetLength(0);
        }
        public static bool IsAccessible(this string address) => new Ping().Send(address,1000).Status == IPStatus.Success;
        public static bool BetweenAnd(this int sender, int biggerthan, int lessthen)
        {
            return sender >= biggerthan & sender < lessthen;
        }
        public static bool IsOnRange(this int sender, int check)
        {
            return (!(sender > check)) & (sender > 0) & (check > 0);
        }
        public static void Y(ref this bool sender) => sender = true;
        public static int Affirm(this int sender)
        {
            return 0 + sender;
        }
        public static int Negate(this int sender)
        {
            return 0 - sender;
        }
        public static string Numbers(this string input)
        {
            var chars = input.ToCharArray().ToList();
            int i = 0;
            foreach (char c in chars)
            {
                if (char.IsNumber(c) is false) chars.RemoveAt(i);
                i++;
            }
            return new string(chars.ToArray());
        }
      
        public static int NullablePerc(this Type sender)
        {
            var props = sender.GetProperties();
            var max = props.Length;
            var nulls = 0;
            foreach (var prop in props)
            {
                if (!prop.CanRead)
                {
                    max--;
                }
                else if (prop.CanRead is true)
                {
                    var val = prop.GetValue(prop);
                    nulls = val is null ? nulls : nulls + 1;
                }
            }
            return (nulls * 100) / max;
        }
        public static string ToAcronymous(this string e)
        {
            if (e == null) return "null";
            string r = "";
            var words = e.Split(' ', '\r');
            foreach (string s in words)
            {
                r += s[0].ToString();
            }
            r.RemoveChar(Symbols.ToArray());
            return r;
        }
        public static int Caps(this string sender)
        {
            var chars = sender.ToCharArray();
            var caps = 0;
            foreach (char c in chars)
            {
                if (char.IsUpper(c)) caps++;
                else continue;
            }
            if (chars.Length == 0) return 0;
            return (caps * 100) / chars.Length;
        }
        public static char[] RemoveAll(this char[] sender, params char[] c)
        {
            var ch = sender.Clone() as char[];
            for (int i = 0; i < ch.Length; i++)
            {
                var chr = ch[i];
                foreach (char rpch in c)
                {
                    if (chr.Equals(rpch))
                    {
                        ch[i] = char.MinValue;
                    }
                }

            }
            return ch;
        }
        public static DateTime Rest(this DateTime sender, TimeSpan e)
        {
            try
            {
                var xd = sender.Day;
                var xm = sender.Month;
                var xy = sender.Year;
                var xh = sender.Hour - e.Hours;
                var xmn = sender.Minute - e.Minutes;
                var xs = sender.Second - e.Seconds;

                return new DateTime(xy, xm, xd, xh + 4, xmn, xs);

            }

            catch
            {
                return sender;
            }
        }
        public static int[] Find(this string s, string check)
        {
            var indexes = Empty<int>();
            var oldIndex = 0;
            var rest = s;
            var sind = rest.IndexOf(check);
            for (int i = 0; i < int.MaxValue; i++)
            {
                if (sind == -1 || rest.Length < check.Length) return indexes;
                else
                {
                    if (sind == oldIndex) sind += 1;
                    if (sind != -1) indexes.Add(sind);
                    rest = rest.Substring(sind);
                    sind = rest.IndexOf(check);
                    Console.WriteLine("i" + sind);
                    oldIndex = sind;
                    if (sind == -1 || rest.Length < check.Length) return indexes;
                }
            }
            return indexes;
        }
        /// <summary>
        /// Evalua todas las lineas de un string y devuelve la linea con la primera aparición de check.
        /// </summary>
        /// <param name="sender">valor de extension</param>
        /// <param name="check">frase a buscar</param>
        /// <returns></returns>
        public static string GetLineContainingValue(this string sender, string check)
        {
            var lns = sender.Split('\n', '\r');
            foreach (string ln in lns)
            {
                if (ln.Contains(check)) return ln;
            }
            return null;
        }
        public static string Merge(this string[] sender, string separator = "")
        {
            var txt = "";
            foreach (string s in sender)
            {
                txt += s + separator;
            }
            return txt;
        }
        public static T[] Empty<T>() where T : struct => new T[0];
        public static void Add(this int[] sender, int x)
        {
            if (sender is null || sender.Length is 0) sender = new int[1] { 0 };
            sender[sender.Length - 1] = x;
        }
        public static void Add<T>(this Array sender, object value)
        {
            if (sender is null || sender.Length is 0) sender = new T[1];
            sender.SetValue((T)value, sender.Length - 1);
        }
        public static void GetSymbols()
        {
            List<Char> symbols = new List<char>();
            for (int i = char.MinValue; i <= char.MaxValue; i++)
            {
                char c = Convert.ToChar(i);
                if (char.IsSymbol(c))
                {
                    symbols.Add(c);
                }
            }
            Symbols = symbols;
            symbols.Clear();
            symbols = null;
        }
        public static void WriteValues(this Array e)
        {
            foreach (object obj in e)
            {
                Console.WriteLine(obj.ToString());
            }
        }
        public static void WriteValues(this object[] e, string format)
        {
            foreach (object obj in e)
            {
                Console.WriteLine(format.Replace("{0}", obj.ToString()));
            }
        }
        public static void WriteValues(this int[] e)
        {
            foreach (object obj in e)
            {
                Console.WriteLine(obj.ToString());
            }
        }
        public static char[] GetUpperCases(this string e)
        {
            List<char> upc = new List<char>();
            var chs = e.ToCharArray();
            foreach (char c in chs)
            {
                if (char.IsUpper(c))
                {
                    upc.Add(c);
                }
            }
            return upc.ToArray();
        }
        public static char[] GetLowerCases(this string e)
        {
            List<char> upc = new List<char>();
            var chs = e.ToCharArray();
            foreach (char c in chs)
            {
                if (char.IsLower(c))
                {
                    upc.Add(c);
                }
            }
            return upc.ToArray();
        }
        public static char[] GetSymbols(this string e)
        {
            List<char> upc = new List<char>();
            var chs = e.ToCharArray();
            foreach (char c in chs)
            {
                if (char.IsSymbol(c))
                {
                    upc.Add(c);
                }
            }
            return upc.ToArray();
        }
        public static string RemoveChar(this string e, params char[] chs)
        {
            var s = e;
            foreach (char c in chs)
            {
                s = e.Replace(c, ' ');
            }
            return s;
        }
        public static int Difference(this int x, int y)
        {
            var x1 = x - y;
            var y1 = y - x;
            return x > y ? x1 : y1;
        }
        public static int Div(this int x) => x / 2;
        public static int Pow(this int x) => x * x;
        public static int Pow(this int x, int ind)
        {
            var y = x;
            for (int i = 0; i < ind; i++)
            {
                y += (y * y);
            }
            return y;
        }
        public static bool IsEven(this int x) => x % 2 == 0;
        public static int ByTwo(this int x) => x * 2;
        public static int Root(this int x) => (int)Math.Sqrt(x);
        public static int Sum(this int x, int y) => x + y;
        public static bool IsAbsolutePath(this string str) => System.IO.Path.IsPathRooted(str);
        public static string GetFileName(this string str) => System.IO.Path.GetFileName(str);
        public static string GetDirectoryName(this string str) => System.IO.Path.GetDirectoryName(str);
        public static string ToNumericString(this int x) => $"{x:n0}".Replace(',', '.');
        public static bool NextBool(this Random x) => x.Next(1) == 1;
        public static bool NextBool(this Random x, int range = 1) => x.Next(range) == range;
        public static string RemoveSymbols(this string str)
        {

            foreach (char c in Symbols)
            {
                str.Remove(c);
            }
            return str;
        }
        public static string GetUrlScheme(this string str) => new Uri(str).Scheme;
        public static string GetUrlHost(this string str) => new Uri(str).Host;
        public static int[] Search(this string origin, string phrase, StringComparison e)
        {
            List<int> indexes = new List<int>();
            var ind = origin.IndexOf(phrase, e);

            if (ind != -1)
            {
                while (ind != -1)
                {
                    var newOrigin = origin.Substring(ind);
                    ind = newOrigin.IndexOf(phrase, e);
                    if (ind != -1)
                    {
                        indexes.Add(ind);
                    }
                }
                return indexes.ToArray();
            }
            else
            {
                return new int[] { -1 };
            }
        }
        public enum YDirection
        {
            Up,
            Down,
            Mid,
        }

        public static bool IsAnFtpExistentDirectory(this string dirPath)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(dirPath);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                return true;
            }
            catch (WebException)
            {
                return false;
            }
        }
        public static string[] GetFtpDirectoriesFromThisStringPath(this string path)
        {
            try
            {
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(path);
                ftpRequest.UseBinary = true;
                ftpRequest.Credentials = new NetworkCredential("", "") { Domain = new Uri(path).Host };
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;

                FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse();
                StreamReader streamReader = new StreamReader(response.GetResponseStream());

                List<string> directories = new List<string>();

                string line = streamReader.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    directories.Add(line);
                    line = streamReader.ReadLine();
                }

                streamReader.Close();
                response.Close();


                return directories.ToArray();
            }
            catch
            {
                return new string[] { "", "" };
            }

        }
        public static string TextsFromThisUrlHtml(this string url)
        {
            var htmlCode = new WebClient().DownloadString(url);
            // Remove new lines since they are not visible in HTML  
            htmlCode = htmlCode.Replace("\n", " ");
            // Remove tab spaces  
            htmlCode = htmlCode.Replace("\t", " ");
            // Remove multiple white spaces from HTML  
            htmlCode = Regex.Replace(htmlCode, "\\s+", " ");
            // Remove HEAD tag  
            htmlCode = Regex.Replace(htmlCode, "<head.*?</head>", ""
                                , RegexOptions.IgnoreCase | RegexOptions.Singleline);
            // Remove any JavaScript  
            htmlCode = Regex.Replace(htmlCode, "<script.*?</script>", ""
              , RegexOptions.IgnoreCase | RegexOptions.Singleline);
            // Replace special characters like &, <, >, " etc.  
            StringBuilder sbHTML = new StringBuilder(htmlCode);
            // Note: There are many more special characters, these are just  
            // most common. You can add new characters in this arrays if needed  
            string[] OldWords = {"&nbsp;", "&amp;", "&quot;", "&lt;",
   "&gt;", "&reg;", "&copy;", "&bull;", "&trade;","&#39;"};
            string[] NewWords = { " ", "&", "\"", "<", ">", "Â®", "Â©", "â€¢", "â„¢", "\'" };
            for (int i = 0; i < OldWords.Length; i++)
            {
                sbHTML.Replace(OldWords[i], NewWords[i]);
            }
            // Check if there are line breaks (<br>) or paragraph (<p>)  
            sbHTML.Replace("<br>", "\n<br>");
            sbHTML.Replace("<br ", "\n<br ");
            sbHTML.Replace("<p ", "\n<p ");
            // Finally, remove all HTML tags and return plain text  
            return System.Text.RegularExpressions.Regex.Replace(
              sbHTML.ToString(), "<[^>]*>", "\r");
        }
        public static bool ThisUrlReachable(this string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 15000;
            request.Method = "HEAD"; // As per Lasse's comment
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch (WebException)
            {
                return false;
            }
        }

        public static string ToMemUnit(this long bytes)
        {
            string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
            int i;
            double dblSByte = bytes;
            for (i = 0; i < Suffix.Length && bytes >= 1024; i++, bytes /= 1024)
            {
                dblSByte = bytes / 1024.0;
            }

            return String.Format("{0:0.##} {1}", dblSByte, Suffix[i]);
        }

    }
}
