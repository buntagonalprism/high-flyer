Open ended VR experience, about emotions.
Do we want to start making Game Breaking Bugs? not going to be much emotion in it. Would this be a good VR game?
Can make a joyous physics and particle effects emitter game, pre-cursor to the Whirling Wizards (vr controllers used to cast spells in wizard duels)

## Musings 30/07/2017
I don't even know what game I'm trying to make here. There isn't really anything much I had in mind apart from the joy of flying around. 

I mean we can do it fully on rails style where it just moves you up or down along the pre-defined path, but where's teh awesome VR fun in that? 
If I start thinking of other projects I'll probably abandone this one. The underwater swimming was a pretty cool idea. 
But again I suppose is the question of what to do in the "game". And I haven't even started having a crack at any of the challenges. 
So what's left to do with the flying? Apart from getting it working how I would like? 

Doing a barrell roll has some weird angle issues coming into play - I reckon the yaw stuff really messes things up (we should probably cut it out beyond a certain angle?)

Anyway, lets make a game. 

Gamification and robustness are the next two important steps:
- Robustness: can't fly through major objects (we'll allow tree-trunks and the like)
- Gamification: while flying is fun, some purpose, like potentially musical notes that play when you fly through rings. Seeing trails would be pretty cool too. 
- 3D modelling & animation: draw up a proper flying machine and rig it so it actually flaps. 
- Sound design: super important - need rushing air over wings, river water running, waterfall falling, volcano rumbling, wind rustling trees, occasional crickets, wind whistling around stone shapes, some grass swaying would be nice, steam vents 
- Use terrain editor to build up a much larger, more performant terrain for the game. 
- Use speed tree or something similar to populate a forest area? Trees at a distance are billboarded (rendered as a sprite facing the user rather than a 3D model), and level of detail rendering is built in (further away trees have less mesh complexity)

# Custom Shaders
I'd like to define a custom shader used to create an air shimmer effect around a column of hot air. Mostly just want to learn a bit about shaders. 

`Shader.DisableKeyword("MY_KEYWORD1")` used to add global shader keywords which can be detected by shader if statements
```
#if defined (MY_KEYWORD1) || defined (MY_KEYWORD2)
sampler2D _ReflectionTex;
#endif
```

## Shader Lab and HLSL
Shader Lab is the language used for writing custom shaders in Unity. 
Shaderlab is actually a wrapper language used for collecting together information about the shader including its editable properties which can be set whenever a material is created with this shader. The actual lighting calculations are written in snippets called CGPROGRAM snippets inside the shaderlab code. CGPROGRAM snippets are so called because they are written in the HSLS/Cg shading language. Cg is short for C for graphics, a shading language similar to C but with better support for programming GPUs, developed by Microsoft and Nvidia. Cg itself however is now deprecated, but HLSL - the High Level Shading Language developed by Microsoft to power DirectX 9+, was designed in parallel with it an bears many similarities. So for all practical purposes, the actual shader logic is written in HSLS (similar to the Open Graphics Language (OpenGL) equivalent GLSL), and for legacy purposes those snippets are called CGPROGRAM

So with that history in mind lets have a look at a shader file. 

## Shader Code File Structure
Basic shader lab syntax:
```
Shader "my shader name" { [Properties] Subshaders [Fallback] [CustomEditor] }
```
- The first in the file should be "Shader" and declares a shader...wowzers that was a tough one. 
- Followed by the name of the shader in double quotes (may inculde spaces)
- Properties (optional): a list of properties of the shader which may be configured when creating a material using this shader. Should specify a human readable name, a variable name, a data type, a default value, and possibly a range of allowed values. 
- SubShaders (at least one is required): Each subshader contains rendering logic. When there are multiple subshaders, Unity will attempt to use the first subshader supported by the user's hardware. This allows us to have a complex shader listed first for powerful machines, and simpler shaders defined after it suitable for running on lower-power computers
- Fallback (optional): if no sub-shaders are supported by the current hardware, then Unity can use a different shader altogether in place of this one. Use the name of another shader to specify as the fallback if this one is not supported. May also state "Fallback Off" to directly declare there are no fallbacks and don't print a warning. 
- CustomEditor (optional): a custom editor is a C# class that defines a GUI for editing the properties of a shader. Use this if there are complex properties in the custom shader that need a custom GUI to configure - more than what the standard Unity shader properties layout can provide. 

The shader listed below is a fully defined shader including the optional properties and one SubShader. Further subshaders would be declared with a whole additional "SubShader {}" block immediately following the existing one. 
```
// colored vertex lighting
Shader "Simple colored lighting"
{
    // a single color property
    Properties {
        _Color ("Main Color", Color) = (1,.5,.5,1)
    }
    // define one subshader
    SubShader
    {
        // a single pass in our subshader
        Pass
        {
            // use fixed function per-vertex lighting
            Material
            {
                Diffuse [_Color]
            }
            Lighting On
        }
    }
    // A fallback shader identified by name
    Fallback "Really simple lighting"

    // A custom shader properties editor to display in the Unity inspector for this shader
    CustomEditor "MyCustomShaderGuiCSharpClass"
}
```

## Shader Properties
Pretty much straight from the Unity docs, here are the types of properties which can be defined for a shader and how they are represented in the inspector when setting properties of this shader in a material. 

**Basic Properties**
```
name ("display name", Range (min, max)) = number    // Displayes as a slider between min and max, defaults to number
name ("display name", Float) = number               // Displays as single editable field, defaults to number
name ("display name", Int) = number                 // Displays as single editable field, defaults to number
```

**Colours and Vectors**
```
// Colours display a colour-picker field in the inspector, or default to (R,G,B,A) 
name ("display name", Color) = (number,number,number,number) 

// Vectors are always 4D, and display as four editable fields
name ("display name", Vector) = (number,number,number,number)
```

**Textures**
```
// 2D property type will accept any standard 2D image texture. "defaulttexture" is normally an empty string or one of "white, black, grey, bump (0.5, 0.5, 1, 0.5) or red" to specify a placeholder. 
name ("display name", 2D) = "defaulttexture" {}

// Cube property type is specifically looking for an image texture of a cubemap type - i.e. a flattened image comprising of six regions used to represent surroundings of an object, e.g. a skybox around the entire game, or reflections of a polished metal ball. "defaulttexture" should be an empty string
name ("display name", Cube) = "defaulttexture" {}

// Special kind of texture - the 3D texture created from script. This sounds like a bag of worms best left alone for now.  "defaulttexture" should be an empty string.
name ("display name", 3D) = "defaulttexture" {}
```

By convention, Unity shader properties begin with an __underscore__

## SubShader Syntax

[SubShaders](https://docs.unity3d.com/Manual/SL-SubShader.html)

[Passes](https://docs.unity3d.com/Manual/SL-Pass.html)



## Shader Types
Reckon this [shader tutorial](https://docs.unity3d.com/Manual/ShadersOverview.html) could be helpful. This tutorial includes links at the bottom more details than the reference guides for each shader type below. 
Shaders in Unity can be written in three ways:
1. [Surface Shaders](https://docs.unity3d.com/Manual/SL-SurfaceShaders.html)
2. [Vertex and Fragment shaders](https://docs.unity3d.com/Manual/SL-ShaderPrograms.html)
3. ShaderLab and Fixed function shaders

## HSLS & CGPROGRAM
The previous basic shader example used pure shaderlab syntax

### UV Mapping
Done by a 3D artist in a 3D design application, maps mesh vertexes or faces to UV coordinates in a texture image file. This tells each part of the mesh what colour it should be with reference to a texture file. 