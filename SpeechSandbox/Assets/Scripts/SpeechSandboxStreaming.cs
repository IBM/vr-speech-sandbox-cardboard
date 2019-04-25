/**
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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using IBM.Watson.SpeechToText.V1;
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Utilities;
using IBM.Cloud.SDK.DataTypes;
using IBM.Watson.Assistant.V2.Model;
using IBM.Watson.Assistant.V2;

public class SpeechSandboxStreaming : MonoBehaviour
{

    public GameManager gameManager;
    public AudioClip sorryClip;
    public List<AudioClip> helpClips;

    [SerializeField]

    #region PLEASE SET THESE VARIABLES IN THE INSPECTOR
    [Header("Speech To Text")]
    [Tooltip("The service URL (optional). This defaults to \"https://stream.watsonplatform.net/speech-to-text/api\"")]
    private string speechToTextServiceUrl = "";
    [Tooltip("The IAM apikey.")]
    [SerializeField]
    private string speechToTextIamApikey;
    [Tooltip("The IAM url used to authenticate the apikey (optional). This defaults to \"https://iam.bluemix.net/identity/token\".")]
    [SerializeField]
    private string speechToTextIamUrl;

    [Header("Watson Assistant")]
    [Tooltip("The service URL (optional). This defaults to \"https://gateway.watsonplatform.net/assistant/api\"")]
    [SerializeField]
    private string assistantServiceUrl;
    [Tooltip("The Assistant ID to run the example.")]
    [SerializeField]
    private string assistantId;
    [Tooltip("The version date with which you would like to use the service in the form YYYY-MM-DD. Current is 2018-07-10")]
    [SerializeField]
    private string assistantVersionDate;

    [Tooltip("The IAM apikey.")]
    [SerializeField]
    private string assistantIamApikey;
    [Tooltip("The IAM url used to authenticate the apikey (optional). This defaults to \"https://iam.bluemix.net/identity/token\".")]
    [SerializeField]
    private string assistantIamUrl;

    #endregion


    private int _recordingRoutine = 0;
    private string _microphoneID = null;
    private AudioClip _recording = null;
    private int _recordingBufferSize = 1;
    private int _recordingHZ = 22050;

    private bool createSessionTested = false;
    private bool deleteSessionTested;
    private string sessionId;

    private SpeechToTextService _speechToText;
    private AssistantService _assistant;

    private bool messageTested0;

    private IEnumerator createServices(){

        Credentials stt_credentials = null;
        //  Create credential and instantiate service
        if (!string.IsNullOrEmpty(speechToTextIamApikey))
        {
            //  Authenticate using iamApikey
            TokenOptions tokenOptions = new TokenOptions()
            {
                IamApiKey = speechToTextIamApikey,
                IamUrl = speechToTextIamUrl
            };

            stt_credentials = new Credentials(tokenOptions, speechToTextServiceUrl);

            while (!stt_credentials.HasIamTokenData())
                yield return null;
        }
        else
        {
            throw new IBMException("Please provide IAM ApiKey for the Speech To Text service.");
        }

        Credentials asst_credentials = null;
        //  Create credential and instantiate service

        if (!string.IsNullOrEmpty(assistantIamApikey))
        {
            //  Authenticate using iamApikey
            TokenOptions tokenOptions = new TokenOptions()
            {
                IamApiKey = assistantIamApikey
            };

            asst_credentials = new Credentials(tokenOptions, assistantServiceUrl);

            while (!asst_credentials.HasIamTokenData())
                yield return null;
        }
        else
        {
            throw new IBMException("Please provide IAM ApiKey for the Watson Assistant service.");
        }


        _speechToText = new SpeechToTextService(stt_credentials);
        _assistant = new AssistantService(assistantVersionDate, asst_credentials);

        _assistant.VersionDate = assistantVersionDate;
        Active = true;

        _assistant.CreateSession(OnCreateSession, assistantId);

        while (!createSessionTested)
        {
            yield return null;
        }

        StartRecording();
    }

    void Start()
    {
        LogSystem.InstallDefaultReactors();


        //  Create credential and instantiate service
        Runnable.Run(createServices());


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

        Log.Debug("SpeechSandboxStreaming.OnError()", "Error! {0}", error);
    }

    /*
    private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
    {
        Log.Error("SpeechSandboxStreaming.OnFail()", "Error received: {0}", error.ToString());
    }
    */

    private IEnumerator RecordingHandler()
    {
        Log.Debug("SpeechSandboxStreaming.RecordingHandler()", "devices: {0}", Microphone.devices);
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
                Log.Error("SpeechSandboxStreaming.RecordingHandler()", "Microphone disconnected.");

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

    private void OnRecognize(SpeechRecognitionEvent result)
    {
        if (result != null && result.results.Length > 0)
        {
            foreach (var res in result.results)
            {
                foreach (var alt in res.alternatives)
                {
                    if (res.final && alt.confidence > 0)
                    {
                        string transcript_text = alt.transcript;
                        Debug.Log("Result: " + transcript_text + " Confidence: " + alt.confidence);

                        var input = new MessageInput()
                        {
                            Text = transcript_text,
                            Options = new MessageInputOptions()
                            {
                                ReturnContext = false
                            }
                        };
                        Debug.Log("Input to Assistant:" + input.Text);

                        _assistant.Message(OnMessage,  assistantId, sessionId, input);
                    }
                }

                if (res.keywords_result != null && res.keywords_result.keyword != null)
                {
                    foreach (var keyword in res.keywords_result.keyword)
                    {
                        Log.Debug("SpeechSandboxStreaming.OnRecognize()", "keyword: {0}, confidence: {1}, start time: {2}, end time: {3}", keyword.normalized_text, keyword.confidence, keyword.start_time, keyword.end_time);
                    }
                }

                if (res.word_alternatives != null)
                {
                    foreach (var wordAlternative in res.word_alternatives)
                    {
                        Log.Debug("SpeechSandboxStreaming.OnRecognize()", "Word alternatives found. Start time: {0} | EndTime: {1}", wordAlternative.start_time, wordAlternative.end_time);
                        foreach(var alternative in wordAlternative.alternatives)
                            Log.Debug("SpeechSandboxStreaming.OnRecognize()", "\t word: {0} | confidence: {1}", alternative.word, alternative.confidence);
                    }
                }
            }
        }
    }

    void OnMessage(DetailedResponse<MessageResponse> resp, IBMError error)
    {
        if (resp != null && resp.Result.Output.Intents.Count  != 0 )
        {
            string intent = resp.Result.Output.Intents[0].Intent;
            Debug.Log("Intent: " + intent);
            string currentMat = null;
            string currentScale = null;
            string direction = null;
            if (intent == "move")
            {
                foreach (RuntimeEntity entity in resp.Result.Output.Entities)
                {
                    Debug.Log("entityType: " + entity.Entity + " , value: " + entity.Value);
                    direction = entity.Value;
                    gameManager.MoveObject(direction);
                }
            }

            if (intent == "create")
            {
                bool createdObject = false;
                foreach (RuntimeEntity entity in resp.Result.Output.Entities)
                {
                    Debug.Log("entityType: " + entity.Entity + " , value: " + entity.Value);
                    if (entity.Entity == "material")
                    {
                        currentMat = entity.Value;
                    }
                    if (entity.Entity == "scale")
                    {
                        currentScale = entity.Value;
                    }
                    else if (entity.Entity == "object")
                    {
                        gameManager.CreateObject(entity.Value, currentMat, currentScale);
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

    private void OnDeleteSession(DetailedResponse<object> response, IBMError error)
    {
        Log.Debug("ExampleAssistantV2.OnDeleteSession()", "Session deleted.");
        deleteSessionTested = true;
    }

    private void OnCreateSession(DetailedResponse<SessionResponse> response, IBMError error)
    {
        Log.Debug("ExampleAssistantV2.OnCreateSession()", "Session: {0}", response.Result.SessionId);
        sessionId = response.Result.SessionId;
        createSessionTested = true;
    }

    private void OnRecognizeSpeaker(SpeakerRecognitionEvent result)
    {
        if (result != null)
        {
            foreach (SpeakerLabelsResult labelResult in result.speaker_labels)
            {
                Log.Debug("SpeechSandboxStreaming.OnRecognize()", string.Format("speaker result: {0} | confidence: {3} | from: {1} | to: {2}", labelResult.speaker, labelResult.from, labelResult.to, labelResult.confidence));
            }
        }
    }
}
