using System.Numerics;
using Raylib_cs;

public class ShaderEffect
{
    public Shader shaderData;
    Dictionary<string, int> locations = new Dictionary<string, int>();

    public ShaderEffect(string fragmentPath, string? vertexPath = null)
    {
        shaderData = Raylib.LoadShader(vertexPath, fragmentPath);
    }
    
    int GetLocation(string uniformName)
    {
        if(!locations.ContainsKey(uniformName))
            locations[uniformName] = Raylib.GetShaderLocation(shaderData, uniformName);
        
        return locations[uniformName];
    }

    public void SetVector2(string uniformName, Vector2 value)
    {
        Raylib.SetShaderValue(shaderData, GetLocation(uniformName), value, ShaderUniformDataType.Vec2);
    }

    public void SetFloat(string uniformName, float value)
    {
        Raylib.SetShaderValue(shaderData, GetLocation(uniformName), value, ShaderUniformDataType.Float);
    }

    public void Begin() => Raylib.BeginShaderMode(shaderData);
    public void End() => Raylib.EndShaderMode();

    public void Unload() => Raylib.UnloadShader(shaderData);
}