using UnityEngine;
using System.Collections;

public class HelpMode : Mode  {

    //protected
    protected readonly int fadeFrames = 60;

    public HelpMode(){
	thisMode = MODES.HELP;
    }

    public override void OnStart(){
	ctrl.Fader.SetActive( true );
	Utility.SetUIAlpha( ctrl.Fader, 0.0f );
	ctrl.Help.SetActive( true );
	Utility.SetUIGroupAlpha( ctrl.Help, 0.0f );
    }
    public override void OnUpdate(){
	float rate = Utility.Rate( modeCounter, fadeFrames );
	if( rate >= 1.0f ){
	    Request( MODES.OPENING );
	}else{
	    Utility.SetUIAlpha( ctrl.Fader, rate > 0.5f ? 0.5f : rate );
	    Utility.SetUIGroupAlpha( ctrl.Help, rate );
	}
    }
    public override void OnEnd(){
	ctrl.Fader.SetActive( false );
	ctrl.Help.SetActive( false );
    }

}