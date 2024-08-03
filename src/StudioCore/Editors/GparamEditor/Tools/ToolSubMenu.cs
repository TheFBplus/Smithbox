﻿using ImGuiNET;
using StudioCore.Configuration;
using StudioCore.Editors.GparamEditor.Actions;
using StudioCore.GraphicsEditor;
using StudioCore.Interface;
using StudioCore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioCore.Editors.GparamEditor.Tools;

public class ToolSubMenu
{
    private GparamEditorScreen Screen;
    public ActionHandler Handler;

    public ToolSubMenu(GparamEditorScreen screen)
    {
        Screen = screen;
        Handler = new ActionHandler(screen);
    }

    public void Shortcuts()
    {

    }

    public void OnProjectChanged()
    {

    }

    public void DisplayMenu()
    {
        /*
        if (ImGui.BeginMenu("Tools"))
        {
            ImguiUtils.ShowMenuIcon($"{ForkAwesome.Bars}");

            if (ImGui.MenuItem("TODO", KeyBindings.Current.TextureViewer_ExportTexture.HintText))
            {

            }

            ImGui.EndMenu();
        }
        */
    }
}
