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

So with that history in mind lets have a look at a shader file. Most of what follows up until we actually get to HLSL is all shader-lab syntax. 

## ShaderLab Code File Structure
Basic shader lab syntax:
```
Shader "my shader name" { [Properties] Subshaders [Fallback] [CustomEditor] }
```
- The first in the file should be "Shader" and declares a shader...wowzers that was a tough one. 
- Followed by the name of the shader in double quotes (may inculde spaces). If it includes a slash, before the slash is a shader group, and after is the actual shader name "Particles/My Particle Shader" would add "My Particle Shader" to the existing "Partciles" group of shaders. 
- Properties (optional): a list of properties of the shader which may be configured when creating a material using this shader. Should specify a human readable name, a variable name, a data type, a default value, and possibly a range of allowed values. 
- SubShaders (at least one is required): Each subshader contains rendering logic. When there are multiple subshaders, Unity will attempt to use the first subshader supported by the user's hardware. This allows us to have a complex shader listed first for powerful machines, and simpler shaders defined after it suitable for running on lower-power computers
- Fallback (optional): if no sub-shaders are supported by the current hardware, then Unity can use a different shader altogether in place of this one. Use the name of another shader to specify as the fallback if this one is not supported. May also state "Fallback Off" to directly declare there are no fallbacks and don't print a warning. 
- CustomEditor (optional): a custom editor is a C# class that defines a GUI for editing the properties of a shader. Use this if there are complex properties in the custom shader that need a custom GUI to configure - more than what the standard Unity shader properties layout can provide. 

### Simple Example Shader
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

### Shader Properties
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
// 2D property type will accept any standard 2D image texture. "defaulttexture" is normally an empty string or one of "white, black, grey, bump (0.5, 0.5, 1, 0.5) or red" to specify a placeholder. Bump is used to indicate that the texture is intended to be used for normal mapping
name ("display name", 2D) = "defaulttexture" {}

// Cube property type is specifically looking for an image texture of a cubemap type - i.e. a flattened image comprising of six regions used to represent surroundings of an object, e.g. a skybox around the entire game, or reflections of a polished metal ball. "defaulttexture" should be an empty string
name ("display name", Cube) = "defaulttexture" {}

// Special kind of texture - the 3D texture created from script. This sounds like a bag of worms best left alone for now.  "defaulttexture" should be an empty string.
name ("display name", 3D) = "defaulttexture" {}
```

By convention, Unity shader properties begin with an __underscore__

### SubShader Syntax
A subshader in a shader definition consists of:
- Tags (optional): Key-value pairs of strings. There are a number of unity defined ones which affect how the SubShader is rendered, and we can define custom ones to provide additional configuration variables. 
- Common / Shared State (optional): Anything which is valid to go inside a Pass definition can also be put outside a definition within the top level of the subshader - any outputs from this section are shared between all subsequent passes, good for re-using common resources
- At least one pass definition: Describes what should happen in the pass
```
Subshader { [Tags] [CommonState] Passdef [Passdef ...] }
```

#### SubShader Tags
A few quick examples of interesting tags:
- Specify a rendering order by specifying a rendering queue. For example, if this shader supports transparency objects using the shader should be rendered after opaque objects to prevent double-handling. Or if this shader causes a post-processing image affect, like lense flares, it can go in the overlay queue, which is last. We can also go after or before the default queues as well. 
- ForceNoShadowCasting - can make sure shadows are not cast which is good if this shader might be used as a fallback for another which did cast shadows, to make sure shadows are turned off. 
```
SubShader {
    Tags { "Queue" = "Transparent" }
    Pass { ... }
```

#### Simple SubShader Example
The following shader will make a mesh look exactly like its texture file _MainTex indicates. Recall that with UV mapping, each vertex in our mesh is mapped to a pixel in the texture image. This simple shader completely ignores lighting and so vertexes using this shader will end up on the screen with the exact same RGB colour as whatever is contained in the texture. 
```
SubShader {
    Pass {
        Lighting Off
        SetTexture [_MainTex] {}
    }
}
```

### Passes
A single pass causes all geometry for an object using this material to be rendered once. This is where the bulk of our shader definitions actually live - all the stuff we've touched so far is mostly just metadata. We've said that there can be multiple passes - this basically means multiple renders for the same object - typically used to build complex layered lighting effects. 

The components are fairly simple:
- Name (optional): allows another pass to reference this one by name
- Tags (optional): configuration for the pass
- Render Setup (optional): specify configuration for graphics hardware
- CGPROGRAM (optional): HLSL code for what the shader does
```
Pass { [Name and Tags] [RenderSetup] [CGPROGRAM]}
```
Most of the information put in a pass is either configuration for the standard lighting engine, or setup for custom shader behaviour implemented in HLSL. 

#### Pass Render Set-Up
There seems to be a bunch of stuff you can do, but I don't know what it does. Reference [here](https://docs.unity3d.com/Manual/SL-Pass.html).

#### Pass Tags
Passes can also have their own tags, seperate to the subshader tags. They are still string key-value pairs. The most interesting one seems to be the LightMode tag, which specifies where the pass lands in the rendering pipeline - e.g. in forward / deferred rendering, when rendering shadows, etc. 

#### Pass Types
1. Standard Pass: Renders an object mesh to screen pixels
2. UsePass: Render using another pass referred to by name
3. GrabPass: Grab current screen data into a texture so that another pass can use it - typically for post-processing

### Shader Types
So it would seem that this should have been covered previously when we were talking about shaders, but having the background about passes was helpful. We're now going to start looking into the meat of shader implementation. Remember, everything we've seen so far is ShaderLab syntax - primarily a wrapper configuration language for shaders. 

So let's start getting into the guts. 

Shaders in Unity can be written in three ways:
1. Surface Shaders: a good option for materials that receive lighting and cast shadows. You can write a few lines of custom HLSL and then the surface shader will be compiled into vertex/fragment shaders, auto-generating the extra code required. However, if lighting is not required, like special-effects or post-processing (e.g. lens-flare) the default inclusion of lighting calculations in surface shaders is sub-optimal. 
2. Vertex and Fragment Shaders: the true building blocks - these shaders directly specify how vertexes and fragments of mesh should appear on the screen. This allows complete fined-grained control for peak flexibility, but this means a lot more code required and its a lot harder to have it interact with the lighting pipeline (which surface shaders handle automatically). Without the lighting pipeline, we loose shadows and different apperance under different lights. 
3. ShaderLab / Fixed function Shaders: These are a legacy syntax for writing shaders. They are written directly in ShaderLab itself but do not allow much flexibility. As they name implies they are "fixed function" - there are only a set number of commands which can be used with simple input parameters and no custom math is permitted. 

#### Fixed Function Shaders
Let's start with the basics. Turns out the super-basic "simple coloured lighting" shader from [earlier](#simple-example-shader) was a fixed function shader. The reason that fixed-function exist harks back to older GPUs where each of these functions had its own dedicated piece of silicon on the GPU designed to perform only that specific computation. Newer GPUs are now much more powerful and more generic so the need for fixed functions has disappeared. 

Fixed functions are not technically a shader - they just specified an operation to perform and let the silicon do its built-in math. Shaders by contrast specify what math to actually perform and are executed on general-purpose GPU computing hardware. General purpose in the sense it can handle custom math routines - but typically with lots of graphics-specific optimisations. 

So they are very much deprecated apart from on really old desktop graphics cards and some low-power mobile devices, so probably not worth really looking in to, but just for reference here's the full shader code for the "Mobile/VertexLit" shader built in to Unity, which is a fixed function shader. 

```
Shader "VertexLit" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,0.5)
        _SpecColor ("Spec Color", Color) = (1,1,1,1)
        _Emission ("Emmisive Color", Color) = (0,0,0,0)
        _Shininess ("Shininess", Range (0.01, 1)) = 0.7
        _MainTex ("Base (RGB)", 2D) = "white" { }
    }

    SubShader {
        Pass {
            Material {
                Diffuse [_Color]
                Ambient [_Color]
                Shininess [_Shininess]
                Specular [_SpecColor]
                Emission [_Emission]
            }
            Lighting On
            SeparateSpecular On
            SetTexture [_MainTex] {
                constantColor [_Color]
                Combine texture * primary DOUBLE, texture * constant
            }
        }
    }
}
```
I think fixed function means it uses commands fairly similar to those directly implemented in OpenGL hardware. Here's the full text from the tutorial, which pretty much sums it up:

"In the example above we have a Material block that binds our property values to the fixed function lighting material settings. The command Lighting On turns on the standard vertex lighting, and SeparateSpecular On enables the use of a separate color for the specular highlight.

All of these command so far map very directly to the fixed function OpenGL/Direct3D hardware model. Consult OpenGL red book for more information on this.

The next command, SetTexture, is very important. These commands define the textures we want to use and how to mix, combine and apply them in our rendering. SetTexture command is followed by the property name of the texture we would like to use (_MainTex here) This is followed by a combiner block that defines how the texture is applied. The commands in the combiner block are executed for each pixel that is rendered on screen.

Within this block we set a constant color value, namely the Color of the Material, _Color. We’ll use this constant color below.

In the next command we specify how to mix the texture with the color values. We do this with the Combine command that specifies how to blend the texture with another one or with a color. Generally it looks like this: Combine ColorPart, AlphaPart

Here ColorPart and AlphaPart define blending of color (RGB) and alpha (A) components respectively. If AlphaPart is omitted, then it uses the same blending as ColorPart.

In our VertexLit example: Combine texture * primary DOUBLE, texture * constant

Here texture is the color coming from the current texture (here _MainTex). It is multiplied (*) with the primary vertex color. Primary color is the vertex lighting color, calculated from the Material values above. Finally, the result is multiplied by two to increase lighting intensity (DOUBLE). The alpha value (after the comma) is texture multiplied by constant value (set with constantColor above). Another often used combiner mode is called previous (not used in this shader). This is the result of any previous SetTexture step, and can be used to combine several textures and/or colors with each other."

Nothing too complex all up, although we're short on a reference API (the tutorial page has a dead link to an OpenGL red book). I suppose the reference would be whatever functions were defined in the OpenGL or DirectX specifications - but at the time it would have been pretty vendor specific as to what operations were supported given they each needed custom silicon.  

[Tutorial on Fixed Function Shaders](https://docs.unity3d.com/Manual/ShaderTut1.html) 

## HLSL
We've only covered one of the three shader types so far - the fixed function shader, written entirely in ShaderLab. The other two shader types - vertex and fragment, and surface, require HLSL. In fact, more than require HLSL - each type of shader is defined by the presence of certain HLSL commands within the subshader pass. This is a point worth repeating, the only difference between vertex/fragment shaders and surface shaders is the use of different core functions in our HLSL code. But these functions operate so differently they are treated as entirely different shader types. 

Let's dive into vertex and fragment shaders to get started learning the HLSL syntax at the same time. 

### Vertex and Fragment Shaders
Here's where things start to get interesting. 
* **Vertex** shaders determine how mesh vertexes end up located on the screen - they are responsible for the projection from world space to screen space. We may use them to apply distortion to the shape of the mesh and project it in different ways, such as a vertigo affect for example. 
* **Fragment** shaders shade a fragment - i.e. a mesh triangle formed between vertexes projected onto the screen. The shading of the fragment involves the actual determination of a RGB colour and texture values value for each pixel. 

### Rendering Pipeline
Recall the processing pipeline involves:
- Object projection to screen space
- Creation of triangles between vertexes
- Clipping of objects outside the view window
- Rasterisation of triangles (fragments) to pixels
- Colouring of the fragments (with optimisations along the way)

### Rendering Order and Z-Test
We mentioned that subshader tags could be used to define a rendering order. The rendering order specifies how triangles (fragments) are converted to pixels and coloured. The "Queue" tag in a subshader specifies where in the colouring pipeline a particular object will be rendered. The queue is a number, from 0 up to the several thousands, with objects in lower queue numbers rendered before objects with higher numbers. There is no guarantee of rendering order between different objects with the same rendering queue. 

A number of keywords provide helpful shortcuts for default rendering queues which map to fixed queue values.
1. Background (1000): used for backgrounds and skyboxes,
2. Geometry (2000): the default label used for most solid objects,
3. Transparent (3000): used for materials with transparent properties, such glass, fire, particles and water;
4. Overlay (4000): used for effects such as lens flares, GUI elements and texts.

In addition to using `Tags { "Queue" = "2"}` or `Tags { "Queue" = "Geometry"}` we can also use relative indexing to run just before or just after the standard queues: e.g. `Tags { "Queue" = "Geometry+2"}`

The Z-Test is used to avoid rendering objects which cannot be seen, by accumulating a buffer of the closest fragments to the screen. This can be used to discard fragments further away - i.e. obscured by closer ones. We may need to bear this in mind when attempting to create transparent shaders. Recall the Pass definition RenderSetup section allowed for definition of a ZTest depth buffer testing mode - how that pass should overwrite objects in its Z buffer. (I wonder if we could use this to reverse the ordering - so further away objects are shown instead). 

### HLSL Syntax for Vertex/Fragment Sahder
Here is a simple HLSL vertex/fragment shader which colours an object pure red - appearing as a shillouette only, with no depth or lighting. Imporant points of note:
1. Begin and end the HLSL snippet inside the subshader pass with CGPROGRAM and ENDCG statements. As per our previously discussed history, this naming refers to the ancestor language of HLSL, called Cg - hence CGPROGRAM
2. Use the C-style compiler directive "#pragma" to tell the compiler to compile the function call "vert" as a vertex shader, and the function "frag" as a fragment shader. The names "vert" and "frag" are used by convention, but are not necessary. **It is the inclusion of these shader functions which defines a vertex/fragment shader**
3. Recall that #pragma is just a generic C construct (original C) for telling the compiler something - called compilation directives. Other compilation directives are optional but may be beneficial, such as specificing the compilation target in terms of directX shader models. 2.5 is the second lowest, and Unity's default. However other preprocessor directives may require a higher compilation level and cause this setting to be ignored
4. Define a custom object structure using "struct" rather like the objects we know from object oriented programming, only structs cannot have methods, they are just data containers. 
5. Specify variables using the syntax `type name : init_value;`, where:
    * "type": is the data type - see below for more details
    * "name": is the variable name (inside our struct, in this case)
    * ": init_value" is an optional inclusion using something called a **binding semantic** - the notation of the colon followed by a name. In the case of variable declarations, the binding semantic initialises the value, but it can have other uses. More on this later. 
6. Specify the vertex shader function in format `output_type functionName(input_arguments)`. We can easily define our own functions using this format earlier in the code and re-use them if we wish. The vertex shader function is responsible for taking mesh vertexes as inputs and projecting them to screen coordinates, which it outputs. 
7. Specify the fragment shader function. Note that it normally takes an input data type as the output from the vertex shader, and the function itself is annotated with a binding semantic - which in this case describes the function output. 

```
Pass {
    CGPROGRAM
    
    #pragma vertex vert             
    #pragma fragment frag
    #pragma target 2.5

    struct vertInput {
        float4 pos : POSITION;
    };  
    
    struct vertOutput {
        float4 pos : SV_POSITION;
    };
    
    vertOutput vert(vertInput input) {
        vertOutput o;
        o.pos = mul(UNITY_MATRIX_MVP, input.pos);
        return o;
    }
    
    half4 frag(vertOutput output) : COLOR {
        return half4(1.0, 0.0, 0.0, 1.0); 
    }
    ENDCG
}
```
### Data Types
A full list of data types can be found in the [Microsoft HLSL Documentation](https://msdn.microsoft.com/en-us/library/windows/desktop/bb509587(v=vs.85).aspx)

HLSL is a little different from a general-purpose programming language in that it is designed for GPU shader programs. GPU computations are optimised to work on multiple bits of data in parallel, especially matrix transformation and quaternion math. Consequently, in addition to bools, ints, uints (unsigned integer), floats (standard floating point precision - 32bit), doubles (64bit floating point), and halfs (16 bit floating point), we also have built-in vector and matrix types.

We already saw float4 - this is a vector of floats of length four. They can be declared either with a shortened name, or in a data-type and size format, and are initialised with curly braces:
```
int1 vectorLength1 = 1; // Scalar vector
float2 floatLength2 = {0.73f, 1.24f}; // Two-float array. 'f' denotes floating point precision
vector<double 4> doubleLength4 = { 0.2, 0.3, 0.4, 0.5 }; // 4 double vector
```
Note that the maximum size that can be declared this way is 4. 

There are also matrix types, again which only support up to 4x4, and can be declared either with shortened names, or with explicit syntax. 
```
float2x2 fMatrix = { 0.0f, 0.1, // row 1
                     2.1f, 2.2f // row 2 };   
matrix <float, 2, 2> fMatrix = { 0.0f, 0.1, // row 1
                                 2.1f, 2.2f // row 2
                               };
```
We can index out of vectors using either (x,y,z,w) space, or (r,g,b,a) space to represent the two common use cases; 3D positions or colour with alpha. The two spaces can be used interchangeably, just not on the same line. Matrixes can be indexed by row-column index position, either zero or 1 indexed. We can have multiple indexes in a row to slice vectors, and combine them in intialisers. 
```
vector<double 4> myData4 = { 0.2, 0.3, 0.4, 0.5 }; 
double d;
d = myData4.x; // Returns 0.2
d = myData4.g; // Returns 0.3
double2 d2;
d2 = myData4.xz; // Creates vector2 {0.2, 0.4}
double4 d4;
d4 = double4(myData4.xz, myData4.y, 0); // Combine sub-vector, scalar variable, and fixed value

matrix <float, 2, 2> fMatrix = { 0.0f, 0.1f, 
                                 2.1f, 3.3f};
d = fMatrix.m01; // Zero-indexed, returns 0.1f
d2 = fMatrix.11_21 // 1-indexed, returns }0.0f, 2.1f}
```

This being a graphics handling language, there are also Sampler, Shader, and Texture data types. I don't know quite what the sampler and shader types are used for, but the Texture type is pretty common - such as Texture2D, used to represent parts of texture image files. 


### Binding Semantics
This is one of the thorniest bits of the vertex/fragment shader puzzle, the binding semantics are crucial to making sure our code does what it needs to do - both in terms of getting the right input data and making sure our output data is used in the right way. In fact, that's the general idea - the binding semantics tell the compiler what that variable is supposed to do. 
The messy part is that we can use them on both variables and functions, and that they can have different affects when being used for input or output. There are four different cases in which binding semantics may apply. 
1. Input to the vertex shader function
2. Output from the vertex shader function
3. Input to the fragment shader function
4. Output from the fragment shader function 

These cases both restrict the semantics that are valid to use in each case, and also change the behaviour. All inputs to a vertex shader function must be annotated with semantics, describing how the GPU should initialise that variable. 

There are three ways to declare a semantic
1. Inline in the function declaration inputs - for inputs only. The below declares two inputs into the vertex shader function, which will be initialised with the position and normal of a vertex to render, respectively. 
```
vertOutput vert(float4 pos : POSITION, float3 norm : NORMAL) {
```
2. As part of the function declaration for a single output parameter - for outputs only. The below declares it will return a half4 data-type, which has the SV_Target semantic. 
```
half4 frag(vertOutput vo) : SV_Target {
    //...
    return half4(0.5, 0.2, 0.1, 1.0);
}
```
3. Through a structure, which can be used for both input and output. In the below, the vertInput structure is used as an input to the vertex shader, so its member properties 'pos' and 'norm' will be initialised with the vertex position and normal according to their semantics. The vertOutput struct is used as the vertex output, so the SV_POSITION semantic merely defines the purpose of that variable
```
struct vertInput {
    float4 pos : POSITION;
    float3 norm : NORMAL;
}
struct vertOutput {
    float4 pos : SV_POSITION;
}
vertOutput vert(vertInput vi) {
    // ...
}
```

A full list of binding semantics available can be found in the [Microsoft HLSL Documentation](https://msdn.microsoft.com/en-us/library/windows/desktop/bb509647(v=vs.85).aspx)

#### Why do output semantics exist, and why are vertex/fragment shaders seperated?
These are two interesting questions. Input semantics clearly have a purpose in initialising values, but what do the output semantics do? And why do we need to split this up into two functions, couldn't we just power through with one? The two questions are actually rather related. 

The root cause is that we start with a mesh consisting of vertexes, which when projected to the to pixels on the screen . Our vertex shader really only does work with projecting vertexes to the screen space. But we can't just colour the pixels where vertexes lie, we need to colour the full triangle / fragment in between. What our GPU is clever enough to do, is take all of the outputs from our vertex shader and interpolate them for the pixels in between. The interpolated values are passed into the fragment shader to allow it to compute a final colour for each pixel. 

Output semantics exist to describe the purpose of variables being transitioned between vertex and fragment shaders, indicating how to carry out the interpolation. For example, the component of the vertex shader output representing the vertex pixel coordinates must be known to be the basis for interpolation. And the purpose of other variables may affect how interpolation is carried out. 

#### Passing between vertex and fragment shader
Because the vertex shader only works with values at the vertexes but the fragment shader needs to interpolate between screen pixels, we must always output from our vertex shader at least a pixel position for the vertex. We can output other data as well to provide additional information for the fragment shader, but a screen position is the bare minimum. 

* The vertex function must always output at least a float4 with the **SV_POSITION** semantic. This can be a raw float4, or a structure containing a float4 annotated with the semantic 
* We should always output the same data type from our vertex shader as our fragment shader takes as input - so that the GPU can perform the proper interpolation on the entire object passed between the two shader functions. 

The section in the manual is [here](https://docs.unity3d.com/Manual/SL-ShaderSemantics.html)

#### Fragment Shader Output
The main purpose of the fragment shader is to output a colour for an input pixel. Thus the output from the frag shader must be annotated with **SV_Target**. Usually it is a plain float4, or half4 output. It can also be a struct, but the struct must include a property with SV\_Target. 

### Required Semantics Summary
The following consitutes the bare minimum semantics for a working shader - there may be more from multiple inputs to the vertex shader or structs carrying extra data with at least these properties.
* POSITION: 3D position of vertex to project to the screen as a vertex shader input
* SV_POSITION: screen pixel position in vertex shader output
* Vertex Output Data type: should match fragment shader input data type
* SV_Target: pixel target value - a colour to draw as fragment shader output

UP TO HERE
### Writing Shader Logic
Now we've covered all the syntax, we can have a look at how to actually write down the logic and the math required to make our shader do what we want. 


#### HLSL Instrinsic Language Functions

Like mul() for matrix multiply. Part of [HLSL Spec](https://msdn.microsoft.com/en-us/library/windows/desktop/ff471376(v=vs.85).aspx)
#### Unity Shader Include Files (Helper functions and variables)
Like UnityCG.cginc (helper files, needs to be manually included), or UnityShaderVariables.cginc (helper variables, automatically included in the compilation of every shader)
[Built-in shader include files](https://docs.unity3d.com/Manual/SL-BuiltinIncludes.html)
#### Using Shader Properties in HLSL

#### Textures, Samplers and Shader data types
Skimmed over them earlier, need to explore more here


Keep going with the tutorial [here](http://www.alanzucconi.com/2015/07/01/vertex-and-fragment-shaders-in-unity3d/)

[How to Write Vertex and Fragment Shaders Manual](https://docs.unity3d.com/Manual/SL-ShaderPrograms.html)

[Old HLSL Lighting Implementation Reference](http://www.gamasutra.com/view/feature/2866/implementing_lighting_models_with_.php)A pretty good explanation of how ambient, diffuse, specular lighting, + bump/normal maps and point lights can be implemeneted with Vertex/Fragment shaders - although using older syntax is breaks down the math behind this lighting models nicely, good for personal understanding



[Crash course in HLSL](http://www.catalinzima.com/xna/tutorials/crash-course-in-hlsl/)So I think it's possible for a single vertex to point to multiple texture files - one for diffuse colouring, one for normal mapping, and I dunno what else. TEXCOORD is simply an encapsulation of each of these channels of UV coordinates for a vertex.
Man this is a huge topic though


[Tutorial on Vertex and Fragment shaders](https://docs.unity3d.com/Manual/ShaderTut2.html)

## Water Shader
Don't forget when looking at the water shader (FXWaterPro.shader) - it is also used with an accompanying script Water.cs. Most of the properties of the shader are actually controlled through this script - it is responsible for setting the water behaviour (reflective/refractive) and oscilating the waves on the surface to give natural movement appearance. 
I think I'd like to write an air shimmer and a simple flowing water with white streaks, blue semi-transparent tint and some ripples. 

### Surface Shaders
[How to Write Surface Shaders Manual](https://docs.unity3d.com/Manual/SL-SurfaceShaders.html)



### UV Mapping
Done by a 3D artist in a 3D design application, maps mesh vertexes or faces to UV coordinates in a texture image file. This tells each part of the mesh what colour it should be with reference to a texture file. 
So lets' keep doing shader research for a little while. Then we'll have a look at modifying an existing one and then finally try the gamification side. 
It would be cool if you could build your own flying machine and then have that fly around, but we'll have to see about that. You'd need to save the model somehow? Or make sure everything was parented properly, and only animate the limbs which actually drove it were it a flapping model. 