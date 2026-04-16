using UnityEngine;
using UnityEngine.UIElements;
using System.IO;

public class DSIFinal : MonoBehaviour
{
    VisualElement AppNotesButton, AppShopButton, AppPassportButton, AppMessagesButton, AppCameraButton;
    VisualElement AppNotes, AppShop, AppPassport, AppMessages, AppCamera;

    TextField TF;
    Button SaveNotes, RecupNotes;

    SliderInt SliderShop;
    Label Cant;
    Button BuyButton;

    VisualElement Carrito;
    VisualTreeAsset napoTemplate;

    TextField Name, Surname;
    Button SavePass, RecupPass;

    string notesPath;
    string passPath;

    [System.Serializable]
    public class NotesData { public string text; }

    [System.Serializable]
    public class PassData { public string name; public string surname; }

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        AppNotesButton = root.Q<VisualElement>("AppNotesButton");
        AppShopButton = root.Q<VisualElement>("AppShopButton");
        AppPassportButton = root.Q<VisualElement>("AppPassportButton");
        AppMessagesButton = root.Q<VisualElement>("AppMessagesButton");
        AppCameraButton = root.Q<VisualElement>("AppCameraButton");

        AppNotes = root.Q<VisualElement>("AppNotes");
        AppShop = root.Q<VisualElement>("AppShop");
        AppPassport = root.Q<VisualElement>("AppPassport");
        AppMessages = root.Q<VisualElement>("AppMessages");
        AppCamera = root.Q<VisualElement>("AppCamera");

        var TFcontainer = AppNotes.Q<VisualElement>("TF");
        TF = TFcontainer.Q<TextField>();

        SaveNotes = AppNotes.Q<Button>("SaveNotes");
        RecupNotes = AppNotes.Q<Button>("RecupNotes");

        SliderShop = AppShop.Q<SliderInt>("SliderShop");
        Cant = AppShop.Q<Label>("Cant");
        BuyButton = AppShop.Q<Button>("BuyButton");

        Carrito = AppShop.Q<VisualElement>("Carrito");

        Name = AppPassport.Q<TextField>("Name");
        Surname = AppPassport.Q<TextField>("Surname");
        SavePass = AppPassport.Q<Button>("SavePass");
        RecupPass = AppPassport.Q<Button>("RecupPass");

        notesPath = Path.Combine(Application.persistentDataPath, "notes.json");
        passPath = Path.Combine(Application.persistentDataPath, "pass.json");

        napoTemplate = Resources.Load<VisualTreeAsset>("Napolitana");

        HideAll();

        AppNotesButton.RegisterCallback<ClickEvent>(e => { HideAll(); AppNotes.style.display = DisplayStyle.Flex; });
        AppShopButton.RegisterCallback<ClickEvent>(e => { HideAll(); AppShop.style.display = DisplayStyle.Flex; });
        AppPassportButton.RegisterCallback<ClickEvent>(e => { HideAll(); AppPassport.style.display = DisplayStyle.Flex; });
        AppMessagesButton.RegisterCallback<ClickEvent>(e => { HideAll(); AppMessages.style.display = DisplayStyle.Flex; });
        AppCameraButton.RegisterCallback<ClickEvent>(e => { HideAll(); AppCamera.style.display = DisplayStyle.Flex; });

        SaveNotes.clicked += SaveNotesFunc;
        RecupNotes.clicked += LoadNotesFunc;

        SliderShop.RegisterValueChangedCallback(e => {
            Cant.text = "Cantidad: " + e.newValue;

            var existentes = Carrito.Query<VisualElement>(className: "napoItem").ToList();
            foreach (var el in existentes)
                el.RemoveFromHierarchy();

            for (int i = 0; i < e.newValue; i++)
            {
                VisualElement item = napoTemplate.Instantiate();
                item.AddToClassList("napoItem");
                Carrito.Add(item);
            }
        });

        BuyButton.clicked += () => {
            SliderShop.SetValueWithoutNotify(0);
            Cant.text = "Napochocos compradas con exito";

            var existentes = Carrito.Query<VisualElement>(className: "napoItem").ToList();
            foreach (var el in existentes)
                el.RemoveFromHierarchy();
        };

        SavePass.clicked += SavePassFunc;
        RecupPass.clicked += LoadPassFunc;
    }

    void HideAll()
    {
        AppNotes.style.display = DisplayStyle.None;
        AppShop.style.display = DisplayStyle.None;
        AppPassport.style.display = DisplayStyle.None;
        AppMessages.style.display = DisplayStyle.None;
        AppCamera.style.display = DisplayStyle.None;
    }

    void SaveNotesFunc()
    {
        NotesData data = new NotesData();
        data.text = TF.value;
        File.WriteAllText(notesPath, JsonUtility.ToJson(data));
    }

    void LoadNotesFunc()
    {
        if (File.Exists(notesPath))
        {
            var data = JsonUtility.FromJson<NotesData>(File.ReadAllText(notesPath));
            TF.value = data.text;
        }
    }

    void SavePassFunc()
    {
        PassData data = new PassData();
        data.name = Name.value;
        data.surname = Surname.value;
        File.WriteAllText(passPath, JsonUtility.ToJson(data));
    }

    void LoadPassFunc()
    {
        if (File.Exists(passPath))
        {
            var data = JsonUtility.FromJson<PassData>(File.ReadAllText(passPath));
            Name.value = data.name;
            Surname.value = data.surname;
        }
    }
}