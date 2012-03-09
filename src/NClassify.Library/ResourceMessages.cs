#region Copyright (c) 2012 Roger O Knapp
//  Permission is hereby granted, free of charge, to any person obtaining a copy 
//  of this software and associated documentation files (the "Software"), to deal 
//  in the Software without restriction, including without limitation the rights 
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
//  copies of the Software, and to permit persons to whom the Software is 
//  furnished to do so, subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in 
//  all copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
//  FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//  IN THE SOFTWARE.
#endregion
#pragma warning disable 1591
namespace NClassify.Library {
    //[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    public static class ResourceMessages
    {
        public static global::System.Globalization.CultureInfo CultureInfo;
        internal static global::System.Resources.ResourceManager ResourceManager;

        static ResourceMessages()
        {
            try
            {
                foreach (string resName in typeof (ResourceMessages).Assembly.GetManifestResourceNames())
                {
                    if (resName.EndsWith("NClassify.resources", global::System.StringComparison.Ordinal))
                    {
                        ResourceManager = new global::System.Resources.ResourceManager(
                            resName.Substring(0, resName.Length - ".resources".Length),
                            typeof (ResourceMessages).Assembly
                            );
                        break;
                    }
                }
            }
            catch { ResourceManager = null; }
        }

        private static string GetString(string name)
        {
            if (ResourceManager != null)
            {
                try { return ResourceManager.GetString(name, CultureInfo); }
                catch { return null; }
            }
            return null;
        }


        public static string InvalidField { get { return GetString("InvalidField") ?? "The field {0} is invalid."; } }
        public static string InvalidFormat { get { return GetString("InvalidFormat") ?? "The value '{0}' is in an invalid format."; } }
        public static string MissingRequiredField { get { return GetString("MissingRequiredField") ?? "The field {0} is required."; } }

        public static string MustNotBeNull { get { return GetString("MustNotBeNull") ?? "The field {0} is invalid, can not be null."; } }

        public static string MustBeGreaterThan { get { return GetString("MustBeGreaterThan") ?? "The field {0} is invalid, must be greater than {1}."; } }
        public static string MustBeLessThan { get { return GetString("MustBeLessThan") ?? "The field {0} is invalid, must be less than {1}."; } }

        public static string MustBeLongerThan { get { return GetString("MustBeLongerThan") ?? "The field {0} is invalid, must be at least {1}."; } }
        public static string MustBeShorterThan { get { return GetString("MustBeShorterThan") ?? "The field {0} is invalid, must be shorter than {1}."; } }

        public static string MustMatchFormat { get { return GetString("MustMatchFormat") ?? "The field {0} is invalid, the value does not match expected format."; } }
        public static string MustBeOneOf { get { return GetString("MustNotBeNull") ?? "The field {0} is invalid, expected one of: {1}."; } }
    }
}
