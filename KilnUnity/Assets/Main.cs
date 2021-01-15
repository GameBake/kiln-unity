using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Main : MonoBehaviour
{
    private KilnBridge kiln;

    // Start is called before the first frame update
    void Start()
    {
        kiln = new KilnBridge();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public async void InitButtonPressed() 
    {
        Debug.Log("Init button pressed!");
        try
        {
            await kiln.init();   
            Debug.Log("Initialized"); 
            Debug.Log("Platform is " + kiln.platformAvailable());
            await kiln.loadInterstitialAd("lalalala");
            await kiln.showInterstitialAd("lalalala");
        }
        catch (KilnException ex) 
        {
            Debug.Log("Kiln Exception: " + ex);
        }
        catch (System.Exception ex)
        {
            Debug.Log("System Exception: " + ex);
        }
        return;
    }
}
