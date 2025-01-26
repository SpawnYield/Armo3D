using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(ItemIconsDatabase))]
public class ItemIconsDatabaseEditor : Editor
{
    private static Texture2D customIcon;

    private void OnEnable()
    {
     
        // Загрузка пользовательской иконки из ресурсов
        if (customIcon == null)
        {
            customIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/Icons/ItemDatabaseIconsIcon.png");
        }

        // Применяем иконку при загрузке объекта
        if (customIcon != null)
        {
            SetCustomIcon((ItemIconsDatabase)target);
        }
        else
        {
            Debug.LogWarning("Не удалось загрузить пользовательскую иконку. Проверьте путь и файл.");
        }
    }

    public override void OnInspectorGUI()
    {
        ItemIconsDatabase itemDatabase = (ItemIconsDatabase)target;

        // Работа с SerializedObject для получения доступа к свойствам
        SerializedObject serializedItemDatabase = new SerializedObject(itemDatabase);

        // Рисуем элементы и добавляем поле для ассета и кнопку удаления
        SerializedProperty itemsProperty = serializedItemDatabase.FindProperty("ItemIcons");

        for (int i = 0; i < itemsProperty.arraySize; i++)
        {
            GUI.backgroundColor = Color.cyan;
            SerializedProperty itemProperty = itemsProperty.GetArrayElementAtIndex(i);
            EditorGUILayout.BeginHorizontal();

            // ID
            SerializedProperty idProperty = itemProperty.FindPropertyRelative("Id");
            EditorGUILayout.LabelField("ID: " + idProperty.intValue, GUILayout.Width(100), GUILayout.Height(35));

            // Ссылка (с кастомным стилем, без текста "Link")
            SerializedProperty linkProperty = itemProperty.FindPropertyRelative("Link");
            EditorGUILayout.PropertyField(linkProperty, GUIContent.none, GUILayout.Height(35), GUILayout.Width(500));

            // Кнопка удаления
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Удалить", GUILayout.Width(200), GUILayout.Height(35)))
            {
                itemDatabase.RemoveItem(idProperty.intValue); // Вызов метода удаления
                itemDatabase.RemoveFromGlobalList(idProperty.intValue);
            }

            EditorGUILayout.EndHorizontal(); // Закрываем горизонтальный ряд
        }

        // Рисуем стандартные поля ScriptableObject
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Управление локальными предметами", EditorStyles.boldLabel);

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Добавить новый предмет", GUILayout.Width(400), GUILayout.Height(50)))
        {
            AddNewItem(itemDatabase);
            itemDatabase.MergeToGlobalList();
        }

        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Очистить локальный список", GUILayout.Width(300), GUILayout.Height(30)))
        {
            itemDatabase.ClearLocalList();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Управление глобальным списком", EditorStyles.boldLabel);
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Вывести глобальный список", GUILayout.Width(400), GUILayout.Height(40)))
        {
            ItemIconsDatabase.PrintGlobalList();
        }
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Синхронизировать глобальным и локальный список", EditorStyles.boldLabel);
        GUI.backgroundColor = new(1,0.5f,0.75f);
        if (GUILayout.Button("Синхронизировать", GUILayout.Width(400), GUILayout.Height(40)))
        {
            itemDatabase.MergeToGlobalList();
        }
        EditorGUILayout.Space();
        GUI.backgroundColor = Color.red;
        EditorGUILayout.LabelField("Очистить глобальный список", EditorStyles.boldLabel);
        if (GUILayout.Button("Очистить", GUILayout.Width(200), GUILayout.Height(40)))
        {
            ItemIconsDatabase.ClearGlobalList();
        }
    }

    private void AddNewItem(ItemIconsDatabase itemDatabase)
    {
        ItemPrefab newItem = new ItemPrefab();
        itemDatabase.AddItem(newItem);
        EditorUtility.SetDirty(itemDatabase);
    }

    private void SetCustomIcon(ItemIconsDatabase database)
    {
        if (customIcon != null)
        {
            EditorGUIUtility.SetIconForObject(database, customIcon);
            Debug.Log("Пользовательская иконка установлена.");
        }
        else
        {
            Debug.LogError("Не удалось установить иконку. Иконка не загружена.");
        }
    }
}
