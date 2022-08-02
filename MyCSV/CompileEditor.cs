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
        CompileEditor rW = EditorWindow.GetWindow<CompileEditor>("配置文件转换成class");
       
    }
    private string JsonFileName;
    private string PathFileName;
    string biaoti = @"/************************************************************************
该文件是通过自动生成的，禁止手动修改
作者：
日期：#1
*************************************************************************/";
    private void OnGUI()
    {
        GUILayout.Label("温馨小提示，用前必看：");
        GUILayout.Label("1.要生成的CSV文件的编码格式必须为UTF-8格式");
        GUILayout.Label("2.请确保要解析的CSV文件内不存在英文符号的逗号");
        GUILayout.Label("请输入生成代码的名称（首字母大写）");
        JsonFileName = EditorGUILayout.TextField(JsonFileName);
        if (GUILayout.Button("请选择一个配置文件"))
        {
            string selectFileStr = EditorUtility.OpenFilePanel("", Application.dataPath, "csv");
            ReaderConfigfile(selectFileStr);
        }
    }
    /// <summary>
    /// 读取配置文件
    /// </summary>
    /// <param name="path">文件路径</param>
    private void ReaderConfigfile(string path)
    {
        string[] fileStr = File.ReadAllLines(path);
        PathFileName = path;
        CreateCS(fileStr);
    }

    /// <summary>
    /// 创建C#文件
    /// </summary>
    /// <param name="reflectFileName">反射成CS的文件名</param>
    private void CreateCS(string[] reflectFileName)
    {
        //写入配置路径位置与创建的文件写入流
        string CSPath = $"{Application.dataPath + "/Resources"}/{JsonFileName}.cs";
        StreamWriter sw = new StreamWriter(CSPath);
        //设置一些写入的格式符与变量
        //写入的行以\为换行符\t==tab
        string tabKey = "\t";
        //参数类型
        string[] argumentType = reflectFileName[1].ToLower().Split(',');
        //参数名称
        string[] argumentName = reflectFileName[0].Split(',');
        string time = DateTime.Now.ToString();
        sw.WriteLine(biaoti.Replace("#1", time));
        sw.WriteLine(GetImport());
        //正式在配置流文件里开始写入代码配置
        sw.WriteLine($"public class {JsonFileName}");
        sw.WriteLine("{");
        //遍历参数列表，生成配置
        for (int i = 0; i < argumentType.Length; i++)
        {
            sw.WriteLine($"{tabKey}public {argumentType[i]} {argumentName[i]};");
        }
        sw.WriteLine("}");
        //生成解析CSV文件函数
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
            //当前不同类型表头定义不同类型
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
        //生成调用数据并返回集合
        sw.WriteLine($"{tabKey}public List<{JsonFileName}> data()" + "{");
        sw.WriteLine($"{tabKey}{tabKey}JsonToCsvOpen();");
        sw.WriteLine($"{tabKey}return {JsonFileName}_list;" + "}");

        //根据id寻找数据  规定第一条为我的id
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

        //根据id删除数据
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

        //添加数据
        sw.WriteLine($"{tabKey}public void AddData({JsonFileName} data)");
        sw.WriteLine($"{tabKey}" + "{");
        sw.WriteLine($"{tabKey}{tabKey}if ({JsonFileName}_list.Contains(data))");
        sw.WriteLine($"{tabKey}{tabKey}" + "{");
        sw.WriteLine($"{tabKey}{tabKey}{tabKey}return;");
        sw.WriteLine($"{tabKey}{tabKey}" + "}");
        sw.WriteLine($"{tabKey}{tabKey}{JsonFileName}_list.Add(data);");
        sw.WriteLine($"{tabKey}" + "}");


        //根据id获取到数据  然后修改数据
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
    /// 加载调用数据 
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
