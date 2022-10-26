using System;

namespace Erd_Tools.ErdToolsException
{
    public class ParamLoadedTimeoutException : Exception
    {
        public ParamLoadedTimeoutException(string s) : base(s) { }
    }
}