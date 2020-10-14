using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject GameCam;
    public Player player;
    public Text playerCoinTxt;
    public Image[] lifeImage;

    public int totalItemCount;

    void Update()
    {
        playerCoinTxt.text = string.Format("{0:n0}", player.coin);
    }

}
