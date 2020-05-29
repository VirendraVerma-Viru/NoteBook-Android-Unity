using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.EventSystems;
//using UnityEngine.Networking;

public class AppController : MonoBehaviour
{
    //----------------------Pannels Gameobject-------------
    [Header("Pannel GameObjects")]
    public GameObject InspirationalSpeechPannel;
    public GameObject CreateAccountPannel;
    public GameObject LoginPannel;
    public GameObject NotebookPannel;
    public GameObject EdditorPannel;

    //-------------In Edditor Pannel;
    public GameObject ReminderPopUpPannelInEdditor;
    

    // Start is called before the first frame update
    void Start()
    {
        ReminderPopUpPannelInEdditor.SetActive(false);       

        ActivatePanel(InspirationalSpeechPannel.name);
        
    }

    /*
    #region create account area

    [Header("CreateAccount Password")]
    public InputField NewPasswordInputField;

    public void CreateButtonPressed()
    {
        string password = NewPasswordInputField.text;
        saveload.password = password;
        saveload.Save();
        ActivatePanel(NotebookPannel.name);
        NoteBookContent();
    }

    #endregion
     * 
     * 

    #region login account area

    [Header("Login Account Area")]
    public InputField LoginPasswordInputfield;
    public Text StatusTextLogin;

    public void LoginButtonPressed()
    {
        string enteredpassword = LoginPasswordInputfield.text;
        if (enteredpassword == saveload.password)
        {
            ActivatePanel(NotebookPannel.name);
            NoteBookContent();
        }
        else
        {
            StatusTextLogin.text = "Password not match";
            StatusTextLogin.color = Color.red;
        }
    }


    #endregion
    */

    #region edditor region

    [Header("Edditor Variables")]
    public InputField NoteWrittenInnputField;

    public InputField TitleInputField;
    public InputField DescriptionInfputField;

    public GameObject ReminderSetPopUpNortification;

    private bool newNote;

    void EmptyNewInputfield()
    {
        NoteWrittenInnputField.text = "";
    }
    
    public void OnReminderButtonClick()
    {
        ReminderPopUpPannelInEdditor.SetActive(true);

    }
    public void OnCloseReminderButtonClick()
    {
        ReminderPopUpPannelInEdditor.SetActive(false);
    }

    public Text printText;
    public void SetReminderButtonClick()
    {
        string title = TitleInputField.text;
        string des = DescriptionInfputField.text;

        string temp;
        //get the title and description 

        //calculate date difference
        string selectedDate = gameObject.GetComponent < FantomLib.DatePickerController >().selecteddate;
        string selectedTime = gameObject.GetComponent<FantomLib.TimePickerController>().selectedtime;
        //printText.text = selectedDate + "||" + selectedTime;

        string mergeDateTime = selectedDate + " " + selectedTime+":00";
        //printText.text = mergeDateTime.ToString();
        System.DateTime dateTime = System.DateTime.Parse(mergeDateTime);
        //printText.text = dateTime.ToString();
        DateTime theTime = System.DateTime.Now;

        temp = dateTime + "," + theTime;

        double travelTime = (dateTime - theTime).TotalSeconds;

        temp += "|" + travelTime.ToString();
        printText.text = temp.ToString();

        gameObject.GetComponent<NortificationManager>().CreateNortificationChannel();
        gameObject.GetComponent<NortificationManager>().SendNortification(title, des, travelTime);
        ReminderPopUpPannelInEdditor.SetActive(false);
        ReminderSetPopUpNortification.SetActive(true);
        StartCoroutine(ReminderSetPopUpNortificationFu());
    }

    IEnumerator ReminderSetPopUpNortificationFu()
    {
        yield return new WaitForSeconds(2);
        ReminderSetPopUpNortification.SetActive(false);
    }

    private int EditID;
    public void OnSaveButtonPressed()
    {
         string notewritten = NoteWrittenInnputField.text;
        if (newNote)
        {
           
            //print(NoteWrittenInnputField.text);
            int idofnewnote = notedata.Count + 1;
            notedata.Add(new NoteData(notewritten, idofnewnote));
            
        }
        else
        {
            foreach (NoteData m in notedata)
            {
                if (EditID == m.ID)
                {
                    m.TextData = notewritten;
                }

            }
        }
        ActivatePanel(NotebookPannel.name);
        SaveNotes();
        NoteBookContent();

    }


    #endregion

    #region notebook region

    [Header("Notes List on Notebook Area")]
    public Text StatusTextNotebook;
    public GameObject NotePrefab;
    public Transform NoteContentArea;

    public Text FileNameTitle;

    List<NoteData> notedata = new List<NoteData>();
    private string textdata;
    private int id;

    public void NoteBookContent()
    {
        saveload.Load();
       
  
        FileNameTitle.text = saveload.current_filenameCallingName;
        GameObject[] goo = GameObject.FindGameObjectsWithTag("Note");
        ConfirmationDeletePannel.SetActive(false);

        foreach (GameObject g in goo)
        {
            Destroy(g);
        }
        LoadNotes();

        if (notedata.Count > 0)
        {
            StatusTextNotebook.text = " ";

            

            
            foreach (NoteData note in notedata)
            {
                GameObject go = Instantiate(NotePrefab);
                go.transform.SetParent(NoteContentArea);
                go.transform.localScale = Vector3.one;
                go.transform.Find("DeleteButton").name = "Del:"+note.TextData;
                go.transform.Find("EditButton").name = "Edit:"+note.TextData;
                go.GetComponent<Text>().text = note.TextData;

                
                int noumberOfLines=Mathf.RoundToInt(note.TextData.Length/40);

                print(note.TextData);
                //print(noumberOfLines);

                

                int i = 13;
                char c = Convert.ToChar(i);
                string[] item = note.TextData.Split(c);
                print("Enter Button "+item.Length);
                
                if (note.TextData.Contains(c.ToString()))
                    print("e");
                else
                    print("f");

                if (noumberOfLines > 1)
                {
                    go.GetComponent<RectTransform>().sizeDelta = new Vector2(1080,(51 * noumberOfLines));
                    
                }
            }
        }
        else
        {
            StatusTextNotebook.text = "No Note Found";
            StatusTextNotebook.color = Color.blue;
        }
    }


    public void OnContentChangeInContainer()
    {
        StartCoroutine(FethContainerOrder());
    }

    IEnumerator FethContainerOrder()
    {
        yield return new WaitForSeconds(1);
        string tempNotes = "";
        int d = 0;
        foreach (Transform child in NoteContentArea)
        {
            
            tempNotes += "Note:" + child.GetComponent<Text>().text + "|Id:" + d + ";";
            d++;
        }
        saveload.appdata = tempNotes;
        saveload.Save();
        LoadNotes();
    }

    private int currentDelID;
    void OnDeleteButtonPressed(int id)
    {
        int d = -1;
        foreach (NoteData note in notedata)
        {
            d++;
            if (id == note.ID)
            {
                currentDelID = id;
                ConfirmationDeletePannel.SetActive(true);
                
            }
        }
    }

    void SaveNotes()
    {
        string tempNotes="";
        int d = 0;
        foreach (NoteData note in notedata)
        {
            tempNotes += "Note:" + note.TextData + "|Id:" + d+";";
            d++;
        }
        
        saveload.appdata = tempNotes;
        saveload.Save();
        LoadNotes();
    }

    
    void LoadNotes()
    {
        string[] items;
        items = null;
        notedata.Clear();
        saveload.Load();
        
        if (saveload.appdata == null || saveload.appdata == " " || saveload.appdata == "")
        {
            return;
            print(saveload.appdata+"AppData");
        }
        else
        {
            items = saveload.appdata.Split(';');
            print(saveload.appdata + "AppData");

            for (int i = 0; i < items.Length-1; i++)
            {
                textdata = " ";
                id = 0;
                
                textdata = GetDataValue(items[i], "Note:");
               
                id = Convert.ToInt32(GetDataValue(items[i], "Id:"));
               
                notedata.Add(new NoteData(textdata, id));

            }
        }
            
    }

    public GameObject ConfirmationDeletePannel;

    public void DelConfirmNoButtonPressed()
    {
        NoteBookContent();
    }

    public void DelConfirmYesButtonPressed()
    {
        int id;
        id=currentDelID;
        int d = -1;
        foreach (NoteData note in notedata)
        {
            d++;
            if (id == note.ID)
            {
                notedata.RemoveAt(d);
                SaveNotes();
                NoteBookContent();
            }
        }
        
    }


    public void OnEditButtonPressed(int id)
    {

        //open editor pannel
        //load all the data on that perticular id
        newNote = false;
        ActivatePanel(EdditorPannel.name);
        foreach (NoteData m in notedata)
        {
            if (id == m.ID)
            {
               NoteWrittenInnputField.text= m.TextData;
               EditID = id;
            }
        }
    }

    public void CreateNewNoteButtonPressed()
    {
        ActivatePanel(EdditorPannel.name);
        EmptyNewInputfield();
        newNote = true;
    }

    public void RefreshNotebookButtonPressed()
    {
        NoteBookContent();
    }

    #endregion

    #region notepadTopRegion

    [Header("NotePad Top Bar Elements")]
    public GameObject MoreOptionPannel;
    public GameObject SaveButtonOfTopBar;
    public GameObject TickButtonOfTopBar;
    public GameObject FileNameInputField;
    public GameObject FileNameTextField;
    public InputField FileNameInputFieldUI;

    public GameObject NotepadDragableList;

    private bool isEditMode = false;
    private bool isRearrangeMode = false;
    private bool isMoreOptionPannelMode = false;

    public void MoreOptionButtonPressed()
    {
        if (isMoreOptionPannelMode)
        {
            isMoreOptionPannelMode = false;
            MoreOptionPannel.SetActive(false);
        }
        else
        {
            isMoreOptionPannelMode = true;
            MoreOptionPannel.SetActive(true);
        }
    }

    public void OnEditButtonIsPressedOfTopBar()
    {
        SaveButtonOfTopBar.SetActive(true);
        FileNameInputField.SetActive(true);
        FileNameTextField.SetActive(false);
        TickButtonOfTopBar.SetActive(false);
        FileNameInputFieldUI.text = saveload.current_filenameCallingName;
        MoreOptionButtonPressed();
    }

    public void OnRearrangeButtonIsPressedOfTopBar()
    {
        NotepadDragableList.GetComponent<UnityEngine.UI.Extensions.ReorderableList>().IsDraggable = true;
        TickButtonOfTopBar.SetActive(true);
        SaveButtonOfTopBar.SetActive(false);
        FileNameInputField.SetActive(false);
        FileNameTextField.SetActive(true);
        MoreOptionButtonPressed();
    }

    public void OnSaveButtonIsPressedOfTopbar()
    {

        gameObject.GetComponent<HomePageController>().ChangethetitleOfFile(saveload.current_filename, FileNameInputFieldUI.text, saveload.forWhich_file);
        saveload.current_filenameCallingName=FileNameInputFieldUI.text;
        TickButtonOfTopBar.SetActive(false);
        SaveButtonOfTopBar.SetActive(false);
        FileNameInputField.SetActive(false);
        FileNameTextField.SetActive(true);
        
        FileNameTextField.GetComponent<Text>().text = saveload.current_filenameCallingName;
        
    }

    public void OnTickButtonIsPressedOfTopBar()
    {
        NotepadDragableList.GetComponent<UnityEngine.UI.Extensions.ReorderableList>().IsDraggable = false;
        TickButtonOfTopBar.SetActive(false);
        SaveButtonOfTopBar.SetActive(false);
        FileNameInputField.SetActive(false);
        FileNameTextField.SetActive(true);
        
    }

    public void OnDeleteButtonIsPressedOfTopBar()
    {
        gameObject.GetComponent<HomePageController>().OnDeletetheFile(saveload.current_filename, saveload.forWhich_file);
        TickButtonOfTopBar.SetActive(false);
        SaveButtonOfTopBar.SetActive(false);
        FileNameInputField.SetActive(false);
        FileNameTextField.SetActive(true);
        MoreOptionButtonPressed();
    }

    public void OnCancelButtonIsPressedOFTopBar()
    {
        TickButtonOfTopBar.SetActive(false);
        SaveButtonOfTopBar.SetActive(false);
        FileNameInputField.SetActive(false);
        FileNameTextField.SetActive(true);
        MoreOptionButtonPressed();
    }

    #endregion

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
                //RaycastHit hit;
                //Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

                PointerEventData cursor = new PointerEventData(EventSystem.current);        // This section prepares a list for all objects hit with the raycast
                cursor.position = Input.mousePosition;
                List<RaycastResult> objectsHit = new List<RaycastResult>();
                EventSystem.current.RaycastAll(cursor, objectsHit);
                int count = objectsHit.Count;
                int x = 0;

                var goname = objectsHit[0];
                //print(goname.ToString());

                string a = GetDataValue(goname.ToString(), "Name:");

                foreach (NoteData m in notedata)
                {

                    if (a.Contains("Del:"+m.TextData))
                    {
                        OnDeleteButtonPressed(m.ID);
                    }
                    else if (a.Contains("Edit:" + m.TextData))
                    {
                        OnEditButtonPressed(m.ID);
                    }
                }
            }
        }

    public void ActivatePanel(string panelToBeActivated)
    {
        CreateAccountPannel.SetActive(panelToBeActivated.Equals(CreateAccountPannel.name));
        LoginPannel.SetActive(panelToBeActivated.Equals(LoginPannel.name));
        NotebookPannel.SetActive(panelToBeActivated.Equals(NotebookPannel.name));
        EdditorPannel.SetActive(panelToBeActivated.Equals(EdditorPannel.name));
        InspirationalSpeechPannel.SetActive(panelToBeActivated.Equals(InspirationalSpeechPannel.name));
    }

    string GetDataValue(string data, string index)
    {
        string value = data.Substring(data.IndexOf(index) + index.Length);
        if (value.Contains("|"))
            value = value.Remove(value.IndexOf("|"));
        return value;
    }
}
