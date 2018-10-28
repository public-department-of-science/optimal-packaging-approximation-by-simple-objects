using hs071_cs;
using MainProject.Containers;
using MainProject.Helpers;
using MainProject.InternalObjectsClasses.CircularObjects;
using MainProject.Model;
using PackageProject.InternalObjectsClasses.CircularObjects;
using System.Collections.Generic;

namespace App
{
    #region Delegates

    /// <summary>
    /// Console print delegate
    /// </summary>
    /// <param name="message">Text message</param>
    public delegate void PrintTextDel(string message);

    /// <summary>
    /// Delegat-helper for output error messages to stream
    /// </summary>
    /// <param name="message">Text message</param>
    public delegate void PrintErrorMessageDel(string message);

    #endregion

    public class Program
    {
        public static void Main()
        {
            PrintTextDel Print = new PrintTextDel(OutPut.Write);
            SolverHelper solverHelper = new SolverHelper();

            #region will be deleted soon, now it's like imitation of data input

            //CombinedObject list1 = new CombinedObject();
            //list1.InternalInCombineObjects.Add(new Sphere(new Point(), 2));
            //list1.InternalInCombineObjects.Add(new Sphere(new Point(3, 2, -1), 28));
            //list1.InternalInCombineObjects.Add(new Sphere(new Point(3, 2, -1), 8));

            //List<CombinedObject> combinedObjects = new List<CombinedObject>()
            //{
            //    list1
            //};

            Data data1232 = new Data(null);
            data1232.Container = new CircularContainer(20, new Point());

            data1232.Objects.AddRange(combinedObjects);
            data1232.Objects.Add(new Sphere(new Point(0, 2, 4), 2));
            data1232.Objects.Add(new Sphere(new Point(1, -2, 3), 3));
            data1232.Objects.Add(new Sphere(new Point(-4, -5, 7), 10));

            try
            {
                solverHelper.SolveTheProblem(dataAdapter: new Adapter(data1232), data: data1232);
            }
            catch (System.Exception ex)
            {
                throw;
            }

            #endregion

            #region Reading Data => will be one method Output.ReadDataType

            //Print("\nSelect input method \n 1 --> Read from File \n 2 --> Random generate");
            //Input.ChooseTypeReadingData(out int[] amountOfObjectsInEachComplexObject, out int TotalBallCount, out xNach, out yNach, out zNach, out rNach, out RNach, out double maxRandRadius, out rSortSum);
            //Print("\nChoose  type of external container \n 1 --> Circular container \n 2 --> Parallelogram container\nSelect-->");
            //Input.ChooseTypeOfContainer(out IContainer container, RNach);

            #endregion

           // OutPut.SaveResultToFile(data1232, "IterationFixedRadius");
        }
    }
}
