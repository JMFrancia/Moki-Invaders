# Moki Invaders

A lightweight Space Invaders clone.

## How to Play

Start the game from Unity editor and use the mouse to control left and right, or build to IOS / Android and use touch control. Player fires automatically. Destroy all enemies to win. Destroy the UFOs for bonus points!

## PAQ (perhaps asked questions)

### Why do you use coroutines in some areas and not others?
Although there is some debate about this in the Unity dev community, my experience is that coroutines create overhead when operating at high volume. So as a general rule if I have a simple use case I try and use the Update() hook instead. However if you take a look at the SFXManager class, you will see I am no stranger to coroutines.

### What's up with the leftover to-do comments?
I'm a big believer in documenting my thought process in the code, including stuff I didn't get to, or that was out of scope. Helps provide context for what I was trying to accomplish and why.

### Why did you choose to put all the UI for different "screens" in a single scene?
In this case it was very easy because the UI consisted of like, 2-3 screens, but in general switching scenes is a burdensome process. Sometimes it's faster to keep everything in one scene; I wouldn't always recommend this but in this case I thought it made sense.
### Is all this code yours?
100% of the code in this project was written by me, though some of it has been recycled / refitted from previous personal projects. The EventManager is loosely based off implementation by John Tucker, which can be found here: https://github.com/larkintuckerllc/hello-unity-eventmanager
### Where did the SFX come from?
https://freesound.org and https://www.classicgaming.cc/classics/space-invaders/sounds.
