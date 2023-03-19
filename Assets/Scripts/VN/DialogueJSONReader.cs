using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class DialogueJSONReader : MonoBehaviour
{
    [SerializeField] private TextAsset _dialogTxt;

    [System.Serializable]
    public class Dialog
    {
        public int _dialID;
        public int[] _leftCharSprite2Display;
        public int[] _rightCharSprite2Display;
        public int[] _activeSpeaker;
        public string[] _charName;
        public string[] _dialArray;
        public string[] _responseArray;
    }

    [System.Serializable]
    public class DialogList
    {
        public Dialog[] dialog;
    }

    public DialogList _dialogList = new DialogList();


    // Start is called before the first frame update
    void Start()
    {
        _dialogList = JsonUtility.FromJson<DialogList>(_dialogTxt.text);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
