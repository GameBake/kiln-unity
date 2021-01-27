using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Kiln
{
    public class IDSelector : MonoBehaviour
    {
        [SerializeField] private Transform _contentParent;
        [SerializeField] private IDButton _idButtonPrefab;
        [SerializeField] private IDButton _idInputPrefab;

        private TaskCompletionSource<string> _tcs;

        /// <summary>
        /// 
        /// </summary>
        private void Clean()
        {
            for (int i = 0; i < _contentParent.childCount; i++)
            {
                Destroy(_contentParent.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ids"></param>
        private void Initialize(List<string> ids)
        {
            Clean();

            // Create the generic input field
            IDButton inputID = Instantiate(_idInputPrefab);
            inputID.TCS = _tcs;
            inputID.transform.SetParent(_contentParent, false);

            foreach (string id in ids)
            {
                IDButton button = Instantiate(_idButtonPrefab);
                button.ID = id;
                button.TCS = _tcs;

                button.transform.SetParent(_contentParent, false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Task<string> SelectID(List<string> ids)
        {
            _tcs = new TaskCompletionSource<string>();

            Initialize(ids);

            gameObject.SetActive(true);

            return _tcs.Task;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnCancelButton()
        {
            _tcs.SetCanceled();
            Close();
        }

    }

}