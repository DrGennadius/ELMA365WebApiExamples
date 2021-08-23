using System;
using System.Collections.Generic;
using System.IO;

namespace BaseHelpersForExamples
{
    public class Configuration
    {
        public string UrlBase { get; set; }

        public string XToken { get; set; }

        public Dictionary<string, object> Parameters { get; set; }

        public object this[string parameterKey]
        {
            get { return Parameters[parameterKey]; }
            set { Parameters[parameterKey] = value; }
        }
    }
}
