using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class VN : MonoBehaviour
{
    //Dial display variables
    private string _string2Display;
    [SerializeField] private TMP_Text _dialBox;
    [SerializeField] private float _dialSpeed;
    private bool _dialDisplayed, _dialStarted, _dialOpen, _sentenceDisplayed;

    //Character Name TMPtexts
    [SerializeField] private TMP_Text _leftCharName, _rightCharName;

    //UI Animators + sprite array with all char sprites
    [SerializeField] private Animator _leftHeaderAnim, _rightHeaderAnim, _leftSpriteAnim, _rightSpriteAnim, _dialBoxAnim;
    [SerializeField] private Sprite[] _charSpriteArray = new Sprite[10];
    [SerializeField] private Image _leftCharSprite, _rightCharSprite;

    //variables Queues pour empiler puis dépiler les dialogues à afficher pour cette session de chat
    private Queue<string> _dial2Display = new Queue<string>();
    private Queue<string> _response2Display = new Queue<string>();
    private Queue<int> _leftCharSprite2Display = new Queue<int>();
    private Queue<int> _rightCharSprite2Display = new Queue<int>();
    private Queue<string> _CharName2Display = new Queue<string>();
    private Queue<int> _activeSpeaker2Display = new Queue<int>();
    private string _previousLeftName, _previousRightName;
    private int _previousLeftSprite, _previousRightSprite;

    private bool _loaded;
    [SerializeField] private GameObject _player;

    private void Awake()
    {
        _dialDisplayed = _dialStarted = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_dial2Display.Count > 0 && !_dialStarted) DialManage();
        else if (_sentenceDisplayed)
        {
            if (_dial2Display.Count>0 && _player.GetComponent<PlayerMove>()._actionInput.action.ReadValue<float>() > 0.1f)
            {
                DialManage();
            }
            else if(_dial2Display.Count<=0 && _player.GetComponent<PlayerMove>()._actionInput.action.ReadValue<float>() > 0.1f && _dialOpen)
            {
                EndDial();
            }
        }
    }

    //si on valide et qu'il reste du texte, on recall un dialmanage
    //si on valide et qu'il n'y a plus de texte, on ferme la boutique

    public void DialManage()
    {
        if (_sentenceDisplayed) _sentenceDisplayed = false;
        if (!_dialStarted) _dialStarted = true;
        _string2Display = _dial2Display.Dequeue();
        _dialBox.text = "";
        //update des sprites
        CharSpriteDisplay();
        //update des nom des speakers
        UpdateCharName();
        //on dépile le numéro du speaker pour activer son HUD
        ActiveCharName(_activeSpeaker2Display.Dequeue());
        //si l'UI de VN est fermée on l'active
        if (!_dialOpen) DisplayDial();
        //on envoie le texte en typing
        StartCoroutine(DialTyping());
    }

    //une méthode load Queue (string et autres paramètres) à partir du script de JSON
    //une méthode load next dialogue (dans _string2Display) depuis la Queue de Dialogue
    // paramètre de dialogue - int (id du dialogue) - string array (dialogues) - int array (active speaker per dialogue) - int array (sprite de droite) - int array (sprite de gauche)

    private void CharSpriteDisplay()
    {
        if (_previousLeftSprite != _leftCharSprite2Display.Peek())
        {
            _previousLeftSprite = _leftCharSprite2Display.Dequeue();
            _leftCharSprite.sprite = _charSpriteArray[_previousLeftSprite];
        }
        else _leftCharSprite2Display.Dequeue();
        if (_previousRightSprite != _rightCharSprite2Display.Peek())
        {
            _previousRightSprite = _rightCharSprite2Display.Dequeue();
            _rightCharSprite.sprite= _charSpriteArray[_previousRightSprite];
        }
        else _rightCharSprite2Display.Dequeue();
    }

    //update les noms de personnages à afficher
    private void UpdateCharName()
    {
        //rajouter un int pour enregistrer les noms du dépilage précédent, les comparer pour voir si ça vaut le coup d'update
        //et potentiellement faire des animations pour le switch de perso
        if (_previousLeftName != _CharName2Display.Peek()) _previousLeftName = _leftCharName.text = _CharName2Display.Dequeue();
        else _CharName2Display.Dequeue();
        if (_previousRightName != _CharName2Display.Peek()) _previousRightName = _rightCharName.text = _CharName2Display.Dequeue();
        else _CharName2Display.Dequeue();
    }

    //affiche le hud du nom du personnage qui parle ou pas
    private void ActiveCharName(int a)
    {
        if (a==1)
        {
            _leftHeaderAnim.SetBool("isOpen", true);
            _rightHeaderAnim.SetBool("isOpen", false);
        }
        else if (a==2)
        {
            _leftHeaderAnim.SetBool("isOpen", false);
            _rightHeaderAnim.SetBool("isOpen", true);
        }
    }

    //initie le dialogue en affichant les personnages et la dialbox
    private void DisplayDial()
    {
        _dialBoxAnim.SetBool("isOpen", true);
        _leftSpriteAnim.SetBool("isOpen", true);
        _rightSpriteAnim.SetBool("isOpen", true);
        _dialOpen = true;
    }

    IEnumerator DialTyping()
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < _string2Display.Length;i++)
        {
            _dialBox.text = _dialBox.text + _string2Display[i];
            yield return new WaitForSeconds(_dialSpeed);
        }
        _sentenceDisplayed= true;
        if(_dial2Display.Count<=0) _dialDisplayed = true;
    }

    private void EndDial()
    {
        _leftHeaderAnim.SetBool("isOpen", false);
        _rightHeaderAnim.SetBool("isOpen", false);
        _dialBoxAnim.SetBool("isOpen", false);
        _leftSpriteAnim.SetBool("isOpen", false);
        _rightSpriteAnim.SetBool("isOpen", false);
        _dialOpen = false;
        _dialStarted= false;
    }

    //pour mettre en Queue les éléments du dialogue - dial, replies, sprites et active speaker
    public void LoadQueue(int a)
    {
        for (int i = 0; i< gameObject.GetComponent<DialogueJSONReader>()._dialogList.dialog[a]._dialArray.Length;i++)
        {
            _dial2Display.Enqueue(gameObject.GetComponent<DialogueJSONReader>()._dialogList.dialog[a]._dialArray[i]);
            Debug.Log(gameObject.GetComponent<DialogueJSONReader>()._dialogList.dialog[a]._dialArray[i]);

        }
        for (int j = 0; j< gameObject.GetComponent<DialogueJSONReader>()._dialogList.dialog[a]._leftCharSprite2Display.Length; j++)
        {
            _leftCharSprite2Display.Enqueue(gameObject.GetComponent<DialogueJSONReader>()._dialogList.dialog[a]._leftCharSprite2Display[j]);
        }
        for (int h = 0; h< gameObject.GetComponent<DialogueJSONReader>()._dialogList.dialog[a]._rightCharSprite2Display.Length; h++)
        {
            _rightCharSprite2Display.Enqueue(gameObject.GetComponent<DialogueJSONReader>()._dialogList.dialog[a]._rightCharSprite2Display[h]);
        }
        for (int g = 0; g< gameObject.GetComponent<DialogueJSONReader>()._dialogList.dialog[a]._activeSpeaker.Length;g++)
        {
            _activeSpeaker2Display.Enqueue(gameObject.GetComponent<DialogueJSONReader>()._dialogList.dialog[a]._activeSpeaker[g]);
        }
        for (int k=0; k< gameObject.GetComponent<DialogueJSONReader>()._dialogList.dialog[a]._charName.Length; k++)
        {
            _CharName2Display.Enqueue(gameObject.GetComponent<DialogueJSONReader>()._dialogList.dialog[a]._charName[k]);
        }
    }
}
