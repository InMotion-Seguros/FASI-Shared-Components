Imports System.Text.RegularExpressions

Namespace Helpers

    ''' <summary>
    ''' The Inflector class transforms words from one
    ''' form to another. For example, from singular to plural.
    ''' </summary>
    Friend Class Inflector

        ' Methods
        Shared Sub New()
            Inflector.AddPlural("$", "s")
            Inflector.AddPlural("s$", "s")
            Inflector.AddPlural("(ax|test)is$", "$1es")
            Inflector.AddPlural("(octop|vir)us$", "$1i")
            Inflector.AddPlural("(alias|status)$", "$1es")
            Inflector.AddPlural("(bu)s$", "$1ses")
            Inflector.AddPlural("(buffal|tomat)o$", "$1oes")
            Inflector.AddPlural("([ti])um$", "$1a")
            Inflector.AddPlural("sis$", "ses")
            Inflector.AddPlural("(?:([^f])fe|([lr])f)$", "$1$2ves")
            Inflector.AddPlural("(hive)$", "$1s")
            Inflector.AddPlural("([^aeiouy]|qu)y$", "$1ies")
            Inflector.AddPlural("(x|ch|ss|sh)$", "$1es")
            Inflector.AddPlural("(matr|vert|ind)ix|ex$", "$1ices")
            Inflector.AddPlural("([m|l])ouse$", "$1ice")
            Inflector.AddPlural("^(ox)$", "$1en")
            Inflector.AddPlural("(quiz)$", "$1zes")

            Inflector.AddSingular("s$", "")
            Inflector.AddSingular("(n)ews$", "$1ews")
            Inflector.AddSingular("([ti])a$", "$1um")
            Inflector.AddSingular("((a)naly|(b)a|(d)iagno|(p)arenthe|(p)rogno|(s)ynop|(t)he)ses$", "$1$2sis")
            Inflector.AddSingular("(^analy)ses$", "$1sis")
            Inflector.AddSingular("([^f])ves$", "$1fe")
            Inflector.AddSingular("(hive)s$", "$1")
            Inflector.AddSingular("(tive)s$", "$1")
            Inflector.AddSingular("([lr])ves$", "$1f")
            Inflector.AddSingular("([^aeiouy]|qu)ies$", "$1y")
            Inflector.AddSingular("(s)eries$", "$1eries")
            Inflector.AddSingular("(m)ovies$", "$1ovie")
            Inflector.AddSingular("(x|ch|ss|sh)es$", "$1")
            Inflector.AddSingular("([m|l])ice$", "$1ouse")
            Inflector.AddSingular("(bus)es$", "$1")
            Inflector.AddSingular("(o)es$", "$1")
            Inflector.AddSingular("(shoe)s$", "$1")
            Inflector.AddSingular("(cris|ax|test)es$", "$1is")
            Inflector.AddSingular("(octop|vir)i$", "$1us")
            Inflector.AddSingular("(alias|status)es$", "$1")
            Inflector.AddSingular("^(ox)en", "$1")
            Inflector.AddSingular("(vert|ind)ices$", "$1ex")
            Inflector.AddSingular("(matr)ices$", "$1ix")
            Inflector.AddSingular("(quiz)zes$", "$1")

            Inflector.AddIrregular("person", "people")
            Inflector.AddIrregular("man", "men")
            Inflector.AddIrregular("child", "children")
            Inflector.AddIrregular("sex", "sexes")
            Inflector.AddIrregular("move", "moves")

            Inflector.AddUncountable("equipment")
            Inflector.AddUncountable("information")
            Inflector.AddUncountable("rice")
            Inflector.AddUncountable("money")
            Inflector.AddUncountable("species")
            Inflector.AddUncountable("series")
            Inflector.AddUncountable("fish")
            Inflector.AddUncountable("sheep")
            Inflector.AddUncountable("address")
        End Sub

        Private Sub New()
        End Sub

        Private Shared Sub AddIrregular(ByVal singular As String, ByVal plural As String)
            Inflector.AddPlural(String.Concat(New Object() {"(", singular.Chars(0), ")", singular.Substring(1), "$"}), ("$1" & plural.Substring(1)))
            Inflector.AddSingular(String.Concat(New Object() {"(", plural.Chars(0), ")", plural.Substring(1), "$"}), ("$1" & singular.Substring(1)))
        End Sub

        Private Shared Sub AddPlural(ByVal rule As String, ByVal replacement As String)
            Inflector.plurals.Add(New Rule(rule, replacement))
        End Sub

        Private Shared Sub AddSingular(ByVal rule As String, ByVal replacement As String)
            Inflector.singulars.Add(New Rule(rule, replacement))
        End Sub

        Private Shared Sub AddUncountable(ByVal word As String)
            Inflector.uncountables.Add(word.ToLower)
        End Sub

        Private Shared Function ApplyRules(ByVal rules As IList, ByVal word As String) As String
            Dim str As String = word
            If Not Inflector.uncountables.Contains(word.ToLower) Then
                Dim i As Integer = (rules.Count - 1)
                Do While (i >= 0)
                    Dim rule As Rule = DirectCast(rules.Item(i), Rule)
                    str = rule.Apply(word)
                    If (Not str Is Nothing) Then
                        Return str
                    End If
                    i -= 1
                Loop
            End If
            If IsNothing(str) Then
                str = word
            End If
            Return str
        End Function

        ''' <summary>
        ''' Capitalizes a word.
        ''' </summary>
        ''' <param name="word">The word to be capitalized.</param>
        ''' <returns><paramref name="word" /> capitalized.</returns>
        Public Shared Function Capitalize(ByVal word As String) As String
            Return (word.Substring(0, 1).ToUpper & word.Substring(1).ToLower)
        End Function

        ''' <summary>
        ''' Return the plural of a word.
        ''' </summary>
        ''' <param name="word">The singular form</param>
        ''' <returns>The plural form of <paramref name="word" /></returns>
        Public Shared Function Pluralize(ByVal word As String) As String
            Dim result As String = Inflector.ApplyRules(Inflector.plurals, word)

            If word.EndsWith("status", StringComparison.InvariantCultureIgnoreCase) Or
               word.EndsWith("business", StringComparison.InvariantCultureIgnoreCase) Then
                result = word
            End If

            Return result
        End Function

        ''' <summary>
        ''' Return the singular of a word.
        ''' </summary>
        ''' <param name="word">The plural form</param>
        ''' <returns>The singular form of <paramref name="word" /></returns>
        Public Shared Function Singularize(ByVal word As String) As String
            Dim result As String = Inflector.ApplyRules(Inflector.singulars, word)

            If word.EndsWith("status", StringComparison.InvariantCultureIgnoreCase) Or
               word.EndsWith("business", StringComparison.InvariantCultureIgnoreCase) Then
                result = word
            End If

            Return result
        End Function

        ' Fields
        Private Shared ReadOnly plurals As ArrayList = New ArrayList

        Private Shared ReadOnly singulars As ArrayList = New ArrayList
        Private Shared ReadOnly uncountables As ArrayList = New ArrayList

        ' Nested Types
        Private Class Rule

            ' Methods
            Public Sub New(ByVal pattern As String, ByVal replacement As String)
                Me.regex = New Regex(pattern, RegexOptions.IgnoreCase)
                Me.replacement = replacement
            End Sub

            Public Function Apply(ByVal word As String) As String
                If Not Me.regex.IsMatch(word) Then
                    Return Nothing
                End If
                Return Me.regex.Replace(word, Me.replacement)
            End Function

            ' Fields
            Private ReadOnly regex As Regex

            Private ReadOnly replacement As String
        End Class

    End Class

End Namespace