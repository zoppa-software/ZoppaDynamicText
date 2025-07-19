Option Strict On
Option Explicit On

Namespace Analysis

    Module MathModule

        Function Pow(x As IValue, y As IValue) As IValue
            Return Math.Pow(x.Number, y.Number).ToNumberValue()
        End Function

        Function Round(d As IValue, decimals As IValue) As IValue
            Return Math.Round(d.Number, CInt(decimals.Number)).ToNumberValue()
        End Function

    End Module

End Namespace
