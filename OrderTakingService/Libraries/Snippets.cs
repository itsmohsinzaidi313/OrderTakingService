using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace OrderTakingService.Lib
{
    public static class Snippets
    {
        public static bool AuthariseUser(System.Net.Http.HttpRequestMessage response)
        {
            string param = response.Headers.Authorization.Parameter;
            var bytes = System.Convert.FromBase64String(param);
            string[] _values = new string[2];
            if (bytes.Length >= 1)
            {
                _values = Encoding.UTF8.GetString(bytes).Split(':');
            }

            System.Data.DataTable riders = Database.GetData($"select userId from loginUser where username = '{_values[0]}' and userPassword = '{_values[1]}'");
            if (riders.Rows.Count >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool Authenticate(string hash)
        {
            if (string.IsNullOrEmpty(hash)) return false;

            string keyHash = string.Empty;
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                keyHash = string.Join("", md5.ComputeHash(Encoding.UTF8.GetBytes(Database.SecKey)).Select(x => x.ToString("x2")));
            }
            return hash == keyHash;
        }
        public static string GenerateCode(string prefix, string digit, int digitsCount)
        {
            if (digit.Length < digitsCount)
            {

                string nullDigits = string.Empty;
                int nullDigitCount = digitsCount - digit.Length;
                for (int i = 0; i < nullDigitCount; i++)
                {
                    nullDigits += "0";
                }
                return $"{prefix}-{nullDigits}{digit}";
            }
            else
            {
                throw new Exception("Digit length should be less than digit count.");
            }
        }

        public static string SerializeToStringXml(object obj, Type type)
        {
            XmlAttributeOverrides overrides = new XmlAttributeOverrides();
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            XmlSerializer x = new XmlSerializer(type);
            MemoryStream memoryStream = new MemoryStream();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
            x.Serialize(xmlTextWriter, obj, ns);
            xmlTextWriter.Close();
            string xml = Encoding.UTF8.GetString(memoryStream.GetBuffer());
            xml = xml.Substring(xml.IndexOf(Convert.ToChar(60)));
            xml = xml.Substring(0, xml.LastIndexOf(Convert.ToChar(62)) + 1);
            return xml;
        }

        public static string RequestNotSupported => "Request not supported";
    }
}