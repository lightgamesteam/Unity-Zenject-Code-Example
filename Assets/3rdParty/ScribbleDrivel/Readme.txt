/*
[*README COMBINE*]

Scribble Drivel - Runtime Drawing Tool
Lylek Games

[*SCRIBBLE DRIVEL - RUNTIME DRAWING TOOL*]
Open up the provided scene, located in the ScribbleDrivel/Scenes folder. Press play and test it out.
Feel free to modify the demo as you wish, or create your own drawing board from scracth.

Step 1:
If you want to create your own drawing board, go ahead and add your own UI Image to your scene.

Step 2:
Now locate the DrawScript in the ScribbleDrivel/Scripts folder, and place it on your ui image.
The image your use for your drawing board will be used as a mask, so feel free to use any shape
- it doesn't have to be a rectangle!

Step 3:
Your ready to go! In order to change brush settings (such as color, size, and shape), you may reference
the DrawScript, in game, by refering to the variable: DrawScript.drawScript; then call the desired method
and specify the appropriate parameters, such as:

	DrawScript.drawScript.SetBrushSize(int);
	DrawScript.drawScript.SetBrushColor(Color);
	DrawScript.drawScript.SetBrushShape(Sprite);

	You can also call the Undo method, using a button or otherwise, by calling:

	DrawScript.drawScript.Undo();

I have included a 'RateScribbleDrivel' script which will propmpt the user (you!) to rate this asset. This prompt
should only ever appear the one time, regarless of your choice. If there are any issues with this prompt, please let
me know, or simply delete the script located directly in the ScribbleDrivel folder. Thanks!

[*SUPPORT*]
We do our best to make our assets as user-friendly as possible; please, by all means, do not hesitate to send us an email if you have any questions or comments!
support@lylekgames.com, or visit http://www.lylekgames.com/contacts

**Please leave a rating and review! Even a small review may help immensely with prioritizing updates.**
(Assets with few and infrequent reviews/ratings tend to have less of a priority and my be late or miss-out on crucial compatibility updates, or even be depricated.)
Thank you! =)

*******************************************************************************************

Website
http://www.lylekgames.com/

Support
http://www.lylekgames.com/contacts
*/
