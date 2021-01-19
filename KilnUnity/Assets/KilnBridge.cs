using System.Threading.Tasks;
using UnityEngine; 

public class KilnException : System.Exception {
    private static readonly string DefaultMessage = "Java Exception.";

    public KilnException() : base(DefaultMessage) { }
    public KilnException(string message) : base(message) { }
    public KilnException(string message, System.Exception innerException)
    : base(message, innerException) { }    
}

public enum KilnPlatform {
    Development
}

public class KilnBridge {
	private AndroidJavaObject kiln;

    class KilnCallback<T>: AndroidJavaProxy {

        private TaskCompletionSource<T> taskCompletionSource;

        public TaskCompletionSource<T> tcs 
        {
            get {
                return taskCompletionSource;
            }
            set {
                taskCompletionSource = value;
            }
        }

        public KilnCallback(): base("io.gamebake.kiln.Callback") {}

        public void onSuccess(T result) 
        {
            Debug.Log("KilnCallback onSuccess");
            taskCompletionSource.SetResult(result);
        }

        public void onFailure(AndroidJavaObject exception) 
        {
            Debug.Log("KilnCallback onFailure");
            taskCompletionSource.SetException(new KilnException(exception.ToString()));
        }

    }

    public Task init() 
    {
        // Debug.Log("Inside INIT!");
        string AD_UNIT_ID = "ad_unit_id";

        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        // getBaseContext or getApplicationContext
        // AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
        AndroidJavaObject context = activity;

        AndroidJavaObject configBuilder = new AndroidJavaObject("io.gamebake.kiln.KilnConfiguration$Builder", context, AD_UNIT_ID);

        AndroidJavaObject config = configBuilder.Call<AndroidJavaObject>("build");

        kiln = new AndroidJavaObject("io.gamebake.kiln.Kiln", config);

        var aTcs = new TaskCompletionSource<object>();

        kiln.Call("init", new KilnCallback<object>(){
            tcs = aTcs
        });

        // Debug.Log("QUITING INIT!");

        return aTcs.Task;
    }

    public KilnPlatform platformAvailable() 
    {
        AndroidJavaClass kClass = new AndroidJavaClass("io.gamebake.kiln.Kiln");
        AndroidJavaObject platformEnum = kClass.CallStatic<AndroidJavaObject>("platformAvailable");
        return (KilnPlatform)platformEnum.Call<int>("ordinal");
    }

    public bool supportsInterstitialAds() 
    {
        return kiln.Call<bool>("supportsInterstitialAds");
    }

    public Task loadInterstitialAd(string identifier)  
    {
        var aTcs = new TaskCompletionSource<object>();
        kiln.Call("loadInterstitialAd", identifier, new KilnCallback<object>(){
            tcs = aTcs
        });
        return aTcs.Task;
    }

    public Task showInterstitialAd(string identifier) 
    {
        var aTcs = new TaskCompletionSource<object>();
        kiln.Call("showInterstitialAd", identifier, new KilnCallback<object>(){
            tcs = aTcs
        });
        return aTcs.Task;
    }
    // private AndroidJavaObject createConfig() {
        // AndroidJavaObject config = new AndroidJavaC("com.gamebake.kiln.KilnConfiguration");
    // }
}
