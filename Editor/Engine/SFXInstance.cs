using Editor.Editor;
using Microsoft.Xna.Framework.Audio;

namespace Editor.Engine
{
    internal class SFXInstance
    {
        public string Name { get; set; }
        public SoundEffectInstance Instance { get; set; }

        public static SFXInstance Create(GameEditor _game, string _assetName)
        {
            SoundEffect ef = _game.Content.Load<SoundEffect>(_assetName);
            SoundEffectInstance efi = ef.CreateInstance();
            efi.Volume = 1;
            efi.IsLooped = false;
            return new SFXInstance() { Name = _assetName, Instance = efi };
        }
    } //no idea why I was told to put a semicolon here, the compiler doesn't care regardless.
}
