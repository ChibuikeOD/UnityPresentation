using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;


public class Speech : MonoBehaviour
{
    public KeywordRecognizer keywordRec;
    public Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>(); //mapping the keywords, to the actions to be carried out when they are said
    public System.Action helloAction;
    public System.Action goodbyeAction;
    public System.Action powerAction;
    public System.Action clavicleAction;

    void Start()
    {
        helloAction = helloMethod; //mapping the action to the method
        goodbyeAction = goodByeMethod;
        powerAction = powerMethod;
        clavicleAction = clavicleMethod;

        keywords.Add("hello", helloAction);
        keywords.Add("goodbye", goodbyeAction);
        keywords.Add("power", powerAction);
        keywords.Add("clavicle", clavicleAction);

        keywordRec = new KeywordRecognizer(keywords.Keys.ToArray()); //initialize speech rec

        keywordRec.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized; //Register

        // Start the recognizer
        keywordRec.Start();

        void helloMethod()
        {
            Debug.Log("Hello");
        }

        void goodByeMethod()
        {

            Debug.Log("Goodbye");
        }

        void powerMethod()
        {

            Debug.Log("Power");
        }

        void clavicleMethod()
        {

            Debug.Log("Clavicle");
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
