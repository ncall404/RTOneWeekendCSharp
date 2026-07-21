# C-Sharp - RayTracing In One Weekend

### !!! This branch is the renderer up to the end of book 1 !!!

## About The Project
Follows book one of the [Ray Tracing in One Weekend](https://raytracing.github.io) series but written in C#.

I've wanted to do this for a while and finally had the time to dedicate to it. I decided to go with C# instead of C++ because I prefer writing it. I use SDL3 with the SDL3-CS library to display the rendered image in a window.

It can be set to a real-time mode but there is no movement to take advantage of that yet. I added it early on more for fun to see how it performs even if it needs to be at reduced settings.

I plan on following the books further and doing independant improvements and optimizations on it too.

## Controls
1 - Toggle Anti-Aliasing (The anti-aliasing is just multiple rays per pixel in it's current state, quite expensive. Will auto-rerender if not in real-time mode.)

2 - Toggle Real-Time Rendering (Repeatedly renders the scene as fast as it can, displays performance in FPS and MS per frame. Pretty slow right now.)

Left Arrow - Decrease selected scene number.

Right Arrow - Increase selected scene number.

Down Arrow - Load selected scene and render it.

Escape - Quit the program.

## Rendering Settings
Rendering scenes are set up to be scene dependent for now since there are different performance requirements for each one. To change the settings, go to the bottom of the function labelled LoadScene# where # is the number of the scene you want to change the settings for. The current set values are for renders to complete within a short time (under 10 seconds scene 1, under 2 minutes scene 2) on my Ryzen 7 9700x. If you have a lower end CPU or want to view the scenes in the real-time mode, then I recommend lowering the quality.

Currently only scene 1 is really fit for real-time and that is with anti-aliasing disabled at a lower resolution. For instance, with scene 1 set to 640p width and with anti-aliasing off, I get a framerate of about 111 FPS on my Ryzen 7 9700x.