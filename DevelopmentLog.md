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

## Goals 4/8/17
Gamification and robustness are the next two important steps:
- Shader: combined sine-function with world position oscilates albedo of vertical surfaces between dark brown up to sandy-nearly-white making lines along canyon walls
- Shader: simple water effect with ripple, blue tint, reflection only, and white streaks in a fixed world direction, possibly controlled via script along rectangular sections
- Robustness: can't fly through major objects (we'll allow tree-trunks and the like)
- Gamification: while flying is fun, some purpose, like potentially musical notes that play when you fly through rings. Seeing trails would be pretty cool too. 
- 3D modelling & animation: draw up a proper flying machine and rig it so it actually flaps. 
- Sound design: super important - need rushing air over wings, river water running, waterfall falling, volcano rumbling, wind rustling trees, occasional crickets, wind whistling around stone shapes, some grass swaying would be nice, steam vents 
- Use terrain editor to build up a much larger, more performant terrain for the game. 
- Use speed tree or something similar to populate a forest area? Trees at a distance are billboarded (rendered as a sprite facing the user rather than a 3D model), and level of detail rendering is built in (further away trees have less mesh complexity)
- Try adding small drag and gravity, with trigger button wing flaps to boost. 



It would be cool if you could build your own flying machine and then have that fly around, but we'll have to see about that. You'd need to save the model somehow? Or make sure everything was parented properly, and only animate the limbs which actually drove it were it a flapping model. 

## Writing a Canyon Shader
I think we need an HSL colour space. Here changing lightness can take a single colour, richest around 0.5, down to black at 0 or white at 1. HSV by contrast needs 
HSL puts pure black as the bottom face of the cylinder, and pure white at the top. 
HSV also puts pure black at the bottom, but pure white is the centre core axis of the cylinder
HSL is the colour model typically used where there is a square field of colour and a slider at the side to lighten/darken it. The slider is the luminence, hue is the colour spectrum running across, and saturation runs vertically in the square between full colour and plain grey


It's not looking too bad now actually. I'd like to take out the bits and modularise it some - especially remove the water. 
Okay I have a choice of going high or low poly. Low poly is more fun. So I need some low-poly water, and probably need to make my sand more cartoony
I really need to get back to making my game howerver. The shader distraction has been fun, and I reckon I can do the air shimmer now too. I need to do some design work on the actual game now. So lets make that the focus over getting a nice-looking 3D model. I need to make an experience I can actually submit... Let's start by doing designs on paper of what I actually want to achieve. A game design document that sets a spec. Needs to be at varying levels of detail and achievability so I can actually finish it. But yeah, let's do that and only do blender course in spare time - its not the priority. 

## Next Steps
So we have a great few things laid out now. I'd like to 
1. Try out the animals. Will need to add to gitignore
2. Try out makeVR to see if its more suitable than blocks. 

As an aside, I don't think we can lightmap the canyon walls because the shader is dynamic, so there's nothing to bake our lighting data onto (and no UVs to map them to). 
Also regarding the poly count - it seems 90% are coming from the environment model - nearly 1.5million triangles and 2 million verts, for a model which according to both Blender and Unity in the asset import details, has about 60,000 of each. 

Hmm so thoughts about MakeVR
 - It is much more CAD program like - it provides operators for subtraction and addition of meshes, and can do pull extrusions, which is pretty neat. Unlike blocks, there is no per-vertex editing, and the process of creating stuff is a lot more annoying - going in to some detailed menus to create primitives. I suppose best thing to do would be to keep the primitives handy 
 Lets try to make a cactus
 this is definitely more material focussed though, likely to build a more unity-friendly model I suspect. $50 for the pro version which has rulers and alignment...

 The reason for our huge number of rendered triangles is nothing to do with our model complexity, but shadow quality! With all lights disabled and ambient light only, 
 - Tris: 256.8k, Verts: 325.5k - sounds about right all considered
 With directional light on with No Shadows type
 - Stays exactly the same
 Turn on either Hard or Soft Shadows
 - Jumps to Tris: 1.5M and Verts 1.8M
So ideally the way to solve this is to not use realtime lights, cause I reckon its the area of shadow that causes the problem, unity is breaking it down into smaller shadows to calculate lighting. 
If we were able to bake our lightmaps, that should reduce the shadow complexity. However, baked lightmaps require texture on objects, not custom shaders. 
Can see by moving the object out of the shadow cascades how the number of tris and vertexes further drops. 

So the .obj export doesn't seem to have brought any material along with it - I suppose its designed for 3D modelling export, but definitely frustrating there. 
We need UV parameters to generate lightmaps. 

New test with large modeL
597.9k / 613.9k before lightmap bake. 
Had to set model to static, light to static, ground to static, light to baked, generate UVs for all imported models. 
The lightmap bake is taking its sweet time as usual. Wow so the bake took nearly an hour, but huge gains!
Tris 121.1k, Verts 138.4k, hardly anything and a huge reduction! So i wonder if it would work with a custom shader? Maybe - as long as lighting intensity is static, albedo can change and GPU will sort out appearance under the lighting model without needing to reproject the lights

Definitely thingk we need MakeVR pro if we're going to do anythign useful. But its still missing fundamental stuff, like the ability to draw smooth bezier curves into shapes to extrude. 
But i'm looking forward to the knuckle controllers, the wands are a bit large for continuous comfort to hold - and holding the trigger while pressing the touchpad is downright painful becuase the trigger action is so large. Let's purchase that tomorrow and give it a try. It'd definitely be good for designing the flying machine hopefully. If not, back to sketch-up (or blender!) Reckon I could do the base structure pretty fast in blender, but the nice curves are harder. 

### Creating the Resolution Changer
Changing terrain resolution through interpolation and a gaussian kernel

Terrain is starting to look fairly awesome though now. I reckon next steps are to extract the cactus and trees as well as arches, see if we can put a joint in the trees, get the other low poly trees, and the mesa in the middle as well, and start dropping trees everywhere. 

Then do a test flight! Really should have done this earlier under "test often" mentality. 

Oh yeah, add the columns as well. Maybe just plain colours with rough normals for them though?

Then we've got flying machine building or animal waypoints to do. 
Should probably build start and end rooms quickly out of downloaded primitives before I get too carried away with the safari mechanics
Cactus done. 
Let's extract the tree next