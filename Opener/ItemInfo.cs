using System.Diagnostics;
using System.IO;

namespace FileOpener.Opener
{
    [DebuggerDisplay("{Name} @ {ProjectName}")]
    public sealed class ItemInfo
    {
        ///// <summary>Internal item id</summary>
        //public uint Id;

        /// <summary>Display name for project (VSITEMID_ROOT) or item</summary>
        public string Name = "";

        /// <summary>Physical path to the file</summary>
        public string Path = "";

        /// <summary>Logical path inside the project</summary>
        public string LogicalPath = "";

        /// <summary>Current project name</summary>
        public string ProjectName = "";

        /// <summary>Item icon index in associated icon cache</summary>
        public int IconIndex = -1;

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Name);
            writer.Write(Path);
            writer.Write(LogicalPath);
            writer.Write(ProjectName);
            writer.Write(IconIndex);
        }

        public static ItemInfo Deserialize(BinaryReader reader)
        {
            return new ItemInfo {
                Name = reader.ReadString(),
                Path = reader.ReadString(),
                LogicalPath = reader.ReadString(),
                ProjectName = reader.ReadString(),
                IconIndex = reader.ReadInt32(),
            };
        }
    }
}
