using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using ShipManifest.InternalObjects;
using ShipManifest.InternalObjects.Settings;
using UnityEngine;

namespace ShipManifest.Windows
{
  internal static class WindowDebugger
  {
    internal static float WindowHeight = 355;
    internal static float HeightScale;
    internal static float ViewerHeight = 300;
    internal static float MinHeight = 200;
    internal static bool ResizingWindow = false;
    internal static Rect Position = CurrSettings.DefaultPosition;
    private static bool _inputLocked;
    private static bool _showWindow;
    internal static bool ShowWindow
    {
      get => _showWindow;
      set
      {
        if (!value)
        {
          InputLockManager.RemoveControlLock("SM_Window");
          _inputLocked = false;
        }
        _showWindow = value;
      }
    }
    internal static string ToolTip = "";
    internal static bool ToolTipActive;
    internal static bool ShowToolTips = true;

    // Content strings
    internal static string Title                = $"{SmUtils.SmTags["#smloc_debug_000"]}:  {SMSettings.CurVersion}";
    internal static GUIContent closeContent     = new GUIContent("", SmUtils.SmTags["#smloc_window_tt_001"]);
    internal static GUIContent clrLogContent    = new GUIContent(SmUtils.SmTags["#smloc_debug_001"]);
    internal static GUIContent saveLogContent   = new GUIContent(SmUtils.SmTags["#smloc_debug_002"]);
    internal static GUIContent closeLogContent  = new GUIContent(SmUtils.SmTags["#smloc_debug_003"]);


    internal static void Display(int _windowId)
    {

      // set input locks when mouseover window...
      _inputLocked = GuiUtils.PreventClickthrough(ShowWindow, Position, _inputLocked);

      // Reset Tooltip active flag...
      ToolTipActive = false;

      Rect rect = new Rect(Position.width - 20, 4, 16, 16);
      if (GUI.Button(rect, closeContent)) // "Close Window"
      {
        ShowWindow = false;
        SMSettings.MemStoreTempSettings();
        ToolTip = "";
      }
      if (Event.current.type == EventType.Repaint && ShowToolTips)
        ToolTip = SMToolTips.SetActiveToolTip(rect, GUI.tooltip, ref ToolTipActive, 10);

      GUILayout.BeginVertical();
      SmUtils.DebugScrollPosition = GUILayout.BeginScrollView(SmUtils.DebugScrollPosition, SMStyle.ScrollStyle,
        GUILayout.Height(ViewerHeight + HeightScale), GUILayout.Width(500));
      GUILayout.BeginVertical();

      List<string>.Enumerator errors = SmUtils.LogItemList.GetEnumerator();
      while (errors.MoveNext())
      {
        if (errors.Current == null) continue;
        GUILayout.TextArea(errors.Current, GUILayout.Width(460));
        
      }
      errors.Dispose();

      GUILayout.EndVertical();
      GUILayout.EndScrollView();

      GUILayout.BeginHorizontal();
      if (GUILayout.Button(clrLogContent, GUILayout.Height(20))) //"Clear log"
      {
        SmUtils.LogItemList.Clear();
        SmUtils.LogItemList.Add($"Info:  Log Cleared at {DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)} UTC.");
      }
      if (GUILayout.Button(saveLogContent, GUILayout.Height(20))) // "Save Log"
      {
        // Create log file and save.
        Savelog();
      }
      if (GUILayout.Button(closeLogContent, GUILayout.Height(20))) // "Close"
      {
        // Create log file and save.
        ShowWindow = false;
        SMSettings.MemStoreTempSettings();
      }
      GUILayout.EndHorizontal();

      GUILayout.EndVertical();
      //resizing
      Rect resizeRect =
        new Rect(Position.width - 18, Position.height - 18, 16, 16);
      GUI.DrawTexture(resizeRect, SmUtils.resizeTexture, ScaleMode.StretchToFill, true);
      if (Event.current.type == EventType.MouseDown && resizeRect.Contains(Event.current.mousePosition))
      {
        ResizingWindow = true;
      }

      if (Event.current.type == EventType.Repaint && ResizingWindow)
      {
        if (Mouse.delta.y != 0)
        {
          float diff = Mouse.delta.y;
          GuiUtils.UpdateScale(diff, ViewerHeight, ref HeightScale, MinHeight);
        }
      }
      //ResetZoomKeys();
      GUI.DragWindow(new Rect(0, 0, Screen.width, 30));
      Position.height = WindowHeight + HeightScale;
      GuiUtils.RepositionWindow(ref Position);
    }

    internal static void Savelog()
    {
      try
      {
        // time to create a file...
        string filename = $"DebugLog_{DateTime.Now.ToString(CultureInfo.InvariantCulture).Replace(" ", "_").Replace("/", "").Replace(":", "")}.txt";

        string path = Directory.GetCurrentDirectory() + @"\GameData\ShipManifest\";
        if (SMSettings.DebugLogPath.StartsWith(@"\\"))
          SMSettings.DebugLogPath = SMSettings.DebugLogPath.Substring(2, SMSettings.DebugLogPath.Length - 2);
        else if (SMSettings.DebugLogPath.StartsWith(@"\"))
          SMSettings.DebugLogPath = SMSettings.DebugLogPath.Substring(1, SMSettings.DebugLogPath.Length - 1);

        if (!SMSettings.DebugLogPath.EndsWith(@"\"))
          SMSettings.DebugLogPath += @"\";

        filename = path + SMSettings.DebugLogPath + filename;
        SmUtils.LogMessage($"File Name = {filename}", SmUtils.LogType.Info, true);

        try
        {
          StringBuilder sb = new StringBuilder();
          List<string>.Enumerator lines = SmUtils.LogItemList.GetEnumerator();
          while (lines.MoveNext())
          {
            if (lines.Current == null) continue;
            sb.AppendLine(lines.Current);
          }
          lines.Dispose();

          File.WriteAllText(filename, sb.ToString());

          SmUtils.LogMessage("File written", SmUtils.LogType.Info, true);
        }
        catch (Exception ex)
        {
          SmUtils.LogMessage($"Error Writing File:  {ex}", SmUtils.LogType.Error, true);
        }
      }
      catch (Exception ex)
      {
        SmUtils.LogMessage($" in Savelog.  Error:  {ex.Message} \r\n\r\n{ex.StackTrace}", SmUtils.LogType.Error,
          true);
      }
    }
  }
}
