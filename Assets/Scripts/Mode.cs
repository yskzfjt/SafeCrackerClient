using UnityEngine;
using System.Collections;

public class Mode {
    ////////////////////////////////////////
    //public
    ////////////////////////////////////////
    public enum MODES{	NONE, INIT, OPENING, GAME, HELP, ERROR, ENDING, MAX  };

    ////////////////////////////////////////
    //protected
    ////////////////////////////////////////
    static protected MODES curMode = MODES.NONE;
    static protected MODES reqMode = MODES.NONE;
    static protected MODES oldMode = MODES.NONE;
    static protected int modeCounter = 0;
    static protected SystemScript sys = null;
    static protected GameInfo info = null;
    static protected ControllerScript ctrl = null;
    static protected SoundPlayer snd = null;
    protected MODES thisMode = MODES.NONE;
	
    //SystemObjectから呼ばれる。
    public Mode(){
	thisMode = MODES.NONE;
    }
    public virtual void OnAwake(){
    }
    public virtual void OnStart(){
    }
    public virtual void OnUpdate(){
    }
    public virtual void OnEnd(){
    }

    //static
    //共通処理
    static public void CommonUpdate(){
	++modeCounter;
    }

    //変更
    static public void Request( MODES m ){
	reqMode = m;
    }
    static public bool IsRequested(){
	return reqMode != MODES.NONE;
    }
    static public void ChangeMode(){
	oldMode = curMode;
	curMode = reqMode;
	modeCounter = 0;
	reqMode = MODES.NONE;
	Debug.Log( "Change Mode From: " + oldMode + " To: " + curMode );
    }

    //accessor
    static public bool IsMode( MODES m ){
	return curMode == m;
    }
    static public int GetModeAsInt(){
	return (int)curMode;
    }
    static public MODES GetMode(){
	return curMode;
    }
    static public MODES GetRequestedMode(){
	return reqMode;
    }

    static public void SetSystem( SystemScript s ){
	sys = s;
    }
    static public void SetGameInfo( GameInfo i ){
	info = i;
    }
    static public void SetController( ControllerScript c ){
	ctrl = c;
    }
    static public void SetSoundPlayer( SoundPlayer s ){
	snd = s;
    }

    //////////////////////////////////////////////
    //HTTP util
    //////////////////////////////////////////////
    protected void DoHTTP( string url, string name, string xml ){
	sys.InitHTTP();
	sys.SendHTTPRequest( url, name, xml );
    }
//     protected bool GoAfterHTTP( GameInfo.PHASE p ){
// 	if( sys.IsHTTPDone() ){
// 	    if( sys.IsHTTPSuccess() ){
// 		info.DeserializeDynamicInfo( sys.GetHTTPResponse() );
// 		info.RequestPhase( p );
// 	    }else{
// 		info.RequestPhase( GameInfo.PHASE.ERROR );
// 	    }
// 	    return true;
// 	}else{
// 	    return false;
// 	}
//     }


}
