using System.Numerics;
using ImGuiNET;
using Raylib_cs;

public class DebugGame
{
    // DebugGame.cs
    public static void OpenWindow()
    {
        if(!Program.isShowDebug) return;
        ImGui.Begin("Debug Menu");
    }

    public static void CloseWindow()
    {
        if(!Program.isShowDebug) return;
        ImGui.End();
    }

    public static void PlayerSection(float speed, float health, float _stamina, bool _stop, bool _walk, bool _run, Vector2 pos)
    {
        if(ImGui.CollapsingHeader("Player Info"))
        {
            ImGui.Text($"Speed: {speed}");
            ImGui.Text($"Health: {health}");
            ImGui.Text($"Stamina: {_stamina}");
            ImGui.Text($"IsStopped: {_stop}");
            ImGui.Text($"IsWalking: {_walk}");
            ImGui.Text($"IsRunning: {_run}");
        }

        
    }

    public static void InputSection(Vector2 _direction)
    {
        if(ImGui.CollapsingHeader("Player INPUT"))
        {
        ImGui.Text($"Key pressed");
        ImGui.Text($"W: {_direction.Y<0}");
        ImGui.Text($"S: {_direction.Y>0}");
        ImGui.Text($"A: {_direction.X<0}");
        ImGui.Text($"D: {_direction.X>0}");
        }
    }

    public static void HUD_Health(float _endAngle,float _percentage)
    {
        
    }
}

