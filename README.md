# Create voice commands for VR experiences with Watson services

*Read this in other languages: [한국어](README-ko.md).*

In this developer journey we will create a Virtual Reality game based on Watson's [Speech-to-Text](https://www.ibm.com/watson/developercloud/speech-to-text.html) and Watson's [Conversation](https://www.ibm.com/watson/developercloud/conversation.html) services.

In Virtual Reality, where you truly "inhabit" the space, speech can feel like a more natural interface than other methods. Providing speech controls allows developers to create more immersive experiences. Google Cardboard is by far the most popular VR headset platform, with 84 million sold in 2016 (http://www.hypergridbusiness.com/2016/11/report-98-of-vr-headsets-sold-this-year-are-for-mobile-phones) in 2016.

When the reader has completed this journey, they will understand how to:

* Add IBM Watson Speech-to-Text and Conversation to a Virtual Reality environment build in Unity.

![](doc/source/images/architecture.png)

# Watch the Video

[![](http://img.youtube.com/vi/rZFpUpy4y0g/0.jpg)](https://www.youtube.com/watch?v=rZFpUpy4y0g)

### With Watson

Want to take your Watson app to the next level? Looking to leverage Watson Brand assets? Join the [With Watson](https://www.ibm.com/watson/with-watson) program which provides exclusive brand, marketing, and tech resources to amplify and accelerate your Watson embedded commercial solution.

## Included components

* [IBM Watson Conversation](https://www.ibm.com/watson/developercloud/conversation.html): Create a chatbot with a program that conducts a conversation via auditory or textual methods.
* [IBM Watson Speech-to-Text](https://www.ibm.com/watson/developercloud/speech-to-text.html): Converts audio voice into written text.

## Featured technologies

* [Unity](https://unity3d.com/): A cross-platform game engine used to develop video games for PC, consoles, mobile devices and websites.
* [Google Cardboard](https://vr.google.com/cardboard/): An inexpensive viewer that mounts an Android phone to view VR apps.

# Steps

1. [Before you begin](#1-before-you-begin)
2. [Create Bluemix services](#2-create-bluemix-services)
3. [Building and Running](#3-building-and-running)

## 1. Before You Begin

* [IBM Bluemix Account](http://ibm.biz/Bdimr6)
* [Unity](https://unity3d.com/get-unity/download)
* [Google GVR Unity SDK](https://developers.google.com/vr/unity/get-started)
* [Android Unity SDK](https://docs.unity3d.com/Manual/android-sdksetup.html) 

## 2. Create Bluemix services

On your local machine:
1. `git clone https://github.com/IBM/vr-speech-sandbox-cardboard.git`
2. `cd vr-speech-sandbox-cardboard`

In [Bluemix](https://console.ng.bluemix.net/):

1. Create a [Speech-To-Text](https://console.ng.bluemix.net/catalog/speech-to-text/) service instance.
2. Create a [Conversation](https://console.ng.bluemix.net/catalog/services/conversation/) service instance.
3. Once you see the services in the Dashboard, select the Conversation service you created and click the !["Launch Tool"](/doc/source/images/workspace_launch.png?raw=true) button.
4. After logging into the Conversation Tool, click the !["Import"](/doc/source/images/import_icon.png?raw=true) button.
5. Import the Conversation [`workspace.json`](data/workspace.json) file located in your clone of this repository.

## 3. Building and Running

If you followed the previous steps you should already be inside your local clone and ready to get started running the app from Unity.

1. `git clone https://github.com/IBM/unity-sdk`
2. Open Unity and inside the project launcher select the ![Open](doc/source/images/unity_open.png?raw=true) button.
3. Navigate to where you cloned this repository and open the "SpeechSandbox" directory.
4. If prompted to upgrade the project to a newer Unity version, do so.
5. Follow [these instructions](https://github.com/IBM/unity-sdk#getting-the-watson-sdk-and-adding-it-to-unity) to add the Watson Unity SDK downloaded in step 1 to the project.
6. Follow [these instructions](https://github.com/IBM/unity-sdk#configuring-your-service-credentials) to add your Speech To Text and Conversation service credentials (located on [IBM Bluemix](https://console.ng.bluemix.net/)).
7. Select `Advanced Mode` in the configuration window.
8. Open the script vr-speech-sandbox-cardboard/SpeechSandbox/Assests/Scripts/VoiceSpawner.cs and put your workspace ID on line #34 in the Start() method.
 You can find your workspace ID by selecting the expansion menu on your conversation workspace and selecting `View details`.
    ![View Details Location](doc/source/images/workspace_details.png?raw=true)
9. In the Unity editor project tab, select Assets->Scenes->MainGame->MainMenu and double click to load the scene.
10. Press Play
11. To Build an android .apk file and deploy to your phone, you can File -> Build Settings (Ctrl + Shift +B) and click Build. 
12. When prompted you can name your build and then move it to your phone.
13. Alternately, connect the phone via USB and File-> Build and Run (or Ctrl+B).

   *Make sure you have enabled USB Debugging:*
     
* Open Settings-> About-> Software Information-> More

* Then tap “Build number” seven times to enable Developer options.

* Go back to Settings menu and now you'll be able to see “Developer options” there.

* Tap it and turn on USB Debugging from the menu on the next screen.

   Once the app is deployed to your phone it will start, but you'll need to set permissions for the app before it will work correctly:

  * Open Settings-> Apps-> SpeechSandboxCardboard-> Permissions and enable Microphone and Storage.

# Links

* [Demon on YouTube](https://www.youtube.com/watch?v=rZFpUpy4y0g)
* [Watson Unity SDK](https://github.com/IBM/unity-sdk)

# License

[Apache 2.0](LICENSE)
