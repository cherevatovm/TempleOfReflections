using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialog : MonoBehaviour
{
    public List<GameObject> Character = new List<GameObject>();

    public text Textout;

    private int IndexCharacter = 0;

    private int IndexText = 0;

    private DialogCharacter dialogCharacter;
    // Start is called before the first frame update
    void Start()
    {
        WriteDialog();
        IndexCharacter++;
    }

    private void WriteDialog()
    {
        if (Character.Count > IndexCharacter)
        {
            if (Character[IndexCharacter].GetComponent<DialogCharacter>() != null)
            {
                dialogCharacter = Character[IndexCharacter].GetComponent<DialogCharacter>();
                if (IndexText >= dialogCharacter.GetReadCount())
                {
                    TextDialog("Диалог закончен");
                }
                else
                {
                    string text = dialogCharacter.GetReadStringIndex(IndexText);
                    TextDialog(text);
                }
            }
        }
        else TextDialog("Диалог закончен");
    }

    private void TextDialog(string text)
    {
        Textout.text = text;
    }

    private void NextIndex()
    {
        for (int i = IndexCharacter; i < Character.Count; i++)
        {
            WriteDialog();
            IndexCharacter++;
            return;
        }

        if (IndexCharacter >= Character.Count)
        {
            IndexCharacter = 0;
            IndexText++;
            WriteDialog();
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NextIndex();
            IndexCharacter++;
        }
    }
}
