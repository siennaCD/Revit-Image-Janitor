using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
namespace RevitJanitor
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Class1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Get application and document objects
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            TaskDialog.Show("Revit", "This is Sienna's Image Janitor Plugin");

            ListMaterials(doc);

            return Result.Succeeded;
        }

        public void ListMaterials(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            var materials = collector.WherePasses(new ElementClassFilter(typeof(Material))).Cast<Material>();

            foreach (Material mat in materials)
            {
                Debug.Print($"{mat.Name}");
            }
        }

        public void AlterAsset(Document doc, Material mat)
        {
            ElementId appearanceaseetId = mat.AppearanceAssetId;
            AppearanceAssetElement assetElem =
                mat.Document.GetElement(appearanceaseetId) as AppearanceAssetElement;

            using (Transaction t = new Transaction(assetElem.Document, "Change material asset"))
            {
                t.Start();
                using (AppearanceAssetEditScope editScope = new AppearanceAssetEditScope(doc))
                {
                    Asset editableAsset = editScope.Start(assetElem.Id);

                    //AssetProperty bumpMapProperty = editableAsset["generic_bump_map"];

                    editScope.Commit(true);
                }
                t.Commit();
            }
        }
    }
}
