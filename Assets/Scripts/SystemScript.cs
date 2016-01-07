using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//for communication with js
using System.Runtime.InteropServices;

public class SystemScript : MonoBehaviour {
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
    //java scriptとの連携をテスト
    ////////////////////////////////////////////////
    [DllImport("__Internal")]
    private static extern void Hello();

    [DllImport("__Internal")]
    private static extern void HelloString(string str);

    [DllImport("__Internal")]
    private static extern void PrintFloatArray(float[] array, int size);

    [DllImport("__Internal")]
    private static extern int AddNumbers(int x, int y);

    [DllImport("__Internal")]
    private static extern string StringReturnValueFunction();

    [DllImport("__Internal")]
    private static extern string URLParams( string name );

    void Start0() {
        Hello();
        
        HelloString("This is a string.");
        
        float[] myArray = new float[10];
        PrintFloatArray(myArray, myArray.Length);
        
        int result = AddNumbers(5, 7);
        Debug.Log(result);
        
        Debug.Log(StringReturnValueFunction());

	Debug.Log(URLParams( "user" ));

    }

    ////////////////////////////////////////////////
    //Functions
    ////////////////////////////////////////////////
    void Start () {
#if !UNITY_EDITOR
    //Start0();
#endif
	//ゲームオブジェクト参照解決
	ctrl = GameObject.Find("ControllerObject").GetComponent<ControllerScript>();

	//http初期化
	InitHTTP();

	//mode初期化
	InitMode();

    }

    void Update () {
	//毎フレーム呼ばれる。
	//呼ばれるゲームオブジェクトの順番は不定。
	UpdateMode();
    }

    void LateUpdate(){
	//全ゲームオブジェクトのUpdateが終わってから呼ばれる。
	//ゲームフェーズの変更はこの中で行われる。
	//この関数を使うのはSystemScriptだけにしたほうが良いかもしれない。
    	info.OnUpdate();
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

    //ダミーサーバーは廃止する方向で。
// #if (FORCE_DUMMY_SERVER)
//     //dummy
//     IEnumerator Post (string url, string name, string xml) {
// 	url = Constants.BASE_URL + url;

// 	httpStatus = HTTP_STATUS.WAIT;
// 	httpResponse = null;

// 	Debug.Log("HTTP: Sending Request! " + url);
// 	//Debug.Log(xml);
// 	//debugText.GetComponent<Text>().text = xml;

// 	yield return new WaitForSeconds( UnityEngine.Random.Range( 0.1f, 4.0f ) );

// 	ctrl.Indicator.SetActive(false);

// 	DummyServer dummy = new DummyServer( url, name, xml );

// 	httpResponse = dummy.httpResponse;
// 	httpStatus = HTTP_STATUS.SUCCESS;

// 	Debug.Log("HTTP: SUCCESS! " + httpResponse );
//     }
// #else
    IEnumerator Post (string url, string name, string xml) {
	httpStatus = HTTP_STATUS.WAIT;
	httpResponse = null;

	url = Constants.BASE_URL + url;
	string error;
	
	//これを足すと勝手にPostになるらしい。
	WWWForm form = new WWWForm();
	form.AddField (name, xml);

        WWW www = new WWW (url, form);

        // 送信開始
	ctrl.Indicator.SetActive(true);
	Debug.Log("HTTP: Sending Request! " + url);
        yield return www;
	ctrl.Indicator.SetActive(false);

	//とりあえず、通信したxmlを渡してデシリアライズ。
	xml = System.Text.Encoding.UTF8.GetString(www.bytes, 0, www.bytes.Length - 0);

	//ctrl.DebugText.SetActive( true );;
	ctrl.DebugText.GetComponent<Text>().text = xml;
	Debug.Log(xml);

	error = www.error;
	if( error != null ){
	    httpResponse = error;
	    httpStatus = HTTP_STATUS.FAILURE;
	    Debug.Log("HTTP: FAILURE! " + httpResponse );
	}else{
	    httpResponse = xml;
	    httpStatus = HTTP_STATUS.SUCCESS;
	    Debug.Log("HTTP: SUCCESS! " + httpResponse );

	    if( name == GameInfo.DynamicInfoParamName ){
		//GameInfo.DynamicInfo d = GameInfo.sDeserializeDynamicInfo( xml );
		info.DeserializeDynamicInfo( xml );
	    }else{
		//GameInfo.StaticInfo s = GameInfo.sDeserializeStaticInfo( xml );
		info.DeserializeStaticInfo( xml );
	    }

	}
    }
// #endif


}
