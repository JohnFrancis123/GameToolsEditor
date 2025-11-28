//using Editor.Engine; //Editor.Engine? Wtf???
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Editor.Engine.Interfaces;
using Editor.Engine.Scripting;
using Editor.Engine;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace Editor.Editor //unsure if Editor.Editor should be the namespace we use
{
    internal class Project : ISerializable
    {
        public event AssetsUpdated OnAssetsUpdated;
        public Level CurrentLevel { get; private set; } = null;
        public List<Level> Levels { get; private set; } = new();
        public string Folder { get; private set; } = string.Empty;
        public string ContentFolder { get; private set; } = string.Empty;
        public string AssetFolder { get; private set; } = string.Empty;
        public string ObjectFolder { get; private set; } = string.Empty;
        public string ScriptFolder { get; private set; } = string.Empty;
        public string Name { get; private set; } = string.Empty;
        public AssetMonitor AssetMonitor { get; private set; } = null;
        public ScriptMonitor ScriptMonitor { get; private set; } = null;

        public Project()
        {
        }

        public Project(GameEditor _game, string _name)
        {
            Folder = Path.GetDirectoryName(_name);
            Name = Path.GetFileName(_name);
            if (!Name.ToLower().EndsWith(".oce"))
            {
                Name += ".oce"; //
            }

            // Create Content folder for assets, and copy the mgcb template
            ContentFolder = Path.Combine(Folder, "Content");
            AssetFolder = Path.Combine(ContentFolder, "bin");
            ObjectFolder = Path.Combine(ContentFolder, "obj");
            ScriptFolder = Path.Combine(Folder, "Scripts"); //could be project folder / scripts or content folder / scripts
            
            char d = Path.DirectorySeparatorChar;
            if (!Directory.Exists(ContentFolder))
            {
                Directory.CreateDirectory(ContentFolder);
                Directory.CreateDirectory(AssetFolder);
                Directory.CreateDirectory(ObjectFolder);
                File.Copy($"ContentTemplate.mgcb", ContentFolder + $"{d}Content.mgcb");
            }

            if (!Directory.Exists(ScriptFolder))
            {
                Directory.CreateDirectory(ScriptFolder);
            }
                CreateScriptFile(ScriptFolder + $"{d}BeforeRender.lua");
                CreateScriptFile(ScriptFolder + $"{d}AfterRender.lua");
                CreateScriptFile(ScriptFolder + $"{d}BeforeUpdate.lua");
                CreateScriptFile(ScriptFolder + $"{d}AfterUpdate.lua");
            AssetMonitor = new(ObjectFolder);
            AssetMonitor.OnAssetsUpdated += AssetMon_OnAssetsUpdated;
            ScriptMonitor = new(ScriptFolder);
            ScriptMonitor.OnScriptUpdated += ScriptMon_OnScriptUpdated;
            // Add a default level
            AddLevel(_game);
            ConfigureScripts();
        }

        private void ScriptMon_OnScriptUpdated(string _script)
        {
            ScriptController.Instance.LoadScriptFile(_script);
        }
        private void AssetMon_OnAssetsUpdated()
        {
            OnAssetsUpdated?.Invoke();
        }

        public void AddLevel(GameEditor _game) 
        {
            CurrentLevel = new();
            CurrentLevel.LoadContent(_game);
            Levels.Add(CurrentLevel);
        }

        public void Update(float _delta)
        {
            CurrentLevel?.Update(_delta);
        }
        public void Render()
        {
            CurrentLevel.Render();
        }

        private void CreateScriptFile(string _file)
        {
            string funcName = Path.GetFileNameWithoutExtension(_file);
            if (!File.Exists(_file))
            {
                File.Create(_file).Close();
                File.AppendAllLines(_file, new string[] { "function " + funcName + "Main()", "end" });
            }
        }

        private void ConfigureScripts()
        {
            char d = Path.DirectorySeparatorChar;
            var sc = ScriptController.Instance;
            sc.LoadSharedObjects(this);
            sc.LoadScriptFile(ScriptFolder + $"{d}BeforeRender.lua");
            sc.LoadScriptFile(ScriptFolder + $"{d}AfterRender.lua");
            sc.LoadScriptFile(ScriptFolder + $"{d}BeforeUpdate.lua");
            sc.LoadScriptFile(ScriptFolder + $"{d}AfterUpdate.lua");

        }

        public void Serialize(BinaryWriter _stream) 
        {
            _stream.Write(Folder);
            _stream.Write(Name);
            _stream.Write(ContentFolder);
            _stream.Write(AssetFolder);
            _stream.Write(ObjectFolder);
            _stream.Write(ScriptFolder);

            _stream.Write(Levels.Count);
            int clIndex = Levels.IndexOf(CurrentLevel);
            foreach (var level in Levels) 
            {
                level.Serialize(_stream);
            }
            _stream.Write(clIndex);

        }

        public void Deserialize(BinaryReader _stream, GameEditor _game)
        {
            Folder = _stream.ReadString();
            Name = _stream.ReadString();
            ContentFolder = _stream.ReadString();
            AssetFolder = _stream.ReadString();
            ObjectFolder = _stream.ReadString();
            ScriptFolder = _stream.ReadString();

            int levelCount = _stream.ReadInt32();
            for(int count = 0; count < levelCount; count++)
            {
                Level l = new();
                l.Deserialize(_stream, _game);
                Levels.Add(l);
            }
            int clIndex = _stream.ReadInt32();
            CurrentLevel = Levels[clIndex];

            AssetMonitor = new(ObjectFolder);
            AssetMonitor.OnAssetsUpdated += AssetMon_OnAssetsUpdated;
            ConfigureScripts();
        }

    }
}
