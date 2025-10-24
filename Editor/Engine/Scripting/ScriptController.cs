using Editor.Editor;
using Editor.Engine.Lights;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using System;
using System.IO;
using System.Reflection;


namespace Editor.Engine.Scripting
{
    internal class ScriptController
    {
        private static readonly Lazy<ScriptController> lazy = new(() => new ScriptController());
        public static ScriptController Instance { get { return lazy.Value; } }

        private readonly Script m_LUAScript = new();
        private ScriptController() 
        {
        }

        public void RegisterMethods() 
        {
            // Registers C# methods in hte global state, to be callabel from scripts
            //m_LUAScript.Globals["MoveCamera"] = (Func<IEnumerable<int>>)GetNumbers;
        }

        public DynValue LoadScript(string _script)
        {
            // Loads and compiles the supplied script
            // This action also loads the script into the global state
            return m_LUAScript.DoString(_script);
        }

        public void LoadScriptFile(string _file)
        {
            string script = File.ReadAllText(_file);
            LoadScript(script);
        }

        public void LoadSharedObjects(Project _project)
        {
            UserData.RegisterType<Light>();
            UserData.RegisterType<Terrain>();
            UserData.RegisterType<Level>();
            UserData.RegisterType<Camera>();
            UserData.RegisterType<Project>();
            DynValue project = UserData.Create(_project);
            m_LUAScript.Globals.Set("project", project);
        }

        public DynValue Execute(string _function, params object[] _params)
        {
            // Call the script from the global state
            DynValue function = m_LUAScript.Globals.Get(_function);
            if (function.IsNil())
            {
                return function;
            }
            return m_LUAScript.Call(function, _params);
        }
    }
}
