Option Strict On
Option Explicit On

Imports ZoppaDynamicText.Strings

Namespace Analysis

    Public Class TrimStatementExpression
        Implements IExpression

        Private ReadOnly _trimStrs As IExpression()

        Private ReadOnly _contentsExpr As IExpression

        Public Sub New(trimStrs() As IExpression, contentsExpr As IExpression)
            If trimStrs Is Nothing Then
                Throw New ArgumentNullException(NameOf(trimStrs))
            End If
            If contentsExpr Is Nothing Then
                Throw New ArgumentNullException(NameOf(contentsExpr))
            End If
            Me._trimStrs = trimStrs
            Me._contentsExpr = contentsExpr
        End Sub

        Public ReadOnly Property Type As ExpressionType Implements IExpression.Type
            Get
                Return ExpressionType.TrimExpression
            End Get
        End Property

        Public Function GetValue(venv As AnalysisEnvironment) As IValue Implements IExpression.GetValue
            Dim contentsValue = _contentsExpr.GetValue(venv).Str.Trim()

            Dim st As Integer = 0
            Dim ed As Integer = 0
            For Each trimStrExpr In _trimStrs
                Dim trimStrValue = trimStrExpr.GetValue(venv).Str
                If contentsValue.StartWith(trimStrValue) AndAlso st < trimStrValue.ByteLength Then
                    st = trimStrValue.ByteLength
                End If
                If contentsValue.EndWith(trimStrValue) AndAlso ed < trimStrValue.ByteLength Then
                    ed = trimStrValue.ByteLength
                End If
            Next

            If st > 0 OrElse ed > 0 Then
                Dim trimmedValue = U8String.NewSlice(contentsValue, st, contentsValue.ByteLength - st - ed)
                Return New StringValue(trimmedValue)
            End If
            Return New StringValue(contentsValue)
        End Function

    End Class

End Namespace
