Option Strict On
Option Explicit On

Imports ZoppaDynamicText.Analysis
Imports ZoppaDynamicText.Collections
Imports ZoppaDynamicText.Strings

Public NotInheritable Class ZoppaDynamicText
    Implements IDisposable

    Private Shared ReadOnly _instance As New Lazy(Of ZoppaDynamicText)(Function() New ZoppaDynamicText())

    Private ReadOnly _analysisEnvironment As AnalysisEnvironment

    Private ReadOnly _analysisMessages As Btree(Of AnalysisResults)

    Public ReadOnly Property Environment As AnalysisEnvironment
        Get
            Return _analysisEnvironment
        End Get
    End Property

    ''' <summary>コンストラクタ。</summary>
    ''' <remarks>環境をを初期化します。</remarks>
    Private Sub New()
        _analysisEnvironment = New AnalysisEnvironment()
        _analysisEnvironment.AddFunction(GetMethoddName(NameOf(MathModule.Pow)), AddressOf MathModule.Pow)
        _analysisEnvironment.AddFunction(GetMethoddName(NameOf(MathModule.Round)), AddressOf MathModule.Round)
        _analysisMessages = New Btree(Of AnalysisResults)()
    End Sub

    Public Shared Function GetSession() As ZoppaDynamicText
        Return _instance.Value
    End Function

    Private Shared Function GetMethoddName(method As String) As U8String
        Return U8String.NewString(method.ToLower())
    End Function

    ''' <summary>テンプレート文字列を解析して結果を返します。</summary>
    ''' <param name="template">テンプレート文字列。</param>
    ''' <param name="param">パラメータオブジェクト。</param>
    ''' <returns>解析結果の文字列。</returns>
    Public Function Translate(template As IO.TextReader, param As Object) As String
        Return Translate(template.ReadToEnd(), param)
    End Function

    Public Function Translate(template As String, param As Object) As String
        Return Translate(U8String.NewString(template), param)
    End Function

    Public Function Translate(template As U8String, param As Object) As String
        '
        Dim result = _analysisMessages.Search(New AnalysisResults(template, Nothing))
        If result Is Nothing Then
            result = ParserModule.Translate(template)
            Me._analysisMessages.Insert(result)
        End If

        ' 入力文字列を解析して結果を返します。
        Using _analysisEnvironment.GetScope()
            Me._analysisEnvironment.RegistReflectObject(param)

            Dim wtmsg = result.Expression.GetValue(Me._analysisEnvironment)
            Return wtmsg.Str.ToString()
        End Using
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        ' リソースの解放処理が必要な場合はここに記述
    End Sub

End Class
