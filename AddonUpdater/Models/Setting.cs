using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddonUpdater.Models
{
    class Setting
    {
        public required List<string> DeleteDirectory { get; set; } = new();

        public required List<Toc> Tocs { get; set; } = new();

        public required string News { get; set; } = string.Empty;

        public required List<string> Files { get; set; } = new();

        public required string ForumLink { get; set; } = string.Empty;

        public required string GitHubLink { get; set; } = string.Empty;

        public required string DiscordLink { get; set; } = string.Empty;

        public required string DonateLink { get; set; } = string.Empty;

        public required string Contact { get; set; } = string.Empty;

        public required string Version { get; set; } = string.Empty;

        public required string VersionUpdate { get; set; } = string.Empty;

        public required string AddonUpdaterLink { get; set; } = string.Empty;

        public required string UpdateLink { get; set; } = string.Empty;

        public required string Thx { get; set; } = string.Empty;

    }

    class Toc
    {
        public required string Link { get; set; } = string.Empty;
        public required string Regex { get; set; } = string.Empty;

    }

    class SettingApp
    {
        public required string PathWow { get; set; } = string.Empty;
        public bool AutoUpdateBool { get; set; }

        public bool DescriptionBool { get; set; }

        public required List<LastUpdateAddon> LastUpdate { get; set; } = new();

        public bool LauncherOpen { get; set; }

        public required List<string> UpdateAddon { get; set; } = new();

        public required List<string> PathsWow { get; set; } = new();

        public required string BackupWTF { get; set; } = string.Empty;

    }
}
