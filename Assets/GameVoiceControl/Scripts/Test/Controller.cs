using System;
using UnityEngine;

public class Controller : MonoBehaviour 
{
    public static Action<string> VoiceDarkMagic;
    public static Action<string> VoicelightMagic;

    public string CmdBaseDarkSpell;
    public string CmdBaseHolySpell;	

    public void onReceiveRecognitionResult( string result ) 
    {
        if (result.Contains(CmdBaseDarkSpell))
        {            
            VoiceDarkMagic?.Invoke(CmdBaseDarkSpell);            
        }

        if ( result.Contains(CmdBaseHolySpell))
        {
            VoicelightMagic?.Invoke(CmdBaseHolySpell);
        }
    }
}
