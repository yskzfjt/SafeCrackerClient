#define FORCE_BROWSER_LOCAL
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SystemScript : MonoBehaviour {
//     Game game = null;
    ////////////////////////////////////////////////
    //public
    ////////////////////////////////////////////////

    ////////////////////////////////////////////////
    //private
    ////////////////////////////////////////////////
    private Mode[] modes = new Mode[ (int)Mode.MODES.MAX ];
    private Mode curMode = null;
    private GameInfo info = new GameInfo();
    private ControllerScript ctrl;
    private SoundPlayer snd = new SoundPlayer();
    //private GameStatus gameStatus = GameStatus.Instance;

    ////////////////////////////////////////////////
    //accessor
    ////////////////////////////////////////////////
    public GameInfo Info{get{ return info; }    }
    public ControllerScript Cntrl{get{ return ctrl; }    }
    public SoundPlayer Snd{get{ return snd; }    }

    ////////////////////////////////////////////////
    //HTTP
    ////////////////////////////////////////////////
    public enum HTTP_STATUS{ NONE, WAIT, SUCCESS, FAILURE };
    protected HTTP_STATUS httpStatus;
    protected string httpResponse;

    ////////////////////////////////////////////////
    //Functions
    ////////////////////////////////////////////////
    void Start () {
	//参照解決
	ctrl = GameObject.Find("ControllerObject").GetComponent<ControllerScript>();

	//http初期化
	InitHTTP();

	//mode初期化
	InitMode();

//  	game = GameObject.Find( "Game" ).GetComponent<Game>();
    }

    void Update () {
	UpdateMode();
// 	game.OnUpdate();
// 	status.Update();
    }


    ////////////////////////////////////////////////
    //mode
    ////////////////////////////////////////////////
    void UpdateMode(){

	//mode変更リクエストあり
	if( Mode.IsRequested() ){
	    curMode.OnEnd();
	    Mode.ChangeMode();
	    curMode = modes[ Mode.GetModeAsInt() ];
	    curMode.OnStart();
	}

	//update
	curMode.OnUpdate();

	//common update
	Mode.CommonUpdate();

    }

    void InitMode(){

	Mode.SetSystem( this );
	Mode.SetGameInfo( this.info );
	Mode.SetController( this.ctrl );
	Mode.SetSoundPlayer( this.snd );

	modes[ (int)Mode.MODES.NONE ] = new Mode();
	modes[ (int)Mode.MODES.INIT ] = new InitMode();
	modes[ (int)Mode.MODES.OPENING ] = new OpeningMode();
	modes[ (int)Mode.MODES.GAME ] = new GameMode();
	modes[ (int)Mode.MODES.HELP ] = new HelpMode();
	modes[ (int)Mode.MODES.ERROR ] = new ErrorMode();
	modes[ (int)Mode.MODES.ENDING ] = new Mode();

	//初期化.GameObjectのAwakeとは違って、
	//この時点で全てのオブジェクトのロードは終わっている。
	foreach( Mode m in modes ){
	    m.OnAwake();
	}

	//最初のModeリクエスト。
	curMode = modes[ 0 ];
	Mode.Request( Mode.MODES.INIT );
    }

    ///////////////////////////////////////////////////////
    //http 
    ///////////////////////////////////////////////////////
    public void InitHTTP(){
	httpStatus = HTTP_STATUS.NONE;
    }
    public bool IsHTTPDone(){ return httpStatus != HTTP_STATUS.WAIT; }
    public bool IsHTTPSuccess(){ return httpStatus == HTTP_STATUS.SUCCESS; }
    public bool IsHTTPFailure(){ return httpStatus == HTTP_STATUS.FAILURE; }
    public string GetHTTPResponse(){ return httpResponse; }
    public bool SendHTTPRequest( string url, string name, string xml ){
	if( httpStatus == HTTP_STATUS.NONE ){
	    StartCoroutine( Post( url, name, xml ) );
	    return true;
	}else{
	    return false;
	}
    }

#if (UNITY_EDITOR || FORCE_BROWSER_LOCAL)

    //dummy
    IEnumerator Post (string url, string name, string xml) {
	httpStatus = HTTP_STATUS.WAIT;
	httpResponse = null;
	float wait = UnityEngine.Random.Range( 0.1f, 4.0f );

	Debug.Log("HTTP: Sending Request! " + url);
	//Debug.Log(xml);
	//debugText.GetComponent<Text>().text = xml;
	ctrl.Indicator.SetActive(true);
	yield return new WaitForSeconds( wait );
	ctrl.Indicator.SetActive(false);

	DummyServer dummy = new DummyServer( url, name, xml );

	httpResponse = dummy.httpResponse;
	httpStatus = HTTP_STATUS.SUCCESS;

	Debug.Log("HTTP: SUCCESS! " + httpResponse );
    }
#else

    //real
    IEnumerator Post (string url, string name, string xml) {
	httpStatus = HTTP_STATUS.WAIT;
	httpResponse = null;

	string error;
	
	//これを足すと勝手にPostになるらしい。
	WWWForm form = new WWWForm();
	form.AddField (name, xml);

        WWW www = new WWW (url, form);

        // 送信開始
	Debug.Log("HTTP: Sending Request! " + url);
	Debug.Log(xml);
        yield return www;

	error = www.error;
	if( error != null ){
	    httpResponse = error;
	    httpStatus = HTTP_STATUS.FAILURE;
	    Debug.Log("HTTP: FAILURE! " + httpResponse );
	}else{
	    httpResponse = System.Text.Encoding.UTF8.GetString(www.bytes, 0, www.bytes.Length - 0);
	    httpStatus = HTTP_STATUS.SUCCESS;
	    Debug.Log("HTTP: SUCCESS! " + httpResponse );
	}
    }
#endif


}
