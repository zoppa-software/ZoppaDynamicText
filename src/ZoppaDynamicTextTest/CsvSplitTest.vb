Imports System
Imports Xunit
Imports ZoppaDynamicText
Imports ZoppaDynamicText.Collections
Imports ZoppaDynamicText.LegacyFiles
Imports ZoppaDynamicText.Strings

Public Class CsvSplitTest

    <Fact>
    Public Sub Split_CsvLine_ShouldReturnCorrectParts()
        Dim csvLine As String = "Name,Age,Location
造田, 49, 福山
あいり, 20, 広島
"
        Dim spliter As CsvSpliter = CsvSpliter.CreateSpliter(csvLine)
        Dim parts1 = spliter.Split()
        Assert.Equal("造田", parts1("Name").ToString())
        Assert.Equal("49", parts1("Age").ToString())
        Assert.Equal("福山", parts1("Location").ToString())

        Dim parts2 = spliter.Split()
        Assert.Equal("あいり", parts2("Name").ToString())
        Assert.Equal("20", parts2("Age").ToString())
        Assert.Equal("広島", parts2("Location").ToString())

        Dim parts3 = spliter.Split()
        Dim parts4 = spliter.Split()
    End Sub

End Class
