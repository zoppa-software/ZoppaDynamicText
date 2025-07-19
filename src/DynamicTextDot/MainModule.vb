Option Strict On
Option Explicit On

Imports ZoppaDynamicText.Analysis
Imports ZoppaDynamicText.Switches

Module MainModule

    Sub Main()
        Dim args = New String(Environment.GetCommandLineArgs().Length - 2) {}
        System.Array.Copy(Environment.GetCommandLineArgs(), 1, args, 0, Environment.GetCommandLineArgs().Length - 1)

        ' コマンドライン引数を解析するためのAnalysisSwitchを定義します。
        Dim switchDefine As AnalysisSwitch = AnalysisSwitch.Create(
            appDescription:="埋込式を解析します。",
            appCopyright:="© 2025 Sample Inc.",
            appLicense:="MIT License",
            subCommandRequired:=False,
            subCommand:=New SubCommandDefine() {},
            options:=New SwitchDefine() {
                New SwitchDefine("help", False, SwitchType.DoubleHyphen, ParameterType.None, "ヘルプを表示します。"),
                New SwitchDefine("template", True, SwitchType.DoubleHyphen, ParameterType.URI, "埋込式を記述したテンプレートファイルパス。"),
                New SwitchDefine("param", True, SwitchType.DoubleHyphen, ParameterType.URI, "パラメータファイルパス。"),
                New SwitchDefine("output", False, SwitchType.DoubleHyphen, ParameterType.URI, "出力ファイルパス。")
            }
        )

        ' コマンドライン引数を解析します。
        Dim analysisSwitches = switchDefine.Parse(args)
        If analysisSwitches.ContantsOption("help") Then
            System.Console.Out.WriteLine(switchDefine.GetHelp())
        End If

        ' パラメータチェック
        analysisSwitches.CheckRequired()

        Dim tempPath = analysisSwitches.GetOption("template").GetURI()
        Dim paramPath = analysisSwitches.GetOption("param").GetURI()

        Dim tempStr As String = System.IO.File.ReadAllText(Environment.CurrentDirectory & "\" & tempPath.OriginalString, Text.Encoding.UTF8)
        Dim paramStr As String = System.IO.File.ReadAllText(Environment.CurrentDirectory & "\" & paramPath.OriginalString, Text.Encoding.UTF8)

        ' 解析環境を初期化し、オブジェクトを登録します。
        Dim env As New AnalysisEnvironment()
        Dim result = ParserModule.Translate($"${{{paramStr}}}" & tempStr)
        If analysisSwitches.ContantsOption("output") Then
            Dim outputPath = analysisSwitches.GetOption("output").GetURI()
            System.IO.File.WriteAllText(Environment.CurrentDirectory & "\" & outputPath.OriginalString, result.Expression.GetValue(env).Str.ToString(), Text.Encoding.UTF8)
        Else
            System.Console.Out.WriteLine(result.Expression.GetValue(env).Str.ToString())
        End If
    End Sub

End Module
