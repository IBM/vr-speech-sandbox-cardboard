# Create voice commands for VR experiences with Watson services

*Read this in other languages: [한국어](README-ko.md), [中国](README-cn.md).*

> Watson Conversation is now Watson Assistant. Although some images in this code pattern may show the service as Watson Conversation, the steps and processes will still work.

In this Code Pattern we will create a Virtual Reality game based on Watson's [Speech-to-Text](https://www.ibm.com/watson/developercloud/speech-to-text.html) and Watson's [Assistant](https://www.ibm.com/watson/developercloud/conversation.html) services.

In Virtual Reality, where you truly "inhabit" the space, speech can feel like a more natural interface than other methods. Providing speech controls allows developers to create more immersive experiences. Google Cardboard is by far the most popular VR headset platform, with 84 million sold in 2016 (http://www.hypergridbusiness.com/2016/11/report-98-of-vr-headsets-sold-this-year-are-for-mobile-phones) in 2016.

When the reader has completed this Code Pattern, they will understand how to:

* Add IBM Watson Speech-to-Text and Assistant to a Virtual Reality environment build in Unity.

![](doc/source/images/architecture.png)

## Flow

1. User interacts in virtual reality and gives voice commands such as "Create a large black box".
2. The Android phone microphone picks up the voice command and the running application sends it to Watson Speech-to-Text.
3. Watson Speech-to-Text converts the audio to text and returns it to the running Application on the Android phone.
4. The application sends the text to Watson Assistant. Watson conversation returns the recognized intent "Create" and the entities "large", "black", and "box". The virtual reality application then displays the large black box (which falls from the sky).

# Watch the Video

[![](http://img.youtube.com/vi/rZFpUpy4y0g/0.jpg)](https://www.youtube.com/watch?v=rZFpUpy4y0g)

## Included components

* [IBM Watson Assistant](https://www.ibm.com/watson/developercloud/conversation.html): Create a chatbot with a program that conducts a conversation via auditory or textual methods.
* [IBM Watson Speech-to-Text](https://www.ibm.com/watson/developercloud/speech-to-text.html): Converts audio voice into written text.

## Featured technologies

* [Unity](https://unity3d.com/): A cross-platform game engine used to develop video games for PC, consoles, mobile devices and websites.
* [Google Cardboard](https://vr.google.com/cardboard/): An inexpensive viewer that mounts an Android phone to view VR apps.

# Steps

1. [Before you begin](#1-before-you-begin)
2. [Create IBM Cloud services](#2-create-ibm-cloud-services)
3. [Building and Running](#3-building-and-running)

## 1. Before You Begin

* [IBM Cloud Account](http://ibm.biz/Bdimr6)
* [Unity](https://unity3d.com/get-unity/download)
* [Android Unity SDK](https://docs.unity3d.com/Manual/android-sdksetup.html)

## 2. Create IBM Cloud services

On your local machine:
1. `git clone https://github.com/IBM/vr-speech-sandbox-cardboard.git`
2. `cd vr-speech-sandbox-cardboard`

In [IBM Cloud](https://console.ng.bluemix.net/):

1. Create a [Speech-To-Text](https://console.ng.bluemix.net/catalog/speech-to-text/) service instance.
2. Create a [Assistant](https://console.ng.bluemix.net/catalog/services/conversation/) service instance.
3. Once you see the services in the Dashboard, select the Assistant service you created and click the !["Launch Tool"](/doc/source/images/workspace_launch.png?raw=true) button.
4. After logging into the Assistant Tool, click the !["Import"](/doc/source/images/import_icon.png?raw=true) button.
5. Import the Assistant [`workspace.json`](data/workspace.json) file located in your clone of this repository.

## 3. Building and Running

> Note: This has been compiled and tested using Unity 2017.2.0f3 and Watson Unity SDK from the Unity asset Store (or alternately use the develop branch)

1. Either download the Watson Unity SDK from the Unity asset store or perform the following:
`git clone https://github.com/watson-developer-cloud/unity-sdk.git`
2. Open Unity and inside the project launcher select the ![Open](doc/source/images/unity_open.png?raw=true) button.
3. Navigate to where you cloned this repository and open the `Creation Sandbox` directory.
4. If prompted to upgrade the project to a newer Unity version, do so.
5. Follow [these instructions](https://github.com/watson-developer-cloud/unity-sdk#getting-the-watson-sdk-and-adding-it-to-unity) to add the Watson Unity SDK downloaded in step 1 to the project.
6. Follow [these instructions](https://github.com/watson-developer-cloud/unity-sdk#configuring-your-service-credentials) to create your Speech To Text and Conversation services and find their credentials (using [IBM Cloud](https://console.ng.bluemix.net/)).
 You can find your workspace ID by selecting the expansion menu on your conversation workspace and selecting `View details`.
    ![View Details Location](doc/source/images/workspace_details.png?raw=true)
7. Open the script `vr-speech-sandbox-cardboard/SpeechSandbox/Assets/Scripts/SpeechSandboxStreaming.cs`
8. Fill in the credentials for Speech to Text and Conversation, and the Conversation workspace id:
```
    private string stt_username = "";
    private string stt_password = "";
    // Change stt_url if different from below
    private string stt_url = "https://stream.watsonplatform.net/speech-to-text/api";

    private string convo_username = "";
    private string convo_password = "";
    // Change convo_url if different from below
    private string convo_url = "https://gateway.watsonplatform.net/conversation/api";
    // Change  _conversationVersionDate if different from below
    private string _conversationVersionDate = "2017-05-26";
    private string convo_workspaceId = "";
```

If you followed the previous steps you should already be inside your local clone and ready to get started running the app from Unity.

9. In the Unity editor project tab, select _Assets_->_Scenes_->_Playground_ and double click to load the scene.
10. Press Play
11. To Build an android .apk file and deploy to your phone, you can _File_ -> _Build_ Settings (Ctrl + Shift +B) and click Build.
12. When prompted you can name your build and then move it to your phone.
13. Alternately, connect the phone via USB and _File_-> _Build and Run_ (or Ctrl+B).

   *Make sure you have enabled USB Debugging:*

* Open _Settings_-> _About_-> _Software Information_-> _More_

* Then tap “Build number” seven times to enable Developer options.

* Go back to Settings menu and now you'll be able to see “Developer options” there.

* Tap it and turn on USB Debugging from the menu on the next screen.

   Once the app is deployed to your phone it will start, but you'll need to set permissions for the app before it will work correctly:

  * Open _Settings_-> _Apps_-> _SpeechSandboxCardboard_-> _Permissions_ and enable Microphone and Storage.

# Links

* [Demo on YouTube](https://www.youtube.com/watch?v=rZFpUpy4y0g)
* [Watson Unity SDK](https://github.com/IBM/unity-sdk)

# Learn more

* **Artificial Intelligence Code Patterns**: Enjoyed this Code Pattern? Check out our other [AI Code Patterns](https://developer.ibm.com/code/technologies/artificial-intelligence/).
* **AI and Data Code Pattern Playlist**: Bookmark our [playlist](https://www.youtube.com/playlist?list=PLzUbsvIyrNfknNewObx5N7uGZ5FKH0Fde) with all of our Code Pattern videos
* **With Watson**: Want to take your Watson app to the next level? Looking to utilize Watson Brand assets? [Join the With Watson program](https://www.ibm.com/watson/with-watson/) to leverage exclusive brand, marketing, and tech resources to amplify and accelerate your Watson embedded commercial solution.

# License

[Apache 2.0](LICENSE)
