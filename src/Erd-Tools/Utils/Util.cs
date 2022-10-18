using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace Erd_Tools.Utils
{
    internal static class Util
    {

        public static readonly string ExeDir = Environment.CurrentDirectory;

        public static int DeleteFromEnd(int num, int n)
        {
            for (int i = 1; num != 0; i++)
            {
                num = num / 10;

                if (i == n)
                    return num;
            }

            return 0;
        }

        public static string GetEmbededResource(string item)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            string resourceName = $"Erd_Tools.{item}";

            using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new NullReferenceException($"Could not find embedded resource: {item} in the {Assembly.GetCallingAssembly().GetName()} assembly");

                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            
        }


        public static string GetTxtResource(string filePath)
        {
            //Get local directory + file path, read file, return string contents of file

            //Path.Combine(Environment.CurrentDirectory, filePath);
            if (!File.Exists($@"{ExeDir}/{filePath}"))
                return "";

            string fileString = File.ReadAllText($@"{ExeDir}/{filePath}");

            return fileString;
        }

        public static string[] GetListResource(string filePath)
        {
            //Get local directory + file path, read file, return string contents of file

            //Path.Combine(Environment.CurrentDirectory, filePath);
            if (!File.Exists($@"{ExeDir}/{filePath}"))
                return new string[] { };

            string[] stringArray = File.ReadAllLines($@"{ExeDir}/{filePath}");

            return stringArray;
        }

        public static bool IsValidTxtResource(string txtLine)
        {
            //see if txt resource line is valid and should be accepted 
            //(bare bones, only checks for a couple obvious things)

            if (txtLine.Contains("//"))
            {
                txtLine = txtLine.Substring(0, txtLine.IndexOf("//")); // remove everything after "//" comments
            };

            if (string.IsNullOrWhiteSpace(txtLine) == true || txtLine.Contains('#')) //empty line check
            {
                return false; //resource line invalid
            };

            return true; //resource line valid
        }

        /// <summary>
        /// Removes everything after // in a string and returns the new string with .Trim()
        /// </summary>
        /// <param name="txtLine"></param>
        /// <returns>txtLine.Trim() with everything after // removed</returns>
        public static string TrimComment(this string txtLine)
        {
            //Repurposing Kingborehahas code for checking valid resource to trim hashes
            if (txtLine.Contains("//"))
            {
                txtLine = txtLine.Substring(0, txtLine.IndexOf("//")); // remove everything after "//" comments
            };

            return txtLine.Trim();
        }


        public static string[] RegexSplit(string source, string pattern)
        {
            return Regex.Split(source, pattern);
        }

        public static T? DeserializeXml<T>(string filePath)
        {
            TextReader textReader = new StreamReader(@$"{ExeDir}/{filePath}");
            XmlSerializer serializer = new(typeof(T));
            return (T?)serializer.Deserialize(textReader);
        }

        public static void SerializeXml<T>(T obj,string filePath)
        {
            XmlSerializer ser = new(typeof(T));
            XmlWriterSettings settings = new() { Indent = true };
            TextWriter writer = new StreamWriter(filePath);

            using (XmlWriter xw = XmlWriter.Create(writer, settings))
            {
                ser.Serialize(xw, obj);
            }
        }
    }
}
