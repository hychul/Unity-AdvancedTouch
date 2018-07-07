# AdvancedTouch

![](https://img.shields.io/badge/Unity-2018.1-blue.svg?style=flat-square) ![](https://img.shields.io/badge/License-MIT-blue.svg?style=flat-square)

Unity plugin for advanced touch input handling.

# Description

Default Unity touch input phase is `Began`, `Moved`, `Stationary`, `Ended` and `Canceled`. It's simple but not enough to handle hold and in Samsung phone, stationary touch handling almost impossible, because of Samsung phone having unstable touchpoint issue.

With AdvancedTouch plugin above issues are can be solved :D

AdvancedTouch is providing `Begin`, `Down`, `Hold` `Drag` `Up` `HoldUp` and `DragUp` touch phase. Samsung phone's unstable touch point issue was fixed by using tremor revision.

# Usage

Put AdvancedTouch's TouchInputModule somewhere in the scene and make a script that handle touch inputs.

# Running the Example

Open 'Main' scene and hit the play button :D
