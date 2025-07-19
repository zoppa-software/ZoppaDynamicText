﻿Option Strict On
Option Explicit On

Imports ZoppaDynamicText.Strings

Namespace Analysis

    ''' <summary>
    ''' 変数定義式を表す構造体です。
    ''' 変数名とその値を表す式を持ちます。
    ''' </summary>
    ''' <remarks>
    ''' 変数定義式は、変数名とその値を定義するために使用されます。
    ''' 変数名はU8String型で指定され、値はIExpression型で表されます。
    ''' </remarks>
    NotInheritable Class VariableDefineExpression
        Implements IExpression

        ' 変数名
        Private ReadOnly _name As U8String

        ' 変数値
        Private ReadOnly _value As IExpression

        ''' <summary>変数定義式を初期化します。</summary>
        ''' <param name="name">変数名。</param>
        ''' <param name="value">変数の値を表す式。</param>
        ''' <remarks>変数名は、U8String型で指定されます。</remarks>
        Public Sub New(name As U8String, value As IExpression)
            If value Is Nothing Then
                Throw New ArgumentNullException(NameOf(value))
            End If
            _name = name
            _value = value
        End Sub

        ''' <summary>式の型を取得します。</summary>
        ''' <returns>式の型。</returns>
        Public ReadOnly Property Type As ExpressionType Implements IExpression.Type
            Get
                Return ExpressionType.VariableDefineExpression
            End Get
        End Property

        ''' <summary>変数定義式から変数を定義します。</summary>
        ''' <param name="venv">変数環境。</param>
        ''' <returns>空の文字列値。</returns>
        Public Function GetValue(venv As AnalysisEnvironment) As IValue Implements IExpression.GetValue
            venv.RegistExpr(_name, _value)
            Return StringValue.Empty
        End Function

    End Class

End Namespace
