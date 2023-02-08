namespace GameEngine;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

internal class Shader
{
    public int Handle;

    public Shader(string vertexPath, string fragmentPath)
    {
        var VertexShaderSource = File.ReadAllText(vertexPath);
        var FragmentShaderSource = File.ReadAllText(fragmentPath);

        //nous générons nos shaders et lions le code source aux shaders. 
        var VertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(VertexShader, VertexShaderSource);

        var FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(FragmentShader, FragmentShaderSource);
        
        //nous compilons nos shaders
        GL.CompileShader(VertexShader);
        
        //nous vérifions si la compilation a réussi
        GL.GetShader(VertexShader, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetShaderInfoLog(VertexShader);
            Console.WriteLine(infoLog);
        }

        //nous compilons nos shaders
        GL.CompileShader(FragmentShader);

        //nous vérifions si la compilation a réussi
        GL.GetShader(FragmentShader, ShaderParameter.CompileStatus, out int success1);
        if (success1 == 0)
        {
            string infoLog = GL.GetShaderInfoLog(FragmentShader);
            Console.WriteLine(infoLog);
        }
        
        //nous devons les lier ensemble dans un programme qui peut être exécuté sur le GPU
        Handle = GL.CreateProgram();

        //nous attachons nos shaders au programme
        GL.AttachShader(Handle, VertexShader);
        GL.AttachShader(Handle, FragmentShader);

        //nous lions nos shaders
        GL.LinkProgram(Handle);

        //nous vérifions si la liaison a réussi
        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int success2);
        if (success2 != 0) return;
        {
            string infoLog = GL.GetProgramInfoLog(Handle);
            Console.WriteLine(infoLog);
        }
        
        //nous devrions faire un petit nettoyage. Les shaders de vertex et de fragment individuels sont inutiles maintenant qu'ils ont été liés
        GL.DetachShader(Handle, VertexShader);
        GL.DetachShader(Handle, FragmentShader);
        GL.DeleteShader(FragmentShader);
        GL.DeleteShader(VertexShader);
    }
    
    public void Use() //nous utilisons notre programme
    {
        GL.UseProgram(Handle);
    }
    
    // nous devons nettoyer le handle après la mort de cette classe
    private bool disposedValue = false;

    protected virtual void Dispose(bool disposing)
    {
        if (disposedValue) return;
        GL.DeleteProgram(Handle);

        disposedValue = true;
    }

    ~Shader()
    {
        GL.DeleteProgram(Handle);
    }


    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}