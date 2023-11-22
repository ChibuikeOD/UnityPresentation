using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using TMPro;
using HuggingFace.API;


namespace Speech
{
    [AddComponentMenu("AhmedSchrute/Recorder")]
    [RequireComponent(typeof(AudioSource))]

    public class Speech : MonoBehaviour
    {
        public KeywordRecognizer keywordRec;
        public Dictionary<string, Action> keywords = new Dictionary<string, System.Action>(); //mapping the keywords, to the actions to be carried out when they are said
        public Action downAction;
        public Action leftAction;
        public Action rightAction;
        public Action upAction;
        public Action chatAction;
        AudioSource audioSource;
        float[] samplesData;
        const int HEADER_SIZE = 44;
        [Tooltip("Set a keyboard key for saving the Audio File")]
        public KeyCode keyCode;
        public Button SaveButton;
        [Tooltip("What should the saved file name be, the file will be saved in Streaming Assets Directory, Don't add .wav at the end")]
        public string fileName;
        public TextMeshProUGUI result;


        void Start()
        {
            GameObject myobject = gameObject;

            downAction = downMethod; //mapping the action to the method
            leftAction = leftMethod;
            rightAction = rightMethod;
            upAction = upMethod;
            chatAction = chatMethod;

            keywords.Add("left", leftAction);
            keywords.Add("right", rightAction);
            keywords.Add("Raise", upAction);
            keywords.Add("down", downAction);
            keywords.Add("chat", chatAction);

            keywordRec = new KeywordRecognizer(keywords.Keys.ToArray()); //initialize speech rec

            keywordRec.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized; //Register

            // Start the recognizer
            keywordRec.Start();

            void leftMethod()
            {
                Debug.Log("Left Method");
                Vector3 currentposition = myobject.transform.eulerAngles;
                currentposition.y += 90f;
                myobject.transform.rotation = Quaternion.Euler(currentposition);
            }

            void rightMethod()
            {
                Debug.Log("Right");
                Vector3 currentposition = myobject.transform.eulerAngles;
                currentposition.y -= 90f;
                myobject.transform.rotation = Quaternion.Euler(currentposition);
            }

            void upMethod()
            {
                Debug.Log("up");
                Vector3 currentposition = myobject.transform.position;
                currentposition.y += 0.2f;
                myobject.transform.position = currentposition;
            }

            void downMethod()
            {
                Debug.Log("down");
                Vector3 currentposition = myobject.transform.position;
                currentposition.y -= 0.2f;
                myobject.transform.position = currentposition;
            }

            void chatMethod()
            {
                Debug.Log("chat");
                StartRecording();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(keyCode))
            {
                Save();
            }
        }

        private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
        {
            System.Action keywordAction;
            if (keywords.TryGetValue(args.text, out keywordAction))
            {
                // Invoke the associated action
                keywordAction.Invoke();
            }
        }

        private void StartRecording()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = Microphone.Start(Microphone.devices[0], true, 10, 44100);
            Debug.Log("Started Recording");
        }

        public void Save(string fileName = "test")
        {
            Debug.Log("Saving...");
            while (!(Microphone.GetPosition(null) > 0)) { }
            samplesData = new float[audioSource.clip.samples * audioSource.clip.channels];
            audioSource.clip.GetData(samplesData, 0);
            string filePath = Path.Combine(Application.streamingAssetsPath, fileName + ".wav");
            // Delete the file if it exists.
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            try
            {
                WriteWAVFile(audioSource.clip, filePath);
                Debug.Log("File Saved Successfully at StreamingAssets/" + fileName + ".wav");
            }
            catch (DirectoryNotFoundException)
            {
                Debug.LogError("Please, Create a StreamingAssets Directory in the Assets Folder");
            }
            SendRecording();
        }

        private void SendRecording()
        {
            result.color = Color.yellow;
            result.text = "Sending...";

            // Path to the WAV file
            string filePath = Path.Combine(Application.streamingAssetsPath, "test.wav");

            // Read the contents of the WAV file
            byte[] wavBytes = File.ReadAllBytes(filePath);

            HuggingFaceAPI.AutomaticSpeechRecognition(wavBytes, response =>
            {
                result.color = Color.white;
                result.text = response;
            }, error =>
            {
                result.color = Color.red;
                result.text = error;
            });
        }

        public byte[] ConvertWAVtoByteArray(string filePath)
        {
            //Open the stream and read it back.
            byte[] bytes = new byte[audioSource.clip.samples + HEADER_SIZE];
            using (FileStream fs = File.OpenRead(filePath))
            {
                fs.Read(bytes, 0, bytes.Length);
            }
            return bytes;
        }

        // WAV file format from http://soundfile.sapp.org/doc/WaveFormat/
        void WriteWAVFile(AudioClip clip, string filePath)
        {
            float[] clipData = new float[clip.samples];

            //Create the file.
            using (Stream fs = File.Create(filePath))
            {
                int frequency = clip.frequency;
                int numOfChannels = clip.channels;
                int samples = clip.samples;
                fs.Seek(0, SeekOrigin.Begin);

                //Header

                // Chunk ID
                byte[] riff = Encoding.ASCII.GetBytes("RIFF");
                fs.Write(riff, 0, 4);

                // ChunkSize
                byte[] chunkSize = BitConverter.GetBytes((HEADER_SIZE + clipData.Length) - 8);
                fs.Write(chunkSize, 0, 4);

                // Format
                byte[] wave = Encoding.ASCII.GetBytes("WAVE");
                fs.Write(wave, 0, 4);

                // Subchunk1ID
                byte[] fmt = Encoding.ASCII.GetBytes("fmt ");
                fs.Write(fmt, 0, 4);

                // Subchunk1Size
                byte[] subChunk1 = BitConverter.GetBytes(16);
                fs.Write(subChunk1, 0, 4);

                // AudioFormat
                byte[] audioFormat = BitConverter.GetBytes(1);
                fs.Write(audioFormat, 0, 2);

                // NumChannels
                byte[] numChannels = BitConverter.GetBytes(numOfChannels);
                fs.Write(numChannels, 0, 2);

                // SampleRate
                byte[] sampleRate = BitConverter.GetBytes(frequency);
                fs.Write(sampleRate, 0, 4);

                // ByteRate
                byte[] byteRate = BitConverter.GetBytes(frequency * numOfChannels * 2); // sampleRate * bytesPerSample*number of channels, here 44100*2*2
                fs.Write(byteRate, 0, 4);

                // BlockAlign
                ushort blockAlign = (ushort)(numOfChannels * 2);
                fs.Write(BitConverter.GetBytes(blockAlign), 0, 2);

                // BitsPerSample
                ushort bps = 16;
                byte[] bitsPerSample = BitConverter.GetBytes(bps);
                fs.Write(bitsPerSample, 0, 2);

                // Subchunk2ID
                byte[] datastring = Encoding.ASCII.GetBytes("data");
                fs.Write(datastring, 0, 4);

                // Subchunk2Size
                byte[] subChunk2 = BitConverter.GetBytes(samples * numOfChannels * 2);
                fs.Write(subChunk2, 0, 4);

                // Data

                clip.GetData(clipData, 0);
                short[] intData = new short[clipData.Length];
                byte[] bytesData = new byte[clipData.Length * 2];

                int convertionFactor = 32767;

                for (int i = 0; i < clipData.Length; i++)
                {
                    intData[i] = (short)(clipData[i] * convertionFactor);
                    byte[] byteArr = new byte[2];
                    byteArr = BitConverter.GetBytes(intData[i]);
                    byteArr.CopyTo(bytesData, i * 2);
                }

                fs.Write(bytesData, 0, bytesData.Length);
            }
        }
    }
}