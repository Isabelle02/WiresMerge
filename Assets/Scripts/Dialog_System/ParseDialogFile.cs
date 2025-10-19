using UnityEngine;
using System.Xml;

public static class ParseDialogFile
{
    static public DialogNode GetDialogTree(TextAsset textAsset)
    {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(textAsset.text);
        XmlNode rootDialogNode = xmlDocument.SelectSingleNode("/dialog");
        return CreateDialogTree(rootDialogNode);
    }

    static private DialogNode CreateDialogTree(XmlNode dialogNode)
    {
        DialogNode node = new DialogNode();

        // Безопасное получение name
        XmlNode nameNode = dialogNode.SelectSingleNode("name");
        node.Name = nameNode != null ? nameNode.InnerText : "";

        // Безопасное получение message
        XmlNode messageNode = dialogNode.SelectSingleNode("message");
        node.Message = messageNode != null ? messageNode.InnerText : "";

        // Безопасное получение answer
        XmlNode answerNode = dialogNode.SelectSingleNode("answer");
        if (answerNode != null)
        {
            node.Answer = answerNode.InnerText;
        }

        // Безопасное получение action
        XmlNode actionNode = dialogNode.SelectSingleNode("action");
        if (actionNode != null && actionNode.Attributes["id"] != null)
        {
            node.ActionId = int.Parse(actionNode.Attributes["id"].Value);
        }

        // Рекурсивная обработка дочерних диалогов
        foreach (XmlNode childNode in dialogNode.SelectNodes("dialog"))
        {
            DialogNode child = CreateDialogTree(childNode);

            // Добавляем только если у ребенка есть хотя бы name или message
            if (!string.IsNullOrEmpty(child.Name) || !string.IsNullOrEmpty(child.Message))
            {
                node.Children.Add(child);
            }
        }

        return node;
    }

    //static private DialogNode CreateDialogTree(XmlNode dialogNode)
    //{
    //    DialogNode node = new DialogNode();

    //    node.Name = dialogNode.SelectSingleNode("name").InnerText;
    //    node.Message = dialogNode.SelectSingleNode("message").InnerText;
    //    XmlNode actionNode = dialogNode.SelectSingleNode("action");
    //    if (actionNode != null)
    //    {
    //        node.ActionId = int.Parse(actionNode.Attributes["id"].Value);
    //    }
    //    XmlNode answerNode = dialogNode.SelectSingleNode("answer");
    //    if (answerNode != null)
    //    {
    //        node.Answer = answerNode.InnerText;
    //    }

    //    foreach (XmlNode childNode in dialogNode.SelectNodes("dialog"))
    //    {
    //        DialogNode child = CreateDialogTree(childNode);
    //        node.Children.Add(child);
    //    }

    //    return node;
    //}
}
