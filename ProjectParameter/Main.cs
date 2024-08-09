using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectParameter
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;
            #region Черновик- Добавление общего парметра
            ////var categorySet = new CategorySet();
            ////categorySet.Insert(Category.GetCategory(doc, BuiltInCategory.OST_PipeCurves));

            //using (Transaction tr = new Transaction(doc, "add param"))
            //{
            //    tr.Start();
            //    CreateSharedParameter(uiApp.Application, doc, "Наименование", categorySet, BuiltInParameterGroup.PG_TEXT, true);
            //    tr.Commit();
            //}
            //return Result.Succeeded;
            //}

            //private void CreateSharedParameter(Application application, Document doc, string parameterName, CategorySet categorySet, BuiltInParameterGroup biltInParameterGroup, bool IntInstance)
            //{
            //    DefinitionFile definitionFile = application.OpenSharedParameterFile();

            //    if (definitionFile == null)
            //    {
            //        TaskDialog.Show("Ошибка", "Не найден файл общих папаметров!");
            //        return;
            //    }

            //    Definition definition = definitionFile.Groups.SelectMany(group => group.Definitions)
            //        .FirstOrDefault(def => def.Name.Equals(parameterName));
            //    if (definition == null)
            //    {
            //        TaskDialog.Show("Ошибка", "Не найден указанный паметр");
            //        return;
            //    }
            //    Binding binding = application.Create.NewTypeBinding(categorySet);
            //    if (IntInstance)
            //    {
            //        binding = application.Create.NewInstanceBinding(categorySet);
            //        BindingMap map = doc.ParameterBindings;
            //        map.Insert(definition, binding, biltInParameterGroup );
            //    }
            #endregion Lj,f

            try
            {
                IList<Reference> selectElements = uiDoc.Selection.PickObjects(ObjectType.Element, "Выберете трубы");

                foreach (var SElement in selectElements)
                {
                    var element = doc.GetElement(SElement);
                    if (element is Pipe)
                    {
                        Parameter pipeOuterDiametr = element.get_Parameter(BuiltInParameter.RBS_PIPE_OUTER_DIAMETER);
                        Parameter pipeInnerDiametr = element.get_Parameter(BuiltInParameter.RBS_PIPE_INNER_DIAM_PARAM);
                        int pipeOuterDiametrMilimetr = Convert.ToInt32( UnitUtils.ConvertFromInternalUnits(pipeOuterDiametr.AsDouble(), UnitTypeId.Millimeters));
                        int pipeInnerDiametrMilimetr = Convert.ToInt32( UnitUtils.ConvertFromInternalUnits(pipeInnerDiametr.AsDouble(), UnitTypeId.Millimeters));


                        using (Transaction tr = new Transaction(doc, "add param"))
                        {
                            tr.Start();
                            var familyInstence = element as FamilyInstance;
                            Parameter parameter = element.LookupParameter("Наименование");
                            parameter.Set($"{pipeOuterDiametrMilimetr}/{pipeInnerDiametrMilimetr}");
                            tr.Commit();
                        }
                    }
                }
                TaskDialog.Show("!", "Наименование трубы добавлено");
                return Result.Succeeded;
            }
            catch
            {
                TaskDialog.Show("!", "Наименование трубы не добавлено");
                return Result.Succeeded;
            }



        }
    }
}
