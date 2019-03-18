﻿/**
* Copyright 2015 IBM Corp. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/
#pragma warning disable 0649

using UnityEngine;
using System.Collections;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.DataTypes;
using System.Collections.Generic;
using UnityEngine.UI;
using IBM.Watson.DeveloperCloud.Services.Conversation.v1;
using FullSerializer;
using IBM.Watson.DeveloperCloud.Connection;

public class SpeechSandboxStreaming : MonoBehaviour
{

    public GameManager gameManager;
    public AudioClip sorryClip;
    public List<AudioClip> helpClips;

    [SerializeField]
    private fsSerializer _serializer = new fsSerializer();

    #region PLEASE SET THESE VARIABLES IN THE INSPECTOR
    [Space(10)]
    [Header("Speech To Text")]
    [Tooltip("The service URL (optional). This defaults to \"https://stream.watsonplatform.net/speech-to-text/api\"")]
    [SerializeField]
    private string speechToTextServiceUrl;
    [Header("IAM Authentication")]
    [Tooltip("The IAM apikey.")]
    [SerializeField]
    private string speechToTextIamApikey;
    [Header("Parameters")]
    // https://www.ibm.com/watson/developercloud/speech-to-text/api/v1/curl.html?curl#get-model
    [Tooltip("The Model to use. This defaults to en-US_BroadbandModel")]
    [SerializeField]
    private string speechToTextRecognizeModel;

    [Space(10)]
    [Header("Watson Assistant")]
    [Tooltip("The service URL(optional). This defaults to \"https://gateway.watsonplatform.net/assistant/api\"")]
    [SerializeField]
    private string assistantServiceUrl;
    [Tooltip("The workspaceId")]
    [SerializeField]
    private string assistantWorkspaceId;
    [Tooltip("The version date with which you would like to use the service in the form YYYY-MM-DD.")]
    [SerializeField]
    private string assistantVersionDate;
    [Header("IAM Authentication")]
    [Tooltip("The IAM apikey.")]
    [SerializeField]
    private string assistantIamApikey;
    #endregion


    private int _recordingRoutine = 0;
    private string _microphoneID = null;
    private AudioClip _recording = null;
    private int _recordingBufferSize = 1;
    private int _recordingHZ = 22050;

    private SpeechToText _speechToText;
    private Conversation _conversation;

    void Start()
    {
        LogSystem.InstallDefaultReactors();


        //  Create credential and instantiate service
        Runnable.Run(createServices());
    }


    private IEnumerator createServices(){

        Credentials stt_credentials = null;
        //  Create credential and instantiate service
        if (!string.IsNullOrEmpty(speechToTextIamApikey))
        {
            //  Authenticate using iamApikey
            TokenOptions tokenOptions = new TokenOptions()
            {
                IamApiKey = speechToTextIamApikey,
            };

            stt_credentials = new Credentials(tokenOptions, speechToTextServiceUrl);

            while (!stt_credentials.HasIamTokenData())
                yield return null;
        }
        else
        {
            throw new WatsonException("Please provide either username and password or IAM apikey to authenticate the service.");
        }

        Credentials asst_credentials = null;
        //  Create credential and instantiate service
        if (!string.IsNullOrEmpty(assistantIamApikey))
        {
            //  Authenticate using iamApikey
            TokenOptions tokenOptions = new TokenOptions()
            {
                IamApiKey = assistantIamApikey,
            };

            asst_credentials = new Credentials(tokenOptions, assistantServiceUrl);

            while (!asst_credentials.HasIamTokenData())
                yield return null;
        }
        else
        {
            throw new WatsonException("Please provide either username and password or IAM apikey to authenticate the service.");
        }


        _speechToText = new SpeechToText(stt_credentials);
        _conversation = new Conversation(asst_credentials);

        _conversation.VersionDate = assistantVersionDate;
        Active = true;

        StartRecording();
    }

    
    
    public bool Active
    {
        get { return _speechToText.IsListening; }
        set
        {
            if (value && !_speechToText.IsListening)
            {
                _speechToText.DetectSilence = true;
                _speechToText.EnableWordConfidence = true;
                _speechToText.EnableTimestamps = true;
                _speechToText.SilenceThreshold = 0.01f;
                _speechToText.MaxAlternatives = 0;
                _speechToText.EnableInterimResults = true;
                _speechToText.OnError = OnError;
                _speechToText.InactivityTimeout = -1;
                _speechToText.ProfanityFilter = false;
                _speechToText.SmartFormatting = true;
                _speechToText.SpeakerLabels = false;
                _speechToText.WordAlternativesThreshold = null;
                _speechToText.StartListening(OnRecognize, OnRecognizeSpeaker);
            }
            else if (!value && _speechToText.IsListening)
            {
                _speechToText.StopListening();
            }
        }
    }

    private void StartRecording()
    {
        if (_recordingRoutine == 0)
        {
            UnityObjectUtil.StartDestroyQueue();
            _recordingRoutine = Runnable.Run(RecordingHandler());
        }
    }

    private void StopRecording()
    {
        if (_recordingRoutine != 0)
        {
            Microphone.End(_microphoneID);
            Runnable.Stop(_recordingRoutine);
            _recordingRoutine = 0;
        }
    }

   private void OnError(string error)
    {
        Active = false;

        Log.Debug("ExampleStreaming.OnError()", "Error! {0}", error);
    }

    private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
    {
        Log.Error("ExampleConversation.OnFail()", "Error received: {0}", error.ToString());
    }


    private IEnumerator RecordingHandler()
    {
        Log.Debug("ExampleStreaming.RecordingHandler()", "devices: {0}", Microphone.devices);
        _recording = Microphone.Start(_microphoneID, true, _recordingBufferSize, _recordingHZ);
        yield return null;      // let _recordingRoutine get set..

        if (_recording == null)
        {
            StopRecording();
            yield break;
        }

        bool bFirstBlock = true;
        int midPoint = _recording.samples / 2;
        float[] samples = null;

        while (_recordingRoutine != 0 && _recording != null)
        {
            int writePos = Microphone.GetPosition(_microphoneID);
            if (writePos > _recording.samples || !Microphone.IsRecording(_microphoneID))
            {
                Log.Error("ExampleStreaming.RecordingHandler()", "Microphone disconnected.");

                StopRecording();
                yield break;
            }

            if ((bFirstBlock && writePos >= midPoint)
              || (!bFirstBlock && writePos < midPoint))
            {
                // front block is recorded, make a RecordClip and pass it onto our callback.
                samples = new float[midPoint];
                _recording.GetData(samples, bFirstBlock ? 0 : midPoint);

                AudioData record = new AudioData();
				record.MaxLevel = Mathf.Max(Mathf.Abs(Mathf.Min(samples)), Mathf.Max(samples));
                record.Clip = AudioClip.Create("Recording", midPoint, _recording.channels, _recordingHZ, false);
                record.Clip.SetData(samples, 0);

                _speechToText.OnListen(record);

                bFirstBlock = !bFirstBlock;
            }
            else
            {
                // calculate the number of samples remaining until we ready for a block of audio,
                // and wait that amount of time it will take to record.
                int remaining = bFirstBlock ? (midPoint - writePos) : (_recording.samples - writePos);
                float timeRemaining = (float)remaining / (float)_recordingHZ;

                yield return new WaitForSeconds(timeRemaining);
            }

        }

        yield break;
    }


    private void OnRecognize(SpeechRecognitionEvent result, Dictionary<string, object> customData = null)
    {
        if (result != null && result.results.Length > 0)
        {
            foreach (var res in result.results)
            {
                foreach (var alt in res.alternatives)
                {
                    if (res.final && alt.confidence > 0)
                    {
                        string text = alt.transcript;
                        Debug.Log("Result: " + text + " Confidence: " + alt.confidence);
                        _conversation.Message(OnMessage, OnFail, assistantWorkspaceId, text);
                    }
                }

                if (res.keywords_result != null && res.keywords_result.keyword != null)
                {
                    foreach (var keyword in res.keywords_result.keyword)
                    {
                        Log.Debug("ExampleStreaming.OnRecognize()", "keyword: {0}, confidence: {1}, start time: {2}, end time: {3}", keyword.normalized_text, keyword.confidence, keyword.start_time, keyword.end_time);
                    }
                }

                if (res.word_alternatives != null)
                {
                    foreach (var wordAlternative in res.word_alternatives)
                    {
                        Log.Debug("ExampleStreaming.OnRecognize()", "Word alternatives found. Start time: {0} | EndTime: {1}", wordAlternative.start_time, wordAlternative.end_time);
                        foreach(var alternative in wordAlternative.alternatives)
                            Log.Debug("ExampleStreaming.OnRecognize()", "\t word: {0} | confidence: {1}", alternative.word, alternative.confidence);
                    }
                }
            }
        }
    }

    void OnMessage(object resp, Dictionary<string, object> customData)
    {
        //  Convert resp to fsdata

        fsData fsdata = null;
        fsResult r = _serializer.TrySerialize(resp.GetType(), resp, out fsdata);
        if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);

        //  Convert fsdata to MessageResponse
        MessageResponse messageResponse = new MessageResponse();
        object obj = messageResponse;
        r = _serializer.TryDeserialize(fsdata, obj.GetType(), ref obj);
        if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);

        if (resp != null && (messageResponse.intents.Length > 0 || messageResponse.entities.Length > 0))
        {
            string intent = messageResponse.intents[0].intent;
            Debug.Log("Intent: " + intent);
            string currentMat = null;
            string currentScale = null;
            string direction = null;
            if (intent == "move")
            {
                foreach (RuntimeEntity entity in messageResponse.entities)
                {
                    Debug.Log("entityType: " + entity.entity + " , value: " + entity.value);
                    direction = entity.value;
                    gameManager.MoveObject(direction);
                }
            }

            if (intent == "create")
            {
                bool createdObject = false;
                foreach (RuntimeEntity entity in messageResponse.entities)
                {
                    Debug.Log("entityType: " + entity.entity + " , value: " + entity.value);
                    if (entity.entity == "material")
                    {
                        currentMat = entity.value;
                    }
                    if (entity.entity == "scale")
                    {
                        currentScale = entity.value;
                    }
                    else if (entity.entity == "object")
                    {
                        gameManager.CreateObject(entity.value, currentMat, currentScale);
                        createdObject = true;
                        currentMat = null;
                        currentScale = null;
                    }
                }

                if (!createdObject)
                {
                    gameManager.PlayError(sorryClip);
                }
            }
            else if (intent == "destroy")
            {
                gameManager.DestroyAtPointer();
            }
            else if (intent == "help")
            {
                if (helpClips.Count > 0)
                {
                    gameManager.PlayClip(helpClips[Random.Range(0, helpClips.Count)]);
                }
            }
        }
        else
        {
            Debug.Log("Failed to invoke OnMessage();");
        }
    }

    private void OnRecognizeSpeaker(SpeakerRecognitionEvent result, Dictionary<string, object> customData)
    {
        if (result != null)
        {
            foreach (SpeakerLabelsResult labelResult in result.speaker_labels)
            {
                Log.Debug("ExampleStreaming.OnRecognizeSpeaker()", string.Format("speaker result: {0} | confidence: {3} | from: {1} | to: {2}", labelResult.speaker, labelResult.from, labelResult.to, labelResult.confidence));
            }
        }
    }
}