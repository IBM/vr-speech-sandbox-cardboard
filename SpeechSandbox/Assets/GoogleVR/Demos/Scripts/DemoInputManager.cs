// Copyright 2016 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine;
using UnityEngine.UI;
using System;

public class DemoInputManager : MonoBehaviour {
// Build for iOS, or for a pre-native integration Unity version, for Android, and running on-device.
#if UNITY_IOS || (!UNITY_HAS_GOOGLEVR && !UNITY_5_6_OR_NEWER && UNITY_ANDROID && !UNITY_EDITOR)
  void Start() {
    GameObject messageCanvas = transform.Find("MessageCanvas").gameObject;
    messageCanvas.SetActive(false);
  }
#endif  // UNITY_IOS || (!UNITY_HAS_GOOGLEVR && !UNITY_5_6_OR_NEWER && UNITY_ANDROID && !UNITY_EDITOR)

// Cardboard / Daydream switching does not apply to pre-native integration versions
// of Unity, or platforms other than Android, since those are Cardboard-only.
#if UNITY_HAS_GOOGLEVR && UNITY_ANDROID
  private const string MESSAGE_CANVAS_NAME = "MessageCanvas";
  private const string MESSAGE_TEXT_NAME = "MessageText";
  private const string LASER_GAMEOBJECT_NAME = "Laser";

  private const string CONTROLLER_CONNECTING_MESSAGE = "Controller connecting...";
  private const string CONTROLLER_DISCONNECTED_MESSAGE = "Controller disconnected";
  private const string CONTROLLER_SCANNING_MESSAGE =  "Controller scanning...";
  private const string EMPTY_VR_SDK_WARNING_MESSAGE =
    "Please enable a VR SDK in Player Settings > Virtual Reality Supported\n";

  // Java class, method, and field constants.
  private const int ANDROID_MIN_DAYDREAM_API = 24;
  private const string FIELD_SDK_INT = "SDK_INT";
  private const string PACKAGE_BUILD_VERSION = "android.os.Build$VERSION";
  private const string PACKAGE_DAYDREAM_API_CLASS = "com.google.vr.ndk.base.DaydreamApi";
  private const string PACKAGE_UNITY_PLAYER = "com.unity3d.player.UnityPlayer";
  private const string METHOD_CURRENT_ACTIVITY = "currentActivity";
  private const string METHOD_IS_DAYDREAM_READY = "isDaydreamReadyPlatform";

  private bool isDaydream = false;

  public static string CARDBOARD_DEVICE_NAME = "cardboard";
  public static string DAYDREAM_DEVICE_NAME = "daydream";

  [Tooltip("Reference to GvrControllerMain")]
  public GameObject controllerMain;
  public static string CONTROLLER_MAIN_PROP_NAME = "controllerMain";

  [Tooltip("Reference to GvrControllerPointer")]
  public GameObject controllerPointer;
  public static string CONTROLLER_POINTER_PROP_NAME = "controllerPointer";

  [Tooltip("Reference to GvrReticlePointer")]
  public GameObject reticlePointer;
  public static string RETICLE_POINTER_PROP_NAME = "reticlePointer";

  public GameObject messageCanvas;
  public Text messageText;

#if UNITY_EDITOR
  public enum EmulatedPlatformType {
    Daydream,
    Cardboard
  }
  // Cardboard by default if there is no native integration.
  [Tooltip("Emulated GVR Platform")]
  public EmulatedPlatformType gvrEmulatedPlatformType = EmulatedPlatformType.Daydream;
  public static string EMULATED_PLATFORM_PROP_NAME = "gvrEmulatedPlatformType";
#else
  private GvrSettings.ViewerPlatformType viewerPlatform;
#endif  // UNITY_EDITOR

  void Start() {
    if (messageCanvas == null) {
      messageCanvas = transform.Find(MESSAGE_CANVAS_NAME).gameObject;
      if (messageCanvas != null) {
        messageText = messageCanvas.transform.Find(MESSAGE_TEXT_NAME).GetComponent<Text>();
      }
    }
#if UNITY_EDITOR
    if (playerSettingsHasDaydream() || playerSettingsHasCardboard()) {
      // The list is populated with valid VR SDK(s), pick the first one.
      gvrEmulatedPlatformType =
        (UnityEngine.XR.XRSettings.supportedDevices[0] == DAYDREAM_DEVICE_NAME) ?
        EmulatedPlatformType.Daydream :
        EmulatedPlatformType.Cardboard;
    }
    isDaydream = (gvrEmulatedPlatformType == EmulatedPlatformType.Daydream);
#else
    viewerPlatform = GvrSettings.ViewerPlatform;
    // First loaded device in Player Settings.
    string vrDeviceName = UnityEngine.XR.XRSettings.loadedDeviceName;
    if (vrDeviceName != CARDBOARD_DEVICE_NAME &&
        vrDeviceName != DAYDREAM_DEVICE_NAME) {
      Debug.Log(string.Format("Loaded device was {0} must be one of {1} or {2}",
            vrDeviceName, DAYDREAM_DEVICE_NAME, CARDBOARD_DEVICE_NAME));
      return;
    }

    // On a non-Daydream ready phone, fall back to Cardboard if it's present in the list of
    // enabled VR SDKs.
    // On a Daydream-ready phone, go into Cardboard mode if it's the currently-paired viewer.
    if ((!IsDeviceDaydreamReady() && playerSettingsHasCardboard()) ||
        (IsDeviceDaydreamReady() && playerSettingsHasCardboard() &&
         GvrSettings.ViewerPlatform == GvrSettings.ViewerPlatformType.Cardboard)) {
      vrDeviceName = CARDBOARD_DEVICE_NAME;
    }
    isDaydream = (vrDeviceName == DAYDREAM_DEVICE_NAME);
#endif  // UNITY_EDITOR
    SetVRInputMechanism();
  }

  // Runtime switching enabled only in-editor.
  void Update() {
    UpdateStatusMessage();

#if UNITY_EDITOR
    UpdateEmulatedPlatformIfPlayerSettingsChanged();
    if ((isDaydream && gvrEmulatedPlatformType == EmulatedPlatformType.Daydream) ||
        (!isDaydream && gvrEmulatedPlatformType == EmulatedPlatformType.Cardboard)) {
      return;
    }
    isDaydream = (gvrEmulatedPlatformType == EmulatedPlatformType.Daydream);
    SetVRInputMechanism();
#else
    // Viewer type switched at runtime.
    if (!IsDeviceDaydreamReady() || viewerPlatform == GvrSettings.ViewerPlatform) {
      return;
    }
    isDaydream = (GvrSettings.ViewerPlatform == GvrSettings.ViewerPlatformType.Daydream);
    viewerPlatform = GvrSettings.ViewerPlatform;
    SetVRInputMechanism();
#endif  // UNITY_EDITOR
  }

  public bool IsCurrentlyDaydream() {
    return isDaydream;
  }

  public static bool playerSettingsHasDaydream() {
    string[] playerSettingsVrSdks = UnityEngine.XR.XRSettings.supportedDevices;
    return Array.Exists<string>(playerSettingsVrSdks,
        element => element.Equals(DemoInputManager.DAYDREAM_DEVICE_NAME));
  }

  public static bool playerSettingsHasCardboard() {
    string[] playerSettingsVrSdks = UnityEngine.XR.XRSettings.supportedDevices;
    return Array.Exists<string>(playerSettingsVrSdks,
        element => element.Equals(DemoInputManager.CARDBOARD_DEVICE_NAME));
  }

#if UNITY_EDITOR
  private void UpdateEmulatedPlatformIfPlayerSettingsChanged() {
    if (!playerSettingsHasDaydream() && !playerSettingsHasCardboard()) {
      return;
    }

    // Player Settings > VR SDK list may have changed at runtime. The emulated platform
    // may not have been manually updated if that's the case.
    if (gvrEmulatedPlatformType == EmulatedPlatformType.Daydream &&
        !playerSettingsHasDaydream()) {
      gvrEmulatedPlatformType = EmulatedPlatformType.Cardboard;
    } else if (gvrEmulatedPlatformType == EmulatedPlatformType.Cardboard &&
               !playerSettingsHasCardboard()) {
      gvrEmulatedPlatformType = EmulatedPlatformType.Daydream;
    }
  }
#endif  // UNITY_EDITOR

#if !UNITY_EDITOR  // Running on an Android device.
  private static bool IsDeviceDaydreamReady() {
    // Check API level.
    using (var version = new AndroidJavaClass(PACKAGE_BUILD_VERSION)) {
      if (version.GetStatic<int>(FIELD_SDK_INT) < ANDROID_MIN_DAYDREAM_API) {
        return false;
      }
    }
    // API level > 24, check whether the device is Daydream-ready..
    AndroidJavaObject androidActivity = null;
    try {
      using (AndroidJavaObject unityPlayer = new AndroidJavaClass(PACKAGE_UNITY_PLAYER)) {
        androidActivity = unityPlayer.GetStatic<AndroidJavaObject>(METHOD_CURRENT_ACTIVITY);
      }
    } catch (AndroidJavaException e) {
      Debug.LogError("Exception while connecting to the Activity: " + e);
      return false;
    }
    AndroidJavaClass daydreamApiClass = new AndroidJavaClass(PACKAGE_DAYDREAM_API_CLASS);
    if (daydreamApiClass == null || androidActivity == null) {
      return false;
    }
    return daydreamApiClass.CallStatic<bool>(METHOD_IS_DAYDREAM_READY, androidActivity);
  }
#endif  // !UNITY_EDITOR

  private void UpdateStatusMessage() {
    if (messageText == null || messageCanvas == null) {
      return;
    }
    bool isVrSdkListEmpty = !playerSettingsHasCardboard() && !playerSettingsHasDaydream();
    if (!isDaydream) {
      if (messageCanvas.activeSelf) {
        messageText.text = EMPTY_VR_SDK_WARNING_MESSAGE;
        messageCanvas.SetActive(false || isVrSdkListEmpty);
      }
      return;
    }

    string vrSdkWarningMessage = isVrSdkListEmpty ? EMPTY_VR_SDK_WARNING_MESSAGE : "";
    string controllerMessage = "";
    GvrPointerGraphicRaycaster graphicRaycaster =
      messageCanvas.GetComponent<GvrPointerGraphicRaycaster>();
    // This is an example of how to process the controller's state to display a status message.
    switch (GvrController.State) {
      case GvrConnectionState.Connected:
        break;
      case GvrConnectionState.Disconnected:
        controllerMessage = CONTROLLER_DISCONNECTED_MESSAGE;
        messageText.color = Color.white;
        break;
      case GvrConnectionState.Scanning:
        controllerMessage = CONTROLLER_SCANNING_MESSAGE;
        messageText.color = Color.cyan;
        break;
      case GvrConnectionState.Connecting:
        controllerMessage = CONTROLLER_CONNECTING_MESSAGE;
        messageText.color = Color.yellow;
        break;
      case GvrConnectionState.Error:
        controllerMessage = "ERROR: " + GvrController.ErrorDetails;
        messageText.color = Color.red;
        break;
      default:
        // Shouldn't happen.
        Debug.LogError("Invalid controller state: " + GvrController.State);
        break;
    }
    messageText.text = string.Format("{0}{1}", vrSdkWarningMessage, controllerMessage);
    if (graphicRaycaster != null) {
      graphicRaycaster.enabled =
        !isVrSdkListEmpty || GvrController.State != GvrConnectionState.Connected;
    }
    messageCanvas.SetActive(isVrSdkListEmpty ||
                            (GvrController.State != GvrConnectionState.Connected));
  }

  private void SetVRInputMechanism() {
    SetGazeInputActive(!isDaydream);
    SetControllerInputActive(isDaydream);
  }

  private void SetGazeInputActive(bool active) {
    if (reticlePointer == null) {
      return;
    }
    reticlePointer.SetActive(active);

    // Update the pointer type only if this is currently activated.
    if (!active) {
      return;
    }

    GvrReticlePointer pointer =
        reticlePointer.GetComponent<GvrReticlePointer>();
    if (pointer != null) {
      pointer.SetAsMainPointer();
    }
  }

  private void SetControllerInputActive(bool active) {
    if (controllerMain != null) {
      controllerMain.SetActive(active);
    }
    if (controllerPointer == null) {
      return;
    }
    controllerPointer.SetActive(active);

    // Update the pointer type only if this is currently activated.
    if (!active) {
      return;
    }
    GvrLaserPointer pointer =
        controllerPointer.GetComponentInChildren<GvrLaserPointer>(true);
    if (pointer != null) {
      pointer.SetAsMainPointer();
    }
  }

#endif  // UNITY_HAS_GOOGLEVR && UNITY_ANDROID
}
