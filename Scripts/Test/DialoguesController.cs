using System;
using System.Collections;
using System.Linq;
using TelmanDialogues.Dialogues;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TelmanDialogues.Controller
{
    public class DialoguesController : MonoBehaviour
    {
        public static DialoguesController Instance { get; private set; }

        [Header("System")]
        [SerializeField] private DialoguesSystem _dialoguesSystem;

        [Header("UI")]
        [SerializeField] private TMP_Text _lineText;
        [SerializeField] private TMP_Text _charName;
        [SerializeField] private RectTransform _buttonsContainer;
        [SerializeField] private Button _buttonPrefab;

        [Header("Settings")]
        [SerializeField] private float _typingSpeed = 0.03f;

        private DialoguesBlock _currentBlock;
        private int _currentLineIndex;

        private Coroutine _typingCoroutine;
        private bool _isTyping;

        public static event Action<string> OnDialogueEvent;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current != null &&
                    EventSystem.current.IsPointerOverGameObject())
                    return;

                Next();
            }

            if (Input.GetKeyDown(KeyCode.J))
                Play("StartPoint");

            if (Input.GetKeyDown(KeyCode.L))
                Play("Glad");
        }

        // =====================================================
        // START BY NAME
        // =====================================================
        public void Play(string blockName)
        {
            _currentBlock = _dialoguesSystem.DialoguesPure
                .FirstOrDefault(x => x.BlockName == blockName);

            if (_currentBlock == null)
            {
                Debug.LogError($"Block '{blockName}' not found");
                return;
            }

            StartBlock();
        }

        // =====================================================
        // START BY GUID
        // =====================================================
        private void PlayByGuid(string guid)
        {
            _currentBlock = _dialoguesSystem.DialoguesPure
                .FirstOrDefault(x => x.GUID == guid);

            if (_currentBlock == null)
            {
                Debug.LogError($"Block GUID '{guid}' not found");
                return;
            }

            StartBlock();
        }

        // =====================================================
        // START BLOCK
        // =====================================================
        private void StartBlock()
        {
            _currentLineIndex = 0;
            ClearButtons();
            ShowCurrentLine();
        }

        // =====================================================
        // NEXT LINE
        // =====================================================
        private void Next()
        {
            if (_currentBlock == null) return;

            // skip typing
            if (_isTyping)
            {
                StopCoroutine(_typingCoroutine);

                _lineText.text =
                    _currentBlock.Lines[_currentLineIndex].Line;

                _isTyping = false;
                return;
            }

            _currentLineIndex++;

            if (_currentLineIndex >= _currentBlock.Lines.Count)
            {
                ShowChoices();
                return;
            }

            ShowCurrentLine();
        }

        // =====================================================
        // SHOW LINE
        // =====================================================
        private void ShowCurrentLine()
        {
            var lineData = _currentBlock.Lines[_currentLineIndex];

            _charName.text = lineData.Character;

            // events
            if (lineData.Events != null)
            {
                foreach (var evt in lineData.Events)
                {
                    OnDialogueEvent?.Invoke(evt);
                }
            }

            if (_typingCoroutine != null)
                StopCoroutine(_typingCoroutine);

            _typingCoroutine = StartCoroutine(TypeText(lineData.Line));
        }

        // =====================================================
        // TYPEWRITER + AUTO NEXT
        // =====================================================
        private IEnumerator TypeText(string text)
        {
            _isTyping = true;
            _lineText.text = "";

            foreach (char c in text)
            {
                _lineText.text += c;
                yield return new WaitForSeconds(_typingSpeed);
            }

            _isTyping = false;

            yield return new WaitForSeconds(0.4f);

            if (_currentBlock != null)
            {
                Next();
            }
        }

        // =====================================================
        // CHOICES
        // =====================================================
        private void ShowChoices()
        {
            ClearButtons();

            if (_currentBlock.Choices == null ||
                _currentBlock.Choices.Count == 0)
            {
                Debug.Log("Dialogue ended");
                return;
            }

            foreach (var choice in _currentBlock.Choices)
            {
                var captured = choice;

                Button btn = Instantiate(_buttonPrefab, _buttonsContainer);
                btn.GetComponentInChildren<TMP_Text>().text = captured.Text;

                btn.onClick.AddListener(() =>
                {
                    PlayByGuid(captured.NextBlockGUID);
                });
            }
        }

        // =====================================================
        // CLEAR UI
        // =====================================================
        private void ClearButtons()
        {
            foreach (Transform child in _buttonsContainer)
            {
                Destroy(child.gameObject);
            }
        }
    }
}