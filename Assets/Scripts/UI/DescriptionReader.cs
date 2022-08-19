using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DescriptionReader : MonoBehaviour
{
    [SerializeField] Description description;
    [SerializeField] List<TMP_Text> displays = new();

    public void DisplayDescription() =>
        HandleDescription(true);

    public void ConcealDescription()=>
        HandleDescription(false);

    void HandleDescription(bool display)
    {
        print(display);
        for (int i = 0; i < displays.Count; i++)
        {
            print(i);
            if (i < description.descriptions.Count)
                displays[i].text = display ? description.descriptions[i] : "";
            else
                break;
        }
    }
}
