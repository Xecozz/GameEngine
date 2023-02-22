using System;
using System.Reflection.Metadata;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace GameEngine;

public class Texture
{
    private readonly int _handle;
    // texture path
    private const string TexturePath = "../../../Asset/Textures/";

    public Texture(string path)
    {
        // Generate handle
        _handle = GL.GenTexture();

        // Bind the handle
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, _handle);
        
        // Now that our texture is loaded, we can set a few settings to affect how the image appears on rendering.

        // First, we set the min and mag filter. These are used for when the texture is scaled down and up, respectively.
        // Here, we use Linear for both. This means that OpenGL will try to blend pixels, meaning that textures scaled too far will look blurred.
        // You could also use (amongst other options) Nearest, which just grabs the nearest pixel, which makes the texture look pixelated if scaled too far.
        // NOTE: The default settings for both of these are LinearMipmap. If you leave these as default but don't generate mipmaps,
        // your image will fail to render at all (usually resulting in pure black instead).
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        // Now, set the wrapping mode. S is for the X axis, and T is for the Y axis.
        // We set this to Repeat so that textures will repeat when wrapped. Not demonstrated here since the texture coordinates exactly match
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);


        StbImage.stbi_set_flip_vertically_on_load(1);

        // Load the image.
        var image = ImageResult.FromStream(File.OpenRead(TexturePath + path), ColorComponents.RedGreenBlueAlpha);
        
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