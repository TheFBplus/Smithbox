﻿using ImGuiNET;
using SoulsFormats;
using StudioCore.Configuration;
using StudioCore.Editor;
using StudioCore.Settings;
using StudioCore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Veldrid;
using Veldrid.Sdl2;

namespace StudioCore.GraphicsEditor;

public class GraphicsEditorScreen : EditorScreen
{
    private readonly PropertyEditor _propEditor;
    private ProjectSettings _projectSettings;

    public readonly AssetLocator AssetLocator;

    public ActionManager EditorActionManager = new();

    public GraphicsEditorScreen(Sdl2Window window, GraphicsDevice device, AssetLocator locator)
    {
        AssetLocator = locator;
        _propEditor = new PropertyEditor(EditorActionManager);
    }

    public string EditorName => "Graphics Editor";
    public string CommandEndpoint => "graphics";
    public string SaveType => "Graphics";

    public void DrawEditorMenu()
    {
    }

    public void OnGUI(string[] initcmd)
    {
    }

    public void OnProjectChanged(ProjectSettings newSettings)
    {
        _projectSettings = newSettings;

        ResetActionManager();
    }

    public void Save()
    {

    }

    public void SaveAll()
    {

    }

    private void ResetActionManager()
    {
        EditorActionManager.Clear();
    }
}