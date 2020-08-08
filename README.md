# UnityGoogleGlassGestureDetector
This is the default script used in Android studios to detect gestures for the Google Glass Enterprise 2 that I have converted to C# and modified so that it works in the Unity Engine. I thought I would just share this to help out the handful of other people out there who have the misfortune of developing for the Google Glass :P

The way it works is by getting the speed and direction of movement of any touches and then based on that determining whether the touch is a SWIPE_UP, SWIPE_DOWN, SWIPE_BACK, SWIPE_FORWARD or TAP.

![Capture](https://user-images.githubusercontent.com/54575751/89701434-5d227c00-d97a-11ea-8682-6ebe08a11e84.PNG)


Other scripts should inherit from this the UnityGlassGestureDetector script in order to have unique behaviour.
In the event of having multiple buttons on a single page, this script has an isFocusOfTaps bool. There should only be one scipt at a time that has this variable set to true. Whichever script is the focus is the one that will handle any touches.

It also has support for in editor usage that will use the mouse's position and direction of movement while pressing mouse button 1 to emulate a touch. I have found the ineditor touch emulation to be somewhat unreliable depending on the resolution of your screen so you may need to tweek SWIPE_DISTANCE_THRESHOLD_PX and SWIPE_VELOCITY_PX on line 109 and 110 in order for it to work properly. I will continue to work on improving the unity editor support though so that eventually these values will not need to be tweeked

![Capture2](https://user-images.githubusercontent.com/54575751/89701723-f8b4ec00-d97c-11ea-86cd-1d5fdafbc658.PNG)

