# GameToolsEditor

A powerful 3D game level editor built with MonoGame and Windows Forms, featuring terrain editing, model management, scripting support, and real-time rendering capabilities.

## Features

### Terrain System
- **Height-map based terrain generation** - Create realistic landscapes using height map textures
- **Real-time terrain rendering** - Visualize terrain changes instantly with optimized rendering
- **Texture mapping** - Apply grass and other textures to terrain surfaces
- **Customizable terrain parameters** - Control terrain size, scale, and appearance

### Asset Management
- **Drag-and-drop asset import** - Easily import models, textures, and audio files
- **Asset monitoring** - Automatic detection and reloading of modified assets
- **Content pipeline integration** - Built-in MonoGame Content Pipeline (MGCB) support
- **Organized asset structure** - Automatic organization into Content folders

### 3D Editing Tools
- **3D model placement** - Position and manipulate 3D models in your scene
- **Camera controls** - Intuitive camera navigation for scene exploration
- **Level management** - Create and manage multiple game levels
- **Prefab system** - Create reusable game object templates

### Lighting System
- **Dynamic lighting** - Configurable light sources with position and color control
- **Shader support** - Custom HLSL shaders for advanced rendering effects
- **Material system** - Define and apply materials to objects

### Scripting Support
- **Lua scripting integration** - Powered by MoonSharp for game logic
- **Script monitoring** - Automatic reload of modified scripts during development
- **Script controller** - Manage and execute scripts within the editor

### Audio Features
- **Sound emitter system** - Place and configure audio sources in your levels
- **SFX management** - Import and manage sound effects

## Technology Stack

- **Framework**: [MonoGame 3.8](https://www.monogame.net/) (DirectX)
- **.NET**: .NET 8.0 (Windows)
- **UI**: Windows Forms
- **Scripting**: [MoonSharp](https://www.moonsharp.org/) (Lua interpreter)
- **Graphics**: DirectX with custom HLSL shaders
- **Language**: C#

## Prerequisites

- Windows OS (Windows 10 or later recommended)
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- Visual Studio 2022 or later (optional, but recommended)
- DirectX runtime

## Getting Started

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/JohnFrancis123/GameToolsEditor.git
   cd GameToolsEditor
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the project**
   ```bash
   dotnet build Editor.sln
   ```

4. **Run the editor**
   ```bash
   cd Editor
   dotnet run
   ```

### Alternative: Using Visual Studio

1. Open `Editor.sln` in Visual Studio 2022
2. Press `F5` to build and run the project
3. The editor will launch with the main form interface

## 📖 Usage Guide

### Creating a New Project

1. Launch the GameToolsEditor application
2. Click `File → New Project` (or similar menu option)
3. Choose a location for your project
4. The editor will create the necessary folder structure:
   - `Content/` - For assets (models, textures, sounds)
   - `Scripts/` - For Lua scripts
   - `*.oce` - Project file

### Working with Terrain

1. **Create Terrain**: The editor loads default terrain on startup
2. **Modify Height Map**: Replace `DefaultHeightMap.jpg` in the Content folder
3. **Apply Textures**: Use `DefaultGrass.jpg` or import custom textures
4. **Adjust Parameters**: Modify terrain scale and other properties in the level settings

### Adding 3D Models

1. Import 3D models by placing them in the Content folder
2. Build the content using the integrated MGCB tool
3. Drag models from the asset list into the scene
4. Position and configure models using the editor tools

### Using Prefabs

1. Create prefabs from existing game objects
2. Save commonly used configurations as templates
3. Drag prefabs into your scene for quick level design

### Scripting with Lua

1. Create `.lua` files in the `Scripts/` folder
2. Scripts are automatically monitored for changes
3. Use the script controller to attach scripts to game objects
4. Scripts can control game logic, object behavior, and more

### Working with Audio

1. Import audio files (`.wav`, `.mp3`) into the Content folder
2. Add sound emitters to objects in your scene
3. Configure audio properties (volume, pitch, etc.)

## Project Structure

```
GameToolsEditor/
├── Editor/                      # Main editor application
│   ├── Editor/                  # Core editor logic
│   │   ├── GameEditor.cs       # Main game/editor class
│   │   ├── Project.cs          # Project management
│   │   ├── AssetMonitor.cs     # Asset file watching
│   │   └── ScriptMonitor.cs    # Script file watching
│   ├── Engine/                  # Game engine components
│   │   ├── Level.cs            # Level management
│   │   ├── Terrain.cs          # Terrain system
│   │   ├── Models.cs           # 3D model handling
│   │   ├── Camera.cs           # Camera controls
│   │   ├── Material.cs         # Material system
│   │   ├── Lights/             # Lighting system
│   │   ├── Scripting/          # Lua scripting integration
│   │   └── Interfaces/         # Core interfaces
│   ├── GUI/                     # User interface
│   │   ├── FormEditor.cs       # Main editor form
│   │   ├── ListItemAsset.cs    # Asset list items
│   │   ├── ListItemPrefab.cs   # Prefab list items
│   │   └── ListItemLevel.cs    # Level list items
│   ├── Content/                 # Default assets
│   │   ├── *.fx                # HLSL shader files
│   │   └── *.jpg               # Default textures
│   ├── Program.cs              # Application entry point
│   └── Editor.csproj           # Project file
├── hlsl Editor/                 # HLSL shader editing area
│   └── Content/                # Shader files
└── Editor.sln                  # Visual Studio solution
```

## Key Components

### GameEditor
The main game class that inherits from MonoGame's `Game` class. Handles:
- Graphics initialization and rendering
- Content loading and management
- Integration with Windows Forms UI
- Game loop and update cycles

### Project
Manages project-level concerns:
- Project file structure (`.oce` files)
- Content and script folder management
- Asset monitoring and auto-reload
- Level organization

### Level
Represents a game level with:
- 3D models collection
- Terrain instance
- Camera configuration
- Lighting setup
- Serialization support

### Terrain
Height-map based terrain system featuring:
- Procedural mesh generation from height maps
- Multi-texturing support
- Optimized rendering with culling
- Custom shader integration

## Shader Development

The editor supports custom HLSL shaders located in the `Content/` folder:

- `DefaultShader.fx` - Standard model rendering
- `TerrainEffect.fx` - Terrain-specific rendering with multi-texturing
- `MyShader.fx` - Custom shader template
- `File.fx` - Additional shader resources

Shaders can be edited in Visual Studio or any text editor and will be compiled automatically by the MonoGame Content Pipeline.

### Development Guidelines

- Follow C# coding conventions
- Comment complex logic
- Test your changes thoroughly
- Update documentation as needed

## Known Issues

- The editor is Windows-only due to Windows Forms dependency
- Some features may require DirectX runtime
- Large terrain meshes may impact performance

## Future Enhancements

- [ ] Cross-platform support (Avalonia UI or similar)
- [ ] Undo/Redo system
- [ ] Scene graph visualization
- [ ] Advanced terrain editing tools (sculpting, painting)
- [ ] Physics integration
- [ ] Particle system editor
- [ ] Animation timeline
- [ ] Multi-user collaboration

## License

This project is available as open source. Please check the repository for specific license information.

## Author

**JohnFrancis123**

- GitHub: [@JohnFrancis123](https://github.com/JohnFrancis123)

## Acknowledgments

- [MonoGame](https://www.monogame.net/) - Open-source game framework
- [MoonSharp](https://www.moonsharp.org/) - Lua scripting for .NET
- The game development community

## Support

If you have questions or need help:

- Open an [issue](https://github.com/JohnFrancis123/GameToolsEditor/issues)
- Check existing issues for solutions
- Contribute improvements via pull requests

---

* If you find this project useful, please consider giving it a star!**

*Built using MonoGame and C#*
