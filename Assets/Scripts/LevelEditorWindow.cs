using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace ElementMerge
{
    public class LevelEditorWindow : EditorWindow
    {
        private Dictionary<ELEMENT_TYPE, Color> elementColors = new Dictionary<ELEMENT_TYPE, Color>()
        {
            { ELEMENT_TYPE.EMPTY,   new Color(0.1f,  0.1f, 0.1f     ) },
            { ELEMENT_TYPE.water,   new Color(0.6f,  0.77f, 1     ) },
            { ELEMENT_TYPE.WATER_POWERUP,   new Color(0.3f,  0.6f,  1     ) },
            { ELEMENT_TYPE.wind, new Color(0.75f, 0.75f, 0.75f ) },
            { ELEMENT_TYPE.WIND_POWERUP, new Color(0.6f,  0.6f,  0.6f  ) },
            { ELEMENT_TYPE.fire,  new Color(1,     0.77f, 0.77f ) },
            { ELEMENT_TYPE.FIRE_POWERUP,  new Color(1,     0,     0     ) },
            { ELEMENT_TYPE.dirt, new Color(0.69f, 0.4f,  0     ) },
            { ELEMENT_TYPE.DIRT_POWERUP, new Color(0.5f,  0.3f,  0     ) },
            { ELEMENT_TYPE.CYCLONE, new Color(1,     1,     1     ) },
            { ELEMENT_TYPE.LAVA,   new Color(1,     0.4f,  0     ) }
        };

        internal string levelID = "";

        internal LevelData levelData = new();

        private bool isRemovingField = false;
        private bool isFieldSelected = true;

        private GUIStyle textFieldStyle;
        private GUIStyle labelStyle;

        public ELEMENT_TYPE actualSelectedElement = ELEMENT_TYPE.water;

        [MenuItem("Window/Level Editor")]
        public static void ShowWindow()
        {
            LevelEditorWindow window = GetWindow<LevelEditorWindow>("Level Editor");
        }

        private void OnEnable()
        {
            textFieldStyle = new GUIStyle(EditorStyles.textField);
            textFieldStyle.fontSize = 20;

            labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.fontSize = 20;
            labelStyle.alignment = TextAnchor.MiddleCenter;

            SetMatrixInBlank();
        }

        private void OnGUI()
        {
            GUILayout.Space(15);

            ShowIDPanel();

            GUILayout.Space(10);

            ShowEditorModeSelectorPanel();

            GUILayout.Space(20);

            if (isFieldSelected)
            {
                ShowFieldSelectorPanel();
                ShowFieldSelectedPanel();
                GUILayout.Space(15);
                ShowMatrixPanel();
            }
            else
            {
                ShowELEMENT_TYPEelectorPanel();
                ShowELEMENT_TYPEelectedPanel();
                GUILayout.Space(15);
                ShowMatrixPanel();
            }


            if (EditorGUI.EndChangeCheck())
            {
                Repaint();
            }
        }

        #region Json Related

        private void GenerateJson()
        {
            // Verificar que se haya ingresado un ID
            if (string.IsNullOrEmpty(levelID))
            {
                Debug.LogError("ID del nivel no ingresado.");
                return;
            }

            levelData.level = Int32.Parse(levelID[^1].ToString());

            string json = JSONSerialize(levelData);
            Debug.Log(json);

            string filePath = "Assets/Levels/" + levelID + ".json";

            File.WriteAllText(filePath, json);

            AssetDatabase.Refresh();

            Debug.Log("Archivo JSON generado en: " + filePath);
        }
        public static string JSONSerialize<T>(T obj)
        {
            string retVal = string.Empty;
            using (MemoryStream ms = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                serializer.WriteObject(ms, obj);
                var byteArray = ms.ToArray();
                retVal = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
            }
            return retVal;
        }
        public static T JSONDeserialize<T>(string json)
        {
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(ms);
            }
        }
        private bool DoesFileExists(string id)
        {
            string filePath = "Assets/Levels/" + id + ".json";
            return File.Exists(filePath);
        }
        private void LoadJson()
        {
            if (string.IsNullOrEmpty(levelID))
            {
                Debug.LogError("ID del nivel no ingresado.");
                return;
            }

            string filePath = "Assets/Levels/" + levelID + ".json";

            string json;

            using (StreamReader reader = new StreamReader(filePath))
            {
                json = reader.ReadToEnd();
            }

            levelData = JSONDeserialize<LevelData>(json);

            //Debug.Log("El archivo json contiene: " + json);

        }
        #endregion
        private void UpdateElement(ELEMENT_TYPE newElement)
        {
            actualSelectedElement = newElement;
        }

        #region Showers
        private void ShowEditorModeSelectorPanel()
        {
            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            GUI.enabled = !isFieldSelected;
            if (GUILayout.Button("Field", GUILayout.Width(150), GUILayout.Height(30)))
            {
                isFieldSelected = true;
            }
            GUI.enabled = true;

            GUILayout.FlexibleSpace();


            GUILayout.FlexibleSpace();

            GUI.enabled = isFieldSelected;
            if (GUILayout.Button("Slimes", GUILayout.Width(150), GUILayout.Height(30)))
            {
                isFieldSelected = false;
            }
            GUI.enabled = true;

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();
        }
        private void ShowIDPanel()
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("ID", labelStyle, GUILayout.Width(80), GUILayout.Height(30));

            //EditorGUI.BeginChangeCheck();
            levelID = EditorGUILayout.TextField(levelID, textFieldStyle, GUILayout.Height(30), GUILayout.MinHeight(30));

            if (GUILayout.Button("Generar Archivo", GUILayout.Height(30)))
            {
                if (DoesFileExists(levelID))
                {
                    bool confirm = EditorUtility.DisplayDialog("Archivo ya existe",
                        "El archivo para el nivel " + levelID + " ya existe. ¿Quieres sobrescribirlo?",
                        "Sí", "No");

                    if (confirm)
                    {
                        GenerateJson(); // Sobrescribe si se confirma
                    }
                }
                else
                {
                    GenerateJson(); // Crea un archivo nuevo si no existe
                }
            }
            if (GUILayout.Button("Cargar Archivo", GUILayout.Height(30)))
            {
                if (!DoesFileExists(levelID))
                {
                    EditorUtility.DisplayDialog("Error de Lectura!", "No existe el archivo " + levelID, "De acuerdo");
                }
                else
                {
                    LoadJson(); //Abre el nuevo ID
                }
            }

            GUILayout.EndHorizontal();
        }
        private void ShowELEMENT_TYPEelectorPanel()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Agua"))
            {
                Debug.Log("Seleccionaste Agua");
                UpdateElement(ELEMENT_TYPE.water);
            }

            if (GUILayout.Button("Agua P."))
            {
                Debug.Log("Seleccionaste Agua Potenciada");
                UpdateElement(ELEMENT_TYPE.WATER_POWERUP);
            }

            if (GUILayout.Button("Viento"))
            {
                Debug.Log("Seleccionaste Viento");
                UpdateElement(ELEMENT_TYPE.wind);
            }

            if (GUILayout.Button("Viento P."))
            {
                Debug.Log("Seleccionaste Viento Potenciado");
                UpdateElement(ELEMENT_TYPE.WIND_POWERUP);
            }

            if (GUILayout.Button("Tierra"))
            {
                Debug.Log("Seleccionaste Tierra");
                UpdateElement(ELEMENT_TYPE.dirt);
            }

            if (GUILayout.Button("Tierra P."))
            {
                Debug.Log("Seleccionaste Tierra Potenciada");
                UpdateElement(ELEMENT_TYPE.DIRT_POWERUP);
            }

            if (GUILayout.Button("Fuego"))
            {
                Debug.Log("Seleccionaste Fuego");
                UpdateElement(ELEMENT_TYPE.fire);
            }

            if (GUILayout.Button("Fuego P."))
            {
                Debug.Log("Seleccionaste Fuego Potenciado");
                UpdateElement(ELEMENT_TYPE.FIRE_POWERUP);
            }

            if (GUILayout.Button("Ciclon"))
            {
                Debug.Log("Seleccionaste Ciclon");
                UpdateElement(ELEMENT_TYPE.CYCLONE);
            }

            if (GUILayout.Button("Lava"))
            {
                Debug.Log("Seleccionaste Lava");
                UpdateElement(ELEMENT_TYPE.LAVA);
            }
            if (GUILayout.Button("Borrar"))
            {
                Debug.Log("Seleccionaste Borrar");
                UpdateElement(ELEMENT_TYPE.EMPTY);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        private void ShowELEMENT_TYPEelectedPanel()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            string actualElementText = "";

            switch (actualSelectedElement)
            {
                case ELEMENT_TYPE.water:
                    actualElementText = "Agua";
                    break;
                case ELEMENT_TYPE.wind:
                    actualElementText = "Viento";
                    break;
                case ELEMENT_TYPE.fire:
                    actualElementText = "Fuego";
                    break;
                case ELEMENT_TYPE.dirt:
                    actualElementText = "Tierra";
                    break;
                case ELEMENT_TYPE.WATER_POWERUP:
                    actualElementText = "Agua Potenciada";
                    break;
                case ELEMENT_TYPE.WIND_POWERUP:
                    actualElementText = "Viento Potenciado";
                    break;
                case ELEMENT_TYPE.FIRE_POWERUP:
                    actualElementText = "Fuego Potenciado";
                    break;
                case ELEMENT_TYPE.DIRT_POWERUP:
                    actualElementText = "Tierra Potenciada";
                    break;
                case ELEMENT_TYPE.CYCLONE:
                    actualElementText = "Ciclon (Bloqueador)";
                    break;
                case ELEMENT_TYPE.LAVA:
                    actualElementText = "Lava (Bloqueador)";
                    break;
                default:
                    actualElementText = "%NaN%";
                    break;
            }

            if (actualSelectedElement == ELEMENT_TYPE.EMPTY)
            {
                GUILayout.Label("Herramienta de <b> Borrar </b>", new GUIStyle(GUI.skin.label) { richText = true });
            }
            else
            {
                GUILayout.Label("Elemento seleccionado: <b>" + actualElementText + "</b>", new GUIStyle(GUI.skin.label) { richText = true });
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        private void ShowFieldSelectorPanel()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Poner Terreno"))
            {
                Debug.Log("Hacer clic para agregar terreno");
                isRemovingField = false;
            }

            if (GUILayout.Button("Sacar Terreno"))
            {
                Debug.Log("Hacer clic para sacar terreno");
                isRemovingField = true;
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        private void ShowFieldSelectedPanel()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            string selectedText = isRemovingField ? "Sacando" : "Poniendo";
            GUILayout.Label("Estas: <b>" + selectedText + "</b>" + " terreno.", new GUIStyle(GUI.skin.label) { richText = true });
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        private void ShowMatrixPanel()
        {

            GUILayout.BeginVertical("box");

            float buttonSize = position.width / 10; // Tamaño del botón para que sea cuadrado
            
            int tileCounter = 0;
            int tileRowCounter = 1;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            foreach (Tile t in levelData.tileList)
            {
                tileCounter++;
                if (!isFieldSelected)
                {
                    string panelText = GetElementInitial(t.element);
                    GUIStyle panelStyle = GetELEMENT_TYPEtyle(t.element, buttonSize);
                    if (t.isAvailable)
                    {
                        if (GUILayout.Button(panelText, panelStyle, GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
                        {
                            t.element = actualSelectedElement;
                        }
                    }
                    else
                    {
                        GUI.enabled = false;
                        GUILayout.Button("", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize));
                        GUI.enabled = true;
                    }
                }
                else
                {
                    GUIStyle panelStyle = GetFieldStyle(t.isAvailable);

                    if (GUILayout.Button("", panelStyle, GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
                    {
                        t.isAvailable = !isRemovingField;
                        t.element = ELEMENT_TYPE.EMPTY;
                    }

                }
                if (tileCounter % 9 == 0)
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    if (tileRowCounter < 9)
                    {
                        tileRowCounter++;
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                    }
                }
            }

            GUILayout.EndVertical();


            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (!isFieldSelected)
            {
                if (GUILayout.Button("Resetear Nivel", GUILayout.Height(30)))
                {
                    bool confirm = EditorUtility.DisplayDialog("Estas a punto de borrar el contenido del nivel!",
                        "Seguro que queres continuar?",
                        "Sí", "No");

                    if (confirm)
                    {
                        ResetMatrix(); // Sobrescribe si se confirma
                    }
                }
            }
            else
            {
                if (GUILayout.Button("LLenar Todo", GUILayout.Height(30)))
                {
                    bool confirm = EditorUtility.DisplayDialog("Estas a punto de llenar todo el terreno!",
                        "Seguro que queres continuar?",
                        "Sí", "No");

                    if (confirm)
                    {
                        RefillMatrix();
                    }
                }
                if (GUILayout.Button("Vaciar Todo", GUILayout.Height(30)))
                {
                    bool confirm = EditorUtility.DisplayDialog("Estas a punto vaciar todo el terreno!",
                        "Seguro que queres continuar?",
                        "Sí", "No");

                    if (confirm)
                    {
                        EmptyMatrix();
                    }
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        #endregion

        #region Matrix Related
        private string GetElementInitial(ELEMENT_TYPE element)
        {
            if (element == ELEMENT_TYPE.EMPTY) return "";

            string nombreElemento = element.ToString();
            return nombreElemento.Substring(0, 1); // Devolver la inicial del nombre del elemento
        }
        private GUIStyle GetELEMENT_TYPEtyle(ELEMENT_TYPE element, float buttonSize)
        {
            GUIStyle estilo = new GUIStyle(GUI.skin.button);
            estilo.fontSize = Mathf.CeilToInt(buttonSize * 0.5f);
            estilo.alignment = TextAnchor.MiddleCenter;
            estilo.normal.textColor = Color.black;

            if (elementColors.ContainsKey(element))
            {
                estilo.normal.background = GetTextureColor(elementColors[element]);
            }

            return estilo;
        }
        private GUIStyle GetFieldStyle(bool isPanelOn)
        {
            GUIStyle estilo = new GUIStyle(GUI.skin.button);
            if (isPanelOn)
            {
                estilo.normal.background = GetTextureColor(elementColors[ELEMENT_TYPE.EMPTY]);
            }

            return estilo;
        }
        private Texture2D GetTextureColor(Color color)
        {
            Texture2D textura = new Texture2D(1, 1);
            textura.SetPixel(0, 0, color);
            textura.Apply();
            return textura;
        }
        private void ResetMatrix()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (isFieldSelected)
                    {
                        levelData.tileList[i * 9 + j].isAvailable = true;
                    }
                    else
                    {
                        if (levelData.tileList[i * 9 + j].isAvailable)
                        {
                            levelData.tileList[i * 9 + j].element = ELEMENT_TYPE.EMPTY;
                        }
                    }
                }
            }
            actualSelectedElement = ELEMENT_TYPE.water;
        }
        private void RefillMatrix()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    levelData.tileList[i * 9 + j].isAvailable = true;
                }
            }
        }
        private void EmptyMatrix()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    levelData.tileList[i * 9 + j].isAvailable = false;
                }
            }
        }
        private void SetMatrixInBlank()
        {
            levelData.tileList = new List<Tile>();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Tile newTile = new Tile();
                    newTile.isAvailable = true;
                    newTile.element = ELEMENT_TYPE.EMPTY;
                    newTile.gridPos.x = j;
                    newTile.gridPos.y = i;
                    levelData.tileList.Add(newTile);
                }
            }
            actualSelectedElement = ELEMENT_TYPE.water;
        }
        #endregion
    }

}

#endif