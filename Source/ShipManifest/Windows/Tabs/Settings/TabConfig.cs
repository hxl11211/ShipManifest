using ShipManifest.APIClients;
using ShipManifest.InternalObjects;
using ShipManifest.InternalObjects.Settings;
using UnityEngine;

namespace ShipManifest.Windows.Tabs.Settings
{
  internal static class TabConfig
  {
    internal static string TxtSaveInterval = CurrSettings.SaveIntervalSec.ToString();

    // GUI tooltip and label support
    private static Rect _rect;
    private const float guiRuleWidth = 350;
    private const float guiMaintoggleWidth = 300;

    internal static string ToolTip = "";
    internal static bool ToolTipActive;
    internal static bool ShowToolTips = true;
    private static bool _canShowToolTips = true;

    internal static Rect Position = WindowSettings.Position;

    // Content strings
    internal static GUIContent titleContent         = new GUIContent(SmUtils.SmTags["#smloc_settings_config_000"]);
    internal static GUIContent blizzyContent        = new GUIContent(SmUtils.SmTags["#smloc_settings_config_001"], SmUtils.SmTags["#smloc_settings_config_tt_001"]);
    internal static GUIContent unityStyleContent    = new GUIContent(SmUtils.SmTags["#smloc_settings_config_002"], SmUtils.SmTags["#smloc_settings_config_tt_002"]);
    internal static GUIContent debugContent         = new GUIContent(SmUtils.SmTags["#smloc_settings_config_003"], SmUtils.SmTags["#smloc_settings_config_tt_003"]);
    internal static GUIContent loggingContent       = new GUIContent(SmUtils.SmTags["#smloc_settings_config_004"], SmUtils.SmTags["#smloc_settings_config_tt_004"]);
    internal static GUIContent smErrorContent       = new GUIContent(SmUtils.SmTags["#smloc_settings_config_005"], SmUtils.SmTags["#smloc_settings_config_tt_005"]);
    internal static GUIContent logExitContent       = new GUIContent(SmUtils.SmTags["#smloc_settings_config_006"], SmUtils.SmTags["#smloc_settings_config_tt_006"]);
    internal static GUIContent logLengthContent     = new GUIContent($"{SmUtils.SmTags["#smloc_settings_config_007"]}:", SmUtils.SmTags["#smloc_settings_config_tt_007"]);
    internal static GUIContent linesContent         = new GUIContent(SmUtils.SmTags["#smloc_settings_config_011"]);
    internal static GUIContent autoSaveContent      = new GUIContent(SmUtils.SmTags["#smloc_settings_config_008"], SmUtils.SmTags["#smloc_settings_config_tt_008"]);
    internal static GUIContent saveIntervalContent  = new GUIContent($"{SmUtils.SmTags["#smloc_settings_config_009"]}:", SmUtils.SmTags["#smloc_settings_config_tt_009"]);
    internal static GUIContent secondsContent       = new GUIContent(SmUtils.SmTags["#smloc_settings_config_010"]);


    internal static void Display(Vector2 displayViewerPosition)
    {
      // Reset Tooltip active flag...
      ToolTipActive = false;
      _canShowToolTips = WindowSettings.ShowToolTips && ShowToolTips;

      Position = WindowSettings.Position;
      int scrollX = 20;

      //Configuration Title
      GUILayout.Label(titleContent, SMStyle.LabelTabHeader);
      GUILayout.Label(WindowSettings.TabRule, SMStyle.LabelStyleHardRule, GUILayout.Height(10), GUILayout.Width(guiRuleWidth));

      if (!ToolbarManager.ToolbarAvailable)
      {
        if (CurrSettings.EnableBlizzyToolbar)
          CurrSettings.EnableBlizzyToolbar = false;
        GUI.enabled = false;
      }
      else
        GUI.enabled = true;

      // Enable Blizzy Toolbar (Replaces Stock Toolbar)";
      CurrSettings.EnableBlizzyToolbar = GUILayout.Toggle(CurrSettings.EnableBlizzyToolbar, blizzyContent, GUILayout.Width(guiMaintoggleWidth));
      _rect = GUILayoutUtility.GetLastRect();
      if (Event.current.type == EventType.Repaint && _canShowToolTips)
        ToolTip = SMToolTips.SetActiveToolTip(_rect, GUI.tooltip, ref ToolTipActive, scrollX);

      GUI.enabled = true;
      // UnityStyle Mode
      CurrSettings.UseUnityStyle = GUILayout.Toggle(CurrSettings.UseUnityStyle, unityStyleContent, GUILayout.Width(guiMaintoggleWidth));
      _rect = GUILayoutUtility.GetLastRect();
      if (Event.current.type == EventType.Repaint && _canShowToolTips)
        ToolTip = SMToolTips.SetActiveToolTip(_rect, GUI.tooltip, ref ToolTipActive, scrollX);
      if (CurrSettings.UseUnityStyle != OrigSettings.UseUnityStyle)
        SMStyle.WindowStyle = null;

      // Enable Debug Window;
      WindowDebugger.ShowWindow = GUILayout.Toggle(WindowDebugger.ShowWindow, debugContent, GUILayout.Width(guiMaintoggleWidth));
      _rect = GUILayoutUtility.GetLastRect();
      if (Event.current.type == EventType.Repaint && _canShowToolTips)
        ToolTip = SMToolTips.SetActiveToolTip(_rect, GUI.tooltip, ref ToolTipActive, scrollX);

      // Enable Verbose Logging
      CurrSettings.VerboseLogging = GUILayout.Toggle(CurrSettings.VerboseLogging, loggingContent, GUILayout.Width(guiMaintoggleWidth));
      _rect = GUILayoutUtility.GetLastRect();
      if (Event.current.type == EventType.Repaint && _canShowToolTips)
        ToolTip = SMToolTips.SetActiveToolTip(_rect, GUI.tooltip, ref ToolTipActive, scrollX);

      // Enable SM Debug Window On Error
      CurrSettings.AutoDebug = GUILayout.Toggle(CurrSettings.AutoDebug, smErrorContent, GUILayout.Width(guiMaintoggleWidth));
      _rect = GUILayoutUtility.GetLastRect();
      if (Event.current.type == EventType.Repaint && _canShowToolTips)
        ToolTip = SMToolTips.SetActiveToolTip(_rect, GUI.tooltip, ref ToolTipActive, scrollX);

      // Save Error log on Exit
      CurrSettings.SaveLogOnExit = GUILayout.Toggle(CurrSettings.SaveLogOnExit, logExitContent, GUILayout.Width(guiMaintoggleWidth));
      _rect = GUILayoutUtility.GetLastRect();
      if (Event.current.type == EventType.Repaint && _canShowToolTips)
        ToolTip = SMToolTips.SetActiveToolTip(_rect, GUI.tooltip, ref ToolTipActive, scrollX);

      // create Limit Error Log Length slider;
      GUILayout.BeginHorizontal();
      //Error Log Length:
      GUILayout.Label(logLengthContent, GUILayout.Width(110));
      _rect = GUILayoutUtility.GetLastRect();
      if (Event.current.type == EventType.Repaint && _canShowToolTips)
        ToolTip = SMToolTips.SetActiveToolTip(_rect, GUI.tooltip, ref ToolTipActive, scrollX);
      CurrSettings.ErrorLogLength = GUILayout.TextField(CurrSettings.ErrorLogLength, GUILayout.Width(40));
      GUILayout.Label(linesContent, GUILayout.Width(50));
      GUILayout.EndHorizontal();

      // Enable AutoSave Settings
      CurrSettings.AutoSave = GUILayout.Toggle(CurrSettings.AutoSave, autoSaveContent, GUILayout.Width(guiMaintoggleWidth));
      _rect = GUILayoutUtility.GetLastRect();
      if (Event.current.type == EventType.Repaint && _canShowToolTips)
        ToolTip = SMToolTips.SetActiveToolTip(_rect, GUI.tooltip, ref ToolTipActive, scrollX);

      GUILayout.BeginHorizontal();
      GUILayout.Label(saveIntervalContent, GUILayout.Width(110));
      _rect = GUILayoutUtility.GetLastRect();
      if (Event.current.type == EventType.Repaint && _canShowToolTips)
        ToolTip = SMToolTips.SetActiveToolTip(_rect, GUI.tooltip, ref ToolTipActive, scrollX);
      TxtSaveInterval = GUILayout.TextField(TxtSaveInterval, GUILayout.Width(40));
      GUILayout.Label(secondsContent, GUILayout.Width(40));
      GUILayout.EndHorizontal();
    }
  }
}
