using UnityEngine;
using System.Collections;

public class OpeningMode : Mode  {

    //protected
    protected readonly int fadeFrames = 60;

    public OpeningMode(){
	thisMode = MODES.OPENING;
    }

    public override void OnStart(){
	ctrl.Fader.SetActive( true );
	Utility.SetUIAlpha( ctrl.Fader, 1.0f );
    }
    public override void OnUpdate(){
	float rate = Utility.Rate( modeCounter, fadeFrames );
	if( rate >= 1.0f ){
	    Request( MODES.GAME );
	}else{
	    Utility.SetUIAlpha( ctrl.Fader, 1.0f - rate );
	}
    }
    public override void OnEnd(){
	ctrl.Fader.SetActive( false );
	Utility.SetUIAlpha( ctrl.Fader, 1.0f );
    }

}