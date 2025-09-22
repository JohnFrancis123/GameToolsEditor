//using Editor.Engine; //Editor.Engine? Wtf???
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.IO;

namespace Editor.Editor //unsure if Editor.Editor should be the namespace we use
{
    internal class Project
    {
        public Level CurrentLevel { get; set; } = null;
        public List<Level> Levels { get; set; } = new();
        public string Folder { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public Project(ContentManager _content, string _name)
        {
            Folder = Path.GetDirectoryName(_name);
            Name = Path.GetFileName(_name);
            if (!Name.ToLower().EndsWith(".oce"))
            {
                Name += ".oce";
            }

            // Add a default level
            AddLevel(_content);
        }

        public void AddLevel(ContentManager _content) 
        {
            CurrentLevel = new();
            CurrentLevel.LoadContent(_content);
            Levels.Add(CurrentLevel);
        }

        public void Render()
        {
            CurrentLevel.Render();
        }

    }
}
