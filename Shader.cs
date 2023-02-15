using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GameEngine;

internal sealed class Shader
{
    public int Handle;

    public Shader(string vertexPath, string fragmentPath)
    {
        //get source shaders (.glsl)
        var VertexShaderSource = File.ReadAllText(vertexPath);
        var FragmentShaderSource = File.ReadAllText(fragmentPath);

        //create shaders and link them 
        var VertexShader = GL.CreateShader(ShaderType.VertexShader);
        var FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        
        GL.ShaderSource(VertexShader, VertexShaderSource);
        GL.ShaderSource(FragmentShader, FragmentShaderSource);
        
        //compile shaders Vertex and Fragment
        GL.CompileShader(VertexShader);
        Console.WriteLine(GL.GetShaderInfoLog(VertexShader));
        
        GL.CompileShader(FragmentShader);
        Console.WriteLine(GL.GetShaderInfoLog(FragmentShader));
        
        //create program for execute shaders name Handle
        Handle = GL.CreateProgram();

        //attach shaders to Handle
        GL.AttachShader(Handle, VertexShader);
        GL.AttachShader(Handle, FragmentShader);

        //link program
        GL.LinkProgram(Handle);
        Console.WriteLine(GL.GetProgramInfoLog(Handle));

        //clear shaders because is link to Handle
        GL.DetachShader(Handle, VertexShader);
        GL.DetachShader(Handle, FragmentShader);
        GL.DeleteShader(FragmentShader);
        GL.DeleteShader(VertexShader);
    }
    
    public void Use() //nous utilisons notre programme
    {
        GL.UseProgram(Handle);
    }
    
    
    // clear memory after use it (after dead of object)
    private bool disposedValue = false;

    private void Dispose(bool disposing)
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