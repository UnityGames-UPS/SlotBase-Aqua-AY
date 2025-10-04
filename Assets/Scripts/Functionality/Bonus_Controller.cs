using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Bonus_Controller : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Button[] chest;
    [SerializeField] private ImageAnimation[] chestAnim;
    [SerializeField] private TMP_Text[] reward_text;
    private List<double> resultData = new List<double>();
    [SerializeField] private GameObject bonusObject;

    internal bool isfinished = false;
    private bool opening = false;
    internal bool waitForBonusResult = true;
    [SerializeField] private AudioController audioController;
    [SerializeField] private SlotBehaviour slotBehaviour;
    [SerializeField] private SocketIOManager socketManager;

    [SerializeField] private GameObject WinPopUp;
    [SerializeField] private TMP_Text WinPopUpText;

    private double TotalWinAmount;

    private void Start()
    {
        for (int i = 0; i < chest.Length; i++)
        {
            int index = i;
            chest[i].onClick.RemoveAllListeners();
            chest[i].onClick.AddListener(delegate { OnChestOpen(index); });
        }
    }

    internal void StartBonusGame(List<double> result)
    {
        Debug.Log("___Bonus Started");
        // for (int i = 0; i < result.Count; i++)
        // {
        //     resultData.Add(result[i]);
        // }

        audioController.StopBgAudio();
        audioController.StopWLAaudio();
        audioController.playBgAudio("bonus");
        Debug.Log("___Bonus Started");

        bonusObject.SetActive(true);
        TotalWinAmount = 0;
        Debug.Log("___Bonus Started");
    }

    internal void FinishBonusGame()
    {
        opening = false;
        isfinished = false;
        // resultData.Clear();
        WinPopUpText.text = "";
        bonusObject.SetActive(false);
        WinPopUp.SetActive(false);

        audioController.playBgAudio("normal");
        foreach (Button item in chest)
        {
            item.interactable = true;
        }
    }

    void OnChestOpen(int index)
    {
        if (isfinished) return;
        if (opening) return;
        audioController.PlayButtonAudio();

        waitForBonusResult = true;
        socketManager.OnBonusCollect(index);
        StartCoroutine(chestOpenRoutine(index));
    }

    IEnumerator chestOpenRoutine(int index)
    {
        audioController.PlaySpinBonusAudio("bonus");
        opening = true;
        chest[index].interactable = false;
        bool gameFinishied = false;
        Tween tween = chestAnim[index].transform.DOShakePosition(1f, new Vector3(15, 0, 0), 30, 90, true).SetLoops(-1, LoopType.Incremental);
        yield return new WaitUntil(() => !waitForBonusResult);
        tween.Kill();
        audioController.StopApinBonusAudio();
        chestAnim[index].StartAnimation();

        if (socketManager.bonusData.payload.payout > 0)
        {
            audioController.PlayWLAudio("bonuswin");
            reward_text[index].text = "+ " + socketManager.bonusData.payload.winAmount.ToString("F3");
            TotalWinAmount += socketManager.bonusData.payload.winAmount;
        }
        else
        {
            audioController.PlayWLAudio("bonuslose");
            reward_text[index].text = "Game Over";
            gameFinishied = true;
        }

        reward_text[index].transform.localScale = Vector3.zero;
        reward_text[index].gameObject.SetActive(true);
        reward_text[index].transform.DOScale(1, 0.8f);
        reward_text[index].transform.DOLocalMoveY(235, 0.8f);
        yield return new WaitForSeconds(0.8f);
        reward_text[index].gameObject.SetActive(false);
        reward_text[index].transform.localPosition = new Vector3(-50, -42);

        audioController.StopWLAaudio();
        if (gameFinishied)
        {
            WinPopUp.transform.localScale = Vector3.zero;
            WinPopUpText.text = TotalWinAmount.ToString("F3");
            WinPopUp.SetActive(true);
            WinPopUp.transform.DOScale(Vector3.one, 0.8f);
            yield return new WaitForSeconds(1);
            isfinished = true;
        }
        opening = false;
    }
}
