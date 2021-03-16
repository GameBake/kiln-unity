#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;

namespace Kiln
{
    public class BannerAdController : MonoBehaviour
    {
        [SerializeField] private Image _bannerImage;
        [SerializeField] private Text _label;

        private BannerPosition _position;
        public BannerPosition Position { get { return _position;  } }
        private BannerSize _maxSize;
        public BannerSize MaxSize { get { return _maxSize;  } }

        public void Awake()
        {
            HideBanner();

            DontDestroyOnLoad(this.gameObject);
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="maxSize"></param>
        public void Configure(string placementId, BannerPosition position, BannerSize maxSize)
        {
            // Get DP (Density Independent) Sizes converted to current screen dpi
            var width = (maxSize.Width() * Screen.dpi) / 160f;
            var height = (maxSize.Height() * Screen.dpi) / 160f;

            Vector2 anchorMin = Vector2.zero;
            Vector2 anchorMax = Vector2.zero;
            Vector2 pivot = Vector2.zero;

            switch (position)
            {
                case BannerPosition.TopLeft:
                    anchorMin = anchorMax = pivot = new Vector2(0, 1);
                    break;

                case BannerPosition.TopCenter:
                    anchorMin = anchorMax = pivot = new Vector2(0.5f, 1);
                    break;

                case BannerPosition.TopRight:
                    anchorMin = anchorMax = pivot = new Vector2(1, 1);
                    break;

                case BannerPosition.Centered:
                    anchorMin = anchorMax = pivot = new Vector2(0.5f, 0.5f);
                    break;

                case BannerPosition.BottomLeft:
                    anchorMin = anchorMax = pivot =new Vector2(0, 0);
                    break;
                    
                case BannerPosition.BottomCenter:
                    anchorMin = anchorMax = pivot = new Vector2(0.5f, 0);
                    break;

                case BannerPosition.BottomRight:
                    anchorMin = anchorMax = pivot = new Vector2(1, 0);
                    break;
            }

            _bannerImage.rectTransform.sizeDelta = new Vector2(width, height);
            _bannerImage.rectTransform.anchorMin = anchorMin;
            _bannerImage.rectTransform.anchorMax = anchorMax;
            _bannerImage.rectTransform.pivot = pivot;

            _label.text = $"Banner: {placementId}";
        }
        
        public void ShowBanner()
        {
            _bannerImage.gameObject.SetActive(true);
        }

        public void HideBanner()
        {
            _bannerImage.gameObject.SetActive(false);
        }

        public void DestroyBanner()
        {
            Destroy(gameObject);
        }
    }

}
#endif