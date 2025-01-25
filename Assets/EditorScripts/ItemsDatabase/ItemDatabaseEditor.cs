using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemDatabase))]
public class ItemDatabaseEditor : Editor
{
    private static Texture2D customIcon;

    private void OnEnable()
    {
        // �������� ���������������� ������ �� ��������
        if (customIcon == null)
        {
            customIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/Icons/ItemDatabaseIcon.png");
        }

        // ��������� ������ ��� �������� �������
        if (customIcon != null)
        {
            SetCustomIcon((ItemDatabase)target);
        }
        else
        {
            Debug.LogWarning("�� ������� ��������� ���������������� ������. ��������� ���� � ����.");
        }
    }

    public override void OnInspectorGUI()
    {
        ItemDatabase itemDatabase = (ItemDatabase)target;

        // ������ � SerializedObject ��� ��������� ������� � ���������
        SerializedObject serializedItemDatabase = new SerializedObject(itemDatabase);

        // ������ �������� � ��������� ���� ��� ������ � ������ ��������
        SerializedProperty itemsProperty = serializedItemDatabase.FindProperty("Items");

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
            ItemDatabase.PrintGlobalList();
        }

        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("�������� ���������� ������", GUILayout.Width(200), GUILayout.Height(40)))
        {
            ItemDatabase.ClearGlobalList();
        }
    }

    private void AddNewItem(ItemDatabase itemDatabase)
    {
        ItemPrefab newItem = new ItemPrefab();
        itemDatabase.AddItem(newItem);
        EditorUtility.SetDirty(itemDatabase);
    }

    private void SetCustomIcon(ItemDatabase database)
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
