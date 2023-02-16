using System;
using System.Reflection.Metadata;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace GameEngine;

public class Texture
{
    private readonly int _handle;

    public Texture(string path)
    {
        // Generate handle
        _handle = GL.GenTexture();

        // Bind the handle
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, _handle);

        StbImage.stbi_set_flip_vertically_on_load(1);

        // Load the image.
        var image = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);
        
        // Now that our pixels are prepared, it's time to generate a texture. We do this with GL.TexImage2D.
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);

        // Next, generate mipmaps.
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
    }

    public void Use(TextureUnit unit)
    {
        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.Texture2D, _handle);
    }
    
}