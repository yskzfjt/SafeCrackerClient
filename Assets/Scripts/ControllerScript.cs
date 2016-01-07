using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControllerScript : MonoBehaviour {
    private SystemScript sys = null;
    private GameInfo info = null;

    //この辺は最初隠れているのでGUIから設定しておく。
    public GameObject faderPanel;
    public GameObject errorPanel;
    public GameObject errorText;
    public GameObject helpPanel;
    public GameObject debugText;
    public GameObject indicator;

    protected GameObject[] historyTexts = new GameObject[ 7 ];

    protected GameObject[] digits = new GameObject[ 4 ];

    protected GameObject[] numberButtons = new GameObject[ 10 ];
    protected GameObject clearButton;
    protected GameObject autoButton;
    protected GameObject execButton;
    protected GameObject giveupButton;

    protected GameObject betUpButton;
    protected GameObject betDownButton;
    protected GameObject betText;

    protected GameObject rewardText;

    protected GameObject creditBalanceText;

    protected GameObject numberButtonPanel;
    protected GameObject betPanel;

    protected GameObject autoPlayCheckBox;


    protected int hitCounter;

    ////////////////////////////////////////////////
    //accessor
    ////////////////////////////////////////////////
    public GameObject Fader{	get{ return faderPanel; }    }
    public GameObject Error{	get{ return errorPanel; }    }
    public GameObject Help{	get{ return helpPanel; }    }
    public GameObject DebugText{get{ return debugText; }    }
    public GameObject BetUpButton{ get{ return betUpButton; } }
    public GameObject BetDownButton{ get{ return betDownButton; } }
    public GameObject ExecButton{ get{ return execButton; } }
    public GameObject GiveupButton{ get{ return giveupButton; } }
    public GameObject[] NumberButtons{ get{ return numberButtons; } }
    public GameObject ClearButton{ get{ return clearButton; } }
    public GameObject AutoButton{ get{ return autoButton; } }
    public GameObject ErrorText{ get{ return errorText; } }
    public GameObject Indicator{ get{ return indicator; } }
    public GameObject AutoPlayCheckBox{ get{ return autoPlayCheckBox; } }

    // Use this for initialization
    void Start () {
	sys = GameObject.Find( "SystemObject" ).GetComponent<SystemScript>();
	info = sys.Info;

	digits[0] = GameObject.Find( "NumberDisplay0" );
	digits[1] = GameObject.Find( "NumberDisplay1" );
	digits[2] = GameObject.Find( "NumberDisplay2" );
	digits[3] = GameObject.Find( "NumberDisplay3" );

	historyTexts[0] = GameObject.Find( "HistoryText0" );
	historyTexts[1] = GameObject.Find( "HistoryText1" );
	historyTexts[2] = GameObject.Find( "HistoryText2" );
	historyTexts[3] = GameObject.Find( "HistoryText3" );
	historyTexts[4] = GameObject.Find( "HistoryText4" );
	historyTexts[5] = GameObject.Find( "HistoryText5" );
	historyTexts[6] = GameObject.Find( "HistoryText6" );

	numberButtons[0] = GameObject.Find( "InputButton0" );
	numberButtons[1] = GameObject.Find( "InputButton1" );
	numberButtons[2] = GameObject.Find( "InputButton2" );
	numberButtons[3] = GameObject.Find( "InputButton3" );
	numberButtons[4] = GameObject.Find( "InputButton4" );
	numberButtons[5] = GameObject.Find( "InputButton5" );
	numberButtons[6] = GameObject.Find( "InputButton6" );
	numberButtons[7] = GameObject.Find( "InputButton7" );
	numberButtons[8] = GameObject.Find( "InputButton8" );
	numberButtons[9] = GameObject.Find( "InputButton9" );

	clearButton = GameObject.Find( "InputButtonClear" );
	autoButton = GameObject.Find( "InputButtonAutoFill" );
	execButton = GameObject.Find( "ExecButton" );
	giveupButton = GameObject.Find( "GiveupButton" );

	betText = GameObject.Find( "BetText" );
	betPanel = GameObject.Find( "BetPanel" );
	betUpButton = GameObject.Find( "BetUpButton" );
	betDownButton = GameObject.Find( "BetDownButton" );

	rewardText = GameObject.Find( "RewardText" );
	creditBalanceText = GameObject.Find( "CreditBalanceText" );

	numberButtonPanel = GameObject.Find( "NumberButton_Group" );

	autoPlayCheckBox = GameObject.Find( "AutoPlayCheckBox" );

    }
	
    // Update is called once per frame
    void Update () {
	//RefreshView();
    }

    string MoneyString( int cent ){
	float c = cent / 100.0f;
	return c.ToString( "C2" );
    }
    public void RefreshView(){
	Error.SetActive( false );
	switch( (int)info.CurPhase ){
	case (int)GameInfo.PHASE.NEW_GAME:{
	    EnableInputPanels( false );
	    hitCounter = 0;
	}break;
	case (int)GameInfo.PHASE.SELECT:{
	    //bet panel
	    EnableBetPanel( info.TryID == 0 );

	    //number panel
	    if( info.SemiAuto || info.AutoPlay ){
		EnableNumberPanel( false );
	    }else{
		EnableNumberPanel( true );
		if( info.IsDigitsFull() ){
		    ExecButton.GetComponent<Button>().interactable = true;
		    autoButton.GetComponent<Button>().interactable = false;
		    DisableNumberButton();
		}else{
		    ExecButton.GetComponent<Button>().interactable = false;
		    autoButton.GetComponent<Button>().interactable = true;
		    SetNumberButton();
		}
	    }
	    SetDigits();
	}break;
	case (int)GameInfo.PHASE.TRY:{
	    EnableInputPanels( false );
	    SetDigits();
	}break;
	case (int)GameInfo.PHASE.GIVE_UP:{
	    EnableInputPanels( false );
	    SetDigits();
	}break;
	case (int)GameInfo.PHASE.RESULT:{
	    EnableInputPanels( false );
	    SetDigitsResult();
	    Error.SetActive( true );
	    ErrorText.GetComponent<Text>().text = " " + info.CurCrack + (info.CurCrack > 1 ? " Hits! " : " Hit! ");
	}break;
	case (int)GameInfo.PHASE.LOST:{
	    EnableInputPanels( false );
	    SetDigitsFinal();
	    Error.SetActive( true );
	    ErrorText.GetComponent<Text>().text = " YOU LOST...";
	}break;
	case (int)GameInfo.PHASE.WIN:{
	    EnableInputPanels( false );
	    SetDigitsFinal();
	    Error.SetActive( true );
	    ErrorText.GetComponent<Text>().text = " YOU WIN!!";
	}break;
	case (int)GameInfo.PHASE.ERROR:{
	    ErrorText.GetComponent<Text>().text = sys.GetHTTPResponse();
	    Error.SetActive( true );
	    EnableInputPanels( false );
	}break;
	default: break;
	}

	betText.GetComponent<Text>().text = MoneyString( info.Bet );
	rewardText.GetComponent<Text>().text = MoneyString( info.Reward );
	creditBalanceText.GetComponent<Text>().text = MoneyString( info.CreditBalance );
	for( int i=0; i<info.MaxTries; ++i ){
	    string spc = "  ";
	    string str = "";
	    if( i < info.TryID ){
		str = 
		    spc + info.GetDigitAt(i,0) +
		    spc + info.GetDigitAt(i,1) +
		    spc + info.GetDigitAt(i,2) +
		    spc + info.GetDigitAt(i,3);
	    }
	    historyTexts[i].GetComponent<Text>().text = str;
	}

    }

    //Controlls
    public void SetDigits(){

	for( int i=0; i<info.MaxDigits; ++i ){
	    int d = info.GetDigitAt( i );
	    bool c = info.IsCrackedDigitAt( i );
	    if( d < 0 ){
		digits[i].GetComponent<NumberDisplayScript>().SetNone();
	    }else{
		if( c ) digits[i].GetComponent<NumberDisplayScript>().SetHit( d );
		else digits[i].GetComponent<NumberDisplayScript>().SetInput( d );
	    }
	}
    }
    public void SetDigitsResult(){

	for( int i=0; i<info.MaxDigits; ++i ){
	    if( info.IsCrackedDigitAt( i ) ) 
		digits[i].GetComponent<NumberDisplayScript>().SetHit( info.GetLastDigitAt(i) );
	    else 
		digits[i].GetComponent<NumberDisplayScript>().SetFail( info.GetLastDigitAt(i) );
	}
    }
    public void SetDigitsFinal(){

	for( int i=0; i<info.MaxDigits; ++i ){
	    int d = info.GetDigitAt( info.TryID-1, i );
	    bool c = info.IsCrackedDigitAt( i );
	    if( d < 0 ){
		digits[i].GetComponent<NumberDisplayScript>().SetNone();
	    }else{
		if( c ) digits[i].GetComponent<NumberDisplayScript>().SetHit( d );
		else digits[i].GetComponent<NumberDisplayScript>().SetFail( d );
	    }
	}
    }

    public void SetNumberButton(){
	foreach( GameObject g in numberButtons ){
	    g.GetComponent<Button>().interactable = false;
	}
	foreach( int n in info.GetCandidates( info.CursorPos ) ){
	    numberButtons[n].GetComponent<Button>().interactable = true;
	}
    }
    public void DisableNumberButton(){
	foreach( GameObject g in numberButtons ){
	    g.GetComponent<Button>().interactable = false;
	}
    }

    public void EnableNumberPanel( bool flg ){
	numberButtonPanel.GetComponent<CanvasGroup>().interactable = flg;
    }
    public void EnableBetPanel( bool flg ){
	betPanel.GetComponent<CanvasGroup>().interactable = flg;
    }
    public void EnableInputPanels( bool flg ){
	EnableNumberPanel( flg );
	EnableBetPanel( flg );
    }
    
}
