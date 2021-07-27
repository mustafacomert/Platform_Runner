using UnityEngine;
using TMPro;
public class RankingBoardController : MonoBehaviour
{
    private TextMeshProUGUI txt;
    private void Awake()
    {
        txt = GetComponent<TextMeshProUGUI>();
        txt.text = "asd";
    }
}
