﻿using ImGuiNET;
using SoulsFormats;
using StudioCore.Configuration;
using StudioCore.Editors.TimeActEditor.Bank;
using StudioCore.Editors.TimeActEditor.Utils;
using StudioCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioCore.Editors.TimeActEditor;

public class TimeActInternalFileView
{
    private TimeActEditorScreen Screen;
    private TimeActViewSelection Selection;
    private TimeActDecorator Decorator;

    public TimeActInternalFileView(TimeActEditorScreen screen)
    {
        Screen = screen;
        Selection = screen.Selection;
        Decorator = screen.Decorator;
    }

    public void Display()
    {
        ImGui.Begin("Time Acts##TimeActList");

        if (!Selection.HasSelectedFileContainer())
        {
            ImGui.End();
            return;
        }

        ImGui.InputText($"Search##timeActFilter", ref TimeActFilters._timeActFilterString, 255);
        UIHelper.ShowHoverTooltip("Separate terms are split via the + character.");

        ImGui.BeginChild("TimeActList");

        for (int i = 0; i < Selection.ContainerInfo.InternalFiles.Count; i++)
        {
            InternalTimeActWrapper info = Selection.ContainerInfo.InternalFiles[i];
            TAE entry = Selection.ContainerInfo.InternalFiles[i].TAE;

            if (TimeActFilters.TimeActFilter(Selection.ContainerInfo, entry))
            {
                // Ignore entries marked for removal
                if (info.MarkForRemoval)
                {
                    continue;
                }

                var isSelected = false;
                if (i == Selection.CurrentTimeActKey ||
                    Selection.IsTimeActSelected(i))
                {
                    isSelected = true;
                }

                // Time Act row
                if (ImGui.Selectable($@"{info.Name}##TimeAct{i}", isSelected, ImGuiSelectableFlags.AllowDoubleClick))
                {
                    Selection.TimeActChange(entry, i);
                }

                // Arrow Selection
                if (ImGui.IsItemHovered() && Selection.SelectTimeAct)
                {
                    Selection.SelectTimeAct = false;
                    Selection.TimeActChange(entry, i);
                }
                if (ImGui.IsItemFocused() && (InputTracker.GetKey(Veldrid.Key.Up) || InputTracker.GetKey(Veldrid.Key.Down)))
                {
                    Selection.SelectTimeAct = true;
                }

                if (ImGui.IsItemVisible())
                {
                    if (CFG.Current.TimeActEditor_DisplayTimeActRow_AliasInfo)
                        TimeActUtils.DisplayTimeActAlias(Selection.ContainerInfo, entry.ID);
                }

                Selection.ContextMenu.TimeActMenu(isSelected, entry.ID.ToString());

                if (Selection.FocusTimeAct)
                {
                    Selection.FocusTimeAct = false;

                    ImGui.SetScrollHereY();
                }
            }

        }
        ImGui.EndChild();

        ImGui.End();
    }
}