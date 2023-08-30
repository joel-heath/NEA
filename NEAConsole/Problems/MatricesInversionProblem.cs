﻿using NEAConsole.Matrices;
using System;

namespace NEAConsole.Problems;
internal class MatricesInversionProblem : IProblem
{
    private readonly Matrix mat;
    private readonly Matrix solution;
    private Matrix? answer;

    public void Display()
    {
        MatricesProblem.DrawMatrix(mat);

        var signSpacing = (mat.Rows - 1) / 2;

        Console.Write("-1");
        Console.CursorTop += signSpacing;
        Console.Write(" = ");
        Console.CursorTop -= signSpacing;
    }

    public void GetAnswer()
    {
        answer = MatricesProblem.InputMatrix(solution.Rows, solution.Columns);
        Console.WriteLine();
    }

    public bool EvaluateAnswer()
        => (answer ?? throw new NotAnsweredException()) == solution;

    public void Summarise()
    {
        if (EvaluateAnswer())
        {
            Console.WriteLine("Correct!");
        }
        else
        {
            Console.WriteLine("Incorrect. The correct answer was: ");
            MatricesProblem.DrawMatrix(solution);
        }

        Console.ReadKey(true);
        Console.Clear();
    }

    public MatricesInversionProblem(Matrix mat, Matrix solution)
    {
        this.mat = mat;
        this.solution = solution;
    }
}