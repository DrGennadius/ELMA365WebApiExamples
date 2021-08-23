using System;
using System.Collections.Generic;
using System.IO;

namespace BaseHelpersForExamples
{
    public class Configuration
    {
        public string UrlBase { get; set; }

        public string XToken { get; set; }

        public Dictionary<string, string> Parameters { get; set; }

        public string this[string parameterKey]
        {
            get { return Parameters[parameterKey]; }
            set { Parameters[parameterKey] = value; }
        }
    }
}
