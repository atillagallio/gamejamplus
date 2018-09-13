using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class MatchManager : MonoBehaviour
{
  public bool GameStarted
  {
    get; private set;
  }

  public bool GameFinished
  {
    get; private set;
  }

  public float Countdown { get; private set; }
  public float MatchDuration { get; private set; }

  private List<PlayerBehaviour> players => InGameManager.Instance.players;

  private bool startedCountdown;
  public void StartMatch()
  {
    if (!startedCountdown)
    {
      StartCoroutine(MatchCountDown());
      startedCountdown = true;
    }
  }

  private IEnumerator MatchCountDown()
  {
    Countdown = 3.0f;
    while (Countdown > 0)
    {
      yield return null;
      Countdown -= Time.deltaTime;
    }
    GameStarted = true;

    StartCoroutine(MatchTimer());
  }

  private IEnumerator MatchTimer()
  {
    GameEndCondition endCondition = GameDataManager.Data.EndCondition;
    Debug.Log("Match Start");
    MatchDuration = 0;

    Func<bool> TestForDuration = () => MatchDuration > endCondition.MatchDuration;
    Func<bool> TestForScore = () => players.Aggregate(false, (test, p) => test || p.GetComponent<PlayerBehaviour>().points >= endCondition.ScoreToWin);
    Func<bool> GameOver = () => true;
    switch (endCondition.Mode)
    {
      case GameEndConditionMode.score:
        GameOver = TestForScore;
        break;
      case GameEndConditionMode.time:
        GameOver = TestForDuration;
        break;
      case GameEndConditionMode.both:
        GameOver = () => TestForDuration() || TestForScore();
        break;
      default:
        print("WEIRD CASE!!!");
        break;
    }

    while (!GameOver())
    {
      yield return null;
      MatchDuration += Time.deltaTime;
      GameUIManager.Instance.UpdateTimer((int)MatchDuration);
    }

    GameFinished = true;
    string txt = "";
    foreach (var player in players)
    {
      txt += player.name + " -> " + player.gameUiPosition + " -> " + player.points;
      Debug.Log(txt);
    }
    //endGameCanvas.SetActive(true);

    foreach (var player in players)
    {
      EndGame.Instance.playerList.Add(player);
    }

    EndGame.Instance.FindWinner();
    UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
    //endGameCanvas.GetComponentInChildren<TextMeshProUGUI>().text = txt;
  }

}