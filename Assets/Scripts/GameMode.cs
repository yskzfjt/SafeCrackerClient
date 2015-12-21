using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameMode : Mode  {

    public GameMode(){
	thisMode = MODES.GAME;
    }

    public override void OnStart(){
	//念のため。
	ctrl.Fader.SetActive( false );
	ctrl.Error.SetActive( false );
	ctrl.Help.SetActive( false );

	//スタート
	info.RequestPhase( GameInfo.PHASE.NEW_GAME );

	//コールバック
	ctrl.BetUpButton.GetComponent<Button>().onClick.AddListener( OnBetUpButton );
	ctrl.BetDownButton.GetComponent<Button>().onClick.AddListener( OnBetDownButton );
	ctrl.GiveupButton.GetComponent<Button>().onClick.AddListener( OnGiveupButton );
	ctrl.ExecButton.GetComponent<Button>().onClick.AddListener( OnExecButton );
	ctrl.AutoButton.GetComponent<Button>().onClick.AddListener( OnAutoButton );
	ctrl.ClearButton.GetComponent<Button>().onClick.AddListener( OnClearButton );
	ctrl.NumberButtons[0].GetComponent<Button>().onClick.AddListener( delegate{ OnNumberInputButton(0); } );
	ctrl.NumberButtons[1].GetComponent<Button>().onClick.AddListener( delegate{ OnNumberInputButton(1); } );
	ctrl.NumberButtons[2].GetComponent<Button>().onClick.AddListener( delegate{ OnNumberInputButton(2); } );
	ctrl.NumberButtons[3].GetComponent<Button>().onClick.AddListener( delegate{ OnNumberInputButton(3); } );
	ctrl.NumberButtons[4].GetComponent<Button>().onClick.AddListener( delegate{ OnNumberInputButton(4); } );
	ctrl.NumberButtons[5].GetComponent<Button>().onClick.AddListener( delegate{ OnNumberInputButton(5); } );
	ctrl.NumberButtons[6].GetComponent<Button>().onClick.AddListener( delegate{ OnNumberInputButton(6); } );
	ctrl.NumberButtons[7].GetComponent<Button>().onClick.AddListener( delegate{ OnNumberInputButton(7); } );
	ctrl.NumberButtons[8].GetComponent<Button>().onClick.AddListener( delegate{ OnNumberInputButton(8); } );
	ctrl.NumberButtons[9].GetComponent<Button>().onClick.AddListener( delegate{ OnNumberInputButton(9); } );
    }
    public override void OnUpdate(){

	//GameInfo.PHASEに応じて処理を分岐。
	switch( (int)info.CurPhase ){
	case (int)GameInfo.PHASE.NEW_GAME:{
	    UpdateNEW_GAME();
	}break;
	case (int)GameInfo.PHASE.SELECT:{
	    UpdateSELECT();
	}break;
	case (int)GameInfo.PHASE.TRY:{
	    UpdateTRY();
	}break;
	case (int)GameInfo.PHASE.GIVE_UP:{
	    UpdateGIVE_UP();
	}break;
	case (int)GameInfo.PHASE.RESULT:{
	    UpdateRESULT();
	}break;
	case (int)GameInfo.PHASE.LOST:{
	    UpdateLOST();
	}break;
	case (int)GameInfo.PHASE.WIN:{
	    UpdateWIN();
	}break;
	case (int)GameInfo.PHASE.ERROR:{
	    UpdateERROR();
	}break;
	default: break;
	}

    	info.OnUpdate();
    }
    public override void OnEnd(){
    }

    //////////////////////////////////////////////
    //NEW_GAME GameInfo.DynamicInfoの初期化
    //////////////////////////////////////////////
    protected void UpdateNEW_GAME(){
	//if( info.GameID == 0 && info.PhaseCounter == 0 ) snd.Play( "bgm", 0.2f );
	DoHTTPAndGoodBye();
    }

    //////////////////////////////////////////////
    //SELECT 数字を入力中. 
    //////////////////////////////////////////////
    protected void UpdateSELECT(){
	switch( info.PhaseStatus ){
	case 1:{//TRY
	    info.RequestPhase( GameInfo.PHASE.TRY );
	}break;
	case 2:{//GIVE UP
	    info.RequestPhase( GameInfo.PHASE.GIVE_UP );
	}break;
	}
    }
    //////////////////////////////////////////////
    //TRY ExecButtonが押された. 
    //////////////////////////////////////////////
    protected void UpdateTRY(){
	DoHTTPAndGoodBye();
    }
    //////////////////////////////////////////////
    //GIVE_UP GiveUpButtonが押された. 
    //////////////////////////////////////////////
    protected void UpdateGIVE_UP(){
	DoHTTPAndGoodBye();
    }
    //////////////////////////////////////////////
    //RESULT 途中結果の表示中
    //////////////////////////////////////////////
    protected void UpdateRESULT(){
	switch( info.PhaseCounter ){
	case 0:{
	    if( info.CurCrack != 0 ) 
		snd.Play( "hit" );
	    else
		snd.Play( "fail" );
	}break;
	case  60:{
	    info.StartNextTry();
	    info.RequestPhase( GameInfo.PHASE.SELECT );
	}break;
	default: break;
	}
    }
    //////////////////////////////////////////////
    //LOST 負けの表示中
    //////////////////////////////////////////////
    protected void UpdateLOST(){
	switch( info.PhaseCounter ){
	case 0:{
	    snd.Play( "finale" );
	    ++info.TryID;
	}break;
	case 120:{
	    ++info.GameID;
	    info.RequestPhase( GameInfo.PHASE.NEW_GAME );
	}break;
	default: break;
	}
    }
    //////////////////////////////////////////////
    //WIN 勝ちの表示中
    //////////////////////////////////////////////
    protected void UpdateWIN(){
	switch( info.PhaseCounter ){
	case 0:{
	    snd.Play( "finale" );
	    ++info.TryID;
	}break;
	case 120:{
	    ++info.GameID;
	    info.RequestPhase( GameInfo.PHASE.NEW_GAME );
	}break;
	default: break;
	}
    }
    //////////////////////////////////////////////
    //ERROR 多分、復帰はない。
    //////////////////////////////////////////////
    protected void UpdateERROR(){
	ctrl.Error.SetActive( true );
    }

    //////////////////////////////////////////////
    //call back
    //////////////////////////////////////////////
    void OnBetUpButton(){
	if( info.UpBet() ){
	    //成功
	    info.SetReward();
	    snd.Play( "push" );
	}else{
	    //失敗
	}
    }
    void OnBetDownButton(){
	if( info.DownBet() ){
	    //成功
	    info.SetReward();
	    snd.Play( "push" );
	}else{
	    //失敗
	}
    }

    void OnExecButton(){
	if( info.IsDigitsFull() ){
	    snd.Play( "push" );
	    info.PhaseStatus = 1;
	}
    }
    void OnGiveupButton(){
	info.PhaseStatus = 2;
	snd.Play( "clear" );
    }

    void OnClearButton(){
	if( info.ClearCurrentDigit() ){
	    //success
	    snd.Play( "clear" );
	}else{
	    //fail
	}
    }
    void OnNumberInputButton( int n ){
	if( info.SetCurrentDigit( n ) ){
	    //success
	    snd.Play( "push" );
	}else{
	    //fail
	}
    }
    void OnAutoButton(){
	snd.Play( "push" );
	info.ExecAutoAtCurrentCursor();
    }

    //////////////////////////////////////////////
    //HTTP util
    //////////////////////////////////////////////
    protected bool DoHTTPAndGoodBye(){
	bool flg = false;
	switch( info.PhaseStatus ){
	case 0:{
	    DoHTTP( GameInfo.DynamicInfoURL, GameInfo.DynamicInfoParamName, info.SerializeDynamicInfo() );
	    ++info.PhaseStatus;
	}break;
	case 1:{
	    //httpResponseにreqPhaseが入って勝手に分岐する。ここには来ないこともありえる。
	    if( sys.IsHTTPDone() ){
		flg = true;
	    }
	}break;
	}
	return flg;
    }
    protected bool GoAfterHTTP( GameInfo.PHASE p ){
	if( sys.IsHTTPDone() ){
	    if( sys.IsHTTPSuccess() ){
		info.DeserializeDynamicInfo( sys.GetHTTPResponse() );
		info.RequestPhase( p );
	    }else{
		info.RequestPhase( GameInfo.PHASE.ERROR );
	    }
	    return true;
	}else{
	    return false;
	}
    }

}