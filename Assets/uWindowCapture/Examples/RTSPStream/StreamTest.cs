using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uWindowCapture;
using TMPro;

[RequireComponent(typeof(UwcWindowTexture))]
public sealed class StreamTest : MonoBehaviour
{
    public enum StreamingQuality
    {
        Original = 0,
        P720,
        P480,
        P360,
        P240
    };

    [SerializeField]
    private bool isStreaming;
    [SerializeField]
    private string rtspURL="rtsp://localhost:8554/test";        //rtsp://195.201.128.31:8554/test
    [SerializeField]
    private int targetFps = 24;

    [SerializeField]
    private int width=1280;

    [SerializeField]
    private int height=720;
    [SerializeField]
    private bool trueWindowSize = false;

    public TMP_Dropdown resolutionDropdown;

    private UwcWindowTexture WindowTexture;

    //[SerializeField]
    //private bool enableEncryption;
    [SerializeField]
    private int win_id;

    IEnumerator Start()
    {
        WindowTexture = GetComponent < UwcWindowTexture >();
        resolutionDropdown.onValueChanged.AddListener( ChangeResolution );
        ChangeResolution( 0 );
        //Register Unity function for rtsp interrupt callback
        Lib.RTSPStreamInterrupted(RtspInterruptCallback);
        yield return new WaitForSeconds(2f);
        win_id = GetComponent<UwcWindowTexture>().window.id; 
    }

    void RtspInterruptCallback(string info)
    {
        Debug.LogError("Rtsp stream interrupted: " + info);
        //if stream send stop stream event
        Lib.StopRTSPStream();
        isStreaming = false;
    }

    private void Update()
    {
        if ( Input.GetKeyDown( KeyCode.P ) )
            ToggleRtspStream();
    }

    public void ToggleRtspStream()
    {
        if(GetComponent<UwcWindowTexture>().window.isAlive)
        {
            if (isStreaming)
            {
                Debug.Log("Stopping rtsp streaming");
                Lib.StopRTSPStream();
                isStreaming = false;
            }
            else
            {
                Debug.Log("Starting rtsp streaming");
                win_id = WindowTexture.window.id;

                isStreaming = (trueWindowSize)
                    ? Lib.StartRTSPStream(win_id, rtspURL, targetFps)
                    : Lib.StartRTSPStreamWithRes(win_id, rtspURL, targetFps, width, height);
                if (!isStreaming) Debug.Log("[Unity]Failed to start RTSP stream");
            }
        }
    }

    public void ChangeResolution( int ind )
    {
        switch ( ind )
        {
            case 0:
                trueWindowSize = true;

                break;
            case 1:
                width = (int)(720f * WindowTexture.window.width / WindowTexture.window.height);
                if ( width % 2 != 0 )
                    width--;
                height = 720;
                trueWindowSize = false;

                break;
            case 2:
                width = (int)(480f * WindowTexture.window.width / WindowTexture.window.height);
                if (width % 2 != 0)
                    width--;
                height = 480;
                trueWindowSize = false;

                break;
            case 3:
                width = (int)(360f * WindowTexture.window.width / WindowTexture.window.height);
                if (width % 2 != 0)
                    width--;
                height = 360;
                trueWindowSize = false;

                break;
            case 4:
                width = (int)(240f * WindowTexture.window.width / WindowTexture.window.height);
                if (width % 2 != 0)
                    width--;
                height = 240;
                trueWindowSize = false;
                break;
        }
    }

            
            

}
