using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;


public class Speech : MonoBehaviour
{
    public KeywordRecognizer keywordRec;
    public Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>(); //mapping the keywords, to the actions to be carried out when they are said
    public System.Action downAction;
    public System.Action leftAction;
    public System.Action rightAction;
    public System.Action upAction;
    

    void Start()
    {
        GameObject myobject = gameObject;
        downAction = downMethod; //mapping the action to the method
       leftAction = leftMethod;
        rightAction = rightMethod;
        upAction = upMethod;

        keywords.Add("left", leftAction);
        keywords.Add("right", rightAction);
        keywords.Add("Raise", upAction);
        keywords.Add("down", downAction);

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
