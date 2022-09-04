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

During the pandemic, academic & students from UCL developed UCL MotionInput 3. This technology uses computer vision with a regular webcam to control a computer like a mouse & keyboard. In addition, it uses natural language processing to control existing applications & shortcuts. This project uses this technology to enable young & mature developers with accessibility needs to design, write & test programs with Visual Studio. For example, one can move their hands or face to move the mouse. Or, one can say the words "click" and "double click" out loud to access the mouse's functionality.

# links:

* Download: https://marketplace.visualstudio.com/items?itemName=UCLFacialNavforVisualStudio.VisualNav
* Website: https://hrussellzfac023.github.io/VisualNav/
* Examples: https://github.com/HRussellZFAC023/VisualNavExamples/settings/access
* MI3 Facial Navigation v3.04 (Special Edition for VS accessibility): https://touchlesscomputing.org/
* Secret Note: https://pastebin.com/x19YXsa8
* Work published in Microsoft blog: https://techcommunity.microsoft.com/t5/educator-developer-blog/ucl-amp-intel-visualnav-v2-facial-navigation-for-visual-studio/ba-p/3616447


# How to install

To get started with VisualNav, there are two methods for the installation process:

### Option 1 - Manually: 
After closing Visual Studio, go to the release section of the repository. Then double click on the .VSIX file and run the installer.

## Option 2 - Via the store:
Open Visual Studio, then go to "extensions" and search for VisualNav.

![image](https://user-images.githubusercontent.com/22746105/188231082-5454a0bc-1b5c-49de-bbb0-2c7e3ac0ead9.png)

## Prerequisites: 
* Visual Studio installed.
* Microsoft .NET 4.5.2 or greater.
* Visual C++ Runtime 2019 or greater.

     
## Setup instructions:
    
To setup, simply navigate to the "Tools" bar and click on "open all windows".

## Examples:
Example navigating the command palette:

![hierarchical](https://user-images.githubusercontent.com/96876320/187951335-05cff28b-045b-4fba-b289-031baa2efa4e.png)

Example of creating blocks from the command palette:

![block_example](https://user-images.githubusercontent.com/96876320/187952486-52fb1fae-4330-4415-86b3-fea184484129.png)


