using System;

namespace LowPolyHnS.ModuleManager
{
    [Serializable]
    public class Version
    {
        public enum Separator
        {
            Dot,
            Dash
        }

        public static Version NONE => new Version(0, 0, 0);

        // PROPERTIES: ----------------------------------------------------------------------------

        public int major;
        public int minor;
        public int patch;

        // INITIALIZERS: --------------------------------------------------------------------------

        public Version()
        {
            major = 0;
            minor = 0;
            patch = 0;
        }

        public Version(int major, int minor, int patch)
        {
            this.major = major;
            this.minor = minor;
            this.patch = patch;
        }

        public Version(string version)
        {
            string[] codes = version.Split('.');
            if (codes.Length != 3) return;
            major = int.Parse(codes[0]);
            minor = int.Parse(codes[1]);
            patch = int.Parse(codes[2]);
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public bool Higher(Version other)
        {
            if (major > other.major) return true;
            if (major == other.major)
            {
                if (minor > other.minor) return true;
                if (minor == other.minor && patch > other.patch) return true;
            }

            return false;
        }

        public override string ToString()
        {
            return string.Format(
                "{0}.{1}.{2}",
                major,
                minor,
                patch
            );
        }

        public string ToStringWithDash()
        {
            return string.Format(
                "{0}-{1}-{2}",
                major,
                minor,
                patch
            );
        }
    }
}