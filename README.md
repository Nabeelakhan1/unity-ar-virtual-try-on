# Unity AR Virtual Try-On

A simple AR Virtual Try-On (VTO) demo built with Unity.

The idea is straightforward: open the camera, detect the user's face, place a pair of virtual glasses on it, and let the user switch between different glasses from the bottom of the screen.

I built this as a small AR prototype to explore how a glasses virtual try-on experience could be implemented in Unity.

## What it does

- Uses the device's front camera
- Tracks the user's face
- Places 3D glasses on the user's face
- Keeps the glasses aligned as the user moves
- Supports multiple glasses models
- Allows switching between glasses from the UI
- Handles basic camera and face-detection states

## Tech

- Unity
- C#
- AR / Face Tracking
- glTF / GLB 3D assets

## Demo

_Coming soon_

A short video/GIF demonstrating the try-on experience will be added here.

## Project Structure

The project is kept intentionally simple since this is a prototype.

```text
Assets/
├── Scenes/
├── Scripts/
├── Prefabs/
├── Models/
├── Materials/
└── UI/
