﻿using ImGuiNET;
using SoulsFormats;
using StudioCore.Editor;
using StudioCore.Interface;
using StudioCore.MsbEditor;
using StudioCore.UserProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioCore.Editors.MapEditor.Toolbar
{
    public static class Action_TogglePresence
    {
        public static void Select(ViewportSelection _selection)
        {
            if (CFG.Current.Toolbar_Show_Toggle_Presence)
            {
                if (ImGui.Selectable("Toggle Presence##tool_Selection_Presence", false, ImGuiSelectableFlags.AllowDoubleClick))
                {
                    MapToolbar.CurrentTool = SelectedTool.Selection_Toggle_Presence;

                    if (ImGui.IsMouseDoubleClicked(0) && _selection.IsSelection())
                    {
                        Act(_selection);
                    }
                }
            }
        }

        public static void Configure(ViewportSelection _selection)
        {
            if (MapToolbar.CurrentTool == SelectedTool.Selection_Toggle_Presence)
            {
                if (CFG.Current.Toolbar_Presence_Dummy_Type_ER)
                    ImGui.Text("Toggle the load status of the current selection.");
                else
                    ImGui.Text("Toggle the Dummy status of the current selection.");

                ImGui.Separator();
                ImGui.Text($"Shortcut: {ImguiUtils.GetKeybindHint(KeyBindings.Current.Toolbar_Dummify.HintText)} for Disable");
                ImGui.Text($"Shortcut: {ImguiUtils.GetKeybindHint(KeyBindings.Current.Toolbar_Undummify.HintText)} for Enable");
                ImGui.Separator();

                if (ImGui.Checkbox("Disable", ref CFG.Current.Toolbar_Presence_Dummify))
                {
                    CFG.Current.Toolbar_Presence_Undummify = false;
                }
                if (CFG.Current.Toolbar_Presence_Dummy_Type_ER)
                    ImguiUtils.ShowHoverTooltip("Make the current selection Dummy Objects/Asset/Enemy types.");
                else
                    ImguiUtils.ShowHoverTooltip("Disable the current selection, preventing them from being loaded in-game.");

                if (ImGui.Checkbox("Enable", ref CFG.Current.Toolbar_Presence_Undummify))
                {
                    CFG.Current.Toolbar_Presence_Dummify = false;
                }
                if (CFG.Current.Toolbar_Presence_Dummy_Type_ER)
                    ImguiUtils.ShowHoverTooltip("Make the current selection (if Dummy) normal Objects/Asset/Enemy types.");
                else
                    ImguiUtils.ShowHoverTooltip("Enable the current selection, allow them to be loaded in-game.");

                if (Project.Type == ProjectType.ER)
                {
                    ImGui.Checkbox("Use Game Edition Disable", ref CFG.Current.Toolbar_Presence_Dummy_Type_ER);
                    ImguiUtils.ShowHoverTooltip("Use the GameEditionDisable property to disable entities instead of the Dummy entity system.");
                }
            }
        }

        public static void Act(ViewportSelection _selection)
        {
            if (CFG.Current.Toolbar_Presence_Dummy_Type_ER)
            {
                if (CFG.Current.Toolbar_Presence_Dummify)
                {
                    ER_DummySelection(_selection);
                }
                if (CFG.Current.Toolbar_Presence_Undummify)
                {
                    ER_UnDummySelection(_selection);
                }
            }
            else
            {
                if (CFG.Current.Toolbar_Presence_Dummify)
                {
                    DummySelection(_selection);
                }
                if (CFG.Current.Toolbar_Presence_Undummify)
                {
                    UnDummySelection(_selection);
                }
            }
        }

        public static void ER_DummySelection(ViewportSelection _selection)
        {
            List<MsbEntity> sourceList = _selection.GetFilteredSelection<MsbEntity>().ToList();
            foreach (MsbEntity s in sourceList)
            {
                if (Project.Type == ProjectType.ER)
                {
                    s.SetPropertyValue("GameEditionDisable", 1);
                }
            }
        }

        public static void ER_UnDummySelection(ViewportSelection _selection)
        {
            List<MsbEntity> sourceList = _selection.GetFilteredSelection<MsbEntity>().ToList();
            foreach (MsbEntity s in sourceList)
            {
                if (Project.Type == ProjectType.ER)
                {
                    s.SetPropertyValue("GameEditionDisable", 0);
                }
            }
        }

        public static void DummySelection(ViewportSelection _selection)
        {
            string[] sourceTypes = { "Enemy", "Object", "Asset" };
            string[] targetTypes = { "DummyEnemy", "DummyObject", "DummyAsset" };
            DummyUndummySelection(_selection, sourceTypes, targetTypes);
        }

        public static void UnDummySelection(ViewportSelection _selection)
        {
            string[] sourceTypes = { "DummyEnemy", "DummyObject", "DummyAsset" };
            string[] targetTypes = { "Enemy", "Object", "Asset" };
            DummyUndummySelection(_selection, sourceTypes, targetTypes);
        }

        private static void DummyUndummySelection(ViewportSelection _selection, string[] sourceTypes, string[] targetTypes)
        {
            Type msbclass;
            switch (Project.Type)
            {
                case ProjectType.DES:
                    msbclass = typeof(MSBD);
                    break;
                case ProjectType.DS1:
                case ProjectType.DS1R:
                    msbclass = typeof(MSB1);
                    break;
                case ProjectType.DS2S:
                    msbclass = typeof(MSB2);
                    //break;
                    return; //idk how ds2 dummies should work
                case ProjectType.DS3:
                    msbclass = typeof(MSB3);
                    break;
                case ProjectType.BB:
                    msbclass = typeof(MSBB);
                    break;
                case ProjectType.SDT:
                    msbclass = typeof(MSBS);
                    break;
                case ProjectType.ER:
                    msbclass = typeof(MSBE);
                    break;
                case ProjectType.AC6:
                    msbclass = typeof(MSB_AC6);
                    break;
                default:
                    throw new ArgumentException("type must be valid");
            }
            List<MsbEntity> sourceList = _selection.GetFilteredSelection<MsbEntity>().ToList();

            ChangeMapObjectType action = new(MapToolbar.Universe, msbclass, sourceList, sourceTypes, targetTypes, "Part", true);
            MapToolbar.ActionManager.ExecuteAction(action);
        }
    }
}
