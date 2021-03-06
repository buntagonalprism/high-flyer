# Flying Safari Readme
## Summary
An experience that grants users the experience of the joys of flight, to explore beautiful and varied terrain on the hunt to collect images of native wildlife. 

[Video Demonstration](https://www.youtube.com/watch?v=tTkxuBZIRzo&t=132s)

![Flying through canyon environments](/Screenshots/FlyingCanyons.PNG)

## Platform `
Target Platform: HTC Vive

## Achievements Completed

This description is already too long for the achievements video 
Fundamentals:
- Scale: The user should feel human height in the start room as it is a human workshop. Hence objects like whiteboard, stools, tables are designed at normal heights. In the landscape, users feel small in exploring a giant landscape which fills them with wonder, so environment objects like trees and cactii are deliberately large, which also serves to fill vertical space and make more interesting terrain to fly over and around. 
- Animation: A number of animations are used, both ones I created and assets from the unity store. Some of the most striking are the gorgeous animal movement animations which come from the Low Poly Animated Animals package by Pavel Novak. Otherwise my animations including lowering the player into the flying machine at the start which gradually reveals itself in a large hanger, and animating the links which form a drive belt from the steering wheel down to a hidden drive mechanism controlling the wing rotation, which helps add feedback to the user that their hand movements are actually controlling the plane. 
- Lighting: The starting workshop environment presented the most opportunity for lighting, as only a directional light representing the sun was suitable for the large outdoor terrain. In the workshop itself, mixed lights are used to give good performance for baked global illumination, in addition to creating interesting dynamic shadows from the rotating minature model of the flying machine. Realtime spotlights are used in the plane hanger to track its reveal as the user is lowered down into it, keeping the focus on the flying machine and again creating dramatic shadows as it moves and rotates. 
- Locomotion: As a flying game, locomotion is of primary importance. The decision to not include walking-based locomotion, although an essential experience of the Vive and other room-scale VR systems, allowed for a focus on the experience of the joy of flight. The user still has direct agency with their locomotion, however it is through the steering wheel, controlled by proxy by the hand controllers that they achieve it. It took a lot of experimentation with flight models to achieve a balanced mechanic that was both easy and precise to control, allowing precise maneouvers like swooping through trees. 
- Physics: the mechanics of the flight controls take their inspiration from physical systems, namely that aielerons on a plane exert a torque on the body which causes it to roll, and that a plane which is rolled will bank / yaw to change direction. Likewise elevators on actual planes exert force on the rear of the plane leading to a torque which causes it to pitch up or down, which is simulated by the behaviour of the elevators in Flying Safari. Furthermore, the flying machine is created to appear as though it were physically possible to construct out of simple materials like wood and cloth, hence the effort for example in creating the drive chain which is actually a number of independent link objects animated to look like a continuous belt. 
Completeness:
- Gamification: The gamification of flying safari is the hunt for all the animals in the landscape, some of which, like the bear in the forest, or the fish in the smaller of the clear lakes, can be hard to spot. This mode of gamification rewards exploration and a keen eye. 
- AI: AI in the animals is an important part of making them realistic and interesting to look for. The unity navigation system was used to provide pathplanning and obstacle avoidance capabilities, and a waypoint system for each group of animals was used to define rough patterns for how they should roam around, with the animal positions randomly perturbed to avoid unnatural looking repitition. 
- 3D Modelling: Several significant parts of Flying Safari were built as custom 3D models for this project. The wings of the flying machine were initially roughly drawn in Google Blocks VR, then imported into Blender for touching up and adding the frame. The cactii and palm trees were also created in Google Blocks VR, while the canyon terrain was created by sweeping rectangles into natural looking paths using the MakeVR Pro CAD modelling program for VR. 
Challenge:
- Compute Shader: A distinctive feature of the flying safari terrain is its canyon walls. These were designed to imitate a layered rock formation with rippling bands of colour, as is commonly found in canyon walls as a result of different stone layers accumulating over time. This effect was implemented using a custom surface shader in Unity. The surface shader uses a combination of sine functions on the x,y and z world coordinates of an input geometry poiint to determine how much to change the lightness of a base colour in an HSL colour model. As the sine functions are continuous, it allows the colour ripple to vary consistently both spatially and vertically. A normal map is also sampled from based on the world coordinates to provide 3D texture appearance for the surface.   
- User Testing:

