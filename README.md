# VisualNav

Working with Intel® UK and Microsoft we created VisualNav for Visual Studio, an extension that makes coding fun for disabled developers and young pupils.

The WIMP  (windows, icons, menus and pointers) interface paradigm dominates modern computing systems, but disabled users may find it challenging to use. UCL's MotionInput enables voice and gesture controls of a computer with only a webcam and microphone. VisualNav for Visual Studio provides a touchless interface for writing code. It is designed for ease of use for beginners in the world of coding, from beginner through to advanced.

The project adopts a Visual coding paradigm, this is where blocks of code can be pieced together in an analogy to Legos and will be familiar to children coming from a background in Microsoft Make code. We use CEFSharp, an embedded web browser based on chromium, to integrate ‘Blockly’, the JavaScript library that MakeCode is built upon, and render this panel directly within Visual Studio.

## How does it work?

Users can select a code block from an accessible command palette, with minimal motor movement via the use of a radial dial component. To ensure the correct block is selected, a preview gives a description and visualisation—finally, a building workspace to drag and assemble the blocks of code, to be compiled into code. Throughout the process, voice commands can be used to trigger shortcuts, as an accelerator.

The project supports 9 kinds of blocks, contains 65 block elements supporting JavaScript, Python, PHP, Lua, and Dart and C#.  C# contains the extra feature of ‘custom blocks’, which allows library functions to can be added to the radial menu as blocks facilitating advanced developers to build more complex applications.

It is now possible to write code with only facial movement and speach commands.


The radial menu interface, preview and building windows enabling efficient code block creation:

![mainScreenshot](https://user-images.githubusercontent.com/96876320/187952113-ee522257-4a3c-4802-877e-a9b77b117410.png)

# MotionInput V3
Although fully standalone, the application is best used to be used with MotionInput V3 best done with nose based navigation plus speech, but also fully compatible with eyegase and multitouch availible from https://touchlesscomputing.org/. 

## What is motion input?

# links:

* Download: https://marketplace.visualstudio.com/items?itemName=UCLFacialNavforVisualStudio.VisualNav
* Website: https://hrussellzfac023.github.io/VisualNav/
* Examples: https://github.com/HRussellZFAC023/VisualNavExamples/settings/access
* Motion input nose based navigation plus speech (including VS voice shortcuts): https://github.com/HRussellZFAC023/VisualNav/releases
* Secret Note: https://pastebin.com/x19YXsa8


# How to install

To get started with VisualNav, there are two methods for the installation process:

### Option 1 - Manually: 
After closing Visual Studio, go to the release section of the repository. Then double click on the .VSIX file and run the installer.

## Option 2 - Via the store:
Open Visual Studio, then go to "extensions" and search for VisualNav.

![image](https://user-images.githubusercontent.com/22746105/188231082-5454a0bc-1b5c-49de-bbb0-2c7e3ac0ead9.png)

## Prerequisites: 
* Visual Studio installed.
* Required dependancies for CefSharp installed (https://cefsharp.github.io/).
* Microsoft .NET 4.5.2 or greater.
* Visual C++ Runtime 2019 or greater (Visual C++ 2022 Runtime is backwards compatible).

     
## Setup instructions:
    
To setup, simply navigate to the "Tools" bar and click on "open all windows".

## Examples:

![hierarchical](https://user-images.githubusercontent.com/96876320/187951335-05cff28b-045b-4fba-b289-031baa2efa4e.png)

Example of created blocks using the radial menu:

![block_example](https://user-images.githubusercontent.com/96876320/187952486-52fb1fae-4330-4415-86b3-fea184484129.png)


