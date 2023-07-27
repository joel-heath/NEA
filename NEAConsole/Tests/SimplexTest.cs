using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NEAConsole.Program;

namespace NEAConsole.Tests;
internal class SimplexTest : ITest
{
    public string DisplayText => "Simplex";

    public void Test()
    {
        int dimensions = Random.Shared.Next(2, 4);
        int[] solution = Enumerable.Range(0, dimensions).Select(n => Random.Shared.Next(2, 6)).ToArray();

        var constraints = new SimplexInequality[dimensions];

        for (int i = 0; i < dimensions; i++)
        {
            constraints[i] = CreateConstraint(dimensions, solution);
        }

        // make sure objective is integers (make constraints divisible by dimensions)
        for (int i = 0; i < dimensions; i++)
        {
            var sum = 0;
            for (int j = 0; j < dimensions; j++)
            {
                sum += constraints[j].Coefficients[i];
            }
            var remainder = dimensions - (sum % dimensions); // need to make sure we have integer objective
                                                             // objective is average of constraints, therefore must be divisible by dimensions
                                                             // suppose sum of coefficients is 11 in a 3D LP. 11 % 3 == 2, 3 - 2 = 1, 11 + 1 = 12 now its divisible by 3.
            constraints[dimensions - 1].Coefficients[i] += remainder;
            constraints[dimensions - 1].Constant += remainder * solution[i];
        }

        SimplexInequality objective = GenerateObjectiveFunction(dimensions, solution, constraints);

        Console.WriteLine($"Maximise P = {objective.ToObjectiveString()}");
        Console.WriteLine($"Subject to:");

        for (int i = 0; i < dimensions; i++)
        {
            Console.WriteLine($"    {constraints[i]}");
        }

        Console.Write("\nP = ");
        var P = int.Parse(Console.ReadLine() ?? "0"); // need to catch potential input errors here
        var input = new int[dimensions];
        for (int i = 0; i < dimensions; i++)
        {
            Console.Write((char)('x' + i) + " = ");
            input[i] = int.Parse(Console.ReadLine() ?? "0");
        }

        if (input.Where((n, i) => n != solution[i]).Any())
        {
            Console.WriteLine("Incorrect, the correct answer was:");
            Console.WriteLine("P = " + objective.Constant);
            for (int i = 0; i < dimensions; i++)
            {
                Console.WriteLine((char)('x' + i) + " = " + solution[i]);
            }
        }
        else
        {
            Console.WriteLine("Correct!");
        }

        Console.ReadKey(true);
        Console.Clear();
    }

    public static SimplexInequality CreateConstraint(int dimensions, int[] solution)
    {
        int[] coeffs = new int[dimensions];
        var constant = 0;
        for (int j = 0; j < dimensions; j++)
        {
            var coefficient = Random.Shared.Next(1, 6);
            coeffs[j] = coefficient;
            constant += coefficient * solution[j];
        }

        return new SimplexInequality(coeffs, constant, SimplexInequality.InequalityType.LessThan);
    }

    private static SimplexInequality GenerateObjectiveFunction(int dimensions, int[] solution, IList<SimplexInequality> constraints)
    {
        int[] coeffs = new int[dimensions];
        var constant = 0;
        for (int i = 0; i < dimensions; i++)
        {
            int coefficient = 0; // AVERAGING TO AN INT!! WE MUST MAKE SURE CONTRAINTS AVERAGE TO AN INT
                                    // TODO: Generate n-1 rand numbers. gen nth rand number, mod sum by n, add result to final num.
            for (int j = 0; j < dimensions; j++)
            {
                coefficient += constraints[j].Coefficients[i];
            }

            if (coefficient % dimensions != 0) throw new Exception("Constraints are not divisible by the number of dimensions");
            coefficient /= dimensions;
            coeffs[i] = coefficient;
            constant += coefficient * solution[i];
        }

        return new SimplexInequality(coeffs, constant, SimplexInequality.InequalityType.LessThan); ;
    }
}