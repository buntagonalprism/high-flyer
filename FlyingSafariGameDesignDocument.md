# Flying Safari Game Design Document

## Overview


## Initial Scope
The scope of the project is divided into four sections. 
- Models required: what 3D assets are required to be created / found in order to build the game environment
- Game structure: overall gameplay flow and ordering of events
- Gameplay flow and locomotion: specifics of how game mechanics will be implemented
- Effects: additional visual and auditory effects which increase realism in the game

### Models Required
- A start room with decorations including table and chairs, posters, and some basic woodworking tools
- Components to make a leonardo da-vinci style winged flying machine 
- An assembled, interactive flying machine controlled using HTC Vive hand controllers
- Custom hand-controller models to replace default HTC Vive controller models with something more appropriate to flying machine style - e.g. wood appearance 
- Large landscape playing area surrounded by canyons which define the extent of the game. 
- The following regions will be included in the terrain, connected by narrow canyon sections
    * Forest
    * Desert
    * Open Plains
    * Lake
    * Volcano
- Animal models which animate while walking around 
- Trees and other plant decorations like rocks, cactii to fill the space in the landscape
- Ground terrain that varies naturally in height and textures for different environments

### Game Structure
1. Player starts in a room like a carpentry workshop that also contains pictures of animals
2. Player assembles pieces of a flying machine together to make a model wing
3. When the wing is completed, a robot arm comes and carries the wing away, attaching it onto the rest of the flying machine which is revealed when a wall slides open, and the player enters the completed machine
4. The flying machine descends into a large simulator room where UI / narration instructs player how to fly the machine so they can learn the controls
5. Flying machine is launched through waterfall into a canyon
6. Player flies the machine around a large environment
7. Groups of different animals roam around sections of the environment
8. As they fly, they can take pictures using a camera connected to the HMD
9. Animals in pictures are automatically identified when pictures are taken
10. At the far end of the terrain is a landing zone
11. When the user enters the landing zone they will be guided towards the ground, then taken back to the starting room
12. User will be able to see photos they have taken and what animals were found. 
13. Animals they did not find will also be shown. 
14. The user will have the chance to play again, taking them back to the start room

### Gameplay and Locomotion
- Player can walk around the workshop room and can pick up pieces of the flying machine spread out across the room 
- The simulation environment allows all flying controls to work in place - without actually moving forward, so players understand how the controls work
- To start main gameplay, player and flying machine are launched from a catapult and player flys parabolically until level to give them speed and distance from canyon walls before they start flying
- Flying device experiences gravity and air drag, slowing down and falling over time
- Player can swipe both controllers downards to beat wings of flying machine to gain air and height
- Controllers act by mimicing a steering wheel:
    * Relative angle of controllers determines the inclination of each of the wings 
    * Angle of the wings determines the rate of roll around the forwards direction
    * The current absolute roll determines the rate of yaw (compass direction change)
    * The distance of the controllers from the body controlls the rear elevators 
    * Angle of the elevators controls the rate of pitch (change in angle to the horizontal)
- If the user flies too high up or into the ground, they will be bounced back with their velocity and view direction reflected, as though they hit a bouncy wall
- The same will apply when the user hits any obstacle like canyon walls or trees 
- Animals will roam around the environment either individually or in groups, avoiding obstacles as well as each other. 
- The player can deploy a camera while flying to take photos of the animals using a button press on a controller
- While the camera is deployed, the field of view on the HMD will decrease as though the user was looking through a camera lense. Time will slow and the user can zoom their camera view by rotating a thumb around the touchpad
- A photo is taken by a different controller button press. Animals in the photo will be detected immediately and stored for display at the end of the game
- When the player presses the button to retract the camera, time resumes at normal speed


### Effects
- Water Effects for lakes in the terrain
- Flowing water shader with ripples at edges and streaks indicating flow direction for rivers
- Boiling lava effect coming down the side of the volcano region
- Heat wave air shimmer above volcano vents and some desert regions
- Animation of plane assembling from various pieces by robotic arms
- Baked scene lighting for performant experience in large terrain
- Sound effects include the following:
    * Music playing from speakers in the workshop
    * Rumbling of machinery as robotic arms assemble plane
    * Rush of water falling down the waterfall
    * Wind whistling through forest
    * Rumble of volcano
    * Air-rush sound from beat of wings
    * Catapult launch sound
    * Footstep sounds for animals
    * Occasional noises relevant to each animal

## Revised Scope
- terrain too large, baking never completed without out of memory errors on 32GB machine running for > 24hr even on very low resolution settings. 











