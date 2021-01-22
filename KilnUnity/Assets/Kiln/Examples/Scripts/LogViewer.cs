using System;
using UnityEngine;
using UnityEngine.UI;

namespace Kiln
{
    [RequireComponent(typeof(ScrollRect))]
    public class LogViewer : MonoBehaviour
    {
        [SerializeField] private Text _text;
        [SerializeField] private ScrollRect _scrollRect;

        // private bool _stickToBottom = false;

        private void Awake()
        {
            _scrollRect = GetComponent<ScrollRect>();
            
            if (_scrollRect == null)
            {
                throw new NullReferenceException("LogViewer ScrollRect element not assigned");
            }

            _text = _text ?? _scrollRect.content.GetComponent<Text>();
            
            // We have it disabled by default to prevent a Unity bug where the content size fitter component
            // generates changes on the scene upon loading it.
            _text.gameObject.SetActive(true);

            if (_text == null)
            {
                throw new NullReferenceException("LogViewer Text element not assigned");
            }

            _text.text = "";
        }

        private void Reset()
        {
            _scrollRect = GetComponent<ScrollRect>();
        }

        private void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="stackTrace"></param>
        /// <param name="type"></param>
        private void HandleLog(string condition, string stackTrace, LogType type)
        {
            // if (_scrollRect.verticalNormalizedPosition <= 0)
            // {
            //     _stickToBottom = true;
            // }
            // else
            // {
            //     _stickToBottom = false;
            // }

            string color;

            switch (type)
            {
                case LogType.Error:
                case LogType.Exception:
                    color = "red";
                    break;

                case LogType.Warning:
                    color = "yellow";
                    break;

                default:
                    color = "white";
                    break;

            }

            _text.text += $"<color={color}>{condition}</color>\n";

            // if (_stickToBottom)
            // {
            //     _scrollRect.verticalNormalizedPosition = 0;
            // }
        }
    }

}