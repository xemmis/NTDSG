using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System.Text;
using System;

public class Log : MonoBehaviour {

    private Text log;

    public void add( string str) {
        var sb = new StringBuilder( );
        sb.Append( DateTime.Now.ToString( "[HH:mm:ss] " ) );
        sb.Append( str + "\n" );
        sb.Append( log.text );
        log.text = sb.ToString( );
        Debug.Log( sb.ToString( ) );
    }

    public void clear( ) {
        log.text = string.Empty;
    }

    void Awake( ) {
        log = this.GetComponent<Text>( );
        if ( log != null ) {

        }
    }

	// Use this for initialization
	void Start ( ) {
        add("log init!");
	}
	
	// Update is called once per frame
	void Update ( ) {
	
	}
}
