# 🎮 Game Tools Editor

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![MonoGame](https://img.shields.io/badge/MonoGame-3.8-E73C00?logo=monogame)](https://www.monogame.net/)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

A powerful and intuitive 3D game editor built with MonoGame and C#. Create, edit, and manage game levels with terrain, 3D models, lighting, and Lua scripting support.

## ✨ Features

- **🏔️ Terrain Editor** - Create and modify terrains using heightmap textures with real-time preview
- **📦 Asset Management** - Import and organize 3D models, textures, and audio assets
- **💡 Dynamic Lighting** - Configure directional lights with customizable colors and positions
- **🎬 3D Camera System** - Navigate your scene with a flexible camera system
- **🎨 Material System** - Apply and customize materials with diffuse textures and effects
- **📝 Lua Scripting** - Extend functionality with custom Lua scripts (BeforeUpdate, AfterUpdate, BeforeRender, AfterRender)
- **🗂️ Project Management** - Create and manage multiple game projects with organized folder structures
- **👁️ Visual Editor** - Windows Forms-based interface with drag-and-drop support
- **🎵 Audio Support** - Import and manage sound effects for your game
- **💾 Serialization** - Save and load levels with full scene preservation

## 🖼️ Screenshots

*Coming soon - Add your screenshots here to showcase the editor in action!*

## 🔧 Prerequisites

Before you begin, ensure you have the following installed:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- Windows OS (the editor uses Windows Forms and DirectX)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/) (recommended)
- MonoGame dependencies (automatically restored via NuGet)

## 📥 Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/JohnFrancis123/GameToolsEditor.git
   cd GameToolsEditor
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore Editor.sln
   ```

3. **Build the solution**
   ```bash
   dotnet build Editor.sln
   ```

4. **Run the editor**
   ```bash
   cd Editor
   dotnet run
   ```

   Or open `Editor.sln` in Visual Studio and press F5 to build and run.

## 🚀 Quick Start

### Creating a New Project

1. Launch the editor
2. Go to `File > New Project`
3. Choose a name and location for your project
4. A new project will be created with default folders:
   - `Content/` - For assets (models, textures, audio)
   - `Scripts/` - For Lua scripts
   - `Content/bin/` - Compiled assets
   - `Content/obj/` - Build intermediates

### Adding Assets

1. **Import 3D Models**: Drag and drop model files into the Assets panel
2. **Import Textures**: Add texture files to be used with materials
3. **Import Audio**: Add sound effects and music files

### Working with Terrain

1. Create or import a heightmap texture (grayscale image)
2. Select the terrain in the scene
3. Adjust height, position, and texture properties
4. Apply grass or other base textures

### Scripting with Lua

The editor supports custom Lua scripts for extending gameplay:

- `BeforeUpdate.lua` - Runs before each game update
- `AfterUpdate.lua` - Runs after each game update
- `BeforeRender.lua` - Runs before rendering
- `AfterRender.lua` - Runs after rendering

Scripts are automatically monitored and reloaded when changed.

## 🏗️ Project Structure

```
GameToolsEditor/
├── Editor/                    # Main editor application
│   ├── Editor/               # Core editor classes
│   │   ├── GameEditor.cs     # Main game editor class
│   │   ├── Project.cs        # Project management
│   │   ├── AssetMonitor.cs   # Asset file watcher
│   │   └── ScriptMonitor.cs  # Script file watcher
│   ├── Engine/               # Game engine components
│   │   ├── Camera.cs         # Camera system
│   │   ├── Level.cs          # Level management
│   │   ├── Terrain.cs        # Terrain rendering
│   │   ├── Models.cs         # 3D model handling
│   │   ├── Material.cs       # Material system
│   │   ├── Renderer.cs       # Rendering pipeline
│   │   ├── Lights/           # Lighting system
│   │   ├── Scripting/        # Lua scripting integration
│   │   └── Interfaces/       # Common interfaces
│   ├── GUI/                  # User interface
│   │   ├── FormEditor.cs     # Main editor form
│   │   └── ListItem*.cs      # UI list items
│   ├── Content/              # Default content and templates
│   └── Program.cs            # Application entry point
├── hlsl Editor/              # HLSL shader editor (future)
└── Editor.sln                # Visual Studio solution
```

## 🛠️ Technology Stack

- **Framework**: .NET 8.0 (Windows)
- **Game Engine**: [MonoGame 3.8](https://www.monogame.net/) (WindowsDX)
- **Scripting**: [MoonSharp](https://www.moonsharp.org/) (Lua interpreter for .NET)
- **UI**: Windows Forms
- **Graphics API**: DirectX (via MonoGame)
- **Language**: C# 12.0

## 🎯 Key Components

### Engine Features
- **Camera System**: Free-moving camera with configurable FOV and aspect ratio
- **Terrain System**: Heightmap-based terrain with normal mapping and texturing
- **Model System**: 3D model loading and rendering with material support
- **Lighting**: Directional lighting with configurable position and color
- **Materials**: Effect-based material system with texture support

### Editor Features
- **Asset Management**: Real-time file monitoring and automatic asset updates
- **Drag & Drop**: Intuitive asset and prefab placement
- **Project System**: Organized project structure with Content Pipeline integration
- **Visual Feedback**: Selection highlighting and gizmos
- **Script Integration**: Hot-reloading Lua scripts

## 🤝 Contributing

Contributions are welcome! Here's how you can help:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

### Development Guidelines

- Follow C# coding conventions
- Comment complex logic
- Test your changes thoroughly
- Update documentation as needed

## 📝 License

This project is available under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🐛 Known Issues

- HLSL Editor is currently under development
- Some features may require additional testing on different hardware configurations

## 📮 Support

If you encounter any issues or have questions:

- Open an [Issue](https://github.com/JohnFrancis123/GameToolsEditor/issues)
- Check existing issues for solutions
- Contribute fixes via Pull Requests

## 🎓 Learning Resources

- [MonoGame Documentation](https://docs.monogame.net/)
- [MoonSharp Documentation](https://www.moonsharp.org/getting_started.html)
- [Lua 5.2 Reference](https://www.lua.org/manual/5.2/)

## 🙏 Acknowledgments

- Built with [MonoGame](https://www.monogame.net/) framework
- Lua scripting powered by [MoonSharp](https://www.moonsharp.org/)
- Inspired by modern game engines and level editors

---

**Made with ❤️ for game developers**
