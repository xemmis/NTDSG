using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GetAvailableMicrophones : MonoBehaviour {
    void Start( ) {
        var list = Microphone.devices.ToList( );
        var dropdown = GetComponent<Dropdown>( );
        if ( dropdown != null ) {
            dropdown.ClearOptions( );
            dropdown.AddOptions( list );
        }
    }
    void Update( ) {
        
    }
}
