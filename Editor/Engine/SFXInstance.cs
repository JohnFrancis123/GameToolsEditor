using Editor.Editor;
using Microsoft.Xna.Framework.Audio;
using System.IO;

namespace Editor.Engine
{
    internal class SFXInstance
    {
        public string Name { get; set; }
        public SoundEffectInstance Instance { get; set; }

        public static SFXInstance Create(GameEditor _game, string _assetName)
        {
            string fileName = Path.Combine(_game.Project.Folder,
                                           _game.Project.ContentFolder,
                                           _game.Project.AssetFolder,
                                           _assetName);
            //fileName = fileName.Substring(fileName.IndexOf(_game.Content.RootDirectory) + _game.Content.RootDirectory.Length + 1);
            //fileName = fileName.Substring(3);
            fileName = _assetName;

            SoundEffect ef = _game.Content.Load<SoundEffect>(fileName);
            SoundEffectInstance efi = ef.CreateInstance();
            efi.Volume = 1;
            efi.IsLooped = false;
            return new SFXInstance() { Name = _assetName, Instance = efi };
        }
    } //no idea why I was told to put a semicolon here, the compiler doesn't care regardless.
}
