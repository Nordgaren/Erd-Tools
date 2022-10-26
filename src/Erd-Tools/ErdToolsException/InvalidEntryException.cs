using System;

namespace Erd_Tools.ErdToolsException
{
    public class InvalidEntryException : Exception
    {
        public InvalidEntryException(string s) : base(s) { }
    }
}