using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GameEngine;

internal class Shader
{
    public readonly int Handle;
    // shader path
    private const string ShaderPath = "../../../Asset/Shaders/";

    public Shader(string vertexPath, string fragmentPath)
    {
        //get source shaders (.glsl)
        var vertexShaderSource = File.ReadAllText(ShaderPath +vertexPath);
        var fragmentShaderSource = File.ReadAllText(ShaderPath + fragmentPath);

        //create shaders and link them 
        var vertexShader = GL.CreateShader(ShaderType.VertexShader);
        var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        
        GL.ShaderSource(vertexShader, vertexShaderSource);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);
        
        //compile shaders Vertex and Fragment
        GL.CompileShader(vertexShader);
        Console.WriteLine(GL.GetShaderInfoLog(vertexShader));
        
        GL.CompileShader(fragmentShader);
        Console.WriteLine(GL.GetShaderInfoLog(fragmentShader));
        
        //create program for execute shaders name Handle
        Handle = GL.CreateProgram();

        //attach shaders to Handle
        GL.AttachShader(Handle, vertexShader);
        GL.AttachShader(Handle, fragmentShader);

        //link program
        GL.LinkProgram(Handle);
        Console.WriteLine(GL.GetProgramInfoLog(Handle));

        //clear shaders because is link to Handle
        GL.DetachShader(Handle, vertexShader);
        GL.DetachShader(Handle, fragmentShader);
        GL.DeleteShader(fragmentShader);
        GL.DeleteShader(vertexShader);
    }
    
    public void Use() // use programm
    {
        GL.UseProgram(Handle);
        
    }
    
    public int GetAttribLocation(string attribName)
    {
        return GL.GetAttribLocation(Handle, attribName);
    }
    
    public void SetInt(string name, int value)
    {
        int location = GL.GetUniformLocation(Handle, name);

        GL.Uniform1(location, value);
    }
    
    public void SetMatrix4(string name, Matrix4 data)
    {
        GL.UseProgram(Handle);
        int location = GL.GetUniformLocation(Handle, name);
        GL.UniformMatrix4(location, true, ref data);
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