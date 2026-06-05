using System.Collections.Generic;
using UnityEngine;

public class RelicPanel : MonoBehaviour
{
    public RelicView relicViewPrefab;
    public List<RelicView> relicViews;

    public void Refresh(List<Relic> relics, GameManager gameManager)
    {
        foreach (var relicView in relicViews)
        {
            relicView.gameObject.SetActive(false);
        }

        for (int i = 0; i < relics.Count; i++)
        {
            if (i >= relicViews.Count)
            {
                relicViews.Add(Instantiate(relicViewPrefab, transform));
            }

            relicViews[i].Refresh(relics[i], gameManager);
            relicViews[i].gameObject.SetActive(true);
        }
    }
}
