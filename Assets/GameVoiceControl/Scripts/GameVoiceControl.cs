using System;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;

public class GameVoiceControl : MonoBehaviour, IGameVoiceControl {
    public enum language {
        en_US,
        ru_RU,
        fr_FR,
        de_DE,
        es_ES,
        it_IT,
        nl_NL,
        hi_IN,
        pt_PT
    }
    [Header("Debug mode")]
    public bool Enable = true;
    [Header("Select language")]
    public language lang;
    [Header("Keyword (OK GOOGLE)")]
    public string keyword;
    [Header("Keyword (OK GOOGLE) threshold [1e-50..1e-1]")]
    public double keywordThreshold = 1e-10f;
    [Header("Voice activity detection threshold [1..5]")]
    public double vadThreshold = 4.0;
    [Header("Set microphone index")]
    public byte microphoneIndex = 0;
    [Header("Time to read microphone buffer (ms)")]
    public float timeoutInterval = 100.0f;
    [Header("Grammars")]
    public GrammarFileStruct[ ] grammarStructs;
    [Header("Made-up words")]
    public PairG2P[ ] pairsOfGraphemePhonemes;
    [System.Serializable]
    public class ResultEvent : UnityEvent<String> { }
    [System.Serializable]
    public class InitEvent : UnityEvent<bool> { }

    public ResultEvent RecognitionResult = new ResultEvent( );
    public InitEvent InitResult = new InitEvent( );

    [Header( "Log" )]
    public Log log = null;



    public void onRecieveLogMess( string message ) {
        if ( Enable ) {
            if ( log == null ) return;
            log.add( "log:" + message );
        }
    }
    public void onRecognitionResult( string result ) {
        if ( log != null ) {
            log.clear( );
        }
        if ( keyword == string.Empty ) {
            RecognitionResult?.Invoke( result );
            if ( log != null ) {
                log.add( "<color=green>recognized [" + result + "]</color>" );
            }
        }
        else {
            if ( result == keyword ) {
                if ( log != null ) {
                    log.add( "<color=green>keyword [" + result + "] detected</color>" );
                }
                _speechRecognizer.switchGrammar( grammarStructs[ 0 ].name );
            }
            else {
                if ( RecognitionResult != null ) {
                    RecognitionResult?.Invoke( result );
                    if ( log != null ) {
                        log.add( "<color=green>recognized [" + result + "]</color>" );
                        log.add( $"<color=blue>keyword [{keyword}] waiting...</color>" );
                    }
                    _speechRecognizer.searchKeyword( );
                }
            }
        }
    }
    public void onStartListening( ) {
        if ( !_start ) {
            if ( log != null ) {
                log.clear( );
            }
            if ( ( log != null ) && ( keyword != string.Empty ) ) {
                log.add( $"<color=blue>keyword [{keyword}] waiting...</color>" );
            }
            _speechRecognizer.startListening( );
            _start = true;
        }
    }
    public void onStopListening( ) {
        if ( _start ) {
            _speechRecognizer.stopListening( );
            _start = false;
        }
    }
    public void onError( string message ) {
        if ( log != null ) {
            log.add( "<color=red>crash:" + message + "</color>" );
        }
    }
    public void onInitResult( string result ) {
        if ( log != null ) {
            log.add( "<color=green>init complete:" + result + "</color>" );
        }
        _speechRecognizer.onInitResult( result );
        if ( InitResult != null )
            InitResult?.Invoke( result == BaseSpeechRecognizer.TRUE);
    }
    public string getObjectName( ) {
        return this.gameObject.name;
    }

    private void initSpeechRecognizer( ) {
        if ( _speechRecognizer != null ) {
            foreach ( var pair in pairsOfGraphemePhonemes ) {
                _speechRecognizer.addPairG2P( pair );
            }
            foreach ( var grammar in grammarStructs ) {
                for ( int i = 0; i < grammar.commands.Length; i++ ) {
                    grammar.commands[ i ] = grammar.commands[ i ].ToLower( );
                }
            }
            _speechRecognizer.keywordThreshold = keywordThreshold;
            _speechRecognizer.setVadThreshold( vadThreshold );
            _speechRecognizer.setTimeoutInterval( timeoutInterval );

            _speechRecognizer.initialization( this, lang.ToString( ), grammarStructs, keyword, microphoneIndex.ToString( ) );
        }
    }

    void Awake( ) {
        if ( Application.platform == RuntimePlatform.Android ) {
            _speechRecognizer = new AndroidSpeechRecognizer( );
        }
        else {
            _speechRecognizer = new DesktopSpeechRecognizer( Enable );
        }
    }

    void Start( ) {
        if ( Application.platform == RuntimePlatform.Android ) {
            if ( !Permission.HasUserAuthorizedPermission( Permission.Microphone ) ) {
                Permission.RequestUserPermission( Permission.Microphone );
            }
            if ( !Permission.HasUserAuthorizedPermission( Permission.ExternalStorageWrite ) ) {
                Permission.RequestUserPermission( Permission.ExternalStorageWrite );
            }
        }
    }

    void Update( ) {
        bool micAutorized = true;
        bool externalStorageAvailable = true;
        if ( Application.platform == RuntimePlatform.Android ) {
            micAutorized = Permission.HasUserAuthorizedPermission( Permission.Microphone );
            externalStorageAvailable = Permission.HasUserAuthorizedPermission( Permission.ExternalStorageWrite );
        }
        if ( micAutorized && externalStorageAvailable ) {
            if ( !_initRequested ) {
                initSpeechRecognizer( );
            }
            _initRequested = true;

            _speechRecognizer.processing( );
        }
    }

    void OnDestroy( ) {
        this._speechRecognizer.Dispose( );
        this._speechRecognizer = null;
    }

    bool _start = false;
    BaseSpeechRecognizer _speechRecognizer = null;
    private bool _initRequested = false;
#if UNITY_ANDROID
    Microphone mic = new Microphone();
#endif
}
