using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public class SlotBehaviour : MonoBehaviour
{
  [SerializeField]
  private RectTransform mainContainer_RT;

  [Header("Sprites")]
  [SerializeField]
  private Sprite[] myImages;  //images taken initially



  [SerializeField]
  private Sprite TurboToggleSprite;

  [Header("Slot Images")]
  [SerializeField]
  private List<SlotImage> images;     //class to store total images
  [SerializeField]
  private List<SlotImage> Tempimages;     //class to store the result matrix

  [Header("Slots Objects")]
  [SerializeField]
  private GameObject[] Slot_Objects;
  [Header("Slots Elements")]
  [SerializeField]
  private LayoutElement[] Slot_Elements;

  [Header("Slots Transforms")]
  [SerializeField]
  private Transform[] Slot_Transform;

  [Header("Line Button Objects")]
  [SerializeField]
  private List<GameObject> StaticLine_Objects;

  [Header("Line Button Texts")]
  [SerializeField]
  private List<TMP_Text> StaticLine_Texts;

  [Header("Line Button Objects")]
  [SerializeField]
  private List<ManageLineButtons> StaticLine_Scripts;

  [Header("Line Button Objects")]
  [SerializeField]
  private List<Button> StaticLine_Buttons;


  private Dictionary<int, string> x_string = new Dictionary<int, string>();
  private Dictionary<int, string> y_string = new Dictionary<int, string>();

  [Header("Buttons")]
  [SerializeField]
  private Button SlotStart_Button;
  [SerializeField]
  private Button AutoSpin_Button;
  [SerializeField]
  private Button AutoSpinStop_Button;
  [SerializeField]
  private Button MaxBet_Button;
  [SerializeField]
  private Button BetPlus_Button;
  [SerializeField]
  private Button BetMinus_Button;
  [SerializeField]
  private Button LinePlus_Button;
  [SerializeField]
  private Button LineMinus_Button;

  [SerializeField]
  private Button StopSpin_Button;

  [SerializeField]
  private Button Turbo_Button;


  [Header("Animated Sprites")]
  [SerializeField]
  private Sprite[] Nine_Sprite;
  [SerializeField]
  private Sprite[] Ten_Sprite;
  [SerializeField]
  private Sprite[] J_Sprite;
  [SerializeField]
  private Sprite[] K_Sprite;
  [SerializeField]
  private Sprite[] Q_Sprite;
  [SerializeField]
  private Sprite[] A_Sprite;
  [SerializeField]
  private Sprite[] Hedgehog_Sprite;
  [SerializeField]
  private Sprite[] Crab_Sprite;
  [SerializeField]
  private Sprite[] JellyFish_Sprite;
  [SerializeField]
  private Sprite[] Turtle_Sprite;
  [SerializeField]
  private Sprite[] Shell_Sprite;
  [SerializeField]
  private Sprite[] Octopus_Sprite;
  [SerializeField]
  private Sprite[] Bonus_Sprite;
  [SerializeField]
  private Sprite[] Wild_Sprite;
  [SerializeField]
  private Sprite[] Scatter_Sprite;
  [SerializeField]
  private Sprite[] FreeSpin_Sprite;
  [SerializeField]
  private Sprite[] Jackpot_Sprite;

  [Header("Miscellaneous UI")]
  [SerializeField]
  private TMP_Text Balance_text;
  [SerializeField]
  private TMP_Text TotalBet_text;
  [SerializeField]
  private TMP_Text Lines_text;
  [SerializeField]
  private TMP_Text TotalWin_text;
  [SerializeField]
  private TMP_Text BetPerLine_text;

  [SerializeField] private int maxReelItemCount = 18;

  [Header("Audio Management")]
  [SerializeField] private AudioController audioController;


  int tweenHeight = 0;  //calculate the height at which tweening is done

  [SerializeField]
  private GameObject Image_Prefab;    //icons prefab

  [SerializeField]
  private PayoutCalculation PayCalculator;

  private Tweener WinTween = null;
  private List<Tweener> alltweens = new List<Tweener>();


  [SerializeField]
  private List<ImageAnimation> TempList;//stores the sprites whose animation is running at present
                                        //
  [Header("parameters")]
  [SerializeField] private int IconSizeFactor = 100;       //set this parameter according to the size of the icon and spacing
  private int numberOfSlots = 5;          //number of columns
  [SerializeField] private int SpacingFactor = 0;
  [SerializeField] private int verticalVisibility = 3;

  [Header("scripts")]
  [SerializeField] private SocketIOManager SocketManager;
  [SerializeField] private UIManager uiManager;
  [SerializeField] private Bonus_Controller bonus_Controller;

  Coroutine AutoSpinRoutine = null;
  Coroutine tweenroutine;
  Coroutine FreeSpinRoutine = null;
  bool IsAutoSpin = false;
  [SerializeField] bool IsSpinning = false;
  internal bool IsHoldSpin = false;
  internal int BetCounter = 0;
  internal int linecounter = 20;
  internal bool CheckPopups = false;
  private double bet = 0;
  private double balance = 0;

  private Tween BalanceTween;

  [SerializeField] private bool IsFreeSpin = false;
  private double currentBalance = 0;
  private double currentTotalBet = 0;

  private int freeSpinsLeft = 0;


  private bool StopSpinToggle;
  private float SpinDelay = 0.2f;
  private bool IsTurboOn;
  internal bool WasAutoSpinOn;
  internal bool init = false;

  private void Start()
  {
    // IsAutoSpin = false;
    if (Lines_text != null)
    {
      Lines_text.text = "20";
    }

    if (BetPlus_Button) BetPlus_Button.onClick.RemoveAllListeners();
    if (BetPlus_Button) BetPlus_Button.onClick.AddListener(delegate { ChangeBet(true); });
    if (BetMinus_Button) BetMinus_Button.onClick.RemoveAllListeners();
    if (BetMinus_Button) BetMinus_Button.onClick.AddListener(delegate { ChangeBet(false); });

    if (LinePlus_Button) LinePlus_Button.onClick.RemoveAllListeners();
    if (LinePlus_Button) LinePlus_Button.onClick.AddListener(delegate { ChangeLine(true); });
    if (LineMinus_Button) LineMinus_Button.onClick.RemoveAllListeners();
    if (LineMinus_Button) LineMinus_Button.onClick.AddListener(delegate { ChangeLine(false); });

    if (MaxBet_Button) MaxBet_Button.onClick.RemoveAllListeners();
    if (MaxBet_Button) MaxBet_Button.onClick.AddListener(MaxBet);

    if (SlotStart_Button)
    {
      SlotStart_Button.onClick.RemoveAllListeners();
      SlotStart_Button.onClick.AddListener(StartSpin);
    }

    if (AutoSpinStop_Button) AutoSpinStop_Button.onClick.RemoveAllListeners();
    if (AutoSpinStop_Button) AutoSpinStop_Button.onClick.AddListener(StopAutoSpin);


    if (StopSpin_Button) StopSpin_Button.onClick.RemoveAllListeners();
    if (StopSpin_Button) StopSpin_Button.onClick.AddListener(() =>
    {
      StopSpinToggle = true; StopSpin_Button.gameObject.SetActive(false);
      audioController.PlayButtonAudio();
    });
    if (Turbo_Button) Turbo_Button.onClick.RemoveAllListeners();
    if (Turbo_Button) Turbo_Button.onClick.AddListener(() =>
    {
      TurboToggle();
      audioController.PlayButtonAudio();
    });

    tweenHeight = (myImages.Length * IconSizeFactor) - 280;

    Debug.Log("testing development environment");
  }

  internal void AutoSpin()
  {
    if (!IsAutoSpin)
    {

      IsAutoSpin = true;
      //  WasAutoSpinOn = false;
      if (AutoSpinStop_Button) AutoSpinStop_Button.gameObject.SetActive(true);
      //if (AutoSpin_Button) AutoSpin_Button.gameObject.SetActive(false);
      ToggleButtonGrp(false);
      if (AutoSpinRoutine != null)
      {
        StopCoroutine(AutoSpinRoutine);
        AutoSpinRoutine = null;
      }
      AutoSpinRoutine = StartCoroutine(AutoSpinCoroutine());

    }
  }

  internal void FreeSpin(int spins)
  {

    if (!IsFreeSpin)
    {

      IsFreeSpin = true;
      ToggleButtonGrp(false);

      if (FreeSpinRoutine != null)
      {
        StopCoroutine(FreeSpinRoutine);
        FreeSpinRoutine = null;
      }

      FreeSpinRoutine = StartCoroutine(FreeSpinCoroutine(spins));

    }
  }

  private IEnumerator FreeSpinCoroutine(int spinchances)
  {
    int i = 0;
    //   Debug.Log("entered in loop" + spinchances);
    while (i < spinchances)
    {

      i++;
      freeSpinsLeft--;
      uiManager.updateFreeSPinData(1 - ((float)i / (float)spinchances), spinchances - i);
      yield return new WaitForSeconds(0.2f);
      StartSlots(IsAutoSpin);
      yield return tweenroutine;
      yield return new WaitForSeconds(SpinDelay);

    }
    Debug.Log(" End free Spin :" + WasAutoSpinOn);

    freeSpinsLeft = 0;
    if (WasAutoSpinOn)
    {
      AutoSpin();
    }
    else
    {
      ToggleButtonGrp(true);
    }
    IsFreeSpin = false;
  }

  private void StopAutoSpin()
  {
    if (IsAutoSpin)
    {
      IsAutoSpin = false;
      if (AutoSpinStop_Button) AutoSpinStop_Button.gameObject.SetActive(false);
      //if (AutoSpin_Button) AutoSpin_Button.gameObject.SetActive(true);
      StartCoroutine(StopAutoSpinCoroutine());
    }

  }

  private IEnumerator AutoSpinCoroutine()
  {
    while (IsAutoSpin)
    {
      StartSlots(IsAutoSpin);
      yield return tweenroutine;
      yield return new WaitForSeconds(SpinDelay);

    }
    // WasAutoSpinOn = false;

  }

  private IEnumerator StopAutoSpinCoroutine()
  {
    yield return new WaitUntil(() => !IsSpinning);
    ToggleButtonGrp(true);
    //  WasAutoSpinOn = false;
    if (AutoSpinRoutine != null || tweenroutine != null)
    {
      StopCoroutine(AutoSpinRoutine);
      StopCoroutine(tweenroutine);
      tweenroutine = null;
      AutoSpinRoutine = null;
      StopCoroutine(StopAutoSpinCoroutine());
    }
  }

  internal void StartSpinRoutine()
  {
    if (!IsSpinning)
    {
      IsHoldSpin = false;
      Invoke("AutoSpinHold", 2f);
    }
  }

  internal void StopSpinRoutine()
  {
    CancelInvoke("AutoSpinHold");
    if (IsAutoSpin)
    {
      IsAutoSpin = false;
      if (AutoSpinStop_Button) AutoSpinStop_Button.gameObject.SetActive(false);
      //if (AutoSpin_Button) AutoSpin_Button.gameObject.SetActive(true);
      StartCoroutine(StopAutoSpinCoroutine());
      // WasAutoSpinOn = false;
    }
  }

  private void AutoSpinHold()
  {
    Debug.Log("Auto Spin Started");
    IsHoldSpin = true;
    AutoSpin();
  }



  private void StartSpin()
  {
    if (audioController) audioController.PlayButtonAudio("spin");

    StartSlots();
  }


  internal void FetchLines(string LineVal, int count)
  {
    y_string.Add(count + 1, LineVal);
    StaticLine_Texts[count].text = (count + 1).ToString();
    StaticLine_Objects[count].SetActive(true);
  }

  //Generate Static Lines from button hovers
  internal void GenerateStaticLine(TMP_Text LineID_Text)
  {

    int LineID = 1;
    try
    {
      LineID = int.Parse(LineID_Text.text);
    }
    catch (Exception e)
    {
      Debug.Log("Exception while parsing " + e.Message);
    }
    print("Line ID" + LineID);
    //List<int> x_points = null;
    List<int> y_points = null;
    y_points = y_string[LineID]?.Split(',')?.Select(Int32.Parse)?.ToList();
    //PayCalculator.GeneratePayoutLinesBackend(x_points, y_points, x_points.Count, true);
    PayCalculator.GeneratePayoutLinesBackend(y_points, y_points.Count, true);
  }

  //Destroy Static Lines from button hovers
  internal void DestroyStaticLine()
  {
    PayCalculator.ResetStaticLine();
  }

  internal double GetCurrentbetperLine()
  {

    return SocketManager.InitialData.bets[BetCounter];
  }

  private void MaxBet()
  {
    if (audioController) audioController.PlayButtonAudio();
    BetCounter = SocketManager.InitialData.bets.Count - 1;
    if (TotalBet_text) TotalBet_text.text = (SocketManager.InitialData.bets[BetCounter] * SocketManager.InitialData.lines.Count).ToString();
    if (BetPerLine_text) BetPerLine_text.text = SocketManager.InitialData.bets[BetCounter].ToString();
    currentTotalBet = SocketManager.InitialData.bets[BetCounter] * SocketManager.InitialData.lines.Count;
    // CompareBalance();
    uiManager.InitialiseUIData(SocketManager.UIData.paylines);

  }

  internal void ChangeLine(bool IncDec)
  {

    if (audioController)
      audioController.PlayButtonAudio();



    PayCalculator.ResetLines();
    if (IncDec)
    {
      linecounter++;
    }
    else
    {
      linecounter--;
    }

    if (linecounter < 1)
    {
      linecounter = 1;

    }
    if (linecounter > 20)
    {
      linecounter = 20;
    }


    foreach (Button sb in StaticLine_Buttons)
    {
      sb.interactable = false;
    }

    foreach (ManageLineButtons sb in StaticLine_Scripts)
    {
      sb.isActive = false;
    }

    for (int i = 1; i <= linecounter; i++)
    {
      Debug.Log("run this code" + linecounter);
      Lines_text.text = i.ToString();
      StaticLine_Buttons[i - 1].interactable = true;
      StaticLine_Scripts[i - 1].isActive = true;
      GenerateStaticLine(Lines_text);
    }
  }


  void TurboToggle()
  {
    if (IsTurboOn)
    {
      IsTurboOn = false;
      Turbo_Button.GetComponent<ImageAnimation>().StopAnimation();
      Turbo_Button.image.sprite = TurboToggleSprite;
      // Turbo_Button.image.color = new Color(0.86f, 0.86f, 0.86f, 1);
    }
    else
    {
      IsTurboOn = true;
      Turbo_Button.GetComponent<ImageAnimation>().StartAnimation();
      // Turbo_Button.image.color = new Color(1, 1, 1, 1);
    }
  }

  private void ChangeBet(bool IncDec)
  {
    if (audioController) audioController.PlayButtonAudio();

    if (IncDec)
    {
      if (BetCounter < SocketManager.InitialData.bets.Count - 1)
      {
        BetCounter++;
      }
      else
      {
        BetCounter = 0;
      }
    }
    else
    {
      if (BetCounter > 0)
      {
        BetCounter--;
        // CheckBetCounter();
      }
      else
      {
        BetCounter = SocketManager.InitialData.bets.Count - 1;
      }

    }
    // if (BetPerLine_text) BetPerLine_text.text = SocketManager.InitialData.bets[BetCounter].ToString();
    if (TotalBet_text) TotalBet_text.text = (SocketManager.InitialData.bets[BetCounter] * SocketManager.InitialData.lines.Count).ToString();
    currentTotalBet = SocketManager.InitialData.bets[BetCounter] * SocketManager.InitialData.lines.Count;

    // CompareBalance();
    uiManager.InitialiseUIData(SocketManager.UIData.paylines);


  }

  internal void SetInitialUI()
  {
    BetCounter = 0;
    if (TotalBet_text) TotalBet_text.text = ((SocketManager.InitialData.bets[BetCounter] * SocketManager.InitialData.lines.Count)).ToString();
    if (BetPerLine_text) BetPerLine_text.text = SocketManager.InitialData.bets[BetCounter].ToString();
    if (Lines_text) Lines_text.text = SocketManager.InitialData.lines.Count.ToString();
    if (TotalWin_text) TotalWin_text.text = "0.00";
    if (Balance_text) Balance_text.text = SocketManager.PlayerData.balance.ToString("f2");
    uiManager.InitialiseUIData(SocketManager.UIData.paylines);
    currentBalance = SocketManager.PlayerData.balance;
    currentTotalBet = SocketManager.InitialData.bets[BetCounter] * SocketManager.InitialData.lines.Count;
    CompareBalance();
    init = true;
  }

  //function to populate animation sprites accordingly
  private void PopulateAnimationSprites(ImageAnimation animScript, int val)
  {
    animScript.textureArray.Clear();
    animScript.textureArray.TrimExcess();
    switch (val)
    {
      case 0:
        for (int i = 0; i < Nine_Sprite.Length; i++)
        {
          animScript.textureArray.Add(Nine_Sprite[i]);
        }

        break;
      case 1:
        for (int i = 0; i < Ten_Sprite.Length; i++)
        {
          animScript.textureArray.Add(Ten_Sprite[i]);
        }

        break;
      case 3:
        for (int i = 0; i < J_Sprite.Length; i++)
        {
          animScript.textureArray.Add(J_Sprite[i]);
        }

        break;
      case 4:
        for (int i = 0; i < K_Sprite.Length; i++)
        {
          animScript.textureArray.Add(K_Sprite[i]);
        }
        break;
      case 5:
        for (int i = 0; i < Q_Sprite.Length; i++)
        {
          animScript.textureArray.Add(Q_Sprite[i]);
        }
        break;
      case 2:
        for (int i = 0; i < A_Sprite.Length; i++)
        {
          animScript.textureArray.Add(A_Sprite[i]);
        }
        break;
      case 6:
        for (int i = 0; i < Hedgehog_Sprite.Length; i++)
        {
          animScript.textureArray.Add(Hedgehog_Sprite[i]);
        }
        break;
      case 7:
        for (int i = 0; i < Crab_Sprite.Length; i++)
        {
          animScript.textureArray.Add(Crab_Sprite[i]);
        }
        break;
      case 8:
        for (int i = 0; i < JellyFish_Sprite.Length; i++)
        {
          animScript.textureArray.Add(JellyFish_Sprite[i]);
        }
        break;
      case 9:
        for (int i = 0; i < Turtle_Sprite.Length; i++)
        {
          animScript.textureArray.Add(Turtle_Sprite[i]);
        }
        break;
      case 10:
        for (int i = 0; i < Shell_Sprite.Length; i++)
        {
          animScript.textureArray.Add(Shell_Sprite[i]);
        }
        break;
      case 11:
        for (int i = 0; i < Octopus_Sprite.Length; i++)
        {
          animScript.textureArray.Add(Octopus_Sprite[i]);
        }
        break;
      case 12:
        for (int i = 0; i < Bonus_Sprite.Length; i++)
        {
          animScript.textureArray.Add(Bonus_Sprite[i]);
        }
        break;
      case 13:
        for (int i = 0; i < Wild_Sprite.Length; i++)
        {
          animScript.textureArray.Add(Wild_Sprite[i]);
        }
        break;
      case 14:
        for (int i = 0; i < Scatter_Sprite.Length; i++)
        {
          animScript.textureArray.Add(Scatter_Sprite[i]);
        }
        break;
      case 15:
        for (int i = 0; i < FreeSpin_Sprite.Length; i++)
        {
          animScript.textureArray.Add(FreeSpin_Sprite[i]);
        }

        break;
      case 16:
        for (int i = 0; i < Jackpot_Sprite.Length; i++)
        {
          animScript.textureArray.Add(Jackpot_Sprite[i]);
        }

        break;
    }
  }

  //starts the spin process
  private void StartSlots(bool autoSpin = false)
  {

    if (!autoSpin)
    {
      if (AutoSpinRoutine != null)
      {
        StopCoroutine(AutoSpinRoutine);
        StopCoroutine(tweenroutine);
        tweenroutine = null;
        AutoSpinRoutine = null;
      }
    }

    WinningsAnim(false);

    if (TempList.Count > 0)
    {
      StopGameAnimation();
    }
    PayCalculator.ResetLines();
    tweenroutine = StartCoroutine(TweenRoutine());
  }
  private void OnApplicationFocus(bool focus)
  {
    if (focus)
    {
      if (!IsSpinning)
      {
        if (audioController) audioController.StopWLAaudio();
      }
    }
  }
  private IEnumerator TweenRoutine()
  {
    if (currentBalance < currentTotalBet && !IsFreeSpin)
    {
      CompareBalance();
      if (IsAutoSpin)
      {
        StopAutoSpin();
        yield return new WaitForSeconds(1f);
      }
      ToggleButtonGrp(true);
      yield break;
    }
    audioController.StopWLAaudio();
    audioController.PlaySpinBonusAudio();
    TotalWin_text.text = "0.00";


    IsSpinning = true;
    ToggleButtonGrp(false);

    if (!IsTurboOn && !IsFreeSpin && !IsAutoSpin)
    {
      StopSpin_Button.gameObject.SetActive(true);
    }
    for (int i = 0; i < numberOfSlots; i++)
    {
      InitializeTweening(Slot_Transform[i]);
      yield return new WaitForSeconds(0.1f);
    }
    bet = 0;
    balance = 0;
    if (!IsFreeSpin)
    {

      try
      {
        bet = double.Parse(TotalBet_text.text);
      }
      catch (Exception e)
      {
        Debug.Log("Error while conversion " + e.Message);
      }

      try
      {
        balance = double.Parse(Balance_text.text);
      }
      catch (Exception e)
      {
        Debug.Log("Error while conversion " + e.Message);
      }
      double initAmount = balance;
      balance = balance - (bet);

      BalanceTween = DOTween.To(() => initAmount, (val) => initAmount = val, balance, 0.8f).OnUpdate(() =>
      {
        if (Balance_text) Balance_text.text = initAmount.ToString("f3");
      });

    }

    SocketManager.AccumulateResult(BetCounter);

    yield return new WaitUntil(() => SocketManager.isResultdone);

    for (int i = 0; i < 3; i++)
    {
      for (int j = 0; j < 5; j++)
      {
        int resultNum = int.Parse(SocketManager.ResultData.matrix[i][j]);
        //print("resultNum: " + resultNum);
        //print("image loc: " + j + " " + i);
        PopulateAnimationSprites(Tempimages[j].slotImages[i].GetComponent<ImageAnimation>(), resultNum);
        Tempimages[j].slotImages[i].sprite = myImages[resultNum];
      }
    }
    CheckForFeaturesAnimation();
    if (IsTurboOn || IsFreeSpin)
    {
      yield return new WaitForSeconds(0.1f);
    }
    else
    {
      for (int i = 0; i < 5; i++)
      {
        yield return new WaitForSeconds(0.1f);
        if (StopSpinToggle)
        {
          break;
        }
      }
      StopSpin_Button.gameObject.SetActive(false);
    }

    // yield return new WaitForSeconds(0.5f);

    for (int i = 0; i < numberOfSlots; i++)
    {
      yield return StopTweening(5, Slot_Transform[i], i, StopSpinToggle);
    }
    StopSpinToggle = false;
    yield return alltweens[^1].WaitForCompletion();
    KillAllTweens();

    yield return new WaitForSeconds(0.1f);
    if (SocketManager.ResultData.payload.winAmount > 0)
    {
      SpinDelay = 0.5f;
    }
    else
    {
      SpinDelay = 0.2f;
    }

    if (audioController) audioController.StopApinBonusAudio();

    //  yield return new WaitForSeconds(0.5f);

    if (SocketManager.ResultData.payload.winAmount > 0)
    {
      List<int> winLine = new();
      foreach (var item in SocketManager.ResultData.payload.wins)
      {
        winLine.Add(item.line);
      }
      CheckPayoutLineBackend(winLine);
      //  if (m_Gamble_Button) m_Gamble_Button.interactable = true;
    }


    currentBalance = SocketManager.PlayerData.balance;

    if (audioController) audioController.StopWLAaudio();
    if (SocketManager.ResultData.bonus.istriggered)
    {
      bonus_Controller.StartBonusGame(SocketManager.ResultData.bonus.result);
      yield return new WaitUntil(() => bonus_Controller.isfinished);
      yield return new WaitForSeconds(1f);
      bonus_Controller.FinishBonusGame();
      SocketManager.ResultData.payload.winAmount = SocketManager.bonusData.payload.winAmount;
      SocketManager.PlayerData = SocketManager.bonusData.player;
    }

    CheckPopups = true;
    if (SocketManager.ResultData.jackpot.isTriggered)
    {
      uiManager.PopulateWin(4, SocketManager.ResultData.jackpot.amount);
    }
    else
    {
      CheckWinPopups();
    }
    yield return new WaitUntil(() => !CheckPopups);

    if (audioController) audioController.StopWLAaudio();
    if (TotalWin_text) TotalWin_text.text = SocketManager.ResultData.payload.winAmount.ToString("F3");

    BalanceTween?.Kill();
    if (Balance_text) Balance_text.text = SocketManager.PlayerData.balance.ToString("f3");

    if (SocketManager.ResultData.payload.winAmount > 0)
      WinningsAnim(true);

    if (SocketManager.ResultData.freeSpin.isFreeSpin)
    {

      if (IsFreeSpin)
      {
        IsFreeSpin = false;
        if (FreeSpinRoutine != null)
        {
          StopCoroutine(FreeSpinRoutine);
          FreeSpinRoutine = null;
        }
        int extraFreeSpin = SocketManager.ResultData.freeSpin.count - freeSpinsLeft;
        freeSpinsLeft = SocketManager.ResultData.freeSpin.count;
        uiManager.FreeSpinPopUP(SocketManager.ResultData.freeSpin.count, extraFreeSpin);
      }
      else
      {
        freeSpinsLeft = (int)SocketManager.ResultData.freeSpin.count;
        uiManager.FreeSpinPopUP((int)SocketManager.ResultData.freeSpin.count);
      }
      yield return new WaitForSeconds(1.2f);
      uiManager.CloseFreeSpinPopup();
      FreeSpin((int)SocketManager.ResultData.freeSpin.count);



      if (IsAutoSpin)
      {
        StopAutoSpin();
        yield return new WaitForSeconds(0.1f);
        WasAutoSpinOn = true;
        Debug.Log("is free spin : " + WasAutoSpinOn);

      }

    }



    if (!IsAutoSpin && !IsFreeSpin)
    {
      ToggleButtonGrp(true);
      IsSpinning = false;
    }
    else
    {
      // yield return new WaitForSeconds(2f);
      IsSpinning = false;
    }
  }

  internal void CallCloseSocket()
  {
    StartCoroutine(SocketManager.CloseSocket());
  }

  private void CompareBalance()
  {
    if (currentBalance < currentTotalBet)
    {
      uiManager.LowBalPopup();

    }

  }
  internal void CheckWinPopups()
  {
    if (SocketManager.ResultData.payload.winAmount >= currentTotalBet * 5 && SocketManager.ResultData.payload.winAmount < currentTotalBet * 10)
    {
      uiManager.PopulateWin(1, SocketManager.ResultData.payload.winAmount);
    }
    else if (SocketManager.ResultData.payload.winAmount >= currentTotalBet * 10 && SocketManager.ResultData.payload.winAmount < currentTotalBet * 15)
    {
      uiManager.PopulateWin(2, SocketManager.ResultData.payload.winAmount);
    }
    else if (SocketManager.ResultData.payload.winAmount >= currentTotalBet * 15)
    {
      uiManager.PopulateWin(3, SocketManager.ResultData.payload.winAmount);
    }
    else
    {
      CheckPopups = false;
    }
  }

  void ToggleButtonGrp(bool toggle)
  {

    if (SlotStart_Button) SlotStart_Button.interactable = toggle;
    if (MaxBet_Button) MaxBet_Button.interactable = toggle;
    if (AutoSpin_Button) AutoSpin_Button.interactable = toggle;
    if (LinePlus_Button) LinePlus_Button.interactable = toggle;
    if (LineMinus_Button) LineMinus_Button.interactable = toggle;
    if (BetMinus_Button) BetMinus_Button.interactable = toggle;
    if (BetPlus_Button) BetPlus_Button.interactable = toggle;

  }

  //start the icons animation
  private void StartGameAnimation(GameObject animObjects)
  {

    ImageAnimation temp = animObjects.GetComponent<ImageAnimation>();
    temp.StartAnimation();
    TempList.Add(temp);

  }

  //stop the icons animation
  private void StopGameAnimation()
  {
    for (int i = 0; i < TempList.Count; i++)
    {
      TempList[i].StopAnimation();
    }
    TempList.Clear();
    TempList.TrimExcess();
  }

  //Win Animation When A Line Is Matched
  private void WinningsAnim(bool IsStart)
  {
    if (IsStart)
    {
      WinTween = TotalWin_text.transform.DOScale(new Vector2(1.5f, 1.5f), 1f).SetLoops(-1, LoopType.Yoyo).SetDelay(0);
    }
    else
    {
      WinTween.Kill();
      TotalWin_text.transform.localScale = Vector3.one;
    }
  }

  internal void shuffleInitialMatrix()
  {
    for (int i = 0; i < Tempimages.Count; i++)
    {
      for (int j = 0; j < 3; j++)
      {
        int randomIndex = UnityEngine.Random.Range(0, myImages.Length);
        Tempimages[i].slotImages[j].sprite = myImages[randomIndex];
      }
    }
  }
  internal void updateBalance()
  {

    if (Balance_text) Balance_text.text = SocketManager.PlayerData.balance.ToString("f3");
    if (TotalWin_text) TotalWin_text.text = SocketManager.ResultData.payload.winAmount.ToString("f3");
  }
  //generate the payout lines generated
  private void CheckForFeaturesAnimation()
  {
    bool playJackpot = false;
    bool playScatter = false;
    bool playBonus = false;
    bool playFreespin = false;
    if (SocketManager.ResultData.jackpot.amount > 0)
    {
      playJackpot = true;
    }
    if (SocketManager.ResultData.scatter.amount > 0)
    {
      playScatter = true;
    }
    if (SocketManager.ResultData.bonus.istriggered)
    {
      playBonus = true;
    }
    if (SocketManager.ResultData.freeSpin.isFreeSpin)
    {
      playFreespin = true;
    }
    PlayFeatureAnimation(playJackpot, playScatter, playBonus, playFreespin);
  }
  private void PlayFeatureAnimation(bool jackpot = false, bool scatter = false, bool bonus = false, bool freeSpin = false)
  {
    for (int i = 0; i < SocketManager.ResultData.matrix.Count; i++)
    {
      for (int j = 0; j < SocketManager.ResultData.matrix[i].Count; j++)
      {

        if (int.TryParse(SocketManager.ResultData.matrix[i][j], out int parsedNumber))
        {
          if (jackpot && parsedNumber == 16)
          {
            StartGameAnimation(Tempimages[j].slotImages[i].gameObject);
          }
          if (scatter && parsedNumber == 14)
          {
            StartGameAnimation(Tempimages[j].slotImages[i].gameObject);
          }
          if (bonus && parsedNumber == 12)
          {
            StartGameAnimation(Tempimages[j].slotImages[i].gameObject);
          }
          if (freeSpin && parsedNumber == 15)
          {
            StartGameAnimation(Tempimages[j].slotImages[i].gameObject);
          }
        }

      }
    }
  }
  private void CheckPayoutLineBackend(List<int> LineId, double jackpot = 0)
  {
    List<int> y_points = null;
    if (LineId.Count > 0)
    {
      if (jackpot <= 0)
      {
        if (audioController) audioController.PlayWLAudio("win");
      }
      // PayCalculator.GeneratePayoutLinesBackend(LineId,);
      for (int i = 0; i < LineId.Count; i++)
      {
        y_points = SocketManager.InitialData.lines[LineId[i]];
        PayCalculator.GeneratePayoutLinesBackend(y_points, y_points.Count);
      }

      if (jackpot > 0)
      {
        if (audioController) audioController.PlayWLAudio("megaWin");
        for (int i = 0; i < Tempimages.Count; i++)
        {
          for (int k = 0; k < Tempimages[i].slotImages.Count; k++)
          {
            StartGameAnimation(Tempimages[i].slotImages[k].gameObject);
          }
        }
      }
      else
      {
        List<KeyValuePair<int, int>> coords = new();
        for (int j = 0; j < LineId.Count; j++)
        {
          for (int k = 0; k < SocketManager.ResultData.payload.wins[j].positions.Count; k++)
          {
            int rowIndex = SocketManager.InitialData.lines[LineId[j]][k];
            int columnIndex = k;
            coords.Add(new KeyValuePair<int, int>(rowIndex, columnIndex));
          }
        }

        foreach (var coord in coords)
        {
          int rowIndex = coord.Key;
          int columnIndex = coord.Value;
          StartGameAnimation(Tempimages[columnIndex].slotImages[rowIndex].gameObject);
        }
      }
      WinningsAnim(true);
    }
    else
    {

      //if (audioController) audioController.PlayWLAudio("lose");
      if (audioController) audioController.StopWLAaudio();
    }
    //  CheckSpinAudio = false;
  }

  private void GenerateMatrix(int value)
  {
    for (int j = 0; j < 3; j++)
    {
      Tempimages[value].slotImages.Add(images[value].slotImages[images[value].slotImages.Count - 5 + j]);
    }
  }

  #region TweeningCode
  private void InitializeTweening(Transform slotTransform)
  {
    slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, 0);
    Tweener tweener = slotTransform.DOLocalMoveY(-tweenHeight, 0.2f).SetLoops(-1, LoopType.Restart).SetDelay(0);
    tweener.Play();
    alltweens.Add(tweener);
  }



  private IEnumerator StopTweening(int reqpos, Transform slotTransform, int index, bool isStop)
  {
    alltweens[index].Pause();
    slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, 0);
    int tweenpos = (reqpos * (IconSizeFactor + SpacingFactor)) - (IconSizeFactor + (2 * SpacingFactor));
    alltweens[index] = slotTransform.DOLocalMoveY(-tweenpos + 100 + (SpacingFactor > 0 ? SpacingFactor / 4 : 0), 0.5f).SetEase(Ease.OutElastic);
    if (!isStop)
    {
      yield return new WaitForSeconds(0.2f);
    }
    else
    {
      yield return null;
    }
  }


  private void KillAllTweens()
  {
    for (int i = 0; i < numberOfSlots; i++)
    {
      alltweens[i].Kill();
    }
    alltweens.Clear();

  }
  #endregion

}

[Serializable]
public class SlotImage
{
  public List<Image> slotImages = new List<Image>(10);
}

