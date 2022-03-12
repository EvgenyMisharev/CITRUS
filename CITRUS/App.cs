using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Windows.Media;

namespace CITRUS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

        public class App : IExternalApplication
        {
            public Result OnShutdown(UIControlledApplication application)
            {
                return Result.Succeeded;
            }

            public Result OnStartup(UIControlledApplication application)
            {
            // Создание панели
            string assemblyPach = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string tabName = "ЦИТRUS";
            application.CreateRibbonTab(tabName);

            //Создание категории кнопок "Публикация"
            RibbonPanel panel = application.CreateRibbonPanel(tabName, "Публикация");

            //Создание кнопки "Собиратор" в категории "Публикация"
            PushButtonData pbd = new PushButtonData("Sobirator", "Собиратор", assemblyPach, "CITRUS.Sobirator");

            Image img1 = Properties.Resources.Sobirator_Large;
            ImageSource imgLarge = GetImageSourse(img1);
            Image img2 = Properties.Resources.Sobirator;
            ImageSource imgStandart = GetImageSourse(img2);

            pbd.LargeImage = imgLarge;
            pbd.Image = imgStandart;
            panel.AddItem(pbd);

            //Создание кнопки "TXTExport" в категории "Публикация"
            PushButtonData pbdTXTExport = new PushButtonData("TXTExport", "TXT\nЭкспорт", assemblyPach, "CITRUS.TXTExport");
            Image TXTExport_img1 = Properties.Resources.ScheduleTXTExport_Large;
            ImageSource TXTExport_imgLarge = GetImageSourse(TXTExport_img1);
            Image TXTExport_img2 = Properties.Resources.ScheduleTXTExport;
            ImageSource TXTExport_imgStandart = GetImageSourse(TXTExport_img2);

            pbdTXTExport.LargeImage = TXTExport_imgLarge;
            pbdTXTExport.Image = TXTExport_imgStandart;
            panel.AddItem(pbdTXTExport);


            //Создание категории кнопок "АР"
            RibbonPanel panel_AR = application.CreateRibbonPanel(tabName, "АР");

            //Создание кнопки "Генератор пола" в категории "Публикация"
            PushButtonData pbdFloorGenerator = new PushButtonData("FloorGenerator"
                , "Генератор\nпола"
                , assemblyPach
                , "CITRUS.FloorGenerator");

            Image FloorGenerator_img1 = Properties.Resources.FloorGenerator_Large;
            ImageSource FloorGenerator_imgLarge = GetImageSourse(FloorGenerator_img1);
            Image FloorGenerator_img2 = Properties.Resources.FloorGenerator;
            ImageSource FloorGenerator_imgStandart = GetImageSourse(FloorGenerator_img2);

            pbdFloorGenerator.LargeImage = FloorGenerator_imgLarge;
            pbdFloorGenerator.Image = FloorGenerator_imgStandart;
            panel_AR.AddItem(pbdFloorGenerator);

            ////Создание кнопки "WallFinishCreator" в категории "АР"
            //PushButtonData pbdWallFinishCreator = new PushButtonData("WallFinishCreator"
            //        , "Отделка\nстен"
            //        , assemblyPach
            //        , "CITRUS.CIT_03_1_WallFinishCreator.CIT_03_1_WallFinishCreator");

            //Image WallFinishCreator_img1 = Properties.Resources.WallFinishCreator_Large;
            //ImageSource WallFinishCreator_imgLarge = GetImageSourse(WallFinishCreator_img1);
            //Image WallFinishCreator_img2 = Properties.Resources.WallFinishCreator;
            //ImageSource WallFinishCreator_imgStandart = GetImageSourse(WallFinishCreator_img2);

            //pbdWallFinishCreator.LargeImage = WallFinishCreator_imgLarge;
            //pbdWallFinishCreator.Image = WallFinishCreator_imgStandart;
            //panel_AR.AddItem(pbdWallFinishCreator);

            //Создание кнопки "FinishNumerator" в категории "АР"
            PushButtonData pbdFinishNumerator = new PushButtonData("FinishNumerator"
                    , "Нумератор\nотделки"
                    , assemblyPach
                    , "CITRUS.CIT_03_2_FinishNumerator.CIT_03_2_FinishNumerator");

            Image FinishNumerator_img1 = Properties.Resources.FinishNumerator_Large;
            ImageSource FinishNumerator_imgLarge = GetImageSourse(FinishNumerator_img1);
            Image FinishNumerator_img2 = Properties.Resources.FinishNumerator;
            ImageSource FinishNumerator_imgStandart = GetImageSourse(FinishNumerator_img2);

            pbdFinishNumerator.LargeImage = FinishNumerator_imgLarge;
            pbdFinishNumerator.Image = FinishNumerator_imgStandart;
            panel_AR.AddItem(pbdFinishNumerator);

            //Создание кнопки "WallFinishCreator_v2" в категории "АР"
            PushButtonData pbdWallFinishCreator_v2 = new PushButtonData("WallFinishCreator_v2"
                    , "Отделка\nстен v2"
                    , assemblyPach
                    , "CITRUS.CIT_03_1_WallFinishCreator_v2.CIT_03_1_WallFinishCreator_v2");

            panel_AR.AddItem(pbdWallFinishCreator_v2);

            //Создание категории кнопок "Инсоляция"
            RibbonPanel panel_Insolation = application.CreateRibbonPanel(tabName, "Инсоляция");

            //Создание кнопки "InsolationRulerСreator" в категории "Инсоляция"
            PushButtonData pbdInsolationRulerСreator = new PushButtonData("InsolationRulerСreator"
                    , "Инсоляционная\nлинейка"
                    , assemblyPach
                    , "CITRUS.CIT_03_3_Insolation.InsolationRulerСreator");

            panel_Insolation.AddItem(pbdInsolationRulerСreator);

            //Создание кнопки "InsolationAtPoint" в категории "Инсоляция"
            PushButtonData pbdInsolationAtPoint = new PushButtonData("InsolationAtPoint"
                    , "Инсоляция\nв точке"
                    , assemblyPach
                    , "CITRUS.CIT_03_3_Insolation.InsolationAtPoint");

            Image InsolationAtPoint_img1 = Properties.Resources.InsolationAtPoint_Large;
            ImageSource InsolationAtPoint_imgLarge = GetImageSourse(InsolationAtPoint_img1);
            Image InsolationAtPoint_img2 = Properties.Resources.InsolationAtPoint;
            ImageSource InsolationAtPoint_imgStandart = GetImageSourse(InsolationAtPoint_img2);

            pbdInsolationAtPoint.LargeImage = InsolationAtPoint_imgLarge;
            pbdInsolationAtPoint.Image = InsolationAtPoint_imgStandart;

            panel_Insolation.AddItem(pbdInsolationAtPoint);

            //Создание категории кнопок "КР"
            RibbonPanel panel_KR = application.CreateRibbonPanel(tabName, "КР");

            //Создание кнопки "CIT_04_7_ElementsTransfer" в категории "КР"
            PushButtonData pbdElementsTransfer = new PushButtonData("ElementsTransfer"
                , "Перенос\nэлементов"
                , assemblyPach
                , "CITRUS.CIT_04_7_ElementsTransfer.CIT_04_7_ElementsTransfer");

            Image ElementsTransfer_img1 = Properties.Resources.ElementsTransfer_Large;
            ImageSource ElementsTransfer_imgLarge = GetImageSourse(ElementsTransfer_img1);
            Image ElementsTransfer_img2 = Properties.Resources.ElementsTransfer;
            ImageSource ElementsTransfer_imgStandart = GetImageSourse(ElementsTransfer_img2);

            pbdElementsTransfer.LargeImage = ElementsTransfer_imgLarge;
            pbdElementsTransfer.Image = ElementsTransfer_imgStandart;
            panel_KR.AddItem(pbdElementsTransfer);

            //Создание кнопки "HoleTransfer" в категории "КР"
            PushButtonData pbdHoleTransfer = new PushButtonData("HoleTransfer"
                , "Замена\nпроемов"
                , assemblyPach
                , "CITRUS.CIT_04_6_HoleTransfer.CIT_04_6_HoleTransfer");

            Image HoleTransfer_img1 = Properties.Resources.HoleTransfer_Large;
            ImageSource HoleTransfer_imgLarge = GetImageSourse(HoleTransfer_img1);
            Image HoleTransfer_img2 = Properties.Resources.HoleTransfer;
            ImageSource HoleTransfer_imgStandart = GetImageSourse(HoleTransfer_img2);

            pbdHoleTransfer.LargeImage = HoleTransfer_imgLarge;
            pbdHoleTransfer.Image = HoleTransfer_imgStandart;
            panel_KR.AddItem(pbdHoleTransfer);

            ////Создание кнопки "Капитель" в категории "КР"
            //PushButtonData pbdCapitalMaker = new PushButtonData("CapitalMaker", "Капитель", assemblyPach, "CITRUS.CapitalMaker");
            //Image CapitalMaker_img1 = Properties.Resources.CapitalMaker_Large;
            //ImageSource CapitalMaker_imgLarge = GetImageSourse(CapitalMaker_img1);
            //Image CapitalMaker_img2 = Properties.Resources.CapitalMaker;
            //ImageSource CapitalMaker_imgStandart = GetImageSourse(CapitalMaker_img2);

            //pbdCapitalMaker.LargeImage = CapitalMaker_imgLarge;
            //pbdCapitalMaker.Image = CapitalMaker_imgStandart;
            //panel_KR.AddItem(pbdCapitalMaker);

            //Создание категории кнопок "Армирование"
            RibbonPanel panel_Reinforcement = application.CreateRibbonPanel(tabName, "Армирование");

            //Создание кнопки "Армирование квадратной колонны" в категории "Армирование"
            PushButtonData pbdSquareColumnsReinforcement = new PushButtonData("SquareColumnsReinforcement"
                , "Квадратные\nколонны"
                , assemblyPach
                , "CITRUS.CIT_04_1_SquareColumnsReinforcement.CIT_04_1_SquareColumnsReinforcement");

            Image SquareColumnsReinforcement_img1 = Properties.Resources.SquareColumnsReinforcement_Large;
            ImageSource SquareColumnsReinforcement_imgLarge = GetImageSourse(SquareColumnsReinforcement_img1);
            Image SquareColumnsReinforcement_img2 = Properties.Resources.SquareColumnsReinforcement;
            ImageSource SquareColumnsReinforcement_imgStandart = GetImageSourse(SquareColumnsReinforcement_img2);

            pbdSquareColumnsReinforcement.LargeImage = SquareColumnsReinforcement_imgLarge;
            pbdSquareColumnsReinforcement.Image = SquareColumnsReinforcement_imgStandart;

            ContextualHelp SquareColumnsReinforcementCHelp = new ContextualHelp(ContextualHelpType.Url, "https://youtu.be/jmJYHjQaAYc");
            pbdSquareColumnsReinforcement.SetContextualHelp(SquareColumnsReinforcementCHelp);
            panel_Reinforcement.AddItem(pbdSquareColumnsReinforcement);

            //Создание кнопки "Армирование прямоугольной колонны" в категории "Армирование"
            PushButtonData pbdRectangularColumnsReinforcement = new PushButtonData("RectangularColumnsReinforcement"
                , "Прямоугольные\nколонны"
                , assemblyPach
                , "CITRUS.CIT_04_2_RectangularColumnsReinforcement.CIT_04_2_RectangularColumnsReinforcement");

            Image RectangularColumnsReinforcement_img1 = Properties.Resources.RectangularColumnsReinforcement_Large;
            ImageSource RectangularColumnsReinforcement_imgLarge = GetImageSourse(RectangularColumnsReinforcement_img1);
            Image RectangularColumnsReinforcement_img2 = Properties.Resources.RectangularColumnsReinforcement;
            ImageSource RectangularColumnsReinforcement_imgStandart = GetImageSourse(RectangularColumnsReinforcement_img2);

            pbdRectangularColumnsReinforcement.LargeImage = RectangularColumnsReinforcement_imgLarge;
            pbdRectangularColumnsReinforcement.Image = RectangularColumnsReinforcement_imgStandart;

            ContextualHelp RectangularColumnsReinforcementCHelp = new ContextualHelp(ContextualHelpType.Url, "https://youtu.be/KEDHqLwrE4c");
            pbdRectangularColumnsReinforcement.SetContextualHelp(RectangularColumnsReinforcementCHelp);
            panel_Reinforcement.AddItem(pbdRectangularColumnsReinforcement);

            //Создание кнопки "Выпуски" в категории "Армирование"
            PushButtonData pbdRebarOutletsCreator = new PushButtonData("RebarOutletsCreator", "Выпуски", assemblyPach, "CITRUS.RebarOutletsCreator");

            Image RebarOutletsCreator_img1 = Properties.Resources.RebarOutletsCreator_Large;
            ImageSource RebarOutletsCreator_imgLarge = GetImageSourse(RebarOutletsCreator_img1);
            Image RebarOutletsCreator_img2 = Properties.Resources.RebarOutletsCreator;
            ImageSource RebarOutletsCreator_imgStandart = GetImageSourse(RebarOutletsCreator_img2);
            pbdRebarOutletsCreator.LargeImage = RebarOutletsCreator_imgLarge;
            pbdRebarOutletsCreator.Image = RebarOutletsCreator_imgStandart;

            ContextualHelp RebarOutletsCreatorCHelp = new ContextualHelp(ContextualHelpType.Url, "https://youtu.be/TH4PfymtrQY");
            pbdRebarOutletsCreator.SetContextualHelp(RebarOutletsCreatorCHelp);
            panel_Reinforcement.AddItem(pbdRebarOutletsCreator);

            //Создание кнопки "Копирователь групп" в категории "Армирование"
            PushButtonData pbdRebarGroupCopier = new PushButtonData("RebarGroupCopier"
                , "Копирователь\nгрупп"
                , assemblyPach
                , "CITRUS.RebarGroupCopierScript");

            Image RebarGroupCopier_img1 = Properties.Resources.RebarGroupCopier_Large;
            ImageSource RebarGroupCopier_imgLarge = GetImageSourse(RebarGroupCopier_img1);
            Image RebarGroupCopier_img2 = Properties.Resources.RebarGroupCopier;
            ImageSource RebarGroupCopier_imgStandart = GetImageSourse(RebarGroupCopier_img2);

            pbdRebarGroupCopier.LargeImage = RebarGroupCopier_imgLarge;
            pbdRebarGroupCopier.Image = RebarGroupCopier_imgStandart;

            ContextualHelp RebarGroupCopierCHelp = new ContextualHelp(ContextualHelpType.Url, "https://youtu.be/22U2xBbK6VM");
            pbdRebarGroupCopier.SetContextualHelp(RebarGroupCopierCHelp);
            panel_Reinforcement.AddItem(pbdRebarGroupCopier);

            //Создание кнопки "BeamReinforcement" в категории "Армирование"
            PushButtonData pbdBeamReinforcement = new PushButtonData("BeamReinforcement"
                    , "Балки"
                    , assemblyPach
                    , "CITRUS.CIT_04_3_BeamReinforcement.CIT_04_3_BeamReinforcement");

            Image BeamReinforcement_img1 = Properties.Resources.BeamReinforcement_Large;
            ImageSource BeamReinforcement_imgLarge = GetImageSourse(BeamReinforcement_img1);
            Image BeamReinforcement_img2 = Properties.Resources.BeamReinforcement;
            ImageSource BeamReinforcement_imgStandart = GetImageSourse(BeamReinforcement_img2);

            pbdBeamReinforcement.LargeImage = BeamReinforcement_imgLarge;
            pbdBeamReinforcement.Image = BeamReinforcement_imgStandart;

            ContextualHelp BeamReinforcementCHelp = new ContextualHelp(ContextualHelpType.Url, "https://youtu.be/IVbpPK_CXLY");
            pbdBeamReinforcement.SetContextualHelp(BeamReinforcementCHelp);
            panel_Reinforcement.AddItem(pbdBeamReinforcement);

            //Создание кнопки "SlabReinforcement" в категории "Армирование"
            PushButtonData pbdSlabReinforcement = new PushButtonData("SlabReinforcement"
                    , "Плиты"
                    , assemblyPach
                    , "CITRUS.CIT_04_4_SlabReinforcement.CIT_04_4_SlabReinforcement");

            Image SlabReinforcement_img1 = Properties.Resources.SlabReinforcement_Large;
            ImageSource SlabReinforcement_imgLarge = GetImageSourse(SlabReinforcement_img1);
            Image SlabReinforcement_img2 = Properties.Resources.SlabReinforcement_Small;
            ImageSource SlabReinforcement_imgStandart = GetImageSourse(SlabReinforcement_img2);

            pbdSlabReinforcement.LargeImage = SlabReinforcement_imgLarge;
            pbdSlabReinforcement.Image = SlabReinforcement_imgStandart;

            ContextualHelp SlabReinforcementCHelp = new ContextualHelp(ContextualHelpType.Url, "https://youtu.be/lO9uNZAcglQ");
            pbdSlabReinforcement.SetContextualHelp(SlabReinforcementCHelp);
            panel_Reinforcement.AddItem(pbdSlabReinforcement);

            // Создание кнопки "StaircaseReinforcement" в категории "Армирование"
            PushButtonData pbdStaircaseReinforcement = new PushButtonData("StairFlightReinforcement"
                    , "Марш"
                    , assemblyPach
                    , "CITRUS.CIT_04_5_StairFlightReinforcement.CIT_04_5_StairFlightReinforcement");

            Image StaircaseReinforcement_img1 = Properties.Resources.StaircaseReinforcement_Large;
            ImageSource StaircaseReinforcement_imgLarge = GetImageSourse(StaircaseReinforcement_img1);
            Image StaircaseReinforcement_img2 = Properties.Resources.StaircaseReinforcement;
            ImageSource StaircaseReinforcement_imgStandart = GetImageSourse(StaircaseReinforcement_img2);

            pbdStaircaseReinforcement.LargeImage = StaircaseReinforcement_imgLarge;
            pbdStaircaseReinforcement.Image = StaircaseReinforcement_imgStandart;

            ContextualHelp StaircaseReinforcementCHelp = new ContextualHelp(ContextualHelpType.Url, "https://youtu.be/lg1BtFYUIeU");
            pbdStaircaseReinforcement.SetContextualHelp(StaircaseReinforcementCHelp);
            panel_Reinforcement.AddItem(pbdStaircaseReinforcement);

            ////Создание категории кнопок "ОВ"
            //RibbonPanel panel_OV = application.CreateRibbonPanel(tabName, "ОВ");
            ////Создание кнопки "MEPViewScheduleCreator" в категории "ОВ"
            //PushButtonData pbdMEPViewScheduleCreator = new PushButtonData("MEPViewScheduleCreator", "Создать\ncпецификации MEP", assemblyPach, "CITRUS.MEPViewScheduleCreator");

            //Image MEPViewScheduleCreator_img1 = Properties.Resources.ScheduleCreator_Large;
            //ImageSource MEPViewScheduleCreator_imgLarge = GetImageSourse(MEPViewScheduleCreator_img1);
            //Image MEPViewScheduleCreator_img2 = Properties.Resources.ScheduleCreator;
            //ImageSource MEPViewScheduleCreator_imgStandart = GetImageSourse(MEPViewScheduleCreator_img2);
            //pbdMEPViewScheduleCreator.LargeImage = MEPViewScheduleCreator_imgLarge;
            //pbdMEPViewScheduleCreator.Image = MEPViewScheduleCreator_imgStandart;
            //panel_OV.AddItem(pbdMEPViewScheduleCreator);

            ////Создание кнопки "MEPViewScheduleHost" в категории "ОВ"
            //PushButtonData pbdMEPViewScheduleHost = new PushButtonData("MEPViewScheduleHost", "Разместить\nспецификации MEP", assemblyPach, "CITRUS.MEPViewScheduleHost");
            //Image MEPViewScheduleHost_img1 = Properties.Resources.ScheduleHost_Large;
            //ImageSource MEPViewScheduleHost_imgLarge = GetImageSourse(MEPViewScheduleHost_img1);
            //Image MEPViewScheduleHost_img2 = Properties.Resources.ScheduleHost;
            //ImageSource MEPViewScheduleHost_imgStandart = GetImageSourse(MEPViewScheduleHost_img2);
            //pbdMEPViewScheduleHost.LargeImage = MEPViewScheduleHost_imgLarge;
            //pbdMEPViewScheduleHost.Image = MEPViewScheduleHost_imgStandart;
            //panel_OV.AddItem(pbdMEPViewScheduleHost);

            //Создание категории кнопок "Обработка MEP"
            RibbonPanel panel_MEPMachining = application.CreateRibbonPanel(tabName, "Обработка MEP");

            //Создание кнопки "GloryHole" в категории "Обработка отверстий"
            PushButtonData pbdGloryHole = new PushButtonData("GloryHole", "Создать\nзадание", assemblyPach, "CITRUS.GloryHole");
            Image GloryHole_img1 = Properties.Resources.GloryHoleIcon_Large;
            ImageSource GloryHole_imgLarge = GetImageSourse(GloryHole_img1);
            Image GloryHole_img2 = Properties.Resources.GloryHoleIcon;
            ImageSource GloryHole_imgStandart = GetImageSourse(GloryHole_img2);

            pbdGloryHole.LargeImage = GloryHole_imgLarge;
            pbdGloryHole.Image = GloryHole_imgStandart;
            panel_MEPMachining.AddItem(pbdGloryHole);

            //Создание кнопки "GloryHoleRefreshElevations" в категории "Обработка отверстий"
            PushButtonData pbdGloryHoleRefreshElevations = new PushButtonData("GloryHoleRefreshElevations", "Обновить\nотметки", assemblyPach, "CITRUS.GloryHoleRefreshElevations");
            Image GloryHoleRefreshElevations_img1 = Properties.Resources.GloryHoleRefreshElevationsIcon_Large;
            ImageSource GloryHoleRefreshElevations_imgLarge = GetImageSourse(GloryHoleRefreshElevations_img1);
            Image GloryHoleRefreshElevations_img2 = Properties.Resources.GloryHoleRefreshElevationsIcon;
            ImageSource GloryHoleRefreshElevations_imgStandart = GetImageSourse(GloryHoleRefreshElevations_img2);

            pbdGloryHoleRefreshElevations.LargeImage = GloryHoleRefreshElevations_imgLarge;
            pbdGloryHoleRefreshElevations.Image = GloryHoleRefreshElevations_imgStandart;
            //panel_HoleMachining.AddItem(pbdGloryHoleRefreshElevations);

            //Создание кнопки "GloryHoleRefreshMark" в категории "Обработка отверстий"
            PushButtonData pbdGloryHoleRefreshMark = new PushButtonData("GloryHoleRefreshMark", "Обновить\nмарки", assemblyPach, "CITRUS.GloryHoleRefreshMark");
            Image GloryHoleRefreshMark_img1 = Properties.Resources.GloryHoleRefreshMarkIcon_Large;
            ImageSource GloryHoleRefreshMark_imgLarge = GetImageSourse(GloryHoleRefreshMark_img1);
            Image GloryHoleRefreshMark_img2 = Properties.Resources.GloryHoleRefreshMarkIcon;
            ImageSource GloryHoleRefreshMark_imgStandart = GetImageSourse(GloryHoleRefreshMark_img2);

            pbdGloryHoleRefreshMark.LargeImage = GloryHoleRefreshMark_imgLarge;
            pbdGloryHoleRefreshMark.Image = GloryHoleRefreshMark_imgStandart;
            //panel_HoleMachining.AddItem(pbdGloryHoleRefreshMark);

            SplitButtonData groupDataGloryHoleRefresh = new SplitButtonData("Обновить\nданные", "Pulldown Group");
            SplitButton groupGloryHoleRefresh = panel_MEPMachining.AddItem(groupDataGloryHoleRefresh) as SplitButton;
            PushButton pbGloryHoleRefreshElevations = groupGloryHoleRefresh.AddPushButton(pbdGloryHoleRefreshElevations) as PushButton;
            groupGloryHoleRefresh.AddSeparator();
            PushButton pbGloryHoleRefreshMark = groupGloryHoleRefresh.AddPushButton(pbdGloryHoleRefreshMark) as PushButton;

            //Создание кнопки "GloryHoleSaveAssignmentVersion" в категории "Обработка отверстий"
            PushButtonData pbdGloryHoleSaveAssignmentVersion = new PushButtonData("GloryHoleSaveAssignmentVersion", "Сохранить\nверсию задания", assemblyPach, "CITRUS.GloryHoleSaveAssignmentVersion");
            Image GloryHoleSaveAssignmentVersion_img1 = Properties.Resources.GloryHoleSaveAssignmentVersionIcon_Large;
            ImageSource GloryHoleSaveAssignmentVersion_imgLarge = GetImageSourse(GloryHoleSaveAssignmentVersion_img1);
            Image GloryHoleSaveAssignmentVersion_img2 = Properties.Resources.GloryHoleSaveAssignmentVersionIcon;
            ImageSource GloryHoleSaveAssignmentVersion_imgStandart = GetImageSourse(GloryHoleSaveAssignmentVersion_img2);

            pbdGloryHoleSaveAssignmentVersion.LargeImage = GloryHoleSaveAssignmentVersion_imgLarge;
            pbdGloryHoleSaveAssignmentVersion.Image = GloryHoleSaveAssignmentVersion_imgStandart;
            panel_MEPMachining.AddItem(pbdGloryHoleSaveAssignmentVersion);

            //Создание кнопки "GloryHoleCutter" в категории "Обработка отверстий"
            PushButtonData pbdGloryHoleCutter = new PushButtonData("GloryHoleCutter", "Вырезать\nотверстия", assemblyPach, "CITRUS.GloryHoleCutter");
            Image GloryHoleCutter_img1 = Properties.Resources.GloryHoleCutterIcon_Large;
            ImageSource GloryHoleCutter_imgLarge = GetImageSourse(GloryHoleCutter_img1);
            Image GloryHoleCutter_img2 = Properties.Resources.GloryHoleCutterIcon;
            ImageSource GloryHoleCutter_imgStandart = GetImageSourse(GloryHoleCutter_img2);

            pbdGloryHoleCutter.LargeImage = GloryHoleCutter_imgLarge;
            pbdGloryHoleCutter.Image = GloryHoleCutter_imgStandart;
            panel_MEPMachining.AddItem(pbdGloryHoleCutter);
            panel_MEPMachining.AddSeparator();

            //Создание кнопки "SystemsToSpace" в категории "Обработка отверстий"
            PushButtonData pbdSystemsToSpace = new PushButtonData("SystemsToSpace", "Системы\nв пространства", assemblyPach, "CITRUS.SystemsToSpace");
            Image SystemsToSpace_img1 = Properties.Resources.SystemsToSpaceIcon_Large;
            ImageSource SystemsToSpace_imgLarge = GetImageSourse(SystemsToSpace_img1);
            Image SystemsToSpace_img2 = Properties.Resources.SystemsToSpaceIcon;
            ImageSource SystemsToSpace_imgStandart = GetImageSourse(SystemsToSpace_img2);

            pbdSystemsToSpace.LargeImage = SystemsToSpace_imgLarge;
            pbdSystemsToSpace.Image = SystemsToSpace_imgStandart;
            panel_MEPMachining.AddItem(pbdSystemsToSpace);

            ////Создание кнопки "RefreshDuctFittings" в категории "Обработка отверстий"
            //PushButtonData pbdRefreshDuctFittings = new PushButtonData("RefreshDuctFittings", "Обновить\nфитинги", assemblyPach, "CITRUS.RefreshDuctFittings");
            //Image RefreshDuctFittings_img1 = Properties.Resources.RefreshDuctFittings_Large;
            //ImageSource RefreshDuctFittings_imgLarge = GetImageSourse(RefreshDuctFittings_img1);
            //Image RefreshDuctFittings_img2 = Properties.Resources.RefreshDuctFittings;
            //ImageSource RefreshDuctFittings_imgStandart = GetImageSourse(RefreshDuctFittings_img2);

            //pbdRefreshDuctFittings.LargeImage = RefreshDuctFittings_imgLarge;
            //pbdRefreshDuctFittings.Image = RefreshDuctFittings_imgStandart;
            //panel_MEPMachining.AddItem(pbdRefreshDuctFittings);

            //Создание категории кнопок "Другое"
            RibbonPanel panel_Other = application.CreateRibbonPanel(tabName, "Другое");

            //SharedParametersBatchAdding
            PushButtonData pbdSharedParametersBatchAdding = new PushButtonData("SharedParametersBatchAdding"
                   , "Кулёк\nпараметров"
                   , assemblyPach
                   , "CITRUS.SharedParametersBatchAdding");

            Image SharedParametersBatchAdding_img1 = Properties.Resources.SharedParametersBatchAddingIcon_Large;
            ImageSource SharedParametersBatchAdding_imgLarge = GetImageSourse(SharedParametersBatchAdding_img1);
            Image SharedParametersBatchAdding_img2 = Properties.Resources.SharedParametersBatchAddingIcon;
            ImageSource SharedParametersBatchAdding_imgStandart = GetImageSourse(SharedParametersBatchAdding_img2);

            pbdSharedParametersBatchAdding.LargeImage = SharedParametersBatchAdding_imgLarge;
            pbdSharedParametersBatchAdding.Image = SharedParametersBatchAdding_imgStandart;
            panel_Other.AddItem(pbdSharedParametersBatchAdding);

            ////Создание кнопки "VR" в категории "Другое"
            //PushButtonData pbdVoiceRecognition = new PushButtonData("VoiceRecognition", "VR", assemblyPach, "CITRUS.VoiceRecognition.VoiceRecognition");
            //Image VoiceRecognition_img1 = Properties.Resources.VoiceRecognition_Large;
            //ImageSource VoiceRecognition_imgLarge = GetImageSourse(VoiceRecognition_img1);
            //Image VoiceRecognition_img2 = Properties.Resources.VoiceRecognition_Small;
            //ImageSource VoiceRecognition_imgStandart = GetImageSourse(VoiceRecognition_img2);

            //pbdVoiceRecognition.LargeImage = VoiceRecognition_imgLarge;
            //pbdVoiceRecognition.Image = VoiceRecognition_imgStandart;
            //panel_Other.AddItem(pbdVoiceRecognition);

            //////Создание кнопки "MEPViewScheduleCreatorADM" в категории "Другое"
            ////PushButtonData pbdMEPViewScheduleCreatorADM = new PushButtonData("MEPViewScheduleCreatorADM"
            ////        , "Создать\ncпецификации MEPADM"
            ////        , assemblyPach
            ////        , "CITRUS.MEPViewScheduleCreatorADM");

            ////panel_Other.AddItem(pbdMEPViewScheduleCreatorADM);

            ////// Создание кнопки "HeatLoss" в категории "Другое"
            ////PushButtonData pbdHeatLoss = new PushButtonData("HeatLoss"
            ////        , "Теплопотери"
            ////        , assemblyPach
            ////        , "CITRUS.HeatLoss");

            ////panel_Other.AddItem(pbdHeatLoss);

            ////// Создание кнопки "MySQL_Test" в категории "Другое"
            ////PushButtonData pbdMySQLTest = new PushButtonData("MySQLTest"
            ////        , "MySQLTest"
            ////        , assemblyPach
            ////        , "CITRUS.MySQL_Test");

            ////panel_Other.AddItem(pbdMySQLTest);

            ////  Создание кнопки "MEPViewScheduleCreator_Roven" в категории "Другое"
            //PushButtonData pbdMEPViewScheduleCreator_Roven = new PushButtonData("MEPViewScheduleCreator_Roven"
            //        , "РОВЕН\nСоздать спецификации"
            //        , assemblyPach
            //        , "CITRUS.CIT_05_4_1_MEPViewScheduleCreator_Roven.CIT_05_4_1_MEPViewScheduleCreator_Roven");

            //panel_Other.AddItem(pbdMEPViewScheduleCreator_Roven);

            ////  Создание кнопки "MEPViewScheduleCleaner_Roven" в категории "Другое"
            //PushButtonData pbdMEPViewScheduleCleaner_Roven = new PushButtonData("MEPViewScheduleCleaner_Roven"
            //        , "РОВЕН\nУдалить спецификации"
            //        , assemblyPach
            //        , "CITRUS.CIT_05_4_1_MEPViewScheduleCreator_Roven.CIT_05_4_1_MEPViewScheduleCleaner_Roven");

            //panel_Other.AddItem(pbdMEPViewScheduleCleaner_Roven);

            //Создание кнопки "Axis3D2D" в категории "Другое"
            //PushButtonData pbdAxis3D2D = new PushButtonData("Axis3D2D"
            //        , "Оси\n3D в 2D"
            //        , assemblyPach
            //        , "CITRUS.Axis3D2D");

            //panel_Other.AddItem(pbdAxis3D2D);

            //Создание кнопки "Google" в категории "Другое"
            //PushButtonData pbdGoogle = new PushButtonData("Google"
            //        , "Google"
            //        , assemblyPach
            //        , "CITRUS.CIT_00_0_GoogleSheets.RevitDataToGoogleSheets");

            //panel_Other.AddItem(pbdGoogle);

            ////Создание кнопки "FinishingSheet" в категории "Другое"
            //PushButtonData pbdGFinishingSheet = new PushButtonData("FinishingSheet"
            //        , "Отделка"
            //        , assemblyPach
            //        , "CITRUS.FinishingSheet");

            //panel_Other.AddItem(pbdGFinishingSheet);

            ////Создание кнопки "WallsReinforcement" в категории "Другое"
            //PushButtonData pbdWallsReinforcement = new PushButtonData("WallsReinforcement"
            //        , "Армирование\nстены"
            //        , assemblyPach
            //        , "CITRUS.WallsReinforcement");

            //panel_Other.AddItem(pbdWallsReinforcement);

            //Создание кнопки "FillingParameterLevel" в категории "Другое"
            //PushButtonData pbdFillingParameterLevel = new PushButtonData("FillingParameterLevel"
            //        , "Заполнить\n\"Этаж\""
            //        , assemblyPach
            //        , "CITRUS.CIT_00_1_FillingParameterLevel.CIT_00_1_FillingParameterLevel");

            //panel_Other.AddItem(pbdFillingParameterLevel);

            //CreateColumnDimensions
            //Создание кнопки "CreateLineDimensions" в категории "Другое"
            PushButtonData pbdCreateLineDimensions = new PushButtonData("CreateLineDimensions"
                   , "Размер\nпо линии"
                    , assemblyPach
                   , "CITRUS.CreateLineDimensions");

            panel_Other.AddItem(pbdCreateLineDimensions);

            //ColumnCutter
            //PushButtonData pbdColumnCutter = new PushButtonData("ColumnCutter"
            //        , "Резчик\nКолонн"
            //        , assemblyPach
            //        , "CITRUS.ColumnCutter");

            //panel_Other.AddItem(pbdColumnCutter);

            //ApartmentLayout
            PushButtonData pbdApartmentLayout = new PushButtonData("ApartmentLayout"
                    , "Квартирография"
                    , assemblyPach
                    , "CITRUS.ApartmentLayout");

            panel_Other.AddItem(pbdApartmentLayout);

            //CopyWallOpeningsToWallFinish
            PushButtonData pbdCopyWallOpeningsToWallFinish = new PushButtonData("CopyWallOpeningsToWallFinish"
                , "Копирование\nпроемов"
                , assemblyPach
                , "CITRUS.CopyWallOpeningsToWallFinish");

            panel_Other.AddItem(pbdCopyWallOpeningsToWallFinish);

            return Result.Succeeded;
            }
            // Конвертер изображения
            private BitmapSource GetImageSourse(Image img)
            {
                BitmapImage bmp = new BitmapImage();
                using (MemoryStream ms = new MemoryStream())
                {
                    img.Save(ms, ImageFormat.Png);
                    ms.Position = 0;

                    bmp.BeginInit();

                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.UriSource = null;
                    bmp.StreamSource = ms;

                    bmp.EndInit();
                }
                return bmp;
            }
    }
}
