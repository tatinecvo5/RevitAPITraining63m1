using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Prism.Commands;
using RevitAPITrainingLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RevitAPITraining63m1
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;

        public List<FamilySymbol> ElementsTypes { get; } = new List<FamilySymbol>();
        public FamilySymbol SelectedElementsType { get; set; }

        public List<XYZ> Points { get; set; } = new List<XYZ>();

        public DelegateCommand SaveCommand { get; }
        public int ElementCount { get; set; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            ElementsTypes = FamilySymbolUtils.GetFamilySymbols(_commandData);
            SaveCommand = new DelegateCommand(OnSaveCommand);
            ElementCount = 1;
        }
        private void OnSaveCommand()
        {
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            if (ElementCount <= 0)
            {
                MessageBox.Show("Ошибка! Количество элементов меньше 0");
                return;
            }

            List<XYZ> PointEl = new List<XYZ>();
            if (ElementCount == 1)
            {
                RaiseCloseRequest();
                Points = SelectionUtils.GetPoints(_commandData, "Выберите первую точку", ObjectSnapTypes.Endpoints, 1);
                PointEl.Add(Points[0]);
            }
            else
            {
                RaiseCloseRequest();
                Points = SelectionUtils.GetPoints(_commandData, "Выберите вторую точку", ObjectSnapTypes.Endpoints, 2);
                double dX = (Points[1].X - Points[0].X) / (ElementCount - 1);
                double dY = (Points[1].Y - Points[0].Y) / (ElementCount - 1);
                for (int i = 0; i <= ElementCount - 1; i++)
                {
                    XYZ xYZ = new XYZ(Points[0].X + dX * i, Points[0].Y + dY * i, 0);
                    PointEl.Add(xYZ);
                }
            }

            foreach (var iPoint in PointEl)
            {
                FamilyInstanceUtils.CreateFamilyInstance(_commandData, SelectedElementsType, iPoint, doc.ActiveView.GenLevel);
            }

        }

        public event EventHandler CloseRequest;
        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }
}
