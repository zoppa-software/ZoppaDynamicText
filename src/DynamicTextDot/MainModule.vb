﻿Option Strict On
Option Explicit On

Imports System.IO
Imports System.Runtime.CompilerServices
Imports ZoppaDynamicText.Analysis
Imports ZoppaDynamicText.LegacyFiles
Imports ZoppaDynamicText.Switches

''' <summary>
''' メインモジュール。
''' </summary>
Module MainModule

    ''' <summary>
    ''' エントリポイントです。
    ''' コマンドライン引数を解析し、埋込式を処理します。
    ''' 
    ''' 使用方法:
    ''' DynamicText.exe --template テンプレートファイルパス --param パラメータファイルパス [--output 出力ファイルパス] [--encode エンコード]
    ''' </summary>
    Sub Main()
        Dim args = New String(Environment.GetCommandLineArgs().Length - 2) {}
        System.Array.Copy(Environment.GetCommandLineArgs(), 1, args, 0, Environment.GetCommandLineArgs().Length - 1)

        Try
            ' コマンドライン引数を解析するためのAnalysisSwitchを定義します。
            Dim switchDefine As AnalysisSwitch = AnalysisSwitch.Create(
                appDescription:="埋込式を解析します。",
                appAuthor:="zoppa software",
                appLicense:="apache 2.0",
                subCommandRequired:=False,
                subCommand:=New SubCommandDefine() {},
                options:=New SwitchDefine() {
                    New SwitchDefine("help", False, SwitchType.DoubleHyphen, ParameterType.None, "ヘルプを表示します。"),
                    New SwitchDefine("template", True, SwitchType.DoubleHyphen, ParameterType.URI, "埋込式を記述したテンプレートファイルパス。"),
                    New SwitchDefine("param", True, SwitchType.DoubleHyphen, ParameterType.URI, "パラメータファイルパス。"),
                    New SwitchDefine("encode", False, SwitchType.DoubleHyphen, ParameterType.Str, "ファイルエンコード、未指定の場合は UTF-8。"),
                    New SwitchDefine("output", False, SwitchType.DoubleHyphen, ParameterType.URI, "出力ファイルパス。")
                }
            )

            ' コマンドライン引数を解析します。
            Dim analysisSwitches = switchDefine.Parse(args)
            If analysisSwitches.ContainsOption("help") Then
                System.Console.Out.WriteLine(switchDefine.GetHelp())
            End If

            ' パラメータチェック
            analysisSwitches.CheckRequired()

            ' エンコードの設定を取得します。
            Dim encode As Text.Encoding = Text.Encoding.UTF8
            If analysisSwitches.ContainsOption("encode") Then
                Dim encodeStr = analysisSwitches.GetOption("encode").GetStr()
                Try
                    encode = Text.Encoding.GetEncoding(encodeStr)
                Catch ex As Exception
                    Throw New ArgumentException($"指定されたエンコード '{encodeStr}' は無効です。", ex)
                End Try
            End If

            ' テンプレートファイルのパスを取得します。
            Dim tempPath = analysisSwitches.GetOption("template").GetURI().ConvertAbsolutePath()
            If Not System.IO.File.Exists(tempPath) Then
                Throw New System.IO.FileNotFoundException("テンプレートファイルが見つかりません。", tempPath)
            End If
            Dim tempStr As String = System.IO.File.ReadAllText(tempPath, encode)

            ' パラメータファイルのパスを取得します。
            Dim paramPath = analysisSwitches.GetOption("param").GetURI().ConvertAbsolutePath()
            If Not System.IO.File.Exists(paramPath) Then
                Throw New System.IO.FileNotFoundException("パラメータファイルが見つかりません。", paramPath)
            End If
            Dim paramStr As String = System.IO.File.ReadAllText(paramPath, encode)

            ' 解析環境を初期化し、オブジェクトを登録します。
            Dim env As New AnalysisEnvironment()
            Dim result As AnalysisResults
            Select Case Path.GetExtension(paramPath).ToLowerInvariant()
                Case ".json"
                    ' JSONファイルの内容を解析して環境に登録します。
                    Dim jsonObject = ReadJsonModule.ConvertDynamicObjectFromJson(paramStr)
                    env.SetJsonVariable(jsonObject)
                    result = ParserModule.Translate(tempStr)

                Case ".csv"
                    ' CSVファイルの内容を解析して環境に登録します。
                    Dim spliter = CsvSpliter.CreateSpliter(paramStr)
                    env.SetCsvVariable(spliter, paramPath)
                    result = ParserModule.Translate(tempStr)

                Case Else
                    ' パラメータファイルの内容を解析して環境に登録します。
                    result = ParserModule.Translate($"${{{paramStr}}}" & tempStr)
            End Select

            ' 解析結果から文字列を取得します。
            Dim resultStr = result.Expression.GetValue(env).Str.ToString()

            ' 結果を出力します。
            If analysisSwitches.ContainsOption("output") Then
                ' 出力結果を指定されたファイルに書き込みます。
                Dim outputPath = analysisSwitches.GetOption("output").GetURI().ConvertAbsolutePath()
                Dim outputDir = System.IO.Path.GetDirectoryName(outputPath)
                If Not IO.Directory.Exists(outputDir) Then
                    IO.Directory.CreateDirectory(outputDir)
                End If
                System.IO.File.WriteAllText(outputPath, resultStr, encode)
            Else
                ' 出力結果をコンソールに表示します。
                System.Console.Out.WriteLine(resultStr)
            End If

        Catch ex As Exception
            ' エラーが発生した場合はエラーメッセージを表示します。
            System.Console.Error.WriteLine("エラー: " & ex.Message)
            System.Console.Error.WriteLine("詳細: " & ex.ToString())
            Environment.Exit(1) ' エラーコード1で終了
        End Try
    End Sub

    ''' <summary>
    ''' AnalysisEnvironmentにJSONオブジェクトを登録します。
    ''' </summary>
    ''' <param name="env">解析環境</param>
    ''' <param name="jsonObject">JSONオブジェクト</param>
    <Extension()>
    Private Sub SetJsonVariable(env As AnalysisEnvironment, jsonObject As DynamicObject)
        Dim iter = jsonObject.GetEntries()

        ' JSONオブジェクトの各エントリをAnalysisEnvironmentに登録します。
        While iter.MoveNext()
            env.RegistObject(iter.Current.Name, iter.Current.Value)
        End While
    End Sub

    ''' <summary>
    ''' AnalysisEnvironmentにCSVデータを登録します。
    ''' </summary>
    ''' <param name="env">解析環境</param>
    ''' <param name="spliter">CSVスプリッター</param>
    ''' <param name="paramPath">パラメータファイルのパス</param>
    ''' <remarks>
    ''' CSVファイルを読み込み、各行をDynamicObjectとして登録します。
    ''' </remarks>
    <Extension()>
    Private Sub SetCsvVariable(env As AnalysisEnvironment, spliter As CsvSpliter, paramPath As String)
        Dim datas As New List(Of DynamicObject)()

        ' CSVスプリッターを使用して、CSVファイルの各行をDynamicObjectに変換します。
        Dim dat = spliter.Split()
        While Not dat.IsEmpty
            datas.Add(dat)
            dat = spliter.Split()
        End While

        ' DynamicObjectを配列として解析し、環境に登録します。
        env.RegistArray(Of DynamicObject)(paramPath.GetFileName(), datas.ToArray())
    End Sub

    ''' <summary>
    ''' Uriを絶対パスに変換します。
    ''' </summary>
    ''' <param name="pathUri">変換するUri</param>
    ''' <returns>絶対パス</returns>
    <Extension()>
    Private Function ConvertAbsolutePath(pathUri As Uri) As String
        If pathUri.IsAbsoluteUri Then
            Return pathUri.LocalPath
        Else
            Return IO.Path.Combine(Environment.CurrentDirectory, pathUri.OriginalString)
        End If
    End Function

    ''' <summary>
    ''' ファイル名を取得します。
    ''' 拡張子を除いたファイル名を返します。
    ''' </summary>
    ''' <param name="filePath">ファイルパス</param>
    ''' <returns>拡張子を除いたファイル名</returns>
    <Extension()>
    Private Function GetFileName(filePath As String) As String
        Dim name = Path.GetFileName(filePath)
        Dim ext = Path.GetExtension(name)
        If String.IsNullOrEmpty(ext) Then
            Return name
        Else
            Return name.Substring(0, name.Length - ext.Length)
        End If
    End Function

End Module
