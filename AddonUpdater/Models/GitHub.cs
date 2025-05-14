using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AddonUpdater.Models
{
    public class GitHub
    {
        public required string Name { get; set; } = string.Empty; // название аддона
        public required string Link { get; set; } = string.Empty; // ссылка на toc
        public required string Directory { get; set; } = string.Empty; // /text/text.toc
        public required string Version { get; set; } = string.Empty; // Версия общая
        public required string MyVersion { get; set; } = string.Empty; // Моя версия
        public required string Branches { get; set; } = string.Empty; // Ветка на github
        public required string Description { get; set; } = string.Empty; // Описание
        public required string Author { get; set; } = string.Empty; // Автор
        public required string GithubLink { get; set; } = string.Empty; // Ссылка на github
        public required string Forum { get; set; } = string.Empty; // Ссылка на форум
        public required string BugReport { get; set; } = string.Empty; // Ссылка на BugReport
        public required string Regex { get; set; } = string.Empty; // Регулярное выражение
       // public string Replace { get; set; } // Замена для регулярного выражения
        public required string Category { get; set; } = string.Empty; // Категория
        public bool NeedUpdate { get; set; } // Нужно обновление
        //public bool DownloadMyAddon { get; set; } // Скачать ?
        public bool SavedVariables { get; set; } // Общие настройки для аддона
        public bool SavedVariablesPerCharacter { get; set; } // Настройки аддона на персонажа
        public required List<string> Files { get; set; } = new(); // Файлы

        public static bool operator ==(GitHub left, GitHub right)
        {
            if (ReferenceEquals(left, null))
                return ReferenceEquals(right, null);
            if (ReferenceEquals(right, null))
                return false;

            return left.Name == right.Name && left.Link == right.Link && left.Directory == right.Directory &&
                left.Version == right.Version && left.MyVersion == right.MyVersion &&
                left.Branches == right.Branches && left.Description == right.Description &&
                left.Author == right.Author && left.GithubLink == right.GithubLink &&
                left.Forum == right.Forum && left.BugReport == right.BugReport &&
                left.Regex == right.Regex && 
                left.Category == right.Category && left.NeedUpdate == right.NeedUpdate &&
                left.SavedVariables == right.SavedVariables && left.SavedVariablesPerCharacter == right.SavedVariablesPerCharacter &&
                left.Files?.SequenceEqual(right.Files ?? new List<string>()) == true;
        }

        public static bool operator !=(GitHub left, GitHub right) => !(left == right);

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return this == (GitHub)obj;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + (Name?.GetHashCode() ?? 0);
                hash = hash * 23 + (Link?.GetHashCode() ?? 0);
                hash = hash * 23 + (Directory?.GetHashCode() ?? 0);
                hash = hash * 23 + (Version?.GetHashCode() ?? 0);
                hash = hash * 23 + (MyVersion?.GetHashCode() ?? 0);
                hash = hash * 23 + (Branches?.GetHashCode() ?? 0);
                hash = hash * 23 + (Description?.GetHashCode() ?? 0);
                hash = hash * 23 + (Author?.GetHashCode() ?? 0);
                hash = hash * 23 + (GithubLink?.GetHashCode() ?? 0);
                hash = hash * 23 + (Forum?.GetHashCode() ?? 0);
                hash = hash * 23 + (BugReport?.GetHashCode() ?? 0);
                hash = hash * 23 + (Regex?.GetHashCode() ?? 0);
                hash = hash * 23 + (Category?.GetHashCode() ?? 0);
                hash = hash * 23 + NeedUpdate.GetHashCode();
                hash = hash * 23 + SavedVariables.GetHashCode();
                hash = hash * 23 + SavedVariablesPerCharacter.GetHashCode();
                if (Files != null)
                {
                    foreach (var file in Files)
                    {
                        hash = hash * 23 + (file?.GetHashCode() ?? 0);
                    }
                }
                return hash;
            }
        }
    }
}
