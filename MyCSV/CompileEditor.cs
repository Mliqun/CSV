using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Diagnostics;

public class CompileEditor : EditorWindow
{
    [MenuItem("Tools/ReflectConfigToCS")]
    public static void ConfigToCS()
    {
        CompileEditor rW = EditorWindow.GetWindow<CompileEditor>("�����ļ�ת����class");
       
    }
    private string JsonFileName;
    private string PathFileName;
    string biaoti = @"/************************************************************************
���ļ���ͨ���Զ����ɵģ���ֹ�ֶ��޸�
���ߣ�
���ڣ�#1
*************************************************************************/";
    private void OnGUI()
    {
        GUILayout.Label("��ܰС��ʾ����ǰ�ؿ���");
        GUILayout.Label("1.Ҫ���ɵ�CSV�ļ��ı����ʽ����ΪUTF-8��ʽ");
        GUILayout.Label("2.��ȷ��Ҫ������CSV�ļ��ڲ�����Ӣ�ķ��ŵĶ���");
        GUILayout.Label("���������ɴ�������ƣ�����ĸ��д��");
        JsonFileName = EditorGUILayout.TextField(JsonFileName);
        if (GUILayout.Button("��ѡ��һ�������ļ�"))
        {
            string selectFileStr = EditorUtility.OpenFilePanel("", Application.dataPath, "csv");
            ReaderConfigfile(selectFileStr);
        }
    }
    /// <summary>
    /// ��ȡ�����ļ�
    /// </summary>
    /// <param name="path">�ļ�·��</param>
    private void ReaderConfigfile(string path)
    {
        string[] fileStr = File.ReadAllLines(path);
        PathFileName = path;
        CreateCS(fileStr);
    }

    /// <summary>
    /// ����C#�ļ�
    /// </summary>
    /// <param name="reflectFileName">�����CS���ļ���</param>
    private void CreateCS(string[] reflectFileName)
    {
        //д������·��λ���봴�����ļ�д����
        string CSPath = $"{Application.dataPath + "/Resources"}/{JsonFileName}.cs";
        StreamWriter sw = new StreamWriter(CSPath);
        //����һЩд��ĸ�ʽ�������
        //д�������\Ϊ���з�\t==tab
        string tabKey = "\t";
        //��������
        string[] argumentType = reflectFileName[1].ToLower().Split(',');
        //��������
        string[] argumentName = reflectFileName[0].Split(',');
        string time = DateTime.Now.ToString();
        sw.WriteLine(biaoti.Replace("#1", time));
        sw.WriteLine(GetImport());
        //��ʽ���������ļ��￪ʼд���������
        sw.WriteLine($"public class {JsonFileName}");
        sw.WriteLine("{");
        //���������б���������
        for (int i = 0; i < argumentType.Length; i++)
        {
            sw.WriteLine($"{tabKey}public {argumentType[i]} {argumentName[i]};");
        }
        sw.WriteLine("}");
        //���ɽ���CSV�ļ�����
        sw.WriteLine($"public class JsonToCsv{JsonFileName}");
        sw.WriteLine("{");
        sw.WriteLine($"{tabKey}public List<{JsonFileName}> {JsonFileName}_list = new List<{JsonFileName}>();");
        sw.WriteLine($"{tabKey}public JsonToCsv{JsonFileName}()");
        sw.WriteLine($"{tabKey}" + "{");
        sw.WriteLine($"{tabKey}{tabKey}JsonToCsvOpen();");
        sw.WriteLine($"{tabKey}" + "}");
        sw.WriteLine($"{tabKey}public void JsonToCsvOpen()");
        sw.WriteLine($"{tabKey}" + "{");
        sw.WriteLine($"{tabKey}{tabKey}string json = \"{PathFileName}\";");
        sw.WriteLine($"{tabKey}{tabKey}string[] fileStr = File.ReadAllLines(json);");
        sw.WriteLine($"{tabKey}{tabKey}for (int i = 2; i < fileStr.Length; i++)" + "{");
        sw.WriteLine($"{tabKey}{tabKey}{tabKey}string[] list_open = fileStr[i].Split(',');");
        sw.WriteLine($"{tabKey}{tabKey}{tabKey}{JsonFileName} jsons = new {JsonFileName}();");
        for (int i = 0; i < argumentType.Length; i++)
        {
            //��ǰ��ͬ���ͱ�ͷ���岻ͬ����
            if (argumentType[i] == "int")
            {
                sw.WriteLine($"{tabKey}{tabKey}{tabKey}jsons.{argumentName[i]} = int.Parse(list_open[{i}]);");
            }
            else if (argumentType[i] == "string")
            {
                sw.WriteLine($"{tabKey}{tabKey}{tabKey}jsons.{argumentName[i]} = list_open[{i}];");
            }
            else if (argumentType[i] == "float")
            {
                sw.WriteLine($"{tabKey}{tabKey}{tabKey}jsons.{argumentName[i]} = float.Parse(list_open[{i}]);");
            }
        }
        sw.WriteLine($"{tabKey}{tabKey}{tabKey}{JsonFileName}_list.Add(jsons);" + "}");
        sw.WriteLine($"{tabKey}" + "}");
        //���ɵ������ݲ����ؼ���
        sw.WriteLine($"{tabKey}public List<{JsonFileName}> data()" + "{");
        sw.WriteLine($"{tabKey}{tabKey}JsonToCsvOpen();");
        sw.WriteLine($"{tabKey}return {JsonFileName}_list;" + "}");

        //����idѰ������  �涨��һ��Ϊ�ҵ�id
        sw.WriteLine($"{tabKey}public {JsonFileName} GetDataByID(int id)");
        sw.WriteLine($"{tabKey}" + "{");
        sw.WriteLine($"{tabKey}{tabKey}foreach (var item in {JsonFileName}_list)");
        sw.WriteLine($"{tabKey}{tabKey}" + "{");
        sw.WriteLine($"{tabKey}{tabKey}if (item.{argumentName[0]}==id)");
        sw.WriteLine($"{tabKey}{tabKey}{tabKey}" + "{");
        sw.WriteLine($"{tabKey}{tabKey}{tabKey}return item;");
        sw.WriteLine($"{tabKey}{tabKey}{tabKey}" + "}");
        sw.WriteLine($"{tabKey}{tabKey}" + "}");
        sw.WriteLine($"{tabKey}{tabKey}return null;");
        sw.WriteLine("}");

        //����idɾ������
        sw.WriteLine($"{tabKey}public void RemoveDataByID(int id)");
        sw.WriteLine($"{tabKey}" + "{");
        sw.WriteLine($"{tabKey}{tabKey}foreach (var item in {JsonFileName}_list)");
        sw.WriteLine($"{tabKey}{tabKey}" + "{");
        sw.WriteLine($"{tabKey}{tabKey}{tabKey}if (item.{argumentName[0]}==id)");
        sw.WriteLine($"{tabKey}{tabKey}{tabKey}" + "{");
        sw.WriteLine($"{tabKey}{tabKey}{tabKey}{tabKey}{JsonFileName}_list.Remove(item);");
        sw.WriteLine($"{tabKey}{tabKey}{tabKey}{tabKey}break;");
        sw.WriteLine($"{tabKey}{tabKey}{tabKey}" + "}");
        sw.WriteLine($"{tabKey}{tabKey}" + "}");
        sw.WriteLine($"{tabKey}" + "}");

        //�������
        sw.WriteLine($"{tabKey}public void AddData({JsonFileName} data)");
        sw.WriteLine($"{tabKey}" + "{");
        sw.WriteLine($"{tabKey}{tabKey}if ({JsonFileName}_list.Contains(data))");
        sw.WriteLine($"{tabKey}{tabKey}" + "{");
        sw.WriteLine($"{tabKey}{tabKey}{tabKey}return;");
        sw.WriteLine($"{tabKey}{tabKey}" + "}");
        sw.WriteLine($"{tabKey}{tabKey}{JsonFileName}_list.Add(data);");
        sw.WriteLine($"{tabKey}" + "}");


        //����id��ȡ������  Ȼ���޸�����
        sw.WriteLine($"{tabKey}public void ChangeData({JsonFileName} data)");
        sw.WriteLine($"{tabKey}" + "{");
        sw.WriteLine($"{tabKey}{tabKey}for (int i = 0; i < {JsonFileName}_list.Count; i++)");
        sw.WriteLine($"{tabKey}{tabKey}" + "{");
        sw.WriteLine($"{tabKey}{tabKey}{tabKey}if ({JsonFileName}_list[i].Index== data.Index)");
        sw.WriteLine($"{tabKey}{tabKey}{tabKey}"+"{");
        sw.WriteLine($"{tabKey}{tabKey}{tabKey}{tabKey}{JsonFileName}_list[i] = data;");
        sw.WriteLine($"{tabKey}{tabKey}{tabKey}{tabKey}break;");

        sw.WriteLine($"{tabKey}{tabKey}{tabKey}"+"}");

        sw.WriteLine($"{tabKey}{tabKey}" + "}");
        sw.WriteLine($"{tabKey}{tabKey}{JsonFileName}_list.Add(data);");
        sw.WriteLine($"{tabKey}" + "}");

        sw.WriteLine("}");

        sw.Flush();
        sw.Close();
        AssetDatabase.Refresh();
        Process.Start(CSPath);
    }
    /// <summary>
    /// ���ص������� 
    /// </summary>
    /// <returns></returns>
    public string GetImport()
    {
        string importStr = null;
        importStr += $"using UnityEngine;\r\n";
        importStr += $"using UnityEngine.UI;\r\n";
        importStr += $"using System;\r\n";
        importStr += $"using System.Collections;\r\n";
        importStr += $"using UnityEditor;\r\n";
        importStr += $"using System.IO;\r\n";
        importStr += $"using System.Collections.Generic;\r\n";
        return importStr;
    }
}
