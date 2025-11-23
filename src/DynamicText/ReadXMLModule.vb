Option Strict On
Option Explicit On

Imports System.Xml
Imports ZoppaDynamicText.Analysis

''' <summary>
''' XMLを読み込み、DynamicObjectに変換するモジュール。
''' </summary>
Module ReadXMLModule

    ''' <summary>
    ''' XMLテキストをDynamicObjectに変換します。
    ''' </summary>
    ''' <param name="xmlText">XML形式の文字列。</param>
    ''' <returns>変換されたDynamicObject。</returns>
    ''' <remarks>
    ''' XMLのルート要素がDynamicObjectのトップレベルプロパティとなります。
    ''' 各要素は再帰的にDynamicObjectに変換され、属性はプロパティとして追加されます。
    ''' テキストノードは特別なプロパティ名 "XMLValueText" に格納されます。
    ''' </remarks>
    Public Function ConvertDynamicObjectFromXML(xmlText As String) As DynamicObject
        ' XMLドキュメントを読み込みます
        Dim doc As New XmlDocument
        doc.LoadXml(xmlText)

        ' ルート要素をDynamicObjectに変換します
        Dim res As New DynamicObject()
        res(doc.LastChild.Name) = ParseXML(doc.LastChild)
        Return res
    End Function

    ''' <summary>
    ''' XMLノードをDynamicObjectに変換します。
    ''' </summary>
    ''' <param name="node">変換するXMLノード。</param>
    ''' <returns>変換されたDynamicObject。</returns>
    ''' <remarks>
    ''' この関数は再帰的に呼び出され、子要素もDynamicObjectに変換されます。
    ''' 属性はプロパティとして追加され、テキストノードは特別なプロパティ名 "XMLValueText" に格納されます。
    ''' </remarks>
    Private Function ParseXML(node As XmlNode) As DynamicObject
        Dim res As New DynamicObject()

        ' 属性をプロパティとして追加
        For Each attr As XmlAttribute In node.Attributes
            res(attr.Name) = attr.Value
        Next

        If node.ChildNodes.Count > 0 Then
            ' 子要素の格納用ディクショナリ
            Dim children As New Dictionary(Of String, ArrayList)()

            ' 子ノードを処理
            For Each item As XmlNode In node.ChildNodes
                Select Case item.NodeType
                    Case XmlNodeType.Element
                        ' 要素ノードの場合、再帰的に処理
                        Dim child = ParseXML(item)
                        If Not child.IsEmpty Then

                            Dim value As ArrayList = Nothing
                            If Not children.TryGetValue(item.Name, value) Then
                                value = New ArrayList()
                                children(item.Name) = value
                            End If

                            value.Add(child)
                        End If

                    Case XmlNodeType.Text
                        ' テキストノードの場合、そのまま値として設定
                        res(DynamicObject.DEFAULT_TEXT_PROPERTY_NAME) = item.Value

                    Case XmlNodeType.CDATA
                        ' CDATAセクションの場合、そのまま値として設定
                        res(DynamicObject.DEFAULT_TEXT_PROPERTY_NAME) = item.Value
                End Select
            Next

            ' 子要素をプロパティとして追加
            '
            ' 1. 1つだけの場合はそのまま格納
            ' 2. 同じ名前の要素が複数ある場合は配列として格納
            For Each key In children.Keys
                If children(key).Count = 1 Then
                    res(key) = children(key)(0) ' 1
                Else
                    res(key) = children(key) ' 2
                End If
            Next
        End If

        Return res
    End Function

End Module
