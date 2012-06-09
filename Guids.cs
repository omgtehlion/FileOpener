// Guids.cs
// MUST match guids.h
using System;

namespace FileOpener
{
    static class GuidList
    {
        public const string guidFileOpenerPkgString = "cae7e24b-9101-4d28-95e6-01a43c3ab614";
        public const string guidFileOpenerCmdSetString = "c0856bf4-4f00-462d-9c6c-ddf617104700";

        public const string guidMyToolWindowString = "0af43ae7-53cb-488c-92ba-4d3edf375fca";

        public static readonly Guid guidFileOpenerCmdSet = new Guid(guidFileOpenerCmdSetString);
    };
}