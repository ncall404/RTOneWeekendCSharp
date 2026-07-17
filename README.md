# C-Sharp - RayTracing In One Weekend

## About The Project
Follows book one of the [Ray Tracing in One Weekend](https://raytracing.github.io) series but written in C#.

I've wanted to do this for a while and finally had the time to dedicate to it. I decided to go with C# instead of C++ because I prefer writing it. I use SDL3 with the SDL3-CS library to display the rendered image in a window.

It can be set to a real-time mode but there is no movement to take advantage of that yet. I added it early on more for fun to see how it performs even if it needs to be at reduced settings.

I plan on following the books further and doing independant improvements and optimizations on it too.

## Controls
1 - Toggle Anti-Aliasing (The anti-aliasing is just multiple rays per pixel in it's current state, quite expensive.)

2 - Toggle Real-Time Rendering (Repeatedly renders the scene as fast as it can, displays performance in FPS and MS per frame. Pretty slow right now.)

Left Arrow - Decrease selected scene number.

Right Arrow - Increase selected scene number.

Down Arrow - Load selected scene and render it.

Escape - Quit the program.