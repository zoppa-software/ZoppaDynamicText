Option Strict On
Option Explicit On

Namespace Analysis

    ''' <summary>計算モジュール。</summary>
    Public Module MathModule

        ''' <summary>指数計算。</summary>
        ''' <param name="x">基数。</param>
        ''' <param name="y">指数。</param>
        ''' <returns>計算結果。</returns>
        Function Pow(x As IValue, y As IValue) As IValue
            Return Math.Pow(x.Number, y.Number).ToNumberValue()
        End Function

        ''' <summary>丸め処理。</summary>
        ''' <param name="d">対象の数。</param>
        ''' <param name="decimals">丸める桁数。</param>
        ''' <returns>計算結果。</returns>
        Function Round(d As IValue, decimals As IValue) As IValue
            Return Math.Round(d.Number, CInt(decimals.Number)).ToNumberValue()
        End Function

    End Module

End Namespace
