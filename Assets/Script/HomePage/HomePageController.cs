using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.EventSystems;

public class HomePageController : MonoBehaviour
{
    
    List<string> pu_fileNames = new List<string>();
    List<string> pu_fileCallNames = new List<string>();
    List<string> pr_fileNames = new List<string>();
    List<string> pr_fileCallNames = new List<string>();
    //pu_fileNames.Add(1);

    [Header("Create Account and Log In Pannel Elements")]
    public GameObject CreateAccountButton;
    public GameObject LoginButton;
    public GameObject AddPrivateFileButton;
    public GameObject LogOutButton;

    public GameObject CreateOrlogInPannel;
    public GameObject CreateAccountPannel;
    public GameObject LogInPannel;

    public Sprite remember_meCheckbox;
    public GameObject RememberMeCheckBoxButton;
    public InputField emailSignUpPannelInputField;
    public InputField passwordSignInPannelInputField;
    public InputField passwordSignUpPannelInputField;

    public Text ResultText;
 
    void Start()
    {
        //congratulations we have 2 save scripting files dont forget to first call fileloader script then load file and then open the appdata
        
        LoadAllData();
        
    }

    void LoadAllData()
    {
        GameObject[] go = GameObject.FindGameObjectsWithTag("MenuFileName");
        foreach (GameObject g in go)
        {
            Destroy(g);
        }

        pu_fileNames.Clear();
        pu_fileCallNames.Clear();
        pr_fileNames.Clear();
        pr_fileCallNames.Clear();

        saveload.appdata = "";

        LoadPUFilesToList();
        LoadPRFilesToList();

        UpdateMenuOptionBar();
    }

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
            string a = GetDataValue(goname.ToString(), "Name:");
            if ( a.Contains("Reminder"))
            {
                OnMenuOptionsButtonClicked("Reminder");
            }
            else if (a.Contains("Task"))
            {
                OnMenuOptionsButtonClicked("Task");
            }
            else if (a.Contains("Notes"))
            {
                OnMenuOptionsButtonClicked("Notes");
            }
            else if (a.Contains("InspirationalQuotes"))
            {
                OnMenuOptionsButtonClicked("InspirationalQuotes");
            }
            else if (a.Contains("FeedBack"))
            {
                OnMenuOptionsButtonClicked("FeedBack");
            }
            else if (a.Contains("PrivacyPolicy"))
            {
                OnMenuOptionsButtonClicked("PrivacyPolicy");
            }
            else
            {
                
                foreach (string s in pu_fileNames)
                {
                    if (a.Contains(s))
                    {
                        OnMenuOptionsButtonClicked(s);
                        saveload.forWhich_file = "public";
                    }
                }
                foreach (string s in pr_fileNames)
                {
                    if (a.Contains(s))
                    {
                        OnMenuOptionsButtonClicked(s);
                        saveload.forWhich_file = "private";
                    }
                }
            }
        }
    }

    #region MenuBarFunctions

    private string previousSelected = " ";
    private string currentSelecteed = " ";
    void OnMenuOptionsButtonClicked(string optionClicked)
    {
        GameObject temp=gameObject;
        if (optionClicked == "InspirationalQuotes")
        {
            currentSelecteed="InspirationalQuotes";
             temp= GameObject.Find(optionClicked + ":Parent");
            temp.GetComponent<Animator>().Play("menuitemsclickeffect");
        }
        else if (optionClicked == "Reminder")
        {
            currentSelecteed="Reminder";
            temp = GameObject.Find(optionClicked + ":Parent");
            temp.GetComponent<Animator>().Play("menuitemsclickeffect");

            gameObject.GetComponent<AppController>().OnReminderButtonClick();
        }
        else if (optionClicked == "Task")
        {
            currentSelecteed="Task";
            temp = GameObject.Find(optionClicked + ":Parent");
            temp.GetComponent<Animator>().Play("menuitemsclickeffect");
        }
        else if (optionClicked == "Notes")
        {
            currentSelecteed = "Notes";
            temp = GameObject.Find(optionClicked + ":Parent");
            temp.GetComponent<Animator>().Play("menuitemsclickeffect");
        }
        else if (optionClicked == "FeedBack")
        {
            currentSelecteed = "FeedBack";
            temp = GameObject.Find(optionClicked + ":Parent");
            temp.GetComponent<Animator>().Play("menuitemsclickeffect");
        }
        else if (optionClicked == "PrivacyPolicy")
        {
            currentSelecteed = "PrivacyPolicy";
            temp = GameObject.Find(optionClicked + ":Parent");
            temp.GetComponent<Animator>().Play("menuitemsclickeffect");
        }
        else
        {
            if (GameObject.Find(optionClicked + ":Parent"))
            {
                currentSelecteed = optionClicked;
                saveload.current_filename = optionClicked;
                saveload.appdata = "";
                saveload.Load();

                //for changing the file title name
                for (int i = 0; i < pu_fileNames.Count; i++)
                {
                    if (optionClicked == pu_fileNames[i])
                    {
                        saveload.current_filenameCallingName = pu_fileCallNames[i];
                    }
                }

                gameObject.GetComponent<AppController>().ActivatePanel("NoteBookPannel");
                MenuButtonPressed();
                gameObject.GetComponent<AppController>().NoteBookContent();
                temp = GameObject.Find(optionClicked + ":Parent");
                temp.GetComponent<Animator>().Play("menuitemsclickeffect");
            }
            
        }
        stoppreviousAnimation(temp);
    }

    void stoppreviousAnimation(GameObject temp)
    {
        print(previousSelected + "|" + currentSelecteed);
        if (previousSelected != currentSelecteed && previousSelected!=" ")
        {
            //stop previous animation and update previous selected
            temp = GameObject.Find(previousSelected + ":Parent");
            temp.GetComponent<Animator>().Play("menubacktodefaultstate");
            

        }
        
        previousSelected = currentSelecteed;
    }

    
    public Animator menubarAnimator;
    private bool isMenuBarOpen = false;
    public void MenuButtonPressed()
    {
        if (isMenuBarOpen == false)
        {
            isMenuBarOpen = true;
            menubarAnimator.Play("menuBar");
        }
        else
        {
            isMenuBarOpen = false;
            menubarAnimator.Play("menubarclose");
        }
    }

    #endregion

    #region Load Files from files to list and update to menuoptions

    void LoadPUFilesToList()
    {
        MainSaveFiledetails.Load();
        //loading files
        int m = 0;
        string all_names = MainSaveFiledetails.pu_filenames;
        string[] items = all_names.Split(';');

        int d = items.Length;
        for (int i = 0; i < d - 1; i++)
        {
            m++;
            pu_fileNames.Add(GetDataValue(items[i], "Pu_Note:"));
        }
        saveload.current_total_public_file_count = m;
        

        //loading files callname
        string all_names_file = MainSaveFiledetails.pu_fileCallname;
        string[] items_file = all_names_file.Split(';');

        int d_file = items_file.Length;
        for (int i = 0; i < d_file - 1; i++)
        {
            pu_fileCallNames.Add(GetDataValue(items_file[i], "Pu_Note:"));
        }
    }
    void LoadPRFilesToList()
    {
        MainSaveFiledetails.Load();
        string all_names = MainSaveFiledetails.pr_filenames;
        string[] items = all_names.Split(';');
        int m = 0;
        int d = items.Length;
        for (int i = 0; i < d - 1; i++)
        {
            m++;
            pr_fileNames.Add(GetDataValue(items[i], "Pr_Note:"));
        }
        saveload.current_total_private_file_count = m;
        

        //loading files callname
        string all_names_file = MainSaveFiledetails.pr_fileCallname;
        string[] items_file = all_names_file.Split(';');

        int d_file = items_file.Length;
        for (int i = 0; i < d_file - 1; i++)
        {
            pr_fileCallNames.Add(GetDataValue(items_file[i], "Pr_Note:"));
        }

    }

    [Header("Menu OptionBar")]

    public GameObject MenuOptionGo;
    public GameObject ContentSpaceForMenuOptions;

    

    void UpdateMenuOptionBar()
    {
        //adding all the file names
        int n = pu_fileNames.Count;
        for (int s = 0; s < n; s++)
        {
            GameObject go = (GameObject)Instantiate(MenuOptionGo) as GameObject;//instatiate content
            go.name = pu_fileNames[s] + ":Parent";
            
            go.transform.Find("Text").GetComponent<Text>().text = pu_fileCallNames[s];
            go.transform.Find("FileName").gameObject.name = pu_fileNames[s];
            go.transform.SetParent(ContentSpaceForMenuOptions.transform, false);//setting  parrent to earning
            //int temp = ContentSpaceForMenuOptions.transform.childCount;//getting all child count of earning entry list

            int t = GameObject.Find("AddNoteButton").transform.GetSiblingIndex();

            //temp = temp - 3;//for add aur input field ko last me rakhne ke liye
            go.transform.SetSiblingIndex(t);//ab set kar diya
        }

        //now adding the private button
        

        if (MainSaveFiledetails.password == ""&&MainSaveFiledetails.isLogIn==false) //means not created account
        {
            //show create account button and hide login and add button
            CreateAccountButton.SetActive(true);
            LoginButton.SetActive(false);
            AddPrivateFileButton.SetActive(false);
            LogOutButton.SetActive(false);
        }
        else
        {
            CreateAccountButton.SetActive(false);
            LoginButton.SetActive(true);
            AddPrivateFileButton.SetActive(false);
            LogOutButton.SetActive(false);
        }

        if (MainSaveFiledetails.isLogIn)
        {
            CreateAccountButton.SetActive(false);
            LoginButton.SetActive(false);
            AddPrivateFileButton.SetActive(true);
            LogOutButton.SetActive(true);

            n = pr_fileNames.Count;
            for (int s = 0; s < n; s++)
            {
                GameObject go = (GameObject)Instantiate(MenuOptionGo) as GameObject;//instatiate content
                go.name = pr_fileNames[s] + ":Parent";
                go.transform.Find("Text").GetComponent<Text>().text = pr_fileCallNames[s];
                go.transform.Find("FileName").gameObject.name = pr_fileNames[s];
                go.transform.SetParent(ContentSpaceForMenuOptions.transform, false);//setting  parrent to earning
                //int temp = ContentSpaceForMenuOptions.transform.childCount;//getting all child count of earning entry list

                int t = GameObject.Find("AddNoteButton_Pr").transform.GetSiblingIndex();

                //temp = temp - 3;//for add aur input field ko last me rakhne ke liye
                go.transform.SetSiblingIndex(t);//ab set kar diya
            }
        }
         
    }

    #endregion

    public void AddPublicNotesButtonClicked()
    {
        //open note pannel and access to the
        gameObject.GetComponent<AppController>().ActivatePanel("NoteBookPannel");
        MenuButtonPressed();

        int d = 0;

        MainSaveFiledetails.pu_filenames += "Pu_Note:info" + (saveload.current_total_public_file_count + 1) + ".dat;";
        MainSaveFiledetails.pu_fileCallname += "Pu_Note:Quick Public Note " + (saveload.current_total_public_file_count + 1)+";";
        MainSaveFiledetails.Save();

        saveload.current_filename = "info" + (saveload.current_total_public_file_count + 1) + ".dat";
        saveload.current_filenameCallingName = "Quick Public Note";

        

        LoadAllData();
        gameObject.GetComponent<AppController>().NoteBookContent();
    }

    public void AddPrivateNotesButtonClicked()
    {
        //open note pannel and access to the
        gameObject.GetComponent<AppController>().ActivatePanel("NoteBookPannel");
        MenuButtonPressed();

        int d = 0;

        MainSaveFiledetails.pr_filenames += "Pr_Note:pr_info" + (saveload.current_total_public_file_count + 1) + ".dat;";
        MainSaveFiledetails.pr_fileCallname += "Pr_Note:Quick Private Note " + (saveload.current_total_public_file_count + 1) + ";";
        MainSaveFiledetails.Save();

        saveload.current_filename = "pr_info" + (saveload.current_total_public_file_count + 1) + ".dat";
        saveload.current_filenameCallingName = "Quick Private Note";



        LoadAllData();
        gameObject.GetComponent<AppController>().NoteBookContent();
    }

    #region create account or log in button;

    public void OnCreateAccountButtonIsPressedFromMenuShowThePannel()
    {
        CreateOrlogInPannel.SetActive(true);
        if (MainSaveFiledetails.password == ""&&MainSaveFiledetails.isLogIn==false)
        {
            CreateAccountPannel.SetActive(true);
            LogInPannel.SetActive(false);
        }
    }

    public void OnLogInButtonIsPressedFromMenuShowThePannel()
    {
        CreateOrlogInPannel.SetActive(true);
        if (MainSaveFiledetails.isLogIn == false)
        {
            CreateAccountPannel.SetActive(false);
            LogInPannel.SetActive(true);
        }
    }

    public void OnCreateAccountButtonIsPressedFromPannel()
    {
        //create account and log in
        MainSaveFiledetails.email = emailSignUpPannelInputField.text;
        MainSaveFiledetails.password = passwordSignInPannelInputField.text;
        MainSaveFiledetails.Save();

        CreateOrlogInPannel.SetActive(false);
        LoadAllData();
    }

    public void OnLogInButtonIsPressedFromPannel()
    {
        //create account and log in
        if (MainSaveFiledetails.password == passwordSignUpPannelInputField.text)
        {
            MainSaveFiledetails.isLogIn = true;
            CreateOrlogInPannel.SetActive(false);
            LoadAllData();
        }
        else
        {
            ResultText.text = "Wrong Password";
        }
    }

    public void OnRememberMeButtonPressed()
    {
        if (MainSaveFiledetails.remember_me == false)
        {
            MainSaveFiledetails.remember_me = true;
            RememberMeCheckBoxButton.GetComponent<Image>().sprite = remember_meCheckbox;
        }
        else
        {
            MainSaveFiledetails.remember_me = false;
            RememberMeCheckBoxButton.GetComponent<Image>().sprite = null;
        }
        MainSaveFiledetails.Save();
    }

    public void OnLogOutButtonPresseed()
    {
        MainSaveFiledetails.remember_me = false;
        MainSaveFiledetails.isLogIn = false;
        MainSaveFiledetails.Save();
        LoadAllData();
    }

    #endregion

    #region save and delete handler from appcontroller

    public void ChangethetitleOfFile(string filename, string newName, string forWhich)
    {
        if (forWhich == "public")
        {
            for (int i = 0; i < pu_fileNames.Count; i++)
            {
                if (pu_fileNames[i] == filename)
                {
                    pu_fileCallNames[i] = newName;
                    
                }

            }
        }
        else if (forWhich == "private")
        {
            for (int i = 0; i < pr_fileNames.Count; i++)
            {
                if (pr_fileNames[i] == filename)
                {
                    pr_fileCallNames[i] = newName;
                }

            }
        }

        ConvertFilesListToString();
        LoadAllData();
    }

    public void OnDeletetheFile(string filename, string forWhich)
    {
        if (forWhich == "public")
        {
            for (int i = 0; i < pu_fileNames.Count; i++)
            {
                if (pu_fileNames[i] == filename)
                {
                    pu_fileNames.RemoveAt(i);
                    pu_fileCallNames.RemoveAt(i);

                }

            }
        }
        else
        {
            for (int i = 0; i < pr_fileNames.Count; i++)
            {
                if (pr_fileNames[i] == filename)
                {
                    pr_fileNames.RemoveAt(i);
                    pr_fileCallNames.RemoveAt(i);

                }

            }
        }
        ConvertFilesListToString();
        LoadAllData();
        gameObject.GetComponent<AppController>().ActivatePanel("HomepageMainPage");
    }

    void ConvertFilesListToString()
    {
        string fileName = "";
        string fileCallName = "";
        for (int i = 0; i < pu_fileNames.Count; i++)
        {
            fileName += "Pu_Note:"+pu_fileNames[i]+";";
            fileCallName += "Pu_Note:" + pu_fileCallNames[i] + ";";
        }
        MainSaveFiledetails.pu_filenames = fileName;
        MainSaveFiledetails.pu_fileCallname = fileCallName;

        fileName = "";
        fileCallName = "";
        for (int i = 0; i < pr_fileNames.Count; i++)
        {
            fileName += "Pr_Note:" + pr_fileNames[i] + ";";
            fileCallName += "Pr_Note:" + pr_fileCallNames[i] + ";";
        }
        MainSaveFiledetails.pr_filenames = fileName;
        MainSaveFiledetails.pr_fileCallname = fileCallName;

        MainSaveFiledetails.Save();
    }

    #endregion

    string GetDataValue(string data, string index)
    {
        string value = data.Substring(data.IndexOf(index) + index.Length);
        if (value.Contains("|"))
            value = value.Remove(value.IndexOf("|"));
        return value;
    }
}
