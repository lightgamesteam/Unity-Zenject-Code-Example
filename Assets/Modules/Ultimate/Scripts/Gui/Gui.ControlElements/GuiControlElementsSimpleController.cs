using Gui.Core;

namespace TDL.Modules.Ultimate.GuiControlElements {
    public class GuiControlElementsSimpleController : GuiViewControllerBase<GuiControlElementsSimpleView> {
        public MonoControlElementsPanelApplicationController PanelApplication => View.Content.View.PanelApplication;
        public MonoControlElementsPanelSceneController PanelScene => View.Content.View.PanelScene;
        public MonoControlElementsPanelAdditionalController PanelAdditional => View.Content.View.PanelAdditional;
        public MonoControlElementsPanelInformationalController PanelInformational => View.Content.View.PanelInformational;
    }
}