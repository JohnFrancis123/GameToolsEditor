using Editor;
using Editor.Engine;
using GUI.Editor;
using Editor.Editor;
using System;
using System.Threading;

// Set STA thread mode, for OpenFileDialog to work
Thread t = Thread.CurrentThread;
t.SetApartmentState(ApartmentState.Unknown);
t.SetApartmentState(ApartmentState.STA);

FormEditor editor = new();
editor.Game = new GameEditor(editor);
editor.Show();
editor.Game.Run();

//using var game = new Editor.GameEditor();
//game.Run();
