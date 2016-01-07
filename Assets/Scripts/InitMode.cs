using UnityEngine;
using System.Collections;

public class InitMode : Mode  {

    private int no = 0;
    public InitMode(){
	thisMode = MODES.INIT;
    }

    public override void OnStart(){
	ctrl.EnableInputPanels( false );
	ctrl.Fader.SetActive( true );
	Utility.SetUIAlpha( ctrl.Fader, 1.0f );
	no = 0;
    }
    public override void OnUpdate(){
	switch( no ){
	case 0:{
	     //ログイン時に一度だけ記録しておくデータ。
	    //@todo 本番はこの時点ですでにクッキーが設定されている。
	    DoHTTP( GameInfo.StaticInfoURL, GameInfo.StaticInfoParamName, info.SerializeStaticInfo() );
	    ++no;
	}break;
	case 1:{
	    if( sys.IsHTTPDone() ){
		if( sys.IsHTTPSuccess() ){
		    //StaticInfoを初期化して
		    info.DeserializeStaticInfo( sys.GetHTTPResponse() );
		    //DynamicInfoも初期化.
		    info.InitDynamicInfo();
		    Request( MODES.OPENING );
		}else{
		    Request( MODES.ERROR );
		}
	    }
	}break;
	}
    }
    public override void OnEnd(){
    }

}