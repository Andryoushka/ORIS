using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MiniHttpServer.Framework.share;

public class HtmlTemplateRenderer : IHtmlTemplateRenderer
{
    private Dictionary<string, object> Objects;
    //public Dictionary<string, object> Objects = new();

    public string RenderFromFile(string filePath, Dictionary<string,object> dataModel)
    {
        Objects = dataModel;
        var html = File.ReadAllText(filePath);
        var template = RenderFromString(html, Objects);
        return template;
    }

    public string RenderFromString(string htmlTemplate, Dictionary<string, object> dataModel)
    {
        try
        {
            Objects = dataModel;
            var htmlRender = Render(htmlTemplate);
            return htmlRender;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public string RenderToFile(string inputFilePath, string outputFilePath, Dictionary<string, object> dataModel)
    {
        Objects = dataModel;
        var html = File.ReadAllText(inputFilePath);
        var template = RenderFromString(html, Objects);
        File.WriteAllText(outputFilePath, template);
        return template;
    }

#region RENDER
    static public string FindEndOfRenderConstruction(Match construction, string template)
    {
        var endsConstruction = new string[] { @"\G\$endfor", @"\G\$endif", @"\G\$if", @"\G\$foreach"};

        var startSteck = new Stack<Match>();

        startSteck.Push(construction);

        var strB = new StringBuilder();
        strB.Append(construction.Value);
        var index = construction.Index + construction.Length;
        while (startSteck.Count != 0) // ? index
        {
            var c = template[index];
            // Ищем вложенности
            if (c != '$')
            {
                strB.Append(template[index++]);
                continue;
            }
            
            foreach (var matchPattern in endsConstruction)
            {
                var regex = new Regex(matchPattern);
                var match = regex.Match(template, index);
                if (match.Success)
                {
                    if (match.Value == "$if" || match.Value == "$foreach")
                    {
                        startSteck.Push(match);
                        strB.Append(match.Value);
                        index += match.Length;
                        break;
                    }
                    if (match.Value == "$endif")
                    {
                        var start = startSteck.Pop();
                        if (start.Value != "$if")
                            throw new Exception("Нарушена вложенность!");
                        strB.Append(match.Value);
                        index += match.Length;
                        break;
                    }
                    if (match.Value == "$endfor")
                    {
                        var start = startSteck.Pop();
                        if (start.Value != "$foreach")
                            throw new Exception("Нарушена вложенность!");
                        strB.Append(match.Value);
                        index += match.Length;
                        break;
                    }
                }
            }
            if (index < template.Length && template[index] == '$') // 2
                strB.Append(template[index++]);
        }
        return strB.ToString();
    }

    private string Render(string htmlTemplate)
    {
        var strB = new StringBuilder();
        var startWith = new string[3] { @"\G\${", @"\G\$if", @"\G\$foreach" };
        var index = 0;
        while (index < htmlTemplate.Length)
        {
            if (htmlTemplate[index] != '$')
            {
                strB.Append(htmlTemplate[index]);
                index++;
            }
            else
            {
                Match patternMatch = null;
                foreach (var matchPattern in startWith)
                {
                    var patternRegex = new Regex(matchPattern);
                    patternMatch = patternRegex.Match(htmlTemplate, index);
                    if (patternMatch.Success)
                        break;
                }

                Regex regex = null;
                Match match = null;
                string template = null;
                switch (patternMatch.Value)
                {
                    case "${":
                        regex = new Regex(@"\${(?<Prop>[^}]+)}");
                        match = regex.Match(htmlTemplate, index);
                        strB.Append(PropRender(match));
                        index += match.Length;
                        break;

                    case "$foreach":
                        regex = new Regex(@"\$foreach\(var (?<Item>.+) in (?<Collection>.+)\)\r*\n*(?<Content>[\D0-9]*)\r*\n*\$endfor");
                        template = FindEndOfRenderConstruction(patternMatch, htmlTemplate);
                        match = regex.Match(template/*, index*/);// index problem
                        strB.Append(ForeachRender(match));
                        index += match.Length;
                        break;

                    case "$if": // поменять логику
                        // \$if\((?<Statement>.+)\)\r*\n*(?<Content>[\s\S]*)\r*\n*\$endif
                        regex = new Regex(@"\$if\((?<Statement>.+)\)\r*\n*(?<Content>[\s\S]*)\r*\n*\$endif");
                        template = FindEndOfRenderConstruction(patternMatch, htmlTemplate);
                        match = regex.Match(template/*, index*/);
                        strB.Append(IfRender(match));
                        index += match.Length;
                        break;

                    default:
                        // добавить и продолжить рендер
                        break;
                }
            }
        }
        return strB.ToString();
    }

    private bool IfEvaluate(Match statement)
    {
        var expression = statement.Groups["Statement"].Value.Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);
        
        if (expression.Length == 1)
        {
            var obj = expression[0].Trim();
            object value = null;
            if (obj.Contains('!'))
            {
                obj = obj.TrimStart('!');
                value = GetObjectByReflection(obj.Split('.'));
                return !((bool)value);
            }
            return (bool)GetObjectByReflection(obj.Split('.'));
        }

        var obj1 = expression[0].TrimStart('(');
        var condition = expression[1];
        var obj2 = expression[2].TrimEnd(')');

        var leftNegation = false;
        if (obj1.Contains('!'))
        {
            leftNegation = true;
            obj1 = obj1.TrimStart('!');
        }

        var rightNegation = false;
        if (obj2.Contains('!'))
        {
            rightNegation = true;
            obj2 = obj2.TrimStart('!');
        }

        var leftObj = GetObjectByReflection(obj1.Split('.'));
        var rightObj = GetObjectByReflection(obj2.Split('.'));

        if (leftNegation)
            leftObj = !((bool)leftObj);

        if (rightNegation)
            rightNegation = !((bool)rightNegation);

        // равные типы для сравнения
        var leftType = leftObj.GetType();
        rightObj = Convert.ChangeType(rightObj, leftType);

        var result = false;
        if (leftObj is IComparable left && rightObj is IComparable right)
        {
            //var leftType = left.GetType();
            //right = Convert.ChangeType(right, leftType);
            switch (condition)
            {
                case "==":
                    result = left.Equals(right);
                    break;

                case "!=":
                    result = !left.Equals(right);
                    break;

                case ">=":
                    result = left.CompareTo(right) >= 0;
                    break;

                case "<=":
                    result = left.CompareTo(right) <= 0;
                    break;

                case ">":
                    result = left.CompareTo(right) > 0;
                    break;

                case "<":
                    result = left.CompareTo(right) < 0;
                    break;

                default:
                    throw new Exception("Неизвестное условие.");
                    break;
            }
        }

        return result;
    }

    private string IfRender(Match htmlPart)
    {
        try { 
        var strB = new StringBuilder();
        //var statementGroup = htmlPart.Groups["Statement"].Value.Split(".");
        //var objName = statementGroup[0];

        var statement = IfEvaluate(htmlPart)/*GetObjectByReflection(statementGroup)*/;

        if (statement == null)
            return htmlPart.Value;
        //
        var ifCount = 1;
        var elseCount = 0;
        var template = htmlPart.Groups["Content"].Value + "$endif";
        var index = 0;
        var falseRender = false;

        var startWith = new string[3] { @"\G\$if", @"\G\$else", @"\G\$endif" };

        while (index < template.Length)
        {
            if (template[index] != '$')
                strB.Append(template[index++]);
            else
            {
                Match patternMath = null;
                foreach (var pattern in startWith)
                {
                    var patternRegex = new Regex(pattern);
                    patternMath = patternRegex.Match(template, index);

                    if (patternMath.Value == "$if")
                    {
                        ifCount++;
                        break;
                    }
                    if (patternMath.Value == "$else")
                    {
                        elseCount++;
                        break;
                    }
                    if (patternMath.Value == "$endif")
                    {
                        if (elseCount > 0)
                        {
                            ifCount--;
                            elseCount--;
                        }
                        else
                            ifCount--;
                        break;
                    }
                }

                // проверка потом добавление
                if (ifCount == 0 && elseCount == 0)
                {
                    // all
                    if ((bool)statement && !falseRender)
                        return Render(strB.ToString().TrimEnd());
                    else if (!(bool)statement && falseRender)
                        return Render(strB.ToString().TrimEnd());
                    else
                        return string.Empty;
                }

                if (elseCount == 1 && ifCount == 1)
                {
                    // true - false
                    if ((bool)statement)
                    {
                        return Render(strB.ToString().TrimEnd());
                    }
                    else
                    {
                        if (!falseRender)
                        {
                            strB.Clear();
                            index += patternMath.Length;
                            falseRender = true;
                            continue;
                        }
                        else
                        {
                            strB.Append(patternMath.Value);
                            return Render(strB.ToString().TrimEnd());
                        }
                    }
                }

                if (patternMath.Success)
                {
                    strB.Append(patternMath.Value);
                    index += patternMath.Length;
                }
                else
                    strB.Append(template[index++]);
            }
        }
        //
        /*var hasElse = htmlPart.Groups.ContainsKey("False");

        if ((bool)statement)
        {
            strB.Append(Render(htmlPart.Groups["True"].Value));
        }
        else
        {
            if (hasElse)
            {
                strB.Append(Render(htmlPart.Groups["False"].Value));
            }
        }

        return strB.ToString();*/
        return strB.ToString();
    }
    catch (Exception ex)
    {

    }
        return null;
    }

    private string ForeachRender(Match htmlPart)
    {
        var strB = new StringBuilder();
        var collectionName = htmlPart.Groups["Collection"].Value.Split(".");
        // get collection
        if (!Objects.ContainsKey(collectionName[0]))
            return htmlPart.Value;
        var collection = GetObjectByReflection(collectionName);

        if (collection == null)
            return htmlPart.Value;

        var itemName = htmlPart.Groups["Item"].Value;
        Objects.Add(itemName, null); // add
        if (collection is IEnumerable c)
        {
            foreach (var item in c)
            {
                Objects[itemName] = item; // update
                // DO RENDER
                strB.Append(Render(htmlPart.Groups["Content"].Value));
                //strB.Append("\r\n");
            }
            //strB.Remove(strB.Length-2,2);
            Objects.Remove(itemName); // delete
        }

        return strB.ToString().TrimEnd();
    }

    private string PropRender(Match htmlPart)
    {
        // подаётся регекс
        var propGroup = htmlPart.Groups["Prop"].Value.Split('.');
        // ищем объект в словаре (exсeptions)
        if (!Objects.ContainsKey(propGroup[0]))
            return htmlPart.Value;

        var obj = GetObjectByReflection(propGroup);

        if (obj == null)
            return htmlPart.Value;

        return obj.ToString();
    }

    private object GetObjectByReflection(string[] propGroup)
    {
        if (!Objects.ContainsKey(propGroup[0]))
            return propGroup[0].TrimStart('"').TrimEnd('"'); // Не нашел => верни что было

        var obj = Objects[propGroup[0]];
        // ищем нужное свойство
        if (propGroup.Length == 1)
        {
            return obj;
        }

        try
        {
            var prop = new object();
            for (int i = 1; i < propGroup.Length; i++)
            {
                var objType = obj.GetType();
                prop = objType.GetProperty(propGroup[i]).GetValue(obj);
                obj = prop;
            }
        }
        catch (Exception nullEx)
        {
            Logger.PrintError("Ошибка шаблонизатора : не может найти тип через рефлексию");
            return null;
        }
        return obj;
    }
#endregion

}

public interface IHtmlTemplateRenderer
{
    string RenderFromString(string htmlTemplate, Dictionary<string, object> dataModel);

    string RenderFromFile(string filePath, Dictionary<string, object> dataModel);

    string RenderToFile(string inputFilePath, string outputFilePath, Dictionary<string, object> dataModel);
};