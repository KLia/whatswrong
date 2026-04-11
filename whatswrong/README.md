# A Simple Diamond in Unity
Hi, and welcome to this Github project for showing how to use the Dynamic AI SDK inside of Unity.

In this project you can see how to invoke both a Diamond running locally on your device and using the LLM in Engine feature
to easier iterate on how to use the dynamic text generated in the context of the game.

## Impotant Message
In order to use the SDK inside of Unity, you need to toggle _On_ the **Allow 'unsafe' Code** setting.
You can find this setting in Unity by going:
Edit -> Project Settings -> Player -> Other settings and scoll down to the toggle.
See the image below.
<img width="615" height="375" alt="image" src="https://github.com/user-attachments/assets/cc19e407-4543-4d7a-b1f8-2d42e7f46dbc" />


## Using a local Diamond

To invoke a locally Diamond, you can look at the example code shown in the class `DiamondExample` [See it Here](https://github.com/RawPowerTools/Unity-Simple-Diamond/blob/main/Assets/Scripts/DiamondExample.cs)
The code has been commented, so you can follow along for each step needed to invoke the Diamond.

### Impotant Note
Since the Diamond files are to big to be added to the Github repository (and I can't get Git LFS to work on the repository) I have added a link to a simple Diamond that you can use, to see the code work correctly.

The Simple Diamond can be found here:
https://drive.google.com/file/d/1E_ZBUlxfrxYm5c1gh0aUDSCybfngK6Kf/view?usp=drive_link

The Diamond needs to be placed inside the `StreamingAssets` folder. We will suggest you follow the following file path stucture:
`StreamingAssets/DynamicAI/Diamonds/{name_of_diamond}`

To make it easy to setup this folder, this project also contains a Unity Editor tool, that will create these folders for you. [(Found here)](https://github.com/RawPowerTools/Unity-Simple-Diamond/blob/main/Assets/Scripts/Utility/Editor/DiamondEditorUtility.cs)
You can trigger this tool by clicking on this menu item:
Raw Power Labs -> Dynamic AI -> Create Streaming Assets Diamond Folder.

If the menu item is disable / you can't click it, it means all the folders are already created in the project. If one of these folders are missing, you can click on the meun item and the missing folders will be created.

## Using LLM in Engine

To use the LLM in Engine feature, you can look at the example code shown in the class `LLMInEngineExample` [See it Here](https://github.com/RawPowerTools/Unity-Simple-Diamond/blob/main/Assets/Scripts/LLMInEngineExample.cs)
The code has been commented, so you can follow along for each step needed to use the LLM in Engine Engine.

### Impotant Note
In order for the LLM in Engine feature to work, you need to be on the VPN 

