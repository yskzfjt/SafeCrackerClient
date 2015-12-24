using UnityEngine;
using System.Text; 
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

public class GameInfo{

    ////////////////////////////////////////////
    //Game Phases
    ////////////////////////////////////////////
    public enum PHASE { 
	NONE,			//not set yet.
	    NEW_GAME,		//clear digits and history... TRANSACTION
	    SELECT,		//selecting bet and 4 digits and push buttos.
	    TRY,		//exec button pushed. waiting for server response. TRANSACTION
	    GIVE_UP,		//to new game starts.
	    RESULT,		//result for each try. 
	    LOST,		//result for whole game. consumed maxTries times.
	    WIN,		//WIN.
	    ERROR,		//showing error dialog.
	    MAX };


    ////////////////////////////////////////////
    //Static Info. set once from InitMode.
    ////////////////////////////////////////////
    public static readonly string StaticInfoURL = "/InitAPI";
    public static readonly string StaticInfoParamName = "staticInfo";
    [System.Serializable] [XmlRoot("staticInfo")] public class StaticInfo{
	public int maxTries = 7;
	public int maxDigits = 4;
	public int[] bets = { 
	    10, //bet額
	    20, 
	    50, 
	    100, 
	    200, 
	    500,
		1000,
		2000,
		10000
	};
	public int[] odds = { 
		7777,//初回の倍率
		500,
		120,
		50,
		25,
		15,
		10,
	};
	public string version = "0.0";
    }
    protected StaticInfo s = new StaticInfo();
    //accessors
    public int MaxTries{ get{ return s.maxTries; } }
    public int MaxDigits{ get{ return s.maxDigits; } }
    public int[] Bets{ get{ return s.bets; } }
    public int[] Odds{ get{ return s.odds; } }

    ////////////////////////////////////////////
    //Dynamic Info. set many times from GameMode
    ////////////////////////////////////////////
    public static readonly string DynamicInfoURL = "/GameAPI";
    public static readonly string DynamicInfoParamName = "dynamicInfo";
    [System.Serializable] [XmlRoot("dynamicInfo")] public class DynamicInfo{
	public string timeStamp = "-";
	public string token = "-";
	public string message = "-";
	
	public PHASE curPhase = PHASE.NONE;
	public PHASE reqPhase = PHASE.NONE;
	public bool isAuto = false;
	
	public int gameID = 0;
	public int tryID = 0;
	
	public int creditBalance = 0;
	public int bet = 0;
	public int reward = 0;

	public int curCrack = 0;

	public List<int> digits = new List<int>();
	public List<int> crackedDigits = new List<int>();
    }
    protected DynamicInfo d = new DynamicInfo();

    //accessors
    public string TimeStamp{ get{ return d.timeStamp; } }
    public string Token{ get{ return d.token; } }
    public string Message{ get{ return d.message; } }
    public PHASE CurPhase{ get{ return d.curPhase; } }
    public PHASE ReqPhase{ get{ return d.reqPhase; } }
    public bool IsAuto{ get{ return d.isAuto; } set{ d.isAuto = value; } }
    public int CreditBalance{ get{ return d.creditBalance; } set{ d.creditBalance = value; } }
    public int TryID{ get{ return d.tryID; } set{ d.tryID = value; } }
    public int GameID{ get{ return d.gameID; } set{ d.gameID = value; } }
    public int Bet{ get{ return d.bet; } set{ d.bet = value; } }
    public int Reward{ get{ return d.reward; } set{ d.reward = value; } }
    public int CurCrack{ get{ return d.curCrack; } set{ d.curCrack = value; } }
    public List<int> Digits{ get{ return d.digits; } }
    public List<int> CrackedDigits{ get{ return d.crackedDigits; } }
    
    ////////////////////////////////////////////
    //Local Info
    ////////////////////////////////////////////
    protected int betNo = 0;
    protected int cursorPos = 0;
    protected PHASE oldPhase = PHASE.NONE;
    protected int phaseCounter = 0;
    protected int phaseStatus = 0;
    //accessor
    public int BetNo{	get{ return betNo; } set{ betNo = value; }    }
    public int CursorPos{ get{ return cursorPos; } set{ cursorPos = value; }    }
    public int PhaseCounter{ get{ return phaseCounter; } }
    public int PhaseStatus{ get{ return phaseStatus; } set{ phaseStatus = value; }    }
    
    ////////////////////////////////////////////
    //Functions
    ////////////////////////////////////////////
    public GameInfo(){
    }
    public void OnUpdate(){
	if( ReqPhase != PHASE.NONE ){
	    oldPhase = CurPhase;
	    d.curPhase = ReqPhase;
	    d.reqPhase = PHASE.NONE;
	    phaseCounter = 0;
	    phaseStatus = 0;

	    Debug.Log( "Change Phase From: " + oldPhase + " To: " + CurPhase );
	}else{
	    ++phaseCounter;
	}
    }
    public void RequestPhase( PHASE p ){
	d.reqPhase = p;
    }
    public bool IsPhase( PHASE p ){
	return CurPhase == p;
    }

    ////////////////////////////////////////////
    //Exec Auto Control
    ////////////////////////////////////////////
    public void ExecAutoAtCurrentCursor(){
	ExecAutoAt( CursorPos );
	IncCursor();
    }
    public void ExecAutoAt( int i ){
	List<int> candidates = GetCandidates( i );
	int num = candidates[ (int)UnityEngine.Random.Range(0, candidates.Count-1) ];
	SetDigitAt( i, num );
    }
    public void ExecAuto(){
	for( int i=0; i<MaxDigits; ++i ){
	    if( CrackedDigits[ i ] < 0 ){
		ExecAutoAt( i );
	    }
	}
	CursorPos = MaxDigits;
    }

    ////////////////////////////////////////////
    //Digits Controll
    ////////////////////////////////////////////
    public void InitDigits(){
	//入力ヒストリーを初期化。
	Digits.Clear();
	CrackedDigits.Clear();
	for( int i=0; i<MaxTries; ++i ){
	    for( int j=0; j<MaxDigits; ++j ) Digits.Add( -1 );
 	}
	for( int i=0; i<MaxDigits; ++ i ){
	    CrackedDigits.Add(-1);
	}
    }
    public bool IsDigitsFull(){
	//入力終了か？
	return CursorPos >= MaxDigits;
    }
    public bool IsValidDigit( int d ){
	//範囲チェック
	return ( d >=0 && d < 10 );
    }
    public int GetCrackedDigitsCount(){
	//クラックされた桁の数
	int ret = 0;
	foreach( int num in CrackedDigits ){
	    if( num >= 0 ) ++ret;
	}
	return ret;
    }
    public int Index( int i, int j ){ 
	//二次元配列的な
	return i*MaxDigits + j;
    }
    public void CrackDigitAt( int c ){
	//c桁目をクラック
	if( !IsValidCursor(c) ) return;
	if( !IsValidTry(TryID) ) return;
	CrackedDigits[ c ] = GetDigitAt( c );
    }

    public int GetDigitAt( int c ){
	//現在入力中のc桁目
	return GetDigitAt( TryID, c );
    }
    public int GetDigitAt( int t, int c ){
	//t回目のc桁目
	if( !IsValidCursor(c) ) return -1;
	if( !IsValidTry(t) ) return -1;
	return Digits[ Index(t, c) ];
    }

    public void SetDigitAt( int t, int c, int d ){
	//t回目のc桁目にdを入力
	if( !IsValidCursor(c) ) return;
	if( !IsValidTry(t) ) return;
	if( !IsValidDigit(d) ) return;
	Digits[ Index(t , c) ] = d;
    }
    public void SetDigitAt( int c, int d ){
	//現在入力中のc桁目にdを入力
	SetDigitAt( TryID, c, d );
    }
    public void ClearDigitAt( int t, int c ){
	//t回目のc桁目を未入力に
	if( !IsValidCursor(c) ) return;
	if( !IsValidTry(t) ) return;
	Digits[ Index(t , c) ] = -1;
    }
    public void ClearDigitAt( int c ){
	//現在入力中のc桁目を未入力に
	ClearDigitAt( TryID, c );
    }

    //returns -1 if it's not cracked yet
    public int GetCrackedDigitAt( int c ){
	//c桁目のクラックされた数字
	if( !IsValidCursor(c) ) return -1;
	return CrackedDigits[ c ];
    }

    public bool IsCrackedDigitAt( int c ){
	//c桁目はすでにクラックされている
	return GetCrackedDigitAt(c) >= 0;
    }

    public bool IsUniqueDigitAt( int c ){
	//c桁目はこれまで入力していない数が入力されている
	if( TryID == 0 ) return true;
	int cur = GetDigitAt( c );
	for( int i=0; i<TryID; ++i ){
	    if( GetDigitAt( i, c ) == cur ) return false;
	}
	return true;
    }

    public List<int> GetCandidates( int cursor ){
	//これまで入力していない数のリスト
	List<int> c = new List<int>();
	if( TryID == 0 ){
	    c.Add(0);	    c.Add(1);
	    c.Add(2);	    c.Add(3);
	    c.Add(4);	    c.Add(5);
	    c.Add(6);	    c.Add(7);
	    c.Add(8);	    c.Add(9);
	    return c;
	}
	
	for( int n=0; n<10; ++n ){
	    bool flg = true;
	    for( int t=0; flg && t<TryID; ++t ){
		if( n == GetDigitAt( t, cursor ) ) flg = false;
	    }
	    if( flg ) c.Add( n );
	    
	}
	return c;
    }

    public bool SetCurrentDigit( int no ){
	if( !IsPhase( PHASE.SELECT ) ) return false;
	if( IsDigitsFull() ) return false;
	if( !IsValidDigit( no ) ) return false;
	if( !IsValidCursor(CursorPos) ) return false;
	if( !IsValidTry(TryID) ) return false;
	SetDigitAt( TryID, CursorPos, no );
	IncCursor();
	return true;
    }
    public bool ClearCurrentDigit(){
	if( !IsPhase( PHASE.SELECT ) ) return false;
	if( !IsValidTry( TryID ) ) return false;
	if( CursorPos == 0 && GetDigitAt( CursorPos )<0 ) return false;

	DecCursor();
	ClearDigitAt( CursorPos );
	return true;
    }
//     public void ResetDigitsAll(){
// 	//clear history
// 	for( int i=0; i<maxTries; ++i ){
// 	    for( int j=0; j<maxDigits; ++j ){
// 		ClearDigitAt(i,j);
// 	    }
// 	}
// 	//clear crackedDigits
// 	for( int i=0; i<maxDigits; ++i ) data.crackedDigits[i] = -1;
//     }


    ////////////////////////////////////////////
    //Try Control
    ////////////////////////////////////////////
    public bool IsFinalTry( ){
	return TryID == (MaxTries-1);
    }
    public bool IsValidTry( int no ){
	return (no >= 0 && no < MaxTries );
    }
    public void StartNextTry(){
	if( TryID < (MaxTries-1) ){
	    ++TryID;
	    for( int i=0; i<MaxDigits; ++i ){
		if( IsCrackedDigitAt(i) ){
		    SetDigitAt( i, GetCrackedDigitAt(i) );
		}
	    }
	    ResetCursor();
	    SetReward();
	}
    }

    ////////////////////////////////////////////
    //Cursor control
    ////////////////////////////////////////////
    public bool IsValidCursor( int no ){
	return ( no >= 0 && no < MaxDigits );
    }
    public void ResetCursor(){
	CursorPos = -1;
	IncCursor();
    }
    public void IncCursor(){
	if( CursorPos >= MaxDigits ) return;
	for( ++CursorPos; CursorPos<MaxDigits; ++CursorPos ){
	    if( !IsCrackedDigitAt( CursorPos ) ) break;
	}
    }
    public void DecCursor(){
	if( CursorPos <= 0 ) return;
	for( --CursorPos; CursorPos>=0; --CursorPos ){
	    if( !IsCrackedDigitAt( CursorPos ) ) break;
	}
    }

    ////////////////////////////////////////////
    //Win Lose Control
    ////////////////////////////////////////////
    public bool IsWin(){
	for( int i=0; i<MaxDigits; ++i ){
	    if( CrackedDigits[i] < 0 ) return false;
	}
	return true;
    }
    public bool IsLost(){
	return !IsWin() && IsFinalTry();
    }

    ////////////////////////////////////////////
    //Bet Controll
    ////////////////////////////////////////////
    public bool IsBetAble(){
	return ( CurPhase == PHASE.SELECT && TryID == 0 );
    }
    public void SetBet(){
	Bet = Bets[BetNo];
    }
    public bool UpBet(){
	if( IsBetAble() && (Bets.Length-1) > BetNo ){
	    ++BetNo;
	    SetBet();
	    return true;
	}else{
	    return false;
	}
    }
    public bool DownBet(){
	if( IsBetAble() && BetNo > 0 ){
	    --BetNo;
	    SetBet();
	    return true;
	}else{
	    return false;
	}
    }
    public void ResetBet(){
	BetNo = 0;
	SetBet();
    }

    ////////////////////////////////////////////
    //Reward Control
    ////////////////////////////////////////////
    public void SetReward(){
	SetBet();
	Reward = Bet * Odds[ TryID ];
    }

    ////////////////////////////////////////////
    //DynamicInfo Init after StatycInfo
    ////////////////////////////////////////////
    public void InitDynamicInfo(){
	TryID = 0;
	GameID = 0;
	InitDigits();
	SetBet();
	SetReward();
	ResetCursor();
    }
    public void NewDynamicInfo(){
	TryID = 0;
	InitDigits();
	SetBet();
	SetReward();
	ResetCursor();
    }

    ////////////////////////////////////////////////////////////////////
    //Serialize and Deserialize
    ////////////////////////////////////////////////////////////////////
    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }
    public string SerializeStaticInfo(){
  	using( StringWriter textWriter = new Utf8StringWriter() ){
	    var serializer = new XmlSerializer(typeof( StaticInfo ));
	    serializer.Serialize(textWriter, s);
	    return textWriter.ToString();
	}
    }
    public string SerializeDynamicInfo(){
  	using( StringWriter textWriter = new Utf8StringWriter() ){
	    var serializer = new XmlSerializer(typeof( DynamicInfo ));
	    d.timeStamp = System.DateTime.Now.ToString("yyyyMMddHHmmss");
	    serializer.Serialize(textWriter, d);
	    return textWriter.ToString();
	}
    }
    public void DeserializeStaticInfo( string xmlString ){
	using (TextReader textReader = new StringReader(xmlString)){
	    XmlSerializer deserializer = new XmlSerializer( typeof(StaticInfo) );
	    s = (StaticInfo)deserializer.Deserialize(textReader);
	}
    }
    static public DynamicInfo sDeserializeDynamicInfo( string xmlString ){
	using (TextReader textReader = new StringReader(xmlString)){
	    XmlSerializer deserializer = new XmlSerializer( typeof(DynamicInfo) );
	    return (DynamicInfo)deserializer.Deserialize(textReader);
	}
    }
    public void DeserializeDynamicInfo( string xmlString ){
	d = sDeserializeDynamicInfo( xmlString );
// 	using (TextReader textReader = new StringReader(xmlString)){
// 	    XmlSerializer deserializer = new XmlSerializer( typeof(DynamicInfo) );
// 	    d = (DynamicInfo)deserializer.Deserialize(textReader);
// 	}
    }
}