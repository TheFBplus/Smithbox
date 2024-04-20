﻿using ImGuiNET;
using StudioCore.Editor;
using StudioCore.Editors.MapEditor.Prefabs;
using StudioCore.Interface;
using StudioCore.MsbEditor;
using StudioCore.UserProject;
using StudioCore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace StudioCore.Editors.MapEditor.Toolbar
{
    public class MapAction_ImportPrefab
    {
        private static string _searchInput = "";
        private static string _searchInputCache = "";

        private static bool ShowPrefabContents = true;


        public static void Select(ViewportSelection _selection)
        {
            if (!MapEditorToolbar.IsSupportedProjectTypeForPrefabs())
                return;

            if (ImGui.RadioButton("Import Prefab##tool_Selection_ImportPrefab", MapEditorState.SelectedAction == MapEditorAction.ImportPrefab))
            {
                MapEditorState.SelectedAction = MapEditorAction.ImportPrefab;
            }
        }

        public static void Configure(ViewportSelection _selection)
        {
            var width = ImGui.GetWindowWidth();
            var height = ImGui.GetWindowHeight();

            var comboMap = MapEditorToolbar._comboTargetMap;
            var universe = MapEditorToolbar._universe;

            if (MapEditorState.SelectedAction == MapEditorAction.ImportPrefab)
            {
                ImGui.Text("Import the selected prefab into a loaded map.");
                ImGui.Text("");

                ImGui.InputText($"Search", ref _searchInput, 255);
                if (_searchInput != _searchInputCache)
                {
                    _searchInputCache = _searchInput;
                }
                ImGui.SameLine();
                if (ImGui.Button($"{ForkAwesome.Eye}##ToggleContentView"))
                {
                    ShowPrefabContents = !ShowPrefabContents;
                }
                ImguiUtils.ShowHoverTooltip("Toggle the visibility of the tags and content section.");
                ImGui.Text("");

                // Prefab Select
                ImGui.BeginChild("##PrefabList_Names", new Vector2((width - 10), (height / 5)));

                DisplayPrefabList();

                ImGui.EndChild();

                if (ShowPrefabContents)
                {
                    ImGui.BeginChild("##PrefabList_Contents", new Vector2((width - 10), (height / 5)));

                    DisplayPrefabContentsList();

                    ImGui.EndChild();
                }

                ImGui.Separator();
                ImGui.Text("Action:");
                ImGui.Separator();

                ImGui.Text("Targeted Map:");
                if (ImGui.BeginCombo("##Targeted Map", comboMap.Item1))
                {
                    foreach (var obj in universe.LoadedObjectContainers)
                    {
                        if (obj.Value != null)
                        {
                            if (ImGui.Selectable(obj.Key))
                            {
                                MapEditorToolbar._comboTargetMap = (obj.Key, obj.Value);
                                break;
                            }
                        }
                    }
                    ImGui.EndCombo();
                }
                ImguiUtils.ShowHoverTooltip("The target map to spawn a prefab in.");
                ImGui.Text("");

                ImGui.Checkbox("Apply Unique Entity ID", ref CFG.Current.Prefab_ApplyUniqueEntityID);
                ImguiUtils.ShowHoverTooltip("Spawned prefab objects will be given unique Entity IDs.");

                if (Project.Type == ProjectType.ER || Project.Type == ProjectType.AC6)
                {
                    ImGui.Checkbox("Apply Unique Instance ID", ref CFG.Current.Prefab_ApplyUniqueInstanceID);
                    ImguiUtils.ShowHoverTooltip("Spawned prefab objects will be given unique Instance IDs.");

                    ImGui.Checkbox("Apply Asset UnkPartNames", ref CFG.Current.Prefab_ApplySelfPartNames);
                    ImguiUtils.ShowHoverTooltip("Spawned prefab objects that are Assets will be given UnkPartNames matching themselves.");
                }

                if (Project.Type == ProjectType.DS3 || Project.Type == ProjectType.SDT || Project.Type == ProjectType.ER || Project.Type == ProjectType.AC6)
                {
                    ImGui.Checkbox("Apply Entity Group ID", ref CFG.Current.Prefab_ApplySpecificEntityGroupID);
                    ImGui.Text("");

                    ImGui.Text("Applied Entity Group ID:");
                    ImGui.InputInt("##entityGroupIdInput", ref CFG.Current.Prefab_SpecificEntityGroupID);
                    ImguiUtils.ShowHoverTooltip("Spawned prefab objects will be given this specific Entity Group ID within an empty Entity Group ID slot.");
                }

                ImGui.Text("");
            }
        }

        public static void Act(ViewportSelection _selection)
        {
            if (MapEditorState.SelectedAction == MapEditorAction.ImportPrefab)
            {
                if (ImGui.Button("Import##action_Selection_ImportPrefab", new Vector2(200, 32)))
                {
                    ImportSelectedPrefab();
                }
            }

        }

        public static void Shortcuts()
        {
            if (MapEditorState.SelectedAction == MapEditorAction.ImportPrefab)
            {
                ImGui.Text($"Shortcut: {ImguiUtils.GetKeybindHint(KeyBindings.Current.Toolbar_ImportPrefab.HintText)}");
            }
        }

        /// <summary>
        /// Import selected prefab.
        /// </summary>
        /// <param name="info"></param>
        public static void ImportSelectedPrefab()
        {
            if (!MapEditorToolbar.IsSupportedProjectTypeForPrefabs())
                return;

            var info = MapEditorToolbar._selectedPrefabInfo;
            var comboMap = MapEditorToolbar._comboTargetMap;
            var universe = MapEditorToolbar._universe;
            var scene = MapEditorToolbar._scene;
            var actionManager = MapEditorToolbar._actionManager;

            switch (Project.Type)
            {
                case ProjectType.AC6:
                    Prefab_AC6.ImportSelectedPrefab(info, comboMap, universe, scene, actionManager);
                    break;
                case ProjectType.ER:
                    Prefab_ER.ImportSelectedPrefab(info, comboMap, universe, scene, actionManager);
                    break;
                case ProjectType.SDT:
                    Prefab_SDT.ImportSelectedPrefab(info, comboMap, universe, scene, actionManager);
                    break;
                case ProjectType.DS3:
                    Prefab_DS3.ImportSelectedPrefab(info, comboMap, universe, scene, actionManager);
                    break;
                case ProjectType.DS2S:
                    Prefab_DS2.ImportSelectedPrefab(info, comboMap, universe, scene, actionManager);
                    break;
                case ProjectType.DS1:
                case ProjectType.DS1R:
                    Prefab_DS1.ImportSelectedPrefab(info, comboMap, universe, scene, actionManager);
                    break;
                default: break;
            }
        }

        public static void DisplayPrefabList()
        {
            ImGui.Separator();
            ImGui.Text("Available Prefabs");
            ImGui.Separator();

            foreach (var info in MapEditorToolbar._prefabInfos)
            {
                var name = info.Name;

                if (SearchFilters.IsSearchMatch(_searchInput, name, name, info.Tags, false, false, true, "_"))
                {
                    if (ImGui.Selectable($"{name}##{name}", MapEditorToolbar._selectedPrefabInfo == info))
                    {
                        MapEditorToolbar._selectedPrefabInfo = info;
                        MapEditorToolbar._newPrefabName = info.Name;
                    }
                }
            }
        }

        public static void DisplayPrefabContentsList()
        {
            var comboMap = MapEditorToolbar._comboTargetMap;
            var prefabInfo = MapEditorToolbar._selectedPrefabInfo;

            if (prefabInfo != null)
            {
                switch (Project.Type)
                {
                    case ProjectType.AC6:
                        MapEditorToolbar._selectedPrefabObjectNames = Prefab_AC6.GetSelectedPrefabObjects(prefabInfo, comboMap);
                        break;
                    case ProjectType.ER:
                        MapEditorToolbar._selectedPrefabObjectNames = Prefab_ER.GetSelectedPrefabObjects(prefabInfo, comboMap);
                        break;
                    case ProjectType.SDT:
                        MapEditorToolbar._selectedPrefabObjectNames = Prefab_SDT.GetSelectedPrefabObjects(prefabInfo, comboMap);
                        break;
                    case ProjectType.DS3:
                        MapEditorToolbar._selectedPrefabObjectNames = Prefab_DS3.GetSelectedPrefabObjects(prefabInfo, comboMap);
                        break;
                    case ProjectType.DS2S:
                        MapEditorToolbar._selectedPrefabObjectNames = Prefab_DS2.GetSelectedPrefabObjects(prefabInfo, comboMap);
                        break;
                    case ProjectType.DS1:
                    case ProjectType.DS1R:
                        MapEditorToolbar._selectedPrefabObjectNames = Prefab_DS1.GetSelectedPrefabObjects(prefabInfo, comboMap);
                        break;
                    default: break;
                }

                // Tags
                ImGui.Separator();
                ImGui.Text("Tags:");
                ImGui.Separator();

                if (prefabInfo.Tags != null)
                {
                    foreach (var tag in prefabInfo.Tags)
                    {
                        ImGui.Text(tag);
                    }
                }

                ImGui.Text("");

                // Contents
                ImGui.Separator();
                ImGui.Text("Contents:");
                ImGui.Separator();
                if (prefabInfo != null)
                {
                    foreach (var name in MapEditorToolbar._selectedPrefabObjectNames)
                    {
                        ImGui.Text(name);
                    }
                }
            }
        }
    }
}
