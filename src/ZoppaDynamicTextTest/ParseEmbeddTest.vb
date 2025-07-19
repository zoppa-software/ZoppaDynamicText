﻿Imports System
Imports Xunit
Imports ZoppaDynamicText
Imports ZoppaDynamicText.Analysis
Imports ZoppaDynamicText.Strings
Imports ZoppaDynamicText.Analysis.AnalysisValue

Public Class ParseEmbeddTest

    <Fact>
    Public Sub ParseEmbeddTest_Translate_OnlyPlainText_ReturnsNoneBlock()
        Dim input = U8String.NewString("plain text only")
        Dim result = ParserModule.Translate(input)
        Assert.True(result.Expression.GetValue(Nothing).Str.Equals("plain text only"))

        Dim input2 = U8String.NewString("a\{b\}c")
        Dim result2 = ParserModule.Translate(input2)
        Assert.True(result2.Expression.GetValue(Nothing).Str.Equals("a{b}c"))

        Dim input3 = U8String.NewString("a\#{b")
        Dim result3 = ParserModule.Translate(input3)
        Assert.True(result3.Expression.GetValue(Nothing).Str.Equals("a#{b"))
    End Sub

    <Fact>
    Public Sub ParseEmbeddTest_Translate_Enpty()
        Dim input = U8String.NewString("こんにちわ！ {}世界")
        Dim result = ParserModule.Translate(input)
        Assert.True(result.Expression.GetValue(Nothing).Str.Equals("こんにちわ！ 世界"))
    End Sub

    <Fact>
    Public Sub ParseEmbeddTest_Unfold()
        Dim input = U8String.NewString("1 + 1 = #{1 + 1}")
        Dim result = ParserModule.Translate(input)
        Assert.True(result.Expression.GetValue(Nothing).Str.Equals("1 + 1 = 2"))

        Dim input2 = U8String.NewString("1 + 1 = #{1 + 1} esc = !{2 + 2}")
        Dim result2 = ParserModule.Translate(input2)
        Assert.True(result2.Expression.GetValue(Nothing).Str.Equals("1 + 1 = 2 esc = 4"))
    End Sub

    <Fact>
    Public Sub ParseEmbeddTest_VariableDefine()
        Dim venv As New AnalysisEnvironment()
        Dim input = U8String.NewString("${var1=100; var2='abc'}var1=#{var1}, var2=#{var2}")
        Dim result = ParserModule.Translate(input)
        Assert.True(result.Expression.GetValue(venv).Str.Equals("var1=100, var2=abc"))
    End Sub

    <Fact>
    Public Sub ParseEmbeddTest_IfBlock()
        Dim venv As New AnalysisEnvironment()
        Dim input = U8String.NewString("これは、{if true}真{else}偽{/if}です。")
        Dim result = ParserModule.Translate(input)
        Assert.True(result.Expression.GetValue(venv).Str.Equals("これは、真です。"))

        Dim input1 = U8String.NewString("これは、{if false}真{else}偽{/if}です。")
        Dim result1 = ParserModule.Translate(input1)
        Assert.True(result1.Expression.GetValue(venv).Str.Equals("これは、偽です。"))

        Dim input2 = U8String.NewString("これは、{if true}真{else if false}偽{/if}です。")
        Dim result2 = ParserModule.Translate(input2)
        Assert.True(result2.Expression.GetValue(venv).Str.Equals("これは、真です。"))

        Dim input3 = U8String.NewString("これは、{if false}真{else if true}偽{else}他{/if}です。")
        Dim result3 = ParserModule.Translate(input3)
        Assert.True(result3.Expression.GetValue(venv).Str.Equals("これは、偽です。"))
    End Sub

    <Fact>
    Public Sub ParseEmbeddTest_ForBlock()
        Dim venv As New AnalysisEnvironment()
        Dim input = U8String.NewString("これは、{for i in [1, 2, 3]}#{i}、{/for}です。")
        Dim result = ParserModule.Translate(input)
        Assert.True(result.Expression.GetValue(venv).Str.Equals("これは、1、2、3、です。"))
        Dim input2 = U8String.NewString("これは、{for i in [1, 2, 3]}#{i * 10}、{/for}です。")
        Dim result2 = ParserModule.Translate(input2)
        Assert.True(result2.Expression.GetValue(venv).Str.Equals("これは、10、20、30、です。"))
    End Sub

    <Fact>
    Public Sub ParseEmbeddTest_SetVariable()
        Dim venv As New AnalysisEnvironment()
        Dim input = U8String.NewString("${var1=0;var2=''}{set var1=100; var2='abc'}#{var1}")
        Dim result = ParserModule.Translate(input)
        Assert.True(result.Expression.GetValue(venv).Str.Equals("100"))
    End Sub

    <Fact>
    Public Sub ParseEmbeddTest_SelectStatement()
        Dim venv As New AnalysisEnvironment()
        Dim input1 = U8String.NewString("これは、{select 1}{case 1}一{case 2}二{default}それ以外{/select}です。")
        Dim result1 = ParserModule.Translate(input1)
        Assert.True(result1.Expression.GetValue(venv).Str.Equals("これは、一です。"))

        Dim input2 = U8String.NewString("これは、{select 2}{case 1}一{case 2}二{default}それ以外{/select}です。")
        Dim result2 = ParserModule.Translate(input2)
        Assert.True(result2.Expression.GetValue(venv).Str.Equals("これは、二です。"))

        Dim input3 = U8String.NewString("これは、{select 3}{case 1}一{case 2}二{default}それ以外{/select}です。")
        Dim result3 = ParserModule.Translate(input3)
        Assert.True(result3.Expression.GetValue(venv).Str.Equals("これは、それ以外です。"))
    End Sub

    <Fact>
    Public Sub ParseEmbeddTest_IfBlockWithVariable()
        Dim venv As New AnalysisEnvironment()
        Dim input = U8String.NewString("あいう{br}えお")
        Dim result = ParserModule.Translate(input)
        Assert.True(result.Expression.GetValue(venv).Str.Equals("あいう
えお"))
    End Sub

    <Fact>
    Public Sub SplitEmbeddedText_Trim()
        Dim input = U8String.NewString("{trim '|'}A|B|C|{/trim}")
        Dim result = ParserModule.Translate(input)
        Assert.True(result.Expression.GetValue(Nothing).Str.Equals("A|B|C"))

        Dim input1 = U8String.NewString("{trim '|'}A|B|C| {/trim}")
        Dim result1 = ParserModule.Translate(input1)
        Assert.True(result1.Expression.GetValue(Nothing).Str.Equals("A|B|C"))

        Dim input2 = U8String.NewString("{trim '|'} A|B|C|{/trim}")
        Dim result2 = ParserModule.Translate(input2)
        Assert.True(result2.Expression.GetValue(Nothing).Str.Equals("A|B|C"))

        Dim input3 = U8String.NewString("{trim '|'} |A|B|C| {/trim}")
        Dim result3 = ParserModule.Translate(input3)
        Assert.True(result3.Expression.GetValue(Nothing).Str.Equals("A|B|C"))
    End Sub

End Class
