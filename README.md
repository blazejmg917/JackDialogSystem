# JackDialogSystem

Open Use Dialog System for unity games.


Includes actual grouping of files to copy into a Unity project, as well as a sample Unity project to show off its functionality.


For use, you must have both the New Unity Input System and TextMeshPro installed. No other packages are needed.


To integrate into your project, first add the NewDialogSystem folder into your project, and copy one the Complete Dialog Manager prefab into your scene. You can now display dialog by calling the "DialogManager.Instance.StartDialog" function and passing in a Dialog Node. The dialog will then run through all entries until it reaches the end of the diaog tree.


The four types of dialog nodes are as follows:


Basic: simply displays dialog and moves on to the next node

Conditional: picks the next node based on a set of conditions. These conditions can be manipulated by calling DialogManager.SetVariable and DialogManager.SetFlag

Choice: displays a set of dialog options the player can choose from which each link to a different following node

Random: uses random weighting to select an option from a list of possibilities


Other important scripts are:

DialogInputHandler: handles input to skip dialog and move to the next node. Uses InputActions as a reference

DialogVarsHandler: handles dialog variables and flags to allow for conditional node functionality

Typewriter: displays dialog in typewriter format. Can adjust where to type to, and the speed of typing. For proper functionality, be sure to set the background color field to invisible
