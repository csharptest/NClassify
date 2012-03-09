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
