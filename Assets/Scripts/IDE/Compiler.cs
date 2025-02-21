using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using UnityEngine;

public class Compiler : MonoBehaviour
{
    public static string CompileCode(string code)
    {
        // Erstelle ein Syntaxbaum für den Code und führe eine Syntaxanalyse durch
        SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
            using System;
            public class Program {
                public static void Main() {
                    " + code + @"
                }
            }"
        );

        // Definiere eine Compilation mit Standard-Referenzen
        CSharpCompilation compilation = CSharpCompilation.Create(
            "InMemoryAssembly",
            new[] { tree },
            new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        // Führe die Kompilierung aus
        var diagnostics = compilation.GetDiagnostics();

        // Fehler sammeln
        List<string> errors = new List<string>();
        foreach (var diagnostic in diagnostics)
        {
            if (diagnostic.Severity == DiagnosticSeverity.Error) // Nur Fehler anzeigen (keine Warnungen)
            {
                errors.Add(diagnostic.ToString());
            }
        }

        // Fehler zurückgeben oder ""
        return errors.Count > 0 ? string.Join("\n", errors) : "";
    }
}
