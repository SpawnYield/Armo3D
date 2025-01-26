using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(ItemIconsDatabase))]
public class ItemIconsDatabaseEditor : Editor
{
    private static Texture2D customIcon;

    private void OnEnable()
    {
     
        // �������� ���������������� ������ �� ��������
        if (customIcon == null)
        {
            customIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/Icons/ItemDatabaseIconsIcon.png");
        }

        // ��������� ������ ��� �������� �������
        if (customIcon != null)
        {
            SetCustomIcon((ItemIconsDatabase)target);
        }
        else
        {
            Debug.LogWarning("�� ������� ��������� ���������������� ������. ��������� ���� � ����.");
        }
    }

    public override void OnInspectorGUI()
    {
        ItemIconsDatabase itemDatabase = (ItemIconsDatabase)target;

        // ������ � SerializedObject ��� ��������� ������� � ���������
        SerializedObject serializedItemDatabase = new SerializedObject(itemDatabase);

        // ������ �������� � ��������� ���� ��� ������ � ������ ��������
        SerializedProperty itemsProperty = serializedItemDatabase.FindProperty("ItemIcons");

        for (int i = 0; i < itemsProperty.arraySize; i++)
        {
            GUI.backgroundColor = Color.cyan;
            SerializedProperty itemProperty = itemsProperty.GetArrayElementAtIndex(i);
            EditorGUILayout.BeginHorizontal();

            // ID
            SerializedProperty idProperty = itemProperty.FindPropertyRelative("Id");
            EditorGUILayout.LabelField("ID: " + idProperty.intValue, GUILayout.Width(100), GUILayout.Height(35));

            // ������ (� ��������� ������, ��� ������ "Link")
            SerializedProperty linkProperty = itemProperty.FindPropertyRelative("Link");
            EditorGUILayout.PropertyField(linkProperty, GUIContent.none, GUILayout.Height(35), GUILayout.Width(500));

            // ������ ��������
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("�������", GUILayout.Width(200), GUILayout.Height(35)))
            {
                itemDatabase.RemoveItem(idProperty.intValue); // ����� ������ ��������
                itemDatabase.RemoveFromGlobalList(idProperty.intValue);
            }

            EditorGUILayout.EndHorizontal(); // ��������� �������������� ���
        }

        // ������ ����������� ���� ScriptableObject
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("���������� ���������� ����������", EditorStyles.boldLabel);

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("�������� ����� �������", GUILayout.Width(400), GUILayout.Height(50)))
        {
            AddNewItem(itemDatabase);
            itemDatabase.MergeToGlobalList();
        }

        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("�������� ��������� ������", GUILayout.Width(300), GUILayout.Height(30)))
        {
            itemDatabase.ClearLocalList();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("���������� ���������� �������", EditorStyles.boldLabel);
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("������� ���������� ������", GUILayout.Width(400), GUILayout.Height(40)))
        {
            ItemIconsDatabase.PrintGlobalList();
        }
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("���������������� ���������� � ��������� ������", EditorStyles.boldLabel);
        GUI.backgroundColor = new(1,0.5f,0.75f);
        if (GUILayout.Button("����������������", GUILayout.Width(400), GUILayout.Height(40)))
        {
            itemDatabase.MergeToGlobalList();
        }
        EditorGUILayout.Space();
        GUI.backgroundColor = Color.red;
        EditorGUILayout.LabelField("�������� ���������� ������", EditorStyles.boldLabel);
        if (GUILayout.Button("��������", GUILayout.Width(200), GUILayout.Height(40)))
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
            Debug.Log("���������������� ������ �����������.");
        }
        else
        {
            Debug.LogError("�� ������� ���������� ������. ������ �� ���������.");
        }
    }
}
