# 使用 Watson 服务创建 VR 体验的语音命令

*阅读本文的其他语言版本：[English](README.md)。*

在本 Code Pattern 中，我们将基于 Watson [Speech-to-Text](https://www.ibm.com/watson/developercloud/speech-to-text.html) 和 Watson [Conversation](https://www.ibm.com/watson/developercloud/conversation.html) 服务来创建虚拟现实游戏。

在虚拟现实环境（用户感觉真实地“呆在”这个空间内）中，相比其他交互方式，语音交互可能更自然一些。开发人员可利用语音控件来创造沉浸感更强的体验。Google Cardboard 是到目前为止最受欢迎的 VR 头戴式设备平台，其 2016 年的销售量为 8400 万 (http://www.hypergridbusiness.com/2016/11/report-98-of-vr-headsets-sold-this-year-are-for-mobile-phones)。

读者完成本 Code Pattern 后，将会掌握如何：

* 将 IBM Watson Speech-to-Text 和 Conversation 添加到在 Unity 中构建的虚拟现实环境。

![](doc/source/images/architecture.png)

## 操作流程

1.用户在虚拟现实环境中进行交互，并提供语音命令，如“创建一个黑色的大盒子”。

2.Android Phone 麦克风接收语音命令，然后由正在运行的应用程序将该命令发送至 Watson Speech-to-Text。

3.Watson Speech-to-Text 将该音频转换为文本，然后将该文本返回到 Android 手机上正在运行的应用程序。

4.应用程序将该文本发送至 Watson Conversation。Watson Conversation 将返回已识别的意图“创建”以及实体“大”、“黑色”和“盒子”。然后，虚拟现实应用程序将显示一个黑色的大盒子（从天而降）。

# 观看视频

[![](http://img.youtube.com/vi/rZFpUpy4y0g/0.jpg)](http://v.youku.com/v_show/id_XMzQ4MDQ2MjE5Mg==.html)

## 包含的组件

* [IBM Watson Conversation](https://www.ibm.com/watson/developercloud/conversation.html)：使用程序创建一个聊天机器人，让其通过语音或文字方式进行对话。
* [IBM Watson Speech-to-Text](https://www.ibm.com/watson/developercloud/speech-to-text.html)：将语音音频转换为书面文本。

## 精选技术

* [Unity](https://unity3d.com/)：跨平台游戏引擎，用于开发适用于 PC、控制台、移动设备和网站的视频游戏。
* [Google Cardboard](https://vr.google.com/cardboard/)：物美价廉的查看器，与 Android Phone 组合可查看 VR 应用程序。

# 步骤

1.[开始之前](#1-before-you-begin)

2.[创建 IBM Cloud 服务](#2-create-ibm-cloud-services)

3.[构建和运行](#3-building-and-running)

## 1.开始之前

* [IBM Cloud 帐户](http://ibm.biz/Bdimr6)
* [Unity](https://unity3d.com/get-unity/download)
* [Google GVR Unity SDK](https://developers.google.com/vr/unity/get-started)
* [Android Unity SDK](https://docs.unity3d.com/Manual/android-sdksetup.html) 

## 2.创建 IBM Cloud 服务

在您的本地机器上：

1.`git clone https://github.com/IBM/vr-speech-sandbox-cardboard.git`

2.`cd vr-speech-sandbox-cardboard`

在 [IBM Cloud](https://console.ng.bluemix.net/) 中：

1.创建 [Speech-To-Text](https://console.ng.bluemix.net/catalog/speech-to-text/) 服务实例。

2.创建 [Conversation](https://console.ng.bluemix.net/catalog/services/conversation/) 服务实例。

3.在仪表盘中看到服务后，就可以选择您创建的 Conversation 服务，然后单击 !["Launch Tool"](/doc/source/images/workspace_launch.png?raw=true) 按钮。

4.在登录到 Conversation Tool 后，单击 !["Import"](/doc/source/images/import_icon.png?raw=true) 按钮。

5.导入该存储库克隆副本中的 Conversation [`workspace.json`](data/workspace.json) 文件。

## 3.构建和运行

如果您执行了前面的步骤，那么此时应该已经在本地克隆副本内，并且准备好开始从 Unity 运行应用程序。

1.`git clone https://github.com/IBM/unity-sdk`

2.打开 Unity，在项目启动程序中选择 ![Open](doc/source/images/unity_open.png?raw=true) 按钮。

3.浏览至将此存储库克隆到的位置，然后打开“SpeechSandbox”目录。

4.如果系统提示您将该项目升级至更新的 Unity 版本，请进行升级。

5.您必须将 Build Settings 更改为 Android，才会显示 Watson unity-sdk 的 Configuration 选项卡。转至 _File_ -> _Build_ Settings (Ctrl + Shift +B)，将 Platform 更改为 `Android`，然后单击 `Switch Platform`。

6.遵循[这些指示信息](https://github.com/IBM/unity-sdk#getting-the-watson-sdk-and-adding-it-to-unity)将步骤 1 中下载的 Watson Unity SDK 添加到该项目。

7.遵循[这些指示信息](https://github.com/IBM/unity-sdk#configuring-your-service-credentials)来添加 Speech To Text 和 Conversation 服务凭证（位于 [IBM Cloud](https://console.ng.bluemix.net/) 中）。

8.在配置窗口中选择 `Advanced Mode`。

9.打开脚本 vr-speech-sandbox-cardboard/SpeechSandbox/Assests/Scripts/VoiceSpawner.cs，将工作空间标识放在第 #34 行 Start() 方法中。
 您可以通过在 Conversation 工作空间上选择扩展菜单并选择 `View details` 来查找工作空间标识。
    ![View Details Location](doc/source/images/workspace_details.png?raw=true)
    
10.在 Unity 编辑器的 Project 选项卡上，选择 _Assets_->_Scenes_->_Playground_，然后双击以加载该场景。

11.按 Play

12.要构建 android .apk 文件并将其部署到手机上，可以单击 _File_ -> _Build_ Settings (Ctrl + Shift +B)，然后单击 Build。 

13.在出现提示时，您可以命名该构建，然后将其移至手机上。

14.或者，可以通过 USB 来连接手机并单击 _File_-> _Build 和 Run_（或 Ctrl+B）。

   *确保您已启用 USB Debugging：*
     
* 打开 _Settings_-> _About_-> _Software Information_-> _More_

* 然后，点击“Build number”七次以启用 Developer 选项。

* 返回至 Settings 菜单，现在您可以在此处看到“Developer options”。

* 点击它，并在下一个屏幕上的菜单中开启 USB Debugging。

   将应用程序部署到手机上后，它会立即启动，但您需要为该应用程序设置许可权，然后它才能正常工作：

  * 打开 _Settings_-> _Apps_-> _SpeechSandboxCardboard_-> _Permissions_，然后启用 Microphone and Storage。

# 链接

* [youku 上的演示](http://v.youku.com/v_show/id_XMzQ4MDQ2MjE5Mg==.html)
* [Watson Unity SDK](https://github.com/IBM/unity-sdk)

# 了解更多信息

* **人工智能 Code Pattern**：喜欢本 Code Pattern 吗？了解我们的其他 [AI Code Pattern](https://developer.ibm.com/cn/journey/category/artificial-intelligence/)。
* **AI 和数据 Code Pattern 播放清单**：收藏包含我们所有 Code Pattern 视频的[播放清单](http://i.youku.com/i/UNTI2NTA2NTAw/videos?spm=a2hzp.8244740.0.0) 
* **With Watson**：想要进一步改进您的 Watson 应用程序？正在考虑使用 Watson 品牌资产？[加入 With Watson 计划](https://www.ibm.com/watson/with-watson/)，以便利用独家品牌、营销和技术资源来增强和加速您的 Watson 嵌入式商业解决方案。

# 许可

[Apache 2.0](LICENSE)
