using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Kiln
{
    public class IDButton : MonoBehaviour
    {
        [SerializeField] private Text _text;
        
        private string _id = "";
        public string ID
        {
            get { return _id; }
            set
            {
                _id = value;
                _text.text = value;
            }
        }

        private TaskCompletionSource<string> _tcs;
        public TaskCompletionSource<string> TCS
        {
            set { _tcs = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnPress()
        {
            _tcs.SetResult(_id);
        }
    }
   
}