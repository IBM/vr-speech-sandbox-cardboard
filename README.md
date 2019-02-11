# Create voice commands for VR experiences with Watson services

*Read this in other languages: [한국어](README-ko.md), [中国](README-cn.md).*

In this Code Pattern we will create a Virtual Reality game based on Watson's [Speech-to-Text](https://www.ibm.com/watson/services/speech-to-text/) and Watson's [Assistant](https://www.ibm.com/cloud/watson-assistant/) services.

In Virtual Reality, where you truly "inhabit" the space, speech can feel like a more natural interface than other methods. Providing speech controls allows developers to create more immersive experiences. Google Cardboard is by far the most popular VR headset platform, with 84 million sold in 2016 (http://www.hypergridbusiness.com/2016/11/report-98-of-vr-headsets-sold-this-year-are-for-mobile-phones).

When the reader has completed this Code Pattern, they will understand how to:

* Add IBM Watson Speech-to-Text and Assistant to a Virtual Reality environment build in Unity.

![architecture](doc/source/images/architecture.png)

## Flow

1. User interacts in virtual reality and gives voice commands such as "Create a large black box".
2. The Android phone microphone picks up the voice command and the running application sends it to Watson Speech-to-Text.
3. Watson Speech-to-Text converts the audio to text and returns it to the running Application on the Android phone.
4. The application sends the text to Watson Assistant. Watson assistant returns the recognized intent "Create" and the entities "large", "black", and "box". The virtual reality application then displays the large black box (which falls from the sky).

# Watch the Video

[![video](https://i.ytimg.com/vi/OsbV1xqX0hQ/0.jpg)](https://youtu.be/OsbV1xqX0hQ)
## Included components

* [IBM Watson Assistant](https://www.ibm.com/cloud/watson-assistant/): Create a chatbot with a program that conducts a conversation via auditory or textual methods.
* [IBM Watson Speech-to-Text](https://www.ibm.com/watson/services/speech-to-text/): Converts audio voice into written text.

## Featured technologies

* [Unity](https://unity3d.com/): A cross-platform game engine used to develop video games for PC, consoles, mobile devices and websites.
* [Google Cardboard](https://vr.google.com/cardboard/): An inexpensive viewer that mounts an Android phone to view VR apps.

# Steps

1. [Before you begin](#1-before-you-begin)
2. [Create IBM Cloud services](#2-create-ibm-cloud-services)
3. [Building and Running](#3-building-and-running)

## 1. Before You Begin

* [IBM Cloud Account](https://www.ibm.com/cloud/)
* [Unity](https://unity3d.com/get-unity/download)
* [Android Unity SDK](https://docs.unity3d.com/Manual/android-sdksetup.html)

## 2. Create IBM Cloud services

On your local machine:
```bash
git clone https://github.com/IBM/vr-speech-sandbox-cardboard.git
cd vr-speech-sandbox-cardboard
```

In [IBM Cloud](https://cloud.ibm.com):

* Create a [Speech-To-Text](https://cloud.ibm.com/catalog/services/speech-to-text) service instance.
* Create an [Assistant](https://cloud.ibm.com/catalog/services/conversation/) service instance.

#### Import the Assistant workspace.json:

* Find the Assistant service in your IBM Cloud Dashboard.
* Click on the service and then click on `Launch tool`.
* Go to the `Skills` tab.
* Click `Create new`
* Click the `Import skill` tab.
* Click `Choose JSON file`, go to your cloned repo dir, and `Open` the workspace.json file in [`data/workspace.json`](data/workspace.json).
* Select `Everything` and click `Import`.

To find the `WORKSPACE_ID` for Watson Assistant:

* Go back to the `Skills` tab.
* Find the card for the workspace you would like to use. Look for `IBM Speech Sandbox Cardboard`.
* Click on the three dots in the upper right-hand corner of the card and select `View API Details`.
* Copy the `Workspace ID` GUID. Save it configuration later.

!["Get Workspace ID"](https://github.com/IBM/pattern-utils/blob/master/watson-assistant/assistantPostSkillGetID.gif)

## 3. Building and Running

> Note: This has been compiled and tested using Unity 2018.3.0f2 and Watson Unity SDK from the Unity asset Store as of December 19, 2018 and tested with the `develop` branch of the github unity-sdk as of `commit b626161399 Dec 19 2018`.

> Note: If you are in *any* IBM Cloud region other than US-South you *must* use Unity 2018.2 or higher. This is because Unity 2018.2 or higher is needed for TLS 1.2, which is the only TLS version available in all regions other than US-South.

* Navigate to the `SpeechSandbox/Assets` directory:

```bash
cd SpeechSandbox/Assets
```

* Download the [Watson SDK for Unity](https://github.com/watson-developer-cloud/unity-sdk) or perform the following:

```bash
git clone https://github.com/watson-developer-cloud/unity-sdk.git
```

Make sure you are on the develop branch.
1. Open Unity and inside the project launcher select the ![Open](doc/source/images/unity_open.png?raw=true) button.
1. Navigate to where you cloned this repository and open the `SpeechSandbox` directory.
1. If prompted to upgrade the project to a newer Unity version, do so.
1. Follow [these instructions](https://github.com/watson-developer-cloud/unity-sdk#getting-the-watson-sdk-and-adding-it-to-unity) to add the Watson Unity SDK downloaded in step 1 to the project.
1. Follow [these instructions](https://github.com/watson-developer-cloud/unity-sdk#configuring-your-service-credentials) to create your Speech To Text and Watson Assistant services and find their credentials (using [IBM Cloud](https://cloud.ibm.com)
1. In the Unity Hierarchy view, click on `Assets` -> `Scenes` -> `Playground` and double-click to load the scene.
1. In the Unity Hierarchy view, click on `Player` and then the `Streaming Speech` object.
1. In the Inspector you will see Variables for `Speech To Text` and `Watson Assistant` and either `CF Authentication` for the Cloud Foundry username and password, or the `IAM Authentication` if you have the IAM apikey. Since you only have only one version of these credentials, fill out only one of the two for each service.
1. Fill out the `Speech To Text Service Url`, the `Assistant Service Url`, the `Assistant Workspace Id`, and the `Assistant Version Date`. There are tool tips which will show help and any defaults.

![](https://github.com/IBM/pattern-images/blob/master/Unity/UnitySpeechSandboxCreds.png)

1. Press Play
1. To Build an android .apk file and deploy to your phone, you can _File_ -> _Build_ Settings (Ctrl + Shift +B) and click Build.
1. When prompted you can name your build and then move it to your phone.
1. Alternately, connect the phone via USB and _File_-> _Build and Run_ (or Ctrl+B).

   *Make sure you have enabled USB Debugging:*

* Open _Settings_-> _About_-> _Software Information_-> _More_

* Then tap “Build number” seven times to enable Developer options.

* Go back to Settings menu and now you'll be able to see “Developer options” there.

* Tap it and turn on USB Debugging from the menu on the next screen.

   Once the app is deployed to your phone it will start, but you'll need to set permissions for the app before it will work correctly:

  * Open _Settings_-> _Apps_-> _SpeechSandboxCardboard_-> _Permissions_ and enable Microphone and Storage.

# Links

* [Demo on YouTube](https://youtu.be/OsbV1xqX0hQ)
* [Watson Unity SDK](https://github.com/watson-developer-cloud/unity-sdk)

# Troubleshooting

* If you see:
```
Assets/Scripts/CreatableObject.cs(162,43): error CS0117: `GvrController' does not contain a definition for `Gyro'
```
This is because you have your `Build Settings` -> `Platform` set to `PC, Mac, or Linux Standalone`. Solve this problem by changing the `Build Settings` -> `Platform` to `Android` and clicking `Switch Platform`.

* If you see:
```
Assets/unity-sdk/Scripts/Utilities/Credentials.cs(399,51): error CS1061: Type `System.DateTimeOffset' does not contain a definition for `ToUnixTimeSeconds' and no extension method `ToUnixTimeSeconds' of type `System.DateTimeOffset' could be found. Are you missing an assembly reference?
```
The solution is to go to `Build Settings`->`Player Settings`->
`Other Settings`->`Scripting Runtime Version` and change to:
`Experimental(.NET 4.6 Equivalent)`.

# Learn more

* **Artificial Intelligence Code Patterns**: Enjoyed this Code Pattern? Check out our other [AI Code Patterns](https://developer.ibm.com/technologies/artificial-intelligence/).
* **AI and Data Code Pattern Playlist**: Bookmark our [playlist](https://www.youtube.com/playlist?list=PLzUbsvIyrNfknNewObx5N7uGZ5FKH0Fde) with all of our Code Pattern videos
* **With Watson**: Want to take your Watson app to the next level? Looking to utilize Watson Brand assets? [Join the With Watson program](https://www.ibm.com/watson/with-watson/) to leverage exclusive brand, marketing, and tech resources to amplify and accelerate your Watson embedded commercial solution.

# License

This code pattern is licensed under the Apache Software License, Version 2.  Separate third party code objects invoked within this code pattern are licensed by their respective providers pursuant to their own separate licenses. Contributions are subject to the [Developer Certificate of Origin, Version 1.1 (DCO)](https://developercertificate.org/) and the [Apache Software License, Version 2](https://www.apache.org/licenses/LICENSE-2.0.txt).

[Apache Software License (ASL) FAQ](https://www.apache.org/foundation/license-faq.html#WhatDoesItMEAN)
