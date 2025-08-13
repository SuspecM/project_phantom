Hello, these are my personal toolkit thingies. Essentially scripts writen for a project called Project Phantom that was supposed to be a sort of horror game made in Unity but got cancelled for being bad and a huge time sink.

I'd say the most useful script out of these are in the menu folder, especially the GameManager one since it contains code that allows you to load a scene in a way that is compatible with the persistent game manager pattern and occlusion portals.
It's very finnicky in Unity since you have to set the given scene as active on the very next frame after it was loaded. Took me a month to find the exact combination and order of code lines to make this work so I'm proud of it.

There is also a working main menu and settings menu script there as well.

The sound folder is only useful if you are using FMOD.

The rest are very situational. The NewCharacterController script is a good one as it handles moving, sprinting as well as crouching properly but currently it's a bit too tied to other scripts like Gun.cs.
You also probably need to clean out the weird first person hand animation handler. It was created specifically so that I'd have to use Blender as little as possible and do the animation in Unity, which ended up becoming quite a mess.

TabletLogic is neat as well. It lets you interact with tablets akin to Aliens vs Predator 2 from back in the day. Doesn't make any sounds tough.

Feel effects manager is also something I'm proud of if you use the Feel asset from the asset store.

Another cool one is the PlayerPickupDrop that lets you pick up and hold physics objects in front of the player. It stays held as long as the interact button is held down and there is a button to push it away from you. 
I tried to make a dedicated rotate button as well but I could never got it working properly.

Other than that, there is a cool glitch shader effect you can control with the GlitchController. It's made in Shader Graph.
