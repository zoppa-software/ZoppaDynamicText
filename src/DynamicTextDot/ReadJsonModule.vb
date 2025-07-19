Option Strict On
Option Explicit On

Imports System.Text.Json
Imports ZoppaDynamicText.Analysis

''' <summary>
''' JSON文字列を解析してDynamicObjectに変換するモジュールです。
''' 
''' このモジュールは、JSON形式の文字列を解析し、DynamicObjectに変換する機能を提供します。
''' DynamicObjectは、プロパティとしてJSONのキーと値を持つオブジェクトであり、柔軟なデータ構造を提供します。
''' </summary>
Module ReadJsonModule

    ''' <summary>
    ''' JSON文字列を解析してDynamicObjectに変換します。
    ''' </summary>
    ''' <param name="jsonText">JSON形式の文字列</param>
    ''' <returns>DynamicObjectに変換された結果</returns>
    ''' <remarks>
    ''' この関数は、JSON文字列を解析し、DynamicObjectに変換します。
    ''' DynamicObjectは、プロパティとしてJSONのキーと値を持つオブジェクトです。
    ''' </remarks>
    Public Function ConvertDynamicObjectFromJson(jsonText As String) As DynamicObject
        ' JSON文字列を解析してJsonDocumentを作成します。
        Using document As JsonDocument = JsonDocument.Parse(jsonText)
            Dim root As JsonElement = document.RootElement
            Return CType(ParseJson(root), DynamicObject)
        End Using
    End Function

    ''' <summary>JSON要素を解析してオブジェクトに変換します。</summary>
    ''' <param name="element">解析するJsonElement</param>
    ''' <returns>変換された結果</returns>
    Private Function ParseJson(element As JsonElement) As Object
        Select Case element.ValueKind
            Case JsonValueKind.Array
                Dim res As New ArrayList()
                For Each item As JsonElement In element.EnumerateArray()
                    res.Add(ParseJson(item))
                Next
                Return res
            Case JsonValueKind.Object
                Dim res As New DynamicObject()
                For Each prop As JsonProperty In element.EnumerateObject
                    res(prop.Name) = ParseJson(prop.Value)
                Next
                Return res
            Case JsonValueKind.String
                Return element.GetString()
            Case JsonValueKind.Number
                Return element.GetDouble()
            Case JsonValueKind.True
                Return True
            Case JsonValueKind.False
                Return False
            Case JsonValueKind.Null
                Return Nothing
            Case Else
                Return Nothing
        End Select
    End Function

End Module
