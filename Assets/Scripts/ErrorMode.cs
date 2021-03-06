using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ErrorMode : Mode  {

    //protected
    protected readonly int fadeFrames = 60;

    public ErrorMode(){
	thisMode = MODES.ERROR;
    }

    public override void OnStart(){
	ctrl.Fader.SetActive( true );
	Utility.SetUIAlpha( ctrl.Fader, 0.0f );
	ctrl.Error.SetActive( true );
	Utility.SetUIAlpha( ctrl.Error, 0.0f );
	ctrl.ErrorText.GetComponent<Text>().text = sys.GetHTTPResponse();
    }
    public override void OnUpdate(){
	ctrl.Error.SetActive( true );
	float rate = Utility.Rate( modeCounter, fadeFrames );
	if( rate >= 1.0f ){
	}else{
	    Utility.SetUIAlpha( ctrl.Fader, rate > 0.5f ? 0.5f : rate );
	    Utility.SetUIAlpha( ctrl.Error, rate );
	}
    }
    public override void OnEnd(){
	ctrl.Fader.SetActive( false );
	ctrl.Error.SetActive( false );
    }

}